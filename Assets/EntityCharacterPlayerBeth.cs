using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterPlayerBeth : EntityCharacterPlayer {
    new PlayerAnimatorBeth m_Animator;
    protected override PlayerAnimator GetAnimatorController(Animator animator,Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent)
    {
        m_Animator= new PlayerAnimatorBeth(animator, _OnAnimEvent);
        return m_Animator;
    }
    protected float f_rollCheck;
    protected bool m_rolling => f_rollCheck > 0;
    Vector3 m_rollDirection,m_rollStartDirection;
    protected override void OnAbilityDown()
    {
        base.OnAbilityDown();

        f_rollCheck = 1f;
        m_rollStartDirection = transform.forward;
        Vector2 rollAxisDirection = m_MoveAxisInput == Vector2.zero ? new Vector2(0, 1) : m_MoveAxisInput;
        m_rollDirection = base.CalculateMoveDirection(rollAxisDirection);
        m_Animator.BeginRoll(rollAxisDirection);
    }

    protected override void OnCharacterUpdate(float deltaTime)
    {
        base.OnCharacterUpdate(deltaTime);
        if (m_rolling)
        {
            f_rollCheck -= deltaTime;
            if (!m_rolling)
                m_Animator.EndRoll();
        }
        EnableHitbox(!m_rolling);
    }
    protected override void OnDead()
    {
        base.OnDead();
        f_rollCheck = 0;
    }

    protected override bool CalculateCanInteract() => m_rolling ? false : base.CalculateCanInteract();
    protected override float CalculateBaseMovementSpeed() => m_rolling?F_MovementSpeed*1.5f:base.CalculateBaseMovementSpeed();
    protected override Vector3 CalculateMoveDirection(Vector2 moveAxisInput) => m_rolling ? m_rollDirection : base.CalculateMoveDirection(moveAxisInput);
    protected override Quaternion CalculateTargetRotation() => m_rolling ? Quaternion.LookRotation(m_rollStartDirection, Vector3.up) : base.CalculateTargetRotation();

    class PlayerAnimatorBeth:PlayerAnimator
    {
        static readonly int HS_T_Roll = Animator.StringToHash("t_roll");
        static readonly int HS_F_RollSpeed = Animator.StringToHash("fm_roll");
        static readonly int HS_F_RollForward = Animator.StringToHash("f_rollForward");
        static readonly int HS_F_RollStrafe = Animator.StringToHash("f_rollStrafe");
        public PlayerAnimatorBeth(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator, _OnAnimEvent)
        {
        }
        public void BeginRoll(Vector2 rollDirection)
        {
            m_Animator.SetTrigger(HS_T_Roll);
            m_Animator.SetFloat(HS_F_RollForward, rollDirection.y);
            m_Animator.SetFloat(HS_F_RollStrafe, rollDirection.x);
            m_Animator.SetFloat(HS_F_RollSpeed, 1f);
        }
        public void EndRoll()
        {
            m_Animator.SetTrigger(HS_T_Activate);
        }
    }
}
