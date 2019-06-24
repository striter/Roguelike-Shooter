using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;
using System;

[RequireComponent(typeof(NavMeshAgent),typeof(NavMeshObstacle))]
public class EntityEnermyBase : EntityBase {
    EnermyAIControllerBasic m_AI;
    BarrageBase m_Barrage;
    float OnAttackTarget(EntityBase target) => m_Barrage.Play(this, target)+m_EntityInfo.m_BarrageDuration.Random();
    bool OnCheckTarget(EntityBase target) => target.B_IsPlayer!=B_IsPlayer && !target.b_IsDead;
    public override void Init(SEntity entityInfo)
    {
        Init(entityInfo, false);
        m_AI = new EnermyAIControllerBasic(this, entityInfo, OnAttackTarget,OnCheckTarget);

        SBarrage barrageInfo = ExcelManager.GetBarrageProperties(entityInfo.m_BarrageIndex);
        switch (barrageInfo.m_BarrageType)
        {
            default: Debug.LogError("Invalid Barrage Type:" + barrageInfo.m_BarrageType); break;
            case enum_BarrageType.Single: m_Barrage = new BarrageBase(barrageInfo);break;
            case enum_BarrageType.Multiple: m_Barrage = new BarrageMultiple(barrageInfo); break;
        }
    }

    public override void SetTarget(EntityBase target)
    {
        base.SetTarget(target);
        m_AI.SetTarget(target);
    }
    protected override void OnDead()
    {
        m_AI.Deactivate();
        m_Barrage.Deactivate();
        base.OnDead();
    }
    protected override void OnDisable()
    {
        m_AI.Deactivate();
        m_Barrage.Deactivate();
        base.OnDisable();
    }
    class EnermyAIControllerBasic:ISingleCoroutine
    {
        protected EntityEnermyBase m_EntityControlling;
        protected EntityBase m_Target;
        protected Transform transform => m_EntityControlling.tf_Head;
        protected Transform targetTransform => m_Target.tf_Head;
        protected NavMeshAgent m_Agent;
        protected NavMeshObstacle m_Obstacle;
        protected Func<EntityBase,float> OnAttackTarget;
        protected Func<EntityBase, bool> OnCheckTarget;
        protected float f_AttackRange,f_ChaseRange;
        protected bool b_battleCheckObstacle,b_movementCheckObstacle;
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

        public EnermyAIControllerBasic(EntityEnermyBase _entityControlling, SEntity _entityInfo, Func<EntityBase, float> _onAttack, Func<EntityBase, bool> _onCheck)
        {
            m_EntityControlling = _entityControlling;
            f_AttackRange = _entityInfo.m_AIAttackRange;
            f_ChaseRange = _entityInfo.m_AIChaseRange;
            b_battleCheckObstacle = _entityInfo.m_BattleCheckObsatacle;
            b_movementCheckObstacle = _entityInfo.m_MovementCheckObstacle;
            m_Obstacle = m_EntityControlling.GetComponent<NavMeshObstacle>();
            m_Agent = m_EntityControlling.GetComponent<NavMeshAgent>();
            m_Agent.speed = _entityInfo.m_moveSpeed;
            OnAttackTarget = _onAttack;
            OnCheckTarget = _onCheck;
            B_AgentEnabled = false;
        }

        public void SetTarget(EntityBase entity)
        {
            m_Target = entity;
            B_AgentEnabled = true;
            this.StartSingleCoroutine(0, TrackTarget());
        }

        public void Deactivate()
        {
            B_AgentEnabled = false;
            this.StopAllSingleCoroutines();
        }
        RaycastHit[] m_Raycasts;
        float f_aiSimulatedTime;
        float f_movementCheck,f_battleStatusCheck;
        Vector3 v3_TargetDirection;
        bool b_ChasedTarget;
        bool b_CanAttackTarget;
        bool b_AgentReachDestination;
        bool b_idled = false;
        bool b_targetVisible;
        IEnumerator TrackTarget()
        {
            for (; ; )
            {
                f_aiSimulatedTime += GameConst.F_EnermyAICheckTime;
                CalculateAllParams();
                CheckBattle();
                CheckMovement();
                yield return new WaitForSeconds(GameConst.F_EnermyAICheckTime);
            }
        }
        void CalculateAllParams()
        {
            v3_TargetDirection = (targetTransform.position -transform.position).normalized;
            b_targetVisible = CheckTargetVisible();
            b_ChasedTarget = (!b_movementCheckObstacle || b_targetVisible) && TCommon.GetXZDistance(targetTransform.position, transform.position) < f_ChaseRange;
            b_CanAttackTarget = (!b_battleCheckObstacle || b_targetVisible) && TCommon.GetXZDistance(targetTransform.position, transform.position) < f_AttackRange;
            b_AgentReachDestination = m_Agent.destination == Vector3.zero || TCommon.GetXZDistance(transform.position, m_Agent.destination) < 5f;
        }
        void CheckMovement()
        {
            if (f_aiSimulatedTime < f_movementCheck)
                return;

            if (!b_idled && b_AgentReachDestination && b_ChasedTarget && UnityEngine.Random.Range(0, 2) > 0)
            {
                b_idled = true;
                B_AgentEnabled = false;
                f_movementCheck= f_aiSimulatedTime+ UnityEngine.Random.Range(2f, 3f);
                return;
            }

            b_idled = false;
            B_AgentEnabled = true;

            if (AgentStucked())
                m_Agent.SetDestination(GetUnstuckPosition());
            else if (b_AgentReachDestination)
                m_Agent.SetDestination(GetSamplePosition());
        }
        void CheckBattle()
        {
            if (f_aiSimulatedTime < f_battleStatusCheck)
                return;

            if (b_CanAttackTarget)
            {
                float barrageDuration = OnAttackTarget(m_Target);
                f_battleStatusCheck = f_aiSimulatedTime + barrageDuration;
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
            return NavMesh.SamplePosition(m_EntityControlling.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f)), out sampleHit, 50, -1) ? sampleHit.position : Vector3.zero;
        }

