using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponDamageProjectileRailgun : WeaponDamageProjectile
{
    protected int m_StoreProjectileDataIndex { get; private set; }

    public override void OnPoolItemInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_StoreProjectileDataIndex = GameExpression.GetPlayerExtraWeaponIndex(m_WeaponInfo.m_Index);
    }

    protected override void OnStoreTrigger(float duration,bool success)=> FireProjectile(success ? m_StoreProjectileDataIndex : m_BaseSFXWeaponIndex, GetWeaponDamageInfo(m_BaseDamage),duration);
}
