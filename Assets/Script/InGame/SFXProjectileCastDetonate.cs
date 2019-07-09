using GameSetting;
using UnityEngine;

public class SFXProjectileCastDetonate : SFXProjectileCastTrigger {
    protected override bool B_RecycleOnHit => false;
    public int I_IndicatorIndex;
    public float F_DetonateDuration = 5f;
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, float duration)
    {
        OnPlayPreset();
        m_Simulator = new ProjectilePhysicsSimulator(transform.position, Vector3.down, Vector3.down, F_Speed);
        PlaySFX(sourceID,F_DetonateDuration+5f);
    }
    protected override void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        base.OnHitTarget(hit, hitCheck);
        transform.position = hit.point;
        transform.rotation = Quaternion.LookRotation(hit.normal);
        transform.SetParent(hit.collider.transform);
        m_Trail.enabled = false;
        f_TimeCheck = Time.time + F_DetonateDuration;
        if (I_IndicatorIndex > 0)
            ObjectManager.SpawnSFX<SFXIndicator>(I_IndicatorIndex, transform.position, Vector3.up).PlayDuration(I_SourceID, transform.position, Vector3.up, F_DetonateDuration);
    }
}
