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
    public SFXAudioBase Play(int _sourceID,AudioClip _clip,bool _loop,Transform _attachTo)
    {
        m_Audio.clip = _clip;
        m_Audio.pitch = Random.Range(0.9f, 1.1f);
        m_Audio.loop = _loop;
        AttachTo(_attachTo);
        base.PlaySFX(_sourceID,_loop?0:_clip.length,0);
        return this;
    }
    public void SetPitch(float _pitch)=> m_Audio.pitch = _pitch;
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
    protected override void OnRecycle()
    {
        AttachTo(null);
        ObjectPoolManager<int,SFXAudioBase>.Recycle(I_SFXIndex, this);
    }
}
