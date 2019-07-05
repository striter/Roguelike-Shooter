using GameSetting;
using UnityEngine;

public class SFXProjectileCastMeteor : SFXProjectileCastTrigger {
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, SProjectileInfo projectileInfo, float duration)
    {
        OnPlayPreset(projectileInfo);
        float distance = Vector3.Distance(transform.position, targetPosition);
        Vector3 startPos = targetPosition + Vector3.up * 20 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * distance;
        Vector3 spreadDirection = (targetPosition - startPos).normalized;
        m_Simulator = new ProjectilePhysicsSimulator(startPos, spreadDirection, Vector3.down, projectileInfo.m_HorizontalSpeed, projectileInfo.m_HorizontalDistance, projectileInfo.m_VerticalSpeed, projectileInfo.m_VerticalAcceleration);
        transform.position = startPos;
        ObjectManager.SpawnSFX<SFXIndicator>(m_ProjectileInfo.m_relativeSFX2, startPos, spreadDirection).Play(sourceID, startPos, spreadDirection, projectileInfo.m_HorizontalSpeed, 1.5f);
        PlaySFX(sourceID, Vector3.Distance(startPos,targetPosition)/projectileInfo.m_HorizontalSpeed);
    }
}
