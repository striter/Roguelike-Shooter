using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EntityComponent))]
public class SFXProjectileDestroyable : SFXProjectile {
     protected EntityComponent m_Health { get; private set; }
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Health = GetComponentInChildren<EntityComponent>();
        m_Health.Init(-1);
        m_Health.ActionOnDead(OnRecycle);
    }
    public override void Play(DamageDeliverInfo damageInfo, Vector3 direction, Vector3 targetPosition)
    {
        base.Play(damageInfo, direction, targetPosition);
        m_Health.OnActivate( GameManager.Instance.GetEntity(damageInfo.I_SourceID).m_Flag);
    }
}
