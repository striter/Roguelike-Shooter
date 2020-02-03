using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterAIEliteSavage : EntityCharacterAIElite {
    public float F_AttackMoveMultiply = 2f;
    public override float m_baseMovementSpeed => base.m_baseMovementSpeed*m_AttackMoveMultiply;
    float m_AttackMoveMultiply = 1f;
    public override void OnActivate(enum_EntityFlag _flag, int _spawnerID, float startHealth)
    {
        base.OnActivate(_flag, _spawnerID, startHealth);
        m_AttackMoveMultiply =  1f;
    }
    protected override void OnAttackAnim(bool startAttack)
    {
        base.OnAttackAnim(startAttack);
        m_AttackMoveMultiply = startAttack ? F_AttackMoveMultiply : 1f;
        OnExpireChange();
    }
}
