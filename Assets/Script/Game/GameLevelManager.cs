using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TTiles;
using UnityEngine.AI;
using System.Threading.Tasks;
using System.Threading;

public class GameLevelManager : SingletonMono<GameLevelManager>,ICoroutineHelperClass {
    public bool m_LevelTest = true;
    public enum_ChunkEventType m_TestEventType = enum_ChunkEventType.PerkAcquire;

    ObjectPoolListComponent<int, LevelChunkGame> m_ChunkPool;
    public Dictionary<int, LevelChunkGame> m_GameChunks => m_ChunkPool.m_ActiveItemDic;
    public Light m_DirectionalLight { get; private set; }
    public Texture2D m_MapTexture { get; private set; }
    public Texture2D m_FogTexture { get; private set; }
    public System.Random random { get; private set; }
    enum_ChunkRevealType[,] m_FogRevealation;
    public bool CheckTileRevealed(int i, int j) => m_FogRevealation[i, j] == enum_ChunkRevealType.Revealed;
    TileAxis m_MapOrigin, m_MapSize;
    Vector3 m_MapOriginPos;

    int m_chunkLifting = -1;
    TileAxis m_MinimapPrepos = TileAxis.Zero;
    bool m_MinimapUpdating = false;
    CancellationTokenSource m_TokenSource = new CancellationTokenSource();

    public Vector2 GetOffsetPosition(Vector3 worldPosition){
        Vector3 offset= (worldPosition - m_MapOriginPos) / LevelConst.I_TileSize;
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

    public void UpdateMinimap(Vector3 playerPosition)
    {
        Vector3 offset = playerPosition - m_MapOriginPos;
        TileAxis fogRevealPos = new TileAxis((int)offset.x/LevelConst.I_TileSize, (int)offset.z/LevelConst.I_TileSize );

        if (m_MinimapUpdating||m_MinimapPrepos == fogRevealPos)
            return;
        m_MinimapPrepos = fogRevealPos;
        StartCoroutine(UpdateMinimap(m_MinimapPrepos));
    }

    IEnumerator UpdateMinimap(TileAxis updatePos)
    {
        m_MinimapUpdating = true;
        yield return Task.Run(() => {
            for (int i = 0; i < m_MapSize.X; i++)
                for (int j = 0; j < m_MapSize.Y; j++)
                { 
                    if (m_FogRevealation[i, j]> enum_ChunkRevealType.PrepareEnd)
                        continue;

                    TileAxis offsetAxis = updatePos - new TileAxis(i, j);
                    if (offsetAxis.SqrLength > LevelConst.I_UIPlayerViewFadeSqrRange)
                        continue;

                    float sqrMagnitude = offsetAxis.SqrMagnitude;
                    if (sqrMagnitude <= LevelConst.I_UIPlayerViewRevealSqrRange)
                        m_FogRevealation[i, j] = enum_ChunkRevealType.PreRevealed;
                    else if (sqrMagnitude <= LevelConst.I_UIPlayerViewFadeSqrRange)
                        m_FogRevealation[i, j] =  enum_ChunkRevealType.PreFaded;
                }
        }, m_TokenSource.Token).TaskCoroutine();
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
                if (m_FogRevealation[i, j] > enum_ChunkRevealType.PrepareEnd||m_FogRevealation[i,j]== enum_ChunkRevealType.PreFog)
                    continue;

                m_FogTexture.SetPixel(i, j, m_FogRevealation[i, j] == enum_ChunkRevealType.PreFaded ? LevelConst.C_MapFogRevealFadeColor : LevelConst.C_MapFogRevealClearColor);
                if (m_FogRevealation[i, j] == enum_ChunkRevealType.PreRevealed)
                    m_FogRevealation[i, j] = enum_ChunkRevealType.Revealed;
            }
        m_FogTexture.Apply();
    }


