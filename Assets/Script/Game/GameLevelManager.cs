﻿using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TTiles;
using UnityEngine.AI;
using System.Threading.Tasks;
using System.Linq;

public class GameLevelManager : SingletonMono<GameLevelManager>, ICoroutineHelperClass
{
    ObjectPoolListComponent<int, LevelChunkGame> m_ChunkPool; 
    public Vector3 m_LevelCenter { get; private set; }
    public float m_LevelHeight { get; private set; }
    public float m_LevelWidth { get; private set; }

    public Texture2D m_MapTexture { get; private set; }
    public Texture2D m_FogTexture { get; private set; }

    enum_ChunkRevealType[,] m_FogRevealation;
    public bool CheckTileRevealed(int i, int j) => m_FogRevealation[i, j] == enum_ChunkRevealType.Revealed;
    TileAxis  m_MapSize;

    #region Get
    public Vector2 GetOffsetPosition(Vector3 worldPosition)
    {
        Vector3 offset = worldPosition / LevelConst.I_TileSize;
        return new Vector2(offset.x, offset.z);
    }

    public float GetMapAngle(float cameraYAngle) => cameraYAngle;
    public LevelChunkGame GetChunk(int chunkIndex)
    {
        if (m_ChunkPool.ContainsItem(chunkIndex))
            return m_ChunkPool.GetItem(chunkIndex);
        Debug.LogError("Chun Index Not Found" + chunkIndex);
        return null;
    }

    public TileAxis GetMapAxis(Vector3 worldPosition)
    {
        Vector3 offset = worldPosition / LevelConst.I_TileSize;
        return new TileAxis(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.z));
    }
    #endregion

    #region Quadrant
    protected TileAxis m_QuadrantRange, m_QuadrantSize;
    List<int> m_ActiveQuadrants=new List<int>();
    TileAxis m_PrePlayerMapAxis;
    TileAxis m_PrePlayerQuadrantAxis;
    bool m_MinimapUpdating = false;

    TileAxis GetQuadrantAxis(TileAxis playerAxis)
    {
        TileAxis quadrantAxis = playerAxis / m_QuadrantSize;
        if (quadrantAxis.X == m_QuadrantRange.X)
            quadrantAxis.X = m_QuadrantRange.X - 1;
        if (quadrantAxis.Y == m_QuadrantRange.Y)
            quadrantAxis.Y = m_QuadrantRange.Y - 1;
        return quadrantAxis;
    }

    void CheckActiveQuadrants(TileAxis playerQuadrant)
    {
        m_ActiveQuadrants.Clear();
        List<TileAxis> willActiveQuadrants = TileTools.GetAxisRange(m_QuadrantRange.X, m_QuadrantRange.Y, playerQuadrant - TileAxis.One, playerQuadrant + TileAxis.One);
        m_ChunkPool.m_ActiveItemDic.Traversal((LevelChunkGame chunk) =>
        {
            bool validChunk = willActiveQuadrants.Contains(chunk.m_QuadrantAxis);
            chunk.SetActivate(validChunk);
            if (!validChunk)
                return;
            m_ActiveQuadrants.Add(chunk.m_QuadrantIndex);
        });
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnQuadrantCheck, m_ActiveQuadrants);
    }

#if UNITY_EDITOR
    public bool m_DrawQuadrant = false;
    private void OnDrawGizmos()
    {
        if (m_DrawQuadrant && m_ChunkPool != null && m_ChunkPool.Count > 0)
        {
            m_ChunkPool.m_ActiveItemDic.Traversal((LevelChunkGame chunk) =>
            {
                bool playerAtQuadrant = m_PrePlayerQuadrantAxis == chunk.m_QuadrantAxis;
                bool activateQuadrant = m_ActiveQuadrants.Contains(chunk.m_QuadrantIndex);
                Gizmos.color = playerAtQuadrant ? Color.red :(activateQuadrant ? Color.yellow:Color.white);
                Vector3 quadrantSource = chunk.m_ChunkMapBounds.m_Origin.ToPosition();
                Vector3 size = chunk.m_ChunkMapBounds.m_Size.ToPosition() + Vector3.up * (playerAtQuadrant ? 2f : (activateQuadrant ? 1f : .5f));
                Gizmos.DrawWireCube(quadrantSource + size / 2, size);
            });
            Vector3 mapSize = m_MapSize.ToPosition();
            Gizmos.DrawWireCube(mapSize / 2, mapSize);
        }
    }
