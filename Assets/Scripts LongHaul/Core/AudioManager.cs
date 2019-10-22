using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager: SimpleSingletonMono <AudioManager>
{
    protected AudioSource m_AudioBackground { get; private set; }
    AudioClip m_Clip;
    float m_baseVolume = 1f;
    public virtual float m_Volume => m_baseVolume;
    protected override void Awake()
    {
        base.Awake();
        m_AudioBackground = GetComponent<AudioSource>();
        m_AudioBackground.loop = true;
        m_AudioBackground.playOnAwake = false;
        m_AudioBackground.volume = m_Volume;
        m_baseVolume = 1f;
    }

    protected void SwitchClip(AudioClip _Clip)
    {
        if (m_Clip == _Clip)
            return;

        m_Clip = _Clip;
        m_AudioBackground.clip = m_Clip;
        m_AudioBackground.Play();
    }
}
