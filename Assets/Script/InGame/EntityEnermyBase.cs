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
    EnermyAIControllerBase m_AI;
    EnermyAnimator m_Animator;
    bool OnCheckTarget(EntityBase target) => target.B_IsPlayer!=B_IsPlayer && !target.b_IsDead;
    public override void Init(SEntity entityInfo)
    {
        Init(entityInfo, false);
        Transform tf_Barrel = transform.FindInAllChild("Barrel");
        EnermyWeaponBase weapon=null;
        switch (entityInfo.m_WeaponType)
        {
            default: Debug.LogError("Invalid Barrage Type:" + entityInfo.m_WeaponType); break;
            case enum_EnermyWeaponType.Single: weapon = new BarrageRange(this,tf_Barrel, m_EntityInfo.m_ProjectileSFX); break;
            case enum_EnermyWeaponType.MultipleFan: weapon = new BarrageMultipleFan(this,tf_Barrel, m_EntityInfo.m_ProjectileSFX); break;
            case enum_EnermyWeaponType.MultipleLine: weapon = new BarrageMultipleLine(this,tf_Barrel, m_EntityInfo.m_ProjectileSFX);break;
            case enum_EnermyWeaponType.Melee: weapon = new EnermyMelee(this,tf_Barrel); break;
        }
        m_AI = new EnermyAIControllerBase(this,weapon, entityInfo, OnFireAnim, OnCheckTarget);
        if (E_AnimatorIndex == EnermyAnimator.enum_AnimIndex.Invalid)
            Debug.LogError("Please Set Prefab AnimIndex!");
    }
    public override void OnSpawn(int id)
    {
        base.OnSpawn(id);
        m_Animator = new EnermyAnimator(tf_Model.GetComponent<Animator>(), E_AnimatorIndex, OnAnimKeyEvent);
    }
    protected override void Update()
    {
        m_Animator.SetRun(0,m_AI.B_AgentEnabled?1:0);
    }
    void OnFireAnim(EntityBase target)
    {
        m_Animator.OnAttack();
    }
    protected void OnAnimKeyEvent(EnermyAnimator.enum_AnimEvent animEvent)
    {
        switch (animEvent)
        {
            case EnermyAnimator.enum_AnimEvent.Fire:
                m_AI.OnFireAnimTriggered();
                break;
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
        m_Animator.OnDead();
        base.OnDead();
    }
    protected override void OnDisable()
    {
        m_AI.Deactivate();
        base.OnDisable();
    }


    public class EnermyAnimator : AnimatorClippingTime
    {
        public enum enum_AnimIndex
        {
            Invalid=-1,
            Axe=1,
            Spear=2,
            Bow=3,
            CastL=4,
            CastR=5,
            Sword=6,
            Pistol_L=7
        }
        public enum enum_AnimEvent
        {
            Invalid=-1,
            Fire=1,
            FootL,
            FootR,
            Death,
        }
        static readonly int HS_T_Dead = Animator.StringToHash("t_dead");
        static readonly int HS_I_AnimIndex = Animator.StringToHash("i_weaponType");
        static readonly int HS_T_Activate = Animator.StringToHash("t_activate");
        static readonly int HS_T_Attack = Animator.StringToHash("t_attack");
        static readonly int HS_F_Strafe = Animator.StringToHash("f_strafe");
        static readonly int HS_F_Forward = Animator.StringToHash("f_forward");
        Action<enum_AnimEvent> OnAnimEvent;
        public EnermyAnimator(Animator _animator, enum_AnimIndex _animIndex,Action<enum_AnimEvent> _OnAnimEvent) : base(_animator)
        {
            m_Animator.fireEvents = true;
            m_Animator.SetInteger(HS_I_AnimIndex,(int)_animIndex);
            m_Animator.SetTrigger(HS_T_Activate);
            OnAnimEvent = _OnAnimEvent;
            m_Animator.GetComponent<TAnimatorEvent>().Attach((string eventName)=> {
                OnAnimEvent((enum_AnimEvent)Enum.Parse(typeof(enum_AnimEvent), eventName));
            });
        }
        public void SetRun(float strafe,float forward)
        {
            m_Animator.SetFloat(HS_F_Strafe, strafe);
            m_Animator.SetFloat(HS_F_Forward, forward);
        }
        public void OnAttack()
        {
            m_Animator.SetTrigger(HS_T_Attack);
        }
        public void OnDead()
        {
            m_Animator.SetTrigger(HS_T_Dead);
        }
    }
    #region AI
    class EnermyAIControllerBase : ISingleCoroutine
    {
        protected EntityEnermyBase m_EntityControlling;
        protected EntityBase m_Target;
        protected Transform transform => m_Agent.transform;
        protected Transform headTransform => m_EntityControlling.tf_Head;
        protected Transform targetHeadTransform => m_Target.tf_Head;
        protected NavMeshAgent m_Agent;
        protected NavMeshObstacle m_Obstacle;
        protected Action<EntityBase> OnFireAnimStart;
        protected Func<EntityBase, bool> OnCheckTarget;
        protected SEntity m_EntityInfo;
        protected EnermyWeaponBase m_Weapon;
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

        public EnermyAIControllerBase(EntityEnermyBase _entityControlling, EnermyWeaponBase _weapon, SEntity _entityInfo, Action<EntityBase> _onAttack, Func<EntityBase, bool> _onCheck)
        {
            m_EntityControlling = _entityControlling;
            m_EntityInfo = _entityInfo;
            m_Weapon = _weapon;
            m_Obstacle = m_EntityControlling.GetComponent<NavMeshObstacle>();
            m_Agent = m_EntityControlling.GetComponent<NavMeshAgent>();
            m_Agent.speed = _entityInfo.m_moveSpeed;
            OnFireAnimStart = _onAttack;
            OnCheckTarget = _onCheck;
            B_AgentEnabled = false;
        }

        public void SetTarget(EntityBase entity)
        {
            m_Target = entity;
            B_AgentEnabled = true;
            this.StartSingleCoroutine(0, TrackTarget());
            this.StartSingleCoroutine(1, Tick());
        }

        public void Deactivate()
        {
            B_AgentEnabled = false;
            m_Weapon.Deactivate();
            this.StopAllSingleCoroutines();
        }
        RaycastHit[] m_Raycasts;
        float f_aiSimulatedTime;
        float f_movementCheck, f_battleStatusCheck;
        Vector3 v3_TargetDirection;
        float f_targetDistance;
        bool b_targetOutChaseRange;
        bool b_targetOutAttackRange;
        bool b_MoveTowardsTarget;
        bool b_CanAttackTarget;
        bool b_AgentReachDestination;
        bool b_idled = false;
        float f_rotateParam=1f;
        bool b_targetVisible;
        int i_targetUnvisibleCount;
        float f_targetAngle;
        bool b_targetHideBehindWall => i_targetUnvisibleCount == 40;

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

        IEnumerator Tick()
        {
            for (; ; )
            {
                v3_TargetDirection = TCommon.GetXZLookDirection(headTransform.position, targetHeadTransform.position);
                f_targetAngle = TCommon.GetAngle(v3_TargetDirection,transform.forward,Vector3.up);
                m_Agent.updateRotation = b_targetOutAttackRange;
                if (!m_Agent.updateRotation)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(v3_TargetDirection, Vector3.up), .2f* f_rotateParam);
                yield return null;
            }
        }

        void CalculateAllParams()
        {
            b_targetVisible = CheckTargetVisible();
            if (!b_targetVisible)
                i_targetUnvisibleCount=i_targetUnvisibleCount+1>40?40:i_targetUnvisibleCount+1;
            else
                i_targetUnvisibleCount= i_targetUnvisibleCount-1<=0?0:i_targetUnvisibleCount-1;
            f_targetDistance = TCommon.GetXZDistance(targetHeadTransform.position, headTransform.position);
            b_targetOutChaseRange = f_targetDistance > m_EntityInfo.m_AIChaseRange;
            b_targetOutAttackRange = f_targetDistance > m_EntityInfo.m_AIAttackRange;
            b_MoveTowardsTarget = b_targetHideBehindWall || b_targetOutChaseRange;
            b_CanAttackTarget =  !b_targetOutAttackRange&&(!m_EntityInfo.m_BattleCheckObsatacle || b_targetVisible) && Mathf.Abs(f_targetAngle)<15 ;
            b_AgentReachDestination = m_Agent.destination == Vector3.zero || TCommon.GetXZDistance(headTransform.position, m_Agent.destination) < 1f;
        }

        void CheckBattle()
        {
            if (f_aiSimulatedTime < f_battleStatusCheck)
                return;
            if (b_CanAttackTarget)
            {
                float barrageDuration = m_Weapon.Preplay(m_Target) + m_EntityInfo.m_BarrageDuration.Random();
                f_battleStatusCheck = f_aiSimulatedTime + barrageDuration;
                OnFireAnimStart(m_Target);
            }
        }

        public void OnFireAnimTriggered()
        {
            m_Weapon.Play();
        }

        void CheckMovement()
        {
            if (f_aiSimulatedTime < f_movementCheck)
                return;

            if (!b_idled && b_AgentReachDestination && !b_targetOutChaseRange && UnityEngine.Random.Range(0, 2) > 0)
            {
                b_idled = true;
                B_AgentEnabled = false;
                f_movementCheck = f_aiSimulatedTime + UnityEngine.Random.Range(2f, 3f);
                return;
            }

            b_idled = false;
            B_AgentEnabled = true;

            if (AgentStucked())
                m_Agent.SetDestination(GetUnstuckPosition());
            else if (b_AgentReachDestination)
                m_Agent.SetDestination(GetSamplePosition());
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
            return NavMesh.SamplePosition(m_EntityControlling.transform.position + new Vector3( UnityEngine.Random.Range(-10f, 10f), 0, 0), out sampleHit, 50, -1) ? sampleHit.position : Vector3.zero;
        }

        NavMeshHit sampleHit;
        Vector3 GetSamplePosition()
        {
            Vector3 targetPosition = targetHeadTransform.position;
            Vector3 m_SamplePosition= m_EntityControlling.transform.position+ (b_MoveTowardsTarget? v3_TargetDirection : -v3_TargetDirection).normalized*5;
            m_SamplePosition = m_SamplePosition + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
            if (NavMesh.SamplePosition(m_SamplePosition, out sampleHit, 100, -1))
                targetPosition = sampleHit.position;
            else if (NavMesh.SamplePosition(m_Target.transform.position, out sampleHit, 100, -1))
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
    #endregion
    #region EnermyWeapon
    class EnermyWeaponBase : ISingleCoroutine
    {
        protected EntityEnermyBase m_EntityControlling;
        protected EntityBase m_Target;
        protected Transform attacherTransform => m_EntityControlling.tf_Head;
        protected Transform targetTransform => m_Target.tf_Head;
        protected Transform transform;
        public virtual bool B_ActivateLockRotation => false;
        protected SEntity m_Info
        {
            get
            {
                if (m_EntityControlling == null)
                    Debug.LogError("Null Entity Controlling?");
                return m_EntityControlling.m_EntityInfo;
            }
        }
        public EnermyWeaponBase(EntityEnermyBase _controller,Transform _transform)
        {
            m_EntityControlling = _controller;
            transform = _transform;
        }
        public virtual float Preplay( EntityBase _target)
        {
            m_Target = _target;
            return int.MaxValue;
        }
        public virtual void Play()
        {
            Debug.LogError("Override This Please");
        }
        public void Deactivate()
        {
            this.StopAllSingleCoroutines();
        }
    }
    class EnermyMelee : EnermyWeaponBase
    {
        public override bool B_ActivateLockRotation => true;
        public EnermyMelee(EntityEnermyBase _controller, Transform _transform) : base(_controller, _transform)
        {
        }
        public override float Preplay( EntityBase _target)
        {
            base.Preplay(_target);
            return m_Info.m_Firerate;
        }
        public override void Play()
        {
            if (m_Info.m_MuzzleSFX != -1)
                ObjectManager.SpawnSFX<SFXParticles>(m_Info.m_MuzzleSFX, attacherTransform.position,attacherTransform.forward);
            Vector3 meleeDirection = attacherTransform.forward;
            ObjectManager.SpawnSFX<SFXCast>(m_Info.m_ProjectileSFX, attacherTransform.position, meleeDirection).Play(m_EntityControlling.I_EntityID);
        }
    }
    class BarrageRange : EnermyWeaponBase
    {
        protected bool b_preAim;
        protected int i_projectileCount;
        protected float f_projectileSpeed;
        public BarrageRange(EntityEnermyBase _controller, Transform _transform,int _projectileIndex) : base(_controller, _transform)
        {
            
            f_projectileSpeed = _projectileIndex == -1 ? 0 : ObjectManager.GetSFX<SFXProjectile>(_projectileIndex).F_Speed; ;
        }
        public override float Preplay(EntityBase _target)
        {
            base.Preplay(_target);
            b_preAim = UnityEngine.Random.Range(0, 2) > 0;
            i_projectileCount = m_Info.m_ProjectileCount.Random();
            return i_projectileCount * m_Info.m_Firerate;
        }
        public override void Play()
        {
            this.StartSingleCoroutine(0, TIEnumerators.TickCount(BarrageWave, i_projectileCount, m_Info.m_Firerate));
        }

        protected Vector3 GetHorizontalDirection()
        {
            Vector3 targetDirection= Vector3.Normalize((b_preAim ? (m_Target.m_PrecalculatedTargetPos(Vector3.Distance(targetTransform.position, attacherTransform.position) /f_projectileSpeed)) : targetTransform.position) - attacherTransform.position);
            targetDirection.y = 0;
            return targetDirection.normalized;
        }
        protected virtual void BarrageWave()
        {
            Vector3 horizontalOffsetDirection = GameExpression.V3_RangeSpreadDirection(GetHorizontalDirection(), m_Info.m_HorizontalSpread, Vector3.zero, targetTransform.right);
            FireBullet(transform.position, horizontalOffsetDirection);
        }
        protected void FireBullet(Vector3 startPosition,Vector3 direction)
        {
            if (m_Info.m_MuzzleSFX != -1)
                ObjectManager.SpawnSFX<SFXParticles>(m_Info.m_MuzzleSFX, startPosition, direction).Play(m_EntityControlling.I_EntityID);
            ObjectManager.SpawnSFX<SFXProjectile>(m_Info.m_ProjectileSFX, startPosition, direction).Play(m_EntityControlling.I_EntityID, direction, targetTransform.position);
        }
    }
    class BarrageMultipleLine : BarrageRange
    {
        public BarrageMultipleLine(EntityEnermyBase _controller, Transform _transform, int _projectileIndex) : base(_controller, _transform, _projectileIndex)
        {
        }
        protected override void BarrageWave()
        {
            base.BarrageWave();
            int waveCount = m_Info.m_RangeExtension.Random();
            Vector3 startDirection = GetHorizontalDirection();
            Vector3 startPosition = transform.position - attacherTransform.right*m_Info.m_OffsetExtension*((waveCount-1)/2f);
            for (int i = 0; i < waveCount; i++)
                FireBullet(startPosition+ attacherTransform.right*m_Info.m_OffsetExtension*i, startDirection);
        }
    }
    class BarrageMultipleFan : BarrageRange
    {
        public BarrageMultipleFan(EntityEnermyBase _controller, Transform _transform, int _projectileIndex) : base(_controller, _transform, _projectileIndex)
        {
        }
        protected override void BarrageWave()
        {
            int waveCount = m_Info.m_RangeExtension.Random();
            Vector3 startDirection = GetHorizontalDirection();
            float startFanAngle= -m_Info.m_OffsetExtension*(waveCount-1)/2f;
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 fanDirection = startDirection.RotateDirection(Vector3.up, startFanAngle + i * m_Info.m_OffsetExtension);
                FireBullet(transform.position, fanDirection);
            }
        }
    }
    #endregion
}
