using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(AudioSource))]
public class GameAudioManager : AudioManager
{
    protected static GameAudioManager ninstance;
    public static new GameAudioManager Instance => ninstance;
    protected override void Awake()
    {
        base.Awake();
        ninstance = this;
    }
    static float m_volumeMultiply = 1f;
    public override float m_BGVolume => base.m_BGVolume * m_volumeMultiply;
    Dictionary<enum_GameMusic, AudioClip> m_GameMusic = new Dictionary<enum_GameMusic, AudioClip>();
    Dictionary<enum_GameAudioSFX, AudioClip> m_AudioClips = new Dictionary<enum_GameAudioSFX, AudioClip>();
    public AudioClip GetSFXClip(enum_GameAudioSFX sfx) => m_AudioClips[sfx];

    public void Init(bool inGame)
    {
        base.Init();
        TCommon.TraversalEnum((enum_GameMusic music) =>
        {
            AudioClip clip = ((inGame && music > enum_GameMusic.GameMusicStart && music < enum_GameMusic.GameMusicEnd) || (!inGame && music > enum_GameMusic.CampMusicStart && music < enum_GameMusic.CampMusicEnd)) ? TResources.GetAudioClip_Background(music) : null;
            if (clip) m_GameMusic.Add(music, clip);
        });
        if (inGame) TCommon.TraversalEnum((enum_GameAudioSFX audio) => { m_AudioClips.Add(audio, TResources.GetAudioClip_SFX(audio)); });

        TBroadCaster<enum_BC_UIStatus>.Add<float>(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_PageClose, OnPageClose);
        OptionsManager.event_OptionChanged += OnOptionChanged;
        OnOptionChanged();
    }
    public void OnRecycle(bool inGame)
    {
        base.OnRecycle();
        TBroadCaster<enum_BC_UIStatus>.Remove<float>(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_PageClose, OnPageClose);
        OptionsManager.event_OptionChanged -= OnOptionChanged;
    }

    void OnPageOpen(float bulletTime)
    {
        //SetBGPitch(Mathf.Lerp(.6f, 1f, bulletTime));
    }
    void OnPageClose()
    {
        //SetBGPitch(1f);
    }
    void OnOptionChanged()
    {
        m_volumeMultiply = OptionsManager.F_MusicVolume;
        SetSFXVolume(OptionsManager.F_SFXVolume);
    }
    #region Game
    public SFXAudioBase PlayClip(int sourceID, AudioClip _clip, bool _loop, Transform _target) => base.PlayClip(sourceID, _clip, OptionsManager.F_SFXVolume, _loop, _target);
    public SFXAudioBase PlayClip(int sourceID, AudioClip _clip, bool _loop, Vector3 _pos) => base.PlayClip(sourceID, _clip, OptionsManager.F_SFXVolume, _loop, _pos);
    public SFXAudioBase PlayClip(int sourceID, AudioClip _clip, bool _loop) => base.PlayClip(sourceID, _clip, OptionsManager.F_SFXVolume, _loop);
    #endregion

    public void PlayClip(enum_GameMusic music, bool loop)
    {
        if (m_GameMusic.ContainsKey(music))
            SwitchBackground(m_GameMusic[music], loop);
        else
            Debug.LogWarning("None Music Found Of:" + music);
    }
    public void Stop() => StopBackground();
}
