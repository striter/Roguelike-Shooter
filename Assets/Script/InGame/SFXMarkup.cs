using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXMarkup : SFXParticles {
    protected override bool B_AutoStop => false;
    EntityBase target;
    Action OnMarkupDead;
    public void Play(int sourceID,EntityBase _target,Action _OnMarkupDead)
    {
        base.Play(sourceID);
        OnMarkupDead = _OnMarkupDead;
        target = _target;
    }
    protected override void Update()
    {
        base.Update();
        if (!B_ParticlesPlaying)
            return;

        transform.position = target.transform.position;
        if(target.m_Health.b_IsDead)
        {
            OnMarkupDead();
            StopParticles();
        }
    }
}
