using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(AudioSource))]
public class GameAudioManager : AudioManager 
{
    public static new GameAudioManager Instance { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    Dictionary<enum_GameMusic, AudioClip> m_GameMusic = new Dictionary<enum_GameMusic, AudioClip>();

    public override void Init()
    {
        base.Init();
        TCommon.TraversalEnum((enum_GameMusic music) =>
        {
            AudioClip clip =  TResources.GetAudioClip_Background(music) ;
            if (clip) m_GameMusic.Add(music, clip);
        });
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageBeginLoad, OnStageBeginLoad);
        TBroadCaster<enum_BC_GameStatus>.Add<SBigmapLevelInfo>(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish); ;
        TBroadCaster<enum_BC_GameStatus>.Add<bool>(enum_BC_GameStatus.OnGameFinish, OnGameFinish);
    }
    public override void Recycle()
    {
        base.Recycle();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageBeginLoad, OnStageBeginLoad);
        TBroadCaster<enum_BC_GameStatus>.Remove<SBigmapLevelInfo>(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish); ;
        TBroadCaster<enum_BC_GameStatus>.Remove<bool>(enum_BC_GameStatus.OnGameFinish, OnGameFinish);
    }

    void OnStageBeginLoad()
    {

    }

    void OnChangeLevel(SBigmapLevelInfo info)
    {
        switch (info.m_LevelType)
        {
            case enum_TileType.Start:
            case enum_TileType.CoinsTrade:
            case enum_TileType.ActionAdjustment:
                PlayBGM(enum_GameMusic.Relax, true);
                break;
        }
    }

    void OnBattleStart()
    {
        switch (GameManager.Instance.m_GameLevel.m_LevelType)
        {
            case enum_TileType.ActionAdjustment:
            case enum_TileType.Battle:
                PlayBGM(enum_GameMusic.FightRelax, true);
                break;
            case enum_TileType.End:
                PlayBGM(enum_GameMusic.FightHard, true);
                break;
        }
    }
    void OnBattleFinish() =>  Stop();
    void OnGameFinish(bool win) => PlayBGM(win ? enum_GameMusic.GameWin : enum_GameMusic.GameLost, false);

    void PlayBGM(enum_GameMusic music, bool loop)
    {
        if (m_GameMusic.ContainsKey(music))
            SwitchBackground(m_GameMusic[music], loop);
        else
            Debug.LogWarning("None Music Found Of:" + music);
    }
    public void Stop() => StopBackground();
}
