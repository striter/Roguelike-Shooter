using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTiles;
using LevelSetting;
using GameSetting;

public class LevelChunkEditor : LevelChunkBase
{
    public static LevelChunkEditor Instance { get; private set; }
    public Transform tf_CameraPos { get; private set; }
    public enum_TileSubType m_EditMode { get; private set; } = enum_TileSubType.Ground;
    public enum_LevelStyle m_EditStyle { get; private set; } =  enum_LevelStyle.Invalid;
    public bool m_GameViewMode { get; private set; }
    enum_LevelStyle m_ViewStyle;
    public LevelTileEditorData[,] m_TilesData { get; private set; }
    ObjectPoolListComponent<int, LevelTileEditorSelection> m_SelectionTiles;
    LevelTileEditorSelection m_SelectingTile;
    enum_TileDirection m_SelectingDirection = enum_TileDirection.Top;
    List<LevelTileEditorData> temp_RelativeTiles = new List<LevelTileEditorData>();
    Dictionary<enum_TileObjectType, int> m_ItemRestriction;
    System.Random m_Random = new System.Random();
    Light m_directionalLight;
    private void Awake()
    {
        Instance = this;
        tf_CameraPos = transform.Find("CameraPos");
        m_SelectionTiles = new ObjectPoolListComponent<int, LevelTileEditorSelection>(transform.Find("SelectionPool"), "SelectionItem");
        m_SelectingTile = transform.Find("SelectingTile").GetComponent<LevelTileEditorSelection>();
        m_directionalLight = transform.Find("Directional Light").GetComponent<Light>();
        Init();
    }
    private void Start()
    {
        TPSCameraController.Instance.Attach(tf_CameraPos, true, true);
        LevelObjectManager.Register( TResources.GetChunkEditorTiles());
    }


    public void Init(LevelChunkData _data)
    {
        m_TilesData = new LevelTileEditorData[_data.Width, _data.Height];
        InitData(_data, m_Random);

        tf_CameraPos.transform.localPosition = new Vector3(m_Width / 2f * LevelConst.I_TileSize, 0, 0);
        TPSCameraController.Instance.SetCameraPosition(tf_CameraPos.transform.localPosition);
        TPSCameraController.Instance.SetCameraRotation( 60, 0);
        m_ItemRestriction = LevelExpressions.GetChunkRestriction(m_ChunkType); 
        m_EditMode = enum_TileSubType.Ground;
        m_GameViewMode = false;
        CheckEditMode();
    }

