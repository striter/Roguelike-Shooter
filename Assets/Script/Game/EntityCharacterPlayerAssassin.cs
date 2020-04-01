using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerAssassin : EntityCharacterPlayer {
    #region Preset
    public float F_AssasinTriggerCooldown = 1f;
    public float F_AssassinttackRange = 8;
    public float F_AssassinAttackDuration = .2f;

    public float F_normalFinish = .25f;
    public float F_eliteFInish = .15f;
    public float F_AbilityDamagePerStack = 30;
    public int I_MaxAbilityDamageStack = 5;
    public float F_AssassinDamageStackResetDuration = 20;
    #endregion
    public int m_AssassinDamageStack { get; private set; }
    TimeCounter m_AssassinAttackTimer, m_AssassinCooldownTimer,m_AssassinStackResetTimer;
    EntityCharacterBase m_AssassinTarget;
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Assassin;
    public override float m_AbilityCooldownScale => m_AssassinCooldownTimer.m_TimeLeftScale;
    public override bool m_AbilityAvailable => !m_AssassinCooldownTimer.m_Timing&&m_AimingTarget&& Vector3.Distance(transform.position,m_AimingTarget.transform.position)<F_AssassinttackRange;
    protected override float CalculateMovementSpeedBase() => m_AssassinAttackTimer.m_Timing ? 0 : base.CalculateMovementSpeedBase();
    protected override bool CheckWeaponFiring() => !m_AssassinAttackTimer.m_Timing && base.CheckWeaponFiring();

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_AssassinAttackTimer = new TimeCounter(F_AssassinAttackDuration, true);
        m_AssassinCooldownTimer = new TimeCounter(F_AssasinTriggerCooldown, true);
        m_AssassinStackResetTimer = new TimeCounter(F_AssassinDamageStackResetDuration,true);
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
        m_CharacterSkinEffect.SetCloak(true,0f,F_AssassinAttackDuration);
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
            }
        }

        base.OnAliveTick(deltaTime);
    }

    void OnAssassinBlow()
    {
        if (m_AssassinTarget.m_IsDead)
            return;
        m_CharacterSkinEffect.SetCloak(false,0f,F_AssassinAttackDuration);
        Teleport(NavigationManager.NavMeshPosition(m_AssassinTarget.transform.position- m_AssassinTarget.transform.forward), m_AssassinTarget.transform.rotation);
        m_AssassinTarget.m_HitCheck.TryHit(m_WeaponCurrent.GetWeaponDamageInfo(m_WeaponCurrent.F_BaseDamage+ F_AbilityDamagePerStack*m_AssassinDamageStack));

        bool isElite = (m_AssassinTarget as EntityCharacterAI != null);
        isElite=isElite&&(m_AssassinTarget as EntityCharacterAI).E_SpawnType== enum_EnermyType.Elite;

        if (m_AssassinTarget.m_Health.F_HealthMaxScale > (isElite ? F_eliteFInish : F_normalFinish))
            return;
        m_AssassinTarget.m_HitCheck.TryHit(new DamageInfo(m_EntityID, m_AssassinTarget.m_Health.m_MaxHealth, enum_DamageType.HealthPenetrate));
        m_AssassinStackResetTimer.Replay();
        if (m_AssassinDamageStack < I_MaxAbilityDamageStack)
            m_AssassinDamageStack += 1;
    }
}
