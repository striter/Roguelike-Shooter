﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageCastAnimKatana : WeaponDamageCastAnim
{
    public float F_DashDistance = 6;
    protected int m_DashCastIndex { get; private set; }

    protected bool m_Dashing = false;
    public override void OnPoolInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_DashCastIndex = GameExpression.GetPlayerExtraWeaponIndex(m_WeaponInfo.m_Index);
    }
    protected override void OnStoreTrigger(float animDuration, float storeTimeLeft)
    {
        m_Dashing = storeTimeLeft==0;
        OnAttackAnim(animDuration, m_Dashing ? 1 : 0);
    }

    protected override void OnKeyAnim()
    {
        DoMeleeCast(m_Dashing ? m_DashCastIndex : m_BaseSFXWeaponIndex, m_Dashing,GetMeleeSize());
        OnAmmoCost();
        if (m_Dashing)
            m_Attacher.PlayTeleport(NavigationManager.NavMeshPosition(m_Attacher.transform.position + m_Attacher.transform.forward * F_DashDistance * GetMeleeSize()), m_Attacher.transform.rotation);
    }

}
