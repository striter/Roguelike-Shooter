using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXBlast : SFXBase {
    ParticleSystem[] m_Particles;
    HitCheckDetect m_Detect;
    float f_damage;
    public override void Init(enum_SFX type)
    {
        base.Init(type);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        m_Detect = new HitCheckDetect(OnBlastStatic,OnBlastDynamic,OnBlastEntity,OnBlastError);
    }
    public void Play(int sourceID, float damage, float radius)
    {
        base.Play(sourceID,3);      //Temporaty Test
        f_damage = damage;
        transform.localScale = Vector3.one * (radius*2);
        TCommon.TraversalArray(m_Particles, (ParticleSystem particle) => { particle.Play(); });
        Collider[] collider = Physics.OverlapSphere(transform.position,radius,GameLayer.Physics.I_EntityOnly);
        for (int i = 0; i < collider.Length; i++)
        {
            m_Detect.DoDetect(collider[i]);
        }
    }
    protected virtual void OnBlastEntity(HitCheckEntity entity)
    {
        if (entity.I_AttacherID != I_SourceID)
            entity.TryHit(GameExpression.F_RocketBlastDamage(f_damage, Vector3.Distance(transform.position, entity.transform.position)));
    }
    protected virtual void OnBlastStatic()
    {

    }
    protected virtual void OnBlastDynamic()
    {

    }
    protected virtual void OnBlastError()
    {

    }
}
