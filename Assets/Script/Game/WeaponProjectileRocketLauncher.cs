using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileRocketLauncher : WeaponProjectileBase {

    public int m_BlastCastSFXWeaponIndex { get; private set; }
    public override float F_BaseDamage => GameObjectManager.GetSFXWeaponData<SFXCast>(m_BlastCastSFXWeaponIndex).F_Damage;
    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_BlastCastSFXWeaponIndex = GameExpression.GetWeaponSubIndex(m_BaseSFXWeaponIndex);
    }

}
