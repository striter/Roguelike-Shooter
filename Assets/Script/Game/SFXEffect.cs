using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXEffect : SFXParticles
{
    public enum_EffectType E_AttachTo;
    public void Play(EntityCharacterBase entity)
    {
        base.PlayControlled(entity.m_EntityID);
        switch (E_AttachTo)
        {
            case enum_EffectType.FeetAttach:
                transform.SetPositionAndRotation(entity.transform.position, entity.transform.rotation);
                break;
            case enum_EffectType.HeadAttach:
                transform.SetPositionAndRotation(entity.tf_Head.position, entity.tf_Head.rotation);
                break;
            case enum_EffectType.WeaponMesh:
                m_Particle.m_Particles.Traversal((ParticleSystem particle) =>
                {
                    ParticleSystem.ShapeModule shape = particle.shape;
                    shape.shapeType = ParticleSystemShapeType.MeshRenderer;
                    shape.meshRenderer = entity.m_WeaponSkin;
                });
                break;
        }
        AttachTo(entity.transform);
    }
}
