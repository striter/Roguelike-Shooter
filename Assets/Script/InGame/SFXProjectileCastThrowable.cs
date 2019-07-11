using System.Collections;
using System.Collections.Generic;
using TPhysics;
using UnityEngine;
using GameSetting;
public class SFXProjectileCastThrowable : SFXProjectileCastTrigger {
    public float F_DirectionPitchAngle = 45;
    protected Vector3 v3_RotateEuler;
    protected Vector3 v3_RotateDirection;
    protected override PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition)
    {
        v3_RotateEuler = Quaternion.LookRotation( direction).eulerAngles;
        v3_RotateDirection= new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        return new ThrowablePhysicsSimulator(transform.position,direction,transform.right,targetPosition, F_DirectionPitchAngle, F_Speed);
    }
    protected override Quaternion GetRotation(Vector3 direction) => Quaternion.Euler(v3_RotateEuler);
    protected override void Update()
    {
        base.Update();
        v3_RotateEuler += v3_RotateDirection * Time.deltaTime*100f;
    }
}
