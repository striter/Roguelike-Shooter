using GameSetting;
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
    LevelChunkGame m_GameChunk;
    public Light m_DirectionalLight { get; private set; }
    protected List<ChunkGenerateData> m_ChunkDatas=new List<ChunkGenerateData>();
    protected override void Awake()
    {
        base.Awake();
        m_GameChunk = transform.Find("GameChunk").GetComponent<LevelChunkGame>();
        m_GameChunk.Init();
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
        OptionsManager.event_OptionChanged += OnOptionChanged;
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        NavigationManager.ClearNavMeshDatas();
        OptionsManager.event_OptionChanged -= OnOptionChanged;
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    void OnOptionChanged()
    {
        m_DirectionalLight.shadows = OptionsManager.m_OptionsData.m_ShadowOff ? LightShadows.None : LightShadows.Hard;
    }
    
    public void GenerateStage(enum_GameStyle style, List<GameLevelData> levelGenerateData, System.Random random)
    {
        m_ChunkDatas.Clear();
        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        GameRenderData[] customizations = TResources.GetRenderData(style);
        GameRenderData randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem(random);
        randomData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);

        Dictionary<enum_LevelType, List<LevelChunkData>> chunkDatas = TResources.GetChunkDatas();
        levelGenerateData.Traversal((GameLevelData data) => {
            m_ChunkDatas.Add(new ChunkGenerateData(chunkDatas[data.m_ChunkType].RandomItem(random),data.m_EventType));
        });
    }
    
    public void OnStartLevel(int chunkIndex,System.Random _random , Action<enum_ChunkEventType, enum_TileObjectType, ChunkGameObjectData> OnLevelObjectGenerate)
    {
        ChunkGenerateData _data = m_ChunkDatas[chunkIndex];
        Vector3 size = _data.m_Data.m_Size.ToPosition();
        m_GameChunk.InitGameChunk(_data, _random, NavigationManager.UpdateChunkData);
        NavigationManager.InitNavMeshData(m_GameChunk.transform, new Bounds(size / 2, new Vector3(size.x, LevelConst.I_TileSize, size.z)));

        m_GameChunk.m_ChunkObjects.Traversal((enum_TileObjectType obejctType,List<ChunkGameObjectData> objectDatas)=> {
            objectDatas.Traversal((ChunkGameObjectData data) => { OnLevelObjectGenerate(_data.m_EventType,obejctType,data); });
        });
    }
    void OnBattleStart()
    {
        m_GameChunk.SetBlocksLift(true);
        NavigationManager.UpdateChunkData();
    }

    void OnBattleFinish()
    {
        m_GameChunk.SetBlocksLift(false);
        NavigationManager.UpdateChunkData();
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
            }
        });
    }

    public static void RecycleAll()
    {
        ObjectPoolManager<enum_TileGroundType, TileGroundBase>.RecycleAll();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.RecycleAll();
    }

    public static void Clear()
    {
        ObjectPoolManager<enum_TileGroundType, TileGroundBase>.DestroyAll();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.DestroyAll();
    }
    
    public static TileObjectBase GetObjectItem(enum_TileObjectType type, Transform trans) => ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TileGroundBase GetGroundItem(enum_TileGroundType type, Transform trans) => ObjectPoolManager<enum_TileGroundType, TileGroundBase>.Spawn(type, trans,Vector3.zero,Quaternion.identity);
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

    public static void InitNavMeshData(Transform transform, Bounds bound)
    {
        ClearNavMeshDatas();
        m_Transform = transform;
        m_Bounds = bound;
        NavMeshBuilder.CollectSources(m_Transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, m_Sources);
        m_Data = NavMeshBuilder.BuildNavMeshData(m_BuildSettings, m_Sources, m_Bounds, m_Transform.position, m_Transform.rotation);
        m_DataInstance = NavMesh.AddNavMeshData(m_Data);
    }

    public static void UpdateChunkData()
    {
        NavMeshBuilder.CollectSources(m_Transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, m_Sources);
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