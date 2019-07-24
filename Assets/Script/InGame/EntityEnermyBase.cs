﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;
using System;

[RequireComponent(typeof(NavMeshAgent),typeof(NavMeshObstacle))]
public class EntityEnermyBase : EntityBase {
    public enum_EnermyAnim E_AnimatorIndex= enum_EnermyAnim.Invalid;
    EnermyAIControllerBase m_AI;
    EnermyAnimator m_Animator;
    [Range(0,3)]
    public float F_AttackRotateParam=1f;
    [Range(0, 100)]
    public int I_AttackPreAimPercentage = 50;
    public bool B_AttackMove = true;
    bool OnCheckTarget(EntityBase target) => target.B_IsPlayer!=B_IsPlayer && !target.m_HealthManager.b_IsDead;
    public override void Init(SEntity entityInfo)
    {
        Init(entityInfo, false);
        tf_Model.transform.rotation = Quaternion.Euler(-15, 0, 0);
        Transform tf_Barrel = transform.FindInAllChild("Barrel");
        EnermyWeaponBase weapon=null;
        int weaponIndex = GameExpression.GetEnermyWeaponIndex( entityInfo.m_Index, 0);
        SFXBase weaponInfo = ObjectManager.EnermyDamageSourceInfo<SFXBase>(weaponIndex);
        SFXProjectile projectile = weaponInfo as SFXProjectile;
        if (projectile)
        {
            switch (projectile.E_ProjectileType)
            {
                default: Debug.LogError("Invalid Barrage Type:" + projectile.E_ProjectileType); break;
                case  enum_EnermyWeaponProjectile.Single: weapon = new BarrageRange(projectile,this, tf_Barrel, GetDamageBuffInfo); break;
                case enum_EnermyWeaponProjectile.MultipleFan: weapon = new BarrageMultipleFan(projectile,this, tf_Barrel , GetDamageBuffInfo); break;
                case enum_EnermyWeaponProjectile.MultipleLine: weapon = new BarrageMultipleLine(projectile,this, tf_Barrel, GetDamageBuffInfo); break;
            }
        }
        SFXCast cast = weaponInfo as SFXCast;
        if (cast)
        {
            switch (cast.E_CastType)
            {
                case  enum_CastControllType.CastFromOrigin: weapon = new EnermyCaster(cast,this, tf_Barrel, GetDamageBuffInfo); break;
                case enum_CastControllType.CastControlledForward: weapon = new EnermyCasterControlled(cast,this, tf_Barrel, GetDamageBuffInfo); break;
                case enum_CastControllType.CastAtTarget: weapon = new EnermyCasterTarget(cast,this, tf_Barrel, GetDamageBuffInfo); break;
            }
        }
        SFXBuffApply buffApply = weaponInfo as SFXBuffApply;
        if (buffApply)
            weapon = new BuffApply(buffApply,this,tf_Barrel,GetDamageBuffInfo);

        m_AI = new EnermyAIControllerBase(this,weapon, entityInfo, OnAttackAnim, OnCheckTarget);
        if (E_AnimatorIndex == enum_EnermyAnim.Invalid)
            Debug.LogError("Please Set Prefab AnimIndex!");
    }
    public override void OnSpawn(int id)
    {
        base.OnSpawn(id);
        m_Animator = new EnermyAnimator(tf_Model.GetComponent<Animator>(), E_AnimatorIndex, OnAnimKeyEvent);
    }
    public override void OnActivate()
    {
        base.OnActivate();
        m_AI.OnActivate();
    }
    protected override void Update()
    {
        base.Update();
        m_Animator.SetRun(0, m_AI.B_AgentEnabled ? 1 : 0);
    }
    protected override void OnDead()
    {
        m_AI.Deactivate();
        m_Animator.OnDead();
        base.OnDead();
    }
    protected override void OnDisable()
    {
        if (m_AI != null)
            m_AI.Deactivate();
        base.OnDisable();
    }
    void OnAttackAnim(EntityBase target,bool startAttack)
    {
        m_Animator.OnAttack(startAttack);
    }
    protected void OnAnimKeyEvent(EnermyAnimator.enum_AnimEvent animEvent)
    {
        switch (animEvent)
        {
            case EnermyAnimator.enum_AnimEvent.Fire:
                m_AI.OnAttackTriggerd();
                break;
        }
    }


