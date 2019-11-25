﻿using System.Collections;
using System.Collections.Generic;
using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileTargetSwap : SFXProjectile {
    public int I_BounceCount = 2;
    public float I_SwapRange = 10;
    protected override bool B_StopPhysicsOnHit => false;
    protected override bool B_StopParticlesOnHit => false;
    protected override PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) =>new ReflectBouncePSimulator<HitCheckBase>(transform, transform.position, direction, Vector3.down, F_Speed, F_Height, F_Radius,I_BounceCount, OnSwapTarget, GameLayer.Mask.I_All, OnHitTargetBreak, CanHitTarget);
    protected override float F_PlayDuration(Vector3 startPos, Vector3 endPos) => 10f;
    enum_EntityFlag m_SourceFlag;
    public override void Play(DamageDeliverInfo deliverInfo, Vector3 direction, Vector3 targetPosition)
    {
        base.Play(deliverInfo, direction, targetPosition);
        m_SourceFlag = GameManager.Instance.GetEntity(deliverInfo.I_SourceID).m_Flag;
    }

    Vector3 OnSwapTarget(Vector3 inforward,Vector3 normal)
    {
        Vector3 direction = Vector3.Reflect(inforward,normal);
        direction.y = 0;
        return direction.normalized;
    }
}