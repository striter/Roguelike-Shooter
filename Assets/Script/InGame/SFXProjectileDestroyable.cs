using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(HitCheckDynamic))]
public class SFXProjectileDestroyable : SFXProjectile {
    public int I_MaxHealth;
    HitCheckDynamic m_hitCheck;
    HealthBase m_Health;
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_hitCheck = GetComponent<HitCheckDynamic>();
        m_Health = new HealthBase(null, OnDestroyed);
    }
    public override void Play(DamageDeliverInfo buffInfo, Vector3 direction, Vector3 targetPosition)
    {
        base.Play(buffInfo, direction, targetPosition);
        m_hitCheck.Attach(buffInfo.I_SourceID, OnReceiveDamage);
    }
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        m_Health.OnActivate(I_MaxHealth);
        if (I_MaxHealth < 0)
            Debug.LogError("Destroyable Health Lower Than 0!"+gameObject.name);
    }
    bool OnReceiveDamage(DamageInfo info)
    {
        m_Health.OnReceiveDamage(info, 1);
        return true;
    }
    protected virtual void OnDestroyed()
    {
        OnRecycle();
    }
}
