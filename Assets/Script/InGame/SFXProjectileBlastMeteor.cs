using GameSetting;
using UnityEngine;

public class SFXProjectileBlastMeteor : SFXProjectileBlastTrigger {
    protected override void Play(int sourceID, int impactSFXIndex, int blastIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        OnPlayPreset(damage, impactSFXIndex, blastIndex);
        float distance = Vector3.Distance(transform.position, destination);
        Vector3 startPos = destination + Vector3.up * 20 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * distance;
        Vector3 spreadDirection = (destination - startPos).normalized;
        m_Simulator = new ProjectilePhysicsSimulator(startPos, spreadDirection, Vector3.down, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration);
        transform.position = startPos;
        ObjectManager.SpawnSFX<SFXIndicator>(50001, startPos, spreadDirection).Play(sourceID,startPos,destination,horiSpeed,5);
        PlaySFX(sourceID, duration);
    }
}
