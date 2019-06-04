using GameSetting;
using UnityEngine;

public class SFXBulletBolt : SFXBullet {

    public override void Play(int sourceID, float damage, Vector3 direction, float horizontalSpeed, float horizontalDrag, float verticalAcceleration, float duration = -1)
    {
        base.Play(sourceID, damage, direction, horizontalSpeed,horizontalDrag, verticalAcceleration, GameConst.I_BoltMaxLastTime);
    }
    protected override void OnHitStatic(HitCheckStatic hitStatic)
    {
        B_SimulatePhysics = false;
        transform.SetParent(hitStatic.transform);
    }

    protected override void OnHitEntity(HitCheckEntity entity)
    {
        B_SimulatePhysics = false;
        transform.SetParent(entity.transform);
        if (GameExpression.B_CanHitTarget(entity, I_SourceID))
            entity.TryHit(m_bulletDamage);
    }
}
