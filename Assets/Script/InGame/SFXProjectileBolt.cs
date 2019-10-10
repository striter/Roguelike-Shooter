using GameSetting;
using UnityEngine;

public class SFXProjectileBolt : SFXProjectile {
    protected override bool B_RecycleOnHit => false;
    protected override bool OnHitTargetCanPenetrate(RaycastHit hit,HitCheckBase entity)
    {
        transform.SetParent(hit.collider.transform);
        if(m_Trail)
        m_Trail.enabled = false;
        SetLifeTime(GameConst.I_BoltLastTimeAfterHit);
        if(entity!=null&&entity.m_HitCheckType== enum_HitCheck.Entity)
            (entity as HitCheckEntity).AttachHitMark(this);
        return base.OnHitTargetCanPenetrate(hit, entity);
    }
}
