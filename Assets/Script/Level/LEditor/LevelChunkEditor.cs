﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTiles;
using LevelSetting;
using GameSetting;

public class LevelChunkEditor : LevelChunkBase
{
    public static LevelChunkEditor Instance { get; private set; }
    public Transform tf_CameraPos { get; private set; }
    public enum_LevelEditorEditType m_EditType { get; private set; } = enum_LevelEditorEditType.Invalid;
    public enum_BattleStyle m_EditStyle { get; private set; } = enum_BattleStyle.Invalid;
    public bool m_GameViewMode { get; private set; }
    enum_BattleStyle m_ViewStyle;
    public LevelTileEditorData[,] m_TilesData { get; private set; }
    ObjectPoolListComponent<int, LevelTileEditorSelection> m_SelectionTiles;
    LevelTileEditorSelection m_SelectingTile;
    enum_TileDirection m_SelectingDirection = enum_TileDirection.Top;
    List<LevelTileEditorData> temp_RelativeTiles = new List<LevelTileEditorData>();
    System.Random m_Random = new System.Random();
    public Light m_DirectionalLight { get; private set; }
    Transform tf_Selections;
    public int m_Width { get; private set; }
    public int m_Height { get; private set; }
    private void Awake()
    {
        Instance = this;
        tf_CameraPos = transform.Find("CameraPos");
        tf_Selections = tf_CameraPos.Find("Selections");
        m_SelectionTiles = new ObjectPoolListComponent<int, LevelTileEditorSelection>(tf_CameraPos.Find("Selections/SelectionPool"), "SelectionItem");
        m_SelectingTile = tf_CameraPos.Find("Selections/SelectingTile").GetComponent<LevelTileEditorSelection>();
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
        OnInitItem();
    }
    private void Start()
    {
        FPSCameraController.Instance.Attach(tf_CameraPos, false);
        LevelObjectManager.Register(TResources.GetChunkEditorTiles());
    }
    public LevelChunkEditor CheckDatas()
    {
        m_TilesData.Traversal((LevelTileEditorData data) => {
            data.UpdateTerrain(TileTools.GetDirectionAxies(m_Width, m_Height, data.m_Axis, TileTools.m_EdgeDirections, GetEditorItemData), TileTools.GetDirectionAxies(m_Width, m_Height, data.m_Axis, TileTools.m_AngleDirections, GetEditorItemData), m_Random);
        });
        return this;
    }