    protected override bool WillGenerateTile(ChunkTileData data) => true;
    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data,System.Random random)
    {
        base.OnTileInit(tile, axis, data,random);
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
            m_GameViewMode = false;
            m_EditMode = m_EditMode == enum_TileSubType.Invalid ? enum_TileSubType.Ground : m_EditMode + 1;
            if (m_EditMode > enum_TileSubType.Ground)
                m_EditMode = enum_TileSubType.Object;
            CheckEditMode();
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            m_EditMode = enum_TileSubType.Invalid;
            if (!m_GameViewMode)
                m_ViewStyle = TCommon.RandomEnumValues<enum_LevelStyle>();
            else
            {
                m_ViewStyle++;
                if (m_ViewStyle > enum_LevelStyle.Undead)
                    m_ViewStyle = enum_LevelStyle.Forest;
            }
            m_GameViewMode = true;
            CheckEditMode();
        }

        MouseRayChunkEdit();
    }
    #region EditMode
    void CheckEditMode()
    {
        enum_LevelStyle targetStyle = m_GameViewMode ? m_ViewStyle : enum_LevelStyle.Invalid;
        GameRenderData randomData = GameRenderData.Default();
        if (targetStyle != m_EditStyle)
        {
            m_TilesData.Traversal((LevelTileEditorData tile) => { tile.Clear(); });
            m_SelectingTile.Clear();
            m_SelectionTiles.m_ActiveItemDic.Traversal((LevelTileEditorSelection tile) => { tile.Clear();  });
            m_EditStyle = targetStyle;
            LevelObjectManager.Register(m_EditStyle == enum_LevelStyle.Invalid ? TResources.GetChunkEditorTiles() : TResources.GetChunkTiles(m_EditStyle));
            GameRenderData.Default().DataInit(m_directionalLight,CameraController.Instance.m_Camera);
            GameRenderData[] customizations = TResources.GetRenderData(targetStyle);
            randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem();
        }
        randomData.DataInit(m_directionalLight, CameraController.Instance.m_Camera);

        ChangeEditSelection(null);
        m_SelectionTiles.ClearPool();
        m_SelectingTile.Init(new TileAxis(-6,-1),ChunkTileData.Default(), m_Random);
        int index = 0;
        switch(m_EditMode)
        {
            default:break;
            case enum_TileSubType.Ground:
                TCommon.TraversalEnum((enum_TileGroundType type)=> {
                    LevelTileEditor editorTile = m_SelectionTiles.AddItem((int)type);
                    editorTile.Init(new TileAxis(index, 0), ChunkTileData.Create( type, enum_TileObjectType.Invalid, enum_TileDirection.Top), m_Random);
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
                    m_SelectionTiles.AddItem((int)type).Init(new TileAxis(index, yOffset), ChunkTileData.Create( enum_TileGroundType.Invalid, type, enum_TileDirection.Top), m_Random);
                    index += type.GetSizeAxis(enum_TileDirection.Top).X;
                    if (index > 20)
                    {
                        index = 0;
                        yOffset -= 3;
                    }
                });
                break;
        }
        System.Random random = new System.Random(Time.time.GetHashCode());
        m_SelectionTiles.m_ActiveItemDic.Traversal((LevelTileEditorSelection tile) => {
            tile.Init(tile.m_Axis, tile.m_Data, random);
        });
        m_TilesData.Traversal((LevelTileEditorData tile) => {
            tile.Init(tile.m_Axis, tile.m_Data, random);
        });
    }

    bool ObjectRegisted(enum_TileObjectType type) => !type.IsEditorTileObject() || m_ItemRestriction.ContainsKey(type);
    
    bool ObjectRestrictRunsOut(enum_TileObjectType type)
    {
        if (!type.IsEditorTileObject())
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
        if (m_GameViewMode)
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

            if(CanEditTile(editorTile,true))
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
            m_SelectingTile.RotateDirection(m_SelectingDirection,m_Random);
            if (CameraController.Instance.InputRayCheck(Input.mousePosition, GameSetting.GameLayer.Mask.I_Interact, ref raycastHit))
                RotateDataTile(raycastHit.transform.GetComponent<LevelTileEditor>());
        }
    }
    bool CanEditTile(LevelTileEditor targetTile,bool objectSelfIncluded)
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
                        editable =(!objectSelfIncluded&&tile.m_Axis==targetTile.m_Axis)||ObjectEditable(tile, m_SelectingTile.m_Data.m_ObjectType);
                        return !editable;
                    });
                }
                break;
        }
        return editable;
    }

    bool ObjectEditable(LevelTileEditor tile,enum_TileObjectType objectType)
    {
        if (tile.m_Data.m_ObjectType != enum_TileObjectType.Invalid)
            return false;

        return LevelExpressions.TileObjectEditable(tile.m_Data.m_GroundType);
    }

    void EditDataTile(LevelTileEditor targetTile)
    {
        if (!targetTile.isDataTile)
            return;
        LevelTileEditorData data = targetTile as LevelTileEditorData;
        switch (m_EditMode)
        {
            case enum_TileSubType.Ground:
                data.SetData(m_SelectingTile.m_Data.m_GroundType, m_SelectingDirection, m_Random);
                break;
            case enum_TileSubType.Object:
                {
                    if (ObjectRestrictRunsOut(m_SelectingTile.m_Data.m_ObjectType))
                        return;
                    data.SetData(m_SelectingTile.m_Data.m_ObjectType, m_SelectingDirection, m_Random);
                }
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
                data.SetData(enum_TileGroundType.Main, enum_TileDirection.Top, m_Random);
                break;
            case enum_TileSubType.Object:
                data.SetData(enum_TileObjectType.Invalid, enum_TileDirection.Top, m_Random);
                break;
        }
    }

    void RotateDataTile(LevelTileEditor targetTile)
    {
        if (!targetTile.isDataTile)
            return;

        LevelTileEditorData data = targetTile as LevelTileEditorData;
        ChangeEditSelection(data);
        if (CanEditTile(data, false))
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
                        editData.ChangeGroundType(editorTile.m_Data.m_GroundType);
                    }
                    break;
                case enum_TileSubType.Object:
                    {
                        if (editorTile.m_Data.m_ObjectType == enum_TileObjectType.Invalid)
                            return;
                        editData.ChangeObjectType(editorTile.m_Data.m_ObjectType);
                    }
                    break;
            }
        }
        editData = editData.ChangeDirection(m_SelectingDirection);
        m_SelectingTile.Init(new TileAxis(-6, -1), editData, m_Random);
        m_SelectionTiles.m_ActiveItemDic.Traversal((int index, LevelTileEditorSelection selection) =>{ selection.SetSelecting(selection.m_Data==editData);});
    }
    #endregion
}
