using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXProjectileBeam : SFXProjectile {
    protected override bool B_RecycleOnHit => false;
    protected override bool B_DisablePhysicsOnHit => false;
    public int I_BeamLength = 10;

    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, SProjectileInfo projectileInfo, float duration)
    {
        base.Play(sourceID, direction, targetPosition, projectileInfo, I_BeamLength/projectileInfo.m_HorizontalSpeed);
    }

    protected override void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        base.OnHitTarget(hit, hitCheck);
        switch(hitCheck.m_HitCheckType)
        {
            case GameSetting.enum_HitCheck.Static:
            case GameSetting.enum_HitCheck.Dynamic:
                OnPlayFinished();
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,transform.position+transform.forward* I_BeamLength);
    }
    #endif
}
