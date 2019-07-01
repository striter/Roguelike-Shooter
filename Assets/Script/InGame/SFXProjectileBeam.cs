using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileBeam : SFXProjectile {
    public int I_BeamLength = 10;
    List<Collider> m_TargetHit = new List<Collider>();
    protected override void Play(int sourceID, int impactSFXIndex, int blastSFXIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        base.Play(sourceID, impactSFXIndex, blastSFXIndex, direction, destination, damage, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration, I_BeamLength/horiSpeed);
    }
    protected override bool OnHitTarget(RaycastHit hitInfo)
    {
        if(!m_TargetHit.Contains(hitInfo.collider))
        {
            m_TargetHit.Add(hitInfo.collider);
            base.OnHitTarget(hitInfo);
        }
        
        return false;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,transform.position+transform.forward* I_BeamLength);
    }
#endif
}
