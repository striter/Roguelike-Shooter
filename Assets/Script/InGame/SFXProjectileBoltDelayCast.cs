using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileBoltDelayCast : SFXProjectileBolt {
    protected override bool B_StopParticlesOnHit => false;
    public float F_DelayDuration;
    protected override void Play()
    {
        base.Play();
        if (F_DelayDuration < 0)
            Debug.LogError("Delay Duration Less Than 0");
    }
    protected override bool OnHitTargetCanPenetrate(RaycastHit hit, HitCheckBase entity)
    {
        bool canPenetrate = base.OnHitTargetCanPenetrate(hit, entity);
        SetLifeTime(F_DelayDuration);
        return canPenetrate;
    }
    public override void Stop()
    {
        base.Stop();
        GameObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetEquipmentSubIndex(I_SFXIndex), transform.position + F_Height * transform.forward, Vector3.up).Play(m_DamageInfo.m_detail);
        OnRecycle();
    }
}
