using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileBlastMeteor : SFXProjectileBlastTrigger {
    protected override void Play(int sourceID, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        OnPlayPreset(damage);
        B_SimulatePhysics = true;
        m_Damage = damage;
        Vector3 startPos = transform.position + Vector3.up*20;
        m_Simulator = new ProjectilePhysicsSimulator(startPos, destination-startPos, Vector3.down, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration);
        Play(sourceID, duration);
    }
}
