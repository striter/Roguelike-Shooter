using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SFXRelativeAudio : SFXRelativeBase {
    public AudioClip[] m_Clips;
    public override void Init()
    {
        base.Init();
        if (m_Clips.Length <= 0)
            Debug.LogError("Set Audio Clip Here!");
    }
    public override void OnPlay()
    {
        base.OnPlay();
        AudioManager.Instance.PlayClip(m_SFXSource.I_SourceID, m_Clips.RandomItem(), transform);
    }
}
