using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileDestroyableSpreadDelay : SFXProjectileDestroyableSpread {
    public float F_DelayDuration;
    protected override float F_PlayDelay => F_DelayDuration;
    protected override PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) => null;
    public override void Play( DamageDeliverInfo buffInfo, Vector3 direction, Vector3 targetPosition)
    {
        targetPosition = LevelManager.NavMeshPosition(targetPosition) + Vector3.up * .5f;
        transform.position = targetPosition;
        base.Play(buffInfo, direction, targetPosition);
    }
}
