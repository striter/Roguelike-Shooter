using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TTiles;
using UnityEngine.AI;
using System.Threading.Tasks;
public class GameLevelManager : SimpleSingletonMono<GameLevelManager> {
    public bool GamePlayTest = false;

    ObjectPoolSimpleComponent<int, LevelChunkBase> m_ChunkPool;
    public Dictionary<int, ChunkGameData> m_GameChunks { get; private set; } = new Dictionary<int, ChunkGameData>();
    public Light m_DirectionalLight { get; private set; }
    public Texture2D m_MapTexture { get; private set; }
    public System.Random random { get; private set; }
    TileAxis m_MapOrigin, m_MapSize;
    Vector3 m_MapOriginPos;
    public Vector2 GetMapPosition(Vector3 worldPosition)
    {
        Vector3 offset =   worldPosition- m_MapOriginPos;
        return new Vector2(offset.x,offset.z)/LevelConst.I_TileSize;
    }

    protected override void Awake()
    {
        base.Awake();
        m_ChunkPool = new ObjectPoolSimpleComponent<int, LevelChunkBase>(transform.Find("Level"),"ChunkItem",(LevelChunkBase chunk)=> { chunk.Init(); });
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        NavigationManager.ClearNavMeshDatas();
    }

    public IEnumerator Generate(enum_LevelStyle style,string seed,System.Random random)
    {
        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        GameRenderData[] customizations = TResources.GetRenderData(style);
        GameRenderData randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem(random);
        randomData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);
        Dictionary<enum_ChunkType, List<LevelChunkData>> datas = TResources.GetChunkDatas();
        List<ChunkGenerateData> gameChunkGenerate=null;

