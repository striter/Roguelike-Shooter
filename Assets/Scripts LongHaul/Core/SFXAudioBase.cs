using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SFXAudioBase : SFXBase {

    AudioSource m_Audio;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Audio = GetComponent<AudioSource>();
        m_Audio.playOnAwake=false;
    }
    public SFXAudioBase Play(int _sourceID,AudioClip _clip,Transform _attachTo)
    {
        m_Audio.clip = _clip;
        m_Audio.pitch = Random.Range(0.95f, 1.05f);
        AttachTo(_attachTo);
        base.PlaySFX(_sourceID,_clip.length,0);
        return this;
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        m_Audio.Play();
    }
    protected override void OnStop()
    {
        base.OnStop();
        m_Audio.Stop();
    }
}
