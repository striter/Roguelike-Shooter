using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXBulletLaserRay : SFXBullet {
    float f_rayCheck;
    public override void Play(int sourceID, float damage, Vector3 direction, Vector2 bulletSpeed, float duration = -1)
    {
        base.Play(sourceID, damage, direction, bulletSpeed, GameConst.I_LaserMaxLastTime);
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
