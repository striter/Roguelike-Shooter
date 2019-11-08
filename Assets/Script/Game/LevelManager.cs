using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using UnityEngine.AI;
using System;
using LPWAsset;

public class LevelManager : SimpleSingletonMono<LevelManager>,ISingleCoroutine {
    public Transform tf_LevelParent { get; private set; }
    public enum_Style m_StyleCurrent { get; private set; } = enum_Style.Invalid;
    public SBigmapLevelInfo m_currentLevel { get; private set; }
    public SBigmapLevelInfo[,] m_MapLevelInfo { get; private set; }
    public Light m_DirectionalLight { get; protected set; }
    public Transform m_InteractParent => m_currentLevel.m_Level.tf_Interact;
    public System.Random m_mainSeed;
    public Action<SBigmapLevelInfo> OnLevelPrepared;
    public Action OnStageFinished;
    protected override void Awake()
    {
        base.Awake();
        tf_LevelParent = transform.Find("LevelParent");
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
    }
    protected void Start()
    {
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        RemoveNavmeshData();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    public void GenerateAllEnviorment(enum_Style _LevelStyle,System.Random seed,Action<SBigmapLevelInfo> _OnLevelPrepared,Action _OnStageFinished,Action<SBigmapLevelInfo> _OnEachLevelGenerate)
    {
        OnLevelPrepared = _OnLevelPrepared;
        OnStageFinished = _OnStageFinished;
        m_mainSeed = seed;
        m_StyleCurrent = _LevelStyle;
        m_MapLevelInfo= GenerateBigmapLevels(m_StyleCurrent, m_mainSeed, tf_LevelParent,6,5, _OnEachLevelGenerate);
        StyleColorData[] customizations = TResources.GetAllStyleCustomization(_LevelStyle);
        StyleColorData randomData= customizations.Length == 0? StyleColorData.Default():customizations.RandomItem(m_mainSeed);
        randomData.DataInit(m_DirectionalLight);
        StaticBatchingUtility.Combine(tf_LevelParent.gameObject);
        m_MapLevelInfo.Traversal((SBigmapLevelInfo info) =>
        {
            info.SetLevelShow(false);
        });
    }


    #region Level
    void OnStageStart()
    {
        m_currentLevel = m_MapLevelInfo.Find(p => p.m_LevelType == enum_TileType.Start);
        PrepareCurrentLevel();
    }

    void PrepareCurrentLevel()     //Make Current Level Available (AI Bake)
    {
        m_currentLevel.SetLevelShow(true);
        BuildNavMeshData(m_currentLevel.m_Level);
        OnLevelPrepared(m_currentLevel);
        m_currentLevel.SetTileLocking(enum_TileLocking.Unlocked);
    }

    public void OnChangeLevel(TileAxis targetAxis)
    {
        if (m_currentLevel.m_TileAxis == targetAxis)
            return;

        m_currentLevel.SetLevelShow(false);
        m_currentLevel = (m_MapLevelInfo.Get(targetAxis));
        PrepareCurrentLevel();
    }

    void OnChangeLevel()
    {
        foreach (enum_TileDirection direction in m_currentLevel.m_Connections.Keys)     //Reveal Around Islands
        {
            SBigmapLevelInfo info = m_MapLevelInfo.Get(m_currentLevel.m_Connections[direction]);
            if (info != null && info.m_TileLocking == enum_TileLocking.Unseen)
                m_MapLevelInfo.Get(m_currentLevel.m_Connections[direction]).SetTileLocking(enum_TileLocking.Unlockable);
        }
    }
    void OnBattleFinish()
    {
        if(m_currentLevel.m_Level.m_levelType!= enum_TileType.End)
            m_currentLevel.SetTileLocking(enum_TileLocking.Locked);
    }
    #endregion
    #region BigMap
    public static SBigmapLevelInfo[,] GenerateBigmapLevels(enum_Style _levelStyle,System.Random _seed,Transform _generateParent,int _bigmapWidth, int _bigmapHeight,Action<SBigmapLevelInfo> _OnEachLevelGenerate)
    {
        //Generate Big Map All Tiles
        SBigmapTileInfo[,] bigmapTiles = new SBigmapTileInfo[_bigmapWidth, _bigmapHeight];
        for (int i = 0; i < _bigmapWidth; i++)
            for (int j = 0; j < _bigmapHeight; j++)
                bigmapTiles[i, j] = new  SBigmapTileInfo(new TileAxis(i, j), enum_TileType.Invalid, enum_TileLocking.Unseen);

        List<SBigmapTileInfo> mainRoadTiles = new List<SBigmapTileInfo>();
        List<SBigmapTileInfo> subGenerateTiles = new List<SBigmapTileInfo>();
        List<SBigmapTileInfo> rewardTiles = new List<SBigmapTileInfo>();
        //Calculate Main Path
        TileAxis startAxis = new TileAxis(_seed.Next(2, _bigmapWidth - 2), _seed.Next(2, _bigmapHeight - 2));
        int mainPathCount =  6;
        mainRoadTiles =  bigmapTiles.TileRandomFill(_seed, startAxis,(SBigmapTileInfo tile)=> { tile.ResetTileType(enum_TileType.Battle); },p=>p.m_LevelType== enum_TileType.Invalid, mainPathCount);
        mainRoadTiles[0].ResetTileType(enum_TileType.Start);
        mainRoadTiles[mainRoadTiles.Count-1].ResetTileType(enum_TileType.End);
        //Connect Main Path Tiles
        for (int i = 0; i < mainRoadTiles.Count - 1; i++)
            ConnectTile(mainRoadTiles[i], mainRoadTiles[i + 1]);

        //Generate Most Reward Island
        subGenerateTiles.AddRange(mainRoadTiles.GetRange(2, mainRoadTiles.Count - 3));
        if (subGenerateTiles.Count == 3)
        {
            rewardTiles.Add(subGenerateTiles[0]);
            rewardTiles.Add(subGenerateTiles[2]);
        }
        else
        {
            int rewardIndex = 0;
            subGenerateTiles.TraversalRandom( (SBigmapTileInfo tile) => {
                rewardIndex++;
                if (rewardIndex % 2 == 0)
                    rewardTiles.Add(tile);
                return false; }, _seed);
        }

        //Create Sub Battle Tile
        SBigmapTileInfo subBattleTile = null;
        subGenerateTiles.TraversalRandom( (SBigmapTileInfo tile)=> {
            TTiles.TTiles.m_FourDirections.TraversalRandom( (enum_TileDirection direction) =>
            {
                SBigmapTileInfo targetSubBattleTile = bigmapTiles.Get(tile.m_TileAxis.DirectionAxis(direction));
                if (targetSubBattleTile!=null&& targetSubBattleTile.m_LevelType== enum_TileType.Invalid)
                {
                    subBattleTile = targetSubBattleTile;
                    subBattleTile.ResetTileType(enum_TileType.Battle);
                    subGenerateTiles.Add(subBattleTile);
                }
                return subBattleTile!=null;
            }, _seed);
            return subBattleTile!=null;
        }, _seed);

        //Connect Sub Battle Tile To All Tiles Nearby
        if (subBattleTile!=null)
        TTiles.TTiles.m_FourDirections.TraversalRandom( (enum_TileDirection direction) => {
            SBigmapTileInfo nearbyTile = bigmapTiles.Get(subBattleTile.m_TileAxis.DirectionAxis(direction));
            if (nearbyTile != null && (nearbyTile.m_LevelType== enum_TileType.CoinsTrade|| nearbyTile.m_LevelType == enum_TileType.Battle))
                ConnectTile(subBattleTile,nearbyTile);
            return false; }, _seed);
        
        //Generate Last Reward Tile
        subGenerateTiles.RemoveAll(p => p.m_LevelType == enum_TileType.CoinsTrade);
        SBigmapTileInfo subRewardTile = null;
        subGenerateTiles.TraversalRandom( (SBigmapTileInfo tile) =>
        {
            TTiles.TTiles.m_FourDirections.TraversalRandom( (enum_TileDirection direction) =>
            {
                SBigmapTileInfo targetSubrewardTile = bigmapTiles.Get(tile.m_TileAxis.DirectionAxis(direction));
                if (targetSubrewardTile != null && targetSubrewardTile.m_LevelType == enum_TileType.Invalid)
                {
                    subRewardTile = targetSubrewardTile;
                    rewardTiles.Add(subRewardTile);
                    ConnectTile(subRewardTile, tile);
                }
                return subRewardTile!=null;
            }, _seed);
            return subRewardTile != null;
        }, _seed);

        //Set All Reward Tiles
        List<enum_TileType> rewardTypes = new List<enum_TileType>() { enum_TileType.ActionAdjustment, enum_TileType.BattleTrade, enum_TileType.CoinsTrade };
        for (int i = 0; i < rewardTiles.Count; i++)
        {
            if (rewardTypes.Count == 0)
            {
                Debug.LogError("Invalid Type Here,Use Trader By Default");
                rewardTiles[i].ResetTileType( enum_TileType.CoinsTrade);
                continue;
            }
            enum_TileType type = rewardTypes.RandomItem(_seed);
            rewardTypes.Remove(type);
            rewardTiles[i].ResetTileType(type);
        }

        //Load All map Levels And Set Material
        Dictionary<enum_LevelItemType,List<LevelItemBase>> levelItemPrefabs = GameObjectManager.RegisterLevelItem(_levelStyle);
        SBigmapLevelInfo[,] m_MapLevelInfo = new SBigmapLevelInfo[bigmapTiles.GetLength(0), bigmapTiles.GetLength(1)];      //Generate Bigmap Info
        for (int i = 0; i < _bigmapWidth; i++)
            for (int j = 0; j < _bigmapHeight; j++)
            {
                m_MapLevelInfo[i, j] = new SBigmapLevelInfo(bigmapTiles[i, j]);
                if (m_MapLevelInfo[i, j].m_LevelType != enum_TileType.Invalid)
                {
                    enum_LevelGenerateType generateType = m_MapLevelInfo[i, j].m_LevelType.ToPrefabType();
                    SLevelGenerate innerData = GameDataManager.GetItemGenerateProperties(_levelStyle, generateType, true);
                    SLevelGenerate outerData = GameDataManager.GetItemGenerateProperties(_levelStyle, generateType, false);

                    m_MapLevelInfo[i, j].GenerateMap(GameObjectManager.SpawnLevelPrefab(_generateParent), innerData, outerData, levelItemPrefabs, _seed);
                    _OnEachLevelGenerate(m_MapLevelInfo[i, j]);
                }
            }
        
        return m_MapLevelInfo;
    }
    static void ConnectTile(SBigmapTileInfo tileStart,SBigmapTileInfo tileEnd)
    {
        enum_TileDirection directionConnection = tileStart.m_TileAxis.OffsetDirection(tileEnd.m_TileAxis);
        tileStart.m_Connections.Add(directionConnection, tileEnd.m_TileAxis);
        tileEnd.m_Connections.Add(directionConnection.DirectionInverse(), tileStart.m_TileAxis);
        
        if (tileEnd.m_LevelType == enum_TileType.End)       //Add Special Place For Portal To Generate
            tileEnd.m_Connections.Add(directionConnection, new TileAxis(-1,-1));
    }
    #endregion
    #region Navigation
    protected NavMeshDataInstance m_NavMeshDataEntity, m_NavMeshDataInteract;
    static NavMeshHit sampleHit;
    public static Vector3 NavMeshPosition(Vector3 samplePosition, bool maskEntity = true)
    {
        if (NavMesh.SamplePosition(samplePosition, out sampleHit, 20, 1 << (maskEntity ? 0 : 3)))
            return sampleHit.position;
        return samplePosition;
    }

    void BuildNavMeshData(LevelBase itemSetLevel)
    {
        RemoveNavmeshData();

        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        Bounds bound = new Bounds(Vector3.zero, new Vector3(itemSetLevel.I_InnerHalfLength * 2-1, .2f, itemSetLevel.I_InnerHalfLength * 2-1));
        NavMeshBuilder.CollectSources(itemSetLevel.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, sources);
        m_NavMeshDataEntity = NavMesh.AddNavMeshData(NavMeshBuilder.BuildNavMeshData(NavMesh.GetSettingsByIndex(0), sources, bound, Vector3.zero, itemSetLevel.transform.rotation));

        NavMeshBuilder.CollectSources(itemSetLevel.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 3, new List<NavMeshBuildMarkup>() { }, sources);
        m_NavMeshDataInteract = NavMesh.AddNavMeshData(NavMeshBuilder.BuildNavMeshData(NavMesh.GetSettingsByIndex(1), sources, bound, Vector3.zero, itemSetLevel.transform.rotation));
    }

    void RemoveNavmeshData()
    {
        NavMesh.RemoveNavMeshData(m_NavMeshDataEntity);
        NavMesh.RemoveNavMeshData(m_NavMeshDataInteract);
    }
    #endregion
}
