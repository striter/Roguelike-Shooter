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
    public float F_AdditionalLasetTimeAfterHit = 10f;
    protected override float F_Duration(Vector3 startPos, Vector3 endPos) => base.F_Duration(startPos,endPos)+ F_AdditionalLasetTimeAfterHit;
    protected override PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition)=>new ParacurveSimulator(transform,transform.position,targetPosition, F_DirectionPitchAngle, F_Speed,F_Height,F_Radius,B_RandomRotation, GameLayer.Physics.I_All, B_Bounce,  F_BounceHitAngleMax, F_BounceSpeedMultiply, OnPhysicsCasted,p=>CanHitTarget(p.Detect()));
}
