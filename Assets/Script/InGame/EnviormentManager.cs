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
    protected NavMeshData m_NavMeshData;
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
        m_MapLevelInfo= GenerateBigmapLevels(m_StyleCurrent,seed,tf_LevelParent,5,4);
    }
    #region Level
    void OnGameStart()
    {
        m_currentLevel = m_MapLevelInfo.Find(p => p.m_TileType == enum_BigmapTileType.Start);
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
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(m_currentLevel.m_Level.transform, -1, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>() { }, sources);
        Bounds bound = new Bounds(Vector3.zero, new Vector3(100, 20, 100));
        m_NavMeshData = NavMeshBuilder.BuildNavMeshData(setting, sources, bound, Vector3.zero, Quaternion.identity);
        NavMesh.AddNavMeshData(m_NavMeshData);
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

        Vector3 playerStartPos = m_currentLevel.m_Level.m_Portals.Find(p => p.E_Direction == toDirection.Inverse()).m_worldPos;
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelStart, playerStartPos);
    }

    #endregion
    #region BigMap
    public static SBigmapLevelInfo[,] GenerateBigmapLevels(enum_LevelStyle _levelStyle,string bigMapSeed,Transform _generateParent,int _bigmapWidth, int _bigmapHeight)
    {
        string seed = bigMapSeed == "" ? System.DateTime.Now.ToShortTimeString() : bigMapSeed;
        System.Random mainSeed = new System.Random(seed.GetHashCode());
        
        //Generate Big Map All Tiles
        SBigmapTileInfo[,] bigmapTiles = new SBigmapTileInfo[_bigmapWidth, _bigmapHeight];
        for (int i = 0; i < _bigmapWidth; i++)
            for (int j = 0; j < _bigmapHeight; j++)
                bigmapTiles[i, j] = new  SBigmapTileInfo(new TileAxis(i, j), enum_BigmapTileType.Invalid,_levelStyle);


        //Calculate Main Path And Connect
        List<SBigmapTileInfo> tilePath = new List<SBigmapTileInfo>();
        List<enum_TileDirection> directionOutcluded = new List<enum_TileDirection>();

        SBigmapTileInfo startTile = bigmapTiles.TileRandomEdge( mainSeed,p=>p.m_TileType== enum_BigmapTileType.Invalid, directionOutcluded);
        startTile.ResetTileType(enum_BigmapTileType.Start);
        SBigmapTileInfo endTile = bigmapTiles.TileRandomEdge(mainSeed, p => p.m_TileType == enum_BigmapTileType.Invalid&&p.m_TileAxis.AxisOffset(startTile.m_TileAxis)>4, directionOutcluded);
        endTile.ResetTileType(enum_BigmapTileType.BattleEnd);

        bigmapTiles.PathFindForClosestApproch(startTile, endTile, tilePath, (SBigmapTileInfo info) => { info.ResetTileType(enum_BigmapTileType.Battle); });    //Match The Path For Main Road
        ConnectPaths(tilePath);

        //Calculate Sub Path And Connect
        tilePath = tilePath.FindAll(p=>p.m_TileType== enum_BigmapTileType.Battle);  //Select Only Battle Island
        SBigmapTileInfo mainPathReward = tilePath.RandomItem(mainSeed);
        mainPathReward.ResetTileType(enum_BigmapTileType.Reward);
        SBigmapTileInfo subPathReward=bigmapTiles.TileRandomEdge(mainSeed, p => p.m_TileType == enum_BigmapTileType.Invalid && p.m_TileAxis.AxisOffset(mainPathReward.m_TileAxis) >1, directionOutcluded);
        subPathReward.ResetTileType(enum_BigmapTileType.Reward);
        tilePath.Clear();

        bigmapTiles.PathFindForClosestApproch(subPathReward, mainPathReward, tilePath,(SBigmapTileInfo tile)=> { tile.ResetTileType(enum_BigmapTileType.Battle); },p=>p.m_TileType== enum_BigmapTileType.Battle,p=>p.m_TileType!= enum_BigmapTileType.Invalid&&p.m_TileType!= enum_BigmapTileType.Battle);
        ConnectPaths(tilePath);

        //Load All map Levels
        Dictionary<enum_BigmapTileType, List<LevelBase>> levelPrefabDic = new Dictionary<enum_BigmapTileType, List<LevelBase>>();
        TResources.LoadAll<LevelBase>("Level/Main/" + _levelStyle).Traversal((LevelBase level)=> {
            if (!levelPrefabDic.ContainsKey(level.m_levelType))
                levelPrefabDic.Add(level.m_levelType, new List<LevelBase>());
            levelPrefabDic[level.m_levelType].Add(level);
                });

        LevelItemBase[] levelItemPrefabs = TResources.LoadAll<LevelItemBase>("Level/Item/" + _levelStyle);
        SBigmapLevelInfo[,] m_MapLevelInfo = new SBigmapLevelInfo[bigmapTiles.GetLength(0), bigmapTiles.GetLength(1)];
        for (int i = 0; i < _bigmapWidth; i++)
            for (int j = 0; j < _bigmapHeight; j++)
            {
                m_MapLevelInfo[i, j] = new SBigmapLevelInfo(bigmapTiles[i, j]);
                if (m_MapLevelInfo[i, j].m_TileType != enum_BigmapTileType.Invalid)
                {
                    m_MapLevelInfo[i, j].GenerateMap(_generateParent, levelPrefabDic[m_MapLevelInfo[i,j].m_TileType].RandomItem(mainSeed),levelItemPrefabs, seed + i + j,mainSeed);
                    m_MapLevelInfo[i, j].m_Level.SetActivate(false);
                }
            }
        return m_MapLevelInfo;
    }
    static void ConnectPaths(List<SBigmapTileInfo> _paths)
    {
        for (int i = 0; i < _paths.Count - 1; i++)
        {
            enum_TileDirection directionConnection = _paths[i].m_TileAxis.OffsetDirection(_paths[i + 1].m_TileAxis);
            _paths[i].m_Connections.Add(directionConnection, _paths[i + 1]);
            _paths[i + 1].m_Connections.Add(directionConnection.Inverse(), _paths[i]);

            if (_paths[i].m_TileType == enum_BigmapTileType.Start)      //Add Start Pos And End Pos For Portal
                _paths[i].m_Connections.Add(directionConnection.Inverse(),null);
            if (_paths[i + 1].m_TileType == enum_BigmapTileType.BattleEnd)
                _paths[i + 1].m_Connections.Add(directionConnection,null);
        }
    }
    #endregion

    #region Test
    public static Color BigmapTileColor(enum_BigmapTileType type)
    {
        switch (type)
        {
            default:
                return TCommon.ColorAlpha(Color.blue, .5f);
            case enum_BigmapTileType.Battle:
                return TCommon.ColorAlpha(Color.yellow, .5f);
            case enum_BigmapTileType.Reward:
                return TCommon.ColorAlpha(Color.green, .5f);
            case enum_BigmapTileType.Start:
                return TCommon.ColorAlpha(Color.grey, .5f);
            case enum_BigmapTileType.BattleEnd:
                return TCommon.ColorAlpha(Color.black, .5f);
        }
    }
    #endregion
}
