using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TPhysics;

public class SFXBullet : SFXBase {
    protected float m_bulletDamage;
    AccelerationSimulator m_Simulator;
    HitCheckDetect m_Detect;
    TrailRenderer m_Trail;
    Vector3 m_Direction;
    float f_sphereWidth;
    public bool B_SimulatePhysics { get; protected set; }
    public override void Init(enum_SFX type)
    {
        base.Init(type);
        m_Detect = new HitCheckDetect(OnHitStatic,OnHitDynamic,OnHitEntity,OnHitError);
        m_Trail = GetComponent<TrailRenderer>();
        f_sphereWidth = m_Trail.startWidth / 2;
    }
    public virtual  void Play(int sourceID, float damage,Vector3 direction,float horizontalSpeed,float horizontalDrag, float verticalAcceleration, float duration= -1)
    {
        m_bulletDamage = damage;
        m_Direction = direction;
        m_Simulator = new AccelerationSimulator(transform.position, m_Direction, Vector3.down, horizontalSpeed, -horizontalDrag, 0, verticalAcceleration,false);
        B_SimulatePhysics = true;
        m_Trail.Clear();
        base.Play(sourceID,duration==-1? GameConst.I_NormalBulletLastTime:duration);
    }
    protected override void Update()
    {
        base.Update();
        if (B_SimulatePhysics)
        {
            Vector3 prePosition;
            Vector3 curPosition = m_Simulator.Simulate(Time.deltaTime, out prePosition);

            Vector3 castDirection = prePosition == curPosition ? m_Direction : curPosition - prePosition;
            transform.rotation = Quaternion.LookRotation(castDirection);
            RaycastHit rh_info;
            if (Physics.SphereCast(new Ray(prePosition, castDirection), f_sphereWidth, out rh_info, Vector3.Distance(prePosition, curPosition), GameLayer.Physics.I_All))
            {
                transform.position = rh_info.point;
                m_Detect.DoDetect(rh_info.collider);
            }
            else
            {
                transform.position = curPosition;
            }
        }
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
