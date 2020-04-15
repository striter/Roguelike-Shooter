﻿using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterAIEliteSavage : EntityCharacterAIElite {
    public float F_AttackMoveMultiply = 2f;
    public override float m_baseMovementSpeed => base.m_baseMovementSpeed*m_AttackMoveMultiply;
    float m_AttackMoveMultiply = 1f;
    protected override void OnEntityActivate(enum_EntityFlag flag, float startHealth = 0)
    {
        base.OnEntityActivate(flag, startHealth);
        m_AttackMoveMultiply = 1f;
    }

    protected override void OnAttackAnim(bool startAttack)
    {
        base.OnAttackAnim(startAttack);
        m_AttackMoveMultiply = startAttack ? F_AttackMoveMultiply : 1f;
        OnExpireChange();
    }
}
