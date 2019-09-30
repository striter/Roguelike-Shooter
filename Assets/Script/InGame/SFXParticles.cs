using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXParticles : SFXBase
{
    protected ParticleSystem[] m_Particles { get; private set; }
    protected float m_ParticleDuration { get; private set; }
    protected SFXRelativeBase[] m_relativeSFXs;
    protected virtual bool B_PlayOnAwake => true;
    protected bool B_ParticlesPlaying;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_relativeSFXs = GetComponentsInChildren<SFXRelativeBase>();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Init(); });
        m_Particles = transform.GetComponentsInChildren<ParticleSystem>();
        m_Particles.Traversal((ParticleSystem particle) => {
            if (particle.main.duration > m_ParticleDuration)
                m_ParticleDuration = particle.main.duration;

            particle.Stop();
        });
    }
    public virtual void Play(int sourceID,float duration=0)
    {
        if (duration == 0)
            duration = m_ParticleDuration;
        PlaySFX(sourceID,duration);
        if (B_PlayOnAwake)
            PlayParticles();
    }

    protected override void SetLifeTime(float duration) => base.SetLifeTime(duration + GameConst.F_ParticlesMaxStopTime);
    public void ResetParticles()
    {
        m_relativeSFXs.Traversal((SFXRelativeBase sfxRelative) => { sfxRelative.OnReset(); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Clear(); });
     }
    public void PlayParticles()
    {
        B_ParticlesPlaying = true;
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.Play(); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Play(); });
    }
    protected override void Update()
    {
        base.Update();
        if (B_PlayOnAwake && B_ParticlesPlaying && f_timeLeft < GameConst.F_ParticlesMaxStopTime)
            StopParticles();
    }
    protected override void OnRecycle()
    {
        base.OnRecycle();
        m_relativeSFXs.Traversal((SFXRelativeBase relative) => { relative.OnRecycle(); });
        ResetParticles();
    }
    public virtual void StopParticles()
    {
        transform.SetParent(GameObjectManager.TF_SFXWaitForRecycle);
        base.SetLifeTime(GameConst.F_ParticlesMaxStopTime);
        B_ParticlesPlaying = false;
        m_relativeSFXs.Traversal((SFXRelativeBase sfxRelative) => { sfxRelative.Stop(); });
        m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
    }
}
