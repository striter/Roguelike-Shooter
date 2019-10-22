using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager: SimpleSingletonMono <AudioManager>
{
    protected AudioSource m_Audio { get; private set; }
    AudioClip m_Clip;
    float m_baseVolume = 1f;
    public virtual float m_Volume => m_baseVolume;
    protected override void Awake()
    {
        base.Awake();
        m_Audio = GetComponent<AudioSource>();
        m_Audio.loop = true;
        m_Audio.playOnAwake = false;
        m_Audio.volume = m_Volume;
        m_baseVolume = 1f;
    }

    protected void SwitchClip(AudioClip _Clip)
    {
        if (m_Clip == _Clip)
            return;
        m_Clip = _Clip;
    }

    protected virtual void Update()
    {
        m_Audio.volume = m_Volume;
        if (m_Audio.clip == m_Clip)
        {
            m_baseVolume = Mathf.Lerp(m_baseVolume, 1f, Time.deltaTime);
        }
        else
        {
            m_baseVolume = Mathf.Lerp(m_baseVolume, 0f, Time.deltaTime);
            if (m_baseVolume <= .05f)
            {
                m_Audio.clip = m_Clip;
                m_Audio.Play();
            }
        }
    }
}
