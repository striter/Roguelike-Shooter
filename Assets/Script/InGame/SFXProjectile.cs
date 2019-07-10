using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TPhysics;
[RequireComponent(typeof(CapsuleCollider))]
public class SFXProjectile : SFXBase {
    protected PhysicsSimulator m_Simulator;
    protected CapsuleCollider m_Collider;
    protected TrailRenderer m_Trail;
    List<int> m_TargetHitted = new List<int>();
    public bool B_SimulatePhysics { get; protected set; }
    protected virtual bool B_RecycleOnHit => true;
    protected virtual bool B_DisablePhysicsOnHit => true;
    protected virtual bool B_HitMultiple => true;
    protected virtual bool B_DealDamage => true;
    public float F_Damage;
    public float F_Speed;
    public int I_ImpactIndex;
    protected virtual PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition) => new ProjectilePhysicsSimulator(transform.position, direction, Vector3.down, F_Speed);
    protected virtual Quaternion GetRotation(Vector3 direction) => Quaternion.LookRotation(direction);
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Trail = transform.GetComponentInChildren<TrailRenderer>();
        m_Collider = GetComponent<CapsuleCollider>();
        m_Collider.enabled = false;
    }

    public virtual void Play(int sourceID,Vector3 direction,Vector3 targetPosition,float duration=-1)
    {
        OnPlayPreset();
        m_Simulator = GetSimulator(direction,targetPosition);
        PlaySFX(sourceID, duration==-1?GameConst.I_ProjectileMaxDistance/ F_Speed : duration);
    }

    protected virtual void OnPlayPreset()
    {
        B_SimulatePhysics = true;
        m_Trail.enabled = true;
        m_Trail.Clear();
        m_TargetHitted.Clear();

        if (F_Damage <= 0)
            Debug.LogError("Error Damage Less Or Equals 0");
        if (F_Speed <= 0)
            Debug.LogError("Error Speed Less Or Equals 0");
        if (I_ImpactIndex < 0)
            Debug.LogError("Error Impact Index Less 0");
    }
    protected override void Update()
    {
        base.Update();
        if (B_SimulatePhysics)
        {
            Vector3 prePosition;
            Vector3 curPosition = m_Simulator.Simulate(Time.deltaTime, out prePosition);

            Vector3 castDirection = prePosition == curPosition ? m_Simulator.m_Direction : curPosition - prePosition;
            transform.rotation = GetRotation(castDirection);
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
        if (B_DealDamage&&entity!=null&&!m_TargetHitted.Contains(entity.I_AttacherID) && GameManager.B_CanDamageEntity(entity, I_SourceID))
        {
            entity.TryHit(F_Damage);
            m_TargetHitted.Add(entity.I_AttacherID);
        }
    }
    protected void SpawnImpact(RaycastHit hitInfo, HitCheckBase hitParent)
    {
        if (I_ImpactIndex > 0)
        {
            SFXParticles impact = ObjectManager.SpawnSFX<SFXParticles>(I_ImpactIndex, hitInfo.point, hitInfo.normal, null);
            if (hitParent != null && hitParent.m_HitCheckType == enum_HitCheck.Entity)
                (hitParent as HitCheckEntity).AttachTransform(impact);
            else
                impact.transform.SetParent(hitInfo.transform);
            impact.Play(I_SourceID);
        }
    }
}
