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
    int m_currentEditSelection;
    List<LevelTileEditorData> temp_RelativeTiles = new List<LevelTileEditorData>();
    private void Awake()
    {
        Instance = this;
        tf_CameraPos = transform.Find("CameraPos");
        m_SelectionTiles = new ObjectPoolSimpleComponent<int, LevelTileEditorSelection>(transform.Find("SelectionPool"), "SelectionItem");
        m_SelectingTile = transform.Find("SelectingTile").GetComponent<LevelTileEditorSelection>();
    }

    public override void Init(LevelChunkData _data)
    {
        m_TilesData = new LevelTileEditorData[_data.Width, _data.Height];
        base.Init(_data);
        TPSCameraController.Instance.Attach(tf_CameraPos,true,true);
        tf_CameraPos.transform.localPosition = new Vector3(m_Width / 2f * LevelConst.I_TileSize, 0, 0);
        TPSCameraController.Instance.SetCameraRotation(60,0);
        m_EditMode = enum_TileSubType.Ground;
        OnEditModeChange();
    }
    protected override bool WillGenerateTile(LevelTileData data) => true;
    protected override void OnTileInit(LevelTileNew tile, TileAxis axis, LevelTileData data)
    {
        base.OnTileInit(tile, axis, data);
        m_TilesData[axis.X,axis.Y]=(tile as LevelTileEditorData);
    }
    

    public void Resize(int sizeX,int sizeY)
    {
        Init(LevelChunkData.NewData(sizeX, sizeY, m_TilesData));
    }
    public void Desize()
    {
        int newSizeX = 1;
        int newSizeY = 1;
        m_TilesData.Traversal((LevelTileEditorData tile) =>
        {
            if(tile.m_ContainsInfo)
            {
                if (newSizeX < tile.m_Axis.X+1)
                    newSizeX = tile.m_Axis.X+1;
                if (newSizeY < tile.m_Axis.Y+1)
                    newSizeY = tile.m_Axis.Y+1;
            }
        });
        Init(LevelChunkData.NewData(newSizeX, newSizeY, m_TilesData));
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
            OnEditModeChange();
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
        OnEditModeChange();
    }

    void OnEditModeChange()
    {
        ChangeEditSelection(null);
        m_SelectionTiles.ClearPool();
        m_SelectingTile.Init(new TileAxis(-4,0),LevelTileData.Default());
        int index = 0;
        switch(m_EditMode)
        {
            default:break;
            case enum_TileSubType.Ground:
                ObjectPoolManager<enum_TileGroundType, TileGroundBase>.GetRegistedList().Traversal((enum_TileGroundType type)=> {
                    LevelTileEditor editorTile = m_SelectionTiles.AddItem((int)type);
                    editorTile.Init(new TileAxis(index, 0), LevelTileData.Create(enum_TilePillarType.Invalid, type, enum_TileObjectType.Invalid, enum_TileDirection.Top));
                    index += 1;
                    if (index > 20)
                        index = 0;
                });
                break;
            case enum_TileSubType.Object:
                int yOffset = 0;
                ObjectPoolManager<enum_TileObjectType, TileObjectBase>.GetRegistedList().Traversal((enum_TileObjectType type) => {
                    m_SelectionTiles.AddItem((int)type).Init(new TileAxis(index, yOffset), LevelTileData.Create(enum_TilePillarType.Invalid,  enum_TileGroundType.Invalid, type, enum_TileDirection.Top));
                    index += type.GetSizeAxis(enum_TileDirection.Top).X;
                    if (index > 20)
                    {
                        index = 0;
                        yOffset -= 3;
                    }
                });
                break;
            case enum_TileSubType.Pillar:
                ObjectPoolManager<enum_TilePillarType, TilePillarBase>.GetRegistedList().Traversal((enum_TilePillarType type) => {
                    m_SelectionTiles.AddItem((int)type).Init(new TileAxis(index ++, 0), LevelTileData.Create(type, enum_TileGroundType.Invalid,   enum_TileObjectType.Invalid, enum_TileDirection.Top));
                });
                break;
        }
        m_SelectionTiles.m_ActiveItemDic.Traversal((LevelTileEditorSelection tile) => { tile.OnEditSelectionChange(); });
        m_TilesData.Traversal((LevelTileEditorData tile) => { tile.OnEditSelectionChange(); });
    }
    #endregion
    #region Edit
    void MouseRayChunkEdit()
    {
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

            if (m_currentEditSelection <= 0)
                return;
            if(CanAddTile(editorTile))
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
            if (CameraController.Instance.InputRayCheck(Input.mousePosition, GameSetting.GameLayer.Mask.I_Interact, ref raycastHit))
            {
                RotateDataTile(raycastHit.transform.GetComponent<LevelTileEditor>());
                return;
            }
            m_SelectingDirection = m_SelectingDirection.Next();
            m_SelectingTile.RotateDirection(m_SelectingDirection);
        }
    }
    bool CanAddTile(LevelTileEditor targetTile)
    {
        if (!targetTile.isDataTile)
            return false;

        bool editable = true;

        switch (m_EditMode)
        {
            case enum_TileSubType.Object:
                {
                    if (!m_TilesData.Get(targetTile.m_Axis,(( enum_TileObjectType)m_currentEditSelection).GetSizeAxis(m_SelectingDirection), ref temp_RelativeTiles))
                        return false;
                    enum_TileObjectType objectType = (enum_TileObjectType)m_currentEditSelection;
                    temp_RelativeTiles.TraversalBreak((LevelTileEditorData tile) =>
                    {
                        editable = EditorTileEditable(tile,objectType);
                        return !editable;
                    });
                }
                break;
        }
        return editable;
    }

    bool EditorTileEditable(LevelTileEditor tile,enum_TileObjectType objectType)
    {
        if (objectType == enum_TileObjectType.Connection1x5)
            return m_TilesData.CheckIsEdge(tile.m_Axis);

        switch (tile.m_Data.m_GroundType)
        {
            default:
                return true;
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
                data.SetData((enum_TileGroundType)m_currentEditSelection,m_SelectingDirection);
                break;
            case enum_TileSubType.Object:
                {
                    data.SetData((enum_TileObjectType)m_currentEditSelection, m_SelectingDirection);
                    m_TilesData.Get(targetTile.m_Axis, ((enum_TileObjectType)m_currentEditSelection).GetSizeAxis(m_SelectingDirection), ref temp_RelativeTiles);
                    temp_RelativeTiles.Traversal((LevelTileEditorData relativeTile) =>
                    {
                        if (relativeTile.m_Data.m_GroundType == enum_TileGroundType.Invalid)
                        {
                            LevelTileEditorData relativeData = relativeTile as LevelTileEditorData;
                            relativeData.SetData(enum_TileGroundType.Main, enum_TileDirection.Top);
                        }
                    });
                }
                break;
            case enum_TileSubType.Pillar:
                data.SetData((enum_TilePillarType)m_currentEditSelection, m_SelectingDirection);
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

    void RotateDataTile(LevelTileEditor editorTile)
    {
        if (!editorTile.isDataTile)
            return;

        editorTile.RotateDirection(editorTile.m_Data.m_Direction.Next());
    }

    void ChangeEditSelection(LevelTileEditor editorTile )
    {
        int editSelection = -1;
        if(editorTile)
        {
            m_SelectingTile.Init(new TileAxis(-4, 0), editorTile.m_Data);
            switch (m_EditMode)
            {
                case enum_TileSubType.Ground: editSelection = (int)editorTile.m_Data.m_GroundType;break;
                case enum_TileSubType.Object: editSelection = (int)editorTile.m_Data.m_ObjectType; break;
                case enum_TileSubType.Pillar: editSelection = (int)editorTile.m_Data.m_PillarType; break;
            }
        }

        if (m_currentEditSelection == editSelection)
            return;

        m_currentEditSelection = editSelection;
        m_SelectingDirection = enum_TileDirection.Top;
        m_SelectionTiles.m_ActiveItemDic.Traversal((int index, LevelTileEditorSelection selection) =>
        {
            selection.SetSelecting(m_currentEditSelection == index);
        });
    }
    #endregion
}
