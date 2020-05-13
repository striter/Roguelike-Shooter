using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileRocketLauncher : WeaponProjectileBase {
    public int m_BlastCastSFXWeaponIndex { get; private set; }
    public override void OnPoolItemInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_BlastCastSFXWeaponIndex = GameExpression.GetWeaponSubIndex(m_BaseSFXWeaponIndex);
    }
}
