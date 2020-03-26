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
    Dictionary<enum_ChunkType, List<LevelChunkData>> m_ChunkDatas;
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
    
    public void GenerateStage(enum_GameStyle style, System.Random random)
    {
        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        GameRenderData[] customizations = TResources.GetRenderData(style);
        GameRenderData randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem(random);
        randomData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);

        m_ChunkDatas= TResources.GetChunkDatas(); 
    }
    
    public void OnStartLevel(enum_ChunkType levelType,System.Random _random , Action<enum_TileObjectType, ChunkGameObjectData> OnLevelObjectGenerate)
    {
        m_GameChunk.InitGameChunk( m_ChunkDatas[levelType].RandomItem(_random),_random,NavigationManager.UpdateChunkData);
        Vector3 size = m_GameChunk.m_Size.ToPosition();
        NavigationManager.InitNavMeshData(m_GameChunk.transform, new Bounds(size / 2, new Vector3(size.x, .5f, size.z)));
        m_GameChunk.m_ChunkObjects.Traversal((enum_TileObjectType obejctType,List<ChunkGameObjectData> objectDatas)=> {
            objectDatas.Traversal((ChunkGameObjectData data) => { OnLevelObjectGenerate(obejctType,data); });
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