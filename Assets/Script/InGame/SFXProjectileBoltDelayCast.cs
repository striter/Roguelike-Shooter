using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileBoltDelayCast : SFXProjectileBolt {
    Transform tf_CastPoint;
    public float F_DelayDuration;
    float f_castCheck;
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        tf_CastPoint = transform.Find("CastPoint");
    }
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (F_DelayDuration < 0)
            Debug.LogError("Delay Duration Less Than 0");
    }
    protected override void OnHitTarget(RaycastHit hit, HitCheckBase entity)
    {
        base.OnHitTarget(hit, entity);
        f_castCheck =  F_DelayDuration;
    }
    protected override void Update()
    {
        base.Update();
        if (B_SimulatePhysics)
            return;
        f_castCheck -= Time.deltaTime;
        if(f_castCheck<=0)
        {
            ObjectManager.SpawnDamageSource<SFXCast>(GameExpression.GetEnermyWeaponSubIndex(I_SFXIndex), tf_CastPoint.position, Vector3.up).Play(I_SourceID, m_DamageInfo.m_BuffApply);
            OnPlayFinished();
        }
    }
}
