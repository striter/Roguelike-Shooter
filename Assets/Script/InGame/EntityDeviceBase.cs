using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityDeviceBase : EntityCharacterBase {
    public override enum_EntityController m_Controller => enum_EntityController.Device; 
    EntityDetector m_Detect;
    protected List<EntityCharacterBase> m_DetectLink=new List<EntityCharacterBase>();
    ParticleSystem[] m_Particles;
    public override void Init(int _poolIndex)
    {
        base.Init(_poolIndex);
        m_Detect = transform.Find("EntityDetector").GetComponent<EntityDetector>();
        m_Detect.Init(OnEntityDetect);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
    }
    public override void OnActivate(enum_EntityFlag _flag, float startHealth = 0)
    {
        base.OnActivate(_flag, startHealth);
        m_Particles.Traversal((ParticleSystem particle) => { particle.Play(); });
    }
    protected override void OnDead()
    {
        base.OnDead();
        m_DetectLink.Clear();
        m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
    }

    void OnEntityDetect(HitCheckEntity entity, bool enter)
    {
        if (m_Health.b_IsDead)
            return;

        switch (entity.m_Attacher.m_Controller)
        {
            default:break;
            case enum_EntityController.Player:
            case enum_EntityController.AI:
                {
                    EntityCharacterBase target = entity.m_Attacher as EntityCharacterBase;
                    if (enter)
                        m_DetectLink.Add(target);
                    else
                        m_DetectLink.Remove(target);
                } 
                break;
        }
    }
}
