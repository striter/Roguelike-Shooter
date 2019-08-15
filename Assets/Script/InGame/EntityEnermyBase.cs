using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;
using System;

[RequireComponent(typeof(NavMeshAgent),typeof(NavMeshObstacle))]
public class EntityEnermyBase : EntityBase {
    public enum_EntityType E_EnermyType= enum_EntityType.Invalid;
    public enum_EnermyAnim E_AnimatorIndex= enum_EnermyAnim.Invalid;
    public float F_AIChaseRange;
    public float F_AIAttackRange;
    public RangeFloat F_AttackDuration;
    public float F_AttackSpread;
    public RangeInt F_AttackTimes;
    public float F_AttackRate;
    EnermyAIControllerBase m_AI;
    EnermyAnimator m_Animator;
    [Range(0,3)]
    public float F_AttackRotateParam=1f;
    [Range(0, 100)]
    public int I_AttackPreAimPercentage = 50;
    public bool B_AttackMove = true;
    public bool B_SwitchTarget = false;
    bool OnCheckTarget(EntityBase target) => target.m_Flag!=m_Flag && !target.m_HealthManager.b_IsDead;
    public override void Init(int entityPresetIndex)
    {
        base.Init(entityPresetIndex, enum_EntityFlag.Enermy);
        tf_Model.transform.rotation = Quaternion.Euler(-15, 0, 0);
        m_AI = new EnermyAIControllerBase(this, InitWeapon(GameExpression.GetEnermyWeaponIndex(entityPresetIndex, 0)), OnAttackAnim, OnCheckTarget);
    }

    protected virtual EnermyWeaponBase InitWeapon(int weaponIndex)
    {
        Transform tf_Barrel = transform.FindInAllChild("Barrel");
        EnermyWeaponBase weapon = null;
        SFXBase weaponInfo = ObjectManager.GetDamageSource<SFXBase>(weaponIndex);

        SFXProjectile projectile = weaponInfo as SFXProjectile;
        if (projectile)
        {
            switch (projectile.E_ProjectileType)
            {
                default: Debug.LogError("Invalid Barrage Type:" + projectile.E_ProjectileType); break;
                case enum_ProjectileFireType.Single: weapon = new BarrageRange(projectile, this, tf_Barrel, GetDamageBuffInfo); break;
                case enum_ProjectileFireType.MultipleFan: weapon = new BarrageMultipleFan(projectile, this, tf_Barrel, GetDamageBuffInfo); break;
                case enum_ProjectileFireType.MultipleLine: weapon = new BarrageMultipleLine(projectile, this, tf_Barrel, GetDamageBuffInfo); break;
            }
        }

        SFXCast cast = weaponInfo as SFXCast;
        if (cast)
        {
            switch (cast.E_CastType)
            {
                case enum_CastControllType.CastFromOrigin: weapon = new EnermyCaster(cast, this, tf_Barrel, GetDamageBuffInfo); break;
                case enum_CastControllType.CastSelfDetonate: weapon = new EnermyCasterSelfDetonateAnimLess(cast,this,tf_Barrel,GetDamageBuffInfo,()=> { OnAnimKeyEvent( TAnimatorEvent.enum_AnimEvent.Fire); },OnDead,tf_Model.Find("BlinkModel"));break;
                case enum_CastControllType.CastControlledForward: weapon = new EnermyCasterControlled(cast, this, tf_Barrel, GetDamageBuffInfo); break;
                case enum_CastControllType.CastAtTarget: weapon = new EnermyCasterTarget(cast, this, tf_Barrel, GetDamageBuffInfo); break;
            }
        }

        SFXBuffApply buffApply = weaponInfo as SFXBuffApply;
        if (buffApply)
            weapon = new BuffApply(buffApply, this, tf_Barrel, GetDamageBuffInfo);

        SFXEntitySpawner entitySpawner = weaponInfo as SFXEntitySpawner;
        if (entitySpawner)
            weapon = new WeaponEntitySpawner(entitySpawner,this,tf_Barrel,GetDamageBuffInfo);

        return weapon;
    }

