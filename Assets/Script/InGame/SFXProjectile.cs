using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(CapsuleCollider))]
public class SFXProjectile : SFXBase {
    protected float m_Damage;
    protected ProjectilePhysicsSimulator m_Simulator;
    HitCheckDetect m_Detect;
    protected CapsuleCollider m_Collider;
    TrailRenderer m_Trail;
    protected int i_impactSFXIndex,i_blastSFXIndex;
    protected HitCheckEntity m_hitEntity;
    public bool B_SimulatePhysics { get; protected set; }
    protected virtual bool B_RecycleOnHit => true;
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Detect = new HitCheckDetect(OnHitStatic,OnHitDynamic,OnHitEntity,OnHitError);
        m_Trail = transform.GetComponentInChildren<TrailRenderer>();
        m_Collider = GetComponent<CapsuleCollider>();
        m_Collider.enabled = false;
    }
    public void PlayWeapon(int sourceID, Vector3 direction,Vector3 targetPosition,SWeapon weaponInfo)
    {
        Play(sourceID, weaponInfo.m_ImpactSFXIndex,weaponInfo.m_BlastSFXIndex, direction,targetPosition,weaponInfo.m_Damage,weaponInfo.m_HorizontalSpeed,weaponInfo.m_HorizontalDistance,0,weaponInfo.m_VerticalAcceleration,  GameConst.I_WeaponProjectileMaxDistance / weaponInfo.m_HorizontalSpeed );
    }
    public void PlayBarrage(int sourceID, Vector3 direction, Vector3 targetPosition, SEntity barrageInfo,float duration =-1)
    {
        Play(sourceID, barrageInfo.m_ImpactSFXIndex,barrageInfo.m_BlastSFXIndex, direction, targetPosition, barrageInfo.m_ProjectileDamage, barrageInfo.m_ProjectileSpeed,200,0,0,  GameConst.I_BarrageProjectileMaxDistance / barrageInfo.m_ProjectileSpeed );
    }
    protected virtual void Play(int sourceID,int impactSFXIndex,int blastSFXIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed,float horiDistance,float vertiSpeed,float vertiAcceleration, float duration)
    {
        OnPlayPreset(damage, impactSFXIndex,blastSFXIndex);
        m_Simulator = new ProjectilePhysicsSimulator(transform.position, direction, Vector3.down, horiSpeed, horiDistance,vertiSpeed, vertiAcceleration);
        PlaySFX(sourceID, duration);
    }
    protected void OnPlayPreset(float damage,int impactSFXIndex,int _blastSFXIndex)
    {
        m_Damage = damage;
        i_impactSFXIndex = impactSFXIndex;
        i_blastSFXIndex = _blastSFXIndex;
        B_SimulatePhysics = true;
        m_Trail.Clear();
        m_hitEntity = null;
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
            float distance = Vector3.Distance(prePosition, curPosition);
            distance = distance > m_Collider.height ? distance : m_Collider.height;
            transform.position = curPosition;
            RaycastHit[] hitTargets = Physics.SphereCastAll(new Ray(prePosition, castDirection), m_Collider.radius, distance, GameLayer.Physics.I_All);
            for (int i = 0; i < hitTargets.Length; i++)
                if (OnHitTarget(hitTargets[i]))
                {
                    if(B_RecycleOnHit)
                        OnPlayFinished();
                    break;
                }
        }
    }

    protected virtual bool OnHitTarget(RaycastHit hitInfo)
    {
        if (hitInfo.collider == m_Collider)
            return false;

        m_Detect.DoDetect(hitInfo.collider);

        if (i_impactSFXIndex > 0)
        {
            SFXParticles impact= ObjectManager.SpawnSFX<SFXParticles>(i_impactSFXIndex, hitInfo.point, hitInfo.normal, hitInfo.transform);
            impact.Play(I_SourceID);
            if (m_hitEntity != null)
                m_hitEntity.AttachTransform(impact);
        }

        if (m_hitEntity!=null&&GameManager.B_CanHitTarget(m_hitEntity, I_SourceID))
            m_hitEntity.TryHit(m_Damage);

        return true;
    }

    protected virtual void OnHitEntity(HitCheckEntity hitEntity)
    {
        m_hitEntity = hitEntity;
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
}
