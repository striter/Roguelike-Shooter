using GameSetting;
using UnityEngine;

public class SFXProjectileBolt : SFXProjectile {
    protected override bool B_RecycleOnHit => false;
    protected override void OnHitStatic(HitCheckStatic hitStatic)
    {
        transform.SetParent(hitStatic.transform);
        f_TimeCheck = Time.time + GameConst.I_BoltLastTimeAfterHit;
    }

    protected override void OnHitEntity(HitCheckEntity entity)
    {
        f_TimeCheck = Time.time + GameConst.I_BoltLastTimeAfterHit;
        entity.AttachTransform(this);
        if (GameManager.B_CanHitTarget(entity, I_SourceID))
            entity.TryHit(m_Damage);
    }
    protected override void OnHitDynamic(HitCheckDynamic hitDynamic)
    {
        OnPlayFinished();
    }

    protected override void OnHitError()
    {
        OnPlayFinished();
    }
}
