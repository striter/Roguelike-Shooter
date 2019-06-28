﻿using UnityEngine;
using GameSetting;
using System;

[RequireComponent(typeof(Collider))]
public class HitCheckBase : MonoBehaviour {
    public virtual enum_HitCheck m_HitCheckType => enum_HitCheck.Invalid;
    Func<float,bool> OnHitCheck;
    protected Collider m_Collider;
    protected void Awake()
    {
        gameObject.layer = m_HitCheckType.ToLayer();
    }
    protected void Attach(Func<float,bool> _OnHitCheck)
    {
        m_Collider = GetComponent<Collider>();
        OnHitCheck = _OnHitCheck;
    }
    public virtual bool TryHit(float amount)
    {
        if (OnHitCheck == null)
            return false;

        return OnHitCheck(amount);
    }
    public void SetEnable(bool enable)
    {
        m_Collider.enabled = enable;
    }
}
