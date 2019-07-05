using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileSplit : SFXProjectile {
    protected override void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        base.OnHitTarget(hit, hitCheck);
        for (int i = 0; i < 4; i++)
        {
            Vector3 splitDirection = transform.forward.RotateDirection(Vector3.up, i * 90);
            ObjectManager.SpawnSFX<SFXProjectile>(m_ProjectileInfo.m_relativeSFX1, hit.point, Vector3.up).Play(I_SourceID, splitDirection,hit.point+ splitDirection*10,m_ProjectileInfo.GetSplitInfo());
        }
    }
}
