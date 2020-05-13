using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastMeleeKatana : WeaponCastMelee
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


    public override void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType)
    {
        if(!m_Dashing)
        {
            base.OnAnimEvent(eventType);
            return;
        }

        if (eventType != TAnimatorEvent.enum_AnimEvent.Fire)
            return;
        DoMeleeCast(m_DashCastIndex);
        m_Attacher.PlayTeleport(NavigationManager.NavMeshPosition(m_Attacher.transform.position + m_Attacher.transform.forward * F_DashDistance), m_Attacher.transform.rotation);
    }

}
