using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EntityComponent))]
public class SFXProjectileDestroyable : SFXProjectile {
    protected EntityComponent m_Health { get; private set; }
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_Health = GetComponentInChildren<EntityComponent>();
        m_Health.OnPoolItemInit(-1,null);
    }
    public override void Play(DamageDeliverInfo deliverInfo, Vector3 direction, Vector3 targetPosition)
    {
        m_Health.OnActivate(GameManager.Instance.GetEntity(deliverInfo.I_SourceID).m_Flag);
        m_Health.Play(OnStop);
        base.Play(deliverInfo, direction, targetPosition);
    }
    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnStop);
    }

    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnStop);
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
