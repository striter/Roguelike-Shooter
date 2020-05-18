using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
public class SFXParticles : SFXBase
{
    protected SFXRelativeBase[] m_relativeSFXs;
    public TSpecialClasses.ParticleControlBase m_Particle { get; private set; }
    public bool m_LoopParticles { get; private set; }
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_relativeSFXs = GetComponentsInChildren<SFXRelativeBase>();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Init(); });
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
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Play(this); });
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        m_Particle.Play();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.OnPlay(); });
    }

    protected override void OnStop()
    {
        base.OnStop();
        transform.SetParent(GameObjectManager.TF_SFXWaitForRecycle);
        m_Particle.Stop();
        m_relativeSFXs.Traversal((SFXRelativeBase sfxRelative) => { sfxRelative.OnStop(); });
    }

    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_Particle.Clear();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.OnRecycle(); });
    }

    protected void SetParticlesActive(bool active) => m_Particle.SetActive(active);
}