    public override void OnSpawn(int id)
    {
        base.OnSpawn(id);
        Animator animator = tf_Model.GetComponent<Animator>();
        if (animator)
            m_Animator = new EnermyAnimator(animator, E_AnimatorIndex, OnAnimKeyEvent);
        m_AI.OnActivate();
    }
    protected override void OnDead()
    {
        m_AI.Deactivate();
        if (m_Animator != null)
            m_Animator.OnDead();
        base.OnDead();
    }

    protected override void OnInfoChange()
    {
        base.OnInfoChange();
        m_AI.OnInfoChange();
    }
    protected override void Update()
    {
        base.Update();
        if (m_Animator != null)
            m_Animator.SetRun( m_AI.B_AgentEnabled ? 1 : 0);
    }
    void OnAttackAnim(EntityBase target,bool startAttack)
    {
        if(m_Animator!=null)
             m_Animator.OnAttack(startAttack);
    }
    protected void OnAnimKeyEvent(TAnimatorEvent.enum_AnimEvent animEvent)
    {
        switch (animEvent)
        {
            case TAnimatorEvent.enum_AnimEvent.Fire:
                m_AI.OnAttackTriggerd();
                break;
        }
    }


    public class EnermyAnimator : AnimatorClippingTime
    {
        static readonly int HS_T_Dead = Animator.StringToHash("t_dead");
        static readonly int HS_I_AnimIndex = Animator.StringToHash("i_weaponType");
        static readonly int HS_T_Activate = Animator.StringToHash("t_activate");
        static readonly int HS_T_Attack = Animator.StringToHash("t_attack");
        static readonly int HS_F_Forward = Animator.StringToHash("f_forward");
        static readonly int HS_B_Attack = Animator.StringToHash("b_attack");
        Action<TAnimatorEvent.enum_AnimEvent> OnAnimEvent;
        public EnermyAnimator(Animator _animator, enum_EnermyAnim _animIndex,Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator)
        {
            m_Animator.fireEvents = true;
            m_Animator.SetInteger(HS_I_AnimIndex,(int)_animIndex);
            m_Animator.SetTrigger(HS_T_Activate);
            OnAnimEvent = _OnAnimEvent;
            m_Animator.GetComponent<TAnimatorEvent>().Attach(OnAnimEvent);
        }
        public void SetRun(float forward)
        {
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
        protected EntityEnermyBase m_Entity;
        protected EntityBase m_Target;
        protected Transform transform => m_Agent.transform;
        protected Transform headTransform => m_Entity.tf_Head;
        protected Transform targetHeadTransform => m_Target.tf_Head;
        protected NavMeshAgent m_Agent;
        protected NavMeshObstacle m_Obstacle;
        protected Action<EntityBase, bool> OnAttackAnim;
        protected Func<EntityBase, bool> OnCheckTarget;
        protected EnermyWeaponBase m_Weapon;
        protected EntityInfoManager m_Info => m_Entity.m_EntityInfo;
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

        public EnermyAIControllerBase(EntityEnermyBase _entityControlling, EnermyWeaponBase _weapon, Action<EntityBase, bool> _onAttack, Func<EntityBase, bool> _onCheck)
        {
            m_Entity = _entityControlling;
            m_Weapon = _weapon;
            m_Obstacle = m_Entity.GetComponent<NavMeshObstacle>();
            m_Agent = m_Entity.GetComponent<NavMeshAgent>();
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
        public void OnInfoChange()
        {
            m_Agent.speed = m_Info.F_MovementSpeed;
        }
        public void Deactivate()
        {
            B_AgentEnabled = false;
            m_Weapon.OnDeactivate();
            this.StopAllSingleCoroutines();
        }
        RaycastHit[] m_Raycasts;
        float f_movementSimulate,f_battleSimulate;
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
                if(f_movementSimulate>0)
                f_movementSimulate -= GameConst.F_EnermyAICheckTime*m_Info.F_MovementSpeedMultiply;
                if(f_battleSimulate>0)
                f_battleSimulate -= m_Info.F_ReloadRateTick( GameConst.F_EnermyAICheckTime);

                if (!b_targetAvailable)
                    RecheckTarget();

                CheckTargetParams();
                CheckBattle();
                CheckMovement();
                yield return new WaitForSeconds(GameConst.F_EnermyAICheckTime);
            }
        }
        void RecheckTarget()
        {
            m_Target = GameManager.Instance.GetRandomEntity(m_Entity.I_EntityID, m_Weapon.B_TargetAlly ?  enum_EntityFlag.Enermy: enum_EntityFlag.Player, null);
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
            b_targetOutChaseRange = f_targetDistance > m_Entity.F_AIChaseRange;
            b_targetOutAttackRange = f_targetDistance > m_Entity.F_AIAttackRange;
            b_MoveTowardsTarget = b_targetHideBehindWall || b_targetOutChaseRange;
            b_CanAttackTarget = !b_targetOutAttackRange  && Mathf.Abs(f_targetAngle) < 15 && !Physics.SphereCast(new Ray(headTransform.position, headTransform.forward), 1f, 2, GameLayer.Mask.I_Static);
            b_AgentReachDestination = m_Agent.destination == Vector3.zero || TCommon.GetXZDistance(headTransform.position, m_Agent.destination) < 1f;
        }
        #region Attack
        int i_playCount;
        bool b_preAim;
        bool b_attacking;
        void CheckBattle()
        {
            if (!b_targetAvailable|| f_battleSimulate >0)
                return;
            if (b_CanAttackTarget)
                f_battleSimulate =  OnStartAttacking();
        }
        float OnStartAttacking()
        {
            b_preAim = TCommon.RandomPercentage() >= m_Entity.I_AttackPreAimPercentage;
            i_playCount = m_Entity.F_AttackTimes.RandomRangeInt();
            i_playCount = i_playCount <= 0 ? 1 : i_playCount;       //Make Sure Play Once At Least
            this.StartSingleCoroutine(2, Attack(i_playCount, m_Entity.F_AttackRate));
            return m_Entity.F_AttackRate * i_playCount + m_Entity.F_AttackDuration.RandomRangeFloat();
        }
        IEnumerator Attack(int count,float fireRate)
        {
            float m_attackSimulate= 0;
            b_attacking = true;
            m_Weapon.OnPlayAnim(true);
            OnAttackAnim(m_Target, true);
            count--;
            for (; ; )
            {
                m_attackSimulate +=m_Entity.m_EntityInfo.F_FireRateTick(Time.deltaTime);
                if (m_attackSimulate >= fireRate)
                {
                    m_attackSimulate -= fireRate;

                    if (count <= 0)
                    {
                        OnAttackFinished();
                        yield break;
                    }
                    OnAttackAnim(m_Target, true);
                    count--;
                }
                yield return null;
            }
        }
        void OnAttackFinished()
        {
            b_attacking = false;
            m_Weapon.OnPlayAnim(false);
            OnAttackAnim(m_Target, false);

            if (m_Entity.B_SwitchTarget)
                RecheckTarget();
        }

