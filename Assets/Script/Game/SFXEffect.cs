using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXEffect : SFXParticles
{
    public enum_EffectAttach E_AttachTo;
    public bool B_IsMeshGlow;
    public void Play(EntityCharacterBase entity)
    {
        base.PlayControlled(entity.m_EntityID);
        Transform attachTo=null;
        switch (E_AttachTo)
        {
            case enum_EffectAttach.Feet:
                attachTo = entity.transform;
                break;
            case enum_EffectAttach.Head:
                attachTo = entity.tf_Head;
                break;
            case enum_EffectAttach.WeaponModel:
                attachTo = entity.tf_WeaponModel;
                break;
        }
        if (attachTo == null)
            Debug.LogError("Invalid Attach Found Of:" + E_AttachTo + "," + entity.name);

        transform.SetPositionAndRotation(attachTo.position,attachTo.rotation);
        AttachTo(entity.transform);

        if (!B_IsMeshGlow)
            return;

        MeshRenderer targetMesh = attachTo.GetComponent<MeshRenderer>();
        if (targetMesh == null)
            Debug.Log("Null Mesh Found!");
        m_Particle.m_Particles.Traversal((ParticleSystem particle) =>
        {
            ParticleSystem.ShapeModule shape = particle.shape;
            shape.meshRenderer = targetMesh;
        });
    }
}
