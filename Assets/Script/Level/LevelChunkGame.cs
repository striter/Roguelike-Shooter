using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;

public class LevelChunkGame : LevelChunkBase
{
    public int m_chunkIndex { get; private set; }
    public Vector3 m_ChunkOrigin { get; private set; }
    public Bounds m_ChunkBounds { get; private set; }
    public Dictionary<enum_TileObjectType, List<ChunkGameObjectData>> m_ChunkObjects = new Dictionary<enum_TileObjectType, List<ChunkGameObjectData>>();
    public List<LevelChunkGame> m_NearbyChunks = new List<LevelChunkGame>();
    public List<LevelTileBase> m_RoadBlockTiles = new List<LevelTileBase>();
    public void InitGameChunk(ChunkGenerateData _data, System.Random _random)
    {
        m_chunkIndex = _data.m_ChunkIndex;
        m_ChunkOrigin = _data.m_Axis.ToPosition();
        m_ChunkBounds = new Bounds(_data.m_Data.m_Size.ToPosition() / 2f + Vector3.up * LevelConst.I_TileSize, new Vector3(_data.m_Data.m_Size.X, 1, _data.m_Data.m_Size.Y) * LevelConst.I_TileSize);
        transform.localPosition = m_ChunkOrigin;
        m_ChunkObjects.Clear();
        m_NearbyChunks.Clear();

        bool isConnectionChunk = _data.m_Data.Type == enum_ChunkType.Connection;
        Dictionary<TileAxis, enum_TileDirection> m_ConnectionIndex = new Dictionary<TileAxis, enum_TileDirection>();
        InitData(_data.m_Data, _random, (TileAxis axis, ChunkTileData tileData) => {

            if (tileData.m_ObjectType.IsEditorTileObject())
            {
                if (!m_ChunkObjects.ContainsKey(tileData.m_ObjectType))
                    m_ChunkObjects.Add(tileData.m_ObjectType, new List<ChunkGameObjectData>());


                if (tileData.m_ObjectType == enum_TileObjectType.RConnection1x5)
                {
                    m_ConnectionIndex.Add(axis,tileData.m_Direction);
                    if (isConnectionChunk)
                        return tileData;
                }
                Vector3 worldPosition = m_ChunkOrigin + Vector3.up * LevelConst.I_TileSize + axis.ToPosition() + tileData.m_ObjectType.GetSizeAxis(tileData.m_Direction).ToPosition() / 2f;
                m_ChunkObjects[tileData.m_ObjectType].Add(new ChunkGameObjectData(worldPosition, tileData.m_Direction.ToRotation()));
                return tileData.ChangeObjectType(enum_TileObjectType.Invalid);
            }
            return tileData;
        });
    }

    public void AddChunkConnection(LevelChunkGame chunk) => m_NearbyChunks.Add(chunk);
    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data, System.Random random)
    {
        base.OnTileInit(tile, axis, data, random);
        if (data.m_GroundType == enum_TileGroundType.Block)
            m_RoadBlockTiles.Add(tile);
    }

    public void BattleBlockLift(bool lift)
    {
        m_NearbyChunks.Traversal((LevelChunkGame chunk) => { chunk.SetBlocksLift(lift);  });
        SetBlocksLift(lift);
    }

    void SetBlocksLift(bool lift)
    {
        m_RoadBlockTiles.Traversal((LevelTileBase tile) =>
        {
            tile.transform.position = new Vector3(tile.transform.position.x, lift ? LevelConst.I_TileSize : 0, tile.transform.position.z);
        });
    }
}
