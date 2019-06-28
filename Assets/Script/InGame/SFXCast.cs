using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SFXCast : SFXBase {
    ParticleSystem[] m_Particles;
    HitCheckDetect m_Detect;
    protected float f_damage;
    protected float f_blastTime;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        f_blastTime = .5f;      //Test  Change To 0 If Needed
        m_Particles.Traversal((ParticleSystem particle)=> {
            particle.Stop();
            if (particle.main.duration > f_blastTime)
                f_blastTime = particle.main.duration;
        });
        m_Detect = new HitCheckDetect(OnBlastStatic,OnBlastDynamic,OnBlastEntity,OnBlastError);
    }
    public virtual void Play(int sourceID, float damage)
    {
        PlaySFX(sourceID,f_blastTime); 
        f_damage = damage;
        OnBlast();
    }
    protected virtual void OnBlast()
    {
        TCommon.Traversal(m_Particles, (ParticleSystem particle) => { particle.Play(); });
        Collider[] collider = OnBlastCheck();
        for (int i = 0; i < collider.Length; i++)
        {
            m_Detect.DoDetect(collider[i]);
        }
    }
    protected virtual Collider[] OnBlastCheck()
    {
        Debug.LogError("Override This Please");
        return null;
    }
    protected void OnBlastEntity(HitCheckEntity hitEntity)
    {
        if (GameManager.B_CanHitTarget(hitEntity, I_SourceID))
            OnDamageEntity(hitEntity);
    }
    protected virtual void OnDamageEntity(HitCheckEntity hitEntity)
    {
        hitEntity.TryHit(f_damage);
    }
    protected virtual void OnBlastStatic(HitCheckStatic hitStatic)
    {

    }
    protected virtual void OnBlastDynamic(HitCheckDynamic hitDynamic)
    {

    }
    protected virtual void OnBlastError()
    {

    }
}
