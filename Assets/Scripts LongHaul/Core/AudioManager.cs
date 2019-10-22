using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager: SimpleSingletonMono <AudioManager>
{
    protected AudioSource m_AudioBackground { get; private set; }
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
}
