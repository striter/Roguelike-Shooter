using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class SFXBullet : SFXBase {
    protected float m_bulletDamage;
    AccelerationSimulator m_Simulator;
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
        m_Simulator = new AccelerationSimulator(transform.position, direction, Vector3.down, horizontalSpeed, -horizontalDrag, 0, verticalAcceleration,false);
        B_SimulatePhysics = true;
        m_Trail.Clear();
        base.Play(sourceID,duration==-1? GameConst.I_NormalBulletLastTime:duration);
    }
    Vector3 simulatedLookDirection;
    private void FixedUpdate()
    {
        if (B_SimulatePhysics)
        {
            transform.position = m_Simulator.Simulate(Time.fixedDeltaTime,out simulatedLookDirection);
            transform.rotation =Quaternion.LookRotation( simulatedLookDirection);
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
