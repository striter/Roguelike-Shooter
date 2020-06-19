using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerVampire : EntityCharacterPlayer {
    #region Preset
    public float F_AbilityCooldown = 1f;
    public float F_AbilityAttackRange = 8;
    public float F_AbilityAttackDuration = .2f;

    public float F_AbilityBaseDamage = 20;
    public float F_DamageIncreased = 50;
    public int I_MaxAbilityDamageStack = 5;
    public float F_AbilityStackResetDuration = 20;

    public float F_AbilityKillsHealPercent = .1f;
    public float F_AbilityKillsHealPercentAdditivePerRank = .01f;
    #endregion

    public int m_AssassinDamageStack { get; private set; }
    TimerBase m_AssassinAttackTimer, m_AssassinCooldownTimer,m_AssassinStackResetTimer;
    EntityCharacterBase m_AssassinTarget;
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Vampire;
    public override float m_AbilityCooldownScale => m_AssassinCooldownTimer.m_TimeLeftScale;
    public override bool m_AbilityAvailable => !m_AssassinCooldownTimer.m_Timing&&m_AimingTarget&& Vector3.Distance(transform.position,m_AimingTarget.transform.position)<F_AbilityAttackRange;
    public override float GetBaseMovementSpeed() => m_AssassinAttackTimer.m_Timing ? 0 : base.GetBaseMovementSpeed();
    protected override bool CheckWeaponFiring() => !m_AssassinAttackTimer.m_Timing && base.CheckWeaponFiring();

    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_AssassinAttackTimer = new TimerBase(F_AbilityAttackDuration, true);
        m_AssassinCooldownTimer = new TimerBase(F_AbilityCooldown, true);
        m_AssassinStackResetTimer = new TimerBase(F_AbilityStackResetDuration,true);
        m_AssassinDamageStack = 0;
    }
    public override void OnAbilityDown(bool down)
    {
        base.OnAbilityDown(down);
        if (!down ||m_IsDead|| !m_AbilityAvailable)
            return;

        m_AssassinTarget = m_AimingTarget;
        m_AssassinCooldownTimer.Replay();
        m_AssassinAttackTimer.Replay();
        m_CharacterSkinEffect.SetCloak(true,0f,F_AbilityAttackDuration);
    }

    protected override void OnAliveTick(float deltaTime)
    {
        m_AssassinCooldownTimer.Tick(deltaTime);

        if (m_AssassinStackResetTimer.m_Timing)
        {
            m_AssassinStackResetTimer.Tick(deltaTime);
            if (!m_AssassinStackResetTimer.m_Timing)
                m_AssassinDamageStack = 0;
        }

        if (m_AssassinAttackTimer.m_Timing)
        {
            m_AssassinAttackTimer.Tick(Time.deltaTime);
            if (!m_AssassinAttackTimer.m_Timing)
            {
                OnAssassinBlow();
                m_CharacterInfo.OnAbilityTrigger();
            }
        }

        base.OnAliveTick(deltaTime);
    }

    void OnAssassinBlow()
    {
        m_CharacterSkinEffect.SetCloak(false, 0f, F_AbilityAttackDuration);
        PlayTeleport(NavigationManager.NavMeshPosition(m_AssassinTarget.transform.position - m_AssassinTarget.transform.forward), m_AssassinTarget.transform.rotation);
        if (!m_AssassinTarget.m_IsDead)
        {
            Debug.Log(m_CharacterInfo.m_RankManager.m_Rank);
            m_AssassinTarget.m_HitCheck.TryHit(m_CharacterInfo.GetDamageInfo(F_AbilityBaseDamage + F_DamageIncreased * m_CharacterInfo.m_RankManager.m_Rank, 0,-1, enum_DamageType.Basic, enum_DamageIdentity.PlayerAbility)); 
            if (m_AssassinTarget.m_IsDead)
                m_HitCheck.TryHit(new DamageInfo(m_EntityID, enum_DamageIdentity.PlayerAbility).SetDamage(-m_Health.m_MaxHealth*(F_AbilityKillsHealPercent+m_CharacterInfo.m_RankManager.m_Rank*F_AbilityKillsHealPercentAdditivePerRank), enum_DamageType.Health));
        }

        if (!m_AssassinTarget.m_IsDead)
            return;
        m_AssassinStackResetTimer.Replay();
        if (m_AssassinDamageStack < I_MaxAbilityDamageStack)
            m_AssassinDamageStack += 1;
    }

    protected override void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
    {
        base.OnCharacterHealthChange(damageInfo, damageEntity, amountApply);
    }

    protected override void OnDead()
    {
        base.OnDead();
        m_AssassinAttackTimer.Stop();
    }
}
