using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageProjectileBow : WeaponDamageProjectile {
    protected override void OnStoreTrigger(float animDuration, float storeTimeLeft)
    {
        FireProjectile(m_BaseSFXWeaponIndex,GetWeaponDamageInfo(m_BaseDamage),animDuration);
    }
}
