using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EntityItemBase))]
public class SFXProjectileDestroyable : SFXProjectile {
    HitCheckDynamic m_hitCheck;
    EntityItemBase m_Health;
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Health = GetComponentInChildren<EntityItemBase>();
        m_Health.Init(-1);
    }
    public override void Play(DamageDeliverInfo buffInfo, Vector3 direction, Vector3 targetPosition)
    {
        base.Play(buffInfo, direction, targetPosition);
    }

    protected virtual void OnDestroyed()
    {
        OnRecycle();
    }
}
