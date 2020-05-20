using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TSpecialClasses;
using GameSetting;
using System;
using TPhysics;

[RequireComponent(typeof(NavMeshAgent))]
public class EntityCharacterGameAI : EntityCharacterGameBase {
    #region Preset
    public enum_EnermyAnim E_AnimatorIndex= enum_EnermyAnim.Invalid;
    public float F_AIChaseRange;
    public float F_AIAttackRange;
    public int I_BuffApplyOnHit;
    public float F_AttackRate;
    public RangeFloat F_AttackDuration;
    public RangeInt F_AttackTimes;
    [Range(0,3)] public float F_AttackRotateParam=1f;
    [Range(0, 100)] public int I_AttackPreAimPercentage = 50;
    public bool B_AttackMove = true;
    public float F_AttackFrontCheck = 2f;
    #endregion
    GameCharacterAnimator m_Animator;
    bool OnCheckTarget(EntityCharacterBase target) => target.m_Flag!=m_Flag && !target.m_IsDead;
    Transform tf_Barrel;
    public override Transform tf_Weapon => tf_Barrel;
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        tf_Barrel = transform.FindInAllChild("Barrel");
        InitAI(CharacterWeaponHelperBase.AcquireCharacterWeaponHelper(GameExpression.GetAIWeaponIndex(m_Identity, 0),this,F_Spread));
        if (E_AnimatorIndex != enum_EnermyAnim.Invalid)
            m_Animator = new GameCharacterAnimator(tf_Model.GetComponent<Animator>(), OnAnimKeyEvent);
        m_Agent.enabled = false;
    }

    protected DamageInfo GetDamageInfo() => m_CharacterInfo.GetDamageInfo(F_BaseDamage, 0f,I_BuffApplyOnHit, enum_DamageType.Basic);
    protected override void OnEntityActivate(enum_EntityFlag flag)
    {
        base.OnEntityActivate(flag);
        if (m_Animator != null) m_Animator.OnActivate(E_AnimatorIndex);

        m_Health.OnActivate(I_MaxHealth);
        m_Agent.enabled = true;
        AIActivate();
    }

    public override void OnPoolRecycle()
    {
        base.OnPoolRecycle();
        m_Agent.enabled = false;
    }
    protected override void OnRevive()
    {
        base.OnRevive();
        if (m_Animator != null) m_Animator.OnRevive();
    }
    protected override void OnDead()
    {
        base.OnDead();
        AIDeactivate();
        if (m_Animator != null)  m_Animator.OnDead();
        m_Agent.enabled = false;
    }
    protected override void OnExpireChange()
    {
        base.OnExpireChange();
        if (m_Animator != null)  m_Animator.SetMovementFireSpeed(m_CharacterInfo.m_MovementSpeedMultiply, m_CharacterInfo.m_FireRateMultiply);
        m_Agent.speed = m_CharacterInfo.GetMovementSpeed;
    }

    Vector3 m_DamageImpact;
    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (m_Animator != null) m_Animator.SetForward(m_Moving ? 1f:0f);
        m_DamageImpact = Vector3.Lerp( m_DamageImpact, Vector3.zero, Time.deltaTime * 10f);
        AITick(Time.deltaTime);
    }

    protected override void OnDeadTick(float deltaTime)
    {
        base.OnDeadTick(deltaTime);
        if (m_Animator!=null)  m_Animator.SetPause(false);
        m_DamageImpact = Vector3.Lerp( m_DamageImpact, Vector3.zero, Time.deltaTime * 7f);
        transform.position = NavigationManager.NavMeshPosition(transform.position + m_DamageImpact*deltaTime);
    }

    protected override float OnReceiveDamageAmount(DamageInfo damageInfo, Vector3 direction)
    {
        float amount = base.OnReceiveDamageAmount(damageInfo, direction);
        m_DamageImpact += direction * GameConst.AI.F_AIDeadImpactPerDamageValue * amount;
        return amount;
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
    protected CharacterWeaponHelperBase m_Weapon;
    TimerBase m_TargetingTimer = new TimerBase(), m_MoveTimer = new TimerBase(), m_MoveOrderTimer = new TimerBase(),m_BattleDurationTimer=new TimerBase(),m_BattleFireTimer=new TimerBase();

    public EntityCharacterBase m_Target { get; private set; }
    public bool m_AISimluating { get; private set; }
    public bool m_Moving => m_AgentEnabled && m_Agent.remainingDistance>.2f;
    public bool m_AgentEnabled
    {
        get
        {
            return !m_Agent.isStopped;
        }
        set
        {
            m_Agent.isStopped = !value;
        }
    }

    void InitAI(CharacterWeaponHelperBase _weapon)
    {
        m_Weapon = _weapon;
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.stoppingDistance = 0f;
    }

    void AIActivate()
    {
        m_AgentEnabled = false;
        m_b_attacking = false;
        m_Target = null;
        m_MoveTimer.SetTimerDuration(0);
        m_BattleDurationTimer.SetTimerDuration(0);
        m_BattleFireTimer.SetTimerDuration(0);
        m_MoveOrderTimer.SetTimerDuration(0);
        m_TargetingTimer.SetTimerDuration(0);
        AISetSimulate(true);
    }
    public void AIDeactivate()
    {
        StopMoving();
        FinishAttack();
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
        
        if(BattleTargetCheck(deltaTime))
            BattleTick(deltaTime);
    }
    
    bool BattleTargetCheck(float deltaTime)
    {
        m_TargetingTimer.Tick(deltaTime);
        if (m_Target && !m_Target.m_TargetAvailable)
            m_TargetingTimer.Stop();

        if (m_TargetingTimer.m_Timing)
            return m_Target;

        m_Target = GameManager.Instance.GetNeariesCharacter(this, m_Weapon.B_TargetAlly, false);
        m_TargetingTimer.SetTimerDuration(m_Target ? GameConst.AI.F_AIReTargetCheckParam : GameConst.AI.F_AITargetCheckParam);
        return m_Target;
    }
    
    #region Battle
    Vector3 m_m_targetDirection;
    float m_m_targetDistance;
    bool m_m_targetOutChaseRange;
    bool m_m_targetOutAttackRange;
    bool m_b_CanStartAttack;
    bool m_b_CanKeepAttack;
    bool m_m_targetRotationWithin;
    bool m_b_attacking = false;
    void BattleTick(float deltaTime)
    {
        BattleMovement(deltaTime);
        BattleRotation(deltaTime);

        if (!m_b_attacking)
            BattleCheck(deltaTime);
        else
            AttackCheck(deltaTime);
    }

    void BattleCheck(float deltaTime)
    {
        bool attackBlocked = FrontBlocked();
        m_b_CanStartAttack = !m_m_targetOutAttackRange && m_m_targetRotationWithin && !attackBlocked;
        m_b_CanKeepAttack = !attackBlocked;

        m_BattleDurationTimer.Tick(m_CharacterInfo.DoReloadRateTick(deltaTime));
        if (m_BattleDurationTimer.m_Timing || !m_b_CanStartAttack)
            return;

        StartAttack(F_AttackTimes.Random(), TCommon.RandomPercentageInt() >= I_AttackPreAimPercentage);
    }

    int i_playCount = -1;
    bool b_preAim = false;
    void StartAttack(int attackTimes, bool preAim)
    {
        i_playCount = (attackTimes <= 0 ? 1 : attackTimes);       //Make Sure Play Once At Least
        b_preAim = preAim;
        m_b_attacking = true;
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_OnWillAIAttack, this);
        if (m_Weapon.B_LoopAnim)
        {
            OnAttackAnim(true);
            m_BattleFireTimer.SetTimerDuration( F_AttackRate);
        }
    }

    void PauseAttack()
    {
        if (m_b_attacking)
            m_Weapon.OnStopPlay();
    }

    void AttackCheck(float deltaTime)
    {
        if (!m_b_CanKeepAttack)
        {
            FinishAttack();
            return;
        }

        if (i_playCount <= 0)
            return;

        m_BattleFireTimer.Tick(m_CharacterInfo.DoFireRateTick(deltaTime));
        if (m_BattleFireTimer.m_Timing)
            return;
        m_BattleFireTimer.SetTimerDuration(F_AttackRate);

        i_playCount--;
        if (m_Weapon.B_LoopAnim)
            OnAttackAnimTrigger();
        else
            OnAttackAnim(true);
    }

    protected virtual void OnAttackAnimTrigger()
    {
        if (m_Target)
            m_Weapon.OnPlay(b_preAim, m_Target,GetDamageInfo());

        if (i_playCount <= 0)
            FinishAttack();
    }

    void FinishAttack()
    {
        m_b_attacking = false;
        m_Weapon.OnStopPlay();
        OnAttackAnim(false);
        m_BattleDurationTimer.SetTimerDuration(F_AttackDuration.Random());
    }
    #region Position
    void BattleMovement(float deltaTime)
    {
        m_m_targetDistance = TCommon.GetXZDistance(m_Target.transform.position, transform.position);
        m_m_targetOutChaseRange = m_m_targetDistance > F_AIChaseRange;
        m_m_targetOutAttackRange = m_m_targetDistance > F_AIAttackRange;

        float movementDelta = deltaTime * m_CharacterInfo.m_MovementSpeedMultiply;
        m_MoveTimer.Tick(movementDelta);
        m_MoveOrderTimer.Tick(movementDelta);
        if (m_MoveTimer.m_Timing)
            return;

        m_MoveTimer.SetTimerDuration(GameConst.AI.F_AIMovementCheckParam);
        if (ForceHoldPosition()) { StopMoving(); return; }

        if (m_MoveOrderTimer.m_Timing)
            return;

        if (!m_m_targetOutChaseRange && m_Moving && TCommon.RandomPercentageInt() > GameConst.AI.I_AIBattleIdlePercentage)
        {
            StopMoving();
            m_MoveOrderTimer.SetTimerDuration(GameConst.AI.RF_AIBattleIdleDuration.Random());
            return;
        }

        SetDestination(GetSamplePosition(m_m_targetOutChaseRange));
        m_MoveOrderTimer.SetTimerDuration(GameConst.AI.F_AIMaxRepositionDuration);
    }

    bool ForceHoldPosition() => m_CharacterInfo.GetMovementSpeed == 0 || (m_b_attacking && !B_AttackMove);

    void StopMoving() { m_AgentEnabled = false; }
    void SetDestination(Vector3 destination) { m_AgentEnabled = true; m_Agent.SetDestination(destination); }
    Vector3 GetSamplePosition(bool forward)
    {
        if (forward)
            return m_Target.transform.position + TCommon.RandomXZSphere()* 3f;
        else
            return transform.position + 5 * -m_m_targetDirection;
    }
    bool FrontBlocked() => F_AttackFrontCheck > 0f && Physics.SphereCast(new Ray(tf_Head.position, transform.forward), .5f, F_AttackFrontCheck, GameLayer.Mask.I_Static);

    void BattleRotation(float deltaTime)
    {
        m_Agent.updateRotation = false;
        m_m_targetDirection = TCommon.GetXZLookDirection(transform.position, m_Target.transform.position);
        m_m_targetRotationWithin = Mathf.Abs(TCommon.GetAngle(m_m_targetDirection, transform.forward, Vector3.up)) < 15;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_m_targetDirection, Vector3.up), Time.deltaTime * 5 * (m_b_attacking ? F_AttackRotateParam : 1));
    }
    #endregion
    #endregion
    #endregion
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
