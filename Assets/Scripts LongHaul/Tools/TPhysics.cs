using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPhysics
{
    public abstract class PhysicsSimulator<T> where T:MonoBehaviour
    {
        protected Vector3 m_startPos;
        protected Vector3 m_Direction;
        public float m_simulateTime { get; protected set; }
        public abstract void Simulate(float deltaTime);
        public abstract Vector3 GetSimulatedPosition(float elapsedTime);
    }
    public class PhysicsSimulatorCapsule<T> : PhysicsSimulator<T> where T:MonoBehaviour
    {
        protected Transform transform;
        protected float m_castHeight, m_castRadius;
        protected int m_hitLayer;
        protected Func<RaycastHit, T,bool> OnTargetHitBreak;
        protected Predicate<T> CanHitTarget;
        public PhysicsSimulatorCapsule(Transform _transform, Vector3 _startPos,Vector3 _direction, float _height,float _radius, int _hitLayer, Func<RaycastHit, T,bool> _onTargetHit,Predicate<T> _CanHitTarget)
        {
            m_simulateTime = 0f;
            transform = _transform;
            m_startPos = _startPos;
            transform.position = _startPos;
            m_Direction = _direction;
            m_castHeight = _height;
            m_castRadius = _radius;
            m_hitLayer = _hitLayer;
            OnTargetHitBreak = _onTargetHit;
            CanHitTarget = _CanHitTarget;
        }
        public override void Simulate(float deltaTime)
        {
            if (deltaTime == 0) return;
            m_simulateTime += deltaTime;
            Vector3 previousPosition = transform.position;
            transform.position = GetSimulatedPosition(m_simulateTime);
            Vector3 castDirection =(transform.position-previousPosition).normalized;
            transform.rotation = Quaternion.LookRotation(castDirection);
            float distance = Vector3.Distance(previousPosition, transform.position);
            distance = distance > m_castHeight ? distance : m_castHeight;
            OnTargetsHit(Physics.SphereCastAll(new Ray(previousPosition, castDirection), m_castRadius, distance, m_hitLayer));
        }
        public override Vector3 GetSimulatedPosition(float elapsedTime)
        {
            Debug.Log("Override This Please");
            return Vector3.zero;
        }
        public void OnTargetsHit(RaycastHit[] castHits)
        {
            for (int i = 0; i < castHits.Length; i++)
            {
                T temp = castHits[i].collider.GetComponent<T>();
                if (CanHitTarget(temp)&&OnTargetHit(castHits[i],temp))
                        break;
            }   
        }
        public virtual bool OnTargetHit(RaycastHit hit, T template)
        {
            return OnTargetHitBreak(hit,template);
        }
    }
    public class AccelerationSimulator<T>: PhysicsSimulatorCapsule<T> where T:MonoBehaviour
    {
        protected Vector3 m_HorizontalDirection, m_VerticalDirection;
        protected float m_horizontalSpeed, m_horizontalAcceleration;
        public AccelerationSimulator(Transform _transform, Vector3 _startPos, Vector3 _horizontalDirection, Vector3 _verticalDirection, float _horizontalSpeed, float _horizontalAcceleration, float _height, float _radius, int _hitLayer, Func<RaycastHit, T, bool> _onTargetHit, Predicate<T> _CanHitTarget) : base(_transform,_startPos, _horizontalDirection, _height,_radius,_hitLayer,_onTargetHit,_CanHitTarget)
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

    public class ParacurveSimulator<T> : PhysicsSimulatorCapsule<T> where T:MonoBehaviour
    {
        float f_speed;
        float f_vertiAcceleration;
        float f_bounceHitMaxAnlge;
        bool b_randomRotation;
        bool b_bounceOnHit;
        float f_bounceSpeedMultiply;
        bool B_SpeedOff => f_speed <= 0;
        protected Vector3 v3_RotateEuler;
        protected Vector3 v3_RotateDirection;
        public ParacurveSimulator(Transform _transform, Vector3 _startPos, Vector3 _endPos, float _angle, float _horiSpeed, float _height, float _radius, bool randomRotation, int _hitLayer, bool bounce, float _bounceHitAngle, float _bounceSpeedMultiply, Func<RaycastHit, T,bool> _onBounceHit, Predicate<T> _OnBounceCheck) : base(_transform, _startPos, Vector3.zero, _height, _radius, _hitLayer, _onBounceHit, _OnBounceCheck)
        {
            Vector3 horiDirection = TCommon.GetXZLookDirection(_startPos, _endPos);
            Vector3 horiRight = horiDirection.RotateDirection(Vector3.up, 90);
            m_Direction = horiDirection.RotateDirection(horiRight, -_angle);
            f_speed = _horiSpeed / Mathf.Cos(_angle * Mathf.Deg2Rad);
            float horiDistance = Vector3.Distance(_startPos, _endPos);
            float duration = horiDistance / _horiSpeed;
            float vertiDistance = Mathf.Tan(_angle * Mathf.Deg2Rad) * horiDistance;
            f_vertiAcceleration = Expressions.GetAcceleration(0, vertiDistance, duration);
            b_randomRotation = randomRotation;
            b_bounceOnHit = bounce;
            f_bounceHitMaxAnlge = _bounceHitAngle;
            f_bounceSpeedMultiply = _bounceSpeedMultiply;
            v3_RotateEuler = Quaternion.LookRotation(m_Direction).eulerAngles;
            v3_RotateDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        }

        public override void Simulate(float deltaTime)
        {
            if (B_SpeedOff)
                return;

            base.Simulate(deltaTime);

            if (b_randomRotation)
            {
                v3_RotateEuler += v3_RotateDirection * deltaTime * 300f;
                transform.rotation = Quaternion.Euler(v3_RotateEuler);
            }
        }
        public override bool OnTargetHit(RaycastHit hit,T template)
        {
            if(!b_bounceOnHit)
                return base.OnTargetHit(hit, template);

            Vector3 bounceDirection = hit.point == Vector3.zero ? Vector3.up : hit.normal;
            float bounceAngle = TCommon.GetAngle(bounceDirection, Vector3.up, Vector3.up);
            if (bounceAngle > 15)
                m_Direction = bounceDirection;
            m_startPos = transform.position;
            m_simulateTime = 0;

            f_speed -= .1f;
            f_speed *= f_bounceSpeedMultiply;
            if (f_speed < m_castRadius)
                f_speed = 0;

            if (f_bounceHitMaxAnlge != 0 && bounceAngle < f_bounceHitMaxAnlge)      //OnBounceHitTarget
               return base.OnTargetHit(hit,template);

            return true;
        }

        public override Vector3 GetSimulatedPosition(float elapsedTime) => m_startPos + m_Direction * f_speed * elapsedTime + Vector3.down * f_vertiAcceleration * elapsedTime * elapsedTime;
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