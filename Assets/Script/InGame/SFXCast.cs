using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SFXCast : SFXBase,ISingleCoroutine {
    ParticleSystem[] m_Particles;
    public float F_Damage;
    public int I_TickCount=1;
    public float F_Tick = .5f;
    public bool b_casting;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        m_Particles.Traversal((ParticleSystem particle)=> {particle.Stop();});
    }
    public virtual void Play(int sourceID)
    {
        this.StartSingleCoroutine(0, TIEnumerators.TickCount(OnBlast, I_TickCount, F_Tick,()=> {
            m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
        }));
        PlaySFX(sourceID, I_TickCount*F_Tick+5f);
        b_casting = true;
    }
    public virtual void PlayControlled(int sourceID,bool play)
    {
        b_casting = play;
        if (play)
        {
            this.StartSingleCoroutine(0, TIEnumerators.Tick(OnBlast, F_Tick));
            PlaySFX(sourceID, int.MaxValue);
        }
        else
        {
            this.StopSingleCoroutine(0);
            f_TimeCheck = Time.time + 5f;
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
        hitEntity.TryHit(F_Damage);
    }
}
