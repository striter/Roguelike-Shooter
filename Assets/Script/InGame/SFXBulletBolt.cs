using GameSetting;
using UnityEngine;

public class SFXBulletBolt : SFXBullet {

    public override void Play(int sourceID, Vector3 direction, SWeapon weaponInfo, float duration = -1)
    {
        base.Play(sourceID, direction, weaponInfo, GameConst.I_BoltMaxLastTime);
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
