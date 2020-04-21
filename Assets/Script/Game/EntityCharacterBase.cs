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
    protected virtual CharacterExpireManager GetEntityInfo() => new CharacterExpireManager(this, m_HitCheck.TryHit, OnExpireChange);
    
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
        m_CharacterInfo.OnActivate();
    }

    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        m_CharacterSkinEffect.OnDisable();
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
    protected void OnSubCharacterActivate(enum_EntityFlag _flag, int _spawnerID , float startHealth )
    {
        OnEntityActivate(_flag, startHealth);
        m_SpawnerEntityID = _spawnerID;
    }

    protected virtual void OnExpireChange(){ }

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
        m_CharacterInfo.OnRevive();
        EntityHealth health = (m_Health as EntityHealth);
        health.OnSetStatus( health.m_MaxHealth,health.m_MaxArmor);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterRevive, this);
    }

    protected override bool OnReceiveDamage(DamageInfo damageInfo, Vector3 damageDirection)
    {
        if (!base.OnReceiveDamage(damageInfo, damageDirection))
            return false;

        damageInfo.m_BaseBuffApply.Traversal((SBuff buffInfo) => { m_CharacterInfo.AddBuff(damageInfo.m_SourceID, buffInfo); });

        return true;
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
