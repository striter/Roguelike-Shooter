﻿using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TTiles;
using UnityEngine.AI;
using System.Threading.Tasks;
public class GameLevelManager : SimpleSingletonMono<GameLevelManager> {
    ObjectPoolListComponent<int, LevelChunkGame> m_ChunkPool;
    public Dictionary<int, LevelChunkGame> m_GameChunks => m_ChunkPool.m_ActiveItemDic;
    public Light m_DirectionalLight { get; private set; }
    public Texture2D m_MapTexture { get; private set; }
    public System.Random random { get; private set; }
    TileAxis m_MapOrigin, m_MapSize;
    Vector3 m_MapOriginPos;

    int m_chunkLifting = -1;
    public Vector2 GetMapPosition(Vector3 worldPosition)
    {
        Vector3 offset =   worldPosition- m_MapOriginPos;
        return new Vector2(offset.x,offset.z)/LevelConst.I_TileSize;
    }

    protected override void Awake()
    {
        base.Awake();
        m_ChunkPool = new ObjectPoolListComponent<int, LevelChunkGame>(transform.Find("Level"),"ChunkItem",(LevelChunkGame chunk)=> { chunk.Init(); });
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
        TBroadCaster<enum_BC_GameStatus>.Add<int>(enum_BC_GameStatus.OnBattleStart, OnBattleTrigger);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove<int>(enum_BC_GameStatus.OnBattleStart, OnBattleTrigger);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        NavigationManager.ClearNavMeshDatas();
    }
    #region Generate
    public IEnumerator Generate(enum_LevelStyle style,string seed,System.Random random)
    {
        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        GameRenderData[] customizations = TResources.GetRenderData(style);
        GameRenderData randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem(random);
        randomData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);
        Dictionary<enum_ChunkType, List<LevelChunkData>> datas = TResources.GetChunkDatas();
        List<ChunkGenerateData> gameChunkGenerate=new List<ChunkGenerateData>();
        m_chunkLifting = -1;

        Action<ChunkGenerateData, List<ChunkGenerateData>> ConnectGameData = (ChunkGenerateData startData, List<ChunkGenerateData> connectDatas) =>
        {
            ChunkGenerateData dataTemp = startData;
            connectDatas.Traversal((ChunkGenerateData data) =>
            {
                dataTemp.OnConnectionSet(data.m_PreChunkConnectPoint);
                data.OnConnectionSet(data.m_ChunkConnectPoint);
                dataTemp = data;
                gameChunkGenerate.Add(data);
            });
        };

        yield return Task.Run(() =>
        {
            while(true)
            {
                gameChunkGenerate.Clear();
                //Generate Main Chunks
                gameChunkGenerate.Add(new ChunkGenerateData(gameChunkGenerate.Count, TileAxis.Zero, datas[enum_ChunkType.Start].RandomItem(random)));
                List<enum_ChunkType> mainChunkType = new List<enum_ChunkType>()
                    {  enum_ChunkType.Connection,enum_ChunkType.Battle, enum_ChunkType.Connection, enum_ChunkType.Battle, enum_ChunkType.Connection, enum_ChunkType.Event, enum_ChunkType.Connection, enum_ChunkType.Battle, enum_ChunkType.Connection, enum_ChunkType.Event, enum_ChunkType.Connection, enum_ChunkType.Battle, enum_ChunkType.Connection, enum_ChunkType.Event, enum_ChunkType.Connection, enum_ChunkType.Battle, enum_ChunkType.Connection, enum_ChunkType.Event, enum_ChunkType.Connection, enum_ChunkType.Battle, enum_ChunkType.Connection, enum_ChunkType.Event, enum_ChunkType.Connection, enum_ChunkType.Battle, enum_ChunkType.Connection, enum_ChunkType.Final };

                List<ChunkGenerateData> mainConnectionChunks = null;
                //Gemerate Main Chunks
                mainConnectionChunks = TryGenerateChunkDatas(gameChunkGenerate.Count, gameChunkGenerate[0], gameChunkGenerate, datas, mainChunkType, random);
                if (mainConnectionChunks == null)
                    continue;
                ConnectGameData(gameChunkGenerate[0],mainConnectionChunks);
                //Generate Sub Chunks
                List<enum_ChunkType> subChunkType = new List<enum_ChunkType>() { enum_ChunkType.Connection, enum_ChunkType.Battle, enum_ChunkType.Connection, enum_ChunkType.Event };
                Func<int,bool> GenerateSubChunks = (int subCount) => {

                    ChunkGenerateData subStartChunk = null;
                    List<ChunkGenerateData> subConnectionChunks = null;
                    mainConnectionChunks.TraversalRandomBreak((ChunkGenerateData mainChunk) =>
                    {
                        if (!mainChunk.HaveEmptyConnection())
                            return false;
                        subStartChunk = mainChunk;
                        subConnectionChunks = TryGenerateChunkDatas(subCount*1000, subStartChunk, gameChunkGenerate, datas, subChunkType, random);
                        return subConnectionChunks != null;
                    }, random);

                    if (subConnectionChunks == null)
                        return false;

                    ConnectGameData(subStartChunk, subConnectionChunks);
                    return true;
                };

                if (!GenerateSubChunks(1))
                    continue;
                if (!GenerateSubChunks(2))
                    continue;
                if (!GenerateSubChunks(3))
                    continue;

                break;
            }
        }).TaskCoroutine();

