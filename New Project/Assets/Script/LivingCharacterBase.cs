using UnityEngine;
using TSpecial;
using System.Collections;

public class LivingCharacterBase : LivingBase, ISingleCoroutine
{
    public float F_MoveSpeed = 3.5f;
    public float F_AttackRange = 1f;
    public float F_FollowRange = 5f;
    public float F_DetectRange = 10f;
    public virtual enum_DamageType E_DamageType => enum_DamageType.Invalid;
    Renderer[] m_renderers;
    protected RagdollAnimationTransition m_transition;
    protected NavigationAgentAI<HitCheckLiving> m_AIController;
    protected override void Awake()
    {
        base.Awake();
        m_renderers = GetComponentsInChildren<Renderer>();
        Rigidbody[] rigidbodys = new Rigidbody[ar_hitChecks.Length];
        for (int i = 0; i < rigidbodys.Length; i++)
            rigidbodys[i] = ar_hitChecks[i].rb_current;
        m_transition = new RagdollAnimationTransition(GetComponent<Animator>(), rigidbodys, transform.Find("Model/Bip01/Bip01 Pelvis"));
        m_transition.Reset(true);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        m_AIController.Start(NavigationAgentAI<HitCheckLiving>.enum_AIState.Idle, true,null);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        m_transition.Reset(true);
        m_AIController.OnDestroy();
        TCommon.TraversalArray(m_renderers, (Renderer rd) => { rd.material.SetFloat("_DissolveAmount", 0); });

        if (cor_attack != null)
            StopCoroutine(cor_attack);
        this.StopAllCoroutines();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_AIController.OnDestroy();
    }
    public override bool? TakeDamage(float damage, enum_DamageType damageType,LivingBase hitSource)
    {
        if (damageType == enum_DamageType.Interact)
        {
            OnInteract(hitSource);
            return true;
        }
        else
        {
            HitCheckLiving target = hitSource.m_HitPart as HitCheckLiving;
            if (!m_AIController.B_HaveAttackTarget && IsAvailableTarget(target,false))
                m_AIController.SetTarget(target);
            return base.TakeDamage(damage, damageType, hitSource);
        }
    }
    protected virtual void OnInteract(LivingBase player)
    {

    }
    protected virtual bool IsAvailableTarget(HitCheckLiving living,bool isFriendly)
    {
            return !living.m_Attacher.isDead && living.m_Attacher.E_Flag != enum_Flags.Neutal &&(isFriendly? living.m_Attacher.E_Flag== E_Flag:living.m_Attacher.E_Flag != E_Flag);
    }
    Coroutine cor_attack;
    protected virtual float AttackTarget(HitCheckLiving target)
    {
        if (cor_attack != null)
            StopCoroutine(cor_attack);

        cor_attack = StartCoroutine(IE_Attack(target));
        return 0f;
    }
    protected virtual IEnumerator IE_Attack(HitCheckLiving target)
    {
        Debug.LogError("Override And Hide This Please");
        yield return null;
    }
    protected override void OnDead()
    {
        m_AIController.OnDead();
        m_transition.SetState(false);
        this.StartSingleCoroutine(1, TIEnumerators.ChangeValueTo((float value) => {
            TCommon.TraversalArray(m_renderers, (Renderer rd) => { rd.material.SetFloat("_DissolveAmount", value); });
        }, 0, 1, 10f, Recycle));

        if (cor_attack != null)
            StopCoroutine(cor_attack);
    }
}