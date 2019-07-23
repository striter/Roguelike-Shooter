using UnityEngine;
using GameSetting;
using System;

[RequireComponent(typeof(Collider))]
public class HitCheckBase : MonoBehaviour {
    public virtual enum_HitCheck m_HitCheckType => enum_HitCheck.Invalid;
    public int I_AttacherID = 0;
    Func<DamageInfo, bool> OnHitCheck;
    protected Collider m_Collider;
    protected void Awake()
    {
        gameObject.layer = m_HitCheckType.ToLayer();
    }
    protected void Attach(int index,Func<DamageInfo, bool> _OnHitCheck)
    {
        I_AttacherID= index;
        m_Collider = GetComponent<Collider>();
        OnHitCheck = _OnHitCheck;
    }
    public virtual bool TryHit(DamageInfo amount)
    {
        if (OnHitCheck == null)
            return false;

        return OnHitCheck(amount);
    }
    public void SetEnable(bool enable)
    {
        m_Collider.enabled = enable;
    }
}
