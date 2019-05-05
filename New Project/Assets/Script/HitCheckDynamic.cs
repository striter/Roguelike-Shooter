using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheckDynamic : HitCheckBase
{
    public enum_MaterialType E_MaterialType = enum_MaterialType.Invalid;
    public override enum_checkObjectType E_Type => enum_checkObjectType.Dynamic;
    public override enum_WeaponSFX E_ImpactEffectType => E_MaterialType.ToWeaponSFX();
    public override bool B_ShowImpactDecal => false;
    protected override void Awake()
    {
        base.Awake();
        StopHold();
    }
    public HitCheckDynamic StartHold()
    {
        rb_current.useGravity = false;
        rb_current.freezeRotation = true;
        return this;
    }
    public void StopHold()
    {
        rb_current.useGravity = true;
        rb_current.freezeRotation = false;
    }
    public void Throw(float strength, Vector3 direction)
    {
        rb_current.AddForce(strength*direction);
    }
}
