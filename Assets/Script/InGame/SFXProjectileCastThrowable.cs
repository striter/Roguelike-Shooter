﻿using System.Collections;
using System.Collections.Generic;
using TPhysics;
using UnityEngine;
using GameSetting;
public class SFXProjectileCastThrowable : SFXProjectileCastTrigger {
    public float F_DirectionPitchAngle = 45;
    public bool B_RandomRotation=false;
    public bool B_Bounce = false;
    protected override PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition)
    {
        return new ThrowablePhysicsSimulator(transform.position,direction,transform.right,targetPosition, F_DirectionPitchAngle, F_Speed,m_Collider,B_RandomRotation, GameLayer.Physics.I_All, B_Bounce, GameLayer.I_Static, OnPhysicsCasted);
    }
}
