using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;
using System;

[RequireComponent(typeof(NavMeshAgent),typeof(NavMeshObstacle))]
public class EntityCharacterAI : EntityCharacterBase {
    public enum_EnermyAnim E_AnimatorIndex= enum_EnermyAnim.Invalid;
    public override enum_EntityController m_Controller => enum_EntityController.AI;
    public float F_AIChaseRange;
    public float F_AIAttackRange;
    public RangeFloat F_AttackDuration;
    public RangeInt F_AttackTimes;
    public float F_AttackRate;
    protected EnermyAIControllerBase m_AI { get; private set; }
    EnermyAnimator m_Animator;
    public bool B_BattleCheckObstacle = false;
    [Range(0,3)]
    public float F_AttackRotateParam=1f;
    [Range(0, 100)]
    public int I_AttackPreAimPercentage = 50;
    public bool B_AttackMove = true;
    public float F_AttackFrontCheck = 2f;
    bool OnCheckTarget(EntityCharacterBase target) => target.m_Flag!=m_Flag && !target.m_IsDead;
    public override Vector3 m_PrecalculatedTargetPos(float time)=> tf_Head.position;
    Transform tf_Barrel;
    public override Transform tf_Weapon => tf_Barrel;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        tf_Barrel = transform.FindInAllChild("Barrel");
        m_AI = new EnermyAIControllerBase(this, EquipmentBase.AcquireEquipment(GameExpression.GetAIEquipmentIndex(m_Identity, 0),this,m_CharacterInfo.GetDamageBuffInfo), OnAttackAnim, OnCheckTarget);
        if (E_AnimatorIndex != enum_EnermyAnim.Invalid)
            m_Animator = new EnermyAnimator(tf_Model.GetComponent<Animator>(), OnAnimKeyEvent);
    }

    public override void OnActivate(enum_EntityFlag _flag, int _spawnerID, float startHealth)
    {
        if (m_Animator!=null)
            m_Animator.OnActivate(E_AnimatorIndex);
        m_AI.OnActivate();
        base.OnActivate(_flag,_spawnerID,startHealth);
    }

    protected override void OnRevive()
    {
        base.OnRevive();
        if (m_Animator != null)
            m_Animator.OnRevive();
    }
    protected override void OnDead()
    {
        base.OnDead();
        m_AI.OnDeactivate();
        if (m_Animator != null)
            m_Animator.OnDead();
    }

    protected override void OnExpireChange()
    {
        base.OnExpireChange();
        if (m_Animator != null)
            m_Animator.SetMovementSpeed(m_CharacterInfo.F_MovementSpeed/4f);
        m_AI.m_Agent.speed = m_CharacterInfo.F_MovementSpeed;
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (m_Animator != null)
        {
            m_Animator.SetForward(m_AI.B_AgentEnabled ? 1f:0f);
            m_Animator.SetPause(m_CharacterInfo.B_Effecting( enum_CharacterEffect.Freeze));
        }

        m_AI.SetPlay(!m_CharacterInfo.B_Effecting( enum_CharacterEffect.Freeze));
        m_AI.OnTick(Time.deltaTime);
    }

    protected override bool OnReceiveDamage(DamageInfo damageInfo, Vector3 damageDirection)
    {
        if(damageDirection!=Vector3.zero)
            transform.Translate(damageDirection *GameConst.F_AIDamageTranslate * -damageInfo.m_AmountApply);
        return base.OnReceiveDamage(damageInfo, damageDirection);
    }

    protected virtual void OnAttackAnim(bool startAttack)
    {
        if (m_Animator!=null) m_Animator.OnAttack(startAttack);
        else if(startAttack) m_AI.OnAttackAnimTrigger();
    } 

    protected void OnAnimKeyEvent(TAnimatorEvent.enum_AnimEvent animEvent)
    {
        switch (animEvent)
        {
            case TAnimatorEvent.enum_AnimEvent.Fire:
                m_AI.OnAttackAnimTrigger();
                break;
        }
    }
    
    protected class EnermyAnimator : CharacterAnimator
    {
        static readonly int HS_T_Attack = Animator.StringToHash("t_attack");
        static readonly int HS_B_Attack = Animator.StringToHash("b_attack");
        public EnermyAnimator(Animator _animator,Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator,_OnAnimEvent)
        {
            m_Animator.fireEvents = true;
        }
        public void OnActivate(enum_EnermyAnim _animIndex) => OnActivate((int)_animIndex);
        public void OnAttack(bool attack)
        {
            if(attack)
                m_Animator.SetTrigger(HS_T_Attack);
            m_Animator.SetBool(HS_B_Attack,attack);
        }
    }
    #region AI
    protected class  EnermyAIControllerBase
    {
        public NavMeshAgent m_Agent { get; private set; }
        protected NavMeshObstacle m_Obstacle;
        protected EntityCharacterAI m_Entity;
        protected EntityCharacterBase m_Target;
        protected Transform transform => m_Agent.transform;
        protected Transform headTransform => m_Entity.tf_Head;
        protected Transform targetHeadTransform => m_Target.tf_Head;
        protected Action<bool> OnAttackAnim;
        protected Func<EntityCharacterBase, bool> OnCheckTarget;
        protected EquipmentBase m_Weapon;
        protected CharacterInfoManager m_Info => m_Entity.m_CharacterInfo;
        RaycastHit[] m_Raycasts;
        float f_movementSimulate,f_movementOrderSimulate, f_battleSimulate, f_calculateSimulate, f_checkTargetSimulate,f_fireCheckSimulate;
        Vector3 m_forwardDirection;
        float f_targetDistance;

        bool b_targetOutChaseRange;
        bool b_targetOutAttackRange;
        bool b_CanStartAttack;
        bool b_CanKeepAttack;
        bool b_targetVisible;
        bool b_targetRotationWithin;
        bool b_targetAvailable => m_Target != null && GameManager.Instance.CheckEntityTargetable(m_Target); 
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

        public EnermyAIControllerBase(EntityCharacterAI _entityControlling, EquipmentBase _weapon, Action<bool> _onAttack, Func<EntityCharacterBase, bool> _onCheck)
        {
            m_Entity = _entityControlling;
            m_Weapon = _weapon;
            m_Agent = m_Entity.GetComponent<NavMeshAgent>();
            m_Obstacle = m_Entity.GetComponent<NavMeshObstacle>();
            m_Obstacle.carving = true;
            m_Obstacle.carveOnlyStationary = false;
            m_Obstacle.height = m_Agent.height;
            m_Obstacle.radius=m_Agent.radius-.2f;
            m_Agent.stoppingDistance = 0f;
            OnAttackAnim = _onAttack;
            OnCheckTarget = _onCheck;
        }

        public void OnActivate()
        {
            B_AgentEnabled = false;
            m_Agent.enabled = false;
            m_Obstacle.enabled = true;
            b_attacking = false;
            f_battleSimulate = 0f;
            f_movementSimulate = 0f;
            f_checkTargetSimulate = 0f;
            f_calculateSimulate = 0f;
            f_targetDistance = 0f;
            SetPlay(true);
        }
        public void OnDeactivate()
        {
            SetPlay(false);
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
            if (b_targetAvailable&&f_checkTargetSimulate > 0)
            {
                f_checkTargetSimulate -= deltaTime;
                return;
            }
            f_checkTargetSimulate = GameConst.F_AITargetCheckParam;

            m_Target = GameManager.Instance.GetAvailableEntity(m_Entity,m_Weapon.B_TargetAlly,m_Entity.B_BattleCheckObstacle);
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

            b_targetVisible = GameManager.Instance.CheckEntityObstacleBetween(m_Entity,m_Target);
            
            f_targetDistance = TCommon.GetXZDistance(targetHeadTransform.position, headTransform.position);
            b_targetOutChaseRange = f_targetDistance > m_Entity.F_AIChaseRange;
            b_targetOutAttackRange = f_targetDistance > m_Entity.F_AIAttackRange;
            bool attackBlocked = (m_Entity.B_BattleCheckObstacle && !b_targetVisible) || FrontBlocked();
            b_CanStartAttack = b_targetAvailable&& !b_targetOutAttackRange  && b_targetRotationWithin && !attackBlocked;
            b_CanKeepAttack = b_targetAvailable && !attackBlocked;
        }
        #endregion

        #region Battle
        void CheckBattle(float deltaTime)
        {
            if(b_attacking)
            {
                CheckAttack(deltaTime);
                return;
            }

            if (f_battleSimulate > 0)
                f_battleSimulate -= m_Info.F_ReloadRateTick(deltaTime);

            if (f_battleSimulate > 0 || !b_CanStartAttack)
                return;
            
            StartAttack(m_Entity.F_AttackTimes.Random(), TCommon.RandomPercentage() >= m_Entity.I_AttackPreAimPercentage);
        }

        bool b_attacking=false;
        int i_playCount=-1;
        bool b_preAim=false;
        void StartAttack(int attackTimes,bool preAim)
        {
            i_playCount = (attackTimes <= 0 ? 1 : attackTimes);       //Make Sure Play Once At Least
            b_preAim = preAim;
            b_attacking = true;
            m_Weapon.OnSetAttack(true);

            if (m_Weapon.B_LoopAnim)
                OnAttackAnim(true);
        }
        void CheckAttack(float deltaTime)
        {

            if (!b_CanKeepAttack)
            {
                OnAttackFinished();
                return;
            }

            if (i_playCount <= 0)
                return;

            if (f_fireCheckSimulate > 0)
            {
                f_fireCheckSimulate -= m_Entity.m_CharacterInfo.F_FireRateTick(deltaTime);
                return;
            }
            f_fireCheckSimulate = m_Entity.F_AttackRate;

            i_playCount--;
            if (m_Weapon.B_LoopAnim)
                OnAttackAnimTrigger();
            else
                OnAttackAnim(true);
        }

        public void OnAttackAnimTrigger()
        {
            if (b_targetAvailable)
                m_Weapon.Play(b_preAim, m_Target);

            if (i_playCount <= 0)
                OnAttackFinished();
        } 

        void OnAttackFinished()
        {
            b_attacking = false;
            m_Weapon.OnSetAttack(false);
            OnAttackAnim(false);
            f_battleSimulate = m_Entity.F_AttackDuration.RandomRangeFloat();
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

            bool inrangeIdle = B_AgentEnabled&&!b_targetOutChaseRange && TCommon.RandomPercentage()>GameConst.I_AIIdlePercentage;
            if (inrangeIdle)
                StopMoving();
            else
                SetDestination(GetSamplePosition());
            f_movementOrderSimulate = inrangeIdle ? GameExpression.GetAIIdleDuration() : GameConst.F_AIMaxRepositionDuration;
        }

        bool CheckHoldPosition() => m_Entity.m_CharacterInfo.F_MovementSpeed == 0 || (b_attacking && !m_Entity.B_AttackMove);
        bool CheckDestinationReached() => B_AgentEnabled && m_Agent.remainingDistance <= .3f;

        void StopMoving() { B_AgentEnabled = false; }
        void SetDestination(Vector3 destination) { B_AgentEnabled = true; m_Agent.SetDestination(destination); }
        Vector3 GetSamplePosition()=> LevelManager.NavMeshPosition(m_Entity.transform.position + (b_targetOutChaseRange ? 5 : -5) * (m_forwardDirection.normalized) + TCommon.RandomXZSphere(3f));
        bool FrontBlocked()=> m_Entity.F_AttackFrontCheck>0f && Physics.SphereCast(new Ray(headTransform.position, headTransform.forward), .5f, m_Entity.F_AttackFrontCheck, GameLayer.Mask.I_Static);

        
        void CheckRotation(float deltaTime)
        {
            if (!b_targetAvailable)
                return;

            m_forwardDirection = TCommon.GetXZLookDirection(headTransform.position, targetHeadTransform.position);
            if (m_forwardDirection == Vector3.zero)
                return;

            b_targetRotationWithin = Mathf.Abs(TCommon.GetAngle(m_forwardDirection, transform.forward, Vector3.up)) < 15;
            m_Agent.updateRotation = b_targetOutAttackRange;
            if (!m_Agent.updateRotation)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_forwardDirection, Vector3.up), Time.deltaTime * 5 * (b_attacking ? m_Entity.F_AttackRotateParam : 1));
        }
        #endregion
    }
    #endregion
#if UNITY_EDITOR
    CapsuleCollider hitbox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying &&GameManager.Instance&&GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        if(!hitbox)
        hitbox = GetComponent<CapsuleCollider>();
        Gizmos.color = Color.red;
        Gizmos_Extend.DrawWireCapsule(transform.position+Vector3.up*hitbox.height/2,Quaternion.LookRotation(transform.forward,transform.up),hitbox.transform.localScale,hitbox.radius,hitbox.height);
    }
#endif
}
