
using System.Collections.Generic;
using TSpecial;
using UnityEngine;

public class LivingCharacterTester : LivingCharacterBase
{
    public override enum_Flags E_Flag => enum_Flags.TesterCO;
    public override enum_DamageType E_DamageType => enum_DamageType.Melee;
    public float F_AttackCoolDown=.5f;
    public float F_AttackDamage = 10f;
    public TesterAnimator m_Animator;
    protected override void Awake()
    {
        base.Awake();
        m_Animator = new TesterAnimator(GetComponent<Animator>(),new List<SAnimatorParam> (){new SAnimatorParam("attack",TesterAnimator.fm_Attack,F_AttackCoolDown) });
        m_AIController = new NavigationAgentAI<HitCheckLiving>(transform, GameLayersPhysics.IL_AllLiving, F_MoveSpeed, F_DetectRange, F_AttackRange, F_FollowRange, IsAvailableTarget, AttackTarget);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        m_Animator.Reset();
    }
    protected override float AttackTarget(HitCheckLiving target)
    {
        target.OnHitCheck(F_AttackDamage, E_DamageType, transform.position - target.m_Attacher.transform.position,this);
        m_Animator.Attack();
        return F_AttackCoolDown;
    }
}
public class TesterAnimator : AnimatorClippingTime
{
    #region Readonly
    public static readonly int t_Attack=Animator.StringToHash("t_attack");
    public static readonly int fm_Attack = Animator.StringToHash("fm_attack");
    #endregion
    public TesterAnimator(Animator _animator, List<SAnimatorParam> _animatorParams) : base(_animator, _animatorParams)
    {
    }
    public void Attack()
    {
        am_current.SetTrigger(t_Attack);
    }
}