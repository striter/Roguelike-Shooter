using GameSetting;
using UnityEngine;

public class SFXProjectileBolt : SFXProjectile {
    protected override void Play(int sourceID, int impactSFXIndex,int blastIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        base.Play(sourceID, impactSFXIndex, blastIndex, direction, destination, damage, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration, GameConst.I_BoltMaxLastTime);
    }

    protected override void OnHitStatic(HitCheckStatic hitStatic)
    {
        transform.SetParent(hitStatic.transform);
    }

    protected override void OnHitEntity(HitCheckEntity entity)
    {
        entity.AttachTransform(this);
        if (GameManager.B_CanHitTarget(entity, I_SourceID))
            entity.TryHit(m_Damage);
    }
}
