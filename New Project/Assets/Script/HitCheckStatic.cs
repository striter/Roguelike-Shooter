using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheckStatic : HitCheckBase
{
    public enum_MaterialType E_MaterialType = enum_MaterialType.Invalid;
    public override enum_checkObjectType E_Type => enum_checkObjectType.Static;
    public override enum_WeaponSFX E_ImpactEffectType => E_MaterialType.ToWeaponSFX();
    public override bool B_ShowImpactDecal => B_ImpactShowDecal;
    public bool B_ImpactShowDecal = true;
    public override bool? OnHitCheck(float damage, enum_DamageType type, Vector3 direction,LivingBase damageSource)
    {
        return OnHit?.Invoke(damage,type, damageSource);
    }
    public override void Attach(Func<float, enum_DamageType,LivingBase, bool?> _OnHit)
    {
        OnHit = _OnHit;
    }
}
