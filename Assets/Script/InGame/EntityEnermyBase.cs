using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;
using System;

[RequireComponent(typeof(NavMeshAgent),typeof(NavMeshObstacle))]
public class EntityEnermyBase : EntityBase {
    public EnermyAnimator.enum_AnimIndex E_AnimatorIndex= EnermyAnimator.enum_AnimIndex.Invalid;
    EnermyAIControllerBasic m_AI;
    EnermyWeaponBase m_Barrage;
    EnermyAnimator m_Animator;
    float OnAttackTarget(EntityBase target) => m_Barrage.Play(this, target)+m_EntityInfo.m_BarrageDuration.Random();
    bool OnCheckTarget(EntityBase target) => target.B_IsPlayer!=B_IsPlayer && !target.b_IsDead;
    public override void Init(SEntity entityInfo)
    {
        Init(entityInfo, false);
        m_AI = new EnermyAIControllerBasic(this, entityInfo, OnAttackTarget,OnCheckTarget);
        switch (entityInfo.m_WeaponType)
        {
            default: Debug.LogError("Invalid Barrage Type:" + entityInfo.m_WeaponType); break;
            case enum_EnermyWeaponType.Single: m_Barrage = new BarrageRange(); break;
            case enum_EnermyWeaponType.Multiple: m_Barrage = new BarrageMultiple(); break;
            case enum_EnermyWeaponType.Melee: m_Barrage = new EnermyMelee(); break;
        }
    }
    public override void OnActivate(int id)
    {
        base.OnActivate(id);
        m_Animator = new EnermyAnimator(tf_Model.GetComponent<Animator>(), new List<SAnimatorParam>() { new SAnimatorParam("Death_01", Animator.StringToHash("fp_dead"), 1f) }, E_AnimatorIndex);

    }
    protected override void Update()
    {
        m_Animator.SetRun(m_AI.B_AgentEnabled);
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
        m_Animator.OnDead();
        base.OnDead();
    }
    protected override void OnDisable()
    {
        m_AI.Deactivate();
        m_Barrage.Deactivate();
        base.OnDisable();
    }


    public class EnermyAnimator : AnimatorClippingTime
    {
        public enum enum_AnimIndex
        {
            Invalid=-1,
            Axe=0,
            Spear=1,
        }
        static readonly int HS_B_Run = Animator.StringToHash("b_run");
        static readonly int HS_T_Dead = Animator.StringToHash("t_dead");
        static readonly int HS_I_AnimIndex = Animator.StringToHash("i_weaponType");
        static readonly int HS_T_Activate = Animator.StringToHash("t_activate");
        static readonly int HS_T_Attack = Animator.StringToHash("t_attack");

        public EnermyAnimator(Animator _animator, List<SAnimatorParam> _params, enum_AnimIndex _animIndex) : base(_animator, _params)
        {
            m_Animator.fireEvents = false;
        }
        public void SetRun(bool run)
        {
            m_Animator.SetBool(HS_B_Run, run);
        }
        public void OnDead()
        {
            m_Animator.SetTrigger(HS_T_Dead);
        }
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
            Vector3 m_SamplePosition= m_EntityControlling.transform.position+ (b_ChasedTarget?direction:-direction).normalized*5;
            m_SamplePosition = m_SamplePosition + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
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
    
    class EnermyWeaponBase : ISingleCoroutine
    {
        protected EntityEnermyBase m_EntityControlling;
        protected EntityBase m_Target;
        protected Transform transform => m_EntityControlling.tf_Head;
        protected Transform targetTransform => m_Target.tf_Head;

