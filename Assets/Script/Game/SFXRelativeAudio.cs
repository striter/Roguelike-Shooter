using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SFXRelativeAudio : SFXRelativeBase {
    public bool B_PlayDuringDelay;
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
    public override void Play(SFXParticles _source)
    {
        base.Play(_source);
        if (B_PlayDuringDelay)
            PlayAudio();
    }

    public override void OnPlay()
    {
        base.OnPlay();
        if (B_PlayDuringDelay)
            StopAudio();
        else
            PlayAudio();
    }

    private void Update()
    {
        if (!m_Audio || !B_PlayDuringDelay)
            return;

        m_Audio.SetPitch(Mathf.Lerp(2f,1f,m_SFXSource.f_delayLeftScale));
    }

    public override void OnStop()
    {
        base.OnStop();
        if (!B_PlayDuringDelay)
            StopAudio();
    }

    void PlayAudio()
    {
        if (m_Audio)
            m_Audio.Stop();

        m_Audio = B_Attach ? GameAudioManager.Instance.PlayClip(m_SFXSource.I_SourceID, m_Clips.RandomItem(), B_Loop, transform) : GameAudioManager.Instance.PlayClip(m_SFXSource.I_SourceID, m_Clips.RandomItem(), B_Loop, transform.position);
    }
    void StopAudio()
    {
        if (m_Audio&&B_Loop)
            m_Audio.Stop();

        m_Audio = null;
    }
}
