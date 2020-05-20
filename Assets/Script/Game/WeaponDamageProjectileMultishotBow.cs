using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class WeaponDamageProjectileMultishotBow : WeaponDamageProjectile {
    public int m_ExtraShotAdditiveStoreSuccessful=2;
    protected override void OnStoreTrigger(float animDuration, float storeTimeLeft)
    {
        bool successful = storeTimeLeft == 0;
        FireProjectile(successful, m_BaseSFXWeaponIndex, animDuration, successful ? m_ExtraShotAdditiveStoreSuccessful : 0);
    }
}
