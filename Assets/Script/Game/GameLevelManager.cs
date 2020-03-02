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
    ObjectPoolListComponent<int, LevelChunkGame> m_ChunkPool;
    public Dictionary<int, LevelChunkGame> m_GameChunks => m_ChunkPool.m_ActiveItemDic;
    public Light m_DirectionalLight { get; private set; }
    public System.Random random { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        m_ChunkPool = new ObjectPoolListComponent<int, LevelChunkGame>(transform.Find("Level"), "ChunkItem", (LevelChunkGame chunk) => { chunk.Init(); });
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
        OptionsManager.event_OptionChanged += OnOptionChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        NavigationManager.ClearNavMeshDatas();
        OptionsManager.event_OptionChanged -= OnOptionChanged;
    }
    void OnOptionChanged()
    {
        m_DirectionalLight.shadows = OptionsManager.m_OptionsData.m_ShadowOff ? LightShadows.None : LightShadows.Hard;
    }
    
    public IEnumerator Generate(enum_LevelStyle style, string seed, System.Random random)
    {
        LevelObjectManager.Register(TResources.GetChunkTiles(style));
        GameRenderData[] customizations = TResources.GetRenderData(style);
        GameRenderData randomData = customizations.Length == 0 ? GameRenderData.Default() : customizations.RandomItem(random);
        randomData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);

        Dictionary<enum_ChunkType, List<LevelChunkData>> chunkDatas = TResources.GetChunkDatas();
        yield return null;
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