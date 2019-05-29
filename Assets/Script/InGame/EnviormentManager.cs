using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using UnityEngine.AI;

public class EnviormentManager : SimpleSingletonMono<EnviormentManager> {
    public Transform tf_LevelParent { get; private set; }
    public enum_LevelStyle m_StyleCurrent { get; private set; } = enum_LevelStyle.Invalid;
    public static SBigmapLevelInfo m_currentLevel { get; private set; }
    public SBigmapLevelInfo[,] m_MapLevelInfo { get; private set; }
    protected NavMeshDataInstance m_NavMeshData;
    protected override void Awake()
    {
        base.Awake();
        tf_LevelParent = transform.Find("LevelParent");
    }
    protected void Start()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Add(enum_BC_GameStatusChanged.OnGameStart, OnGameStart);
        TBroadCaster<enum_BC_GameStatusChanged>.Add(enum_BC_GameStatusChanged.OnLevelFinish, OnLevelFinished);
    }
    protected void OnDestroy()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Remove(enum_BC_GameStatusChanged.OnGameStart, OnGameStart);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove(enum_BC_GameStatusChanged.OnLevelFinish, OnLevelFinished);
    }
    public void GenerateEnviorment(enum_LevelStyle _LevelStyle,string seed="")
    {
        m_StyleCurrent = _LevelStyle;
        m_MapLevelInfo= GenerateBigmapLevels(m_StyleCurrent,seed,tf_LevelParent,6,5,new TileAxis(2,2));
    }
    #region Level
    void OnGameStart()
    {
        m_currentLevel = m_MapLevelInfo.Find(p => p.m_TileType == enum_LevelType.Start);
        TBroadCaster<enum_BC_UIStatusChanged>.Trigger(enum_BC_UIStatusChanged.PlayerLevelStatusChanged, m_MapLevelInfo, m_currentLevel.m_TileAxis);
        OnLevelStart();

        foreach (enum_TileDirection direction in m_currentLevel.m_Connections.Keys)     //Player Start For Start Level
        {
            if (m_currentLevel.m_Connections[direction] == null)
            {
                Vector3 playerStartPos = m_currentLevel.m_Level.m_Portals.Find(p => p.E_Direction == direction).m_worldPos;
                TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelStart, playerStartPos);
                break;
            }
        }
    }

    void OnLevelStart()     //Make Current Level Available (AI Bake)
    {
        ObjectManager.RecycleAllInteract(enum_Interact.Interact_Portal);
        m_currentLevel.m_Level.SetActivate(true);
        NavMeshBuildSettings setting = NavMesh.CreateSettings();
        setting.agentRadius = .5f;
        setting.agentClimb = .8f;
        setting.agentHeight = 2f;
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(m_currentLevel.m_Level.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, sources);
        Bounds bound = new Bounds(Vector3.zero, new Vector3(100, 20, 100));
        NavMeshData data = NavMeshBuilder.BuildNavMeshData(setting, sources, bound, Vector3.zero, Quaternion.identity);
        NavMesh.RemoveNavMeshData(m_NavMeshData);
        m_NavMeshData= NavMesh.AddNavMeshData(data);
    }

    void OnLevelFinished()
    {
        List<LevelTilePortal> portalPositions = m_currentLevel.m_Level.m_Portals;
        foreach (LevelTilePortal portal in portalPositions)
        {
            if (m_currentLevel.m_Connections[portal.E_Direction] != null)
            {
                SBigmapTileInfo tile = m_currentLevel.m_Connections[portal.E_Direction];
                ObjectManager.SpawnInteract<InteractPortal>(enum_Interact.Interact_Portal, portal.m_worldPos).InitPortal(portal.E_Direction,tile.m_TileType,OnPortalEntered);
            }
        }
    }

    void OnPortalEntered(enum_TileDirection toDirection)
    {
        m_currentLevel.m_Level.SetActivate(false);
        m_currentLevel = (m_MapLevelInfo.Get( m_currentLevel.m_Connections[toDirection].m_TileAxis));
        TBroadCaster<enum_BC_UIStatusChanged>.Trigger(enum_BC_UIStatusChanged.PlayerLevelStatusChanged, m_MapLevelInfo, m_currentLevel.m_TileAxis);
        OnLevelStart();

        Vector3 playerStartPos = m_currentLevel.m_Level.m_Portals.Find(p => p.E_Direction == toDirection.DirectionInverse()).m_worldPos;
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelStart, playerStartPos);
    }

    #endregion
    #region BigMap
    public static SBigmapLevelInfo[,] GenerateBigmapLevels(enum_LevelStyle _levelStyle,string bigMapSeed,Transform _generateParent,int _bigmapWidth, int _bigmapHeight,TileAxis startAxis)
    {
        string seed = bigMapSeed == "" ? System.DateTime.Now.ToShortTimeString() : bigMapSeed;
        System.Random mainSeed = new System.Random(seed.GetHashCode());
        
        //Generate Big Map All Tiles
        SBigmapTileInfo[,] bigmapTiles = new SBigmapTileInfo[_bigmapWidth, _bigmapHeight];
        for (int i = 0; i < _bigmapWidth; i++)
            for (int j = 0; j < _bigmapHeight; j++)
                bigmapTiles[i, j] = new  SBigmapTileInfo(new TileAxis(i, j), enum_LevelType.Invalid,_levelStyle);

        #region elderVersion
        ////Calculate Main Path And Connect
        //List<SBigmapTileInfo> tilePath = new List<SBigmapTileInfo>();
        //List<enum_TileDirection> directionOutcluded = new List<enum_TileDirection>();

        //SBigmapTileInfo startTile = bigmapTiles.TileEdgeRandom(mainSeed, p => p.m_TileType == enum_BigmapTileType.Invalid, directionOutcluded);
        //startTile.ResetTileType(enum_BigmapTileType.Start);
        //SBigmapTileInfo endTile = bigmapTiles.TileEdgeRandom(mainSeed, p => p.m_TileType == enum_BigmapTileType.Invalid && p.m_TileAxis.AxisOffset(startTile.m_TileAxis) > 4, directionOutcluded);
        //endTile.ResetTileType(enum_BigmapTileType.BattleEnd);

        //bigmapTiles.PathFindForClosestApproch(startTile, endTile, tilePath, (SBigmapTileInfo info) => { info.ResetTileType(enum_BigmapTileType.Battle); });    //Match The Path For Main Road
        //ConnectPaths(tilePath);

        ////Calculate Sub Path And Connect
        //tilePath = tilePath.FindAll(p => p.m_TileType == enum_BigmapTileType.Battle);  //Select Only Battle Island
        //SBigmapTileInfo mainPathReward = tilePath.RandomItem(mainSeed);
        //mainPathReward.ResetTileType(enum_BigmapTileType.Reward);
        //SBigmapTileInfo subPathReward = bigmapTiles.TileEdgeRandom(mainSeed, p => p.m_TileType == enum_BigmapTileType.Invalid && p.m_TileAxis.AxisOffset(mainPathReward.m_TileAxis) > 1, directionOutcluded);
        //subPathReward.ResetTileType(enum_BigmapTileType.Reward);
        //tilePath.Clear();

        //bigmapTiles.PathFindForClosestApproch(subPathReward, mainPathReward, tilePath, (SBigmapTileInfo tile) => { tile.ResetTileType(enum_BigmapTileType.Battle); }, p => p.m_TileType == enum_BigmapTileType.Battle, p => p.m_TileType != enum_BigmapTileType.Invalid && p.m_TileType != enum_BigmapTileType.Battle);
        //ConnectPaths(tilePath);
        #endregion

        List<SBigmapTileInfo> mainRoadTiles = new List<SBigmapTileInfo>();
        List<SBigmapTileInfo> subGenerateTiles = new List<SBigmapTileInfo>();
        //Calculate Main Path
        int mainPathCount = mainSeed.Next(2) == 1 ? 5 : 6;
        mainRoadTiles =  bigmapTiles.TileRandomFill(mainSeed,startAxis,(SBigmapTileInfo tile)=> { tile.ResetTileType(enum_LevelType.Battle); },p=>p.m_TileType== enum_LevelType.Invalid, mainPathCount);
        mainRoadTiles[0].ResetTileType(enum_LevelType.Start);
        mainRoadTiles[mainRoadTiles.Count-1].ResetTileType(enum_LevelType.BattleEnd);
        //Connect Main Path Tiles
        for (int i = 0; i < mainRoadTiles.Count - 1; i++)
            ConnectTile(mainRoadTiles[i], mainRoadTiles[i + 1]);

        //Generate Most Reward Island
        subGenerateTiles.AddRange(mainRoadTiles.GetRange(2, mainRoadTiles.Count - 3));
        if (subGenerateTiles.Count == 3)
        {
            subGenerateTiles[0].ResetTileType(enum_LevelType.Reward);
            subGenerateTiles[2].ResetTileType(enum_LevelType.Reward);
        }
        else
        {
            int rewardIndex = 0;
            subGenerateTiles.TraversalRandom(mainSeed,(SBigmapTileInfo tile) => {
                rewardIndex++;
                if (rewardIndex % 2 == 0)
                    tile.ResetTileType(enum_LevelType.Reward);
                return false; });
        }

        //Create Sub Battle Tile
        SBigmapTileInfo subBattleTile = null;
        subGenerateTiles.TraversalRandom(mainSeed,(SBigmapTileInfo tile)=> {
            TTiles.TTiles.m_AllDirections.TraversalRandom(mainSeed, (enum_TileDirection direction) =>
            {
                SBigmapTileInfo targetSubBattleTile = bigmapTiles.Get(tile.m_TileAxis.DirectionAxis(direction));
                if (targetSubBattleTile!=null&& targetSubBattleTile.m_TileType== enum_LevelType.Invalid)
                {
                    subBattleTile = targetSubBattleTile;
                    subBattleTile.ResetTileType(enum_LevelType.Battle);
                    subGenerateTiles.Add(subBattleTile);
                }
                return subBattleTile!=null;
            });
            return subBattleTile!=null;
        });

        //Connect Sub Battle Tile To All Tiles Nearby
        if (subBattleTile!=null)
        TTiles.TTiles.m_AllDirections.TraversalRandom(mainSeed, (enum_TileDirection direction) => {
            SBigmapTileInfo nearbyTile = bigmapTiles.Get(subBattleTile.m_TileAxis.DirectionAxis(direction));
            if (nearbyTile != null && (nearbyTile.m_TileType== enum_LevelType.Reward|| nearbyTile.m_TileType == enum_LevelType.Battle))
                ConnectTile(subBattleTile,nearbyTile);
            return false; });


        //Generate Last Reward Tile
        subGenerateTiles.RemoveAll(p => p.m_TileType == enum_LevelType.Reward);
        SBigmapTileInfo subRewardTile = null;
        subGenerateTiles.TraversalRandom(mainSeed, (SBigmapTileInfo tile) =>
        {
            TTiles.TTiles.m_AllDirections.TraversalRandom(mainSeed, (enum_TileDirection direction) =>
            {
                SBigmapTileInfo targetSubrewardTile = bigmapTiles.Get(tile.m_TileAxis.DirectionAxis(direction));
                if (targetSubrewardTile != null && targetSubrewardTile.m_TileType == enum_LevelType.Invalid)
                {
                    subRewardTile = targetSubrewardTile;
                    subRewardTile.ResetTileType(enum_LevelType.Reward);
                    ConnectTile(subRewardTile, tile);
                }
                return subRewardTile!=null;
            });
            return subRewardTile != null;
        });


        //Load All map Levels
        Dictionary<enum_LevelPrefabType, List<LevelBase>> levelPrefabDic = TResources.GetAllStyledLevels(_levelStyle);

        LevelItemBase[] levelItemPrefabs = TResources.GetAllLevelItems(_levelStyle);
        SBigmapLevelInfo[,] m_MapLevelInfo = new SBigmapLevelInfo[bigmapTiles.GetLength(0), bigmapTiles.GetLength(1)];
        for (int i = 0; i < _bigmapWidth; i++)
            for (int j = 0; j < _bigmapHeight; j++)
            {
                m_MapLevelInfo[i, j] = new SBigmapLevelInfo(bigmapTiles[i, j]);
                if (m_MapLevelInfo[i, j].m_TileType != enum_LevelType.Invalid)
                {
                    enum_LevelPrefabType prefabType = m_MapLevelInfo[i, j].m_TileType.ToPrefabType();   //Select Random Level From Dic
                    LevelBase level = levelPrefabDic[prefabType].RandomItem(mainSeed);
                    if(levelPrefabDic[prefabType].Count>1)
                       levelPrefabDic[prefabType].Remove(level);

                    m_MapLevelInfo[i, j].GenerateMap(_generateParent, level, levelItemPrefabs, seed + i + j,mainSeed);
                }
            }
        return m_MapLevelInfo;
    }
    static void ConnectTile(SBigmapTileInfo tileStart,SBigmapTileInfo tileEnd)
    {
        enum_TileDirection directionConnection = tileStart.m_TileAxis.OffsetDirection(tileEnd.m_TileAxis);
        tileStart.m_Connections.Add(directionConnection, tileEnd);
        tileEnd.m_Connections.Add(directionConnection.DirectionInverse(), tileStart);

        if (tileStart.m_TileType == enum_LevelType.Start)      //Add Start Pos And End Pos For Portal
            tileStart.m_Connections.Add(directionConnection.DirectionInverse(), null);
        if (tileEnd.m_TileType == enum_LevelType.BattleEnd)
            tileEnd.m_Connections.Add(directionConnection, null);
    }
    #endregion

    #region Test
    public static Color BigmapTileColor(enum_LevelType type)
    {
        switch (type)
        {
            default:
                return TCommon.ColorAlpha(Color.blue, .5f);
            case enum_LevelType.Battle:
                return TCommon.ColorAlpha(Color.red, .5f);
            case enum_LevelType.Reward:
                return TCommon.ColorAlpha(Color.green, .5f);
            case enum_LevelType.Start:
                return TCommon.ColorAlpha(Color.grey, .5f);
            case enum_LevelType.BattleEnd:
                return TCommon.ColorAlpha(Color.black, .5f);
        }
    }
    #endregion
}
