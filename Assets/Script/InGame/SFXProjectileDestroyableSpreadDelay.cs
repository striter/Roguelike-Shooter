using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileDestroyableSpreadDelay : SFXProjectileDestroyableSpread,ISingleCoroutine {
    public float F_DelayDuration;
    protected override PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) => null;
    public override void Play( DamageDeliverInfo buffInfo, Vector3 direction, Vector3 targetPosition)
    {
        targetPosition = LevelManager.NavMeshPosition(targetPosition) + Vector3.up * .5f;
        transform.localScale = Vector3.zero;
        this.StartSingleCoroutine(1, TIEnumerators.PauseDel(F_DelayDuration, () => {
            transform.localScale = Vector3.one;
            transform.position = targetPosition;
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
            base.Play(buffInfo, direction, targetPosition);
        }));
    }
    protected override void SpawnIndicator(Vector3 position, Vector3 direction, float duration)
    {
        duration = F_DelayDuration;
        base.SpawnIndicator(position, direction, duration);
    }
    public void OnDisable()
    {
        this.StopSingleCoroutine(1);
    }
}
