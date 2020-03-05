using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using LevelSetting;
using System;

[RequireComponent(typeof(HitCheckInteract))]
public class LevelTileEditor : LevelTileBase {

    public virtual bool isDataTile => false;
    BoxCollider m_Collider;
    public enum_EditorGroundType m_EditorGroundType { get; protected set; }
    public ChunkTileData m_Data { get; protected set; }
    public LevelTileEditorGround m_EditorGround { get; private set; }

    public override void Clear()
    {
        base.Clear();
        if (m_EditorGround)
        {
            m_EditorGround.DoRecycle();
            m_EditorGround = null;
        }
    }

    public override void InitTile(TileAxis axis, ChunkTileData data, System.Random random)
    {
        m_Collider = GetComponent<BoxCollider>();
        base.InitTile(axis, data, random);
        if (m_EditorGroundType != enum_EditorGroundType.Invalid)
        {
            m_EditorGround = LevelObjectManager.GetEditorGroundItem(m_EditorGroundType, tf_Models);
            m_EditorGround.OnGenerateItem(data,random);
            m_EditorGround.transform.localPosition = Vector3.zero;
        }
    }

    public virtual void InitEditorTile(TileAxis axis, ChunkTileData data,System.Random random)
    {
        m_Data = data;
        ChunkTileData targetData=m_Data;
        m_EditorGroundType = enum_EditorGroundType.Invalid;
        if (!LevelChunkEditor.Instance.m_GameViewMode)
        {
            m_EditorGroundType = targetData.m_TerrainType.GetEditorGroundType();
            targetData = targetData.ChangeTerrainType(enum_TileTerrainType.Invalid);
            if (LevelChunkEditor.Instance.m_EditGround)
                targetData = targetData.ChangeObjectType(enum_TileObjectType.Invalid);
        }
        InitTile(axis, targetData, random);
    }

    protected override bool WillGenerateObject(enum_TileObjectType type) => LevelChunkEditor.Instance.m_GameViewMode ?   base.WillGenerateObject(type): type != enum_TileObjectType.Invalid;

    public void RotateDirection(enum_TileDirection direction,System.Random random)
    {
        m_Data = m_Data.ChangeDirection(direction);
        InitEditorTile(m_Axis, m_Data,random);
    }

}
