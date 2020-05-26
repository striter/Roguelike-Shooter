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
    Dictionary<enum_BattleVFX, AudioClip> m_GameClips = new Dictionary<enum_BattleVFX, AudioClip>();
    public AudioClip GetGameSFXClip(enum_BattleVFX sfx) => m_GameClips[sfx];
    public override void Init()
    {
        base.Init();
        TCommon.TraversalEnum((enum_BattleVFX audio) => { m_GameClips.Add(audio, TResources.GetGameClip(audio)); });
        TBroadCaster<enum_BC_UIStatus>.Add<float>(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_PageClose, OnPageClose);
        OptionsDataManager.event_OptionChanged += OnOptionChanged;
        OnOptionChanged();
    }

    public void Recycle(bool inGame)
    {
        base.Destroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<float>(enum_BC_UIStatus.UI_PageOpen, OnPageOpen);
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_PageClose, OnPageClose);
        OptionsDataManager.event_OptionChanged -= OnOptionChanged;
    }

    public SFXAudioBase Play3DClip(int sourceID, AudioClip _clip, bool _loop, Transform _target) => base.PlayClip(sourceID, _clip, OptionsDataManager.F_SFXVolume, _loop, _target);
    public SFXAudioBase Play3DClip(int sourceID, AudioClip _clip, bool _loop, Vector3 _pos) => base.PlayClip(sourceID, _clip, OptionsDataManager.F_SFXVolume, _loop, _pos);
    public SFXAudioBase Play2DClip(int sourceID, AudioClip _clip) => base.PlayClip(sourceID,_clip, OptionsDataManager.F_SFXVolume, false);
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
        m_volumeMultiply = OptionsDataManager.F_MusicVolume;
        SetSFXVolume(OptionsDataManager.F_SFXVolume);
    }
}
