﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SFXRelativeAudio : SFXRelativeBase {
    public bool B_Attach;
    public bool B_Loop;
    public AudioClip[] m_Clips;
    SFXAudioBase m_Audio;
    public override void Init()
    {
        base.Init();
        if (m_Clips.Length <= 0)
            Debug.LogError("Set Audio Clip Here!");
    }
    public override void OnPlay()
    {
        base.OnPlay();
        m_Audio =AudioManager.Instance.PlayClip(m_SFXSource.I_SourceID, m_Clips.RandomItem(),B_Loop,transform.position, B_Attach?transform:null);
    }
    public override void OnStop()
    {
        base.OnStop();
        if(B_Loop)
            m_Audio.Stop();
    }
}