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
        m_Particles.Traversal((ParticleSystem particle) => {
            particle.Stop();
        });
    }
    public virtual void Play(int sourceID,float duration=0f,float delayDuration=0f)
    {
        PlaySFX(sourceID,duration,delayDuration);
    }
    public void ResetParticles()
    {
        m_relativeSFXs.Traversal((SFXRelativeBase sfxRelative) => { sfxRelative.OnReset(); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Clear(); });
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Play(); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Play(); });
    }

    protected override void OnStop()
    {
        base.OnStop();
        transform.SetParent(GameObjectManager.TF_SFXWaitForRecycle);
        m_relativeSFXs.Traversal((SFXRelativeBase sfxRelative) => { sfxRelative.Stop(); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
    }

    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.OnRecycle(); });
        ResetParticles();
    }

    public void Stop()
    {
        SetLifeTime(0f);
    }
}
