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
    public int m_chunkIndex { get; private set; }
    public TileBounds m_ChunkMapBounds { get; private set; }
    public enum_ChunkEventType m_ChunkEventType { get; private set; } = enum_ChunkEventType.Invalid;
    public List<LevelChunkGame> m_NearbyChunks { get; private set; } = new List<LevelChunkGame>();
    public List<TileGroundBlockLift> m_RoadBlockTiles { get; private set; } = new List<TileGroundBlockLift>();
    public Dictionary<enum_TileObjectType, List<ChunkGameObjectData>> m_ChunkObjects { get; private set; } = new Dictionary<enum_TileObjectType, List<ChunkGameObjectData>>();
    Action<int> OnChunkObjectDestroy;

    public void InitGameChunk(TileAxis mapOrigin, ChunkGenerateData _data, System.Random _random,Action<int> OnChunkObjectDestroy)
    {
        m_ChunkObjects.Clear();
        m_NearbyChunks.Clear();
        m_RoadBlockTiles.Clear();
        m_chunkIndex = _data.m_ChunkIndex;
        m_ChunkEventType = _data.m_EventType;
        gameObject.name = string.Format("{0}({1})", m_chunkIndex, _data.m_Data.name);
        this.OnChunkObjectDestroy = OnChunkObjectDestroy;
        m_ChunkMapBounds = new TileBounds(_data.m_GenerateCheckBounds.m_Origin-mapOrigin, _data.m_GenerateCheckBounds.m_Size);
        transform.localPosition = _data.m_Origin.ToPosition();
        ShowTiles(true);
        InitData(_data.m_Data, _random, (TileAxis axis, ChunkTileData tileData) => {
            if (tileData.m_ObjectType.IsEditorTileObject())
            {
                if (!m_ChunkObjects.ContainsKey(tileData.m_ObjectType))
                    m_ChunkObjects.Add(tileData.m_ObjectType, new List<ChunkGameObjectData>());

                Vector3 worldPosition = (_data.m_Origin+ axis).ToPosition()  + Vector3.up * LevelConst.I_TileSize + tileData.m_ObjectType.GetSizeAxis(tileData.m_Direction).ToPosition() / 2f;
                m_ChunkObjects[tileData.m_ObjectType].Add(new ChunkGameObjectData(worldPosition, tileData.m_Direction.ToRotation()));
                return tileData.ChangeObjectType(enum_TileObjectType.Invalid);
            }
            return tileData;
        });
    }
    private void OnEnable()
    {
        TBroadCaster<enum_BC_GameStatus>.Add<List<int>>(enum_BC_GameStatus.OnChunkQuadrantCheck, OnQuadrantChunkCheck);
    }
    private void OnDisable()
    {
        TBroadCaster<enum_BC_GameStatus>.Remove<List<int>>(enum_BC_GameStatus.OnChunkQuadrantCheck, OnQuadrantChunkCheck);
    }
    public void OnQuadrantChunkCheck(List<int> showChunkIndex) => ShowTiles(showChunkIndex.Contains(m_chunkIndex));

    public void AddChunkConnection(LevelChunkGame chunk) => m_NearbyChunks.Add(chunk);

    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data, System.Random random)
    {
        base.OnTileInit(tile, axis, data, random);
        if (tile.m_Object)
            tile.m_Object.GameInit(GameExpression.GetLevelObjectHealth( tile.m_Object.m_ObjectType),OnObjectDestroyed);
        if (data.m_GroundType == enum_TileGroundType.Block)
            m_RoadBlockTiles.Add(tile.m_Ground as TileGroundBlockLift);
    }
    void OnObjectDestroyed() => OnChunkObjectDestroy(m_chunkIndex);

    void ShowTiles(bool show) => m_TilePool.transform.SetActivate(show);

    public List<int> BattleBlockLift(bool lift)
    {
        List<int> chunkIndexes = new List<int>();
        chunkIndexes.Add(m_chunkIndex);
        SetBlocksLift(lift);
        m_NearbyChunks.Traversal((LevelChunkGame chunk) => {
            chunkIndexes.Add(chunk.m_chunkIndex);
            chunk.SetBlocksLift(lift);  });
        return chunkIndexes;
    }

    void SetBlocksLift(bool lift)
    {
        ShowTiles(true);
        m_RoadBlockTiles.Traversal((TileGroundBlockLift tile) => {
            tile.SetLift(lift);
        });
    }
}
