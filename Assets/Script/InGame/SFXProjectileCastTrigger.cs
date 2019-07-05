using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile {
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, SProjectileInfo projectileInfo, float duration)
    {
        base.Play(sourceID, direction, targetPosition, projectileInfo, Vector3.Distance(transform.position, targetPosition) / projectileInfo.m_HorizontalSpeed);
    }

    protected override void OnPlayFinished()
    {
        base.OnPlayFinished();

        if (m_ProjectileInfo.m_relativeSFX1 == -1)
            Debug.LogError("Error!Should Set Blast Index While Using This SFXProjectile:" + I_SFXIndex);

        ObjectManager.SpawnSFX<SFXCast>(m_ProjectileInfo.m_relativeSFX1, transform.position, Vector3.up).Play(I_SourceID, m_ProjectileInfo.m_damage);
    }
}
