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
    public TileAxis m_QuadrantAxis { get; private set; }
    public TileBounds m_ChunkMapBounds { get; private set; }
    public Bounds m_ChunkBaseBounds { get; private set; }
    public Bounds m_ChunkCullBounds { get; private set; }
    public TileAxis m_Size { get; private set; }
    public List<LevelChunkGame> m_NearbyChunks { get; private set; } = new List<LevelChunkGame>();

    public Transform m_InteractParent { get; private set; }

    public override void OnInitItem()
    {
        base.OnInitItem();
        m_InteractParent = transform.Find("Interacts");
    }

    public void InitGameChunk( ChunkQuadrantData _data, System.Random _random)
    {
        m_NearbyChunks.Clear();
        m_QuadrantAxis = _data.m_QuadrantAxis;
        gameObject.name = m_Identity+"|"+ m_QuadrantAxis.ToString();
        transform.localPosition = _data.m_QuadrantBounds.m_Origin.ToPosition();
        m_ChunkMapBounds = _data.m_QuadrantBounds;
        Vector3 quadrantSource = m_ChunkMapBounds.m_Origin.ToPosition();
        Vector3 quadrantSize = m_ChunkMapBounds.m_Size.ToPosition() + Vector3.up * LevelConst.I_TileSize;
        m_ChunkBaseBounds = new Bounds(quadrantSource + quadrantSize / 2, quadrantSize);
        Vector3 xzOffset = (TileAxis.One * 2).ToPosition();
        Vector3 yoffset = Vector3.up*LevelConst.I_TileSize*2;
        m_ChunkCullBounds = new Bounds(m_ChunkBaseBounds.center+xzOffset/2,m_ChunkBaseBounds.size+xzOffset+yoffset);

        InitData(_data.m_QuadrantBounds.m_Size.X,_data.m_QuadrantBounds.m_Size.Y,_data.m_QuadrantDatas, _random, (TileAxis axis, ChunkTileData tileData) => {
            if (!tileData.m_ObjectType.IsEditorTileObject())
                return tileData;
            return tileData.ChangeObjectType(enum_TileObjectType.Invalid);
        });
        StaticBatchingUtility.Combine(m_TilePool.transform.gameObject);
    }
}
