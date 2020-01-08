using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTiles;
using LevelSetting;
public class LevelChunkEditor : LevelChunk
{
    public static LevelChunkEditor Instance { get; private set; }
    Transform tf_CameraPos;
    public enum_TileSubType m_EditMode { get; private set; } = enum_TileSubType.Ground;
    public bool m_ShowAllModel { get; private set; }
    public LevelTileEditorData[,] m_TilesData { get; private set; }
    ObjectPoolSimpleComponent<int, LevelTileEditorSelection> m_SelectionTiles;
    LevelTileEditorSelection m_SelectingTile;
    enum_TileDirection m_SelectingDirection = enum_TileDirection.Top;
    List<LevelTileEditorData> temp_RelativeTiles = new List<LevelTileEditorData>();
    Dictionary<enum_TileObjectType, int> m_ItemRestriction;
    private void Awake()
    {
        Instance = this;
        tf_CameraPos = transform.Find("CameraPos");
        m_SelectionTiles = new ObjectPoolSimpleComponent<int, LevelTileEditorSelection>(transform.Find("SelectionPool"), "SelectionItem");
        m_SelectingTile = transform.Find("SelectingTile").GetComponent<LevelTileEditorSelection>();
    }
    private void Start()
    {
        TPSCameraController.Instance.Attach(tf_CameraPos, true, true);
    }

    public override void Init(LevelChunkData _data)
    {
        m_TilesData = new LevelTileEditorData[_data.Width, _data.Height];
        base.Init(_data);

        tf_CameraPos.transform.localPosition = new Vector3(m_Width / 2f * LevelConst.I_TileSize, 0, 0);
        TPSCameraController.Instance.SetCameraRotation(60, 0);

        m_ItemRestriction = LevelExpressions.GetChunkRestriction(m_ChunkType); 
        m_EditMode = enum_TileSubType.Ground;
        CheckEditMode();
    }
    protected override bool WillGenerateTile(ChunkTileData data) => true;
    protected override void OnTileInit(LevelTileNew tile, TileAxis axis, ChunkTileData data)
    {
        base.OnTileInit(tile, axis, data);
        m_TilesData[axis.X,axis.Y]=(tile as LevelTileEditorData);
    }

    public void Resize(int sizeX,int sizeY)
    {
        Init(LevelChunkData.NewData(sizeX, sizeY, m_ChunkType, m_TilesData));
    }

    private void Update()
    {
        MoveCameraCheck();
        ChunkEditCheck();
    }

    void MoveCameraCheck()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
            return;

