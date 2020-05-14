using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class WeaponItemHealPotion : WeaponItemBase {

    protected override void OnKeyAnim()
    {
        base.OnKeyAnim();
        m_Attacher.m_HitCheck.TryHit(GetWeaponDamageInfo(-m_BaseDamage, enum_DamageType.Health));
    }
}
