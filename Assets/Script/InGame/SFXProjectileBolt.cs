using GameSetting;
using UnityEngine;

public class SFXProjectileBolt : SFXProjectile {
    protected override bool B_StopPhysicsOnHit => true;
    protected override bool OnHitTargetCanPenetrate(RaycastHit hit,HitCheckBase entity)
    {
        if(m_Trail)
         m_Trail.enabled = false;
        AttachTo(hit.collider.transform);
        SetLifeTime(GameConst.I_BoltLastTimeAfterHit);
        return base.OnHitTargetCanPenetrate(hit, entity);
    }
}
