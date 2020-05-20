using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class WeaponItemHealPotion : WeaponItemBase {
    public float F_BaseHealAmount = 50f;
    public float F_HealAmountAdditivePerEnhance = 10f;
    public int I_ParticleIndex;
    public float GetHealDamage() => F_BaseHealAmount + F_HealAmountAdditivePerEnhance *m_EnhanceLevel;

    protected override void OnKeyAnim()
    {
        base.OnKeyAnim();
        m_Attacher.m_HitCheck.TryHit(new DamageInfo(m_Attacher.m_EntityID).SetDamage(-GetHealDamage(), enum_DamageType.Health));
        GameObjectManager.SpawnParticles(I_ParticleIndex, m_Attacher.transform.position, Vector3.up).PlayControlled(m_Attacher.m_EntityID).AttachTo(m_Attacher.transform);
    }
}
