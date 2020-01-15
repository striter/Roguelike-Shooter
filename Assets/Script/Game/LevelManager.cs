using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using UnityEngine.AI;
using System;
using LPWAsset;
using System.Threading.Tasks;

public class LevelManager : SimpleSingletonMono<LevelManager> {
    public Transform tf_LevelParent { get; private set; }
    public enum_LevelStyle m_StyleCurrent { get; private set; } = enum_LevelStyle.Invalid;
    public SBigmapLevelInfo m_currentLevel => m_MapLevelInfo[m_currentLevelIndex];
    public SBigmapLevelInfo m_previousLevel => m_currentLevelIndex > 0? m_MapLevelInfo[m_currentLevelIndex - 1]:null;
    public int m_currentLevelIndex = -1;
    public List<SBigmapLevelInfo> m_MapLevelInfo { get; private set; } = new List<SBigmapLevelInfo>();
    public bool m_Loading { get; private set; }
    public Light m_DirectionalLight { get; protected set; }
    public Transform m_InteractParent => m_currentLevel.m_Level.tf_Interact;
    public System.Random m_mainSeed;
    protected override void Awake()
    {
        base.Awake();
        tf_LevelParent = transform.Find("LevelParent");
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        RemoveNavmeshData();
    }
    public IEnumerator GenerateLevel(enum_LevelStyle _LevelStyle, System.Random seed)
    {
        m_mainSeed = seed;
        m_StyleCurrent = _LevelStyle;
        m_Loading = true;
        StyleColorData[] customizations = TResources.GetAllStyleCustomization(_LevelStyle);
        StyleColorData randomData = customizations.Length == 0 ? StyleColorData.Default() : customizations.RandomItem(m_mainSeed);
        randomData.DataInit(m_DirectionalLight);

        Dictionary<enum_LevelItemType, List<LevelItemBase>> levelItemPrefabs = GameObjectManager.RegisterLevelItem(m_StyleCurrent);
        LevelBase levelPrefab = TResources.GetLevelBase(_LevelStyle);
        m_MapLevelInfo.Clear();
        List<enum_LevelType> randomLevels = GameExpression.GetRandomLevels(m_mainSeed);
        int count = randomLevels.Count;
        for (int i = 0; i < count; i++)
        {
            SBigmapLevelInfo levelInfo = new SBigmapLevelInfo(randomLevels[i]);
            m_MapLevelInfo.Add(levelInfo);
            if (levelInfo.m_LevelType != enum_LevelType.Invalid)
            {
                LevelBase _level = Instantiate(levelPrefab, tf_LevelParent);
                _level.Init();
                _level.transform.localRotation = Quaternion.Euler(0, seed.Next(360), 0);
                _level.transform.localPosition = Vector3.zero;
                _level.transform.localScale = Vector3.one;
                levelInfo.SetLevelShow(false);
                levelInfo.SetMap(_level);
            }
        }

        yield return Task.Run(()=> {
            m_MapLevelInfo.Traversal((SBigmapLevelInfo levelInfo) =>
            {
                levelInfo.m_Level.PrepareGenerateData(GameDataManager.GetLevelGenerateData(m_StyleCurrent, levelInfo.m_LevelType.ToPrefabType(), true), GameDataManager.GetLevelGenerateData(m_StyleCurrent, levelInfo.m_LevelType.ToPrefabType(), false), levelItemPrefabs, levelInfo.m_LevelType, m_mainSeed);
            });
        }).TaskCoroutine();
         
        m_MapLevelInfo.Traversal((SBigmapLevelInfo levelInfo) => { levelInfo.m_Level.Generate(OnLevelItemDestroyed); });
        m_Loading = false;
    }
    #region Level
    public void GameStart(Action<SBigmapLevelInfo> OnChangeLevelLoaded) => LoadLevel(0, OnChangeLevelLoaded);
    public void LoadNextLevel(Action<SBigmapLevelInfo> OnChangeLevelLoaded) =>LoadLevel(m_currentLevelIndex + 1, OnChangeLevelLoaded);
    void LoadLevel(int index, Action<SBigmapLevelInfo> OnChangeLevelLoaded)     //Make Current Level Available (AI Bake)
    {
        if(m_currentLevelIndex>=0)
            m_currentLevel.SetLevelShow(false);
        m_currentLevelIndex = index;
        m_currentLevel.SetLevelShow(true);
        BuildNavMeshData(m_currentLevel.m_Level);
        StaticBatchingUtility.Combine(m_currentLevel.m_Level.tf_LevelItem.gameObject);
        OnChangeLevelLoaded(m_currentLevel);
    }
    #endregion
    #region Navigation
    //Consider Use NavmeshComponents Instead(Download From Git)
    protected NavMeshDataInstance m_NavMeshDataEntity, m_NavMeshDataInteract;
    static NavMeshHit sampleHit;
    public static Vector3 NavMeshPosition(Vector3 samplePosition, bool maskEntity = true)
    {
        if (NavMesh.SamplePosition(samplePosition, out sampleHit, 20, 1 << (maskEntity ? 0 : 3)))
            return sampleHit.position;
        return samplePosition;
    }

    void BuildNavMeshData(LevelBase level)
    {
        RemoveNavmeshData();

        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        float border = level.m_InnerBorder;
        Bounds bound = new Bounds(Vector3.zero, new Vector3(border, .2f, border));

        NavMeshBuilder.CollectSources(level.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, sources);
        m_NavMeshDataEntity = NavMesh.AddNavMeshData(NavMeshBuilder.BuildNavMeshData(NavMesh.GetSettingsByIndex(0), sources, bound, Vector3.zero, level.transform.rotation));

        sources.Clear();
        NavMeshBuilder.CollectSources(level.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 3, new List<NavMeshBuildMarkup>() { }, sources);
        m_NavMeshDataInteract = NavMesh.AddNavMeshData(NavMeshBuilder.BuildNavMeshData(NavMesh.GetSettingsByIndex(1), sources, bound, Vector3.zero, level.transform.rotation));
    }

    void OnLevelItemDestroyed()
    {
        BuildNavMeshData(m_currentLevel.m_Level);
    }
    void RemoveNavmeshData()
    {
        NavMesh.RemoveNavMeshData(m_NavMeshDataEntity);
        NavMesh.RemoveNavMeshData(m_NavMeshDataInteract);
    }
    #endregion
}
