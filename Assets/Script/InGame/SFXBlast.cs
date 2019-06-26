using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SFXBlast : SFXBase {
    ParticleSystem[] m_Particles;
    HitCheckDetect m_Detect;
    protected float f_damage;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        m_Detect = new HitCheckDetect(OnBlastStatic,OnBlastDynamic,OnBlastEntity,OnBlastError);
    }
    public void Play(int sourceID, float damage)
    {
        base.Play(sourceID,3);      //Temporaty Test
        f_damage = damage;
        TCommon.Traversal(m_Particles, (ParticleSystem particle) => { particle.Play(); });
        Collider[] collider = BlastCheck(); 
        for (int i = 0; i < collider.Length; i++)
        {
            m_Detect.DoDetect(collider[i]);
        }
    }
    protected virtual Collider[] BlastCheck()
    {
        Debug.LogError("Override This Please");
        return null;
    }
    protected virtual void OnBlastEntity(HitCheckEntity hitEntity)
    {
        Debug.LogError("Override This Please");
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
