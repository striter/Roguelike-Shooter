using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TPhysics;
public class SFXProjectile : SFXDamageBase
{
    #region PresetInfos 
    public enum_ProjectileFireType E_ProjectileType = enum_ProjectileFireType.Invalid;
    [Tooltip("Projectile Type:(Single|Unused) (Multiline/MultiFan| Projectile Count Each Count)")]
    public RangeInt RI_CountExtension;
    [Tooltip("Projectile Type:(Single|Unused) (Multiline|Offset Meter)  (MultiFan|Offset Angle)")]
    public float F_OffsetExtension;

    public float F_Speed;
    public bool B_Penetrate;
    public AudioClip AC_MuzzleClip;
    public int I_TrailIndex;
    public int I_MuzzleIndex;
    public int I_ImpactIndex;
    public int I_IndicatorIndex;
    [Tooltip("Physics Size")]
    public float F_Radius = .5f, F_Height = 1f;
    #endregion

    protected PhysicsSimulator<HitCheckBase> m_PhysicsSimulator { get; private set; }
    protected SFXParticles m_Indicator;
    protected SFXTrail m_Trail;
    protected virtual float F_PlayDuration(Vector3 startPos, Vector3 endPos) => GameConst.I_ProjectileInvalidDistance / F_Speed;
    protected virtual float F_PlayDelay => 0f;
    protected virtual bool B_StopOnPenetradeFail => true;
    protected virtual PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) => new SpeedDirectionPSimulator<HitCheckBase>(transform,transform.position, direction, Vector3.down, F_Speed, F_Height,F_Radius, GameLayer.Mask.I_ProjectileMask, OnHitTargetBreak,CanHitTarget);
    protected DamageInfo m_DamageInfo { get; private set; }
    protected virtual void PlayIndicator(float duration) => m_Indicator.PlayUncontrolled(base.m_SourceID, duration);
    protected Vector3 m_CenterPos => F_Height > F_Radius * 2 ? transform.position + transform.forward * F_Height / 2 : transform.position + transform.forward * F_Radius;
    protected List<int> m_EntityHitted = new List<int>();
    protected bool CanHitTarget(HitCheckBase hitCheck) => !m_EntityHitted.Contains(hitCheck.I_AttacherID) && GameManager.B_CanSFXHitTarget(hitCheck, m_SourceID);
    protected bool CanDamageEntity(HitCheckEntity _entity) => !m_EntityHitted.Contains(_entity.I_AttacherID) && GameManager.B_CanSFXDamageEntity(_entity, m_SourceID);
    public virtual void Play(DamageInfo damageInfo ,Vector3 direction, Vector3 targetPosition )
    {
        base.PlaySFX (damageInfo.m_EntityID, F_PlayDuration(transform.position, targetPosition), F_PlayDelay,true);
        m_DamageInfo= damageInfo;
        m_PhysicsSimulator = GetSimulator(direction, targetPosition);
        SpawnProjectileEffects(targetPosition, F_PlayDelay);
        m_EntityHitted.Clear();
    }

    protected override void OnStop()
    {
        base.OnStop();
        RemoveProjectileEffects();
    }
    protected override void Update()
    {
        base.Update();

        if (B_Playing && m_PhysicsSimulator != null)
            m_PhysicsSimulator.Simulate(Time.deltaTime);
    }
    #region Physics
    protected bool OnHitTargetBreak(RaycastHit hitInfo, HitCheckBase hitCheck)
    {
        bool hitSource = hitInfo.point == Vector3.zero;
        Vector3 hitPoint = hitSource ? transform.position : hitInfo.point;
        Vector3 hitNormal =  -transform.forward;
        SpawnImpact(hitPoint, hitNormal);
        OnHitTarget(hitInfo, hitCheck);
        if (OnHitTargetPenetrate(hitCheck))
            return false;
        
        if (B_StopOnPenetradeFail) OnStop();
        return true;
    }

    protected void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        switch (hitCheck.m_HitCheckType)
        {
            default:
                Debug.LogError("Invalid Item Hitted:" + hit.collider);
                break;
            case enum_HitCheck.Dynamic:
            case enum_HitCheck.Static:
                hitCheck.TryHit(m_DamageInfo, transform.forward);
                break;
            case enum_HitCheck.Entity:
                HitCheckEntity _entity = hitCheck as HitCheckEntity;
                OnHitEntityDealtDamage(_entity);
                m_EntityHitted.Add(_entity.I_AttacherID);
                break;
        }
    }

    protected virtual void OnHitEntityDealtDamage(HitCheckEntity _entity)
    {
        if (CanDamageEntity(_entity))
            _entity.TryHit(m_DamageInfo, transform.forward);
    }

    protected virtual bool OnHitTargetPenetrate(HitCheckBase hitCheck)
    {
        switch (hitCheck.m_HitCheckType)
        {
            default:
                Debug.LogError("Invalid Item Hitted:" + hitCheck);
                return false;
            case enum_HitCheck.Dynamic:
            case enum_HitCheck.Static:
                return false;
            case enum_HitCheck.Entity:
                return B_Penetrate;
        }
    }
    #endregion

    protected void SpawnProjectileEffects(Vector3 position,float delayDuration)
    {
        if(I_TrailIndex>0)
        {
            m_Trail = GameObjectManager.SpawnTrail(I_TrailIndex, m_CenterPos, transform.forward);
            m_Trail.transform.localScale = Vector3.one*(m_DamageInfo.m_CritcalHitted?1f:2f) ;
            m_Trail.AttachTo(transform);
            m_Trail.PlayControlled(m_SourceID);
        }

        if (I_IndicatorIndex > 0)
        {
            m_Indicator = GameObjectManager.SpawnIndicator(I_IndicatorIndex, position, Vector3.up);
            m_Indicator.PlayUncontrolled(m_SourceID ,delayDuration);
        }
    }

    protected void RemoveProjectileEffects()
    {
        if (m_Trail)
        {
            m_Trail.Stop();
            m_Trail = null;
        }
        if (m_Indicator)
        {
            m_Indicator.Stop();
            m_Indicator = null;
        }
    }

    protected void SpawnImpact(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (I_ImpactIndex > 0)
            GameObjectManager.SpawnParticles(I_ImpactIndex, hitPoint, hitNormal).PlayUncontrolled(m_SourceID);
    }

    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying &&GameManager.Instance&&!GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        Gizmos.color = EDITOR_GizmosColor();
        Gizmos_Extend.DrawWireCapsule(m_CenterPos,Quaternion.LookRotation( transform.up,transform.forward), Vector3.one, F_Radius,F_Height);
    }
#endif
}
