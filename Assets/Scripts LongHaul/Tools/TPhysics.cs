using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPhysics
{
    public abstract class PhysicsSimulator
    {
        protected Vector3 m_startPos;
        public Vector3 m_Position { get; protected set; }
        public Quaternion m_Rotation { get; protected set; }
        protected Vector3 m_Direction;
        public float m_simulateTime { get; protected set; }
        public abstract void Simulate(float deltaTime);
        public abstract Vector3 GetSimulatedPosition(float elapsedTime);
    }
    public class PhysicsSimulatorCapsule : PhysicsSimulator
    {
        protected float m_castHeight, m_castRadius;
        protected int m_hitLayer;
        protected Action<RaycastHit[]> OnTargetHit;
        public PhysicsSimulatorCapsule(Vector3 _startPos,Vector3 _direction,  CapsuleCollider _collider, int _hitLayer, Action<RaycastHit[]> _onTargetHit)
        {
            m_simulateTime = 0f;
            m_startPos = _startPos;
            m_Position = _startPos;
            m_Direction = _direction;
            m_castHeight = _collider.height;
            m_castRadius = _collider.radius;
            m_hitLayer = _hitLayer;
            OnTargetHit = _onTargetHit;
        }
        public override void Simulate(float deltaTime)
        {
            m_simulateTime += deltaTime;
            Vector3 simulatedPosition = GetSimulatedPosition(m_simulateTime);
            m_Rotation = Quaternion.LookRotation (simulatedPosition - m_Position);
            float distance = Vector3.Distance(m_Position, simulatedPosition);
            distance = distance > m_castHeight ? distance : m_castHeight;
            OnTargetHitted(Physics.SphereCastAll(new Ray(m_Position, m_Direction), m_castRadius, distance, m_hitLayer));
            m_Position = simulatedPosition;
        }
        public override Vector3 GetSimulatedPosition(float elapsedTime)
        {
            Debug.Log("Override This Please");
            return Vector3.zero;
        }
        public virtual void OnTargetHitted(RaycastHit[] hitTargets)
        {
            if (hitTargets.Length > 0) OnTargetHit(hitTargets);
        }
    }
    public class AccelerationSimulator: PhysicsSimulatorCapsule
    {
        protected Vector3 m_HorizontalDirection, m_VerticalDirection;
        protected float m_horizontalSpeed, m_horizontalAcceleration;
        public AccelerationSimulator(Vector3 _startPos, Vector3 _horizontalDirection, Vector3 _verticalDirection, float _horizontalSpeed, float _horizontalAcceleration, CapsuleCollider _collider, int _hitLayer, Action<RaycastHit[]> _onTargetHit) : base(_startPos, _horizontalDirection, _collider,_hitLayer,_onTargetHit)
        {
            m_HorizontalDirection = _horizontalDirection;
            m_VerticalDirection = _verticalDirection;
            m_horizontalSpeed = _horizontalSpeed;
            m_horizontalAcceleration = _horizontalAcceleration;
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