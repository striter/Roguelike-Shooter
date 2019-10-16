using UnityEngine;
using GameSetting;
using System;

[RequireComponent(typeof(Collider))]
public class HitCheckBase : MonoBehaviour {
    public virtual enum_HitCheck m_HitCheckType => enum_HitCheck.Invalid;
    public virtual int I_AttacherID { get; private set; } = -1;
    Func<DamageInfo,Vector3, bool> OnHitCheck;
    protected Collider m_Collider;
    protected void Awake()
    {
        gameObject.layer = m_HitCheckType.ToLayer();
    }
    protected void Attach(Func<DamageInfo,Vector3, bool> _OnHitCheck)
    {
        m_Collider = GetComponent<Collider>();
        OnHitCheck = _OnHitCheck;
    }

    public bool TryHit(DamageInfo amount) => TryHit(amount, Vector3.zero);
    public virtual bool TryHit(DamageInfo amount,Vector3 direction)
    {
        if (OnHitCheck == null)
            return false;

        return OnHitCheck(amount, direction);
    }
    public void SetEnable(bool enable)
    {
        m_Collider.enabled = enable;
    }
}
