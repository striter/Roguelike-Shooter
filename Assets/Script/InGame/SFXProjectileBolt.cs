using GameSetting;
using UnityEngine;

public class SFXProjectileBolt : SFXProjectile {
    protected override bool B_RecycleOnHit => false;
    protected override void OnHitTarget(RaycastHit hit,HitCheckBase entity)
    {
        base.OnHitTarget(hit,entity);
        transform.SetParent(hit.collider.transform);
        if(m_Trail)
        m_Trail.enabled = false;
        SetLifeTime(GameConst.I_BoltLastTimeAfterHit);
        if(entity!=null&&entity.m_HitCheckType== enum_HitCheck.Entity)
            (entity as HitCheckEntity).AttachHitMark(this);
    }
}
