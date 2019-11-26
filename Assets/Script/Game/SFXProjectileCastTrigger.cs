using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile
{
    protected bool b_trigger = false;
    protected virtual Vector3 v3_castPoint=> transform.position + transform.forward * F_Height;
    protected override void OnPlay()
    {
        base.OnPlay();
        b_trigger = false;
    }
    protected override void OnStop()
    {
        base.OnStop();
        OnCastTrigger(v3_castPoint);
        OnRecycle();
    }

    protected override bool OnHitTargetPenetrate(HitCheckBase hitCheck)
    {
        OnCastTrigger(v3_castPoint);
        return base.OnHitTargetPenetrate(hitCheck);
    }

    protected virtual void OnCastTrigger(Vector3 point)
    {
        if (b_trigger)
            return;
        GameObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetEquipmentSubIndex(m_Identity),point , Vector3.up).Play(m_DamageInfo.m_detail);
        b_trigger = true;
    }
}
