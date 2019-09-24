using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;
using System;

[RequireComponent(typeof(NavMeshAgent),typeof(NavMeshObstacle))]
public class EntityCharacterAI : EntityCharacterBase {
    public enum_CharacterType E_EnermyType= enum_CharacterType.Invalid;
    public enum_EnermyAnim E_AnimatorIndex= enum_EnermyAnim.Invalid;
    public override enum_EntityController m_Controller => enum_EntityController.AI;
    public float F_AIChaseRange;
    public float F_AIAttackRange;
    public RangeFloat F_AttackDuration;
    public RangeInt F_AttackTimes;
    public float F_AttackRate;
    EnermyAIControllerBase m_AI;
    EnermyAnimator m_Animator;
    public bool B_BattleCheckObstacle = false;
    [Range(0,3)]
    public float F_AttackRotateParam=1f;
    [Range(0, 100)]
    public int I_AttackPreAimPercentage = 50;
    public bool B_AttackMove = true;
    public bool B_AttackFrontCheck = true;
    bool OnCheckTarget(EntityCharacterBase target) => target.m_Flag!=m_Flag && !target.m_Health.b_IsDead;
    public override Vector3 m_PrecalculatedTargetPos(float time)=> tf_Head.position;
    protected override HealthBase GetHealthManager()=>new AIHealth(this,OnHealthChanged, OnDead);
    public override void Init(int entityPresetIndex)
    {
        base.Init(entityPresetIndex);
        Transform tf_Barrel = transform.FindInAllChild("Barrel");
        m_AI = new EnermyAIControllerBase(this, EquipmentBase.AcquireEquipment(GameExpression.GetAIEquipment(entityPresetIndex, 0),this,tf_Barrel,m_CharacterInfo.GetDamageBuffInfo), OnAttackAnim, OnCheckTarget);
    }

    public override void OnActivate(enum_EntityFlag _flag)
    {
        if (E_AnimatorIndex!= enum_EnermyAnim.Invalid)
            m_Animator = new EnermyAnimator(tf_Model.GetComponent<Animator>(), E_AnimatorIndex, OnAnimKeyEvent);
        m_AI.OnActivate();
        base.OnActivate(_flag);
    }
    public void SetEnermyDifficulty(enum_GameDifficulty difficulty,enum_StageLevel stageDifficulty)
    {
        m_CharacterInfo.AddBuff(-1,GameExpression.GetEnermyGameDifficultyBuffIndex(difficulty));
        (m_Health as AIHealth).SetHealth(I_MaxHealth*GameExpression.GetAIBaseHealthMultiplier(difficulty), GameExpression.GetAIMaxHealthMultiplier(stageDifficulty));
    }
    protected override void OnDead()
    {
        base.OnDead();
        m_AI.OnDeactivate();
        if (E_AnimatorIndex !=  enum_EnermyAnim.Invalid)
            m_Animator.OnDead();
    }

