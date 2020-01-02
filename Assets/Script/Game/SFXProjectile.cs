using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TPhysics;
public class SFXProjectile : SFXWeaponBase
{
    #region PresetInfos 
    public enum_ProjectileFireType E_ProjectileType= enum_ProjectileFireType.Invalid;
    public float F_Damage;
    public float F_Speed;
    public bool B_Penetrate;
    public AudioClip AC_MuzzleClip;
    public int I_MuzzleIndex;
    public int I_ImpactIndex;
    public int I_IndicatorIndex;
    public int I_HitMarkIndex;
    public int I_BuffApplyOnHit;
    [Tooltip("Physics Size")]
    public float F_Radius = .5f, F_Height = 1f;
    [Tooltip("Projectile Type:(Single|Unused) (Multiline/MultiFan| Projectile Count Each Count)")]
    public RangeInt RI_CountExtension;
    [Tooltip("Projectile Type:(Single|Unused) (Multiline|Offset Meter)  (MultiFan|Offset Angle)")]
    public float F_OffsetExtension;
    #endregion
    protected PhysicsSimulator<HitCheckBase> m_PhysicsSimulator { get; private set; }
    protected SFXIndicator m_Indicator;
    public bool B_PhysicsSimulating { get; private set; }
    protected virtual float F_PlayDuration(Vector3 startPos, Vector3 endPos) => GameConst.I_ProjectileMaxDistance / F_Speed;
    protected virtual float F_PlayDelay => 0f;
    protected virtual bool B_StopParticlesOnHit => true;
    protected virtual bool B_StopPhysicsOnHit => true;
    protected virtual PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) => new SpeedDirectionPSimulator<HitCheckBase>(transform,transform.position, direction, Vector3.down, F_Speed, F_Height,F_Radius, GameLayer.Mask.I_All, OnHitTargetBreak,CanHitTarget);
    protected DamageInfo m_DamageInfo { get; private set; }
    protected int m_SourceID => m_DamageInfo.m_detail.I_SourceID;
    protected virtual void PlayIndicator(float duration) => m_Indicator.Play(I_SourceID, duration);
    protected Vector3 m_CenterPos => F_Height > F_Radius * 2 ? transform.position + transform.forward * F_Height / 2 : transform.position + transform.forward * F_Radius;
    protected List<int> m_EntityHitted = new List<int>();
    protected bool CanHitTarget(HitCheckBase hitCheck) => !m_EntityHitted.Contains(hitCheck.I_AttacherID) && GameManager.B_CanSFXHitTarget(hitCheck, m_SourceID);
    protected bool CanDamageEntity(HitCheckEntity _entity) => !m_EntityHitted.Contains(_entity.I_AttacherID) && GameManager.B_CanSFXDamageEntity(_entity, m_SourceID);
    public virtual void Play(DamageDeliverInfo deliverInfo ,Vector3 direction, Vector3 targetPosition )
    {
        if (I_BuffApplyOnHit > 0)
            deliverInfo.AddExtraBuff(I_BuffApplyOnHit);
        m_DamageInfo=new DamageInfo(F_Damage-damageMinus, enum_DamageType.Basic,deliverInfo);
        m_PhysicsSimulator = GetSimulator(direction, targetPosition);
        SpawnIndicator(targetPosition,Vector3.up, F_PlayDelay);
        m_EntityHitted.Clear();
        base.Play(deliverInfo.I_SourceID, F_PlayDuration(transform.position, targetPosition), F_PlayDelay);
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        B_PhysicsSimulating = true;
        SetParticlesActive(true);
    }
    protected override void OnStop()
    {
        base.OnStop();
        B_PhysicsSimulating = false;
        SetParticlesActive(false);
        if (m_Indicator)
        {
            m_Indicator.Stop();
            m_Indicator = null;
        }
    }
    protected override void Update()
    {
        base.Update();

        if (m_PhysicsSimulator!=null&&B_PhysicsSimulating)
            m_PhysicsSimulator.Simulate(Time.deltaTime);
    }
    #region Physics
    protected bool OnHitTargetBreak(RaycastHit hitInfo, HitCheckBase hitCheck)
    {
        bool hitSource = hitInfo.point == Vector3.zero;
        Vector3 hitPoint = hitSource ? transform.position : hitInfo.point;
        Vector3 hitNormal = hitSource ? -transform.forward : hitInfo.normal;
        SpawnImpact(hitPoint, -transform.forward);
        SpawnHitMark(hitPoint,hitNormal,hitCheck);
        OnHitTarget(hitInfo, hitCheck);
        if (OnHitTargetPenetrate(hitCheck))
            return false;

        if (B_StopParticlesOnHit) OnStop();
        if (B_StopPhysicsOnHit) B_PhysicsSimulating = false;
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
                if (CanDamageEntity(_entity))
                    _entity.TryHit(m_DamageInfo, transform.forward);
                m_EntityHitted.Add(_entity.I_AttacherID);
                OnHitCheckCopies(_entity.I_AttacherID);
                break;
        }
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
    protected virtual void SpawnIndicator(Vector3 position,Vector3 direction,float duration)
    {
        if (I_IndicatorIndex <= 0)
            return;
        m_Indicator = GameObjectManager.SpawnIndicator(I_IndicatorIndex, position, direction);
        m_Indicator.Play(m_SourceID, duration);
    }
    protected void SpawnImpact(Vector3 hitPoint,Vector3 hitNormal)
    {
        if (I_ImpactIndex <= 0)
            return;

        GameObjectManager.SpawnSFX<SFXImpact>(I_ImpactIndex,hitPoint,hitNormal).Play(m_SourceID);
    }
    protected void SpawnHitMark(Vector3 hitPoint,Vector3 hitNormal,HitCheckBase hitParent)
    {
        if (I_HitMarkIndex <= 0)
            return;

        bool showhitMark = GameExpression.B_ShowHitMark(hitParent.m_HitCheckType);
        if (showhitMark)
        {
            SFXHitMark hitMark = GameObjectManager.SpawnSFX<SFXHitMark>(I_HitMarkIndex, hitPoint, hitNormal);
            hitMark.Play(m_SourceID);
            hitMark.AttachTo(hitParent.transform);
        }
    }

    #region ??????????????????????????????????????
    int m_copiesLeft;
    int damageMinus;    //?
    WeaponHelperBase weapon;
    public void PlayerCopyCount(DamageDeliverInfo deliverInfo, Vector3 direction, Vector3 targetPosition, int copyCount,int _damageMinus)//?????
    {
        m_copiesLeft = copyCount;
        damageMinus = _damageMinus;
        Play(deliverInfo, direction, targetPosition);
    }
    
    void OnHitCheckCopies(int hitTargetID)
    {
        if (m_copiesLeft <= 0)
            return;
        EntityCharacterBase target= GameManager.Instance.GetAvailableEntity(GameManager.Instance.GetCharacter(hitTargetID),true,false,5f);
        if (!target)
            return;
        m_copiesLeft--;
        Vector3 direction=TCommon.GetXZLookDirection(transform.position,target.tf_Head.position );
        SFXProjectile projectile = GameObjectManager.SpawnEquipment<SFXProjectile>(m_Identity, transform.position, direction);
        projectile.F_Speed = F_Speed;
        projectile.B_Penetrate = B_Penetrate;
        projectile.PlayerCopyCount(m_DamageInfo.m_detail, direction, transform.position + direction * GameConst.I_ProjectileMaxDistance,m_copiesLeft,0);
        projectile.m_EntityHitted.AddRange(m_EntityHitted);
    }
    #endregion
#if UNITY_EDITOR
    protected override void EDITOR_DEBUG()
    {
        base.EDITOR_DEBUG();
        if (E_ProjectileType == enum_ProjectileFireType.Invalid)
            Debug.LogError("Error Projectile Type Invalid:" + gameObject.name);
        if (F_Speed <= 0)
            Debug.LogError("Error Speed Less Or Equals 0:" + gameObject.name);
        if (I_ImpactIndex < 0)
            Debug.LogError("Error Impact Index Less 0:" + gameObject.name);
    }
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying &&GameManager.Instance&&!GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        Gizmos.color = EDITOR_GizmosColor();
        Gizmos_Extend.DrawWireCapsule(m_CenterPos,Quaternion.LookRotation( transform.up,transform.forward), Vector3.one, F_Radius,F_Height);
    }
#endif
}
