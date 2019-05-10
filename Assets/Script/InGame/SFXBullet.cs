using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class SFXBullet : SFXBase {
    protected float m_bulletDamage;
    Vector3 m_simulateGravity;
    Vector2 m_BulletSpeed;
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
    public virtual  void Play(int sourceID, float damage,Vector3 direction,Vector2 bulletSpeed,float duration= GameConst.I_BulletMaxLastTime)
    {
        m_bulletDamage = damage;
        m_simulateGravity = Vector3.zero;
        m_BulletSpeed = bulletSpeed;
        m_Direction = direction;
        B_SimulatePhysics = true;
        m_Trail.Clear();
        base.Play(sourceID,duration);
    }
    private void FixedUpdate()
    {
        if (B_SimulatePhysics)
        {
            m_simulateGravity += Time.fixedDeltaTime * m_BulletSpeed.y * Vector3.down;
            Vector3 simulateVector = m_Direction * m_BulletSpeed.x + m_simulateGravity;
            transform.rotation = Quaternion.LookRotation(simulateVector);
            transform.Translate(simulateVector * Time.fixedDeltaTime, Space.World);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        m_Detect.DoDetect(other);
    }

    protected virtual void OnHitEntity(HitCheckEntity entity)
    {
        if (GameExpression.B_CanHitTarget(entity,I_SourceID))
        {
            entity.TryHit(m_bulletDamage);
            OnPlayFinished();
        }
    }
    protected virtual void OnHitDynamic()
    {
        OnPlayFinished();
    }
    protected virtual void OnHitStatic()
    {
        OnPlayFinished();
    }
    protected virtual void OnHitError()
    {
        OnPlayFinished();
    }
}