    protected override void OnExpireChange()
    {
        base.OnExpireChange();
        if (E_AnimatorIndex != enum_EnermyAnim.Invalid)
            m_Animator.SetMovementSpeed(m_CharacterInfo.F_MovementSpeed);
        m_AI.OnInfoChange();
    }
    protected override void Update()
    {
        base.Update();
        if (m_Health.b_IsDead)
            return;

        if (E_AnimatorIndex != enum_EnermyAnim.Invalid)
        {
            m_Animator.SetRun(m_AI.B_AgentEnabled ? 1 : 0);
            m_Animator.SetStun(m_CharacterInfo.B_Effecting( enum_CharacterEffect.Stun));
        }

        m_AI.SetPlay(!m_CharacterInfo.B_Effecting( enum_CharacterEffect.Stun));
        m_AI.OnTick(Time.deltaTime);
    }
    protected override bool OnReceiveDamage(DamageInfo damageInfo, Vector3 damageDirection)
    {
        if(damageDirection!=Vector3.zero)
            transform.Translate(damageDirection *GameConst.F_AIDamageTranslate * -damageInfo.m_AmountApply);
        return base.OnReceiveDamage(damageInfo, damageDirection);
    }
    void OnAttackAnim(EntityCharacterBase target,bool startAttack)
    {
        if (E_AnimatorIndex != enum_EnermyAnim.Invalid)
            m_Animator.OnAttack(startAttack);
        else if(startAttack)
            OnAnimKeyEvent(TAnimatorEvent.enum_AnimEvent.Fire);
    }
    protected void OnAnimKeyEvent(TAnimatorEvent.enum_AnimEvent animEvent)
    {
        switch (animEvent)
        {
            case TAnimatorEvent.enum_AnimEvent.Fire:
                m_AI.OnAttackTriggered();
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
        static readonly int HS_FM_Movement = Animator.StringToHash("fm_movement");
        Action<TAnimatorEvent.enum_AnimEvent> OnAnimEvent;
        public EnermyAnimator(Animator _animator, enum_EnermyAnim _animIndex,Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator)
        {
            m_Animator.fireEvents = true;
            m_Animator.SetInteger(HS_I_AnimIndex,(int)_animIndex);
            m_Animator.SetTrigger(HS_T_Activate);
            OnAnimEvent = _OnAnimEvent;
            m_Animator.GetComponent<TAnimatorEvent>().Attach(OnAnimEvent);
        }
        public void SetStun(bool stun)
        {
            m_Animator.speed = stun ? 0 : 1;
        }
        public void SetRun(float forward)
        {
            m_Animator.SetFloat(HS_F_Forward, forward);
        }
        public void SetMovementSpeed( float movementSpeed)
        {
            m_Animator.SetFloat(HS_FM_Movement, movementSpeed / 4f);
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
            m_Animator.fireEvents = false;
        }
    }
    #region AI
    class EnermyAIControllerBase : ISingleCoroutine
    {
        protected EntityCharacterAI m_Entity;
        protected EntityCharacterBase m_Target;
        protected Transform transform => m_Agent.transform;
        protected Transform headTransform => m_Entity.tf_Head;
        protected Transform targetHeadTransform => m_Target.tf_Head;
        protected NavMeshAgent m_Agent;
        protected NavMeshObstacle m_Obstacle;
        protected Action<EntityCharacterBase, bool> OnAttackAnim;
        protected Func<EntityCharacterBase, bool> OnCheckTarget;
        protected EquipmentBase m_Weapon;
        protected CharacterInfoManager m_Info => m_Entity.m_CharacterInfo;
        RaycastHit[] m_Raycasts;
        float f_movementSimulate,f_movementOrderSimulate, f_battleSimulate, f_calculateSimulate, f_checkTargetSimulate;
        Vector3 v3_TargetDirection;
        float f_targetDistance;
        bool b_chaseTarget;
        bool b_targetOutAttackRange;
        bool b_canChaseTarget;
        bool b_CanAttackTarget;
        bool b_targetVisible;
        bool b_targetRotationWithin;
        float f_targetBehindWallCheck;
        bool b_targetHideBehindWall => f_targetBehindWallCheck <=0;
        bool b_targetAvailable => m_Target != null &&!m_Target.m_CharacterInfo.B_Effecting(enum_CharacterEffect.Cloak) && !m_Target.m_Health.b_IsDead;
        bool b_playing;
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

        public EnermyAIControllerBase(EntityCharacterAI _entityControlling, EquipmentBase _weapon, Action<EntityCharacterBase, bool> _onAttack, Func<EntityCharacterBase, bool> _onCheck)
        {
            m_Entity = _entityControlling;
            m_Weapon = _weapon;
            m_Obstacle = m_Entity.GetComponent<NavMeshObstacle>();
            m_Agent = m_Entity.GetComponent<NavMeshAgent>();
            m_Agent.stoppingDistance = 0f;
            OnAttackAnim = _onAttack;
            OnCheckTarget = _onCheck;
            B_AgentEnabled = false;
        }

        public void OnActivate()
        {
            B_AgentEnabled = false;
            b_attacking = false;
            f_battleSimulate = 0f;
            f_movementSimulate = 0f;
            f_checkTargetSimulate = 0f;
            f_calculateSimulate = 0f;
            f_targetDistance = 0f;
        }
        public void OnDeactivate()
        {
            B_AgentEnabled = false;
            m_Weapon.OnDeactivate();
            this.StopAllSingleCoroutines();
        }

        public void OnInfoChange()
        {
            m_Agent.speed = m_Info.F_MovementSpeed;
        }

        public void SetPlay(bool play)
        {
            if (b_playing == play)
                return;

            b_playing = play;
            if (!play)
            {
                StopMoving();
                if (b_attacking)
                    OnAttackFinished();
            }
        }
        public void OnTick(float deltaTime)
        {
            if (!b_playing)
                return;

            CheckTarget(deltaTime);
            CheckTargetParams(deltaTime);
            CheckBattle(deltaTime);
            ChecktAIDestination(deltaTime);
            CheckRotation(deltaTime);
            if (m_Weapon!=null)  m_Weapon.Tick(deltaTime);
        }
        #region TargetIdentity
        void CheckTarget(float deltaTime)
        {
            if (!b_targetAvailable)
                RecheckTarget();

            if (f_checkTargetSimulate > 0)
            {
                f_checkTargetSimulate -= deltaTime;
                return;
            }
            f_checkTargetSimulate = GameConst.F_AITargetCheckParam;

            RecheckTarget();
        }
        void RecheckTarget()
        {
            m_Target = null;
            f_targetDistance = float.MaxValue;

            List<EntityCharacterBase> entites = GameManager.Instance.GetEntities(m_Entity.m_Flag, m_Weapon.B_TargetAlly);
            for (int i = 0; i < entites.Count; i++)
            {
                if (entites[i].I_EntityID == m_Entity.I_EntityID)
                    continue;

                float distance = TCommon.GetXZDistance(headTransform.position, entites[i].tf_Head.position);
                bool obstacleBlocked = !m_Entity.B_BattleCheckObstacle || ObstacleBlocked(entites[i]);
                bool isAvailableClosetTarget = distance < f_targetDistance && obstacleBlocked;
                if (!b_targetAvailable || isAvailableClosetTarget)
                {
                    m_Target = entites[i];
                    f_targetDistance = distance;
                }
            }
        }

        void CheckTargetParams(float deltaTime)
        {
            if (f_calculateSimulate > 0)
            {
                f_calculateSimulate -= deltaTime;
                return;
            }
            f_calculateSimulate = GameConst.F_AITargetCalculationParam;

            if (!b_targetAvailable)
                return;

            b_targetVisible = ObstacleBlocked(m_Target);

            f_targetBehindWallCheck+=((b_targetVisible?.5f:-1)*f_calculateSimulate);
            f_targetBehindWallCheck = Mathf.Clamp(f_targetBehindWallCheck,0,8f);

            f_targetDistance = TCommon.GetXZDistance(targetHeadTransform.position, headTransform.position);
            b_chaseTarget = f_targetDistance > m_Entity.F_AIChaseRange;
            b_targetOutAttackRange = f_targetDistance > m_Entity.F_AIAttackRange;
            b_canChaseTarget = b_targetHideBehindWall || b_chaseTarget;
            b_CanAttackTarget = !b_targetOutAttackRange  && b_targetRotationWithin && (!m_Entity.B_BattleCheckObstacle||b_targetVisible) &&!FrontBlocked();
        }
        #endregion
        #region Battle
        int i_playCount;
        bool b_preAim;
        bool b_attacking;
        void CheckBattle(float deltaTime)
        {
            if (f_battleSimulate > 0)
                f_battleSimulate -= m_Info.F_ReloadRateTick(deltaTime);

            if (!b_targetAvailable|| f_battleSimulate >0||!b_CanAttackTarget)
                return;

            b_preAim = TCommon.RandomPercentage() >= m_Entity.I_AttackPreAimPercentage;
            i_playCount = m_Entity.F_AttackTimes.RandomRangeInt();
            i_playCount = i_playCount <= 0 ? 1 : i_playCount;       //Make Sure Play Once At Least
            b_attacking = true;
            m_Weapon.OnPlayAnim(true);
            this.StartSingleCoroutine(0, Attack(i_playCount, m_Entity.F_AttackRate));

            f_battleSimulate = m_Entity.F_AttackRate * i_playCount + m_Entity.F_AttackDuration.RandomRangeFloat();
        }
        IEnumerator Attack(int count,float fireRate)
        {
            float m_attackSimulate= 0f;
            float m_frontCheck = fireRate;
            for (; ; )
            {
                float fireRateDeltatime= m_Entity.m_CharacterInfo.F_FireRateTick(Time.deltaTime);
                m_frontCheck -= fireRateDeltatime;
                if (m_frontCheck <= 0)
                {
                    m_frontCheck = .3f;
                    if (FrontBlocked())
                    {
                        OnAttackFinished();
                        yield break;
                    }
                }

                m_attackSimulate -= fireRateDeltatime;
                if (m_attackSimulate <= 0 )
                {
                    m_attackSimulate = fireRate;

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

        public void OnAttackTriggered()
        {
            if(b_targetAvailable)
                m_Weapon.Play(b_preAim, m_Target);
        }
        void OnAttackFinished()
        {
            b_attacking = false;
            m_Weapon.OnPlayAnim(false);
            OnAttackAnim(m_Target, false);
        }
        #endregion
        #region Position
        void ChecktAIDestination(float deltaTime)
        {
            //Time Param Calculate
            float movementDelta = deltaTime * m_Info.F_MovementSpeedMultiply;
            if (f_movementOrderSimulate > 0)  f_movementOrderSimulate -= movementDelta;
            if (f_movementSimulate > 0) { f_movementSimulate -= movementDelta; return; }
            f_movementSimulate = GameConst.F_AIMovementCheckParam;

            //Force Hold Position
            if (CheckHoldPosition()) { StopMoving();  return; }

            //Normal Positioning
            if (!CheckDestinationReached() && f_movementOrderSimulate > 0) return;

            bool willIdle = !b_chaseTarget && TCommon.RandomPercentage()>GameConst.I_AIIdlePercentage;
            if (willIdle)
                StopMoving();
            else
                SetDestination(GetSamplePosition());
            f_movementOrderSimulate = willIdle ? GameExpression.GetAIIdleDuration() : GameConst.F_AIMaxRepositionDuration;
        }
        bool CheckHoldPosition() => m_Entity.m_CharacterInfo.F_MovementSpeed == 0 || (b_attacking && !m_Entity.B_AttackMove);
        bool CheckDestinationReached() => B_AgentEnabled && m_Agent.remainingDistance <= .3f;

        void StopMoving() { B_AgentEnabled = false; }
        void SetDestination(Vector3 destination) { B_AgentEnabled = true; m_Agent.SetDestination(destination); } 

        Vector3 GetSamplePosition()=>LevelManager.NavMeshPosition(  m_Entity.transform.position + (b_canChaseTarget?3:-3)*(v3_TargetDirection.normalized) + TCommon.RandomXZSphere(3f));

        bool FrontBlocked()=> m_Entity.B_AttackFrontCheck && Physics.SphereCast(new Ray(headTransform.position, headTransform.forward), .5f, 2, GameLayer.Mask.I_Static);
        bool ObstacleBlocked(EntityCharacterBase target)
        {
            m_Raycasts = Physics.RaycastAll(m_Entity.tf_Head.position, v3_TargetDirection, Vector3.Distance(m_Entity.tf_Head.position, target.tf_Head.position), GameLayer.Mask.I_StaticEntity);
            for (int i = 0; i < m_Raycasts.Length; i++)
            {
                if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Static)
                    return false;
                else if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Entity)
                {
                    HitCheckEntity entity = m_Raycasts[i].collider.GetComponent<HitCheckEntity>();
                    if (entity.m_Attacher.I_EntityID != target.I_EntityID && entity.m_Attacher.I_EntityID != m_Entity.I_EntityID)
                        return false;
                }
            }
            return true;
        }
        
        void CheckRotation(float deltaTime)
        {
            if (!b_targetAvailable)
                return;

            v3_TargetDirection = TCommon.GetXZLookDirection(headTransform.position, targetHeadTransform.position);
            if (v3_TargetDirection == Vector3.zero)
                return;

            b_targetRotationWithin = Mathf.Abs(TCommon.GetAngle(v3_TargetDirection, transform.forward, Vector3.up)) < 15;
            m_Agent.updateRotation = b_targetOutAttackRange;
            if (!m_Agent.updateRotation)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(v3_TargetDirection, Vector3.up), Time.deltaTime * 5 * (b_attacking ? m_Entity.F_AttackRotateParam : 1));
        }
        #endregion
    }
    #endregion
#if UNITY_EDITOR
    CapsuleCollider hitbox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        if(!hitbox)
        hitbox = GetComponent<CapsuleCollider>();
        Gizmos.color = Color.red;
        Gizmos_Extend.DrawWireCapsule(transform.position+Vector3.up*hitbox.height/2,Quaternion.LookRotation(transform.forward,transform.up),hitbox.transform.localScale,hitbox.radius,hitbox.height);
    }
#endif
}