    public void Init(LevelChunkData _data)
    {
        m_Width = _data.Width;
        m_Height = _data.Height;
        m_TilesData = new LevelTileEditorData[_data.Width, _data.Height];
        tf_CameraPos.transform.localPosition = new Vector3(_data.Width/ 2f * LevelConst.I_TileSize, 0, 0);
        m_EditType = enum_LevelEditorEditType.Terrain;
        InitData(_data.Width,_data.Height,_data.GetData(), m_Random);
        m_GameViewMode = false;
        OnCameraEditModeChanged();
        OnEditModeChanged();
    }

    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data, System.Random random)
    {
        m_TilesData[axis.X, axis.Y] = (tile as LevelTileEditorData);
        m_TilesData[axis.X, axis.Y].InitTile(axis, data, random);
    }

    public void Resize(int size, enum_TileDirection direction)=>  Init(LevelChunkData.NewData(size,direction, m_TilesData));
    
    private void Update()
    {
        CameraMoveCheck();
        ChunkEditCheck();
    }
    void OnCameraEditModeChanged()
    {
        CameraController.Instance.m_Camera.orthographic = !m_GameViewMode;
        if (m_GameViewMode)
        {
            CameraController.Instance.SetCameraRotation(60, 0);
        }
        else
        {
            CameraController.Instance.m_Camera.orthographicSize = 10;
            tf_CameraPos.transform.position = Vector3.up * 10;
            CameraController.Instance.SetCameraRotation(90, 0);
        }

    }

    void CameraMoveCheck()
    {
        if (m_GameViewMode)
        {
            tf_CameraPos.Translate((CameraController.Instance.m_Camera.transform.forward * Input.GetAxis("Vertical") + CameraController.Instance.m_Camera.transform.right * Input.GetAxis("Horizontal")).normalized * Time.deltaTime * 40f);
            CameraController.Instance.RotateCamera(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * 50f);
        }
        else
        {
            tf_CameraPos.Translate((Vector3.forward * Input.GetAxis("Vertical") + Vector3.right* Input.GetAxis("Horizontal")).normalized * Time.deltaTime * 40f);
            CameraController.Instance.m_Camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime*500f;
            tf_Selections.transform.position = CameraController.Instance.m_Camera.ViewportToWorldPoint(new Vector3(0,0,5));
            tf_Selections.transform.localScale = Vector3.one * CameraController.Instance.m_Camera.orthographicSize / 20f;
        }
    }

    RaycastHit raycastHit;
    void ChunkEditCheck()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(m_GameViewMode)
            {
                m_GameViewMode = false;
                OnCameraEditModeChanged();
            }

            if (!m_GameViewMode)
                m_EditType = m_EditType.Next();
            OnEditModeChanged();
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (!m_GameViewMode)
            {
                m_ViewStyle = enum_BattleStyle.Horde;
                CheckDatas();
            }
            else
            {
                m_ViewStyle++;
                if (m_ViewStyle > enum_BattleStyle.Undead)
                    m_ViewStyle = enum_BattleStyle.Forest;
            }

            if(!m_GameViewMode)
            {
                m_GameViewMode = true;
                OnCameraEditModeChanged();
            }
            OnEditModeChanged();
        }

        MouseRayChunkEdit();
    }
    #region EditMode
    void OnEditModeChanged()
    {
        LevelEditorUI.Instance.SetActivate(!m_GameViewMode);
        enum_BattleStyle targetStyle = m_GameViewMode ? m_ViewStyle : enum_BattleStyle.Invalid;
        GameRenderData randomData = GameRenderData.Default();
        if (targetStyle != m_EditStyle)
        {
            m_TilesData.Traversal((LevelTileEditorData tile) => { tile.Clear(); });
            m_SelectingTile.Clear();
            m_SelectionTiles.m_ActiveItemDic.Traversal((LevelTileEditorSelection tile) => { tile.Clear(); });
            m_EditStyle = targetStyle;
            LevelObjectManager.Register(m_EditStyle == enum_BattleStyle.Invalid ? TResources.GetChunkEditorTiles() : TResources.GetChunkTiles(m_EditStyle));
            GameRenderData.Default().DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);
            GameRenderData[] customizations = TResources.GetRenderData(targetStyle);
            randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem();
        }
        randomData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);

        m_SelectionTiles.Clear();
        m_SelectingTile.InitTile(new TileAxis(-2,0), ChunkTileData.EditorDefault(), m_Random);
        ChangeEditSelection(null);

        System.Random random = new System.Random(Time.time.GetHashCode());
        m_TilesData.Traversal((LevelTileEditorData tile) => {
            tile.InitTile(tile.m_Axis, tile.m_Data, random);
        });

        if (m_GameViewMode)
            return;
        int index = 0;
        switch(m_EditType)
        {
            case enum_LevelEditorEditType.Terrain:
                TCommon.TraversalEnum((enum_EditorTerrainType type) => {
                    LevelTileEditor editorTile = m_SelectionTiles.AddItem((int)type);
                    editorTile.InitTile(new TileAxis(index, 0), ChunkTileData.Create(type.GetDefaultTerrainType(), enum_TileObjectType.Invalid, enum_TileEdgeObjectType.Invalid, enum_TileDirection.Top), m_Random);
                    index += 1;
                    if (index > 20)
                        index = 0;
                });
                break;
            case enum_LevelEditorEditType.Object:
                int yOffset = 0;
                TCommon.TraversalEnum((enum_TileObjectType type) => {
                    if (!ObjectRegisted(type))
                        return;
                    m_SelectionTiles.AddItem((int)type).InitTile(new TileAxis(index, yOffset), ChunkTileData.Create(enum_TileTerrainType.Invalid, type, enum_TileEdgeObjectType.Invalid, enum_TileDirection.Top), m_Random);
                    index += type.GetSizeAxis(enum_TileDirection.Top).X;
                    if (index > 20)
                    {
                        index = 0;
                        yOffset += 3;
                    }
                });
                break;
            case enum_LevelEditorEditType.EdgeObject:
                TCommon.TraversalEnum((enum_TileEdgeObjectType type) =>
                {
                    LevelTileEditor editorTile = m_SelectionTiles.AddItem((int)type);
                    editorTile.InitTile(new TileAxis(index, 0), ChunkTileData.Create(enum_TileTerrainType.Invalid, enum_TileObjectType.Invalid, type, enum_TileDirection.Top), m_Random);
                    index += 1;
                });
                break;
        }
        m_SelectionTiles.m_ActiveItemDic.Traversal((LevelTileEditorSelection tile) => {
            tile.InitTile(tile.m_Axis, tile.m_Data, random);
        });
    }

    bool ObjectRegisted(enum_TileObjectType type) => !type.IsEditorTileObject() || LevelConst.m_ChunkRestriction.ContainsKey(type);

    bool ObjectRestrictRunsOut(enum_TileObjectType type)
    {
        if (!type.IsEditorTileObject())
            return false;

        Dictionary<enum_TileObjectType, int> m_ItemRestriction = LevelConst.m_ChunkRestriction.DeepCopy();
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

        if (m_EditType== enum_LevelEditorEditType.Terrain)
            return true;

        bool editable = true;
        if (!m_TilesData.Get(targetTile.m_Axis, (m_SelectingTile.m_Data.m_ObjectType).GetSizeAxis(m_SelectingDirection), ref temp_RelativeTiles))
            return false;

        enum_EditorTerrainType targetTileTerrain = targetTile.m_EditorTerrainType;
        temp_RelativeTiles.TraversalBreak((LevelTileEditorData tile) =>
        {
            editable = (!objectSelfIncluded && tile.m_Axis == targetTile.m_Axis) || tile.m_EditorTerrainType==targetTileTerrain;
            return !editable;
        });
        return editable;
    }

    void EditDataTile(LevelTileEditor targetTile)
    {
        if (!targetTile.isDataTile)
            return;
        LevelTileEditorData data = targetTile as LevelTileEditorData;
        switch(m_EditType)
        {
            case enum_LevelEditorEditType.Terrain:
                data.SetTerrain(m_SelectingTile.m_EditorTerrainType, m_Random);
                break;
            case enum_LevelEditorEditType.EdgeObject:
                data.SetData(m_SelectingTile.m_Data.m_EdgeObjectType, m_SelectingDirection, m_Random);
                break;
            case enum_LevelEditorEditType.Object:
                if (ObjectRestrictRunsOut(m_SelectingTile.m_Data.m_ObjectType))
                    return;
                data.SetData(m_SelectingTile.m_Data.m_ObjectType, m_SelectingDirection, m_Random);
                break;
        }
    }

    enum_EditorTerrainType GetEditorItemData(TileAxis axis) => m_TilesData[axis.X, axis.Y].m_EditorTerrainType;

    void RemoveDataTile(LevelTileEditor targetTile)
    {
        if(!targetTile.isDataTile)
            return;
        LevelTileEditorData data = targetTile as LevelTileEditorData;
        switch(m_EditType)
        {
            case enum_LevelEditorEditType.Terrain:
                data.SetTerrain(enum_EditorTerrainType.Invalid, m_Random);
                break;
            case enum_LevelEditorEditType.Object:
                data.SetData(enum_TileObjectType.Invalid, enum_TileDirection.Top, m_Random);
                break;
            case enum_LevelEditorEditType.EdgeObject:
                data.SetData( enum_TileEdgeObjectType.Invalid, enum_TileDirection.Top, m_Random);
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

    void ChangeEditSelection(LevelTileEditor editorTile)
    {
        ChunkTileData editData = ChunkTileData.EditorDefault();
        if (editorTile == null)
            return;

        switch (m_EditType)
        {
            case enum_LevelEditorEditType.Terrain:
                editData.ChangeTerrainType(editorTile.m_EditorTerrainType.GetDefaultTerrainType());
                break;
            case enum_LevelEditorEditType.Object:
                editData.ChangeObjectType(editorTile.m_Data.m_ObjectType);
                break;
            case enum_LevelEditorEditType.EdgeObject:
                editData.ChangeEdgeObjectType(editorTile.m_Data.m_EdgeObjectType);
                break;
        }
        editData = editData.ChangeDirection(m_SelectingDirection);
        m_SelectingTile.InitTile(new TileAxis(-6, -1), editData, m_Random);
        m_SelectionTiles.m_ActiveItemDic.Traversal((int index, LevelTileEditorSelection selection) =>
        {
            bool selecting = false;

            switch (m_EditType)
            {
                case enum_LevelEditorEditType.Terrain:
                    selecting = editData.m_TerrainType == selection.m_Data.m_TerrainType;
                    break;
                case enum_LevelEditorEditType.Object:
                    selecting = editData.m_ObjectType == selection.m_Data.m_ObjectType;
                    break;
                case enum_LevelEditorEditType.EdgeObject:
                    selecting = editData.m_EdgeObjectType == selection.m_Data.m_EdgeObjectType;
                    break;
            }
            selection.SetSelecting(selecting);
        });
        #endregion
    }
}
