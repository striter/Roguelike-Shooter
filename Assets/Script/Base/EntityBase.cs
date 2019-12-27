﻿using UnityEngine;
using GameSetting;
using System;

public class EntityBase : ObjectPoolMonoItem<int>
{
    public int m_EntityID { get; private set; } = -1;
    public virtual bool B_IsCharacter => false;
    public virtual enum_EntityController m_Controller => enum_EntityController.Invalid;
    public int I_MaxHealth;
    public enum_EntityFlag m_Flag { get; private set; }
    public HealthBase m_Health { get; private set; }
    protected virtual HealthBase GetHealthManager() => new HealthBase(OnHealthChanged);
    protected virtual void ActivateHealthManager(float maxHealth) => m_Health.OnSetHealth(maxHealth, true);
    public HitCheckEntity m_HitCheck => m_HitChecks[0];
    protected bool m_HitCheckEnabled { get; private set; } = false;
    protected virtual float DamageReceiveMultiply => 1;
    protected virtual float HealReceiveMultiply => 1f;
    public int m_SpawnerEntityID { get; private set; }
    public bool b_isSubEntity => m_SpawnerEntityID != -1;
    public bool m_IsDead,m_Activating;
    HitCheckEntity[] m_HitChecks;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_HitChecks = GetComponentsInChildren<HitCheckEntity>();
        m_Health = GetHealthManager();
    }
    public virtual void OnActivate( enum_EntityFlag _flag,int _spawnerID=-1, float startHealth =0)
    {
        m_Flag = _flag;
        m_SpawnerEntityID = _spawnerID;
        ActivateHealthManager(startHealth>0? startHealth:I_MaxHealth);
        m_EntityID = GameIdentificationManager.I_EntityID(m_Flag);
        m_HitChecks.Traversal((HitCheckEntity check) => { check.Attach(this, OnReceiveDamage); });
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEntityActivate, this);
        EnableHitbox(true);
        m_IsDead = false;
        m_Activating = true;
    }
    protected virtual bool OnReceiveDamage(DamageInfo damageInfo,Vector3 damageDirection)
    {
        if (m_IsDead)
            return false;

        damageInfo.m_detail.m_OnHitAction?.Invoke(this);
        return m_Health.OnReceiveDamage(damageInfo, DamageReceiveMultiply,HealReceiveMultiply);
    }

    protected virtual void OnHealthChanged(enum_HealthChangeMessage message)
    {
        if (m_Health.m_CurrentHealth <= 0)
            OnDead();
    }
    protected virtual void OnDead()
    {
        m_IsDead = true;
        EnableHitbox(false);
    }
    protected virtual void OnRevive()
    {
        m_IsDead = false;
        EnableHitbox(true);
    }
    protected virtual void OnRecycle()
    {
        m_Activating = false;
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEntityDeactivate, this);
        DoItemRecycle();
    }
    protected virtual void EnableHitbox(bool setHitable)
    {
        if (m_HitCheckEnabled == setHitable)
            return;
        m_HitCheckEnabled = setHitable;
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.SetEnable(m_HitCheckEnabled); });
    }
}
