using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
[RequireComponent(typeof(CapsuleCollider))]
public class SFXProjectile : SFXBase {
    protected float m_Damage;
    protected ProjectilePhysicsSimulator m_Simulator;
    protected CapsuleCollider m_Collider;
    protected TrailRenderer m_Trail;
    protected int i_impactSFXIndex,i_castSFXIndex;
    List<int> m_TargetHitted = new List<int>();
    public bool B_SimulatePhysics { get; protected set; }
    protected virtual bool B_RecycleOnHit => true;
    protected virtual bool B_DisablePhysicsOnHit => true;
    protected virtual bool B_HitMultiple => true;
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
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
    protected virtual void Play(int sourceID,int impactSFXIndex,int castSFXIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed,float horiDistance,float vertiSpeed,float vertiAcceleration, float duration)
    {
        OnPlayPreset(damage, impactSFXIndex,castSFXIndex);
        m_Simulator = new ProjectilePhysicsSimulator(transform.position, direction, Vector3.down, horiSpeed, horiDistance,vertiSpeed, vertiAcceleration);
        PlaySFX(sourceID, duration);
    }
    protected void OnPlayPreset(float damage,int impactSFXIndex,int castSFXIndex)
    {
        m_Damage = damage;
        i_impactSFXIndex = impactSFXIndex;
        i_castSFXIndex = castSFXIndex;
        B_SimulatePhysics = true;
        m_Trail.enabled = true;
        m_Trail.Clear();
        m_TargetHitted.Clear();
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
            {
                HitCheckBase hitCheck = hitTargets[i].collider.Detect();
                if (CanHitTarget(hitCheck))
                {
                    OnHitTarget(hitTargets[i], hitCheck);
                    SpawnImpact(hitTargets[i], hitCheck);
                    OnDamageEntity(hitCheck as HitCheckEntity);
                    if (B_DisablePhysicsOnHit)
                        B_SimulatePhysics = false;
                    if (B_RecycleOnHit)
                        OnPlayFinished();
                    if (!B_HitMultiple)
                        break;
                }
            }
        }
    }
    protected bool CanHitTarget(HitCheckBase hitCheck)
    {
        switch (hitCheck.m_HitCheckType)
        {
            case enum_HitCheck.Entity:
                {
                    HitCheckEntity entity = hitCheck as HitCheckEntity;
                    return !m_TargetHitted.Contains(entity.I_AttacherID) && GameManager.B_CanHitEntity(entity, I_SourceID);
                }
            case enum_HitCheck.Dynamic:
            case enum_HitCheck.Static:
                return true;
        }
        return false;
    }
    protected virtual void OnHitTarget(RaycastHit hit,HitCheckBase hitCheck)
    {
    }
    protected virtual void OnDamageEntity(HitCheckEntity entity)
    {
        if (entity!=null&&!m_TargetHitted.Contains(entity.I_AttacherID) && GameManager.B_CanDamageEntity(entity, I_SourceID))
        {
            entity.TryHit(m_Damage);
            m_TargetHitted.Add(entity.I_AttacherID);
        }
    }
    protected void SpawnImpact(RaycastHit hitInfo, HitCheckBase hitParent)
    {
        if (i_impactSFXIndex > 0)
        {
            SFXParticles impact = ObjectManager.SpawnSFX<SFXParticles>(i_impactSFXIndex, hitInfo.point, hitInfo.normal, null);
            if (hitParent != null && hitParent.m_HitCheckType == enum_HitCheck.Entity)
                (hitParent as HitCheckEntity).AttachTransform(impact);
            else
                impact.transform.SetParent(hitInfo.transform);
            impact.Play(I_SourceID);
        }
    }
}