        //Set Map Data(Origin,Size,Texture)
        int originX = 0, originY = 0, oppositeX = 0, oppositeY = 0;
        gameChunkGenerate.Traversal((ChunkGenerateData chunkData) =>        //Check MapOrigin/MapSize
        {
            TileAxis chunkOrigin = chunkData.m_Axis;
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
        m_MapOrigin = new TileAxis(originX, originY);
        m_MapSize = new TileAxis(oppositeX - originX, oppositeY - originY);
        m_MapOriginPos = transform.TransformPoint(m_MapOrigin.ToPosition());
        //Generate Map Texture
        m_MapTexture = new Texture2D(m_MapSize.X, m_MapSize.Y, TextureFormat.RGBA32, false);
        m_MapTexture.filterMode = FilterMode.Point;
        gameChunkGenerate.Traversal((ChunkGenerateData chunkdata) =>
        {
            Color[] chunkColors = chunkdata.m_Data.CalculateMapTextureColors();
            int length = chunkColors.Length;
            for (int index = 0; index < length; index++)
            {
                TileAxis tileAxis = (chunkdata.m_Axis - m_MapOrigin) + TileTools.GetAxisByIndex(index, chunkdata.m_Data.Width);
                m_MapTexture.SetPixel(tileAxis.X, tileAxis.Y, chunkColors[index]);
            }
        });
        m_MapTexture.Apply();

        m_ChunkPool.ClearPool();
        gameChunkGenerate.Traversal((ChunkGenerateData data) => {
            LevelChunkGame curChunk = m_ChunkPool.AddItem(data.m_ChunkIndex);
            LevelChunkGame preChunk = m_ChunkPool.GetItem(data.m_PreChunkIndex);
            curChunk.InitGameChunk(data, random);
            preChunk.AddChunkConnection(curChunk);
            curChunk.AddChunkConnection(preChunk);
        });

        //GenerateNavigationData
        Bounds mapBounds = new Bounds();
        mapBounds.center = m_MapOriginPos + m_MapSize.ToPosition() / 2f + Vector3.up * LevelConst.I_TileSize;
        mapBounds.size = new Vector3(m_MapSize.X, .1f, m_MapSize.Y) * LevelConst.I_TileSize;
        Dictionary<int, ChunkNavigationData> _ChunkNavigationData = new Dictionary<int, ChunkNavigationData>();
        m_ChunkPool.m_ActiveItemDic.Traversal(( LevelChunkGame data) => { _ChunkNavigationData.Add(data.m_chunkIndex, new ChunkNavigationData(data.transform, data.m_ChunkBounds)); });
        NavigationManager.InitNavMeshData(transform, mapBounds, _ChunkNavigationData);
    }
    
    List<ChunkGenerateData> TryGenerateChunkDatas(int generateIndex,ChunkGenerateData generateStartChunk,List<ChunkGenerateData> intersectsCheckChunks, Dictionary<enum_ChunkType, List<LevelChunkData>> datas,List<enum_ChunkType> generateTypes,System.Random random)
    {
        List<ChunkGenerateData> chunkIntersectsCheckData = new List<ChunkGenerateData>(intersectsCheckChunks);
        List<ChunkGenerateData> chunkGenerateData = new List<ChunkGenerateData>();
        ChunkGenerateData previousChunkGenerate = generateStartChunk;
        for (int i = 0; i < generateTypes.Count; i++)
        {
            ChunkGenerateData nextChunkGenerate = null;
            previousChunkGenerate.m_Data.Connections.TraversalRandomBreak((int previousConnectionIndex) =>
            {
                if (previousChunkGenerate.m_ConnectPoint[previousConnectionIndex])
                    return false;

                ChunkConnectionData m_previousConnectionData = previousChunkGenerate.m_Data.Connections[previousConnectionIndex];
                enum_TileDirection connectDirection = m_previousConnectionData.m_Direction.Inverse();
                datas[generateTypes[i]].TraversalRandomBreak((LevelChunkData curChunkData) =>
                {
                    ChunkConnectionData? nextConnectionData = null;
                    curChunkData.Connections.TraversalRandomBreak((int curConnectionIndex) => {
                        ChunkConnectionData _connectionData = curChunkData.Connections[curConnectionIndex];
                        if (_connectionData.m_Direction == connectDirection)
                            nextConnectionData = _connectionData;

                        if (nextConnectionData.HasValue)
                        {
                            TileAxis nextChunkAxis = previousChunkGenerate.m_Axis + m_previousConnectionData.m_Axis - nextConnectionData.GetValueOrDefault().m_Axis;
                            bool _anyGeneratedChunkIntersects = false;
                            chunkIntersectsCheckData.TraversalBreak((ChunkGenerateData data) =>
                            {
                                _anyGeneratedChunkIntersects = CheckChunkIntersects(nextChunkAxis, curChunkData.m_Size, data.m_Axis, data.m_Data.m_Size,random);
                                return _anyGeneratedChunkIntersects;
                            });
                            if (!_anyGeneratedChunkIntersects)
                            {
                                nextChunkGenerate = new ChunkGenerateData(generateIndex++, nextChunkAxis, curChunkData);
                                nextChunkGenerate.SetPreConnectData(previousChunkGenerate.m_ChunkIndex,previousConnectionIndex,curConnectionIndex);
                            }
                        }
                        return nextConnectionData != null;
                    }, random);
                    return nextChunkGenerate != null;
                }, random);
                return nextChunkGenerate != null;
            }, random);

            if (nextChunkGenerate == null)
            {
                chunkGenerateData = null;
                break;
            }
            else
            {
                chunkGenerateData.Add(nextChunkGenerate);
                chunkIntersectsCheckData.Add(nextChunkGenerate);
                previousChunkGenerate = nextChunkGenerate;
            }
        }
        return chunkGenerateData;
    }

    bool CheckChunkIntersects(TileAxis s1origin,TileAxis s1size,TileAxis s2origin,TileAxis s2size,System.Random random)
    {
        //Edge Clip(All Edge Used For Connection)
        s1origin += TileAxis.One;
        s1size -= TileAxis.One*2;
        s2origin += TileAxis.One;
        s2size -= TileAxis.One*2;

        TileAxis[] square1Axises = new TileAxis[] { s1origin, s1origin + new TileAxis(s1size.X, 0), s1origin + new TileAxis(s1size.Y, 0),s1origin+s1size,s1origin+s1size/2 };
        TileAxis[] square2Axises = new TileAxis[] { s2origin, s2origin + new TileAxis(s2size.X, 0), s2origin + new TileAxis(s2size.Y, 0), s2origin + s2size,s2origin+s2size/2 };
        bool matched = false;
        square1Axises.TraversalRandomBreak((TileAxis s1Axis)=> {
            matched = TileTools.AxisInSquare(s1Axis, s2origin,s2size);
            return matched;
        },random);
        if (!matched)
        {
            square2Axises.TraversalRandomBreak((TileAxis s2Axis) => {
                matched = TileTools.AxisInSquare(s2Axis, s1origin, s1size);
                return matched;
            },random);
        }
        return matched;
    }
    #endregion

    public LevelChunkGame GetChunk(int chunkIndex)
    {
        if (m_ChunkPool.ContainsItem(chunkIndex))
            return m_ChunkPool.GetItem(chunkIndex);
        Debug.LogError("Chun Index Not Found" + chunkIndex);
        return null;
    }

    void OnBattleTrigger(int chunkID)
    {
        m_chunkLifting = chunkID;
        NavigationManager.UpdateNavMeshData(GetChunk(m_chunkLifting).BattleBlockLift(true));
    }

    void OnBattleFinish()
    {
        ;
        NavigationManager.UpdateNavMeshData(GetChunk(m_chunkLifting).BattleBlockLift(false));
        m_chunkLifting = -1;
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
                case enum_TileSubType.Ground:
                    items.Traversal((TileItemBase item) =>
                    {
                        TileGroundBase groundItem = item as TileGroundBase;
                        ObjectPoolManager<enum_TileGroundType, TileGroundBase>.Register(groundItem.m_GroundType, groundItem, 1);
                    });
                    break;
                case enum_TileSubType.Object:
                    items.Traversal((TileItemBase item) =>
                    {
                        TileObjectBase objectItem = item as TileObjectBase;
                        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Register(objectItem.m_ObjectType, objectItem, 1);
                    });
                    break;
                case enum_TileSubType.Pillar:
                    items.Traversal((TileItemBase item) =>
                    {
                        TilePillarBase pillarItem = item as TilePillarBase;
                        ObjectPoolManager<enum_TilePillarType, TilePillarBase>.Register(pillarItem.m_PillarType, pillarItem, 1);
                    });
                    break;
            }
        });
    }

    public static void Clear()
    {
        ObjectPoolManager<enum_TileGroundType, TileGroundBase>.DestroyAll();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.DestroyAll();
        ObjectPoolManager<enum_TilePillarType, TilePillarBase>.DestroyAll();
    }

    public static TilePillarBase GetPillarItem(enum_TilePillarType type, Transform trans) => ObjectPoolManager<enum_TilePillarType, TilePillarBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TileObjectBase GetObjectItem(enum_TileObjectType type, Transform trans) => ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TileGroundBase GetGroundItem(enum_TileGroundType type, Transform trans) => ObjectPoolManager<enum_TileGroundType, TileGroundBase>.Spawn(type, trans,Vector3.zero,Quaternion.identity);
}