    public class EnermyAnimator : AnimatorClippingTime
    {
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
        static readonly int HS_B_Attack = Animator.StringToHash("b_attack");
        Action<enum_AnimEvent> OnAnimEvent;
        public EnermyAnimator(Animator _animator, enum_EnermyAnim _animIndex,Action<enum_AnimEvent> _OnAnimEvent) : base(_animator)
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
        public void OnAttack(bool attack)
        {
            if(attack)
            m_Animator.SetTrigger(HS_T_Attack);
            m_Animator.SetBool(HS_B_Attack,attack);
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
        protected Action<EntityBase, bool> OnAttackAnim;
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

        public EnermyAIControllerBase(EntityEnermyBase _entityControlling, EnermyWeaponBase _weapon, SEntity _entityInfo, Action<EntityBase, bool> _onAttack, Func<EntityBase, bool> _onCheck)
        {
            m_EntityControlling = _entityControlling;
            m_EntityInfo = _entityInfo;
            m_Weapon = _weapon;
            m_Obstacle = m_EntityControlling.GetComponent<NavMeshObstacle>();
            m_Agent = m_EntityControlling.GetComponent<NavMeshAgent>();
            m_Agent.speed = _entityInfo.m_moveSpeed;
            OnAttackAnim = _onAttack;
            OnCheckTarget = _onCheck;
            B_AgentEnabled = false;
        }

        public void OnActivate()
        {
            B_AgentEnabled = true;
            b_attacking = false;

            this.StartSingleCoroutine(0, AICalculationTick());
            this.StartSingleCoroutine(1, TargetTrackTick());
        }

        public void Deactivate()
        {
            B_AgentEnabled = false;
            m_Weapon.OnPlayAnim(false);
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
        bool b_targetVisible;
        int i_targetUnvisibleCount;
        float f_targetAngle = 180;
        bool b_targetHideBehindWall => i_targetUnvisibleCount == 40;
        bool b_targetAvailable => m_Target != null && !m_Target.m_HealthManager.b_IsDead;

        IEnumerator AICalculationTick()
        {
            for (; ; )
            {
                f_aiSimulatedTime += GameConst.F_EnermyAICheckTime;
                CheckTarget();
                CheckTargetParams();
                CheckBattle();
                CheckMovement();
                yield return new WaitForSeconds(GameConst.F_EnermyAICheckTime);
            }
        }
        void CheckTarget()
        {
            if (!b_targetAvailable)
            {
                EntityBase target = GameManager.Instance.GetEntity(m_EntityControlling.I_EntityID,!m_Weapon.B_TargetAlly, null);
                if (target)
                    m_Target = target;
            }
        }

        void CheckTargetParams()
        {
            if (!b_targetAvailable)
                return;

            b_targetVisible = CheckTargetVisible();
            if (!b_targetVisible)
                i_targetUnvisibleCount = i_targetUnvisibleCount + 1 > 40 ? 40 : i_targetUnvisibleCount + 1;
            else
                i_targetUnvisibleCount = i_targetUnvisibleCount - 1 <= 0 ? 0 : i_targetUnvisibleCount - 1;
            f_targetDistance = TCommon.GetXZDistance(targetHeadTransform.position, headTransform.position);
            b_targetOutChaseRange = f_targetDistance > m_EntityInfo.m_AIChaseRange;
            b_targetOutAttackRange = f_targetDistance > m_EntityInfo.m_AIAttackRange;
            b_MoveTowardsTarget = b_targetHideBehindWall || b_targetOutChaseRange;
            b_CanAttackTarget = !b_targetOutAttackRange && (!m_EntityInfo.m_BattleCheckObsatacle || b_targetVisible) && Mathf.Abs(f_targetAngle) < 15 && !Physics.SphereCast(new Ray(headTransform.position, headTransform.forward), 1f, 2, GameLayer.Physics.I_Static);
            b_AgentReachDestination = m_Agent.destination == Vector3.zero || TCommon.GetXZDistance(headTransform.position, m_Agent.destination) < 1f;
        }
        #region Attack
        int i_playCount;
        bool b_preAim;
        bool b_attacking;
        void CheckBattle()
        {
            if (!b_targetAvailable||f_aiSimulatedTime < f_battleStatusCheck)
                return;
            if (b_CanAttackTarget)
                f_battleStatusCheck = f_aiSimulatedTime + OnStartAttacking();
        }
        float OnStartAttacking()
        {
            b_preAim = TCommon.RandomPercentage() >= m_EntityControlling.I_AttackPreAimPercentage;
            i_playCount = m_EntityInfo.m_ProjectileCount.RandomRangeInt();
            i_playCount = i_playCount <= 0 ? 1 : i_playCount;       //Make Sure Play Once At Least
            b_attacking = true;
            m_Weapon.OnPlayAnim(b_attacking);
            this.StartSingleCoroutine(2, TIEnumerators.TickCount(()=> {
                OnAttackAnim(m_Target, b_attacking);
            }, i_playCount, m_EntityInfo.m_Firerate,
            () => {
                b_attacking = false;
                m_Weapon.OnPlayAnim(b_attacking);
                OnAttackAnim(m_Target, b_attacking);
            })
            );
            return m_EntityInfo.m_Firerate * i_playCount + m_EntityInfo.m_BarrageDuration.RandomRangeFloat(); ;
        }
        public void OnAttackTriggerd()
        {
            m_Weapon.Play(b_preAim,m_Target);
        }
        #endregion
        #region Movement

        IEnumerator TargetTrackTick()
        {
            for (; ; )
            {
                if (b_targetAvailable)
                {
                    v3_TargetDirection = TCommon.GetXZLookDirection(headTransform.position, targetHeadTransform.position);
                    f_targetAngle = TCommon.GetAngle(v3_TargetDirection, transform.forward, Vector3.up);
                    m_Agent.updateRotation = b_targetOutAttackRange;
                    if (!m_Agent.updateRotation)
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(v3_TargetDirection, Vector3.up), Time.deltaTime * 5 * (b_attacking ? m_EntityControlling.F_AttackRotateParam : 1));
                }
                yield return null;
            }
        }


        void CheckMovement()
        {
            if ((b_attacking && !m_EntityControlling.B_AttackMove))
            {
                B_AgentEnabled = false;
                return;
            }

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
            return EnviormentManager.RandomNavMeshPosition(m_EntityControlling.transform.position+TCommon.RandomSpherePosition(10f,0,10f), 50);
        }

        Vector3 GetSamplePosition()
        {
            Vector3 m_SamplePosition= m_EntityControlling.transform.position+ (b_MoveTowardsTarget? v3_TargetDirection : -v3_TargetDirection).normalized*5;
            return EnviormentManager.RandomNavMeshPosition(m_SamplePosition+TCommon.RandomSpherePosition(5f,0,5f), 100);
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
        #endregion
    }
    #endregion
    #region EnermyWeapon
    class EnermyWeaponBase 
    {
        public virtual bool B_TargetAlly=>false;
        protected EntityEnermyBase m_EntityControlling;
        protected Transform attacherTransform => m_EntityControlling.tf_Head;
        protected Transform transformBarrel;
        protected Func<DamageBuffInfo> GetBuffInfo;
        protected SEntity m_Info
        {
            get
            {
                if (m_EntityControlling == null)
                    Debug.LogError("Null Entity Controlling?");
                return m_EntityControlling.m_EntityInfo;
            }
        }
        public EnermyWeaponBase(EntityEnermyBase _controller,Transform _transform,Func<DamageBuffInfo> _GetBuffInfo )
        {
            m_EntityControlling = _controller;
            transformBarrel = _transform;
            if (_transform == null)
                Debug.LogError("Null Weapon Barrel Found!");
            GetBuffInfo = _GetBuffInfo;
        }
        public virtual void Play(bool preAim,EntityBase _target)
        {
        }
        public virtual void OnPlayAnim(bool play)
        {

        }
    }
    class EnermyCaster : EnermyWeaponBase
    {
        protected int i_castIndex;
        public EnermyCaster(SFXCast _castInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(_controller, _transform,_GetBuffInfo)
        {
            i_castIndex = _castInfo.I_SFXIndex;
        }
        public override void Play(bool preAim,EntityBase _target)
        {
            ObjectManager.SpawnDamageSource<SFXCast>(i_castIndex, attacherTransform.position, attacherTransform.forward).Play(m_EntityControlling.I_EntityID,GetBuffInfo());
        }
    }
    class EnermyCasterControlled : EnermyCaster
    {
        SFXCast m_Cast;
        public EnermyCasterControlled(SFXCast _castInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(_castInfo, _controller, _transform, _GetBuffInfo)
        {
        }
        public override void Play(bool preAim, EntityBase _target)
        {
            if (m_Cast)
                m_Cast.PlayControlled(m_EntityControlling.I_EntityID, transformBarrel,attacherTransform,false);

            m_Cast = ObjectManager.SpawnDamageSource<SFXCast>(i_castIndex, transformBarrel.position, transformBarrel.forward);
            m_Cast.PlayControlled(m_EntityControlling.I_EntityID, transformBarrel, attacherTransform, true);
        }

        public override void OnPlayAnim(bool play)
        {
            if ( m_Cast)
                m_Cast.PlayControlled(m_EntityControlling.I_EntityID, transformBarrel, attacherTransform, play);
        }
    }
    class EnermyCasterTarget : EnermyCaster
    {
        public EnermyCasterTarget(SFXCast _castInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(_castInfo, _controller, _transform,_GetBuffInfo)
        {
        }
        public override void Play(bool preAim, EntityBase _target)
        {
             ObjectManager.SpawnDamageSource<SFXCast>(i_castIndex, EnviormentManager.RandomNavMeshPosition(_target.transform.position,m_Info.m_HorizontalSpread), _target.transform.up).Play(m_EntityControlling.I_EntityID,GetBuffInfo());
        }

    }
    class BarrageRange : EnermyWeaponBase
    {
        protected float f_projectileSpeed;
        protected int i_projectileIndex, i_muzzleIndex;

        public BarrageRange(SFXProjectile projectileInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(_controller, _transform,_GetBuffInfo)
        {
            i_projectileIndex = projectileInfo.I_SFXIndex;
            i_muzzleIndex = projectileInfo.I_MuzzleIndex;
            f_projectileSpeed = projectileInfo.F_Speed;
        }

        public override void Play(bool preAim, EntityBase _target)
        {
            Vector3 horizontalOffsetDirection = GameExpression.V3_RangeSpreadDirection(GetHorizontalDirection(preAim,_target), m_Info.m_HorizontalSpread, Vector3.zero, transformBarrel.right);
            FireBullet(transformBarrel.position, horizontalOffsetDirection,_target.tf_Head.position);
        }

        protected Vector3 GetHorizontalDirection(bool preAim,EntityBase _target)
        {
            Vector3 targetDirection= Vector3.Normalize((preAim ? (_target.m_PrecalculatedTargetPos(Vector3.Distance(_target.tf_Head.position, attacherTransform.position) /f_projectileSpeed)) : _target.tf_Head.position) - attacherTransform.position);
            targetDirection.y = 0;
            return targetDirection.normalized;
        }

        protected void FireBullet(Vector3 startPosition,Vector3 direction,Vector3 targetPosition)
        {
            if (i_muzzleIndex > 0)
                ObjectManager.SpawnCommonParticles<SFXParticles>(i_muzzleIndex, startPosition, direction).Play(m_EntityControlling.I_EntityID);
            ObjectManager.SpawnDamageSource<SFXProjectile>(i_projectileIndex, startPosition, direction).Play(m_EntityControlling.I_EntityID, direction, targetPosition+TCommon.RandomSpherePosition( m_Info.m_HorizontalSpread,0, m_Info.m_HorizontalSpread),GetBuffInfo());
        }
    }
    class BarrageMultipleLine : BarrageRange
    {
        public BarrageMultipleLine(SFXProjectile projectileInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(projectileInfo,_controller, _transform,_GetBuffInfo)
        {
        }
        public override void Play(bool preAim,EntityBase _target)
        {
            int waveCount = m_Info.m_RangeExtension.RandomRangeInt();
            Vector3 startDirection = GetHorizontalDirection(preAim, _target);
            Vector3 startPosition = transformBarrel.position - attacherTransform.right*m_Info.m_OffsetExtension*((waveCount-1)/2f);
            float distance = TCommon.GetXZDistance(transformBarrel.position, _target.transform.position);
            for (int i = 0; i < waveCount; i++)
                FireBullet(startPosition+ attacherTransform.right*m_Info.m_OffsetExtension*i, startDirection, transformBarrel.position + startDirection * distance);
        }
    }
    class BarrageMultipleFan : BarrageRange
    {
        public BarrageMultipleFan(SFXProjectile projectileInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(projectileInfo, _controller, _transform,_GetBuffInfo)
        {
        }
        public override void Play(bool preAim,EntityBase _target)
        {
            int waveCount = m_Info.m_RangeExtension.RandomRangeInt();
            Vector3 startDirection = GetHorizontalDirection(preAim, _target);
            float startFanAngle= -m_Info.m_OffsetExtension*(waveCount-1)/2f;
            float distance = TCommon.GetXZDistance(transformBarrel.position, _target.transform.position);
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 fanDirection = startDirection.RotateDirection(Vector3.up, startFanAngle + i * m_Info.m_OffsetExtension);
                FireBullet(transformBarrel.position, fanDirection,transformBarrel.position+fanDirection* distance);
            }
        } 
    }
    class BuffApply : EnermyWeaponBase
    {
        public override bool B_TargetAlly => true;
        SBuff m_buffInfo;
        int i_buffApplyIndex;
        public BuffApply(SFXBuffApply buffApplyinfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(_controller, _transform, _GetBuffInfo)
        {
            i_buffApplyIndex = buffApplyinfo.I_SFXIndex;
            m_buffInfo = DataManager.GetEntityBuffProperties(buffApplyinfo.I_BuffIndex);
        }
        public override void Play(bool preAim, EntityBase _target)
        {
            ObjectManager.SpawnDamageSource<SFXBuffApply>(i_buffApplyIndex,transformBarrel.position,Vector3.up).Play(m_EntityControlling.I_EntityID,m_buffInfo,transformBarrel,_target);
        }
    }
    #endregion

#if UNITY_EDITOR
    CapsuleCollider hitbox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_GameDebugGizmos)
            return;

        if(!hitbox)
        hitbox = GetComponent<CapsuleCollider>();
        Gizmos.color = Color.red;
        Gizmos_Extend.DrawWireCapsule(transform.position+Vector3.up*hitbox.height/2,Quaternion.LookRotation(transform.forward,transform.up),hitbox.transform.localScale,hitbox.radius,hitbox.height);
    }
#endif
}
