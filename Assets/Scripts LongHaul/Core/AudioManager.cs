using System.Collections;
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
    protected virtual void Start()
    {
        GameObject obj = new GameObject("AudioObj");
        obj.AddComponent<AudioSource>();
        SFXAudioBase audioObj= obj.AddComponent<SFXAudioBase>();
        ObjectPoolManager<int, SFXAudioBase>.Register(0, audioObj, 5,(SFXAudioBase audios)=>audios.Init(0));
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

    public SFXAudioBase PlayClip(int sourceID,AudioClip _clip,bool _loop,Vector3 _position, Transform _target)
    {
        SFXAudioBase audio= ObjectPoolManager<int, SFXAudioBase>.Spawn(0,null);
        audio.transform.position = _position;
        return audio.Play(sourceID, _clip, _target);
    }
}
