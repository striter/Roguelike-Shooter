﻿using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
public class SFXParticles : SFXBase
{
    Transform m_AttachTo;
    Vector3 m_localPos,m_localDir;
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

    public void AttachTo(Transform _attachTo)
    {
        m_AttachTo = _attachTo;
        if (!_attachTo)
            return;
        m_localPos = _attachTo.InverseTransformPoint(transform.position);
        m_localDir = _attachTo.InverseTransformDirection(transform.forward);
    }

    protected override void Update()
    {
        base.Update();
        if (m_AttachTo == null)
            return;

        transform.position = m_AttachTo.TransformPoint(m_localPos);
        transform.rotation = Quaternion.LookRotation(m_AttachTo.TransformDirection(m_localDir));
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
        m_AttachTo = null;
    }

    public void Stop()
    {
        OnStop();
    }
}
