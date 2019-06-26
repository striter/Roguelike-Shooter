using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class SFXProjectile : SFXBase {
    protected float m_Damage;
    protected ProjectilePhysicsSimulator m_Simulator;
    HitCheckDetect m_Detect;
    TrailRenderer m_Trail;
    float f_sphereWidth;
    protected int i_impactSFXIndex;
    public bool B_SimulatePhysics { get; protected set; }
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Detect = new HitCheckDetect(OnHitStatic,OnHitDynamic,OnHitEntity,OnHitError);
        m_Trail = transform.GetComponentInChildren<TrailRenderer>();
        f_sphereWidth = m_Trail.startWidth / 2;
    }
    public void PlayWeapon(int sourceID, int impactSFXIndex, Vector3 direction,Vector3 targetPosition,SWeapon weaponInfo, float duration= -1)
    {
        Play(sourceID, impactSFXIndex, direction,targetPosition,weaponInfo.m_Damage,weaponInfo.m_HorizontalSpeed,weaponInfo.m_HorizontalDistance,0,weaponInfo.m_VerticalAcceleration, duration == -1 ? GameConst.I_NormalProjectileLastTime : duration);
    }
    public void PlayBarrage(int sourceID, int impactSFXIndex, Vector3 direction, Vector3 targetPosition, SBarrage barrageInfo,float duration =-1)
    {
        Play(sourceID, impactSFXIndex, direction, targetPosition, barrageInfo.m_ProjectileDamage, barrageInfo.m_ProjectileSpeed,200,0,0, duration == -1 ? GameConst.I_BarrageProjectileMaxLastTime : duration);
    }
    protected virtual void Play(int sourceID,int impactSFXIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed,float horiDistance,float vertiSpeed,float vertiAcceleration, float duration)
    {
        OnPlayPreset(damage, impactSFXIndex);
        m_Simulator = new ProjectilePhysicsSimulator(transform.position, direction, Vector3.down, horiSpeed, horiDistance,vertiSpeed, vertiAcceleration);
        Play(sourceID, duration);
    }
    protected void OnPlayPreset(float damage,int impactSFXIndex)
    {
        m_Damage = damage;
        i_impactSFXIndex = impactSFXIndex;
        B_SimulatePhysics = true;
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
           transform.rotation = Quaternion.LookRotation(castDirection);
            RaycastHit rh_info;
            if (Physics.SphereCast(new Ray(prePosition, castDirection), f_sphereWidth, out rh_info, Vector3.Distance(prePosition, curPosition), GameLayer.Physics.I_All))
                OnHitTarget(rh_info);
            else
                transform.position = curPosition;
        }
    }

    protected virtual void OnHitTarget(RaycastHit hitInfo)
    {
        B_SimulatePhysics = false;
        ObjectManager.SpawnSFX<SFXParticles>(i_impactSFXIndex, hitInfo.point,hitInfo.normal,hitInfo.transform).Play(I_SourceID);
        m_Detect.DoDetect(hitInfo.collider);
    }

    protected virtual void OnHitEntity(HitCheckEntity hitEntity)
    {
        if (GameManager.B_CanHitTarget(hitEntity, I_SourceID))
        {
            hitEntity.TryHit(m_Damage);
        }
        OnPlayFinished();
    }
    protected virtual void OnHitDynamic(HitCheckDynamic hitDynamic)
    {
        OnPlayFinished();
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
