using GameSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using TSpecialClasses;
using UnityEngine;

public class EntityCharacterBase : EntityBase
{
    public int I_MaxHealth;
    public int I_DefaultArmor;
    public float F_MovementSpeed;
    public float F_AttackSpread;
    public Transform tf_Model { get; private set; }
    public Transform tf_Head { get; private set; }
    public virtual Transform tf_Weapon=>null;
    public CharacterExpireManager m_CharacterInfo { get; private set; }
    public virtual MeshRenderer m_WeaponSkin { get; private set; }
    public EntityCharacterSkinEffectManager m_CharacterSkinEffect { get; private set; }
    public virtual Vector3 m_PrecalculatedTargetPos(float time)=> tf_Head.position;
    public int m_SpawnerEntityID { get; private set; }
    public bool b_isSubEntity => m_SpawnerEntityID != -1;
    protected virtual CharacterExpireManager GetEntityInfo() => new CharacterExpireManager(this,  OnExpireChange);
    
    public virtual float m_baseMovementSpeed => F_MovementSpeed;
    protected override float DamageReceiveMultiply => m_CharacterInfo.m_DamageReceiveMultiply;
    protected override float HealReceiveMultiply => m_CharacterInfo.m_HealReceiveMultiply;

    public new EntityHealth m_Health=>base.m_Health as EntityHealth;
    protected override HealthBase GetHealthManager()=> new EntityHealth(this, OnUIHealthChanged);
    TimerBase m_DeadCounter = new TimerBase(GameConst.F_EntityDeadFadeTime,true);
    protected virtual enum_GameVFX m_DamageClip => enum_GameVFX.EntityDamage;
    protected virtual enum_GameVFX m_ReviveClip => enum_GameVFX.PlayerRevive;

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        tf_Model = transform.Find("Model");
        tf_Head = transform.Find("Head");
        Transform tf_Skin = tf_Model.Find("Skin");
        List<Renderer> renderers = new List<Renderer>();
        if(tf_Skin) renderers.AddRange(tf_Skin.GetComponentsInChildren<Renderer>().ToList());
        m_CharacterSkinEffect = new EntityCharacterSkinEffectManager(tf_Model,renderers);
        m_CharacterInfo = GetEntityInfo();
    }

    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase>(enum_BC_GameStatus.OnCharacterHealthWillChange, OnCharacterHealthWillChange);
    }

    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        m_CharacterSkinEffect.OnDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase>(enum_BC_GameStatus.OnCharacterHealthWillChange, OnCharacterHealthWillChange);
    }
    public bool m_TargetAvailable =>  !m_IsDead;
    protected override void OnEntityActivate(enum_EntityFlag flag, float startHealth = 0)
    {
        base.OnEntityActivate(flag, startHealth);
        m_CharacterSkinEffect.OnReset();
    }

    protected void OnMainCharacterActivate(enum_EntityFlag _flag)
    {
        OnEntityActivate(_flag,I_MaxHealth);
        m_SpawnerEntityID = -1;
    }
    public virtual EntityCharacterBase OnSubCharacterActivate(enum_EntityFlag _flag, int _spawnerID, float startHealth)
    {
        OnEntityActivate(_flag, startHealth);
        m_SpawnerEntityID = _spawnerID;
        return this;
    }

    protected virtual void OnExpireChange(){ }

    #region Expire Interact
    protected virtual void OnCharacterHealthWillChange(DamageInfo damageInfo, EntityCharacterBase damageEntity)
    {
        if (damageInfo.m_AmountApply <= 0)
            return;

        if (damageInfo.m_SourceID == m_EntityID)
            m_CharacterInfo.OnWillDealtDamage(damageInfo, damageEntity);
        else if (damageEntity.m_EntityID == m_EntityID)
            m_CharacterInfo.OnWillReceiveDamage(damageInfo, damageEntity);
    }

    protected virtual void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
    {
        if (damageInfo.m_SourceID == m_EntityID)
        {
            m_CharacterInfo.OnDealtDamage(damageInfo, damageEntity, amountApply);
        }

        if (damageEntity.m_EntityID == m_EntityID)
        {
            if (amountApply > 0)
                m_CharacterInfo.OnAfterReceiveDamage(damageInfo, damageEntity, amountApply);
            else
                m_CharacterInfo.OnReceiveHealing(damageInfo, damageEntity, amountApply);
        }
    }
    #endregion
    private void Update()
    {
        if (!m_Activating)
            return;

        m_CharacterInfo.Tick(Time.deltaTime);

        if (!m_IsDead)
            OnAliveTick(Time.deltaTime);
        else
            OnDeadTick(Time.deltaTime);
    }
    protected virtual void OnAliveTick(float deltaTime) { }
    protected virtual void OnDeadTick(float deltaTime)
    {
        if (m_DeadCounter.m_Timing)
        {
            m_DeadCounter.Tick(deltaTime);
            m_CharacterSkinEffect.SetDeathEffect(1f-m_DeadCounter.m_TimeLeftScale);
            return;
        }
        if (!m_DeadCounter.m_Timing)
            DoRecycle();
    }

    public virtual void ReviveCharacter()
    {
        if (!m_IsDead)
            return;
        OnRevive();
        m_CharacterSkinEffect.OnReset();
        EntityHealth health = (m_Health as EntityHealth);
        health.OnSetStatus( health.m_MaxHealth,health.m_MaxArmor);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterRevive, this);
    }

    protected override float OnReceiveDamageAmount(DamageInfo damageInfo, Vector3 direction)
    {
        if (m_IsDead)
            return 0;

        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterHealthWillChange, damageInfo, this);
        damageInfo.m_BaseBuffApply.Traversal((SBuff buffInfo) => { m_CharacterInfo.AddExpire(new EntityExpirePreset(damageInfo.m_SourceID, buffInfo)); });
        float amount = base.OnReceiveDamageAmount(damageInfo, direction);
        if (amount != 0)
            TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterHealthChange, damageInfo, this,amount);
        return amount;
    }
    protected override void OnDead()
    {
        base.OnDead();
        m_CharacterInfo.OnDead();
        m_CharacterSkinEffect.SetDeath();
        m_DeadCounter.Replay();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterDead, this);
    }
    public override void DoRecycle()
    {
        base.DoRecycle();
        m_CharacterInfo.OnRecycle();
    }

    protected override void OnUIHealthChanged(enum_HealthChangeMessage type)
    {
        base.OnUIHealthChanged(type);
        m_CharacterSkinEffect.OnHit(type);
        switch (type)
        {
            case enum_HealthChangeMessage.DamageArmor:
            case enum_HealthChangeMessage.DamageHealth:
                AudioManager.Instance.Play3DClip(m_EntityID, AudioManager.Instance.GetGameSFXClip(m_DamageClip), false, transform);
                break;
        }
    }
    
}
