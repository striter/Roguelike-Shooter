using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class WeaponCastAnimBloodThirster : WeaponCastAnim {
    public float F_HealthDrainMultiply = .1f;
    
    public override void OnDealtDamage(float amountApply)
    {
        base.OnDealtDamage(amountApply);
        m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID, enum_DamageIdentity.PlayerWeapon, m_WeaponID).SetDamage(-amountApply * F_HealthDrainMultiply, enum_DamageType.Health));
    }
}
