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
    public bool m_EditGround { get; private set; } = false;
    public enum_GameStyle m_EditStyle { get; private set; } = enum_GameStyle.Invalid;
    public bool m_GameViewMode { get; private set; }
    enum_GameStyle m_ViewStyle;
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
        LevelObjectManager.Register(TResources.GetChunkEditorTiles());
    }


    public void Init(LevelChunkData _data)
    {
        m_TilesData = new LevelTileEditorData[_data.Width, _data.Height];
        tf_CameraPos.transform.localPosition = new Vector3(m_Width / 2f * LevelConst.I_TileSize, 0, 0);
        TPSCameraController.Instance.SetCameraPosition(tf_CameraPos.transform.localPosition);
        TPSCameraController.Instance.SetCameraRotation(60, 0);
        m_ItemRestriction = LevelExpressions.GetChunkRestriction(m_ChunkType);
        m_EditGround = false;
        m_GameViewMode = false;
        InitData(_data, m_Random);
        CheckEditMode();
    }

    protected override bool WillGenerateTile(ChunkTileData data) => true;
    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data, System.Random random)
    {
        m_TilesData[axis.X, axis.Y] = (tile as LevelTileEditorData);
        m_TilesData[axis.X, axis.Y].InitEditorTile(axis, data, random);
    }

    public void Resize(int sizeX, int sizeY)
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
            m_EditGround = !m_EditGround;
            CheckEditMode();
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            m_EditGround = false;
            if (!m_GameViewMode)
                m_ViewStyle = enum_GameStyle.Horde;
            else
            {
                m_ViewStyle++;
                if (m_ViewStyle > enum_GameStyle.Undead)
                    m_ViewStyle = enum_GameStyle.Forest;
            }
            m_GameViewMode = true;
            CheckEditMode();
        }

        MouseRayChunkEdit();
    }
    #region EditMode
    void CheckEditMode()
    {
        enum_GameStyle targetStyle = m_GameViewMode ? m_ViewStyle : enum_GameStyle.Invalid;
        GameRenderData randomData = GameRenderData.Default();
        if (targetStyle != m_EditStyle)
        {
            m_TilesData.Traversal((LevelTileEditorData tile) => { tile.Clear(); });
            m_SelectingTile.Clear();
            m_SelectionTiles.m_ActiveItemDic.Traversal((LevelTileEditorSelection tile) => { tile.Clear(); });
            m_EditStyle = targetStyle;
            LevelObjectManager.Register(m_EditStyle == enum_GameStyle.Invalid ? TResources.GetChunkEditorTiles() : TResources.GetChunkTiles(m_EditStyle));
            GameRenderData.Default().DataInit(m_directionalLight, CameraController.Instance.m_Camera);
            GameRenderData[] customizations = TResources.GetRenderData(targetStyle);
            randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem();
        }
        randomData.DataInit(m_directionalLight, CameraController.Instance.m_Camera);

        m_SelectionTiles.ClearPool();
        m_SelectingTile.InitEditorTile(new TileAxis(-6, -1), ChunkTileData.Default(), m_Random);
        ChangeEditSelection(null);

        System.Random random = new System.Random(Time.time.GetHashCode());
        m_TilesData.Traversal((LevelTileEditorData tile) => {
            tile.InitEditorTile(tile.m_Axis, tile.m_Data, random);
        });

        if (m_GameViewMode)
            return;
        int index = 0;
        if (m_EditGround)
        {
            TCommon.TraversalEnum((enum_EditorGroundType type) => {
                LevelTileEditor editorTile = m_SelectionTiles.AddItem((int)type);
                editorTile.InitEditorTile(new TileAxis(index, 0), ChunkTileData.Create(type.GetEditorTerrainType(), enum_TileObjectType.Invalid, enum_TileDirection.Top), m_Random);
                index += 1;
                if (index > 20)
                    index = 0;
            });

        }
        else
        {
            int yOffset = 0;
            TCommon.TraversalEnum((enum_TileObjectType type) => {
                if (!ObjectRegisted(type))
                    return;
                m_SelectionTiles.AddItem((int)type).InitEditorTile(new TileAxis(index, yOffset), ChunkTileData.Create(enum_TileTerrainType.Invalid, type, enum_TileDirection.Top), m_Random);
                index += type.GetSizeAxis(enum_TileDirection.Top).X;
                if (index > 20)
                {
                    index = 0;
                    yOffset -= 3;
                }
            });
        }
        m_SelectionTiles.m_ActiveItemDic.Traversal((LevelTileEditorSelection tile) => {
            tile.InitEditorTile(tile.m_Axis, tile.m_Data, random);
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

            if (CanEditTile(editorTile, true))
                EditDataTile(editorTile);
        }
        else if (Input.GetMouseButton(1))
        {
            if (!CameraController.Instance.InputRayCheck(Input.mousePosition, GameSetting.GameLayer.Mask.I_Interact, ref raycastHit))
                return;
            RemoveDataTile(raycastHit.transform.GetComponent<LevelTileEditor>());
        }
        else if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            m_SelectingDirection = m_SelectingDirection.Next();
            m_SelectingTile.RotateDirection(m_SelectingDirection, m_Random);
            if (CameraController.Instance.InputRayCheck(Input.mousePosition, GameSetting.GameLayer.Mask.I_Interact, ref raycastHit))
                RotateDataTile(raycastHit.transform.GetComponent<LevelTileEditor>());
        }
    }
    bool CanEditTile(LevelTileEditor targetTile, bool objectSelfIncluded)
    {
        if (!targetTile.isDataTile)
            return false;

        if (m_EditGround)
            return true;

        bool editable = true;

        if (m_SelectingTile.m_Data.m_ObjectType == enum_TileObjectType.Invalid)
            return false;

        if (!m_TilesData.Get(targetTile.m_Axis, (m_SelectingTile.m_Data.m_ObjectType).GetSizeAxis(m_SelectingDirection), ref temp_RelativeTiles))
            return false;

        temp_RelativeTiles.TraversalBreak((LevelTileEditorData tile) =>
        {
            editable = (!objectSelfIncluded && tile.m_Axis == targetTile.m_Axis) || ObjectEditable(tile, m_SelectingTile.m_Data.m_ObjectType);
            return !editable;
        });
        return editable;
    }

    bool ObjectEditable(LevelTileEditor tile, enum_TileObjectType objectType)
    {
        if (tile.m_Data.m_ObjectType != enum_TileObjectType.Invalid)
            return false;

        return LevelExpressions.EditorObjectEditable(tile.m_EditorGroundType);
    }

    void EditDataTile(LevelTileEditor targetTile)
    {
        if (!targetTile.isDataTile)
            return;
        LevelTileEditorData data = targetTile as LevelTileEditorData;
        if (m_EditGround)
        {
            data.SetGround(m_SelectingTile.m_EditorGroundType, m_Random);
            UpdateAxisTerrains(data);
        }
        else
        {
            if (ObjectRestrictRunsOut(m_SelectingTile.m_Data.m_ObjectType))
                return;
            data.SetData(m_SelectingTile.m_Data.m_ObjectType, m_SelectingDirection, m_Random);
        }
    }

    enum_EditorGroundType GetEditorItemData(TileAxis axis) => m_TilesData[axis.X, axis.Y].m_EditorGroundType;

    void RemoveDataTile(LevelTileEditor targetTile)
    {
        if(!targetTile.isDataTile)
            return;
        LevelTileEditorData data = targetTile as LevelTileEditorData;
        if (m_EditGround)
        {
            data.SetGround(enum_EditorGroundType.Main, m_Random);
            UpdateAxisTerrains(data);
        }
        else
            data.SetData(enum_TileObjectType.Invalid, enum_TileDirection.Top, m_Random);
    }

    void UpdateAxisTerrains(LevelTileEditorData data)
    {
        List<TileAxis> nearbyAxies = TileTools.GetDirectionAxies(m_Width, m_Height, data.m_Axis, TileTools.m_AllDirections);
        nearbyAxies.Add(data.m_Axis);
        nearbyAxies.Traversal((TileAxis axis) => {
            LevelTileEditorData axisData = m_TilesData[axis.X, axis.Y] as LevelTileEditorData;
            if (axisData.m_EditorGroundType != enum_EditorGroundType.Water)
                return;
            axisData.UpdateWaterTerrain(TileTools.GetDirectionAxies(m_Width, m_Height, axis, TileTools.m_EdgeDirections, GetEditorItemData), TileTools.GetDirectionAxies(m_Width, m_Height, axis, TileTools.m_AngleDirections, GetEditorItemData), m_Random);
        });
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
        if (editorTile == null)
            return;

        if (m_EditGround)
        {
            if (editorTile.m_Data.m_TerrainType == enum_TileTerrainType.Invalid)
                return;
            editData.ChangeTerrainType(editorTile.m_EditorGroundType== enum_EditorGroundType.Main? enum_TileTerrainType.Ground: enum_TileTerrainType.River_0W);
        }
        else
        {
            if (editorTile.m_Data.m_ObjectType == enum_TileObjectType.Invalid)
                return;
            editData.ChangeObjectType(editorTile.m_Data.m_ObjectType);
        }
        editData = editData.ChangeDirection(m_SelectingDirection);
        m_SelectingTile.InitEditorTile(new TileAxis(-6, -1), editData, m_Random);
        m_SelectionTiles.m_ActiveItemDic.Traversal((int index, LevelTileEditorSelection selection) =>{ selection.SetSelecting(m_EditGround? editData.m_TerrainType == selection.m_Data.m_TerrainType : editData.m_ObjectType == selection.m_Data.m_ObjectType);});
    }
    #endregion
}
