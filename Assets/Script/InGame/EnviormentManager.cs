using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using UnityEngine.AI;
using System;
using LPWAsset;

public class EnviormentManager : SimpleSingletonMono<EnviormentManager> {
    public Transform tf_LevelParent { get; private set; }
    public enum_Style m_StyleCurrent { get; private set; } = enum_Style.Invalid;
    public static SBigmapLevelInfo m_currentLevel { get; private set; }
    public SBigmapLevelInfo[,] m_MapLevelInfo { get; private set; }
    public Light m_DirectionalLight { get; protected set; }
    protected NavMeshDataInstance m_NavMeshData;
    public System.Random m_mainSeed;
    public Action<SBigmapLevelInfo> OnLevelPrepared;
    protected override void Awake()
    {
        base.Awake();
        tf_LevelParent = transform.Find("LevelParent");
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
    }
    protected void Start()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Add(enum_BC_GameStatusChanged.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatusChanged>.Add(enum_BC_GameStatusChanged.OnLevelFinish, OnLevelFinished);
    }
    protected void OnDestroy()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Remove(enum_BC_GameStatusChanged.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove(enum_BC_GameStatusChanged.OnLevelFinish, OnLevelFinished);
    }
    public void GenerateAllEnviorment(enum_Style _LevelStyle,System.Random seed,Action<SBigmapLevelInfo> _OnLevelPrepared)
    {
        OnLevelPrepared = _OnLevelPrepared;
        m_mainSeed = seed;
        m_StyleCurrent = _LevelStyle;
        m_MapLevelInfo= GenerateBigmapLevels(m_StyleCurrent, m_mainSeed, tf_LevelParent,6,5,new TileAxis(2,2));
        StyleColorData[] customizations = TResources.GetAllStyleCustomization(_LevelStyle);
        StyleColorData randomData= customizations.Length == 0? StyleColorData.Default():customizations.RandomItem(m_mainSeed);
        randomData.DataInit(m_DirectionalLight);
        SetPostEffects(_LevelStyle);
    }
    public void SetPostEffects(enum_Style _levelStyle)
    {
        PostEffectManager.RemoveAllPostEffect();
        PostEffectManager.AddPostEffect<PE_BloomSpecific>().SetEffect(2, 10, 2);
        switch (_levelStyle)
        {
            case enum_Style.Undead:
               // PostEffectManager.AddPostEffect<PE_FogDepthNoise>().SetEffect(Color.white).SetEffect(TCommon.ColorAlpha(Color.white, .3f), .4f, -1f, 2f).SetTexture(TResources.Load<Texture>("Texture/Noise_Fog"));
                break;
        }
    }
    #region Level
    void OnStageStart()
    {
        m_currentLevel = m_MapLevelInfo.Find(p => p.m_TileType == enum_TileType.Start);
        OnLevelStart();
    }

    void OnLevelStart()     //Make Current Level Available (AI Bake)
    {
        ObjectManager.RecycleAllInteract(enum_Interaction.Interact_Portal);
        ObjectManager.RecycleAllLevelItem();
        m_currentLevel.StartLevel();
        NavMeshBuildSettings setting = NavMesh.GetSettingsByID(0);
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(m_currentLevel.m_Level.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, sources);
        Bounds bound = new Bounds(Vector3.zero, new Vector3(100, 20, 100));
        NavMeshData data = NavMeshBuilder.BuildNavMeshData(setting, sources, bound, Vector3.zero, Quaternion.identity);
        NavMesh.RemoveNavMeshData(m_NavMeshData);
        m_NavMeshData= NavMesh.AddNavMeshData(data);
        OnLevelPrepared(m_currentLevel);
        m_currentLevel.SetTileLocking(enum_TileLocking.Unlocked);
        TBroadCaster<enum_BC_UIStatusChanged>.Trigger(enum_BC_UIStatusChanged.PlayerLevelStatusChanged, m_MapLevelInfo, m_currentLevel.m_TileAxis);
    }

    void OnPortalEntered(enum_TileDirection toDirection)
    {
        Debug.Log("Stage End");
    }

    public void OnChangeLevel(TileAxis targetAxis)
    {
        if (m_currentLevel.m_TileAxis == targetAxis)
            return;

        m_currentLevel.m_Level.SetActivate(false);
        m_currentLevel = (m_MapLevelInfo.Get(targetAxis));
        OnLevelStart();
    }

