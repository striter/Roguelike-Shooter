using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileBoltDelayCast : SFXProjectileBolt {
    public float F_DelayDuration;
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (F_DelayDuration < 0)
            Debug.LogError("Delay Duration Less Than 0");
    }
    protected override void OnHitTarget(RaycastHit hit, HitCheckBase entity)
    {
        base.OnHitTarget(hit, entity);
        f_TimeCheck = Time.time+ F_DelayDuration;
    }
    protected override void OnRecycle()
    {
        base.OnRecycle();
        ObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetEquipmentSubIndex(I_SFXIndex), transform.position+F_Height*transform.forward, Vector3.up).Play(I_SourceID, m_DamageInfo.m_BuffApply);
    }
}
