using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileDestroyableSpreadDelay : SFXProjectileDestroyableSpread,ISingleCoroutine {
    public float F_DelayDuration;
    public int I_DelayIndicator;
    protected override PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition) => null;
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, DamageBuffInfo buffInfo)
    {
        base.Play(sourceID, direction, targetPosition, buffInfo);
        targetPosition = EnviormentManager.NavMeshPosition(targetPosition) + Vector3.up * .5f;
        transform.localScale = Vector3.zero;
        B_SimulatePhysics = false;
        ObjectManager.SpawnCommonIndicator(I_DelayIndicator, targetPosition, Vector3.up).Play(sourceID, F_DelayDuration);
        this.StartSingleCoroutine(1, TIEnumerators.PauseDel(F_DelayDuration, () => {
            transform.localScale = Vector3.one;
            transform.position = targetPosition;
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
            B_SimulatePhysics = true;
        }));
    }
    public void OnDisable()
    {
        this.StopSingleCoroutine(1);
    }
}
