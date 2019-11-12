using UnityEngine;
using GameSetting;
using System;

public class EntityBase : MonoBehaviour
{
    public int m_EntityID { get; private set; } = -1;
    public int I_PoolIndex { get; private set; } = -1;
    public virtual bool B_IsCharacter => false;
    public virtual enum_EntityController m_Controller => enum_EntityController.Invalid;
    public int I_MaxHealth;
    public enum_EntityFlag m_Flag { get; private set; }
    public HealthBase m_Health { get; private set; }
    protected virtual HealthBase GetHealthManager() => new HealthBase(OnHealthStatus,OnDead);
    protected virtual void ActivateHealthManager(float maxHealth) => m_Health.OnSetHealth(maxHealth, true);
    public HitCheckEntity m_HitCheck => m_HitChecks[0];
    protected virtual float DamageReceiveMultiply => 1;
    protected virtual float HealReceiveMultiply => 1f;
    public int m_SpawnerEntityID { get; private set; }
    public bool b_isSubEntity => m_SpawnerEntityID != -1;
    HitCheckEntity[] m_HitChecks;
    public virtual void Init(int _poolIndex)
    {
        I_PoolIndex = _poolIndex;
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
    }
    protected virtual bool OnReceiveDamage(DamageInfo damageInfo,Vector3 damageDirection)
    {
        if (m_Health.b_IsDead)
            return false;

        damageInfo.m_detail.m_OnHitAction?.Invoke(this);
        return m_Health.OnReceiveDamage(damageInfo, DamageReceiveMultiply,HealReceiveMultiply);
    }

    protected virtual void OnHealthStatus(enum_HealthChangeMessage message)
    {
    }
    protected virtual void OnDead()
    {
        EnableHitbox(false);
    }
    protected virtual void OnRevive()
    {
        EnableHitbox(true);
    }
    protected virtual void OnRecycle()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEntityDeactivate, this);

        if (I_PoolIndex > 0)
            GameObjectManager.RecycleEntity(I_PoolIndex, this);
    }
    protected virtual void EnableHitbox(bool setHitable)
    {
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.SetEnable(setHitable); });
    }
}
