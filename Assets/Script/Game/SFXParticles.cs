using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
public class SFXParticles : SFXBase
{
    public TSpecialClasses.ParticleControlBase m_Particle { get; private set; }
    public bool m_LoopParticles { get; private set; }
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_Particle = new TSpecialClasses.ParticleControlBase(transform.Find("Particles"));
    }

    public SFXParticles PlayUncontrolled(int sourceID, float duration=0,float delayDuration=0)
    {
        Play();
        PlaySFX(sourceID,duration==0? 5f:duration,delayDuration,true);
        return this;
    }

    public SFXParticles PlayControlled(int sourceID, float delayDuration = 0)
    {
        Play();
        PlaySFX(sourceID, 0, delayDuration, false);
        return this;
    }

    protected virtual  void Play()
    {
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        m_Particle.Play();
    }

    protected override void OnStop()
    {
        base.OnStop();
        transform.SetParent(GameObjectManager.TF_SFXWaitForRecycle);
        m_Particle.Stop();
    }

    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_Particle.Clear();
    }
    
}
