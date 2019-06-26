using GameSetting;
using UnityEngine;

public class SFXProjectileBlastMeteor : SFXProjectileBlastTrigger {
    protected override void Play(int sourceID, int impactSFXIndex,int blastIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        OnPlayPreset(damage, impactSFXIndex,blastIndex);
        B_SimulatePhysics = true;
        m_Damage = damage;
        float radius = Vector3.Distance(transform.position, destination);
        Vector3 startPos = destination + Vector3.up * 20 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * radius;
        m_Simulator = new ProjectilePhysicsSimulator(startPos, destination - startPos, Vector3.down, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration);
        transform.position = startPos;
        Play(sourceID, duration);
    }
}
