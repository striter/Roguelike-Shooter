using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SFXCast : SFXBase {
    ParticleSystem[] m_Particles;
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
        hitEntity.TryHit(f_damage);
    }
}
