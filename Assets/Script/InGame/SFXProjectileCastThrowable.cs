using TPhysics;
using UnityEngine;
using GameSetting;
public class SFXProjectileCastThrowable : SFXProjectileCastTrigger {
    public float F_DirectionPitchAngle = 45;
    public bool B_RandomRotation=false;
    public bool B_Bounce = false;
    protected override PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition)=>new ThrowablePhysicsSimulator(transform,transform.position,direction,transform.right,targetPosition, F_DirectionPitchAngle, F_Speed,F_Height,F_Radius,B_RandomRotation, GameLayer.Physics.I_All, B_Bounce, GameLayer.I_Static, OnPhysicsCasted);
}
