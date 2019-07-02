using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileBeam : SFXProjectile {
    protected override bool B_RecycleOnHit => false;
    protected override bool B_DisablePhysicsOnHit => false;
    protected override bool B_CanHitDynamicStatic => false;
    public int I_BeamLength = 10;
    protected override void Play(int sourceID, int impactSFXIndex, int blastSFXIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        base.Play(sourceID, impactSFXIndex, blastSFXIndex, direction, destination, damage, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration, I_BeamLength/horiSpeed);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,transform.position+transform.forward* I_BeamLength);
    }
    #endif
}
