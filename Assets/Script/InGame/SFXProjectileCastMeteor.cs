using GameSetting;
using UnityEngine;

public class SFXProjectileCastMeteor : SFXProjectileCastTrigger {
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, float duration)
    {
        OnPlayPreset();
        float distance = Vector3.Distance(transform.position, targetPosition);
        Vector3 startPos = targetPosition + Vector3.up * 20 + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * distance;
        Vector3 spreadDirection = (targetPosition - startPos).normalized;
        m_Simulator = new ProjectilePhysicsSimulator(startPos, spreadDirection, Vector3.down, F_Speed);
        transform.position = startPos;
        ObjectManager.SpawnSFX<SFXIndicator>(I_IndicatorIndex, startPos, spreadDirection).Play(sourceID, startPos, spreadDirection, F_Speed, 1.5f);
        PlaySFX(sourceID, Vector3.Distance(startPos,targetPosition)/F_Speed);
    }
}
