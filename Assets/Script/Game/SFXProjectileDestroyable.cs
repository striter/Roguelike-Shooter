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
    }
    public override void Play(DamageDeliverInfo deliverInfo, Vector3 direction, Vector3 targetPosition)
    {
        m_Health.OnActivate(GameManager.Instance.GetEntity(deliverInfo.I_SourceID).m_Flag);
        m_Health.Play(OnStop);
        base.Play(deliverInfo, direction, targetPosition);
    }
    protected override void OnPlay()
    {
        m_Health.OnPlay();
        base.OnPlay();
    }
    protected override void OnStop()
    {
        m_Health.OnStop();
        base.OnStop();
    }
}
