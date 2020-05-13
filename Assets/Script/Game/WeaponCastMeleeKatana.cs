using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastMeleeKatana : WeaponCastMelee
{

    protected int m_DashCastIndex { get; private set; }

    public override void OnPoolItemInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_DashCastIndex = GameExpression.GetPlayerExtraWeaponIndex(m_WeaponInfo.m_Index);
    }
    protected override void OnStoreTrigger(bool success)
    {
        if(success)
        {
            ShowCast(m_DashCastIndex, m_Attacher.tf_WeaponAim.position).Play(GetWeaponDamageInfo(m_BaseDamage));
            m_Attacher.PlayTeleport(NavigationManager.NavMeshPosition(m_Attacher.transform.position+m_Attacher.transform.forward*10), m_Attacher.transform.rotation);
            OnAttacherRecoil();
            OnAmmoCost();
        }
        else
        {
            OnAttacherAnim();
        }
    }

}
