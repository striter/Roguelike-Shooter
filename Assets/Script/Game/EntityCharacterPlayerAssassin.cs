using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerAssassin : EntityCharacterPlayer {
    #region Preset
    public float F_AbilityCooldown = 1f;
    public float F_AbilityAttackRange = 5;
    public float F_AbilityAttackDuration = .5f;
    
    public int P_NormalFinishPercentage = 25;
    public int P_EliteFinishPercentage = 15;
    public float F_AbilityDamagePerStack = 20;
    public int I_MaxAbilityDamageStack = 5;
    public float F_AbilityDamageStackExpireDuration = 20;
    #endregion
    EntityCharacterBase m_AssasinTarget;
    Vector3 m_AssassinStartPos,m_AssassinEndPos;
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Assassin;
    TimeCounter m_AbilityAttackTimer, m_AbilityCooldownTimer;
    EntityBase m_AssassultTarget;
    public override float m_AbilityCooldownScale => m_AbilityCooldownTimer.m_TimeLeftScale;
    public override bool m_AbilityAvailable => !m_AbilityCooldownTimer.m_Timing&&m_AimingTarget&& Vector3.Distance(transform.position,m_AimingTarget.transform.position)<F_AbilityAttackRange;
    protected override float CalculateMovementSpeedBase() => m_AbilityAttackTimer.m_Timing ? 0 : base.CalculateMovementSpeedBase();
    protected override bool CheckWeaponFiring() => !m_AbilityAttackTimer.m_Timing && base.CheckWeaponFiring();
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_AbilityAttackTimer = new TimeCounter(F_AbilityAttackDuration, true);
        m_AbilityCooldownTimer = new TimeCounter(F_AbilityCooldown, true);
    }
    public override void OnAbilityDown(bool down)
    {
        base.OnAbilityDown(down);
        if (!down ||m_IsDead|| !m_AbilityAvailable)
            return;

        m_AssassinStartPos = transform.position;
        m_AssassinEndPos = Vector3.Lerp(transform.position, m_AimingTarget.transform.position, .5f);
        m_AssasinTarget = m_AimingTarget;
        m_AbilityCooldownTimer.Replay();
        m_AbilityAttackTimer.Replay();
    }

    protected override void OnAliveTick(float deltaTime)
    {
        m_AbilityCooldownTimer.Tick(deltaTime);
        if (m_AbilityAttackTimer.m_Timing)
        {
            transform.position = Vector3.Lerp(m_AssassinStartPos, m_AssassinEndPos, 1 - m_AbilityAttackTimer.m_TimeLeftScale);
            m_AbilityAttackTimer.Tick(Time.deltaTime);
            EnableHitbox(m_AbilityAttackTimer.m_Timing);
            if (!m_AbilityAttackTimer.m_Timing)
            {
                OnAbilityDealtDamage();
            }
        }
        base.OnAliveTick(deltaTime);
    }

    void OnAbilityDealtDamage()
    {
        if (!m_AimingTarget)
            return;
        Teleport(NavigationManager.NavMeshPosition(m_AimingTarget.transform.position-m_AimingTarget.transform.forward),m_AimingTarget.transform.rotation);
        m_AimingTarget.m_HitCheck.TryHit(m_WeaponCurrent.GetWeaponDamageInfo(m_WeaponCurrent.F_BaseDamage));
    }
}