        protected SEntity m_Info
        {
            get
            {
                if (m_EntityControlling == null)
                    Debug.LogError("Null Entity Controlling?");
                return m_EntityControlling.m_EntityInfo;
            }
        }
        public virtual float Play(EntityEnermyBase _controller, EntityBase _target)
        {
            m_EntityControlling = _controller;
            m_Target = _target;
            return int.MaxValue;
        }
        public void Deactivate()
        {
            this.StopAllSingleCoroutines();
        }
    }
    class EnermyMelee : EnermyWeaponBase
    {
        public override float Play(EntityEnermyBase _controller, EntityBase _target)
        {
            base.Play(_controller,_target);
            this.StartSingleCoroutine(0,TIEnumerators.PauseDel(m_Info.m_Firerate,OnMelee));
            return m_Info.m_Firerate;
        }
        void OnMelee()
        {
            if (m_Info.m_MuzzleSFXIndex != -1)
                ObjectManager.SpawnSFX<SFXParticles>(m_Info.m_MuzzleSFXIndex,transform.position,transform.forward);

            Vector3 meleeDirection =(  targetTransform.position - transform.position).normalized;

            ObjectManager.SpawnSFX<SFXCast>(m_Info.m_BlastSFXIndex, transform.position, meleeDirection).Play(m_EntityControlling.I_EntityID, m_Info.m_ProjectileDamage);

        }
    }
    class BarrageRange : EnermyWeaponBase
    {
        protected bool b_preAim;
        public override float Play(EntityEnermyBase _controller, EntityBase _target)
        {
            base.Play(_controller, _target);
            b_preAim = UnityEngine.Random.Range(0, 2) > 0;
            int count = m_Info.m_ProjectileCount.Random();
            this.StartSingleCoroutine(0, TIEnumerators.TickCount(BarrageWave, count, m_Info.m_Firerate));
            return count * m_Info.m_Firerate;
        }

        protected Vector3 targetDirection => Vector3.Normalize((b_preAim ? (m_Target.m_PrecalculatedTargetPos(Vector3.Distance(targetTransform.position, transform.position) / m_Info.m_ProjectileSpeed)) : targetTransform.position) - transform.position);
        protected virtual void BarrageWave()
        {
            Vector3 horizontalOffsetDirection = GameExpression.V3_RangeSpreadDirection(targetDirection, m_Info.m_HorizontalSpread, Vector3.zero, targetTransform.right);
            Vector3 spreadDirection = GameExpression.V3_RangeSpreadDirection(horizontalOffsetDirection, m_Info.m_DetailSpread, targetTransform.up, targetTransform.right);
            FireBullet(spreadDirection);
        }
        protected void FireBullet(Vector3 direction)
        {
            if (m_Info.m_MuzzleSFXIndex != -1)
                ObjectManager.SpawnSFX<SFXParticles>(m_Info.m_MuzzleSFXIndex, transform.position, transform.forward).Play(m_EntityControlling.I_EntityID);
            ObjectManager.SpawnSFX<SFXProjectile>(m_Info.m_ProjectileSFXIndex, transform.position, transform.forward).PlayBarrage(m_EntityControlling.I_EntityID, direction, targetTransform.position, m_Info);
        }
    }
    class BarrageMultiple : BarrageRange
    {
        protected override void BarrageWave()
        {
            int waveCount = m_Info.m_RangeExtension.Random();
            Vector3 horizontalOffsetDirection = GameExpression.V3_RangeSpreadDirection(targetDirection, m_Info.m_HorizontalSpread, Vector3.zero, targetTransform.right);
            horizontalOffsetDirection = Vector3.Normalize(horizontalOffsetDirection * 100 - targetTransform.right * m_Info.m_OffsetExtension * (waveCount-1)/2f).normalized;
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 offsetDirection = Vector3.Normalize(horizontalOffsetDirection * 100 + targetTransform.right * m_Info.m_OffsetExtension*i).normalized;
                Vector3 spreadDirection = GameExpression.V3_RangeSpreadDirection(offsetDirection, m_Info.m_DetailSpread, targetTransform.up, targetTransform.right);
                FireBullet(spreadDirection);
            }
        }
    }
}
