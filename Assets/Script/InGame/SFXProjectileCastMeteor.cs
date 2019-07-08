﻿using GameSetting;
using UnityEngine;

public class SFXProjectileCastMeteor : SFXProjectileCastTrigger
{
    public int I_IndicatorIndex;
    public float F_SpreadRadius=5;
    public float F_StartHeight = 20;
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (I_IndicatorIndex <= 0)
            Debug.LogError("Indicator Index Less Or Equals 0");
    }
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, float duration)
    {
        OnPlayPreset();
        float distance = Vector3.Distance(transform.position, targetPosition);
        Vector3 startPos = targetPosition + Vector3.up * F_StartHeight + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))*F_SpreadRadius;
        Vector3 spreadDirection = (targetPosition - startPos).normalized;
        m_Simulator = new ProjectilePhysicsSimulator(startPos, spreadDirection, Vector3.down, F_Speed);
        transform.position = startPos;
        ObjectManager.SpawnSFX<SFXIndicator>(I_IndicatorIndex, startPos, spreadDirection).Play(sourceID, startPos, spreadDirection, F_Speed, 1.5f);
        PlaySFX(sourceID, Vector3.Distance(startPos,targetPosition)/F_Speed);
    }
}
