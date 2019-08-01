using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile
{
    protected override bool B_DealDamage => false;
    protected override float F_Duration(Vector3 startPos, Vector3 endPos) => Vector3.Distance(startPos, endPos) / F_Speed;
    protected bool b_trigger = false;
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        b_trigger = false;
    }
    protected override void OnPlayFinished()
    {
        base.OnPlayFinished();
        OnCastTrigger(transform.position + transform.forward * F_Height);
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
