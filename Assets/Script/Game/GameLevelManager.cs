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
    int m_CurrentLevel = -1;
    ObjectPoolListComponent<int, LevelChunkGame> m_GameChunks;
    public Light m_DirectionalLight { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        m_GameChunks = new ObjectPoolListComponent<int, LevelChunkGame>(transform.Find("GameChunks"), "ChunkItem", (LevelChunkGame chunk) => chunk.Init());
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
        m_CurrentLevel = -1;
        m_GameChunks.ClearPool();
        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        GameRenderData[] customizations = TResources.GetRenderData(style);
        GameRenderData randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem(random);
        randomData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);

        Dictionary<enum_LevelType, List<LevelChunkData>> chunkDatas = TResources.GetChunkDatas();
        levelGenerateData.Traversal((int index, GameLevelData data) => {
            LevelChunkGame _chunk = m_GameChunks.AddItem(index);
            _chunk.InitGameChunk(data.m_EventType, chunkDatas[data.m_ChunkType].RandomItem(random), random, NavigationManager.UpdateChunkData);
            _chunk.SetActivate(false);
        });
    }
    
    public void OnStartLevel(int chunkIndex,System.Random _random , Action<enum_ChunkEventType, enum_TileObjectType, ChunkGameObjectData> OnLevelObjectGenerate)
    {
        if(m_CurrentLevel!=-1)
            m_GameChunks.GetItem(m_CurrentLevel).SetActivate(false);

        m_CurrentLevel = chunkIndex;
        LevelChunkGame currentChunk = m_GameChunks.GetItem(m_CurrentLevel);
        currentChunk.SetActivate(true);
        Vector3 size = currentChunk.m_Size.ToPosition();
        NavigationManager.InitNavMeshData(currentChunk.transform, new Bounds(size / 2, new Vector3(size.x, .1f, size.z)));
        currentChunk.m_ChunkObjects.Traversal((enum_TileObjectType obejctType,List<ChunkGameObjectData> objectDatas)=> {
            objectDatas.Traversal((ChunkGameObjectData data) => { OnLevelObjectGenerate(currentChunk.m_EventType,obejctType,data); });
        });
    }
    void OnBattleStart()
    {
        m_GameChunks.GetItem(m_CurrentLevel).SetBlocksLift(true);
        NavigationManager.UpdateChunkData();
    }

    void OnBattleFinish()
    {
        m_GameChunks.GetItem(m_CurrentLevel).SetBlocksLift(false);
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

    public static void RecycleAll()
    {
        ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.RecycleAll();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.RecycleAll();
        ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.RecycleAll();
        ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.RecycleAll();
        ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.RecycleAll();
    }

    public static void Clear()
    {
        ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.DestroyAll();
        ObjectPoolManager<enum_TileObjectType, TileObjectBase>.DestroyAll();
        ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.DestroyAll();
        ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.DestroyAll();
        ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.DestroyAll();
    }
    
    public static bool HaveObjectItem(enum_TileObjectType type)=> ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Registed(type);
    public static bool HaveEdgeObjectItem(enum_TileEdgeObjectType type)=> ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Registed(type);

    public static TileObjectBase GetObjectItem(enum_TileObjectType type, Transform trans) => ObjectPoolManager<enum_TileObjectType, TileObjectBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TileTerrainBase GetTerrainItem(enum_TileTerrainType type, Transform trans) => ObjectPoolManager<enum_TileTerrainType, TileTerrainBase>.Spawn(type, trans,Vector3.zero,Quaternion.identity);
    public static TileEdgeObjectBase GetEdgeObjectItem(enum_TileEdgeObjectType type, Transform trans) => ObjectPoolManager<enum_TileEdgeObjectType, TileEdgeObjectBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static TilePlantsBase GetPlantsItem(enum_TilePlantsType type, Transform trans) => ObjectPoolManager<enum_TilePlantsType, TilePlantsBase>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
    public static LevelTileItemEditorTerrain GetEditorGroundItem(enum_EditorTerrainType type,Transform trans)=> ObjectPoolManager<enum_EditorTerrainType, LevelTileItemEditorTerrain>.Spawn(type, trans, Vector3.zero, Quaternion.identity);
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
        ReCollectSources();
        m_Data = NavMeshBuilder.BuildNavMeshData(m_BuildSettings, m_Sources, m_Bounds, m_Transform.position, m_Transform.rotation);
        m_DataInstance = NavMesh.AddNavMeshData(m_Data);
    }
    public static void UpdateChunkData()
    {
        ReCollectSources();
        NavMeshBuilder.UpdateNavMeshData(m_Data, m_BuildSettings, m_Sources, m_Bounds);
    }

    static void ReCollectSources()
    {
        m_Sources.Clear();
        List<NavMeshBuildSource> _originSources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(m_Transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, _originSources);
        _originSources.Traversal((NavMeshBuildSource source) => {
            if (source.component.gameObject.layer == GameLayer.I_Static)
                m_Sources.Add(source);
        });
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