using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;

[RequireComponent(typeof(NavMeshAgent),typeof(NavMeshObstacle))]
public class EntityEnermyBase : EntityBase {
    NavigationAgentAI<EntityBase> m_AI;
    public override void Init(int id, SEntity entityInfo)
    {
        base.Init(id, entityInfo);
        if(m_AI==null)
            m_AI = new NavigationAgentAI<EntityBase>(transform,GameLayer.I_Static,entityInfo.m_moveSpeed,20,10,5, OnCheckTarget, OnAttackTarget);
    }
    public override void SetTarget(EntityBase target)
    {
        base.SetTarget(target);
        m_AI.SetTarget(target);
    }
    bool OnCheckTarget(EntityBase target,bool isAlly)
    {
        return target.B_IsPlayer&&!target.b_IsDead;
    }
    float OnAttackTarget(EntityBase target)
    {
        return .5f;
    }
    protected override void OnDead()
    {
        base.OnDead();
        m_AI.OnDead();
    }
}
