using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile
{
    public bool B_DealHitDamage = false;
    protected virtual Vector3 v3_castPoint=> transform.position + transform.forward * F_Height;
    protected bool m_CastTriggered = false;
    protected override void OnPlay()
    {
        base.OnPlay();
        m_CastTriggered = false;
    }
    protected override void OnStop()
    {
        base.OnStop();
        if(!m_CastTriggered)
            OnCastTrigger(v3_castPoint);
        OnRecycle();
    }

    protected override void OnHitEntityDealtDamage(HitCheckEntity _entity)
    {
        if (!B_DealHitDamage)
            return;
        base.OnHitEntityDealtDamage(_entity);
    }
    protected override bool OnHitTargetPenetrate(HitCheckBase hitCheck)
    {
        OnCastTrigger(v3_castPoint);
        return base.OnHitTargetPenetrate(hitCheck);
    }

    protected virtual void OnCastTrigger(Vector3 point)
    {
        m_CastTriggered = true;
        GameObjectManager.SpawnSFXWeapon<SFXCast>(GameExpression.GetWeaponSubIndex(m_Identity),point , Vector3.up).Play(m_DamageInfo);
    }
}
