using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXMarkup : SFXParticles {
    EntityBase target;
    Action OnMarkupDead;
    public void Play(int sourceID,EntityBase _target,Action _OnMarkupDead)
    {
        base.PlayControlled(sourceID);
        OnMarkupDead = _OnMarkupDead;
        target = _target;
    }
    protected override void Update()
    {
        base.Update();
        if (!B_Playing)
            return;

        transform.position = target.transform.position;
        if(target.m_IsDead)
        {
            OnStop();
            OnMarkupDead();
            OnMarkupDead = null;
        }
    }
}