        NavMeshHit sampleHit;
        Vector3 GetSamplePosition()
        {
            Vector3 targetPosition= m_Target.transform.position;
            Vector3 direction = m_EntityControlling.transform.position - m_Target.transform.position;
            Vector3 m_SamplePosition= m_EntityControlling.transform.position+ (b_ChasedTarget?direction:-direction).normalized*10;
            m_SamplePosition = m_SamplePosition + new Vector3(UnityEngine.Random.Range(-15f, 15f), 0, UnityEngine.Random.Range(-15f, 15f));
            if (NavMesh.SamplePosition(m_SamplePosition, out sampleHit, 50, -1))
                targetPosition = sampleHit.position;
            else if (NavMesh.SamplePosition(m_Target.transform.position, out sampleHit, 20, -1))
                targetPosition = sampleHit.position;
            return targetPosition;
        }
        bool CheckTargetVisible()
        {
                m_Raycasts = Physics.RaycastAll(m_EntityControlling.tf_Head.position, v3_TargetDirection, Vector3.Distance(m_EntityControlling.tf_Head.position, m_Target.tf_Head.position), GameLayer.Physics.I_StaticEntity);
                for (int i = 0; i < m_Raycasts.Length; i++)
                {
                    if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Static)
                        return false;
                    else if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Entity)
                    {
                        HitCheckEntity entity = m_Raycasts[i].collider.GetComponent<HitCheckEntity>();
                        if (entity.m_Attacher.I_EntityID != m_Target.I_EntityID && entity.m_Attacher.I_EntityID != m_EntityControlling.I_EntityID)
                            return false;
                    }
                }
                return true;
        }
    }
    class BarrageBase : ISingleCoroutine
    {
        protected SBarrage m_Info;
        protected EntityBase m_EntityControlling;
        protected EntityBase m_Target;
        protected Transform transform => m_EntityControlling.tf_Head;
        protected Transform targetTransform => m_Target.tf_Head;
        protected bool b_preAim;

        public BarrageBase(SBarrage barrageInfo)
        {
            m_Info = barrageInfo;
        }

        public float Play(EntityBase _controller, EntityBase _target)
        {
            m_EntityControlling = _controller;
            m_Target = _target;
            b_preAim = UnityEngine.Random.Range(0, 2) > 0;
            int count = m_Info.m_ProjectileCount.Random();
            this.StartSingleCoroutine(0, TIEnumerators.TickCount(BarrageWave,count, m_Info.m_Firerate));
            return count * m_Info.m_Firerate;
        }

        protected Vector3 targetDirection=> Vector3.Normalize((b_preAim ? (m_Target.m_PrecalculatedTargetPos(Vector3.Distance(targetTransform.position, transform.position) / m_Info.m_ProjectileSpeed)) : targetTransform.position) - transform.position);
        protected virtual void BarrageWave()
        {
            Vector3 horizontalOffsetDirection = GameExpression.V3_FireDirectionSpread(targetDirection,m_Info.m_HorizontalSpread,Vector3.zero,targetTransform.right);
            Vector3 spreadDirection = GameExpression.V3_FireDirectionSpread(horizontalOffsetDirection, m_Info.m_ProjectileSpread, targetTransform.up, targetTransform.right);
            FireBullet(spreadDirection);
        }
        protected void FireBullet(Vector3 direction)
        {
            (ObjectManager.SpawnSFX(m_Info.m_ProjectileType, transform) as SFXProjectile).PlayBarrage(m_EntityControlling.I_EntityID, direction,targetTransform.position, m_Info);
        }
        public void Deactivate()
        {
            this.StopAllSingleCoroutines();
        }
    }
    class BarrageMultiple : BarrageBase
    {
        public BarrageMultiple(SBarrage barrageInfo) : base(barrageInfo)
        {
        }
        protected override void BarrageWave()
        {
            int waveCount = m_Info.m_RangeExtension.Random();
            Vector3 horizontalOffsetDirection = GameExpression.V3_FireDirectionSpread(targetDirection, m_Info.m_HorizontalSpread, Vector3.zero, targetTransform.right);
            horizontalOffsetDirection = Vector3.Normalize(horizontalOffsetDirection * 100 - targetTransform.right * m_Info.m_OffsetExtension * (waveCount-1)/2f).normalized;
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 offsetDirection = Vector3.Normalize(horizontalOffsetDirection * 100 + targetTransform.right * m_Info.m_OffsetExtension*i).normalized;
                Vector3 spreadDirection = GameExpression.V3_FireDirectionSpread(offsetDirection, m_Info.m_ProjectileSpread, targetTransform.up, targetTransform.right);
                FireBullet(spreadDirection);
            }
        }
    }
}
