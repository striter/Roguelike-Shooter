using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class WeaponItemHealPotion : WeaponItemBase {
    public int I_ParticleIndex;
    protected override void OnKeyAnim()
    {
        base.OnKeyAnim();
        m_Attacher.m_HitCheck.TryHit(GetWeaponDamageInfo(-m_BaseDamage, enum_DamageType.Health));
        GameObjectManager.SpawnParticles(I_ParticleIndex, m_Attacher.transform.position, Vector3.up).PlayControlled(m_Attacher.m_EntityID).AttachTo(m_Attacher.transform);
    }
}
