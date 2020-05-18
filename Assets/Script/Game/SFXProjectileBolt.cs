using GameSetting;
using UnityEngine;

public class SFXProjectileBolt : SFXProjectile {
    protected override bool OnHitTargetPenetrate(HitCheckBase hitCheck)
    {
        AttachTo(hitCheck.transform);
        SetLifeTime(GameConst.I_BoltLastTimeAfterHit);
        return false;
    }
}
