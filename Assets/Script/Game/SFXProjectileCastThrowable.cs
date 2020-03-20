using TPhysics;
using UnityEngine;
using GameSetting;
public class SFXProjectileCastThrowable : SFXProjectileCastTrigger {
    public float F_DirectionPitchAngle = 45;
    public bool B_RandomRotation=false;
    public bool B_Bounce = false;
    [Range(0, 1)]
    public float F_BounceSpeedMultiply = 1;
    public float F_CastTimeAfterHit = 10f;
    protected override float F_PlayDuration(Vector3 startPos, Vector3 endPos) => Vector3.Distance(startPos,endPos)/F_Speed+ F_CastTimeAfterHit;
    protected override Vector3 v3_castPoint => transform.position;
    protected override PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition)=>new ParacurveBouncePSimulator<HitCheckBase>(transform,transform.position,targetPosition, F_DirectionPitchAngle, F_Speed,F_Height,F_Radius,B_RandomRotation, GameLayer.Mask.I_ProjectileMask, B_Bounce, F_BounceSpeedMultiply, OnHitTargetBreak,CanHitTarget);
}
