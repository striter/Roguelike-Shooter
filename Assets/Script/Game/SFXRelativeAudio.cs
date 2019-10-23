using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SFXRelativeAudio : SFXRelativeBase {
    public AudioClip[] m_Clips;
    AudioSource m_Audio;
    public override void Init()
    {
        base.Init();
        if (m_Clips.Length <= 0)
            Debug.LogError("Set Audio Clip Here!");
    }
    public override void Play()
    {
        base.Play();
        m_Audio.pitch = Random.Range(0.95f, 1.05f);
        m_Audio.clip = m_Clips.RandomItem();
    }
    public override void OnPlay()
    {
        base.OnPlay();
        m_Audio.Play();
    }
    public override void OnStop()
    {
        base.OnStop();
        m_Audio.Stop();
    }
}
