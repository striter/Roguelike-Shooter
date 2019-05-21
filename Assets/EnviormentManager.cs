using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EnviormentManager : SimpleSingletonMono<EnviormentManager> {
    public Transform tf_PlayerStart { get; private set; }
    public Transform tf_LevelStart { get; private set; }
    public enum_LevelStyle m_StyleCurrent { get; private set; } = enum_LevelStyle.Invalid;
    public static LevelBase m_currentLevel { get; private set; }
    public SBigmapLevelInfo[,] m_MapLevelInfo;
    protected override void Awake()
    {
        base.Awake();
        tf_PlayerStart = transform.Find("PlayerStart");
        tf_LevelStart = transform.Find("LevelStart");
    }
    public void StartLevel(enum_LevelStyle _LevelStyle,string seed="")
    {
        System.Random mainSeed =  new System.Random(seed == "" ? System.DateTime.Now.GetHashCode():seed.GetHashCode()) ;
        m_StyleCurrent = _LevelStyle;
    }

    public void SetupBigMap()
    {
        enum_LevelType[,] bigMap = new enum_LevelType[3, 3] ;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                bigMap[i, j] = enum_LevelType.Invalid;



        m_MapLevelInfo = new SBigmapLevelInfo[3, 3];
        for (int i = 0; i < m_MapLevelInfo.GetLength(0); i++)
        {
            for (int j = 0; j < m_MapLevelInfo.GetLength(1); j++)
            {
                m_MapLevelInfo[i, j] = null;
            }
        }
    }

    public static TileMapData GetLevelData(enum_LevelStyle style, string name) => TResources.Load<TileMapData>("Level/Main/" + style + "/"  + name);

    public class SBigmapLevelInfo
    {
        protected Transform m_LevelParent;
        public string m_levelName { get; private set; } = "";
        public enum_LevelStyle m_LevelStyle =>m_currentLevel==null? enum_LevelStyle.Invalid: m_currentLevel.m_levelStyle;
        public enum_LevelType m_LevelType =>m_currentLevel==null? enum_LevelType.Invalid:m_currentLevel.m_levelType;
        public System.Random m_LevelSeed { get; private set; } = null;
        public List<SBigmapLevelInfo> m_Connects { get; private set; } = null;
        private LevelBase m_level;
        public LevelBase m_Level
        {
            get
            {
                if (m_level == null)
                {
                    m_currentLevel = TResources.Instantiate<LevelBase>("Level/Main/" + m_LevelStyle.ToString() + "/" + m_levelName, m_LevelParent);
                    m_currentLevel.Init(m_levelName,m_LevelSeed);
                }
                return m_level;
            }
        }
        public SBigmapLevelInfo(Transform _levelParent, string _levelName, System.Random _levelSeed)
        {
            m_LevelParent = _levelParent;
            m_levelName = _levelName;
            m_LevelSeed = _levelSeed;
        }
        public void SetupConnection(List<SBigmapLevelInfo> _connections)
        {
            m_Connects = _connections;
        }
    }
}
