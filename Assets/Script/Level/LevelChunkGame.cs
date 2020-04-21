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
    public int m_ChunkIndex { get; private set; }
    public enum_ChunkEventType m_ChunkEventType { get; private set; } = enum_ChunkEventType.Invalid;
    public TileBounds m_ChunkMapBounds { get; private set; }
    public TileAxis m_Size { get; private set; }
    public List<LevelChunkGame> m_NearbyChunks { get; private set; } = new List<LevelChunkGame>();
    public List<TileObjectBlockLift> m_RoadBlockTiles { get; private set; } = new List<TileObjectBlockLift>();
    Action OnChunkObjectDestroy;

    Transform m_RoadBlockParent;
    public override void Init()
    {
        base.Init();
        m_RoadBlockParent = transform.Find("RoadBlocks");
    }

    public void InitGameChunk(TileAxis mapOrigin, ChunkGenerateData _data, System.Random _random, Action OnChunkObjectDestroy, Action<enum_ChunkEventType, enum_TileObjectType, ChunkGameObjectData> OnGenerateObjects)
    {
        m_NearbyChunks.Clear();
        m_RoadBlockTiles.Clear();
        m_TilePool.m_ActiveItemDic.Traversal((LevelTileBase tile) => { tile.Clear(); });
        m_TilePool.Clear();

        m_ChunkIndex = _data.m_ChunkIndex;
        m_ChunkEventType = _data.m_EventType;
        gameObject.name = string.Format("{0}({1})", m_ChunkIndex, _data.m_Data.name);
        this.OnChunkObjectDestroy = OnChunkObjectDestroy;;
        transform.localPosition = _data.m_Origin.ToPosition();
        m_ChunkMapBounds = new TileBounds(_data.m_GenerateCheckBounds.m_Origin-mapOrigin, _data.m_GenerateCheckBounds.m_Size);
        InitData(_data.m_Data, _random, (TileAxis axis, ChunkTileData tileData) => {
            if (!tileData.m_ObjectType.IsEditorTileObject())
                return tileData;

            Vector3 worldPosition = transform.position+ axis.ToPosition() + tileData.m_ObjectType.GetSizeAxis(tileData.m_Direction).ToPosition() / 2f;
            OnGenerateObjects(m_ChunkEventType, tileData.m_ObjectType, new ChunkGameObjectData(worldPosition, tileData.m_Direction.ToRotation()));
            return tileData.ChangeObjectType(enum_TileObjectType.Invalid);
        });
        StaticBatchingUtility.Combine(m_TilePool.transform.gameObject);
    }

    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data, System.Random random)
    {
        base.OnTileInit(tile, axis, data, random);
        if (tile.m_Object)
            tile.m_Object.GameInit(GameExpression.GetLevelObjectHealth(tile.m_Object.m_ObjectType), OnChunkObjectDestroy);
        if (data.m_ObjectType == enum_TileObjectType.Block)
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