public static class NavigationManager
{
    class NavigationChunkDetail
    {
        public Transform m_Transform { get; private set; }
        public Bounds m_Bounds { get; private set; }
        public List<NavMeshBuildSource> m_Sources { get; private set; } = new List<NavMeshBuildSource>();
        public NavigationChunkDetail(ChunkNavigationData data)
        {
            m_Transform = data.transform;
            m_Bounds = data.bounds;
        }
    }
    static Transform m_Transform;
    static NavMeshBuildSettings m_BuildSettings = NavMesh.GetSettingsByIndex(0);
    static NavMeshHit sampleHit;
    static NavMeshDataInstance m_CombinedSurfaceData;
    static NavMeshData m_CombinedData;
    static Dictionary<int, NavigationChunkDetail> m_ChunkDetails = new Dictionary<int, NavigationChunkDetail>();
    static Bounds m_CombineBounds;
    static List<NavMeshBuildSource> m_CombineSources=new List<NavMeshBuildSource>();
    static void CollectNavmeshSources()
    {
        m_CombineSources.Clear();
        m_ChunkDetails.Traversal((int index,NavigationChunkDetail detail) =>
        {
            m_CombineSources.AddRange(detail.m_Sources);
        });

    }

    public static void InitNavMeshData(Transform transform, Bounds bound,Dictionary<int,ChunkNavigationData> chunkNavigationDatas)
    {
        ClearNavMeshDatas();

        m_Transform = transform;
        m_CombineBounds = bound;
        chunkNavigationDatas.Traversal((int index,ChunkNavigationData data) => {
            m_ChunkDetails.Add(index,new NavigationChunkDetail(data));
            NavMeshBuilder.CollectSources(data.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, m_ChunkDetails[index].m_Sources);
        });

        CollectNavmeshSources();
        m_CombinedData = NavMeshBuilder.BuildNavMeshData(m_BuildSettings, m_CombineSources, m_CombineBounds, m_Transform.position, m_Transform.rotation);
        m_CombinedSurfaceData = NavMesh.AddNavMeshData(m_CombinedData);
    }
    
