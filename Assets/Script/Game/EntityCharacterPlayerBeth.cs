using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerBeth : EntityCharacterPlayer {
    #region Preset
    public float F_RollSpeedMultiple;
    public float F_RollDuration;
    #endregion
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Beth;
    private new PlayerAnimatorBeth m_Animator;
    protected override WeaponBaseAnimator GetAnimatorController(Animator animator,Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent)
    {
        m_Animator= new PlayerAnimatorBeth(animator, _OnAnimEvent);
        return m_Animator;
    }
    protected TimeCounter m_RollTimer;
    protected bool m_Rolling => m_RollTimer.m_Timing;
    Vector3 m_rollDirection,m_rollingLookRotation;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_RollTimer = new TimeCounter(F_RollDuration, true);
    }
    protected override void OnAbilityTrigger()
    {
        base.OnAbilityTrigger();
        Vector2 rollAxisDirection = m_MoveAxisInput == Vector2.zero ? new Vector2(0, 1) : m_MoveAxisInput;
        m_rollDirection = base.CalculateMoveDirection(rollAxisDirection);
        m_rollingLookRotation = m_rollDirection;
        m_Animator.BeginRoll(F_RollDuration);
        m_RollTimer.Reset();
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (!m_Rolling)
            return;

        m_RollTimer.Tick(deltaTime);
        EnableHitbox(!m_Rolling);
        if (!m_Rolling)
            m_Animator.EndRoll();
    }
    protected override bool CalculateWeaponFire() => !m_Rolling&& base.CalculateWeaponFire();
    protected override float CalculateMovementSpeedBase() => (m_Rolling? F_RollSpeedMultiple :1)* base.CalculateMovementSpeedBase();
    protected override float CalculateMovementSpeedMultiple() => m_Rolling ? 1f : base.CalculateMovementSpeedMultiple();
    protected override Vector3 CalculateMoveDirection(Vector2 moveAxisInput) => m_Rolling ? m_rollDirection : base.CalculateMoveDirection(moveAxisInput);
    protected override Quaternion GetCharacterRotation() => m_Rolling ? Quaternion.LookRotation(m_rollingLookRotation, Vector3.up) : base.GetCharacterRotation();

    class PlayerAnimatorBeth:WeaponBaseAnimator
    {
        static readonly int HS_T_Roll = Animator.StringToHash("t_roll");
        static readonly int HS_F_RollSpeed = Animator.StringToHash("fm_roll");
        public PlayerAnimatorBeth(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator, _OnAnimEvent)
        {
        }
        public void BeginRoll(float rollDuration)
        {
            m_Animator.SetTrigger(HS_T_Roll);
            m_Animator.SetFloat(HS_F_RollSpeed,  1f/ rollDuration);
        }
        public void EndRoll()
        {
            m_Animator.SetTrigger(HS_T_Activate);
        }
    }
}
