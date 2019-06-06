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
            m_AI = new EnermyAIControllerBasic(this,entityInfo);
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
        protected EntityEnermyBase m_EntityControlling;
        protected NavMeshAgent m_Agent;
        protected NavMeshObstacle m_Obstacle;
        protected float f_AttackRange;
        public bool B_AgentEnabled
        {
            get
            {
                return m_Agent.enabled;
            }
            set
            {
                if (value == m_Agent.enabled)
                    return;

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

        public EnermyAIControllerBasic(EntityEnermyBase _entityControlling, SEntity _entityInfo)
        {
            m_EntityControlling = _entityControlling;
            f_AttackRange = _entityInfo.m_AIAttackRange;
            m_Obstacle = m_EntityControlling.GetComponent<NavMeshObstacle>();
            m_Agent = m_EntityControlling.GetComponent<NavMeshAgent>();
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
        RaycastHit[] m_Raycasts;
        bool b_NeedTracking => TCommon.GetXZDistance(m_EntityControlling.transform.position, m_Target.transform.position) < f_AttackRange;
        bool b_TargetVisible {
            get
            {
                m_Raycasts= Physics.RaycastAll(m_EntityControlling.tf_Head.position, m_Target.tf_Head.position - m_EntityControlling.tf_Head.position,Vector3.Distance(m_EntityControlling.tf_Head.position, m_Target.tf_Head.position), GameLayer.Physics.I_StaticEntity);
                for (int i = 0; i < m_Raycasts.Length; i++)
                {
                    if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Static)
                        return false;
                    else if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Entity)
                    {   
                        HitCheckEntity entity = m_Raycasts[i].collider.GetComponent<HitCheckEntity>();
                        if (entity.m_Attacher.I_EntityID!=m_Target.I_EntityID&& entity.m_Attacher.I_EntityID != m_EntityControlling.I_EntityID)
                            return false;
                    }
                }
                return true;
            }
        } 
        bool b_AgentReachDestination => m_Agent.destination==Vector3.zero||TCommon.GetXZDistance(m_EntityControlling.transform.position, m_Agent.destination) < 5f;
        bool b_idled = false;
        IEnumerator TrackTarget()
        {
            for (; ; )
            {
                if (!b_idled&&b_AgentReachDestination && b_TargetVisible && Random.Range(0, 2) > 0)
                {
                    b_idled = true;
                    B_AgentEnabled = false;
                    yield return new WaitForSeconds(Random.Range(2f, 3f));
                    continue;
                }
                b_idled = false;
                B_AgentEnabled = true;
                if (AgentStucked())
                    m_Agent.SetDestination(GetUnstuckPosition());
                else if (b_AgentReachDestination)
                    m_Agent.SetDestination(GetSamplePosition());
                yield return new WaitForSeconds(GameConst.F_EnermyAICheckTime);
            }
        }
        int stuckCount = 0;
        Vector3 previousPos=Vector3.zero;
        bool AgentStucked()
        {
            stuckCount = (previousPos != Vector3.zero && Vector3.Distance(previousPos, m_EntityControlling.transform.position) < .5f) ? stuckCount + 1 : 0;
            previousPos = m_EntityControlling.transform.position;
            return stuckCount>3;
        }
        Vector3 GetUnstuckPosition()
        {
            return NavMesh.SamplePosition(m_EntityControlling.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)), out sampleHit, 50, -1) ? sampleHit.position : Vector3.zero;
        }

        NavMeshHit sampleHit;
        Vector3 GetSamplePosition()
        {
            Vector3 targetPosition= m_Target.transform.position;
            Vector3 direction = m_EntityControlling.transform.position - m_Target.transform.position;
            Vector3 m_SamplePosition= m_EntityControlling.transform.position+ (b_NeedTracking?direction:-direction).normalized*10;
            m_SamplePosition = m_SamplePosition + new Vector3(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f));
            if (NavMesh.SamplePosition(m_SamplePosition, out sampleHit, 50, -1))
                targetPosition = sampleHit.position;
            else if (NavMesh.SamplePosition(m_Target.transform.position, out sampleHit, 20, -1))
                targetPosition = sampleHit.position;
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
