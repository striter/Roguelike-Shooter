using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXProjectileBeam : SFXProjectile {
    protected override bool B_RecycleOnHit => false;
    protected override bool B_DisablePhysicsOnHit => false;
    protected override bool B_HitMultiple => true;
    public int I_BeamLength = 10;
    protected override float F_Duration(Vector3 startPos, Vector3 endPos) => I_BeamLength / F_Speed;

    protected override void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        base.OnHitTarget(hit, hitCheck);
        switch(hitCheck.m_HitCheckType)
        {
            case enum_HitCheck.Static:
            case enum_HitCheck.Dynamic:
                    OnRecycle();
                break;
        }
    }
}
