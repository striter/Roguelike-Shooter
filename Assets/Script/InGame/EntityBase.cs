using UnityEngine;
using GameSetting;

public class EntityBase : MonoBehaviour
{
    public int I_EntityID { get; private set; } = -1;
    public int I_PoolIndex { get; private set; } = -1;
    public virtual bool B_IsCharacter => false;
    public virtual enum_EntityController m_Controller => enum_EntityController.Invalid;
    public int I_MaxHealth;
    public enum_EntityFlag m_Flag { get; private set; }
    public HealthBase m_Health { get; private set; }
    protected virtual HealthBase GetHealthManager() => new HealthBase(OnHealthChanged,OnDead);
    protected virtual void ActivateHealthManager() => m_Health.OnActivate(I_MaxHealth, true);
    public HitCheckEntity m_HitCheck => m_HitChecks[0];
    protected virtual float DamageReceiveMultiply => 1;
    HitCheckEntity[] m_HitChecks;
    public virtual void Init(int _poolIndex)
    {
        I_PoolIndex = _poolIndex;
        m_HitChecks = GetComponentsInChildren<HitCheckEntity>();
        m_Health = GetHealthManager();
    }
    public virtual void OnActivate( enum_EntityFlag _flag)
    {
        m_Flag = _flag;
        ActivateHealthManager();
        I_EntityID = GameIdentificationManager.I_EntityID(m_Flag);
        gameObject.name = I_EntityID.ToString() + "_" + I_PoolIndex.ToString();
        m_HitChecks.Traversal((HitCheckEntity check) => { check.Attach(this, OnReceiveDamage); check.SetEnable(true); });
    }
    protected virtual bool OnReceiveDamage(DamageInfo damageInfo)
    {
        if (m_Health.b_IsDead)
            return false;

        return m_Health.OnReceiveDamage(damageInfo, DamageReceiveMultiply);
    }

    protected virtual void OnHealthChanged(enum_HealthChangeMessage message)
    {
        OnRecycle();
    }
    protected virtual void OnDead()
    {
        TCommon.Traversal(m_HitChecks, (HitCheckEntity check) => { check.HideAllAttaches(); check.SetEnable(false); });
    }
    protected virtual void OnRecycle()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEntityRecycle, this);
        GameObjectManager.RecycleEntity(I_PoolIndex, this);
    }
}
