using GameSetting;
using UnityEngine;

public class SFXProjectileLaserRay : SFXProjectile {
    float f_rayCheck;
    protected override void Play(int sourceID, int impactSFXIndex,int blastSFXIndex, int relativeIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        base.Play(sourceID, impactSFXIndex,blastSFXIndex,relativeIndex, direction, destination, damage, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration, duration);
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
