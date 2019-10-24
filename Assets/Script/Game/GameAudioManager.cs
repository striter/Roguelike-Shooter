﻿using System.Collections;
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
    Dictionary<bool, AudioClip> m_Clips = new Dictionary<bool, AudioClip>();
    protected override void Awake()
    {
        base.Awake();
        ninstance = this;
    }
    public override void OnInit()
    {
        base.OnInit();
        OptionsManager.event_OptionChanged += OnOptionChanged;
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_PageClose, OnPageClose);

        OnOptionChanged();
        if (GameManagerBase.Instance.B_InGame)       //Test
            m_Clips.Add(true, TResources.GetAudioClip_Background(GameManagerBase.Instance.B_InGame, true));
        m_Clips.Add(false, TResources.GetAudioClip_Background(GameManagerBase.Instance.B_InGame, false));
        PlayClip(false);
    }
    public override void OnRecycle()
    {
        base.OnRecycle();
        OptionsManager.event_OptionChanged -= OnOptionChanged;
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish); ;
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_PageClose, OnPageClose);
    }
    void OnBattleStart()=>PlayClip(true);
    void OnBattleFinish()=> PlayClip(false);
    void OnPageOpen() => m_AudioBG.pitch = .8f;
    void OnPageClose() => m_AudioBG.pitch = 1f;
    void PlayClip(bool inBattle)=> SwitchBackground(m_Clips[inBattle]);
    void OnOptionChanged()
    {
        m_volumeMultiply = GameExpression.F_GameMusicVolume( OptionsManager.m_OptionsData.m_MusicVolumeTap);
    }
}
