﻿using System.Collections;
using System.Collections.Generic;
using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileTargetSwap : SFXProjectile {
    public int I_BounceCount = 2;
    protected override bool B_StopOnPenetradeFail => false;
    protected override PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) =>new ReflectBouncePSimulator<HitCheckBase>(transform, transform.position, direction, Vector3.down, F_Speed, F_Height, F_Radius,I_BounceCount, OnSwapTarget, GameLayer.Mask.I_ProjectileMask, OnHitTargetBreak, CanHitTarget);
    protected override float F_PlayDuration(Vector3 startPos, Vector3 endPos) => 5f;

    Vector3 OnSwapTarget(Vector3 inforward,Vector3 normal)
    {
        m_EntityHitted.Clear();
        SetLifeTime(5f);
        Vector3 direction = Vector3.Reflect(inforward,normal);
        direction.y = 0;
        return direction.normalized;
    }
}
