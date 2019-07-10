using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPhysics
{
    public abstract class PhysicsSimulator
    {
        protected Vector3 m_startPos;
        protected Vector3 m_LastPos;
        public Vector3 m_Direction { get; protected set; }
        public float m_simulateTime { get; protected set; }
        public abstract Vector3 Simulate(float deltaTime);
        public abstract Vector3 GetSimulatedPosition(float elapsedTime);
        public abstract Vector3 Simulate(float fixedTime, out Vector3 lastPosition) ;
    }
    public class AccelerationSimulator:PhysicsSimulator
    {
        protected Vector3 m_HorizontalDirection, m_VerticalDirection;
        float m_horizontalSpeed;
        float m_horizontalAcceleration;
        public AccelerationSimulator(Vector3 startPos, Vector3 horizontalDirection, Vector3 verticalDirection, float horizontalSpeed, float horizontalAcceleration)
        {
            m_simulateTime = 0f;
            m_startPos = startPos;
            m_LastPos = startPos;
            m_Direction = horizontalDirection;
            m_VerticalDirection = verticalDirection;
            m_horizontalSpeed = horizontalSpeed;
            m_horizontalAcceleration = horizontalAcceleration;
        }
        public override Vector3 Simulate(float timeElapsed)
        {
            Vector3 simulatedPosition = GetSimulatedPosition( timeElapsed);
            m_LastPos = simulatedPosition;
            return simulatedPosition;
        }
        public override Vector3 Simulate(float fixedTime, out Vector3 lastPosition)
        {
            Vector3 simulatedPosition = GetSimulatedPosition(m_simulateTime);
            lastPosition = m_LastPos;
            m_LastPos = simulatedPosition;
            m_simulateTime += fixedTime;
            return simulatedPosition;
        }
        public override Vector3 GetSimulatedPosition(float elapsedTime)
        {
            Vector3 horizontalShift = Vector3.zero;
            if (!(m_horizontalSpeed > 0 && m_horizontalAcceleration < 0))
            {
                float aboveZeroTime = m_horizontalSpeed / Mathf.Abs(m_horizontalAcceleration);

                horizontalShift += m_HorizontalDirection * Expressions.AccelerationSpeedShift(m_horizontalSpeed, m_horizontalAcceleration, elapsedTime > aboveZeroTime ? aboveZeroTime : elapsedTime);
            }

            Vector3 targetPos = m_startPos + horizontalShift ;
            return targetPos;
        }
    }
    public static class Expressions
    {
        public static float AccelerationSpeedShift(float speed, float acceleration, float elapsedTime)        //All M/S  s=vt+a*t^2/2?
        {
            return SpeedShift(speed,elapsedTime) + acceleration* Mathf.Pow(elapsedTime , 2)/2;
        }
        public static float GetAcceleration(float startSpeed, float distance,float duration)
        {
            return (distance - SpeedShift(startSpeed, duration)) / Mathf.Pow(duration, 2);
        }
        public static float SpeedShift(float speed, float elapsedTime)      //M/s s=vt
        {
            return speed * elapsedTime;
        }
    }
}