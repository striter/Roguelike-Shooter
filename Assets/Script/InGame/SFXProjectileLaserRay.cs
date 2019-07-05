using GameSetting;
using UnityEngine;

public class SFXProjectileLaserRay : SFXProjectile {
    float f_rayCheck;
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, SProjectileInfo projectileInfo, float duration)
    {
        base.Play(sourceID, direction, targetPosition, projectileInfo, duration);
        B_SimulatePhysics = false;
        f_rayCheck = 0f;
    }

    protected override void Update()
    {
        f_rayCheck += Time.deltaTime;
        if (!B_SimulatePhysics&&f_rayCheck >= GameConst.F_LaserRayStartPause)
            B_SimulatePhysics = true;

        base.Update();
    }
}
