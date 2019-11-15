using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterPlayerBeth : EntityCharacterPlayer {

    protected float f_rollCheck;
    protected bool m_rolling => f_rollCheck > 0;
    Vector3 m_rollDirection,m_rollStartDirection;
    protected override void OnAbilityDown()
    {
        base.OnAbilityDown();

        f_rollCheck = 1f;
        m_rollStartDirection = transform.forward;
        m_rollDirection = base.CalculateMoveDirection(m_MoveAxisInput==Vector2.zero?new Vector2(0,1):m_MoveAxisInput);
    }

    protected override void OnCharacterUpdate(float deltaTime)
    {
        base.OnCharacterUpdate(deltaTime);
        if (m_rolling) f_rollCheck -= deltaTime;
        EnableHitbox(m_rolling);
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
}
