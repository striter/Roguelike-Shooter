using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class SFXProjectile : SFXBase {
    public enum enum_SubSFXType
    {
        Invalid=-1,
        Muzzle=1,
        Projectile=2,
        Impact=3,
        HitMark=4,
    }
    public class SubSFX
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
    protected Dictionary<enum_SubSFXType, SubSFX> m_subSFXDic = new Dictionary<enum_SubSFXType, SubSFX>();
    protected float m_Damage;
    protected ProjectilePhysicsSimulator m_Simulator;
    HitCheckDetect m_Detect;
    TrailRenderer m_Trail;
    float f_sphereWidth;
    public bool B_SimulatePhysics { get; protected set; }
    public override void Init(enum_SFX type)
    {
        base.Init(type);
        TCommon.TraversalEnum((enum_SubSFXType sfx) => { m_subSFXDic.Add(sfx, new SubSFX(transform.Find(sfx.ToString()))); });
        m_Detect = new HitCheckDetect(OnHitStatic,OnHitDynamic,OnHitEntity,OnHitError);
        m_Trail = m_subSFXDic[enum_SubSFXType.Projectile].transform.GetComponentInChildren<TrailRenderer>();
        f_sphereWidth = m_Trail.startWidth / 2;
    }
    public void PlayWeapon(int sourceID,Vector3 direction,Vector3 targetPosition,SWeapon weaponInfo, float duration= -1)
    {
        Play(sourceID,direction,targetPosition,weaponInfo.m_Damage,weaponInfo.m_HorizontalSpeed,weaponInfo.m_HorizontalDistance,0,weaponInfo.m_VerticalAcceleration, duration == -1 ? GameConst.I_NormalProjectileLastTime : duration);
    }
    public void PlayBarrage(int sourceID, Vector3 direction, Vector3 targetPosition, SBarrage barrageInfo,float duration =-1)
    {
        Play(sourceID,direction, targetPosition, barrageInfo.m_ProjectileDamage, barrageInfo.m_ProjectileSpeed,200,0,0, duration == -1 ? GameConst.I_BarrageProjectileMaxLastTime : duration);
    }
    protected virtual void Play(int sourceID, Vector3 direction, Vector3 destination, float damage, float horiSpeed,float horiDistance,float vertiSpeed,float vertiAcceleration, float duration)
    {
        OnPlayPreset(damage);
        m_Simulator = new ProjectilePhysicsSimulator(m_subSFXDic[enum_SubSFXType.Projectile].transform.position, direction, Vector3.down, horiSpeed, horiDistance,vertiSpeed, vertiAcceleration);
        Play(sourceID, duration);
    }
    protected void OnPlayPreset(float damage)
    {
        m_Damage = damage;
        B_SimulatePhysics = true;
        TCommon.TraversalEnum((enum_SubSFXType sfx) => { m_subSFXDic[sfx].transform.localPosition = Vector3.zero; m_subSFXDic[sfx].SetPlay(false); });
        m_subSFXDic[enum_SubSFXType.Projectile].SetPlay(true);
        m_subSFXDic[enum_SubSFXType.Muzzle].SetPlay(true);
        m_Trail.Clear();
    }
    protected override void Update()
    {
        base.Update();
        if (B_SimulatePhysics)
        {
            Vector3 prePosition;
            Vector3 curPosition = m_Simulator.Simulate(Time.deltaTime, out prePosition);

            Vector3 castDirection = prePosition == curPosition ? m_Simulator.m_HorizontalDirection : curPosition - prePosition;
            m_subSFXDic[enum_SubSFXType.Projectile].transform.rotation = Quaternion.LookRotation(castDirection);
            RaycastHit rh_info;
            if (Physics.SphereCast(new Ray(prePosition, castDirection), f_sphereWidth, out rh_info, Vector3.Distance(prePosition, curPosition), GameLayer.Physics.I_All))
                OnHitTarget(rh_info);
            else
                m_subSFXDic[enum_SubSFXType.Projectile].transform.position = curPosition;
        }
    }

    protected virtual void OnHitTarget(RaycastHit hitInfo)
    {
        B_SimulatePhysics = false;
        m_subSFXDic[enum_SubSFXType.Projectile].SetPlay(false);

        m_subSFXDic[enum_SubSFXType.Impact].transform.position = hitInfo.point;
        m_subSFXDic[enum_SubSFXType.Impact].transform.rotation = Quaternion.LookRotation(hitInfo.normal);
        m_subSFXDic[enum_SubSFXType.Impact].SetPlay(true);
        m_subSFXDic[enum_SubSFXType.HitMark].transform.position = hitInfo.point;
        m_subSFXDic[enum_SubSFXType.HitMark].transform.rotation = Quaternion.LookRotation(hitInfo.normal);
        m_subSFXDic[enum_SubSFXType.HitMark].SetPlay(true);

        f_TimeCheck += 10f;
        m_Detect.DoDetect(hitInfo.collider);
    }

    protected virtual void OnHitEntity(HitCheckEntity hitEntity)
    {
        if (GameManager.B_CanHitTarget(hitEntity, I_SourceID))
        {
            hitEntity.AttachTransform(m_subSFXDic[enum_SubSFXType.HitMark].transform);
            hitEntity.TryHit(m_Damage);
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
