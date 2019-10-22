﻿using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileSplit : SFXProjectile {
    public int I_SplitProjectileIndex;
    [Range(0,360)]
    public float F_SplitRange;
    public int I_SplitCount;
    protected override float F_PlayDuration(Vector3 startPos, Vector3 endPos) => Vector3.Distance(transform.position, endPos) / F_Speed;
    protected override void OnStop()
    {
        base.OnStop();
        OnSplit();
        OnRecycle();
    }
    void OnSplit()
    {
        float angleEach = F_SplitRange / I_SplitCount;
        float startAngle = -(I_SplitCount - 1) * angleEach / 2f;
        for (int i = 0; i < I_SplitCount; i++)
        {
            Vector3 splitDirection = transform.forward.RotateDirection(Vector3.up, startAngle + i * angleEach);
            GameObjectManager.SpawnEquipment<SFXProjectile>(I_SplitProjectileIndex,transform.position, Vector3.up).Play(m_DamageInfo.m_detail,splitDirection, transform.position + splitDirection * 10);
        }
    }

    protected override void EDITOR_DEBUG()
    {
        base.EDITOR_DEBUG();
        if (I_SplitProjectileIndex <= 0)
            Debug.LogError("Split Index Less Or Equals 0");
        if (I_SplitCount <= 0)
            Debug.LogError("Fan Count Less Of Equals 0");
    }
}