        public void OnAttackTriggerd()
        {
            m_Weapon.Play(b_preAim, m_Target);
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
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(v3_TargetDirection, Vector3.up), Time.deltaTime * 5 * (b_attacking ? m_Entity.F_AttackRotateParam : 1));
                }
                yield return null;
            }
        }


        void CheckMovement()
        {
            if ((b_attacking && !m_Entity.B_AttackMove))
            {
                B_AgentEnabled = false;
                return;
            }

            if (f_movementSimulate >0)
                return;

            if (!b_idled && b_AgentReachDestination && !b_targetOutChaseRange && UnityEngine.Random.Range(0, 2) > 0)
            {
                b_idled = true;
                B_AgentEnabled = false;
                f_movementSimulate =  UnityEngine.Random.Range(2f, 3f);
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
            stuckCount = (previousPos != Vector3.zero && Vector3.Distance(previousPos, m_Entity.transform.position) < .5f) ? stuckCount + 1 : 0;
            previousPos = m_Entity.transform.position;
            return stuckCount>3;
        }

        Vector3 GetUnstuckPosition()
        {
            return EnviormentManager.NavMeshPosition(m_Entity.transform.position+TCommon.RandomXZSphere(10f));
        }

        Vector3 GetSamplePosition()
        {
            Vector3 m_SamplePosition= m_Entity.transform.position+ (b_MoveTowardsTarget? v3_TargetDirection : -v3_TargetDirection).normalized*5;
            return EnviormentManager.NavMeshPosition(m_SamplePosition+TCommon.RandomXZSphere(5f));
        }

        bool CheckTargetVisible()
        {
            m_Raycasts = Physics.RaycastAll(m_Entity.tf_Head.position, v3_TargetDirection, Vector3.Distance(m_Entity.tf_Head.position, m_Target.tf_Head.position), GameLayer.Mask.I_StaticEntity);
            for (int i = 0; i < m_Raycasts.Length; i++)
            {
                if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Static)
                    return false;
                else if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Entity)
                {
                    HitCheckEntity entity = m_Raycasts[i].collider.GetComponent<HitCheckEntity>();
                    if (entity.m_Attacher.I_EntityID != m_Target.I_EntityID && entity.m_Attacher.I_EntityID != m_Entity.I_EntityID)
                        return false;
                }
            }
            return true;
        }
        #endregion
    }
    #endregion
    #region EnermyWeapon
     protected class EnermyWeaponBase 
    {
        public virtual bool B_TargetAlly=>false;
        protected int i_weaponIndex;
        protected EntityEnermyBase m_Entity;
        protected Transform attacherTransform => m_Entity.tf_Head;
        protected Transform transformBarrel;
        protected Func<DamageBuffInfo> GetBuffInfo;
        protected EntityInfoManager m_Info
        {
            get
            {
                if (m_Entity == null)
                    Debug.LogError("Null Entity Controlling?");
                return m_Entity.m_EntityInfo;
            }
        }
        public EnermyWeaponBase(SFXBase weaponBase,EntityEnermyBase _controller,Transform _transform,Func<DamageBuffInfo> _GetBuffInfo )
        {
            i_weaponIndex = weaponBase.I_SFXIndex;
            m_Entity = _controller;
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
        public virtual void OnDeactivate()
        {

        }
    }
    class EnermyCaster : EnermyWeaponBase
    {
        public EnermyCaster(SFXCast _castInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(_castInfo, _controller, _transform,_GetBuffInfo)
        {
        }
        public override void Play(bool preAim,EntityBase _target)
        {
            ObjectManager.SpawnDamageSource<SFXCast>(i_weaponIndex, attacherTransform.position, attacherTransform.forward).Play(m_Entity.I_EntityID,GetBuffInfo());
        }
    }
    class EnermyCasterSelfDetonateAnimLess : EnermyCaster,ISingleCoroutine
    {
        ModelBlink m_Blink;
        Action OnSelfDetonate,OnDead;
        float timeElapsed;
        public EnermyCasterSelfDetonateAnimLess(SFXCast _castInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo, Action _OnSelfDetonate,Action _OnDead,Transform _blinkModels) : base(_castInfo, _controller, _transform, _GetBuffInfo)
        {
            OnSelfDetonate = _OnSelfDetonate;
            OnDead = _OnDead;
            m_Blink = new ModelBlink(_blinkModels,.25f,.25f);
            timeElapsed = 0;
        }
        public override void OnPlayAnim(bool play)
        {
            base.OnPlayAnim(play);
            if (play)
            {
                timeElapsed = 0;
                this.StartSingleCoroutine(0, TIEnumerators.Tick(Tick));
            }
        }
        void Tick()
        {
            timeElapsed += Time.deltaTime;
            float timeMultiply = 2f * (timeElapsed / 2f);
            m_Blink.Tick(Time.deltaTime*timeMultiply);
            if (timeElapsed > 2f)
            {
                OnSelfDetonate();
                this.StopSingleCoroutine(0);
            }
        }
        public override void Play(bool preAim, EntityBase _target)
        {
            base.Play(preAim, _target);
            OnDead();
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            this.StopSingleCoroutine(0);
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
            OnPlayAnim(false);
            m_Cast = ObjectManager.SpawnDamageSource<SFXCast>(i_weaponIndex, transformBarrel.position, transformBarrel.forward);
            m_Cast.PlayControlled(m_Entity.I_EntityID, transformBarrel, attacherTransform, true);
        }

        public override void OnPlayAnim(bool play)
        {
            if ( m_Cast)
                m_Cast.PlayControlled(m_Entity.I_EntityID, transformBarrel, attacherTransform, play);
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            OnPlayAnim(false);
        }
    }

    class EnermyCasterTarget : EnermyCaster
    {
        public EnermyCasterTarget(SFXCast _castInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(_castInfo, _controller, _transform,_GetBuffInfo)
        {
        }
        public override void Play(bool preAim, EntityBase _target)
        {
             ObjectManager.SpawnDamageSource<SFXCast>(i_weaponIndex, EnviormentManager.NavMeshPosition(_target.transform.position+TCommon.RandomXZSphere(m_Entity.F_AttackSpread)), _target.transform.up).Play(m_Entity.I_EntityID,GetBuffInfo());
        }

    }
    class BarrageRange : EnermyWeaponBase
    {
        protected float f_projectileSpeed { get; private set; }
        protected int i_muzzleIndex { get; private set; }
        protected RangeInt m_CountExtension { get; private set; }
        protected float m_OffsetExtension { get; private set; }

        public BarrageRange(SFXProjectile projectileInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(projectileInfo,_controller, _transform,_GetBuffInfo)
        {
            i_muzzleIndex = projectileInfo.I_MuzzleIndex;
            f_projectileSpeed = projectileInfo.F_Speed;
            m_CountExtension = projectileInfo.RI_CountExtension;
            m_OffsetExtension = projectileInfo.F_OffsetExtension;
        }

        public override void Play(bool preAim, EntityBase _target)
        {
            Vector3 targetPosition = GetSpreadPosition(preAim,_target);
            Vector3 startPosition = transformBarrel.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, targetPosition);
            FireBullet(startPosition, direction,targetPosition);
        }

        protected Vector3 GetSpreadPosition(bool preAim, EntityBase _target) {
            float startDistance = TCommon.GetXZDistance(transformBarrel.position, _target.tf_Head.position);
            Vector3 targetPosition= preAim  ? _target.m_PrecalculatedTargetPos(startDistance / f_projectileSpeed) : _target.tf_Head.position;

            if(preAim&&Mathf.Abs(TCommon.GetAngle(transformBarrel.forward,TCommon.GetXZLookDirection(transformBarrel.position,targetPosition) ,Vector3.up))>90)    //Target Positioned Back, Return Target
                targetPosition=_target.tf_Head.position;

            if (TCommon.GetXZDistance(transformBarrel.position, targetPosition) > m_Entity.F_AttackSpread)      //Target Outside Spread Sphere,Add Spread
                targetPosition+= TCommon.RandomXZSphere(m_Entity.F_AttackSpread);
            return targetPosition;
        } 

        protected void FireBullet(Vector3 startPosition,Vector3 direction,Vector3 targetPosition)
        {
            if (i_muzzleIndex > 0)
                ObjectManager.SpawnParticles<SFXMuzzle>(i_muzzleIndex, startPosition, direction).Play(m_Entity.I_EntityID);
            ObjectManager.SpawnDamageSource<SFXProjectile>(i_weaponIndex, startPosition, direction).Play(m_Entity.I_EntityID, direction, targetPosition,GetBuffInfo());
        }
    }
    class BarrageMultipleLine : BarrageRange
    {
        public BarrageMultipleLine(SFXProjectile projectileInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(projectileInfo,_controller, _transform,_GetBuffInfo)
        {
        }
        public override void Play(bool preAim,EntityBase _target)
        {
            Vector3 startPosition = transformBarrel.position;
            Vector3 targetPosition = GetSpreadPosition(preAim, _target);
            Vector3 direction = TCommon.GetXZLookDirection(startPosition,targetPosition);
            int waveCount = m_CountExtension.RandomRangeInt();
            float distance = TCommon.GetXZDistance(startPosition, targetPosition);
            Vector3 lineBeginPosition = startPosition - attacherTransform.right * m_OffsetExtension * ((waveCount - 1) / 2f);
            for (int i = 0; i < waveCount; i++)
                FireBullet(lineBeginPosition+ attacherTransform.right*m_OffsetExtension*i, direction, transformBarrel.position + direction * distance);
        }
    }
    class BarrageMultipleFan : BarrageRange
    {
        public BarrageMultipleFan(SFXProjectile projectileInfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(projectileInfo, _controller, _transform,_GetBuffInfo)
        {
        }
        public override void Play(bool preAim,EntityBase _target)
        {
            Vector3 targetPosition = GetSpreadPosition(preAim, _target);
            Vector3 startPosition = transformBarrel.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, targetPosition);
            int waveCount = m_CountExtension.RandomRangeInt();
            float beginAnle= -m_OffsetExtension*(waveCount-1)/2f;
            float distance = TCommon.GetXZDistance(transformBarrel.position, _target.transform.position);
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 fanDirection = direction.RotateDirection(Vector3.up, beginAnle + i * m_OffsetExtension);
                FireBullet(transformBarrel.position, fanDirection,transformBarrel.position+fanDirection* distance);
            }
        } 
    }
    class BuffApply : EnermyWeaponBase
    {
        public override bool B_TargetAlly => true;
        SBuff m_buffInfo;
        SFXBuffApply m_Effect;
        public BuffApply(SFXBuffApply buffApplyinfo, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(buffApplyinfo, _controller, _transform, _GetBuffInfo)
        {
            m_buffInfo = DataManager.GetEntityBuffProperties(buffApplyinfo.I_BuffIndex);
        }
        public override void Play(bool preAim, EntityBase _target)
        {
            if(!m_Effect||!m_Effect.b_Playing)
                m_Effect = ObjectManager.SpawnDamageSource<SFXBuffApply>(i_weaponIndex, transformBarrel.position, Vector3.up);
            
            m_Effect.Play(m_Entity.I_EntityID,m_buffInfo,transformBarrel,_target);
        }
    }

    class WeaponEntitySpawner : EnermyWeaponBase
    {
        public WeaponEntitySpawner(SFXEntitySpawner spawner, EntityEnermyBase _controller, Transform _transform, Func<DamageBuffInfo> _GetBuffInfo) : base(spawner,_controller, _transform, _GetBuffInfo)
        {
        }
        public override void Play(bool preAim, EntityBase _target)
        {
            ObjectManager.SpawnDamageSource<SFXEntitySpawner>(i_weaponIndex,transformBarrel.position,Vector3.up).Play(m_Entity.I_EntityID);
        }
    }
    #endregion

#if UNITY_EDITOR
    CapsuleCollider hitbox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        if(!hitbox)
        hitbox = GetComponent<CapsuleCollider>();
        Gizmos.color = Color.red;
        Gizmos_Extend.DrawWireCapsule(transform.position+Vector3.up*hitbox.height/2,Quaternion.LookRotation(transform.forward,transform.up),hitbox.transform.localScale,hitbox.radius,hitbox.height);
    }
#endif
}