#endif
    #endregion
    #region Minimap
    public void CheckMinimapUpdate(TileAxis playerMapPos)
    {
        if (m_MinimapUpdating)
            return;
        StartCoroutine(UpdateMinimap(playerMapPos));
    }

    IEnumerator UpdateMinimap(TileAxis updatePos)
    {
        m_MinimapUpdating = true;
        yield return Task.Run(() =>
        {
            for (int i = 0; i < m_MapSize.X; i++)
                for (int j = 0; j < m_MapSize.Y; j++)
                {
                    if (m_FogRevealation[i, j] > enum_ChunkRevealType.PrepareEnd)
                        continue;

                    TileAxis offsetAxis = updatePos - new TileAxis(i, j);
                    if (offsetAxis.SqrLength > LevelConst.I_UIPlayerViewFadeSqrRange)
                        continue;

                    float sqrMagnitude = offsetAxis.SqrMagnitude;
                    if (sqrMagnitude <= LevelConst.I_UIPlayerViewRevealSqrRange)
                        m_FogRevealation[i, j] = enum_ChunkRevealType.PreRevealed;
                    else if (sqrMagnitude <= LevelConst.I_UIPlayerViewFadeSqrRange)
                        m_FogRevealation[i, j] = enum_ChunkRevealType.PreFaded;
                }
        }).TaskCoroutine();
        m_MinimapUpdating = false;

        OnFogmapPreparationFinish();
        yield break;
    }

    public void ClearAllFog()
    {
        for (int i = 0; i < m_MapSize.X; i++)
            for (int j = 0; j < m_MapSize.Y; j++)
            {
                if (m_FogRevealation[i, j] > enum_ChunkRevealType.PrepareEnd)
                    continue;
                m_FogRevealation[i, j] = enum_ChunkRevealType.PreRevealed;
            }
        OnFogmapPreparationFinish();
    }

    void OnFogmapPreparationFinish()
    {
        for (int i = 0; i < m_MapSize.X; i++)
            for (int j = 0; j < m_MapSize.Y; j++)
            {
                if (m_FogRevealation[i, j] > enum_ChunkRevealType.PrepareEnd || m_FogRevealation[i, j] == enum_ChunkRevealType.PreFog)
                    continue;

                m_FogTexture.SetPixel(i, j, m_FogRevealation[i, j] == enum_ChunkRevealType.PreFaded ? LevelConst.C_MapFogRevealFadeColor : LevelConst.C_MapFogRevealClearColor);
                if (m_FogRevealation[i, j] == enum_ChunkRevealType.PreRevealed)
                    m_FogRevealation[i, j] = enum_ChunkRevealType.Revealed;
            }
        m_FogTexture.Apply();
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_ChunkPool = new ObjectPoolListComponent<int, LevelChunkGame>(transform.Find("GameChunk"),"PoolItem",(LevelChunkGame chunk)=>chunk.Init());
    }

    public void TickGameLevel(Vector3 playerPosition)
    {
        TileAxis playerMapAxis = GetMapAxis(playerPosition);
        TileAxis playerAtQuadrant = GetQuadrantAxis(playerMapAxis);

        if (m_PrePlayerMapAxis != playerMapAxis)
        {
            m_PrePlayerMapAxis = playerMapAxis;
            CheckMinimapUpdate(m_PrePlayerMapAxis);
        }
        if (m_PrePlayerQuadrantAxis != playerAtQuadrant)
        {
            m_PrePlayerQuadrantAxis = playerAtQuadrant;
            CheckActiveQuadrants(m_PrePlayerQuadrantAxis);
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        NavigationManager.ClearNavMeshDatas();
    }
    
    public IEnumerator Generate(enum_GameStyle style, System.Random random,Action<List<ChunkGameObjectData>> OnGenerateEditorObjects)
    {
        m_PrePlayerMapAxis = -TileAxis.One;
        m_PrePlayerQuadrantAxis = -TileAxis.One;
        m_ActiveQuadrants.Clear();

        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        List<ChunkGenerateData> gameChunkGenerate = new List<ChunkGenerateData>();
        List<LevelChunkData> dataGenerateAvoid = new List<LevelChunkData>();
        List<enum_ObjectEventType> mainChunkType = new List<enum_ObjectEventType>() { enum_ObjectEventType.Normal, enum_ObjectEventType.Normal, enum_ObjectEventType.Final };
        List<enum_ObjectEventType> subChunkType = new List<enum_ObjectEventType>() { enum_ObjectEventType.Normal };

        LevelChunkData[] chunkDatas = TResources.GetChunkDatas();
        //Generate All Connected Chunks
        yield return Task.Run(() =>
        {
            short generateCount = 0;
            while (true)
            {
                generateCount++;
                if (generateCount > 10240)
                    throw new Exception("Generate Overtime!");

                gameChunkGenerate.Clear();
                dataGenerateAvoid.Clear();
                Action<ChunkGenerateData, List<ChunkGenerateData>> ConnectGameData = (ChunkGenerateData startData, List<ChunkGenerateData> connectDatas) =>
                {
                    ChunkGenerateData preData = startData;
                    connectDatas.Traversal((ChunkGenerateData data) =>
                    {
                        dataGenerateAvoid.Add(data.m_Data);
                        preData.OnConnectionSet(data.m_PreChunkConnectPoint);
                        data.OnConnectionSet(data.m_ChunkConnectPoint);
                        preData = data;
                        gameChunkGenerate.Add(data);
                    });
                };

                //Generate Main Chunks
                gameChunkGenerate.Add(new ChunkGenerateData(gameChunkGenerate.Count, TileAxis.Zero, chunkDatas.RandomItem(random), enum_ObjectEventType.Start));

                //Gemerate Main Chunks
                List<ChunkGenerateData> mainConnectionChunks = null;
                if (!TryGenerateChunkDatas(gameChunkGenerate.Count, gameChunkGenerate[0], gameChunkGenerate, chunkDatas, dataGenerateAvoid, mainChunkType, random, out mainConnectionChunks))
                    continue;
                ConnectGameData(gameChunkGenerate[0], mainConnectionChunks);
                //Generate Sub Chunks
                bool generateSuccessful=false;
                mainConnectionChunks.TraversalRandomBreak((ChunkGenerateData mainChunk) =>
                {
                    if (!mainChunk.HaveEmptyConnection())
                        return false;
                    List<ChunkGenerateData> subConnectionChunks = null;
                    if (TryGenerateChunkDatas(1000, mainChunk, gameChunkGenerate, chunkDatas, dataGenerateAvoid, subChunkType, random, out subConnectionChunks))
                    {
                        ConnectGameData(mainChunk, subConnectionChunks);
                        generateSuccessful = true;
                        return true;
                    }
                    return false;
                }, random);

                if (generateSuccessful)
                    break;
            }
        }).TaskCoroutine();

        //Generate Chunk Datas Map/Texture/Quadrant
        int originX = 0, originY = 0, oppositeX = 0, oppositeY = 0;
        gameChunkGenerate.Traversal((ChunkGenerateData chunkData) =>        //Check MapOrigin/MapSize
        {
            TileAxis chunkOrigin = chunkData.m_Origin;
            TileAxis chunkOpposite = chunkOrigin + chunkData.m_Data.m_Size;
            if (oppositeX < chunkOpposite.X)
                oppositeX = chunkOpposite.X;
            if (oppositeY < chunkOpposite.Y)
                oppositeY = chunkOpposite.Y;
            if (originX > chunkOrigin.X)
                originX = chunkOrigin.X;
            if (originY > chunkOrigin.Y)
                originY = chunkOrigin.Y;
        });
        TileAxis  _mapOrigin = new TileAxis(originX, originY);
        m_MapSize = new TileAxis(oppositeX - originX, oppositeY - originY);
        //Total Game Map
        TileGenerateData?[,] mapTileDatas = new TileGenerateData?[m_MapSize.X,m_MapSize.Y];
        gameChunkGenerate.Traversal((ChunkGenerateData generateData) =>
        {
            ChunkTileData[] chunkData = generateData.m_Data.GetData();
            for(int i=0;i<generateData.m_Data.Width;i++)
            {
                for (int j = 0; j < generateData.m_Data.Height;j++)
                {
                    TileAxis chunkDataAxis = new TileAxis(i, j);
                    TileAxis wholeDataAxis =   generateData.m_Origin-_mapOrigin+chunkDataAxis;
                    mapTileDatas[wholeDataAxis.X, wholeDataAxis.Y] =new TileGenerateData(generateData.m_EventType, chunkData[TileTools.Get1DAxisIndex(chunkDataAxis, generateData.m_Data.Width)]);
                }
            }
        });

        //Quadrant Generate
        List<ChunkGameObjectData> editorObjectDatas = new List<ChunkGameObjectData>();
        m_QuadrantRange = m_MapSize / LevelConst.I_QuadranteTileSize;
        m_QuadrantSize = new TileAxis(m_MapSize.X / m_QuadrantRange.X, m_MapSize.Y / m_QuadrantRange.Y);
        List<ChunkQuadrantData> _quadrantDatas = new List<ChunkQuadrantData>();
        TileAxis quadrantSizeEdge = new TileAxis(m_MapSize.X % m_QuadrantSize.X, m_MapSize.Y % m_QuadrantSize.Y);
        for (int i = 0; i < m_QuadrantRange.X; i++)
            for (int j = 0; j < m_QuadrantRange.Y; j++)
            {
                TileAxis quadrantAxis = new TileAxis(i, j);
                int quadrantIndex = TileTools.Get1DAxisIndex(quadrantAxis, m_QuadrantRange.X);
                TileAxis quadrantMapOrigin = quadrantAxis * m_QuadrantSize;
                TileAxis quadrantMapSize = m_QuadrantSize;
                if (i == m_QuadrantRange.X - 1)
                    quadrantMapSize.X += quadrantSizeEdge.X;
                if (j == m_QuadrantRange.Y - 1)
                    quadrantMapSize.Y += quadrantSizeEdge.Y;

                ChunkTileData[] quadrantDatas = new ChunkTileData[quadrantMapSize.X*quadrantMapSize.Y];
                bool validData = false;
                for (int x = 0; x < quadrantMapSize.X; x++)
                    for (int y = 0; y < quadrantMapSize.Y; y++)
                    {
                        TileAxis quadrantTileAxis = new TileAxis(x, y);
                        TileAxis mapDataAxis = quadrantMapOrigin+ quadrantTileAxis;
                        TileGenerateData? mapTileData = mapTileDatas[mapDataAxis.X, mapDataAxis.Y];
                        ChunkTileData quadrantTileData=ChunkTileData.Empty();
                        if (mapTileData.HasValue)
                        {
                            quadrantTileData = mapTileData.Value.m_Data;
                            if(quadrantTileData.m_ObjectType.IsEditorTileObject())
                            {
                                Vector3 worldPosition =  mapDataAxis.ToPosition() + quadrantTileData.m_ObjectType.GetSizeAxis(quadrantTileData.m_Direction).ToPosition() / 2f;
                                editorObjectDatas.Add(new ChunkGameObjectData(quadrantIndex, quadrantTileData.m_ObjectType,mapTileData.Value.m_eventType, worldPosition,quadrantTileData.m_Direction.ToRotation()));
                                quadrantTileData = quadrantTileData.ChangeObjectType(enum_TileObjectType.Invalid);
                            }
                            validData = true;
                        }

                        quadrantDatas[TileTools.Get1DAxisIndex(quadrantTileAxis, quadrantMapSize.X)] = quadrantTileData;
                    }

                if (!validData)
                    continue;
                _quadrantDatas.Add(new ChunkQuadrantData(quadrantIndex,quadrantAxis, new TileBounds(quadrantMapOrigin, quadrantMapSize), quadrantDatas));
            }

        OnGenerateEditorObjects(editorObjectDatas);

        Dictionary<int, NavigationQuadrants> _quadrantNavigations = new Dictionary<int, NavigationQuadrants>();

        m_ChunkPool.Clear();
        for(int i=0;i<_quadrantDatas.Count;i++)
        {
            ChunkQuadrantData quadrantData = _quadrantDatas[i];
            LevelChunkGame chunk = m_ChunkPool.AddItem(quadrantData.m_QuadrantIndex);
            chunk.InitGameChunk(quadrantData, random, NavigationManager.UpdateQuadrantData);
            _quadrantNavigations.Add(quadrantData.m_QuadrantIndex,new NavigationQuadrants(chunk));
            yield return null;
        }

        #region Navigation Data
        //GenerateNavigationData
        Bounds mapBounds = new Bounds();
        mapBounds.center = m_MapSize.ToPosition() / 2f ;
        mapBounds.size = new Vector3(m_MapSize.X, .1f, m_MapSize.Y) * LevelConst.I_TileSize;
        NavigationManager.InitNavMeshData(transform, mapBounds, _quadrantNavigations);
        #endregion
        #region Generate Map Texture
        m_MapTexture = new Texture2D(m_MapSize.X, m_MapSize.Y, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
        m_FogTexture = new Texture2D(m_MapSize.X, m_MapSize.Y, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
        m_FogRevealation = new enum_ChunkRevealType[m_MapSize.X, m_MapSize.Y];
        for (int i = 0; i < m_MapSize.X; i++)
            for (int j = 0; j < m_MapSize.Y; j++)
            {
                m_MapTexture.SetPixel(i, j, Color.clear);
                m_FogTexture.SetPixel(i, j, LevelConst.C_MapFogRevealFogColor);
                m_FogRevealation[i, j] = enum_ChunkRevealType.Empty;
            }

        gameChunkGenerate.Traversal((ChunkGenerateData chunkdata) =>
        {
            Color[] chunkColors = chunkdata.m_Data.CalculateMapTextureColors(false);
            int length = chunkColors.Length;
            for (int index = 0; index < length; index++)
            {
                if (chunkColors[index].a <= 0)
                    continue;

                TileAxis tileAxis = chunkdata.m_Origin-_mapOrigin + TileTools.GetAxisByIndex(index, chunkdata.m_Data.Width);
                m_MapTexture.SetPixel(tileAxis.X, tileAxis.Y, chunkColors[index]);

                List<TileAxis> axisRange = TileTools.GetAxisRange(m_MapSize.X, m_MapSize.Y, tileAxis, 5);
                axisRange.Traversal((TileAxis axis) => { m_FogRevealation[axis.X, axis.Y] = enum_ChunkRevealType.PreFog; });
            }
        });
        m_MapTexture.Apply();
        m_FogTexture.Apply();
        #endregion
    }

    bool TryGenerateChunkDatas(int generateStartIndex, ChunkGenerateData generateStartChunk, List<ChunkGenerateData> intersectsCheckChunks, LevelChunkData[] chunkDatas, List<LevelChunkData> chunkDataAvoids, List<enum_ObjectEventType> generateMainTypes, System.Random random, out List<ChunkGenerateData> chunkGenerateData)
    {
        chunkGenerateData = new List<ChunkGenerateData>();
        List<LevelChunkData> chunkAvoidCheck = new List<LevelChunkData>(chunkDataAvoids);
        List<ChunkGenerateData> chunkIntersectsCheckData = new List<ChunkGenerateData>(intersectsCheckChunks);
        ChunkGenerateData previousChunkGenerate = generateStartChunk;
        for (int i = 0; i < generateMainTypes.Count; i++)
        {
            bool isConnectingFinalChunk = i == generateMainTypes.Count - 1;
            ChunkGenerateData nextChunkGenerate = null;
            previousChunkGenerate.m_Data.Connections.TraversalRandomBreak((int previousConnectionIndex) =>
            {
                if (previousChunkGenerate.m_ConnectPoint[previousConnectionIndex])
                    return false;

                ChunkConnectionData m_previousConnectionData = previousChunkGenerate.m_Data.Connections[previousConnectionIndex];
                enum_TileDirection nextConnectDirection = m_previousConnectionData.m_Direction.Inverse();
                chunkDatas.TraversalRandomBreak((LevelChunkData chunkData) =>
                {
                    if (chunkAvoidCheck.Contains(chunkData) || !isConnectingFinalChunk && chunkData.Connections.Length <= 1)
                        return false;
                    
                    chunkData.Connections.TraversalRandomBreak((int curConnectionIndex) =>
                    {
                        ChunkConnectionData _connectionData = chunkData.Connections[curConnectionIndex];
                        if (_connectionData.m_Direction != nextConnectDirection)
                            return false;

                        TileAxis nextChunkAxis = previousChunkGenerate.m_Origin + m_previousConnectionData.m_Axis - _connectionData.m_Axis;
                        nextChunkAxis = TileTools.DirectionAxis(nextChunkAxis,m_previousConnectionData.m_Direction);
                        TileBounds nextChunkSizeCheck = new TileBounds(nextChunkAxis + TileAxis.One, chunkData.m_Size - TileAxis.One * 2);
                        if (chunkIntersectsCheckData.Any(p => p.m_GenerateCheckBounds.Intersects(nextChunkSizeCheck)))
                            return false;

                        nextChunkGenerate = new ChunkGenerateData(generateStartIndex, nextChunkAxis, chunkData, generateMainTypes[i]);
                        nextChunkGenerate.SetPreConnectData(previousChunkGenerate.m_ChunkIndex, previousConnectionIndex, curConnectionIndex);
                        generateStartIndex++;
                        return true;
                    }, random);
                    return nextChunkGenerate != null;
                }, random);
                return nextChunkGenerate != null;
            }, random);

            if (nextChunkGenerate == null)
            {
                chunkGenerateData.Clear();
                return false;
            }
            chunkAvoidCheck.Add(nextChunkGenerate.m_Data);
            chunkGenerateData.Add(nextChunkGenerate);
            chunkIntersectsCheckData.Add(nextChunkGenerate);
            previousChunkGenerate = nextChunkGenerate;
        }
        return true;
    }


}


public static class LevelObjectManager
{
    public static void Register(TileItemBase[] chunkTileItems)
    {
        Clear();
        Dictionary<enum_TileSubType, List<TileItemBase>> itemDic = new Dictionary<enum_TileSubType, List<TileItemBase>>();
        chunkTileItems.Traversal((TileItemBase item) => {
            if (!itemDic.ContainsKey(item.m_Type))
                itemDic.Add(item.m_Type, new List<TileItemBase>());
            itemDic[item.m_Type].Add(GameObject.Instantiate(item));
        });

        itemDic.Traversal((enum_TileSubType type, List<TileItemBase> items) => {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!"); break;
                case enum_TileSubType.Terrain:
                    items.Traversal((TileItemBase item) =>
                    {
                        TileTerrainBase groundItem = item as TileTerrainBase;
                        ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.Register(groundItem.m_GroundType, groundItem, 1);
                    });
                    break;
                case enum_TileSubType.Object:
                    items.Traversal((TileItemBase item) =>
                    {
                        TileObjectBase objectItem = item as TileObjectBase;
                        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Register(objectItem.m_ObjectType, objectItem, 1);
                    });
                    break;
                case enum_TileSubType.EdgeObject:
                    items.Traversal((TileItemBase item) =>
                    {
                        TileEdgeObjectBase editorGroundItem = item as TileEdgeObjectBase;
                        ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Register(editorGroundItem.m_EdgeObjectType, editorGroundItem, 1);
                    });
                    break;
                case enum_TileSubType.Plants:
                    items.Traversal((TileItemBase item) =>
                    {
                        TilePlantsBase plantsItem = item as TilePlantsBase;
                        ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.Register(plantsItem.m_PlantsType, plantsItem, 1);
                    });
                    break;
                case enum_TileSubType.EditorGround:
                    items.Traversal((TileItemBase item) =>
                    {
                        LevelTileItemEditorTerrain editorGroundItem = item as LevelTileItemEditorTerrain;
                        ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.Register(editorGroundItem.m_EditorTerrainType, editorGroundItem, 1);
                    });
                    break;
            }
        });
    }

    public static void DestroyBatchedItem()
    {
        ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.DestroyPoolItem();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.DestroyPoolItem();
        ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.DestroyPoolItem();
        ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.DestroyPoolItem();
        ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.DestroyPoolItem();
    }

    public static void Clear()
    {
        ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.Destroy();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Destroy();
        ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Destroy();
        ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.Destroy();
        ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.Destroy();
    }
    
    public static bool HaveObjectItem(enum_TileObjectType type)=> ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Registed(type);
    public static bool HaveEdgeObjectItem(enum_TileEdgeObjectType type)=> ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Registed(type);

    public static TileObjectBase GetObjectItem(enum_TileObjectType type, Transform trans) => ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TileTerrainBase GetTerrainItem(enum_TileTerrainType type, Transform trans) => ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.Spawn(type, trans,Vector3.zero,Quaternion.identity);
    public static TileEdgeObjectBase GetEdgeObjectItem(enum_TileEdgeObjectType type, Transform trans) => ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TilePlantsBase GetPlantsItem(enum_TilePlantsType type, Transform trans) => ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static LevelTileItemEditorTerrain GetEditorGroundItem(enum_EditorTerrainType type,Transform trans)=> ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
}

public class NavigationQuadrants
{
    public LevelChunkGame m_Chunk { get; private set; }
    public List<NavMeshBuildSource> m_Sources { get; private set; } = new List<NavMeshBuildSource>();
    public NavigationQuadrants(LevelChunkGame chunk)
    {
        m_Chunk = chunk;
    }

    public void RecollectSource()
    {
        m_Sources.Clear();
        NavMeshBuilder.CollectSources(m_Chunk.transform,GameLayer.Mask.I_Static, NavMeshCollectGeometry.PhysicsColliders,0,new List<NavMeshBuildMarkup>() { },m_Sources);
    }
}
public static class NavigationManager
{
    static Transform m_Transform;
    static NavMeshBuildSettings m_BuildSettings = NavMesh.GetSettingsByIndex(0);
    static NavMeshHit sampleHit;
    static NavMeshDataInstance m_DataInstance;
    static NavMeshData m_Data;
    static Bounds m_Bounds;
    static List<NavMeshBuildSource> m_Sources=new List<NavMeshBuildSource>();
    static Dictionary<int, NavigationQuadrants> m_Quadrants = new Dictionary<int, NavigationQuadrants>();
    public static void InitNavMeshData(Transform transform, Bounds bound,Dictionary<int,NavigationQuadrants> quadrants)
    {
        ClearNavMeshDatas();
        m_Transform = transform;
        m_Bounds = bound;
        m_Quadrants = quadrants;

        ReCollectSources();
        m_Data = NavMeshBuilder.BuildNavMeshData(m_BuildSettings, m_Sources, m_Bounds, m_Transform.position, m_Transform.rotation);
        m_DataInstance = NavMesh.AddNavMeshData(m_Data);
    }

    static void ReCollectSources(int quadrantIndex = -1)
    {
        m_Sources.Clear();
        m_Quadrants.Traversal((NavigationQuadrants quadrants) => { if (quadrantIndex == -1 || quadrantIndex == quadrants.m_Chunk.m_QuadrantIndex) quadrants.RecollectSource(); });
        m_Quadrants.Traversal((NavigationQuadrants quadrants) => { m_Sources.AddRange(quadrants.m_Sources); });
    }

    public static void UpdateQuadrantData(int quadrantID)
    {
        ReCollectSources(quadrantID);
        NavMeshBuilder.UpdateNavMeshData(m_Data, m_BuildSettings, m_Sources, m_Bounds);
    }



    public static void ClearNavMeshDatas()
    {
        m_Sources.Clear();
        NavMesh.RemoveNavMeshData(m_DataInstance);
    }

    public static Vector3 NavMeshPosition(Vector3 samplePosition)
    {
        if (NavMesh.SamplePosition(samplePosition, out sampleHit, 20,1>> 0))
            return sampleHit.position;
        return samplePosition;
    }
}