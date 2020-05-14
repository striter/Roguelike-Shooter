﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastAnimKatana : WeaponCastAnim
{
    public float F_DashDistance = 6;
    protected int m_DashCastIndex { get; private set; }

    protected bool m_Dashing = false;
    public override void OnPoolItemInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_DashCastIndex = GameExpression.GetPlayerExtraWeaponIndex(m_WeaponInfo.m_Index);
    }
    protected override void OnStoreTrigger(bool success)
    {
        m_Dashing = success;
        if(success)
            OnAttacherAnim(1);
        else
            OnAttacherAnim(0);
    }

    protected override void OnKeyAnim()
    {
        base.OnKeyAnim();
        DoMeleeCast(m_Dashing ? m_DashCastIndex : m_BaseSFXWeaponIndex, GetMeleeSize());

        if (m_Dashing)
            m_Attacher.PlayTeleport(NavigationManager.NavMeshPosition(m_Attacher.transform.position + m_Attacher.transform.forward * F_DashDistance * GetMeleeSize()), m_Attacher.transform.rotation);
    }

}