    void OnLevelFinished()
    {
        foreach (enum_TileDirection direction in m_currentLevel.m_Connections.Keys)     //Set Connected Island Unlockable
        {
            if (m_MapLevelInfo.Get(m_currentLevel.m_Connections[direction])!=null)
                m_MapLevelInfo.Get(m_currentLevel.m_Connections[direction]).SetTileLocking(enum_TileLocking.Unlockable);
        }

        if (m_currentLevel.m_TileType == enum_TileType.End)        //Generate Portals For End IsLand
        {
            LevelTilePortal portal = m_currentLevel.m_Level.m_Portal;
            ObjectManager.SpawnInteract<InteractPortal>(enum_Interaction.Interact_Portal, portal.m_worldPos).InitPortal(portal.E_Direction, OnPortalEntered);
        }

        TBroadCaster<enum_BC_UIStatusChanged>.Trigger(enum_BC_UIStatusChanged.PlayerLevelStatusChanged, m_MapLevelInfo, m_currentLevel.m_TileAxis);
    }
    static NavMeshHit sampleHit;
    public static Vector3 NavMeshPosition(Vector3 samplePosition)
    {
        if (NavMesh.SamplePosition(samplePosition, out sampleHit, 20, -1))
            return sampleHit.position;
        return samplePosition;
    }
    #endregion
    #region BigMap
    public static SBigmapLevelInfo[,] GenerateBigmapLevels(enum_Style _levelStyle,System.Random bigMapSeed,Transform _generateParent,int _bigmapWidth, int _bigmapHeight,TileAxis startAxis)
    {
        //Generate Big Map All Tiles
        SBigmapTileInfo[,] bigmapTiles = new SBigmapTileInfo[_bigmapWidth, _bigmapHeight];
        for (int i = 0; i < _bigmapWidth; i++)
            for (int j = 0; j < _bigmapHeight; j++)
                bigmapTiles[i, j] = new  SBigmapTileInfo(new TileAxis(i, j), enum_TileType.Invalid,_levelStyle, enum_TileLocking.Locked);

        List<SBigmapTileInfo> mainRoadTiles = new List<SBigmapTileInfo>();
        List<SBigmapTileInfo> subGenerateTiles = new List<SBigmapTileInfo>();
        //Calculate Main Path
        int mainPathCount = bigMapSeed.Next(2) == 1 ? 5 : 6;
        mainRoadTiles =  bigmapTiles.TileRandomFill(bigMapSeed, startAxis,(SBigmapTileInfo tile)=> { tile.ResetTileType(enum_TileType.Battle); },p=>p.m_TileType== enum_TileType.Invalid, mainPathCount);
        mainRoadTiles[0].ResetTileType(enum_TileType.Start);
        mainRoadTiles[mainRoadTiles.Count-1].ResetTileType(enum_TileType.End);
        //Connect Main Path Tiles
        for (int i = 0; i < mainRoadTiles.Count - 1; i++)
            ConnectTile(mainRoadTiles[i], mainRoadTiles[i + 1]);

        //Generate Most Reward Island
        subGenerateTiles.AddRange(mainRoadTiles.GetRange(2, mainRoadTiles.Count - 3));
        if (subGenerateTiles.Count == 3)
        {
            subGenerateTiles[0].ResetTileType(enum_TileType.Reward);
            subGenerateTiles[2].ResetTileType(enum_TileType.Reward);
        }
        else
        {
            int rewardIndex = 0;
            subGenerateTiles.TraversalRandom( (SBigmapTileInfo tile) => {
                rewardIndex++;
                if (rewardIndex % 2 == 0)
                    tile.ResetTileType(enum_TileType.Reward);
                return false; }, bigMapSeed);
        }

        //Create Sub Battle Tile
        SBigmapTileInfo subBattleTile = null;
        subGenerateTiles.TraversalRandom( (SBigmapTileInfo tile)=> {
            TTiles.TTiles.m_AllDirections.TraversalRandom( (enum_TileDirection direction) =>
            {
                SBigmapTileInfo targetSubBattleTile = bigmapTiles.Get(tile.m_TileAxis.DirectionAxis(direction));
                if (targetSubBattleTile!=null&& targetSubBattleTile.m_TileType== enum_TileType.Invalid)
                {
                    subBattleTile = targetSubBattleTile;
                    subBattleTile.ResetTileType(enum_TileType.Battle);
                    subGenerateTiles.Add(subBattleTile);
                }
                return subBattleTile!=null;
            }, bigMapSeed);
            return subBattleTile!=null;
        }, bigMapSeed);

        //Connect Sub Battle Tile To All Tiles Nearby
        if (subBattleTile!=null)
        TTiles.TTiles.m_AllDirections.TraversalRandom( (enum_TileDirection direction) => {
            SBigmapTileInfo nearbyTile = bigmapTiles.Get(subBattleTile.m_TileAxis.DirectionAxis(direction));
            if (nearbyTile != null && (nearbyTile.m_TileType== enum_TileType.Reward|| nearbyTile.m_TileType == enum_TileType.Battle))
                ConnectTile(subBattleTile,nearbyTile);
            return false; }, bigMapSeed);


        //Generate Last Reward Tile
        subGenerateTiles.RemoveAll(p => p.m_TileType == enum_TileType.Reward);
        SBigmapTileInfo subRewardTile = null;
        subGenerateTiles.TraversalRandom( (SBigmapTileInfo tile) =>
        {
            TTiles.TTiles.m_AllDirections.TraversalRandom( (enum_TileDirection direction) =>
            {
                SBigmapTileInfo targetSubrewardTile = bigmapTiles.Get(tile.m_TileAxis.DirectionAxis(direction));
                if (targetSubrewardTile != null && targetSubrewardTile.m_TileType == enum_TileType.Invalid)
                {
                    subRewardTile = targetSubrewardTile;
                    subRewardTile.ResetTileType(enum_TileType.Reward);
                    ConnectTile(subRewardTile, tile);
                }
                return subRewardTile!=null;
            }, bigMapSeed);
            return subRewardTile != null;
        }, bigMapSeed);

        //Load All map Levels And Set Material
        LevelItemBase[] levelItemPrefabs = TResources.GetAllLevelItems(_levelStyle,null);
        Dictionary<LevelItemBase, int> maxItemCountDic = new Dictionary<LevelItemBase, int>();
        LevelBase levelPrefab = TResources.GetLevelPrefab();
        SBigmapLevelInfo[,] m_MapLevelInfo = new SBigmapLevelInfo[bigmapTiles.GetLength(0), bigmapTiles.GetLength(1)];      //Generate Bigmap Info
        for (int i = 0; i < _bigmapWidth; i++)
            for (int j = 0; j < _bigmapHeight; j++)
            {
                m_MapLevelInfo[i, j] = new SBigmapLevelInfo(bigmapTiles[i, j]);
                if (m_MapLevelInfo[i, j].m_TileType != enum_TileType.Invalid)
                {
                    Dictionary<LevelItemBase,int> itemCountDic=m_MapLevelInfo[i, j].GenerateMap(_generateParent, 10,10,levelPrefab, DataManager.GetItemGenerateProperties(m_MapLevelInfo[i,j].m_LevelStyle,m_MapLevelInfo[i,j].m_TileType.ToPrefabType()), levelItemPrefabs, bigMapSeed);
                    itemCountDic.Traversal((LevelItemBase item, int count) => {
                        if (!maxItemCountDic.ContainsKey(item))
                            maxItemCountDic.Add( item, 0);
                        if (maxItemCountDic[item] < count)
                            maxItemCountDic[item] = count;
                    });
                }
            }
        for (int i = 0; i < levelItemPrefabs.Length; i++)
        {
            if (!maxItemCountDic.ContainsKey(levelItemPrefabs[i]))
                GameObject.Destroy(levelItemPrefabs[i].gameObject);
        }
        ObjectManager.RegisterLevelItem(maxItemCountDic);
        return m_MapLevelInfo;
    }
    static void ConnectTile(SBigmapTileInfo tileStart,SBigmapTileInfo tileEnd)
    {
        enum_TileDirection directionConnection = tileStart.m_TileAxis.OffsetDirection(tileEnd.m_TileAxis);
        tileStart.m_Connections.Add(directionConnection, tileEnd.m_TileAxis);
        tileEnd.m_Connections.Add(directionConnection.DirectionInverse(), tileStart.m_TileAxis);
        
        if (tileEnd.m_TileType == enum_TileType.End)       //Add Special Place For Portal To Generate
            tileEnd.m_Connections.Add(directionConnection, new TileAxis(-1,-1));
    }
    #endregion
}
