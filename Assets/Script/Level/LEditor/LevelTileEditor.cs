using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using LevelSetting;
using System;

[RequireComponent(typeof(HitCheckInteract))]
public class LevelTileEditor : LevelTileBase {

    public virtual bool isDataTile => false;
    public enum_EditorTerrainType m_EditorTerrainType { get; protected set; }
    public ChunkTileData m_Data { get; protected set; }
    public LevelTileItemEditorTerrain m_EditorTerrain { get; private set; }
    public LevelTileItemEditorTerrainMap m_EditorMap { get; private set; }

    public override void Clear()
    {
        base.Clear();
        if (m_EditorTerrain)
        {
            m_EditorTerrain.DoRecycle();
            m_EditorTerrain = null;
        }

        if(m_EditorMap)
        {
            m_EditorMap.DoRecycle();
            m_EditorMap = null;
        }
    }
    protected override float GetTerrainHeight(enum_TileTerrain terrain) => m_EditorTerrainType== enum_EditorTerrainType.Invalid ? base.GetTerrainHeight(terrain) : m_EditorTerrainType.GetDefaultTerrainType().GetTerrainHeight();
    public override void InitTile(TileAxis axis, ChunkTileData data, System.Random random)
    {
        if (!LevelChunkEditor.Instance.m_GameViewMode)
        {
            if (!LevelObjectManager.HaveObjectItem(data.m_ObjectType))
                data = data.ChangeObjectType(enum_TileObject.Invalid);
            if (!LevelObjectManager.HaveEdgeObjectItem(data.m_EdgeObjectType))
                data = data.ChangeEdgeObjectType(enum_TileEdgeObject.Invalid);
            if (data.m_TerrainMap == 0)
                data = data.ChangeTerrainMapType(enum_TileTerrainMap.Ground);
        }

        m_Data = data;
        Clear();
        m_EditorTerrainType = enum_EditorTerrainType.Invalid;
        if (!LevelChunkEditor.Instance.m_GameViewMode)
        {
            if (data.m_TerrainMap != enum_TileTerrainMap.Invalid)
            {
                m_EditorMap = LevelObjectManager.GetEditorTerrainMapItem(data.m_TerrainMap, tf_Models);
                m_EditorMap.OnGenerateItem(data, random);
                m_EditorMap.transform.localPosition = Vector3.up * GetTerrainHeight(data.m_TerrainType);
            }

            m_EditorTerrainType = data.m_TerrainType.GetEditorGroundType();
            data = data.ChangeTerrainType(enum_TileTerrain.Invalid);

            if (m_EditorTerrainType != enum_EditorTerrainType.Invalid)
            {
                m_EditorTerrain = LevelObjectManager.GetEditorTerrainItem(m_EditorTerrainType, tf_Models);
                m_EditorTerrain.OnGenerateItem(data, random);
                m_EditorTerrain.transform.localPosition = Vector3.zero;
            }
        }

        base.InitTile(axis,data, random);
    }


    protected override bool WillGenerateObject(enum_TileObject type) => LevelChunkEditor.Instance.m_GameViewMode ? base.WillGenerateObject(type) : type != enum_TileObject.Invalid;
    protected override bool WillGeneratePlants(ChunkTileData data) => LevelChunkEditor.Instance.m_GameViewMode ? base.WillGeneratePlants(data) : false;

    public void RotateDirection(enum_TileDirection direction,System.Random random)
    {
        m_Data = m_Data.ChangeDirection(direction);
        InitTile(m_Axis, m_Data,random);
    }
}
