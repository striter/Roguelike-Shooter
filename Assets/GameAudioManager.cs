using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(AudioSource))]
public class GameAudioManager : AudioManager
{
    protected static GameAudioManager ninstance;
    public static new GameAudioManager Instance => ninstance;
    static float m_volumeMultiply = 1f;
    public override float m_Volume => base.m_Volume * m_volumeMultiply;
    protected override void Awake()
    {
        base.Awake();
        ninstance = this;
    }
    private void Start()
    {
        OptionsManager.event_OptionChanged += OnOptionChanged;
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnGameStart, OnGameStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        OptionsManager.event_OptionChanged -= OnOptionChanged;
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnGameStart, OnGameStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish); ;
    }

    void OnGameStart()
    {
        Play(GameManagerBase.Instance.B_InGame, false);
    }

    void OnBattleStart()
    {
        Play(GameManagerBase.Instance.B_InGame, true);
    }

    void OnBattleFinish()
    {
        Play(GameManagerBase.Instance.B_InGame, false);
    }
    void Play(bool inGame,bool inBattle)
    {
        m_AudioBackground.clip = TResources.GetAudioClip_Background(inGame,inBattle);
        m_AudioBackground.Play();
    }
    void OnOptionChanged()
    {
        m_volumeMultiply = GameExpression.F_GameMusicVolume( OptionsManager.m_OptionsData.m_MusicVolumeTap);
    }
}
