using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : AudioManagerBase
{
    public static new AudioManager Instance { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    static float m_volumeMultiply = 1f;
    public override float m_BGVolume => base.m_BGVolume * m_volumeMultiply;
    Dictionary<enum_GameVFX, AudioClip> m_GameClips = new Dictionary<enum_GameVFX, AudioClip>();
    Dictionary<enum_UIVFX, AudioClip> m_UIClips = new Dictionary<enum_UIVFX, AudioClip>();
    public AudioClip GetGameSFXClip(enum_GameVFX sfx) => m_GameClips[sfx];
    public override void Init()
    {
        base.Init();
        TCommon.TraversalEnum((enum_GameVFX audio) => { m_GameClips.Add(audio, TResources.GetGameClip(audio)); });
        TBroadCaster<enum_BC_UIStatus>.Add<float>(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_PageClose, OnPageClose);
        OptionsManager.event_OptionChanged += OnOptionChanged;
        OnOptionChanged();
    }

    public void Recycle(bool inGame)
    {
        base.Recycle();
        TBroadCaster<enum_BC_UIStatus>.Remove<float>(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_PageClose, OnPageClose);
        OptionsManager.event_OptionChanged -= OnOptionChanged;
    }

    public SFXAudioBase Play3DClip(int sourceID, AudioClip _clip, bool _loop, Transform _target) => base.PlayClip(sourceID, _clip, OptionsManager.F_SFXVolume, _loop, _target);
    public SFXAudioBase Play3DClip(int sourceID, AudioClip _clip, bool _loop, Vector3 _pos) => base.PlayClip(sourceID, _clip, OptionsManager.F_SFXVolume, _loop, _pos);
    public SFXAudioBase Play2DClip(int sourceID, enum_GameVFX _clip) => base.PlayClip(sourceID,GetGameSFXClip( _clip), OptionsManager.F_SFXVolume, false);
    public SFXAudioBase Play2DClip(int sourceID, enum_UIVFX _clip) => base.PlayClip(sourceID, m_UIClips[_clip], OptionsManager.F_SFXVolume,false);
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
}
