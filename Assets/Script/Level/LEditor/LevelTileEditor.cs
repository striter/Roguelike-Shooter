using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using LevelSetting;
using System;

[RequireComponent(typeof(HitCheckInteract))]
public class LevelTileEditor : LevelTileBase {

    public virtual bool isDataTile => false;
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
        if (!LevelChunkEditor.Instance.m_GameViewMode)
        {
            if (!LevelObjectManager.HaveObjectItem(data.m_ObjectType))
                data = data.ChangeObjectType(enum_TileObjectType.Invalid);
            if (!LevelObjectManager.HaveEdgeObjectItem(data.m_EdgeObjectType))
                data = data.ChangeEdgeObjectType(enum_TileEdgeObjectType.Invalid);
        }

        m_Data = data;
        m_EditorGroundType = enum_EditorGroundType.Invalid;
        if (!LevelChunkEditor.Instance.m_GameViewMode)
        {
            m_EditorGroundType = data.m_TerrainType.GetEditorGroundType();
            data = data.ChangeTerrainType(enum_TileTerrainType.Invalid);
            switch (LevelChunkEditor.Instance.m_EditType)
            {
                case enum_LevelEditorEditType.Terrain:
                    data = data.ChangeObjectType(enum_TileObjectType.Invalid).ChangeEdgeObjectType(enum_TileEdgeObjectType.Invalid);
                    break;
            }
        }

        base.InitTile(axis,data, random);
        if (m_EditorGroundType != enum_EditorGroundType.Invalid)
        {
            m_EditorGround = LevelObjectManager.GetEditorGroundItem(m_EditorGroundType, tf_Models);
            m_EditorGround.OnGenerateItem(data, random);
            m_EditorGround.transform.localPosition = Vector3.zero;
        }

    }


    protected override bool WillGenerateObject(enum_TileObjectType type) => LevelChunkEditor.Instance.m_GameViewMode ?   base.WillGenerateObject(type): type != enum_TileObjectType.Invalid;

    public void RotateDirection(enum_TileDirection direction,System.Random random)
    {
        m_Data = m_Data.ChangeDirection(direction);
        InitTile(m_Axis, m_Data,random);
    }

}
