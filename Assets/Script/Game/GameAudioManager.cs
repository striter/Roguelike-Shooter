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
    Dictionary<enum_GameMusic, AudioClip> m_MusicClip = new Dictionary<enum_GameMusic, AudioClip>();
    Dictionary<enum_GameAudio, AudioClip> m_AudioClips = new Dictionary<enum_GameAudio, AudioClip>();
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
        TBroadCaster<enum_BC_GameStatus>.Add<bool>(enum_BC_GameStatus.OnGameFinish, OnGameFinish);
        TBroadCaster<enum_BC_UIStatus>.Add<float>(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_PageClose, OnPageClose);

        OnOptionChanged();
        TCommon.TraversalEnum((enum_GameMusic music) =>
        {
            if (GameManagerBase.Instance.B_InGame||music== enum_GameMusic.Relax)       //Test
                m_MusicClip.Add(music, TResources.GetAudioClip_Background(GameManagerBase.Instance.B_InGame, music));
        });
        TCommon.TraversalEnum((enum_GameAudio audio) =>
        {
            m_AudioClips.Add(audio, TResources.GetAudioClip_SFX(audio));
        });
        PlayClip( enum_GameMusic.Relax,false);
    }
    public override void OnRecycle()
    {
        base.OnRecycle();
        OptionsManager.event_OptionChanged -= OnOptionChanged;
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish); ;
        TBroadCaster<enum_BC_GameStatus>.Remove<bool>(enum_BC_GameStatus.OnGameFinish, OnGameFinish);
        TBroadCaster<enum_BC_UIStatus>.Remove<float>(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_PageClose, OnPageClose);
    }
    void OnBattleStart()=>PlayClip( enum_GameMusic.Fight, true);
    void OnBattleFinish()=> PlayClip( enum_GameMusic.Relax,true);
    void OnGameFinish(bool win) => PlayClip(win? enum_GameMusic.Win: enum_GameMusic.Lost ,false);
    void OnPageOpen(float bulletTime) => m_AudioBG.pitch =Mathf.Lerp(.6f,1f , bulletTime);
    void OnPageClose() => m_AudioBG.pitch = 1f;
    void PlayClip(enum_GameMusic music,bool loop)=> SwitchBackground(m_MusicClip[music],loop);
    void OnOptionChanged()
    {
        m_volumeMultiply = GameExpression.F_GameMusicVolume( OptionsManager.m_OptionsData.m_MusicVolumeTap);
    }
}
