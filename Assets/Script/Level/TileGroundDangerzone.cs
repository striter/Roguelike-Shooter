using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using LevelSetting;

public class TileGroundDangerzone : TileGroundBase {
    TSpecialClasses.AnimationControlBase m_Animation;
    TimeCounter m_TimerReset=new TimeCounter(GameConst.F_DangerzoneResetDuration);
    TimeCounter m_TimerCheck = new TimeCounter(.3f);
    public override void OnGenerateItem(ChunkTileData _data, System.Random random)
    {
        base.OnGenerateItem(_data, random);
        m_Animation = new TSpecialClasses.AnimationControlBase(GetComponent<Animation>());
    }
    RaycastHit m_Hit;
    private void Update()
    {
        m_TimerReset.Tick(Time.deltaTime);
        m_TimerCheck.Tick(Time.deltaTime);
        if (m_TimerReset.m_Timing)
            return;

        if (m_TimerCheck.m_Timing)
            return;
        m_TimerCheck.Reset();

        if (!Physics.Raycast(transform.position+LevelConst.V3_TileUnitCenterOffset, Vector3.up, out m_Hit, 3f, GameLayer.Mask.I_Entity))
            return;

        if (!GameManager.Instance.m_Battling)
            return;

        HitCheckEntity hitCheck = m_Hit.collider.DetectEntity();
        if (hitCheck.m_Attacher.m_ControllType != enum_EntityController.Player)
            return;

        hitCheck.TryHit(new DamageInfo(GameConst.I_DangerzoneDamage, enum_DamageType.Basic, DamageDeliverInfo.Default()));
        m_Animation.Play(true);
        m_TimerReset.Reset();
    } 

}
