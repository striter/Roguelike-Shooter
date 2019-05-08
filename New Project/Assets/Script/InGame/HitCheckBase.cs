using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

[RequireComponent(typeof(Collider))]
public class HitCheckBase : MonoBehaviour {
    public virtual enum_HitCheck m_HitCheckType => GameSetting.enum_HitCheck.Invalid;
    Func<float,bool> OnHitCheck;
    protected Collider m_Collider;

    public void Attach(Func<float,bool> _OnHitCheck)
    {
        m_Collider = GetComponent<Collider>();
        gameObject.layer = m_HitCheckType.ToLayer();
        OnHitCheck = _OnHitCheck;
    }
    public virtual bool TryHit(float amount)
    {
        if (OnHitCheck == null)
            return false;

        return OnHitCheck(amount);
    }
}
