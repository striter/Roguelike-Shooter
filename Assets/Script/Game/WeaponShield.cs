using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponShield : WeaponBase {
    public override enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Shield;
    
    protected override void OnAutoTrigger(float animDuration)
    {
        OnAmmoCost();
    }

    public override bool OnDamageBlockCheck(DamageInfo info) => m_Trigger.m_Triggering;
}
