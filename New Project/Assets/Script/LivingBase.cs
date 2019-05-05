using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBase : MonoBehaviour
{
    public virtual enum_Flags E_Flag => enum_Flags.Invalid;
    public virtual enum_LivingType E_Type
    {
        get
        {
            Debug.LogError("Override This Please");
            return enum_LivingType.Invalid; ;
        }
    }
    public int I_MaxHealth=100;
    public bool isDead { get; protected set; }
    public float f_curHealth { get; private set; }
    protected HitCheckBase[] ar_hitChecks;
    public HitCheckBase m_HitPart => ar_hitChecks[0];
    protected virtual void Awake()
    {
        ar_hitChecks = GetComponentsInChildren<HitCheckBase>();
        TCommon.TraversalArray(ar_hitChecks, (HitCheckBase hcb) => {
            switch (hcb.E_Type)
            {
                case enum_checkObjectType.Player:
                case enum_checkObjectType.Living:
                    (hcb as HitCheckLiving).Attach(TakeDamage,this);
                    break;
                case enum_checkObjectType.Dynamic:
                    hcb.Attach(TakeDamage);
                    break;
                default:
                    Debug.LogError("Add More Cases Here!" + hcb.E_Type.ToString());
                    break;
            }
        });
    }
    protected virtual void OnEnable()
    {
        f_curHealth = I_MaxHealth;
        isDead = false;
    }
    protected virtual void OnDisable()
    {

    }
    protected virtual void OnDestroy()
    {

    }
    public virtual bool? TakeDamage(float damage,enum_DamageType damageType, LivingBase damageSource)
    {
        if (isDead)
            return false;

        f_curHealth -= damage;
        if (f_curHealth <= 0)
        {
            isDead = true;
            OnDead();
        }
        return true;
    }
    protected virtual void OnDead()
    {
        Recycle();
    }
    protected void Recycle()
    {
        EntityManager.RecycleLiving<LivingBase>(E_Type, this);
    }
}
