using System;
using UnityEngine;

public class HitCheckLiving : HitCheckBase
{
    public override enum_checkObjectType E_Type => enum_checkObjectType.Living;
    public enum_EntityHitType E_PartType = enum_EntityHitType.Invalid;
    public override enum_WeaponSFX E_ImpactEffectType => E_PartType.ToWeaponSFX();
    public override bool B_AttachImpactDecal => true;
    public LivingBase m_Attacher { get; private set; }

    public void Attach(Func<float,enum_DamageType,LivingBase, bool?> _OnHit,LivingBase attacher)
    {
        base.Attach(_OnHit);
        m_Attacher = attacher;
    }

    public override bool? OnHitCheck(float damage, enum_DamageType type, Vector3 direction,LivingBase damageSource)
    {
        if (E_PartType == enum_EntityHitType.Critical)
            damage *= 1.5f;
        else if (E_PartType == enum_EntityHitType.Normal)
            damage *= 1;
        else
            damage = 0;

       return base.OnHitCheck(damage,type, direction,damageSource);
    }

}
