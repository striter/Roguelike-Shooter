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
    public Bounds m_ChunkBounds { get; private set; }
    public TileAxis m_Size { get; private set; }
    public List<LevelChunkGame> m_NearbyChunks { get; private set; } = new List<LevelChunkGame>();
    Action<int> OnChunkObjectDestroy;

    public Transform m_InteractParent { get; private set; }
    public override void Init()
    {
        base.Init();
        m_InteractParent = transform.Find("Interacts");
    }

    public void InitGameChunk( ChunkQuadrantData _data, System.Random _random, Action<int> OnChunkObjectDestroy)
    {
        m_NearbyChunks.Clear();
        m_QuadrantIndex = _data.m_QuadrantIndex;
        m_QuadrantAxis = _data.m_QuadrantAxis;
        gameObject.name = m_QuadrantIndex+"|"+ m_QuadrantAxis.ToString();
        this.OnChunkObjectDestroy = OnChunkObjectDestroy;;
        transform.localPosition = _data.m_QuadrantBounds.m_Origin.ToPosition();
        m_ChunkMapBounds = _data.m_QuadrantBounds;
        Vector3 quadrantSource = m_ChunkMapBounds.m_Origin.ToPosition();
        Vector3 quadrantSize = m_ChunkMapBounds.m_Size.ToPosition() + Vector3.up * LevelConst.I_TileSize*2;
        m_ChunkBounds = new Bounds(quadrantSource + quadrantSize / 2, quadrantSize);

        InitData(_data.m_QuadrantBounds.m_Size.X,_data.m_QuadrantBounds.m_Size.Y,_data.m_QuadrantDatas, _random, (TileAxis axis, ChunkTileData tileData) => {
            if (!tileData.m_ObjectType.IsEditorTileObject())
                return tileData;
            return tileData.ChangeObjectType(enum_TileObjectType.Invalid);
        });
        StaticBatchingUtility.Combine(m_TilePool.transform.gameObject);
    }

    void OnObjectDestroy() => OnChunkObjectDestroy(m_QuadrantIndex);
    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data, System.Random random)
    {
        base.OnTileInit(tile, axis, data, random);
        if (tile.m_Object)
            tile.m_Object.GameInit(GameExpression.GetLevelObjectHealth(tile.m_Object.m_ObjectType), OnObjectDestroy);
    }
}
