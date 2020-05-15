using UnityEngine;
using GameSetting;
using System;

public class EntityBase : CObjectPoolStaticPrefabBase<int>
{
    public int I_MaxHealth;
    public int m_EntityID { get; private set; } = -1;
    public virtual enum_EntityType m_ControllType => enum_EntityType.Invalid;
    public enum_EntityFlag m_Flag { get; private set; }
    public HealthBase m_Health { get; private set; }
    protected virtual HealthBase GetHealthManager() => new HealthBase(OnUIHealthChanged);
    public HitCheckEntity m_HitCheck => m_HitChecks[0];
    protected bool m_HitCheckEnabled { get; private set; } = false;
    protected virtual float DamageReceiveMultiply => 1;
    protected virtual float HealReceiveMultiply => 1f;
    public bool m_IsDead { get; private set; }
    public bool m_Activating { get; private set; }
    HitCheckEntity[] m_HitChecks;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_HitChecks = GetComponentsInChildren<HitCheckEntity>();
        m_Health = GetHealthManager();
    }
    protected virtual void OnEntityActivate(enum_EntityFlag flag)
    {
        m_Activating = true;
        m_Flag = flag;
        m_EntityID = GameIdentificationManager.GetEntityID(m_Flag);
        m_Health.OnActivate(I_MaxHealth);
        m_HitChecks.Traversal((HitCheckEntity check) => { check.Attach(this, OnReceiveDamage); });
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEntityActivate, this);
        EnableHitbox(true);
        m_IsDead = false;
    }
    protected bool OnReceiveDamage(DamageInfo damageInfo, Vector3 damageDirection) => OnReceiveDamageAmount(damageInfo, damageDirection) != 0;
    protected virtual float OnReceiveDamageAmount(DamageInfo damageInfo, Vector3 direction)
    {
        if (m_IsDead)
            return 0;

        return m_Health.OnReceiveDamage(damageInfo, DamageReceiveMultiply, HealReceiveMultiply);
    }

    protected virtual void OnUIHealthChanged(enum_HealthChangeMessage message)
    {
        if ( m_Health.m_CurrentHealth <= 0)
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

    public override void OnPoolItemRecycle()
    {
        base.OnPoolItemRecycle();
        m_Activating = false;
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEntityRecycle, this);
    }

    protected virtual void EnableHitbox(bool setHitable)
    {
        if (m_HitCheckEnabled == setHitable)
            return;
        m_HitCheckEnabled = setHitable;
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.SetEnable(m_HitCheckEnabled); });
    }
}
