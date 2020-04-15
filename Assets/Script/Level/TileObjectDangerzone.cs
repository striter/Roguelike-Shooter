using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using LevelSetting;

public class TileObjectDangerzone : TileObjectBase {
    TSpecialClasses.AnimationClipControl m_Animation;
    TimerBase m_TimerReset=new TimerBase(GameConst.F_DangerzoneResetDuration);
    EntityDetector m_EntityDetector;
    public override void OnGenerateItem(ChunkTileData _data, System.Random random)
    {
        base.OnGenerateItem(_data, random);
        m_Animation = new TSpecialClasses.AnimationClipControl(GetComponent<Animation>());
        m_EntityDetector = GetComponentInChildren<EntityDetector>();
        m_EntityDetector.Init(OnInteractCheck);
    }

    List<HitCheckEntity> m_HitEntities = new List<HitCheckEntity>();
    void OnInteractCheck(HitCheckEntity hitCheck,bool enter)
    {
        if (enter)
            m_HitEntities.Add(hitCheck);
        else
            m_HitEntities.Remove(hitCheck);
    }

    RaycastHit m_Hit;
    private void Update()
    {
        m_TimerReset.Tick(Time.deltaTime);
        if (m_TimerReset.m_Timing)
            return;
        
        m_HitEntities.Traversal((HitCheckEntity entity)=>entity.TryHit(new DamageInfo(-1,GameConst.I_DangerzoneDamage, enum_DamageType.Basic)));
        m_Animation.Play(true);
        m_TimerReset.Replay();
    } 

}
