using GameSetting;
using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TTiles;
using UnityEngine;

public class LevelChunkGame : LevelChunkBase
{
    public enum_ChunkEventType m_EventType { get; private set; }
    public TileAxis m_Size { get; private set; }
    public List<LevelChunkGame> m_NearbyChunks { get; private set; } = new List<LevelChunkGame>();
    public List<TileObjectBlockLift> m_RoadBlockTiles { get; private set; } = new List<TileObjectBlockLift>();
    public Dictionary<enum_TileObject, List<ChunkGameObjectData>> m_ChunkObjects { get; private set; } = new Dictionary<enum_TileObject, List<ChunkGameObjectData>>();
    Action OnChunkObjectDestroy;

    Transform m_RoadBlockParent;
    public void InitGameChunk(enum_ChunkEventType _eventType, LevelChunkData _data, System.Random _random,Action OnChunkObjectDestroy)
    {
        m_RoadBlockParent = transform.Find("RoadBlocks");
        m_ChunkObjects.Clear();
        m_NearbyChunks.Clear();
        m_RoadBlockTiles.Clear();
        m_EventType =_eventType;
        m_Size = _data.m_Size;
        gameObject.name = _data.name;
        this.OnChunkObjectDestroy = OnChunkObjectDestroy;
        InitData(_data, _random, (TileAxis axis, ChunkTileData tileData) => {
            if (tileData.m_ObjectType.IsEditorTileObject())
            {
                if (!m_ChunkObjects.ContainsKey(tileData.m_ObjectType))
                    m_ChunkObjects.Add(tileData.m_ObjectType, new List<ChunkGameObjectData>());

                Vector3 worldPosition = axis.ToPosition()  + tileData.m_ObjectType.GetSizeAxis(tileData.m_Direction).ToPosition() / 2f;
                m_ChunkObjects[tileData.m_ObjectType].Add(new ChunkGameObjectData(worldPosition, tileData.m_Direction.ToRotation()));
                return tileData.ChangeObjectType(enum_TileObject.Invalid);
            }
            return tileData;
        });
        StaticBatchingUtility.Combine(m_TilePool.transform.gameObject);
    }

    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data, System.Random random)
    {
        base.OnTileInit(tile, axis, data, random);
        if (tile.m_Object)
            tile.m_Object.GameInit(GameExpression.GetLevelObjectHealth(tile.m_Object.m_ObjectType), OnChunkObjectDestroy);
        if (data.m_ObjectType == enum_TileObject.Block)
        {
            TileObjectBlockLift roadBlock = tile.m_Object as TileObjectBlockLift;
            roadBlock.transform.SetParent(m_RoadBlockParent);
            m_RoadBlockTiles.Add(roadBlock);
        }
    }

    public void SetBlocksLift(bool lift)
    {
        m_RoadBlockTiles.Traversal((TileObjectBlockLift tile) => {
            tile.SetLift(lift);
        });
    }
}
