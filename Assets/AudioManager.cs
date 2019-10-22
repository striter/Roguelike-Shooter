using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SimpleSingletonMono<AudioManager> {
    AudioSource m_BackgroundSource;
    AudioClip m_Clip;
    float m_baseVolume = 1f;
    static float m_volumeMultiply = 1f;
    public static void SetVolueMultiply(float _volumeMultiply) => m_volumeMultiply = _volumeMultiply;
    public float m_Volume => m_baseVolume * m_volumeMultiply;
    protected override void Awake()
    {
        base.Awake();
        m_BackgroundSource = GetComponent<AudioSource>();
        m_BackgroundSource.loop = true;
        m_BackgroundSource.playOnAwake = false;
    }

    public void Play(AudioClip _clip)
    {
        m_Clip = _clip;
        if (m_BackgroundSource.clip == null)
        {
            m_BackgroundSource.clip = m_Clip;
            m_BackgroundSource.Play();
        }
    }

    private void Update()
    {
        m_BackgroundSource.volume = m_Volume;
        if (m_BackgroundSource.clip == m_Clip)
        {
            if (m_BackgroundSource.volume != 1f)
                return;

            m_baseVolume = Mathf.Lerp(m_baseVolume, 1f, Time.deltaTime * 19f);
            return;
        }

        m_baseVolume = Mathf.Lerp(m_baseVolume, 0f, Time.deltaTime*10f);
        if (m_BackgroundSource.volume < .1f)
        {
            m_BackgroundSource.clip = m_Clip;
            m_BackgroundSource.volume = 0f;
            m_BackgroundSource.Play();
        }
    }
    
}