        tf_CameraPos.Translate((CameraController.CameraXZForward * Input.GetAxis("Vertical") + CameraController.CameraXZRightward * Input.GetAxis("Horizontal")).normalized * Time.deltaTime * 40f + Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 500f * Vector3.up);
        CameraController.Instance.RotateCamera(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * 50f);
    }

    RaycastHit raycastHit;
    void ChunkEditCheck()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_ShowAllModel = false;
            m_EditMode = m_EditMode == enum_TileSubType.Invalid ? enum_TileSubType.Ground : m_EditMode + 1;
            if (m_EditMode > enum_TileSubType.Pillar)
                m_EditMode = enum_TileSubType.Object;
            CheckEditMode();
        }

        if (Input.GetKeyDown(KeyCode.CapsLock))
            EnterViewMode();

        MouseRayChunkEdit();
    }
    #region EditMode
    void EnterViewMode()
    {
        m_EditMode = enum_TileSubType.Invalid;
        m_ShowAllModel = true;
        CheckEditMode();
    }

    void CheckEditMode()
    {
        ChangeEditSelection(null);
        m_SelectionTiles.ClearPool();
        m_SelectingTile.Init(new TileAxis(-6,-1),ChunkTileData.Default());
        int index = 0;
        switch(m_EditMode)
        {
            default:break;
            case enum_TileSubType.Ground:
                TCommon.TraversalEnum((enum_TileGroundType type)=> {
                    LevelTileEditor editorTile = m_SelectionTiles.AddItem((int)type);
                    editorTile.Init(new TileAxis(index, 0), ChunkTileData.Create(enum_TilePillarType.Invalid, type, enum_TileObjectType.Invalid, enum_TileDirection.Top));
                    index += 1;
                    if (index > 20)
                        index = 0;
                });
                break;
            case enum_TileSubType.Object:
                int yOffset = 0;
                TCommon.TraversalEnum((enum_TileObjectType type) => {
                    if (!ObjectRegisted(type))
                        return;
                    m_SelectionTiles.AddItem((int)type).Init(new TileAxis(index, yOffset), ChunkTileData.Create(enum_TilePillarType.Invalid, enum_TileGroundType.Invalid, type, enum_TileDirection.Top));
                    index += type.GetSizeAxis(enum_TileDirection.Top).X;
                    if (index > 20)
                    {
                        index = 0;
                        yOffset -= 3;
                    }
                });
                break;
            case enum_TileSubType.Pillar:
                TCommon.TraversalEnum((enum_TilePillarType type) => {
                    m_SelectionTiles.AddItem((int)type).Init(new TileAxis(index ++, 0), ChunkTileData.Create(type, enum_TileGroundType.Invalid,   enum_TileObjectType.Invalid, enum_TileDirection.Top));
                });
                break;
        }
        m_SelectionTiles.m_ActiveItemDic.Traversal((LevelTileEditorSelection tile) => { tile.OnEditSelectionChange(); });
        m_TilesData.Traversal((LevelTileEditorData tile) => { tile.OnEditSelectionChange(); });
    }

    bool ObjectRegisted(enum_TileObjectType type) => (type < enum_TileObjectType.RestrictStart || type > enum_TileObjectType.RestrictEnd) || m_ItemRestriction.ContainsKey(type);
    
    bool ObjectRestrictRunsOut(enum_TileObjectType type)
    {
        if (type < enum_TileObjectType.RestrictStart || type > enum_TileObjectType.RestrictEnd)
            return false;
        
        m_ItemRestriction = LevelExpressions.GetChunkRestriction(m_ChunkType);
        m_TilesData.Traversal((LevelTileEditorData data) => {
            if (m_ItemRestriction.ContainsKey(data.m_Data.m_ObjectType))
                m_ItemRestriction[data.m_Data.m_ObjectType] -= 1;
        });
        if (m_ItemRestriction[type] == 0)
            return true;
        return false;
    }

    #endregion
    #region Edit
    void MouseRayChunkEdit()
    {
        if (m_ShowAllModel)
            return;

        if (Input.GetMouseButton(0))
        {
            if (!CameraController.Instance.InputRayCheck(Input.mousePosition, GameSetting.GameLayer.Mask.I_Interact, ref raycastHit))
                return;
            LevelTileEditor editorTile = raycastHit.transform.GetComponent<LevelTileEditor>();

            if (!editorTile.isDataTile)
            {
                ChangeEditSelection(editorTile);
                return;
            }

            if(CanEditTile(editorTile))
                EditDataTile(editorTile);
        }
        else if (Input.GetMouseButton(1))
        {
            if (!CameraController.Instance.InputRayCheck(Input.mousePosition, GameSetting.GameLayer.Mask.I_Interact, ref raycastHit))
                return;
            RemoveDataTile(raycastHit.transform.GetComponent<LevelTileEditor>());
        }
        else if(Input.GetKeyDown(KeyCode.Mouse2))
        {
            m_SelectingDirection = m_SelectingDirection.Next();
            m_SelectingTile.RotateDirection(m_SelectingDirection);
            if (CameraController.Instance.InputRayCheck(Input.mousePosition, GameSetting.GameLayer.Mask.I_Interact, ref raycastHit))
                RotateDataTile(raycastHit.transform.GetComponent<LevelTileEditor>());
        }
    }
    bool CanEditTile(LevelTileEditor targetTile)
    {
        if (!targetTile.isDataTile)
            return false;

        bool editable = true;

        switch (m_EditMode)
        {
            case enum_TileSubType.Object:
                {
                    if (m_SelectingTile.m_Data.m_ObjectType == enum_TileObjectType.Invalid)
                        return false;

                    if (!m_TilesData.Get(targetTile.m_Axis,(m_SelectingTile.m_Data.m_ObjectType).GetSizeAxis(m_SelectingDirection), ref temp_RelativeTiles))
                        return false;

                    temp_RelativeTiles.TraversalBreak((LevelTileEditorData tile) =>
                    {
                        editable =tile.m_Axis==targetTile.m_Axis||ObjectEditable(tile, m_SelectingTile.m_Data.m_ObjectType);
                        return !editable;
                    });
                }
                break;
            case enum_TileSubType.Ground:
                if (m_SelectingTile.m_Data.m_GroundType== enum_TileGroundType.Invalid)
                    return false;
                editable = !m_TilesData.CheckIsEdge(targetTile.m_Axis);
                break;
            case enum_TileSubType.Pillar:
                if (m_SelectingTile.m_Data.m_PillarType== enum_TilePillarType.Invalid)
                    return false;
                editable = !m_TilesData.CheckIsEdge(targetTile.m_Axis);
                break;
        }
        return editable;
    }

    bool ObjectEditable(LevelTileEditor tile,enum_TileObjectType objectType)
    {
        if (objectType == enum_TileObjectType.Connection1x5)
            return m_TilesData.CheckIsEdge(tile.m_Axis);

        if (tile.m_Data.m_ObjectType != enum_TileObjectType.Invalid)
            return false;

        switch (tile.m_Data.m_GroundType)
        {
            default:
                return !m_TilesData.CheckIsEdge(tile.m_Axis);
            case enum_TileGroundType.Invalid:
            case enum_TileGroundType.Road1:
            case enum_TileGroundType.Road2:
            case enum_TileGroundType.Road3:
            case enum_TileGroundType.Road4:
            case enum_TileGroundType.Bridge:
                return false;
        }
    }

    void EditDataTile(LevelTileEditor targetTile)
    {
        if (!targetTile.isDataTile)
            return;
        LevelTileEditorData data = targetTile as LevelTileEditorData;
        switch (m_EditMode)
        {
            case enum_TileSubType.Ground:
                data.SetData(m_SelectingTile.m_Data.m_GroundType, m_SelectingDirection);
                break;
            case enum_TileSubType.Object:
                {
                    if (ObjectRestrictRunsOut(m_SelectingTile.m_Data.m_ObjectType))
                        return;
                    data.SetData(m_SelectingTile.m_Data.m_ObjectType, m_SelectingDirection);
                }
                break;
            case enum_TileSubType.Pillar:
                data.SetData(m_SelectingTile.m_Data.m_PillarType, m_SelectingDirection);
                if (data.m_Data.m_GroundType == enum_TileGroundType.Invalid)
                    data.SetData(enum_TileGroundType.Main, enum_TileDirection.Top);
                break;
        }
    }

    void RemoveDataTile(LevelTileEditor targetTile)
    {
        if(!targetTile.isDataTile)
            return;
        LevelTileEditorData data = targetTile as LevelTileEditorData;
        switch (m_EditMode)
        {
            case enum_TileSubType.Ground:
                data.SetData(enum_TileGroundType.Invalid, enum_TileDirection.Top);
                break;
            case enum_TileSubType.Object:
                data.SetData(enum_TileObjectType.Invalid, enum_TileDirection.Top);
                break;
            case enum_TileSubType.Pillar:
                data.SetData(enum_TilePillarType.Invalid, enum_TileDirection.Top);
                break;
        }
    }

    void RotateDataTile(LevelTileEditor targetTile)
    {
        if (!targetTile.isDataTile)
            return;

        LevelTileEditorData data = targetTile as LevelTileEditorData;
        ChangeEditSelection(data);
        if (CanEditTile(data))
            EditDataTile(data);
    }

    void ChangeEditSelection(LevelTileEditor editorTile )
    {
        ChunkTileData editData = ChunkTileData.Default();
        if (editorTile != null)
        {
            switch (m_EditMode)
            {
                case enum_TileSubType.Ground:
                    {
                        if (editorTile.m_Data.m_GroundType == enum_TileGroundType.Invalid)
                            return;
                        editData.ChangeGroundType(editorTile.m_Data.m_GroundType, m_SelectingDirection);
                    }
                    break;
                case enum_TileSubType.Object:
                    {
                        if (editorTile.m_Data.m_ObjectType == enum_TileObjectType.Invalid)
                            return;
                        editData.ChangeObjectType(editorTile.m_Data.m_ObjectType, m_SelectingDirection);
                    }
                    break;
                case enum_TileSubType.Pillar:
                    {
                        if (editorTile.m_Data.m_PillarType == enum_TilePillarType.Invalid)
                            return;
                        editData.ChangePillarType(editorTile.m_Data.m_PillarType, m_SelectingDirection);
                    }
                    break;
            }
        }
        m_SelectingTile.Init(new TileAxis(-6, -1), editData);
        m_SelectionTiles.m_ActiveItemDic.Traversal((int index, LevelTileEditorSelection selection) =>{ selection.SetSelecting(selection.m_Data==editData);});
    }
    #endregion
}