    protected override void Awake()
    {
        base.Awake();
        m_ChunkPool = new ObjectPoolListComponent<int, LevelChunkGame>(transform.Find("Level"),"ChunkItem",(LevelChunkGame chunk)=> { chunk.Init(); });
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
        TBroadCaster<enum_BC_GameStatus>.Add<int>(enum_BC_GameStatus.OnBattleStart, OnBattleTrigger);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        OptionsManager.event_OptionChanged += OnOptionChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove<int>(enum_BC_GameStatus.OnBattleStart, OnBattleTrigger);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        NavigationManager.ClearNavMeshDatas();
        OptionsManager.event_OptionChanged -= OnOptionChanged;
    }
    void OnOptionChanged()
    {
        m_DirectionalLight.shadows = OptionsManager.m_OptionsData.m_ShadowOff? LightShadows.None: LightShadows.Hard;
    }
    #region Generate
    public IEnumerator Generate(enum_LevelStyle style, string seed, System.Random random)
    {
        m_MinimapPrepos = -TileAxis.One;
        if (m_MinimapUpdating)
        {
            m_TokenSource.Cancel();
            m_MinimapUpdating = false;
        }

        m_chunkLifting = -1;
        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        GameRenderData[] customizations = TResources.GetRenderData(style);
        GameRenderData randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem(random);
        randomData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);

        Dictionary<enum_ChunkType, List<LevelChunkData>> chunkDatas = TResources.GetChunkDatas();
        List<ChunkGenerateData> gameChunkGenerate = new List<ChunkGenerateData>();

        Func<enum_ChunkEventType> RandomEventType = () => TCommon.RandomPercentage(GameConst.D_ChunkEventPercentage,random);
        List<ChunkPreGenerateData> mainChunkType = m_LevelTest ? new List<ChunkPreGenerateData>() { new ChunkPreGenerateData(enum_ChunkType.Connect),new ChunkPreGenerateData( enum_ChunkType.Event, m_TestEventType), new ChunkPreGenerateData(enum_ChunkType.Teleport), new ChunkPreGenerateData(enum_ChunkType.Final) } :
            new List<ChunkPreGenerateData>()
            {
                new ChunkPreGenerateData( enum_ChunkType.Connect),new ChunkPreGenerateData(enum_ChunkType.Battle),new ChunkPreGenerateData( enum_ChunkType.Connect),  new ChunkPreGenerateData(enum_ChunkType.Event, enum_ChunkEventType.RewardChest),
                new ChunkPreGenerateData( enum_ChunkType.Connect),new ChunkPreGenerateData(enum_ChunkType.Battle),new ChunkPreGenerateData( enum_ChunkType.Teleport), new ChunkPreGenerateData(enum_ChunkType.Event, enum_ChunkEventType.Trader),
                new ChunkPreGenerateData( enum_ChunkType.Connect),new ChunkPreGenerateData(enum_ChunkType.Battle),new ChunkPreGenerateData( enum_ChunkType.Connect), new ChunkPreGenerateData(enum_ChunkType.Event, enum_ChunkEventType.Bonefire),
                new ChunkPreGenerateData( enum_ChunkType.Connect),new ChunkPreGenerateData(enum_ChunkType.Battle),new ChunkPreGenerateData( enum_ChunkType.Teleport), new ChunkPreGenerateData(enum_ChunkType.Event, RandomEventType()),
                new ChunkPreGenerateData( enum_ChunkType.Connect),new ChunkPreGenerateData(enum_ChunkType.Battle),new ChunkPreGenerateData( enum_ChunkType.Connect), new ChunkPreGenerateData(enum_ChunkType.Event, enum_ChunkEventType.Bonefire),
                new ChunkPreGenerateData( enum_ChunkType.Teleport),new ChunkPreGenerateData( enum_ChunkType.Final) };
        List<ChunkPreGenerateData> subChunkType = new List<ChunkPreGenerateData>() {
            new ChunkPreGenerateData(enum_ChunkType.Connect), new ChunkPreGenerateData(  enum_ChunkType.Battle),
            new ChunkPreGenerateData(enum_ChunkType.Teleport), new ChunkPreGenerateData(  enum_ChunkType.Event,RandomEventType()) };

