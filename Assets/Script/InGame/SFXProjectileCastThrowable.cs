using System.Collections;
using System.Collections.Generic;
using TPhysics;
using UnityEngine;
using GameSetting;
public class SFXProjectileCastThrowable : SFXProjectileCastTrigger {
    public float F_DirectionPitchAngle = 45;
    protected override PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition)
    {
        return new ThrowablePhysicsSimulator(transform.position,direction,transform.right,targetPosition, F_DirectionPitchAngle, F_Speed,m_Collider,GameLayer.Physics.I_All, GameLayer.I_Static, OnPhysicsCasted);
    }
}
