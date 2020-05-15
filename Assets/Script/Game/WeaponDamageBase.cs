using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageBase : WeaponBase {

    public float F_Damage = 10;
    public float F_DamageMultiplyPerEnhance = .1f;
    public float F_RecoilPerShot = 2;
    public int I_ExtraBuffApply = -1;
    public override float m_BaseDamage => F_Damage * (1 + F_DamageMultiplyPerEnhance * m_EnhanceLevel);
    public override int m_BuffApply => I_ExtraBuffApply;
    public float m_Recoil => m_Attacher.m_CharacterInfo.F_AimSpreadMultiply * F_RecoilPerShot;
    protected override void OnAmmoCost()
    {
        base.OnAmmoCost();
        m_Attacher.PlayRecoil(m_Recoil);
    }
}
