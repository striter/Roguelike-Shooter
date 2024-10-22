﻿using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterBattleAIEliteSavage : EntityCharacterBattleAIElite {
    public float F_AttackMoveMultiply = 2f;
    public override float GetBaseMovementSpeed() => base.GetBaseMovementSpeed()*m_AttackMoveMultiply;
    float m_AttackMoveMultiply = 1f;
    protected override void OnEntityActivate(enum_EntityFlag flag)
    {
        base.OnEntityActivate(flag);
        m_AttackMoveMultiply = 1f;
    }

    protected override void OnAttackAnim(bool startAttack)
    {
        base.OnAttackAnim(startAttack);
        m_AttackMoveMultiply = startAttack ? F_AttackMoveMultiply : 1f;
        OnExpireChange();
    }
}
