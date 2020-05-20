using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageBase : WeaponBase {

    public float F_Damage = 10;
    public float F_DamageMultiplyPerEnhance = .1f;
    public float F_DamageMultiplyOnStoreSuccessful = .5f;
    public float F_CriticalRate = .1f;
    public float F_RecoilPerShot = 2;
    public int I_ExtraBuffApply = -1;


    public float GetBaseDamage(bool store) =>( F_Damage * (1 + F_DamageMultiplyPerEnhance * m_EnhanceLevel))*(1f+(store?F_DamageMultiplyOnStoreSuccessful:0f));
    public int GetBuffApply() => I_ExtraBuffApply;
    public float GetBaseCriticalRate() => F_CriticalRate;
    public float m_Recoil => m_Attacher.m_CharacterInfo.F_AimSpreadMultiply * F_RecoilPerShot;

    protected override void OnAmmoCost()
    {
        base.OnAmmoCost();
        m_Attacher.PlayRecoil(m_Recoil);
    }

    public DamageInfo GetWeaponDamageInfo(bool storeDamage, enum_DamageType type = enum_DamageType.Basic) => m_Attacher.m_CharacterInfo.GetDamageInfo(GetBaseDamage(storeDamage), GetBaseCriticalRate(), GetBuffApply(), type, enum_DamageIdentity.PlayerWeapon, m_WeaponID);
}
