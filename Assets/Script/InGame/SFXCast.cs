using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SFXCast : SFXBase,ISingleCoroutine {
    ParticleSystem[] m_Particles;
    protected DamageInfo m_DamageInfo;
    public float F_Damage;
    public int I_TickCount=1;
    public float F_Tick = .5f;
    public int I_BuffApplyOnCast;
    protected virtual float F_ParticleDuration => 5f;
    public bool B_Casting { get; private set; } = false;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        m_Particles.Traversal((ParticleSystem particle)=> {particle.Stop();});
        m_DamageInfo = new DamageInfo(F_Damage, enum_DamageType.Area);
    }
    public virtual void Play(int sourceID,DamageBuffInfo buffInfo)
    {
        PlaySFX(sourceID, I_TickCount*F_Tick+5f);
        B_Casting = true;
        if (I_BuffApplyOnCast>0)
            buffInfo.m_BuffAplly.Add(I_BuffApplyOnCast);
        m_DamageInfo.ResetBuff(buffInfo);

        this.StartSingleCoroutine(0, TIEnumerators.TickCount(OnBlast, I_TickCount, F_Tick, () => {
            m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
            B_Casting = false;
        }));
    }
    public virtual void PlayControlled(int sourceID, Transform attachTo, bool play)
    {
        B_Casting = play;
        if (play)
        {
            transform.SetParent(attachTo);
            this.StartSingleCoroutine(0, TIEnumerators.Tick(OnBlast, F_Tick));
            PlaySFX(sourceID, int.MaxValue);
        }
        else
        {
            this.StopSingleCoroutine(0);
            f_TimeCheck = Time.time + F_ParticleDuration;
            m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
        }
    }
    protected void OnDisable()
    {
        this.StopAllCoroutines();
    }
    protected virtual void OnBlast()
    {
        TCommon.Traversal(m_Particles, (ParticleSystem particle) => { particle.Play(); });
        Collider[] collider = OnBlastCheck();
        List<int> targetHitted = new List<int>();
        for (int i = 0; i < collider.Length; i++)
        {
            HitCheckEntity entity = collider[i].DetectEntity();
            if (entity!=null&&!targetHitted.Contains(entity.I_AttacherID)&&GameManager.B_CanDamageEntity(entity, I_SourceID))
            {
                targetHitted.Add(entity.I_AttacherID);
                OnDamageEntity(entity);
            }
        }
    }
    protected virtual Collider[] OnBlastCheck()
    {
        Debug.LogError("Override This Please");
        return null;
    }   
    protected virtual void OnDamageEntity(HitCheckEntity hitEntity)
    {
        hitEntity.TryHit(m_DamageInfo);
    }
}