    public static void UpdateChunkData(int index)
    {
        m_ChunkDetails[index].m_Sources.Clear();
        NavMeshBuilder.CollectSources(m_ChunkDetails[index].m_Transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, m_ChunkDetails[index].m_Sources);
        UpdateNavMeshData();
    }
    public static void UpdateNavMeshData(List<int> chunkIndexes)
    {
        chunkIndexes.Traversal((int index) =>
        {
            m_ChunkDetails[index].m_Sources.Clear();
            NavMeshBuilder.CollectSources(m_ChunkDetails[index].m_Transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, m_ChunkDetails[index].m_Sources);
        });
        UpdateNavMeshData();
    }
    static void UpdateNavMeshData()
    {
        CollectNavmeshSources();
        NavMeshBuilder.UpdateNavMeshData(m_CombinedData, m_BuildSettings, m_CombineSources, m_CombineBounds);
    }

    public static void ClearNavMeshDatas()
    {
        m_ChunkDetails.Clear();
        m_CombineSources.Clear();
        NavMesh.RemoveNavMeshData(m_CombinedSurfaceData);
    }

    public static Vector3 NavMeshPosition(Vector3 samplePosition)
    {
        if (NavMesh.SamplePosition(samplePosition, out sampleHit, 20, 0))
            return sampleHit.position;
        return samplePosition;
    }
}