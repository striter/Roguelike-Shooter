using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileCastDetonate : SFXProjectileCastTrigger {
    protected override bool B_RecycleOnHit => false;
    protected override bool B_DisablePhysicsOnHit => true;
    protected override PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition)=>new ProjectilePhysicsSimulator(transform.position, Vector3.down, Vector3.down, F_Speed,m_Collider,GameLayer.Physics.I_All,OnPhysicsCasted);
    protected override void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        base.OnHitTarget(hit, hitCheck);
        transform.position = hit.point;
        transform.rotation = Quaternion.LookRotation(hit.normal);
        transform.SetParent(hit.collider.transform);
        m_Trail.enabled = false;
        f_TimeCheck = Time.time + ObjectManager.GetSFX<SFXCast>(I_CastIndex).F_DelayDuration;
        OnCastTrigger();
    }
}
