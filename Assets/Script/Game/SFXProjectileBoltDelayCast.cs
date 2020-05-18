using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileBoltDelayCast : SFXProjectileBolt {
    protected override bool B_StopOnPenetradeFail => false;
    public float F_DelayDuration;
    protected override bool OnHitTargetPenetrate(HitCheckBase hitCheck)
    {
        bool canHit = base.OnHitTargetPenetrate(hitCheck);
        SetLifeTime(F_DelayDuration);
        return canHit;
    }
    protected override void OnStop()
    {
        base.OnStop();
        GameObjectManager.SpawnSFXWeapon<SFXCast>(GameExpression.GetWeaponSubIndex(m_Identity), transform.position + F_Height * transform.forward, Vector3.up).Play(m_DamageInfo);
        OnRecycle();
    }
}
