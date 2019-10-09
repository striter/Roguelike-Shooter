using TPhysics;
using UnityEngine;
using GameSetting;
public class SFXProjectileCastThrowable : SFXProjectileCastTrigger {
    public float F_DirectionPitchAngle = 45;
    public bool B_RandomRotation=false;
    public bool B_Bounce = false;
    [Range(0,180)]
    public float F_BounceHitAngleMax = 0;
    [Range(0, 1)]
    public float F_BounceSpeedMultiply = 1;
    public float F_CastTimeAfterHit = 10f;
    protected override float F_Duration(Vector3 startPos, Vector3 endPos) => Vector3.Distance(startPos,endPos)/F_Speed+ F_CastTimeAfterHit;
    protected override Vector3 v3_castPoint => transform.position;
    protected override PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition)=>new ParacurveSimulator<HitCheckBase>(transform,transform.position,targetPosition, F_DirectionPitchAngle, F_Speed,F_Height,F_Radius,B_RandomRotation, GameLayer.Mask.I_All, B_Bounce,  F_BounceHitAngleMax, F_BounceSpeedMultiply, OnHitTargetBreak,CanHitTarget);
}
