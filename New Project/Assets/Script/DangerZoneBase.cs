using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZoneBase : DetectorBase<LivingBase>
{
    public float F_DamagePerTick=10f;
    public float F_TickRate = .5f;
    public bool B_Activate = true;
    protected float f_TickCheck;
    

    protected override void Awake()
    {
        base.Awake();
        this.gameObject.layer = GameLayers.IL_LivingPlayerDetector;

        OnSetActivate(B_Activate);
    }
    #region Public APIs
    public void Switch()
    {
        B_Activate = !B_Activate;
        OnSetActivate(B_Activate);
    }
    public void Activate()
    {
        B_Activate = true;
        OnSetActivate(B_Activate);
    }
    public void Deactivate()
    {
        B_Activate = false;
        OnSetActivate(B_Activate);
    }
    public void OnActivate(bool activate)
    {
        B_Activate = activate;
        OnSetActivate(B_Activate);
    }
    #endregion
    protected virtual  void OnSetActivate(bool activate)
    {
    }

    protected virtual void Update()
    {
        if (B_Activate&&Time.time > f_TickCheck)
        {
            f_TickCheck = Time.time + F_TickRate;
            if (l_targets.Count > 0)
                TCommon.TraversalList(l_targets, (LivingBase target) =>
                {
                    if (target.isDead)
                        l_targets.Remove(target);
                    else
                        target.TakeDamage(F_DamagePerTick, enum_DamageType.DangerZone,null);
                });
        }
    }

    protected override LivingBase GetDetectTarget(Collider other)
    {
        LivingBase target = null;
        if (other.gameObject.layer == GameLayers.IL_Living)
        {
            target = other.GetComponent<HitCheckLiving>().m_Attacher;
        }
        else if (other.gameObject.layer == GameLayers.IL_Player)
        {
            target = other.GetComponent<LivingBase>();
        }
        return target;
    }
}
