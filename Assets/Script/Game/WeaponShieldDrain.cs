using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponShieldDrain : WeaponShield {
    public float F_HealthDrainPerBlock=5;
    public float F_HealthDrainAddivePerEnhance=1;

    public override bool OnDamageBlockCheck(DamageInfo info)
    {
        m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID, enum_DamageIdentity.PlayerWeapon, m_WeaponID).SetDamage(-(F_HealthDrainPerBlock+F_HealthDrainAddivePerEnhance*m_EnhanceLevel), enum_DamageType.Health));
        return base.OnDamageBlockCheck(info);
    }
}
