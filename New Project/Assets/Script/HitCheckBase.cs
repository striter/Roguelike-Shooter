using System;
using UnityEngine;
public class HitCheckBase : MonoBehaviour
{
    public virtual enum_checkObjectType E_Type => enum_checkObjectType.Invalid;
    public virtual enum_WeaponSFX E_ImpactEffectType => enum_WeaponSFX.Invalid;
    public virtual bool B_ShowImpactDecal => true;
    public virtual bool B_AttachImpactDecal => false;
    protected Func<float,enum_DamageType, LivingBase, bool?> OnHit;
    public Rigidbody rb_current { get; private set; }
    protected virtual void Awake()
    {
        rb_current = GetComponent<Rigidbody>();
        gameObject.layer = E_Type.ToGameLayer();
    }
    public virtual void Attach(Func<float, enum_DamageType,LivingBase, bool?> _OnHit)
    {
        OnHit = _OnHit;
        rb_current = GetComponent<Rigidbody>();
        if (rb_current == null)
            Debug.LogWarning("null Rigidbody Attached To HitCheck:" + this.gameObject);
    }
    public virtual bool? OnHitCheck(float damage, enum_DamageType type, Vector3 direction,LivingBase damageSource)
    {
        if (rb_current != null)
            rb_current.AddForce(direction * damage / 5);

        return OnHit?.Invoke(damage,type,damageSource);
    }
}
