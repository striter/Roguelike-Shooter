using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class SFXBullet : SFXBase {
    protected float m_bulletDamage;
    float m_horizontalSpeed;
    float m_horizontalDrag;
    float m_verticalSpeed;
    float m_verticalAcceleration;
    Vector3 m_Direction;
    HitCheckDetect m_Detect;
    TrailRenderer m_Trail;
    public bool B_SimulatePhysics { get; protected set; }
    public override void Init(enum_SFX type)
    {
        base.Init(type);
        m_Detect = new HitCheckDetect(OnHitStatic,OnHitDynamic,OnHitEntity,OnHitError);
        m_Trail = GetComponent<TrailRenderer>();
    }
    public virtual  void Play(int sourceID, float damage,Vector3 direction,float horizontalSpeed,float horizontalDrag, float verticalAcceleration, float duration= -1)
    {
        m_bulletDamage = damage;
        m_horizontalSpeed = horizontalSpeed;
        m_horizontalDrag = horizontalDrag;
        m_verticalSpeed = 0f;      //Vertical Speed Starts At 0 Only Add With Acceleration
        m_verticalAcceleration = verticalAcceleration;
        m_Direction = direction;
        B_SimulatePhysics = true;
        m_Trail.Clear();
        base.Play(sourceID,duration==-1? GameConst.I_NormalBulletLastTime:duration);
    }
    private void FixedUpdate()
    {
        if (B_SimulatePhysics)
        {
            Vector3 simulateVector = m_Direction * m_horizontalSpeed + Vector3.down*m_verticalSpeed;

            if(simulateVector!=Vector3.zero)
                 transform.rotation = Quaternion.LookRotation(simulateVector);

            transform.Translate(simulateVector * Time.fixedDeltaTime, Space.World);

            m_verticalSpeed += m_verticalAcceleration*Time.fixedDeltaTime;
            m_horizontalSpeed -= m_horizontalDrag * Time.fixedDeltaTime;

            if (m_horizontalSpeed <= 0f)
                m_horizontalSpeed = 0f;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        m_Detect.DoDetect(other);
    }

    protected virtual void OnHitEntity(HitCheckEntity hitEntity)
    {
        if (GameExpression.B_CanHitTarget(hitEntity, I_SourceID))
        {
            hitEntity.TryHit(m_bulletDamage);
            OnPlayFinished();
        }
    }
    protected virtual void OnHitDynamic(HitCheckDynamic hitDynamic)
    {
    //    OnPlayFinished();
    }
    protected virtual void OnHitStatic(HitCheckStatic hitStatic)
    {
        OnPlayFinished();
    }
    protected virtual void OnHitError()
    {
        OnPlayFinished();
    }
}
