using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXParticles : SFXBase
{
    protected ParticleSystem[] m_Particles { get; private set; }
    protected float m_Duration { get; private set; }
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Particles = transform.GetComponentsInChildren<ParticleSystem>();
        m_Particles.Traversal((ParticleSystem particle) => {
            if (particle.main.duration > m_Duration)
                m_Duration = particle.main.duration;
            particle.Stop();
                });
    }
    public void Play(int sourceID)
    {
        PlaySFX(sourceID,m_Duration);
        m_Particles.Traversal((ParticleSystem particle) => { particle.Play(); });
    }
}
