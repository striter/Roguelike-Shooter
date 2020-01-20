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
    public override enum_EntityController m_ControllType => enum_EntityController.AI;
    public float F_AIChaseRange;
    public float F_AIAttackRange;
    public RangeFloat F_AttackDuration;
    public RangeInt F_AttackTimes;
    public float F_AttackRate;
    EnermyAnimator m_Animator;
    [Range(0,3)]
    public float F_AttackRotateParam=1f;
    [Range(0, 100)]
    public int I_AttackPreAimPercentage = 50;
    public bool B_AttackMove = true;
    public float F_AttackFrontCheck = 2f;
    bool OnCheckTarget(EntityCharacterBase target) => target.m_Flag!=m_Flag && !target.m_IsDead;
    Transform tf_Barrel;
    public override Transform tf_Weapon => tf_Barrel;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        tf_Barrel = transform.FindInAllChild("Barrel");
        InitAI(WeaponHelperBase.AcquireWeaponHelper(GameExpression.GetAIWeaponIndex(m_Identity, 0),this,m_CharacterInfo.GetDamageBuffInfo));
        if (E_AnimatorIndex != enum_EnermyAnim.Invalid)
            m_Animator = new EnermyAnimator(tf_Model.GetComponent<Animator>(), OnAnimKeyEvent);
    }

    public override void OnActivate(enum_EntityFlag _flag, int _spawnerID, float startHealth)
    {
        if (m_Animator!=null)
            m_Animator.OnActivate(E_AnimatorIndex);
        AIActivate();
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
        AIDeactivate();
        if (m_Animator != null)
            m_Animator.OnDead();
    }

    protected override void OnExpireChange()
    {
        base.OnExpireChange();
        if (m_Animator != null)
            m_Animator.SetMovementSpeed(m_CharacterInfo.F_MovementSpeed/4f);
        m_Agent.speed = m_CharacterInfo.F_MovementSpeed;
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (m_Animator != null)
        {
            m_Animator.SetForward(m_Moving ? 1f:0f);
            m_Animator.SetPause(m_CharacterInfo.B_Effecting( enum_CharacterEffect.Freeze));
        }

        AISetSimulate(!m_CharacterInfo.B_Effecting( enum_CharacterEffect.Freeze));
        AITick(Time.deltaTime);
    }

    protected override void OnDeadTick(float deltaTime)
    {
        base.OnDeadTick(deltaTime);
        if(m_Animator!=null)  m_Animator.SetPause(false);
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
        else if(startAttack) OnAttackAnimTrigger();
    } 

    protected void OnAnimKeyEvent(TAnimatorEvent.enum_AnimEvent animEvent)
    {
        switch (animEvent)
        {
            case TAnimatorEvent.enum_AnimEvent.Fire:
                OnAttackAnimTrigger();
                break;
        }
    }

    #region AI

    protected NavMeshAgent m_Agent;
    protected NavMeshObstacle m_Obstacle;
    protected WeaponHelperBase m_Weapon;
    TimeCounter m_MoveTimer = new TimeCounter(), m_MoveOrderTimer = new TimeCounter(),m_BattleDurationTimer=new TimeCounter(),m_TargetingTimer=new TimeCounter(),m_BattleFireTimer=new TimeCounter();
    Vector3 m_m_forwardDirection;
    Vector3 m_idlePosition;
    float m_m_targetDistance;

    bool m_m_targetOutChaseRange;
    bool m_m_targetOutAttackRange;
    bool m_b_CanStartAttack;
    bool m_b_CanKeepAttack;
    bool m_m_targetRotationWithin;

    public EntityCharacterBase m_Target { get; private set; }
    public bool m_AISimluating { get; private set; }
    public bool m_Moving => m_AgentEnabled && m_Agent.remainingDistance>.2f;
    public bool m_Idling =>!m_Target&& TCommon.GetXZDistance(m_idlePosition,transform.position)<.2f;
    public bool m_AgentEnabled
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

    void InitAI(WeaponHelperBase _weapon)
    {
        m_Weapon = _weapon;
        m_Agent = GetComponent<NavMeshAgent>();
        m_Obstacle = GetComponent<NavMeshObstacle>();
        m_Obstacle.carving = false;
        m_Obstacle.height = m_Agent.height;
        m_Obstacle.radius = m_Agent.radius;
        m_Agent.stoppingDistance = 0f;
    }

    public void AIActivate()
    {
        m_AgentEnabled = false;
        m_Agent.enabled = false;
        m_Obstacle.enabled = true;
        b_attacking = false;
        m_MoveTimer.SetTimer(0);
        m_BattleDurationTimer.SetTimer(0);
        m_BattleFireTimer.SetTimer(0);
        m_MoveOrderTimer.SetTimer(0);
        m_TargetingTimer.SetTimer(0);
        m_m_targetDistance = 0f;
        m_idlePosition = transform.position;
        AISetSimulate(true);
    }
    public void AIDeactivate()
    {
        StopMoving();
        OnAttackFinished();
    }

    public void AISetSimulate(bool play)
    {
        if (m_AISimluating == play)
            return;

        m_AISimluating = play;
        if (!m_AISimluating)
        {
            StopMoving();
            PauseAttack();
        }
    }

    public void AITick(float deltaTime)
    {
        if (!m_AISimluating)
            return;

        CheckTarget(deltaTime);
        
        CheckBattle(deltaTime);
        CheckMovement(deltaTime);
        CheckRotation(deltaTime);
    }

    void CheckTarget(float deltaTime)
    {
        m_TargetingTimer.Tick(deltaTime);
        if (m_TargetingTimer.m_Timing)
            return;
        m_Target = GameManager.Instance.GetAvailableEntity(this, m_Weapon.B_TargetAlly, false, GameConst.F_AITargetDistance);

        m_TargetingTimer.SetTimer(m_Target? GameConst.F_AIReTargetCheckParam:GameConst.F_AITargetCheckParam);
    }

    #region Battle
    void CheckBattle(float deltaTime)
    {
        if (b_attacking)
        {
            CheckAttack(deltaTime);
            return;
        }

        bool attackBlocked = FrontBlocked();
        m_b_CanStartAttack = !m_m_targetOutAttackRange && m_m_targetRotationWithin && !attackBlocked;
        m_b_CanKeepAttack = !attackBlocked;

        m_BattleDurationTimer.Tick(m_CharacterInfo.F_ReloadRateTick(deltaTime));
        if (m_BattleDurationTimer.m_Timing || !m_b_CanStartAttack)
            return;

        StartAttack(F_AttackTimes.RandomRange(), TCommon.RandomPercentage() >= I_AttackPreAimPercentage);
    }

    bool b_attacking = false;
    int i_playCount = -1;
    bool b_preAim = false;
    void StartAttack(int attackTimes, bool preAim)
    {
        i_playCount = (attackTimes <= 0 ? 1 : attackTimes);       //Make Sure Play Once At Least
        b_preAim = preAim;
        b_attacking = true;

        if (m_Weapon.B_LoopAnim)
        {
            OnAttackAnim(true);
            m_BattleFireTimer.SetTimer( F_AttackRate);
        }
    }
    void PauseAttack()
    {
        if (b_attacking)
            m_Weapon.OnStopPlay();
    }
    void CheckAttack(float deltaTime)
    {
        if (!m_b_CanKeepAttack)
        {
            OnAttackFinished();
            return;
        }

        if (i_playCount <= 0)
            return;

        m_BattleFireTimer.Tick(m_CharacterInfo.F_FireRateTick(deltaTime));
        if (m_BattleFireTimer.m_Timing)
            return;
        m_BattleFireTimer.SetTimer(F_AttackRate);

        i_playCount--;
        if (m_Weapon.B_LoopAnim)
            OnAttackAnimTrigger();
        else
            OnAttackAnim(true);
    }

    protected virtual void OnAttackAnimTrigger()
    {
        if (m_Target)
            m_Weapon.OnPlay(b_preAim, m_Target);

        if (i_playCount <= 0)
            OnAttackFinished();
    }

    void OnAttackFinished()
    {
        b_attacking = false;
        m_Weapon.OnStopPlay();
        OnAttackAnim(false);
        m_BattleDurationTimer.SetTimer(F_AttackDuration.RandomRangeFloat());
    }
    #endregion
    #region Position
    void CheckMovement(float deltaTime)
    {
        m_m_targetDistance = m_Target ? TCommon.GetXZDistance(m_Target.transform.position, transform.position) : float.MaxValue;
        m_m_targetOutChaseRange = m_m_targetDistance > F_AIChaseRange;
        m_m_targetOutAttackRange = m_m_targetDistance > F_AIAttackRange;

        float movementDelta = deltaTime * m_CharacterInfo.F_MovementSpeedMultiply;
        m_MoveTimer.Tick(movementDelta);
        m_MoveOrderTimer.Tick(movementDelta);
        if (m_MoveTimer.m_Timing)
            return;
        
        m_MoveTimer.SetTimer(GameConst.F_AIMovementCheckParam);
        if (ForceHoldPosition()) { StopMoving(); return; }
        
        if (m_MoveOrderTimer.m_Timing)
            return;

        if (m_Target)
        {
            if (m_AgentEnabled && !m_m_targetOutChaseRange && TCommon.RandomPercentage() > GameConst.I_AIIdlePercentage)
            {
                StopMoving();
                m_MoveOrderTimer.SetTimer(GameExpression.GetAIIdleDuration());
                return;
            }

            SetDestination(GetSamplePosition(m_m_targetOutChaseRange));
            m_MoveOrderTimer.SetTimer(GameConst.F_AIMaxRepositionDuration);
            return;
        }

        if(!m_Idling)
            SetDestination(m_idlePosition);
        m_MoveOrderTimer.SetTimer(GameConst.F_AIMaxRepositionDuration);
        return;
    }
    bool ForceHoldPosition() => m_CharacterInfo.F_MovementSpeed == 0 || (b_attacking && !B_AttackMove);

    void StopMoving() { m_AgentEnabled = false; }
    void SetDestination(Vector3 destination) { m_AgentEnabled = true; m_Agent.SetDestination(destination); }
    Vector3 GetSamplePosition(bool forward)
    {
        if (forward)
            return m_Target.transform.position + TCommon.RandomXZSphere(3f);
        else
            return transform.position  +5 * -m_m_forwardDirection;
    }
    bool FrontBlocked() => F_AttackFrontCheck > 0f && Physics.SphereCast(new Ray(transform.position, transform.forward), .5f, F_AttackFrontCheck, GameLayer.Mask.I_Static);

    void CheckRotation(float deltaTime)
    {
        m_Agent.updateRotation = !m_Target;
        if (!m_Target)
            return;

        m_m_forwardDirection = TCommon.GetXZLookDirection(transform.position, m_Target.transform.position);
        m_m_targetRotationWithin = Mathf.Abs(TCommon.GetAngle(m_m_forwardDirection, transform.forward, Vector3.up)) < 15;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_m_forwardDirection, Vector3.up), Time.deltaTime * 5 * (b_attacking ? F_AttackRotateParam : 1));
    }
    #endregion
    #endregion
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
#if UNITY_EDITOR
    CapsuleCollider hitbox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying &&GameManager.Instance&&!GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        if(!hitbox)
        hitbox = GetComponent<CapsuleCollider>();
        Gizmos.color = Color.red;
        Gizmos_Extend.DrawWireCapsule(transform.position+Vector3.up*hitbox.height/2,Quaternion.LookRotation(transform.forward,transform.up),hitbox.transform.localScale,hitbox.radius,hitbox.height);
    }
#endif
}
