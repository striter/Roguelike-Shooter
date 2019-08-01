using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXParticles : SFXBase
{
    protected ParticleSystem[] m_Particles { get; private set; }
    protected float m_ParticleDuration { get; private set; }
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Particles = transform.GetComponentsInChildren<ParticleSystem>();
        m_Particles.Traversal((ParticleSystem particle) => {
            if (particle.main.duration > m_ParticleDuration)
                m_ParticleDuration = particle.main.duration;
            particle.Stop();
                });
    }
    public virtual void Play(int sourceID,float duration=-1)
    {
        PlaySFX(sourceID, duration==-1?m_ParticleDuration: duration);
        m_Particles.Traversal((ParticleSystem particle) => { particle.Play(); });
    }
    public void Stop()
    {
        transform.SetParent(null);
        f_TimeCheck = Time.time + 2f;
        m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
    }
}
