﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager: SimpleSingletonMono <AudioManager>
{
    protected AudioSource m_AudioBG { get; private set; }
    AudioClip m_Clip;
    float m_baseVolume = 1f;
    public virtual float m_Volume => m_baseVolume;
    protected override void Awake()
    {
        base.Awake();
        m_AudioBG = GetComponent<AudioSource>();
        m_AudioBG.loop = true;
        m_AudioBG.playOnAwake = false;
        m_AudioBG.volume = m_Volume;
        m_baseVolume = 1f;
    }

    protected void SwitchBackground(AudioClip _Clip)
    {
        if (m_Clip == _Clip)
            return;
        m_Clip = _Clip;
    }

    protected virtual void Update()
    {
        m_AudioBG.volume = m_Volume;
        if (m_AudioBG.clip == m_Clip)
        {
            m_baseVolume = Mathf.Lerp(m_baseVolume, 1f, Time.deltaTime);
        }
        else
        {
            m_baseVolume = Mathf.Lerp(m_baseVolume, 0f, Time.deltaTime);
            if (m_baseVolume <= .05f)
            {
                m_AudioBG.clip = m_Clip;
                m_AudioBG.Play();
            }
        }
    }

    public void PlayClip(AudioClip _clip, Transform _target)
    {

    }

    public void PlayClip(AudioClip _clip, Vector3 _pos)
    {

    }
}
