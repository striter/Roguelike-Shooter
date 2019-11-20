using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
public class SFXParticles : SFXBase
{
    protected SFXRelativeBase[] m_relativeSFXs;
    public TSpecialClasses.ParticleControlBase m_Particle { get; private set; }
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_relativeSFXs = GetComponentsInChildren<SFXRelativeBase>();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Init(); });
        m_Particle = new TSpecialClasses.ParticleControlBase(transform);
    }

    public virtual void Play(int sourceID,float duration=0f,float delayDuration=0f)
    {
        PlaySFX(sourceID,duration,delayDuration);
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Play(this); });
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => {  relative.OnPlay(); });
        m_Particle.Play();
    }

    protected override void OnStop()
    {
        base.OnStop();
        transform.SetParent(GameObjectManager.TF_SFXWaitForRecycle);
        m_relativeSFXs.Traversal((SFXRelativeBase sfxRelative) => { sfxRelative.OnStop(); });
        m_Particle.Stop();
    }

    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.OnRecycle(); });
        m_Particle.Clear();
    }

    protected void SetParticlesActive(bool active) => m_Particle.SetActive(active);
}
