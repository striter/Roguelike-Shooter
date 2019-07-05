using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileSplit : SFXProjectile {
    public int I_SplitProjectileIndex;
    [Range(0,360)]
    public float F_SplitRange;
    public int I_SplitCount;
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, float duration = -1)
    {
        base.Play(sourceID, direction, targetPosition, Vector3.Distance(transform.position,targetPosition)/F_Speed);
    }
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (I_SplitProjectileIndex <= 0)
            Debug.LogError("Split Index Less Or Equals 0");
        if (I_SplitCount <= 0)
            Debug.LogError("Fan Count Less Of Equals 0");
    }
    protected override void OnPlayFinished()
    {
        base.OnPlayFinished();
        OnSplit();
    }
    void OnSplit()
    {
        float angleEach = F_SplitRange / I_SplitCount;
        float startAngle = -(I_SplitCount - 1) * angleEach / 2f;
        for (int i = 0; i < I_SplitCount; i++)
        {
            Vector3 splitDirection = transform.forward.RotateDirection(Vector3.up, startAngle + i * angleEach);
            ObjectManager.SpawnSFX<SFXProjectile>(I_SplitProjectileIndex,transform.position, Vector3.up).Play(I_SourceID, splitDirection, transform.position + splitDirection * 10);
        }
    }
}
