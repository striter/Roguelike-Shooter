using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerUnknown : EntityCharacterPlayer {
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
    Vector3 m_AttackStart, m_AttackEnd;
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Unknown;
    TimeCounter m_AbilityAttackTimer, m_AbilityCooldownTimer;
    public override float m_AbilityCooldownScale => m_AbilityCooldownTimer.m_TimeLeftScale;
    public override bool m_AbilityAvailable => !m_AbilityCooldownTimer.m_Timing&&m_AimAssistTarget&& Vector3.Distance(transform.position,m_AimAssistTarget.transform.position)<F_AbilityAttackRange;
    protected override float CalculateMovementSpeedBase() => m_AbilityAttackTimer.m_Timing ? 0 : base.CalculateMovementSpeedBase();
    protected override bool CheckWeaponFiring() => m_AbilityAttackTimer.m_Timing && base.CheckWeaponFiring();
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

        m_AttackStart = transform.position;
        m_AttackEnd = m_AimAssistTarget.transform.position;
        m_AbilityCooldownTimer.Reset();
        m_AbilityAttackTimer.Reset();
    }

    protected override void OnAliveTick(float deltaTime)
    {
        if (!m_AvailableTarget)
            m_AbilityAttackTimer.Stop();

        m_AbilityCooldownTimer.Tick(deltaTime);
        if (m_AbilityAttackTimer.m_Timing)
        {
            transform.position = Vector3.Lerp(m_AttackStart,m_AttackEnd,1- m_AbilityAttackTimer.m_TimeLeftScale);
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
        Debug.Log("Dealt Damage");
    }

}
