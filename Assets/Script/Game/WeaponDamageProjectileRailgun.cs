using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponDamageProjectileRailgun : WeaponDamageProjectile
{
    protected int m_StoreProjectileDataIndex { get; private set; }

    public override void OnPoolInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_StoreProjectileDataIndex = GameExpression.GetPlayerExtraWeaponIndex(m_WeaponInfo.m_Index);
    }

    protected override void OnStoreTrigger(float duration, float storeTimeLeft)
    {
        bool successful=storeTimeLeft==0;
        FireProjectile(successful, successful ? m_StoreProjectileDataIndex : m_BaseSFXWeaponIndex, duration);
    }
}
