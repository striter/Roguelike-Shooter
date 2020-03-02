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
    public TileBounds m_ChunkMapBounds { get; private set; }
    public enum_ChunkEventType m_ChunkEventType { get; private set; } = enum_ChunkEventType.Invalid;
    public List<LevelChunkGame> m_NearbyChunks { get; private set; } = new List<LevelChunkGame>();
    public List<TileObjectBlockLift> m_RoadBlockTiles { get; private set; } = new List<TileObjectBlockLift>();
    public Dictionary<enum_TileObjectType, List<ChunkGameObjectData>> m_ChunkObjects { get; private set; } = new Dictionary<enum_TileObjectType, List<ChunkGameObjectData>>();
    Action OnChunkObjectDestroy;

    public void InitGameChunk(TileAxis mapOrigin, ChunkGenerateData _data, System.Random _random,Action OnChunkObjectDestroy)
    {
        m_ChunkObjects.Clear();
        m_NearbyChunks.Clear();
        m_RoadBlockTiles.Clear();
        m_ChunkEventType = _data.m_EventType;
        gameObject.name =  _data.m_Data.name;
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

    protected override void OnTileInit(LevelTileBase tile, TileAxis axis, ChunkTileData data, System.Random random)
    {
        base.OnTileInit(tile, axis, data, random);
        if (tile.m_Object)
            tile.m_Object.GameInit(GameExpression.GetLevelObjectHealth(tile.m_Object.m_ObjectType), OnChunkObjectDestroy);
        if (data.m_ObjectType == enum_TileObjectType.Block)
            m_RoadBlockTiles.Add(tile.m_Object as TileObjectBlockLift);
    }

    void ShowTiles(bool show) => m_TilePool.transform.SetActivate(show);
    
    void SetBlocksLift(bool lift)
    {
        ShowTiles(true);
        m_RoadBlockTiles.Traversal((TileObjectBlockLift tile) => {
            tile.SetLift(lift);
        });
    }
}
