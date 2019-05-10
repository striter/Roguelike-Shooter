using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXBlast : SFXBase {
    ParticleSystem[] m_Particles;
    public override void Init(enum_SFX type)
    {
        base.Init(type);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
    }
    public void Play(int sourceID, float damage, float radius)
    {
        base.Play(sourceID,3);
        transform.localScale = Vector3.one * (radius*2);
        TCommon.TraversalArray(m_Particles, (ParticleSystem particle) => { particle.Play(); });
        Collider[] collider = Physics.OverlapSphere(transform.position,radius,GameLayer.Physics.I_EntityOnly);
        for (int i = 0; i < collider.Length; i++)
        {
            Debug.Log(collider[i].gameObject);
            if (collider[i].gameObject.layer == GameLayer.I_Entity)
            {
                HitCheckEntity hitCheck = collider[i].GetComponent<HitCheckEntity>();
                if (hitCheck.I_AttacherID != I_SourceID)
                    hitCheck.TryHit(GameExpression.F_RocketBlastDamage(damage ,Vector3.Distance(transform.position,hitCheck.transform.position)));
            }
        }
    }
}
