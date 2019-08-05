using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile
{
    protected override bool B_DealDamage => false;
    protected bool b_trigger = false;
    protected virtual Vector3 v3_castPoint=> transform.position + transform.forward * F_Height;
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        b_trigger = false;
    }
    protected override void OnRecycle()
    {
        base.OnRecycle();
        OnCastTrigger(v3_castPoint);
    }
    protected override void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        OnCastTrigger(hit.point);
    }
    protected void OnCastTrigger(Vector3 point)
    {
        if (b_trigger)
            return;
        ObjectManager.SpawnDamageSource<SFXCast>(GameExpression.GetEnermyWeaponSubIndex(I_SFXIndex),point , Vector3.up).Play(I_SourceID, m_DamageInfo.m_BuffApply);
        b_trigger = true;
    }
}
