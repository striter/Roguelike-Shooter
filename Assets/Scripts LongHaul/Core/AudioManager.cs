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
    public virtual void OnInit()
    {
        GameObject obj = new GameObject("AudioObj_3D");
        AudioSource source= obj.AddComponent<AudioSource>();
        source.spatialBlend = 1;
        SFXAudioBase audioObj = obj.AddComponent<SFXAudioBase>();
        ObjectPoolManager<int, SFXAudioBase>.Register(0, audioObj, 5, (SFXAudioBase audios) => audios.Init(0));

        obj = new GameObject("AudioObj_2D");
        source = obj.AddComponent<AudioSource>();
        source.spatialBlend = 0;
        audioObj = obj.AddComponent<SFXAudioBase>();
        ObjectPoolManager<int, SFXAudioBase>.Register(1, audioObj, 5, (SFXAudioBase audios) => audios.Init(0));
    }
    public virtual void OnRecycle()
    {
        ObjectPoolManager<int, SFXAudioBase>.ClearAll();
    }
    protected void SwitchBackground(AudioClip _Clip,bool loop)
    {
        if (m_Clip == _Clip)
            return;
        m_Clip = _Clip;
        m_AudioBG.loop = loop;
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

    public SFXAudioBase PlayClip(int sourceID,AudioClip _clip, bool _loop, Transform _target)
    {
        SFXAudioBase audio= ObjectPoolManager< int ,SFXAudioBase >.Spawn(0,null);
        audio.transform.position = _target.position;
        return audio.Play(sourceID, _clip,_loop, _target);
    }
    public SFXAudioBase PlayClip(int sourceID, AudioClip _clip, bool _loop, Vector3 _position)
    {
        SFXAudioBase audio = ObjectPoolManager<int, SFXAudioBase>.Spawn(0, null);
        audio.transform.position = _position;
        return audio.Play(sourceID, _clip, _loop, null);
    }
    public SFXAudioBase PlayClip(int sourceID, AudioClip _clip, bool _loop)
    {
        SFXAudioBase audio= ObjectPoolManager<int, SFXAudioBase>.Spawn(1, null);
        audio.transform.position = Vector3.zero;
        return audio.Play(sourceID,_clip,_loop,null);
    }
}
