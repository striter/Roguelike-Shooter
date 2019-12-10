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
    protected override PlayerAnimator GetAnimatorController(Animator animator,Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent)
    {
        m_Animator= new PlayerAnimatorBeth(animator, _OnAnimEvent);
        return m_Animator;
    }
    protected float f_rollCheck;
    protected bool m_rolling => f_rollCheck > 0;
    Vector3 m_rollDirection,m_rollingLookRotation;
    protected override void OnAbilityTrigger()
    {
        base.OnAbilityTrigger();
        f_rollCheck = F_RollDuration;
        Vector2 rollAxisDirection = m_MoveAxisInput == Vector2.zero ? new Vector2(0, 1) : m_MoveAxisInput;
        bool forward = Vector2.Angle(new Vector2(0, -1), rollAxisDirection) > 60;
        m_rollDirection = base.CalculateMoveDirection(rollAxisDirection);
        m_rollingLookRotation = (forward?1:-11)*m_rollDirection;
        m_Animator.BeginRoll(forward,F_RollDuration);
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (m_rolling)
        {
            f_rollCheck -= deltaTime;
            if (!m_rolling)
                m_Animator.EndRoll();
        }
        //EnableHitbox(!m_rolling);
    }
    protected override void OnDead()
    {
        base.OnDead();
        f_rollCheck = -1;
    }
    protected override bool CalculateWeaponFire() => !m_rolling&& base.CalculateWeaponFire();
    protected override float CalculateMovementSpeedBase() => (m_rolling? F_RollSpeedMultiple :1)* base.CalculateMovementSpeedBase();
    protected override float CalculateMovementSpeedMultiple() => m_rolling ? 1f : base.CalculateMovementSpeedMultiple();
    protected override Vector3 CalculateMoveDirection(Vector2 moveAxisInput) => m_rolling ? m_rollDirection : base.CalculateMoveDirection(moveAxisInput);
    protected override Quaternion GetCharacterRotation() => m_rolling ? Quaternion.LookRotation(m_rollingLookRotation, Vector3.up) : base.GetCharacterRotation();

    class PlayerAnimatorBeth:PlayerAnimator
    {
        static readonly int HS_T_Roll = Animator.StringToHash("t_roll");
        static readonly int HS_F_RollSpeed = Animator.StringToHash("fm_roll");
        static readonly int HS_F_RollForward = Animator.StringToHash("b_rollForward");
        public PlayerAnimatorBeth(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator, _OnAnimEvent)
        {
        }
        public void BeginRoll(bool forward,float rollDuration)
        {
            m_Animator.SetTrigger(HS_T_Roll);
            m_Animator.SetBool(HS_F_RollForward, forward);
            m_Animator.SetFloat(HS_F_RollSpeed,  1f/ rollDuration);
        }
        public void EndRoll()
        {
            m_Animator.SetTrigger(HS_T_Activate);
        }
    }
}
