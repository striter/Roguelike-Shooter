using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;

[RequireComponent(typeof(NavMeshAgent),typeof(NavMeshObstacle))]
public class EntityEnermyBase : EntityBase {
    EnermyAIControllerBasic m_AI;
    public override void Init(int id, SEntity entityInfo)
    {
        base.Init(id, entityInfo);
        if(m_AI==null)
            m_AI = new EnermyAIControllerBasic(transform,entityInfo);
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
        m_AI.OnDead();
        base.OnDead();
    }

    class EnermyAIControllerBasic:ISingleCoroutine
    {
        protected NavMeshAgent m_Agent;
        protected NavMeshObstacle m_Obstacle;
        protected Transform transform;
        protected float f_AttackRange;
        public bool B_AgentEnabled
        {
            get
            {
                return m_Agent.enabled;
            }
            set
            {
                if (value)
                {
                    m_Obstacle.enabled = false;
                    m_Agent.enabled = true;
                    m_Agent.isStopped = false;
                }
                else
                {
                    if (m_Agent.enabled)
                    {
                        m_Agent.isStopped = true;
                        m_Agent.enabled = false;
                    }
                    m_Obstacle.enabled = true;
                }
            }
        }

        public EnermyAIControllerBasic(Transform _transform,SEntity _entityInfo)
        {
            transform = _transform;
            f_AttackRange = _entityInfo.m_AIAttackRange;
            m_Obstacle = transform.GetComponent<NavMeshObstacle>();
            m_Agent = transform.GetComponent<NavMeshAgent>();
            m_Agent.speed = _entityInfo.m_moveSpeed;
            B_AgentEnabled = false;
        }
        public void SetTarget(EntityBase entity)
        {
            m_Target = entity;
            B_AgentEnabled = true;
            this.StartSingleCoroutine(0, TrackTarget());
        }

        EntityBase m_Target;
        bool b_NeedTracking => TCommon.GetXZDistance(transform.position, m_Target.transform.position) < f_AttackRange;
        bool b_TargetVisible => !Physics.Raycast(transform.position, m_Target.transform.position- transform.position, GameLayer.Physics.I_StaticEntity);
        bool b_AgentReachDestination => m_Agent.destination==Vector3.zero||TCommon.GetXZDistance(transform.position, m_Agent.destination) < 2f;

        IEnumerator TrackTarget()
        {
            for (; ; )
            {
                if (b_TargetVisible&&Random.Range(0,2)>0)
                {
                    B_AgentEnabled = false;
                    yield return new WaitForSeconds(Random.Range(2f,3f));
                    continue;
                }

                if (b_AgentReachDestination)
                {
                    B_AgentEnabled = true;
                    m_Agent.SetDestination(GetSamplePosition());
                }

                yield return new WaitForSeconds(GameConst.F_EnermyAICheckTime);
            }
        }
        Vector3 GetSamplePosition()
        {
            Vector3 targetPosition= m_Target.transform.position;
            Vector3 direction = transform.position - m_Target.transform.position;
            Vector3 m_SamplePosition= transform.position+ (b_NeedTracking?direction:-direction).normalized*10;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(m_SamplePosition, out hit, 10, -1))
            {
                m_SamplePosition = m_SamplePosition + new Vector3(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f));
                if (NavMesh.SamplePosition(m_SamplePosition, out hit, 10, -1))
                    targetPosition = hit.position;
            }
            return targetPosition;
        }
        public void OnDestroy()
        {
            this.StopAllSingleCoroutines();
        }
        public void OnDead()
        {
            OnDestroy();
            B_AgentEnabled = false;
        }
    }
}
