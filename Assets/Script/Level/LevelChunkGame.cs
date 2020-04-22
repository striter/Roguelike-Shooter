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
    public int m_QuadrantIndex { get; private set; }
    public TileAxis m_QuadrantAxis { get; private set; }
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

    public void InitGameChunk( ChunkQuadrantData _data, System.Random _random, Action OnChunkObjectDestroy)
    {
        m_NearbyChunks.Clear();
        m_RoadBlockTiles.Clear();
        m_TilePool.m_ActiveItemDic.Traversal((LevelTileBase tile) => { tile.Clear(); });
        m_TilePool.Clear();

        m_QuadrantIndex = _data.m_QuadrantIndex;
        m_QuadrantAxis = _data.m_QuadrantAxis;
        gameObject.name = m_QuadrantIndex+"|"+ m_QuadrantAxis.ToString();
        this.OnChunkObjectDestroy = OnChunkObjectDestroy;;
        transform.localPosition = _data.m_QuadrantBounds.m_Origin.ToPosition();
        m_ChunkMapBounds = _data.m_QuadrantBounds;
        InitData(_data.m_QuadrantBounds.m_Size.X,_data.m_QuadrantBounds.m_Size.Y,_data.m_QuadrantDatas, _random, (TileAxis axis, ChunkTileData tileData) => {
            if (!tileData.m_ObjectType.IsEditorTileObject())
                return tileData;
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