        yield return Task.Run(() =>
        {
            short generateCount = 0;
            while (true)
            {
                generateCount++;
                if (generateCount > 2048)
                   throw new Exception("Generate Overtime!");

                gameChunkGenerate.Clear();
                List<LevelChunkData> dataGenerated = new List<LevelChunkData>();;
                Action<ChunkGenerateData, List<ChunkGenerateData>> ConnectGameData = (ChunkGenerateData startData, List<ChunkGenerateData> connectDatas) =>
                {
                    ChunkGenerateData preData = startData;
                    connectDatas.Traversal((ChunkGenerateData data) =>
                    {
                        dataGenerated.Add(data.m_Data);
                        preData.OnConnectionSet(data.m_PreChunkConnectPoint);
                        data.OnConnectionSet(data.m_ChunkConnectPoint);
                        preData = data;
                        gameChunkGenerate.Add(data);
                    });
                };

                //Generate Main Chunks
                gameChunkGenerate.Add(new ChunkGenerateData(gameChunkGenerate.Count, TileAxis.Zero, chunkDatas[enum_ChunkType.Start].RandomItem(random), enum_ChunkEventType.Invalid));

                //Gemerate Main Chunks
                List<ChunkGenerateData> mainConnectionChunks = null;
                if (! TryGenerateChunkDatas(gameChunkGenerate.Count, gameChunkGenerate[0], gameChunkGenerate, chunkDatas, dataGenerated, mainChunkType, random,out mainConnectionChunks))
                    continue;
                ConnectGameData(gameChunkGenerate[0], mainConnectionChunks);
                
                //Generate Sub Chunks
                if (m_LevelTest)
                    break;
                Func<int, bool> GenerateSubChunks = (int subCount) => {
                    ChunkGenerateData subStartChunk = null;
                    List<ChunkGenerateData> subConnectionChunks = null;
                    mainConnectionChunks.TraversalRandomBreak((ChunkGenerateData mainChunk) =>
                    {
                        if (!mainChunk.HaveEmptyConnection())
                            return false;
                        subStartChunk = mainChunk;
                        return TryGenerateChunkDatas(subCount * 1000, subStartChunk, gameChunkGenerate, chunkDatas,dataGenerated, subChunkType, random,out subConnectionChunks) ;
                    }, random);

                    if (subConnectionChunks==null||subConnectionChunks.Count==0)
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

        m_ChunkPool.ClearPool();
        gameChunkGenerate.Traversal((ChunkGenerateData data) => {
            LevelChunkGame curChunk = m_ChunkPool.AddItem(data.m_ChunkIndex);
            LevelChunkGame preChunk = m_ChunkPool.GetItem(data.m_PreChunkIndex);
            curChunk.InitGameChunk(data, random,NavigationManager.UpdateChunkData);
            preChunk.AddChunkConnection(curChunk);
            curChunk.AddChunkConnection(preChunk);
        });

        //GenerateNavigationData
        Bounds mapBounds = new Bounds();
        mapBounds.center = m_MapOriginPos + m_MapSize.ToPosition() / 2f + Vector3.up * LevelConst.I_TileSize;
        mapBounds.size = new Vector3(m_MapSize.X, .1f, m_MapSize.Y) * LevelConst.I_TileSize;
        Dictionary<int, NavigationManager.NavigationChunkDetail> _ChunkNavigationData = new Dictionary<int, NavigationManager.NavigationChunkDetail>();
        m_ChunkPool.m_ActiveItemDic.Traversal(( LevelChunkGame data) => { _ChunkNavigationData.Add(data.m_chunkIndex, new NavigationManager.NavigationChunkDetail(data.transform)); });
        NavigationManager.InitNavMeshData(transform, mapBounds, _ChunkNavigationData);

        #region Generate Map Texture
        m_MapTexture = new Texture2D(m_MapSize.X, m_MapSize.Y, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode= TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
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

                TileAxis tileAxis = (chunkdata.m_Axis - m_MapOrigin) + TileTools.GetAxisByIndex(index, chunkdata.m_Data.Width);
                m_MapTexture.SetPixel(tileAxis.X, tileAxis.Y, chunkColors[index]);

                List<TileAxis> axisRange = TileTools.GetAxisRange(m_MapSize.X,m_MapSize.Y,tileAxis,5);
                axisRange.Traversal((TileAxis axis) => {  m_FogRevealation[axis.X, axis.Y] = enum_ChunkRevealType.PreFog; });
            }
        });
        m_MapTexture.Apply();
        m_FogTexture.Apply();
        #endregion
    }

   bool TryGenerateChunkDatas(int generateStartIndex,ChunkGenerateData generateStartChunk,List<ChunkGenerateData> intersectsCheckChunks, Dictionary<enum_ChunkType, List<LevelChunkData>> chunkDatas,List<LevelChunkData> chunkDataAvoids,List<ChunkPreGenerateData> generateMainTypes,System.Random random, out List<ChunkGenerateData> chunkGenerateData)
    {
        chunkGenerateData = new List<ChunkGenerateData>();
        List<LevelChunkData> chunkAvoidCheck = new List<LevelChunkData>(chunkDataAvoids);
        List<ChunkGenerateData> chunkIntersectsCheckData = new List<ChunkGenerateData>(intersectsCheckChunks);
        ChunkGenerateData previousChunkGenerate = generateStartChunk;
        for (int i = 0; i < generateMainTypes.Count; i++)
        {
            bool isConnectingFinalChunk = i== generateMainTypes.Count - 1;
            ChunkGenerateData nextChunkGenerate = null;
            previousChunkGenerate.m_Data.Connections.TraversalRandomBreak((int previousConnectionIndex) =>
            {
                if (previousChunkGenerate.m_ConnectPoint[previousConnectionIndex])
                    return false;

                ChunkConnectionData m_previousConnectionData = previousChunkGenerate.m_Data.Connections[previousConnectionIndex];
                enum_TileDirection connectDirection = m_previousConnectionData.m_Direction.Inverse();
                chunkDatas[generateMainTypes[i].m_ChunkType].TraversalRandomBreak((LevelChunkData curChunkData) =>
                {
                    if (chunkAvoidCheck.Contains(curChunkData)|| !isConnectingFinalChunk && curChunkData.Connections.Length <= 1)
                        return false;

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
                            if (_anyGeneratedChunkIntersects)
                                return false;

                            nextChunkGenerate = new ChunkGenerateData(generateStartIndex, nextChunkAxis, curChunkData, generateMainTypes[i].m_EventType);
                            nextChunkGenerate.SetPreConnectData(previousChunkGenerate.m_ChunkIndex, previousConnectionIndex, curConnectionIndex);
                            generateStartIndex++;
                            return true;
                        }
                        return nextConnectionData != null;
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


    void OnBattleTrigger(int chunkID)
    {
        m_chunkLifting = chunkID;
        NavigationManager.UpdateChunkData(GetChunk(m_chunkLifting).BattleBlockLift(true));
    }

    void OnBattleFinish()
    {
        ;
        NavigationManager.UpdateChunkData(GetChunk(m_chunkLifting).BattleBlockLift(false));
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
    public class NavigationChunkDetail
    {
        public Transform m_Transform { get; private set; }
        public List<NavMeshBuildSource> m_Sources { get; private set; } = new List<NavMeshBuildSource>();
        public NavigationChunkDetail(Transform transform)
        {
            m_Transform = transform;
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

    public static void InitNavMeshData(Transform transform, Bounds bound,Dictionary<int, NavigationChunkDetail> chunkNavigationDatas)
    {
        ClearNavMeshDatas();
        m_Transform = transform;
        m_CombineBounds = bound;
        m_ChunkDetails = chunkNavigationDatas;
        ReCollectSources(null);
        m_CombinedData = NavMeshBuilder.BuildNavMeshData(m_BuildSettings, m_CombineSources, m_CombineBounds, m_Transform.position, m_Transform.rotation);
        m_CombinedSurfaceData = NavMesh.AddNavMeshData(m_CombinedData);
    }

    public static void UpdateChunkData(int index)
    {
        ReCollectSources(p => p == index);
        NavMeshBuilder.UpdateNavMeshData(m_CombinedData, m_BuildSettings, m_CombineSources, m_CombineBounds);
    }
    public static void UpdateChunkData(List<int> chunkIndexes)
    {
        ReCollectSources(p => chunkIndexes.Contains(p));
        NavMeshBuilder.UpdateNavMeshData(m_CombinedData, m_BuildSettings, m_CombineSources, m_CombineBounds);
    } 
    static void ReCollectSources(Predicate<int> OnWillUpdate=null)
    {
        m_ChunkDetails.Traversal((int index) => {
            if (OnWillUpdate!=null&&!OnWillUpdate(index))
                return;

            m_ChunkDetails[index].m_Sources.Clear();
            NavMeshBuilder.CollectSources(m_ChunkDetails[index].m_Transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, m_ChunkDetails[index].m_Sources);
        });
        m_CombineSources.Clear();
        m_ChunkDetails.Traversal((int index, NavigationChunkDetail detail) => { m_CombineSources.AddRange(detail.m_Sources); });
    }

    public static void ClearNavMeshDatas()
    {
        m_ChunkDetails.Clear();
        m_CombineSources.Clear();
        NavMesh.RemoveNavMeshData(m_CombinedSurfaceData);
    }

    public static Vector3 NavMeshPosition(Vector3 samplePosition)
    {
        if (NavMesh.SamplePosition(samplePosition, out sampleHit, 20,1>> 0))
            return sampleHit.position;
        return samplePosition;
    }
}