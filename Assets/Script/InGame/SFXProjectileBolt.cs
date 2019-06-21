using GameSetting;
using UnityEngine;

public class SFXProjectileBolt : SFXProjectile {
    protected override void Play(int sourceID, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        base.Play(sourceID, direction, destination, damage, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration, GameConst.I_BoltMaxLastTime);
    }
    protected override void OnHitTarget(RaycastHit hitInfo)
    {
        base.OnHitTarget(hitInfo);
        m_subSFXDic[enum_SubSFXType.Projectile].SetPlay(true);
    }

    protected override void OnHitStatic(HitCheckStatic hitStatic)
    {
        transform.SetParent(hitStatic.transform);
    }

    protected override void OnHitEntity(HitCheckEntity entity)
    {
        entity.AttachTransform(transform);
        if (GameManager.B_CanHitTarget(entity, I_SourceID))
            entity.TryHit(m_Damage);
    }
}
