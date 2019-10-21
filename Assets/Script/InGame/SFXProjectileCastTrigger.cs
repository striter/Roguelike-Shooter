using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile
{
    protected override bool B_DealDamage => false;
    protected bool b_trigger = false;
    protected virtual Vector3 v3_castPoint=> transform.position + transform.forward * F_Height;
    protected override void Play()
    {
        base.Play();
        b_trigger = false;
    }
    public override void OnStop()
    {
        base.OnStop();
        OnCastTrigger(v3_castPoint);
        OnRecycle();
    }
    protected override bool OnHitTargetCanPenetrate(RaycastHit hit, HitCheckBase hitCheck)
    {
        OnCastTrigger(hit.point==Vector3.zero?v3_castPoint:hit.point);
        return false;
    }
    protected virtual void OnCastTrigger(Vector3 point)
    {
        if (b_trigger)
            return;
        
        GameObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetEquipmentSubIndex(I_SFXIndex),point , Vector3.up).Play(m_DamageInfo.m_detail);
        b_trigger = true;
    }
}
