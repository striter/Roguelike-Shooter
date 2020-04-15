using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerBeth : EntityCharacterPlayer {
    #region Preset
    public float F_AbilityCoolDown = 0f;
    public float F_RollSpeedMultiple;
    public float F_RollDuration;
    #endregion
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Beth;
    protected TimerBase m_RollTimer,m_RollCooldown;
    protected bool m_Rolling => m_RollTimer.m_Timing;
    Vector3 m_RollDirection;
    public override bool m_AbilityAvailable => !m_RollCooldown.m_Timing;
    public override float m_AbilityCooldownScale => m_RollCooldown.m_TimeLeftScale;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_RollTimer = new TimerBase(F_RollDuration, true);
        m_RollCooldown = new TimerBase(F_AbilityCoolDown, true);
    }

    public override void OnAbilityDown(bool down)
    {
        base.OnAbilityDown(down);
        if (!down || m_IsDead || !m_AbilityAvailable)
            return;
        Vector2 rollAxisDirection = m_MoveAxisInput == Vector2.zero ? new Vector2(0, 1) : m_MoveAxisInput;
        m_RollDirection = base.CalculateMoveDirection(rollAxisDirection);
        m_Animator.BeginRoll(F_RollDuration);
        m_RollTimer.Replay();
        m_RollCooldown.Replay();
        m_CharacterInfo.OnAbilityTrigger();
        EnableHitbox(false);
    }
    
    protected override void OnAliveTick(float deltaTime)
    {
        m_RollCooldown.Tick(deltaTime);
        if (m_Rolling)
        {
            m_RollTimer.Tick(deltaTime);
            if (!m_Rolling)
            {
                m_WeaponCurrent.AddAmmo(m_WeaponCurrent.I_ClipAmount/2);
                m_CharacterInfo.AddBuff(m_SpawnerEntityID,SBuff.CreateGameBethBuff(m_CharacterInfo.m_ExtraFireRateMultiply*1f+.1f*m_CharacterInfo.m_RankManager.m_Rank,1.5f));
                m_Animator.EndRoll();
                EnableHitbox(true);
            }
        }
        base.OnAliveTick(deltaTime);
    }
    protected override void OnDead()
    {
        m_RollTimer.Stop();
        m_Animator.EndRoll();
        base.OnDead();
    }

    protected override bool CheckWeaponFiring() => !m_Rolling&& base.CheckWeaponFiring();
    protected override float CalculateMovementSpeedBase() => (m_Rolling? F_RollSpeedMultiple :1)* base.CalculateMovementSpeedBase();
    protected override float CalculateMovementSpeedMultiple() => m_Rolling ? 1f : base.CalculateMovementSpeedMultiple();
    protected override Vector3 CalculateMoveDirection(Vector2 moveAxisInput) => m_Rolling ? m_RollDirection : base.CalculateMoveDirection(moveAxisInput);
    protected override Quaternion GetCharacterRotation() => m_Rolling ? Quaternion.LookRotation(m_RollDirection, Vector3.up) : base.GetCharacterRotation();

}