        for (; ; )
        {
            yield return Task.Run(() =>
            {
                try
                {
                    gameChunkGenerate = new List<ChunkGenerateData>();
                    //Generate First Chunk
                    gameChunkGenerate.Add(new ChunkGenerateData(TileAxis.Zero, datas[enum_ChunkType.Start].RandomItem(random)));
                    List<enum_ChunkType> mainChunkType = new List<enum_ChunkType>() { enum_ChunkType.Battle, enum_ChunkType.Event, enum_ChunkType.Battle, enum_ChunkType.Event, enum_ChunkType.Battle, enum_ChunkType.Event, enum_ChunkType.Battle, enum_ChunkType.Event, enum_ChunkType.Battle, enum_ChunkType.Event, enum_ChunkType.Battle };

                    List<ChunkGenerateData> mainChunkGenerate=null;
                    if (!GamePlayTest)
                    {
                        //Gemerate Main Chunks
                        mainChunkGenerate = TryGenerateChunkDatas(gameChunkGenerate[0], gameChunkGenerate, datas, mainChunkType, random);
                        gameChunkGenerate.AddRange(mainChunkGenerate);
                    }

                    //Generate Final Chunk
                    List<ChunkGenerateData> finalChunkGenerate = TryGenerateChunkDatas(gameChunkGenerate[gameChunkGenerate.Count - 1], gameChunkGenerate, datas, new List<enum_ChunkType>() { enum_ChunkType.Final }, random);
                    gameChunkGenerate.AddRange(finalChunkGenerate);

                    if (!GamePlayTest)
                    {
                        //Generate Sub Chunks
                        List<enum_ChunkType> subChunkType = new List<enum_ChunkType>() { enum_ChunkType.Battle, enum_ChunkType.Event };
                        for (int i = 0; i < 3; i++)
                            mainChunkGenerate.TraversalRandomBreak((ChunkGenerateData mainChunkData) =>
                            {
                                List<ChunkGenerateData> subGenerateData = null;
                                if (mainChunkData.CheckEmptyConnections(random))
                                    subGenerateData = TryGenerateChunkDatas(mainChunkData, gameChunkGenerate, datas, subChunkType, random);
                                if (subGenerateData != null)
                                    gameChunkGenerate.AddRange(subGenerateData);
                                return subGenerateData != null;
                            }, random);
                    }   

                }
                catch
                {
                    gameChunkGenerate.Clear();
                }
            }).TaskCoroutine();
           
            if(gameChunkGenerate.Count>0)
            {
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

                m_GameChunks.Clear();
                m_ChunkPool.ClearPool();
                gameChunkGenerate.Traversal((ChunkGenerateData data) => {
                    int chunkIndex = m_GameChunks.Count;
                    LevelChunkBase chunk = m_ChunkPool.AddItem(chunkIndex);
                    m_GameChunks.Add(chunkIndex,chunk.InitGameChunk(data, random));
                });


                //GenerateNavigationData
                Bounds mapBounds = new Bounds();
                mapBounds.center = m_MapOriginPos + m_MapSize.ToPosition() / 2f + Vector3.up * LevelConst.I_TileSize;
                mapBounds.size = new Vector3(m_MapSize.X, .1f, m_MapSize.Y) * LevelConst.I_TileSize;
                Dictionary<int,ChunkNavigationData> _ChunkNavigationData = new Dictionary<int, ChunkNavigationData>();
                int _ChunkFinalIndex = -1;
                m_GameChunks.Traversal((int chunkIndex, ChunkGameData data) =>{
                    _ChunkNavigationData.Add(chunkIndex, new ChunkNavigationData(data.m_ChunkBase.transform, data.m_ChunkBounds));
                    if (data.m_ChunkType== enum_ChunkType.Final) _ChunkFinalIndex=chunkIndex; });
                NavigationManager.InitNavMeshData(transform, mapBounds, _ChunkNavigationData, _ChunkFinalIndex);

                yield break;
            }
            yield return null;
        }
    }

    List<ChunkGenerateData> TryGenerateChunkDatas(ChunkGenerateData generateStartChunk,List<ChunkGenerateData> intersectsCheckChunks, Dictionary<enum_ChunkType, List<LevelChunkData>> datas,List<enum_ChunkType> generateTypes,System.Random random)
    {
        List<ChunkGenerateData> chunkIntersectsCheckData = new List<ChunkGenerateData>(intersectsCheckChunks);
        List<ChunkGenerateData> chunkGenerateData = new List<ChunkGenerateData>();
        ChunkGenerateData previousChunkGenerate = generateStartChunk;
        for (int i = 0; i < generateTypes.Count; i++)
        {
            ChunkGenerateData nextChunkGenerate = null;
            previousChunkGenerate.m_Data.Connections.TraversalRandomBreak((int previousConnectionIndex) =>
            {
                if (previousChunkGenerate.m_Connection[previousConnectionIndex]!= enum_ChunkConnectionType.Empty)
                    return false;

                ChunkConnectionData m_previousConnectionData = previousChunkGenerate.m_Data.Connections[previousConnectionIndex];
                enum_TileDirection connectDirection = m_previousConnectionData.m_Direction.Inverse();
                datas[generateTypes[i]].TraversalRandomBreak((LevelChunkData nextChunkData) =>
                {
                    ChunkConnectionData? nextConnectionData = null;
                    nextChunkData.Connections.TraversalRandomBreak((int nextConnectionIndex) => {
                        ChunkConnectionData _connectionData = nextChunkData.Connections[nextConnectionIndex];
                        if (_connectionData.m_Direction == connectDirection)
                            nextConnectionData = _connectionData;

                        if (nextConnectionData.HasValue)
                        {
                            TileAxis nextChunkAxis = previousChunkGenerate.m_Axis + m_previousConnectionData.m_Axis - nextConnectionData.GetValueOrDefault().m_Axis;
                            bool _anyGeneratedChunkIntersects = false;
                            chunkIntersectsCheckData.TraversalBreak((ChunkGenerateData data) =>
                            {
                                _anyGeneratedChunkIntersects = CheckChunkIntersects(nextChunkAxis, nextChunkData.m_Size, data.m_Axis, data.m_Data.m_Size,random);
                                return _anyGeneratedChunkIntersects;
                            });
                            if (!_anyGeneratedChunkIntersects)
                            {
                                nextChunkGenerate = new ChunkGenerateData(nextChunkAxis, nextChunkData);
                                previousChunkGenerate.OnConnectionSet(previousConnectionIndex, enum_ChunkConnectionType.Export);
                                nextChunkGenerate.OnConnectionSet(nextConnectionIndex, enum_ChunkConnectionType.Entrance);
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
    static NavMeshDataInstance m_ChunksSurfaceData,m_FinalSurfaceData;
    static NavMeshBuildSettings m_BuildSettings = NavMesh.GetSettingsByIndex(0);
    static NavMeshHit sampleHit;
    static Dictionary<int, NavigationChunkDetail> m_ChunkDetails = new Dictionary<int, NavigationChunkDetail>();
    static int m_FinalIndex = -1;
    static NavMeshData m_Data;
    static Bounds m_Bounds;
    static List<NavMeshBuildSource> m_Sources=new List<NavMeshBuildSource>();
    static void CalculateSourceBounds()
    {
        m_Sources.Clear();
        float verticalMin = 0;
        float verticalMax = 0;
        m_ChunkDetails.Traversal((NavigationChunkDetail data) => {
            m_Sources.AddRange(data.m_Sources);
            if (verticalMin > data.m_Transform.position.y)
                verticalMin = data.m_Transform.position.y;
            if (verticalMax < data.m_Transform.position.y)
                verticalMax = data.m_Transform.position.y;
        });
        m_Bounds.center = new Vector3(m_Bounds.center.x, LevelConst.I_TileSize + (verticalMax + verticalMin) / 2, m_Bounds.center.z);
        m_Bounds.size = new Vector3(m_Bounds.size.x, verticalMax - verticalMin + .1f, m_Bounds.size.z);
    }


    public static void InitNavMeshData(Transform transform, Bounds bound,Dictionary<int,ChunkNavigationData> chunkNavigationDatas,int finalIndex)
    {
        ClearNavMeshDatas();
        m_Bounds = bound;
        chunkNavigationDatas.Traversal((int index,ChunkNavigationData data) => {
            m_ChunkDetails.Add(index,new NavigationChunkDetail(data));
            NavMeshBuilder.CollectSources(data.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, m_ChunkDetails[index].m_Sources);
        });
        CalculateSourceBounds();
        m_Data = NavMeshBuilder.BuildNavMeshData(m_BuildSettings, m_Sources, m_Bounds, transform.position, transform.rotation);
        m_ChunksSurfaceData=NavMesh.AddNavMeshData(m_Data);
    }

    public static void UpdateChunkData(int index)
    {
        m_ChunkDetails[index].m_Sources.Clear();
        NavMeshBuilder.CollectSources(m_ChunkDetails[index].m_Transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, m_ChunkDetails[index].m_Sources);
        CalculateSourceBounds();
        NavMeshBuilder.UpdateNavMeshData(m_Data, m_BuildSettings, m_Sources, m_Bounds);
    }

    public static void ClearNavMeshDatas()
    {
        NavMesh.RemoveNavMeshData(m_ChunksSurfaceData);
    }

    public static Vector3 NavMeshPosition(Vector3 samplePosition)
    {
        if (NavMesh.SamplePosition(samplePosition, out sampleHit, 20, 0))
            return sampleHit.position;
        return samplePosition;
    }
}