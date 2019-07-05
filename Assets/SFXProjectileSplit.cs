using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileSplit : SFXProjectile {
    public int I_SplitProjectileIndex;
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (I_SplitProjectileIndex <= 0)
            Debug.LogError("Split Index Less Or Equals 0");
    }
    protected override void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        base.OnHitTarget(hit, hitCheck);
        for (int i = 0; i < 4; i++)
        {
            Vector3 splitDirection = transform.forward.RotateDirection(Vector3.up, i * 90);
            ObjectManager.SpawnSFX<SFXProjectile>(I_SplitProjectileIndex, hit.point, Vector3.up).Play(I_SourceID, splitDirection,hit.point+ splitDirection*10);
        }
    }
}
