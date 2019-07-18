using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TPhysics;
public class SFXProjectile : SFXBase
{
    public float F_Damage;
    public float F_Speed;
    public int I_ImpactIndex;
    public int I_BufFApplyOnHit;
    public float F_Radius = .5f, F_Height = 1f;
    protected PhysicsSimulator m_Simulator;
    protected TrailRenderer m_Trail;
    List<int> m_TargetHitted = new List<int>();
    public bool B_SimulatePhysics { get; protected set; }
    protected virtual float F_Duration(Vector3 startPos, Vector3 endPos) => GameConst.I_ProjectileMaxDistance / F_Speed;
    protected virtual bool B_RecycleOnHit => true;
    protected virtual bool B_DisablePhysicsOnHit => true;
    protected virtual bool B_HitMultiple => false;
    protected virtual bool B_DealDamage => true;
    protected DamageInfo m_DamageInfo;
    protected virtual PhysicsSimulator GetSimulator(Vector3 direction, Vector3 targetPosition) => new ProjectilePhysicsSimulator(transform,transform.position, direction, Vector3.down, F_Speed, F_Height,F_Radius, GameLayer.Physics.I_All, OnPhysicsCasted);
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Trail = transform.GetComponentInChildren<TrailRenderer>();
        m_DamageInfo = new DamageInfo(F_Damage, enum_DamageType.Projectile);
    }

    public virtual void Play(int sourceID, Vector3 direction, Vector3 targetPosition, DamageBuffInfo buffInfo)
    {
        OnPlayPreset();
        if (I_BufFApplyOnHit > 0)
            buffInfo.m_BuffAplly.Add(I_BufFApplyOnHit);
        m_DamageInfo.ResetBuff(buffInfo);
        m_Simulator = GetSimulator(direction, targetPosition);
        PlaySFX(sourceID, F_Duration(transform.position, targetPosition));
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
            m_Simulator.Simulate(Time.deltaTime);
    }
    protected void OnPhysicsCasted(RaycastHit[] hitTargets)
    {
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
    protected virtual void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {

    }
    protected virtual void OnDamageEntity(HitCheckEntity entity)
    {
        if (B_DealDamage && entity != null && !m_TargetHitted.Contains(entity.I_AttacherID) && GameManager.B_CanDamageEntity(entity, I_SourceID))
        {
            entity.TryHit(m_DamageInfo);
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_GizmosInGame)
            return;
        Gizmos.color = Color.yellow;
        Gizmos_Extend.DrawWireCapsule(F_Height>F_Radius*2? transform.position+transform.forward*F_Height/2:transform.position+transform.forward*F_Radius,Quaternion.LookRotation( transform.up,transform.forward), Vector3.one, F_Radius,F_Height);
    }
#endif
}
