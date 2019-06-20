﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class SFXBullet : SFXBase {
    enum enum_SubSFXType
    {
        Invalid=-1,
        Muzzle=1,
        Projectile=2,
        Impact=3,
        HitMark=4,
    }
    class SubSFX
    {
        public Transform transform { get; private set; }
        public ParticleSystem[] m_Particles { get; private set; }
        public SubSFX(Transform _transform)
        {
            transform = _transform;
            m_Particles = _transform.GetComponentsInChildren<ParticleSystem>();
            SetPlay(false);
        }
        public void SetPlay(bool play)
        {
            transform.SetActivate(play);
            if (play)
                m_Particles.Traversal((ParticleSystem particle) => { particle.Play(); });
        }
    }
    Dictionary<enum_SubSFXType, SubSFX> m_subSFXDic = new Dictionary<enum_SubSFXType, SubSFX>();
    protected float m_bulletDamage;
    BulletPhysicsSimulator m_Simulator;
    HitCheckDetect m_Detect;
    TrailRenderer m_Trail;
    Vector3 m_Direction;
    float f_sphereWidth;
    public bool B_SimulatePhysics { get; protected set; }
    public override void Init(enum_SFX type)
    {
        base.Init(type);
        TCommon.TraversalEnum((enum_SubSFXType sfx) => { m_subSFXDic.Add(sfx, new SubSFX(transform.Find(sfx.ToString()))); });
        m_Detect = new HitCheckDetect(OnHitStatic,OnHitDynamic,OnHitEntity,OnHitError);
        m_Trail = m_subSFXDic[ enum_SubSFXType.Projectile].transform.GetComponentInChildren<TrailRenderer>();
        f_sphereWidth = m_Trail.startWidth / 2;
    }
    public virtual void PlayWeapon(int sourceID,Vector3 direction,SWeapon weaponInfo, float duration= -1)
    {
        Play(sourceID,direction,weaponInfo.m_Damage,weaponInfo.m_HorizontalSpeed,weaponInfo.m_HorizontalDistance,0,weaponInfo.m_VerticalAcceleration, duration == -1 ? GameConst.I_NormalBulletLastTime : duration);
    }
    public void PlayBarrage(int sourceID, Vector3 direction,SBarrage barrageInfo,float duration =-1)
    {
        Play(sourceID,direction, barrageInfo.m_BulletDamage, barrageInfo.m_BulletSpeed,200,0,0, duration == -1 ? GameConst.I_BarrageBulletMaxLastTime : duration);
    }
    protected void Play(int sourceID, Vector3 direction, float damage, float horiSpeed,float horiDistance,float vertiSpeed,float vertiAcceleration, float duration)
    {
        B_SimulatePhysics = true;
        TCommon.TraversalEnum((enum_SubSFXType sfx) => { m_subSFXDic[sfx].transform.localPosition = Vector3.zero; m_subSFXDic[sfx].SetPlay(false); });
        m_subSFXDic[enum_SubSFXType.Projectile].SetPlay(true);
        m_subSFXDic[enum_SubSFXType.Muzzle].SetPlay(true);
        m_Trail.Clear();
        m_subSFXDic[enum_SubSFXType.Projectile].transform.localPosition = Vector3.zero;
        m_subSFXDic[enum_SubSFXType.Muzzle].transform.localPosition = Vector3.zero;
        m_bulletDamage = damage;
        m_Direction = direction;
        m_Simulator = new BulletPhysicsSimulator(m_subSFXDic[enum_SubSFXType.Projectile].transform.position, m_Direction, Vector3.down, horiSpeed, horiDistance,vertiSpeed, vertiAcceleration);
        base.Play(sourceID, duration);
    }
    protected override void Update()
    {
        base.Update();
        if (B_SimulatePhysics)
        {
            Vector3 prePosition;
            Vector3 curPosition = m_Simulator.Simulate(Time.deltaTime, out prePosition);

            Vector3 castDirection = prePosition == curPosition ? m_Direction : curPosition - prePosition;
            m_subSFXDic[enum_SubSFXType.Projectile].transform.rotation = Quaternion.LookRotation(castDirection);
            RaycastHit rh_info;
            if (Physics.SphereCast(new Ray(prePosition, castDirection), f_sphereWidth, out rh_info, Vector3.Distance(prePosition, curPosition), GameLayer.Physics.I_All))
            {
                B_SimulatePhysics = false;
                m_subSFXDic[enum_SubSFXType.Projectile].SetPlay(false);

                m_subSFXDic[enum_SubSFXType.Impact].transform.position = rh_info.point;
                m_subSFXDic[enum_SubSFXType.Impact].transform.rotation = Quaternion.LookRotation(rh_info.normal);
                m_subSFXDic[enum_SubSFXType.Impact].SetPlay(true);
                m_subSFXDic[enum_SubSFXType.HitMark].transform.position = rh_info.point;
                m_subSFXDic[enum_SubSFXType.HitMark].transform.rotation = Quaternion.LookRotation(rh_info.normal);
                m_subSFXDic[enum_SubSFXType.HitMark].SetPlay(true);

                f_TimeCheck += 10f;
                m_Detect.DoDetect(rh_info.collider);
            }
            else
            {
                m_subSFXDic[enum_SubSFXType.Projectile].transform.position = curPosition;
            }
        }
    }
    
    protected virtual void OnHitEntity(HitCheckEntity hitEntity)
    {
        if (GameManager.B_CanHitTarget(hitEntity, I_SourceID))
        {
            hitEntity.AttachTransform(m_subSFXDic[enum_SubSFXType.HitMark].transform);
            hitEntity.TryHit(m_bulletDamage);
        }
    }
    protected virtual void OnHitDynamic(HitCheckDynamic hitDynamic)
    {
    }
    protected virtual void OnHitStatic(HitCheckStatic hitStatic)
    {
    }
    protected virtual void OnHitError()
    {
    }
    protected override void OnPlayFinished()
    {
        m_subSFXDic[enum_SubSFXType.HitMark].transform.SetParent(transform);
        base.OnPlayFinished();
    }
}
