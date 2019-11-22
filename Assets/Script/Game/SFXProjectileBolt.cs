﻿using GameSetting;
using UnityEngine;

public class SFXProjectileBolt : SFXProjectile {
    protected override bool B_StopPhysicsOnHit => true;
    protected override bool CheckCanPenetrate(HitCheckBase hitCheck)
    {
        AttachTo(hitCheck.transform);
        SetLifeTime(GameConst.I_BoltLastTimeAfterHit);
        return false;
    }
}
