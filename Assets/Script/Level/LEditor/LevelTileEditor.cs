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

    public override void Clear()
    {
        base.Clear();
        if (m_EditorTerrain)
        {
            m_EditorTerrain.DoRecycle();
            m_EditorTerrain = null;
        }
    }
    protected override float GetTerrainHeight(enum_TileTerrainType terrain) => m_EditorTerrainType== enum_EditorTerrainType.Invalid ? base.GetTerrainHeight(terrain) : m_EditorTerrainType.GetDefaultTerrainType().GetTerrainHeight();
    public override void InitTile(TileAxis axis, ChunkTileData data, System.Random random)
    {
        if (!LevelChunkEditor.Instance.m_GameViewMode)
        {
            if (!LevelObjectManager.HaveObjectItem(data.m_ObjectType))
                data = data.ChangeObjectType(enum_TileObjectType.Invalid);
            if (!LevelObjectManager.HaveEdgeObjectItem(data.m_EdgeObjectType))
                data = data.ChangeEdgeObjectType(enum_TileEdgeObjectType.Invalid);
        }

        m_Data = data;
        m_EditorTerrainType = enum_EditorTerrainType.Invalid;
        if (!LevelChunkEditor.Instance.m_GameViewMode)
        {
            m_EditorTerrainType = data.m_TerrainType.GetEditorGroundType();
            data = data.ChangeTerrainType(enum_TileTerrainType.Invalid);
        }

        Clear();
        base.InitTile(axis,data, random);
        if (m_EditorTerrainType != enum_EditorTerrainType.Invalid)
        {
            m_EditorTerrain = LevelObjectManager.GetEditorGroundItem(m_EditorTerrainType, tf_Models);
            m_EditorTerrain.OnGenerateItem(data, random);
            m_EditorTerrain.transform.localPosition = Vector3.zero;
        }
    }


    protected override bool WillGenerateObject(enum_TileObjectType type) => LevelChunkEditor.Instance.m_GameViewMode ? base.WillGenerateObject(type) : type != enum_TileObjectType.Invalid;
    protected override bool WillGeneratePlants(enum_TileObjectType type) => LevelChunkEditor.Instance.m_GameViewMode ? base.WillGeneratePlants(type) : false;

    public void RotateDirection(enum_TileDirection direction,System.Random random)
    {
        m_Data = m_Data.ChangeDirection(direction);
        InitTile(m_Axis, m_Data,random);
    }
}
