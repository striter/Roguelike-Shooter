using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileBoltDelayCast : SFXProjectileBolt {
    protected override bool B_StopParticlesOnHit => false;
    public float F_DelayDuration;
    protected override bool OnHitTargetCanPenetrate(RaycastHit hit, HitCheckBase entity)
    {
        bool canPenetrate = base.OnHitTargetCanPenetrate(hit, entity);
        SetLifeTime(F_DelayDuration);
        return canPenetrate;
    }
    protected override void OnStop()
    {
        base.OnStop();
        GameObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetEquipmentSubIndex(m_Identity), transform.position + F_Height * transform.forward, Vector3.up).Play(m_DamageInfo.m_detail);
        OnRecycle();
    }


    protected override void EDITOR_DEBUG()
    {
        base.EDITOR_DEBUG();
        if (F_DelayDuration < 0)
            Debug.LogError("Delay Duration Less Than 0");
    }
}
