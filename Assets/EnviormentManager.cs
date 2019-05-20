using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EnviormentManager : SimpleSingletonMono<EnviormentManager> {
    public Transform tf_PlayerStart { get; private set; }
    public Transform tf_LevelStart { get; private set; }
    public static LevelBase m_currentLevel { get; private set; }
    public static System.Random m_randomSeed { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        tf_PlayerStart = transform.Find("PlayerStart");
        tf_LevelStart = transform.Find("LevelStart");
    }
    public void StartLevel(enum_LevelType level,string seed="")
    {
        m_randomSeed = new System.Random(seed==""?System.DateTime.Now.GetHashCode():seed.GetHashCode());
        string levelName = level.ToString() + "1";
        m_currentLevel = TResources.Instantiate<LevelBase>("Level/Main/" + level.ToString()+"/"+levelName, tf_LevelStart);
        m_currentLevel.Init(GameExpression.S_GetLevelGenerateInfo(level),GetLevelData(m_currentLevel.m_LevelType,levelName));
    }
    public static TileMapData GetLevelData(enum_LevelType level, string name) => TResources.Load<TileMapData>("Level/Data/" + level + "/" + name);
}
