using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
public class SFXParticles : SFXBase
{
    protected ParticleSystem[] m_Particles { get; private set; }
    protected SFXRelativeBase[] m_relativeSFXs;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_relativeSFXs = GetComponentsInChildren<SFXRelativeBase>();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Init(); });
        m_Particles = transform.GetComponentsInChildren<ParticleSystem>();
    }
    public virtual void Play(int sourceID,float duration=0f,float delayDuration=0f)
    {
        PlaySFX(sourceID,duration,delayDuration);
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Play(this); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Simulate(0, true, true); });
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => {  relative.OnPlay(); });
        m_Particles.Traversal((ParticleSystem particle) => {  particle.Play(true); });
    }

    protected override void OnStop()
    {
        base.OnStop();
        transform.SetParent(GameObjectManager.TF_SFXWaitForRecycle);
        m_relativeSFXs.Traversal((SFXRelativeBase sfxRelative) => { sfxRelative.OnStop(); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(true); });
    }

    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.OnRecycle(); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Clear(); });
    }

    protected void SetParticlesActive(bool active)
    {
        m_Particles.Traversal((ParticleSystem particle) => { particle.transform.SetActivate(active); });
    }
}
