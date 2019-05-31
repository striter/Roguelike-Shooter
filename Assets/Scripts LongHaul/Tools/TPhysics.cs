using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPhysics
{
    public static class Expressions
    {
        public static float AccelerationShift(float speed, float acceleration, float elapsedTime)        //All M/S  s=vt+a*t^2/2?
        {
            return speed * elapsedTime + acceleration* Mathf.Pow(elapsedTime , 2)/2;
        }
    }
}