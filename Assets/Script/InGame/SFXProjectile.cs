﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TPhysics;
public class SFXProjectile : SFXBase
{
    #region PresetInfos
    public enum_ProjectileFireType E_ProjectileType= enum_ProjectileFireType.Invalid;
    public float F_Damage;
    public float F_Speed;
    public bool B_Penetrate;
    public int I_MuzzleIndex;
    public int I_ImpactIndex;
    public int I_IndicatorIndex;
    public int I_HitMarkIndex;
    public int I_BufFApplyOnHit;
    [Tooltip("Physics Size")]
    public float F_Radius = .5f, F_Height = 1f;
    [Tooltip("Projectile Type:(Single|Unused) (Multiline/MultiFan| Projectile Count Each Count)")]
    public RangeInt RI_CountExtension;
    [Tooltip("Projectile Type:(Single|Unused) (Multiline|Offset Meter)  (MultiFan|Offset Angle)")]
    public float F_OffsetExtension;
    [Tooltip("Will Blink While Projectile Time Limits, Require Prefab Folder Preset,Asset/BlinkModel")]
    public bool B_TargetReachBlink = false;
    public Color C_BlinkColor = Color.red;
    #endregion
    protected PhysicsSimulator<HitCheckBase> m_Simulator;
    protected TrailRenderer m_Trail;
    protected SFXIndicator m_Indicator;
    List<int> m_EntityHitted = new List<int>();
    public bool B_SimulatePhysics { get; protected set; }
    protected virtual float F_Duration(Vector3 startPos, Vector3 endPos) => GameConst.I_ProjectileMaxDistance / F_Speed;
    protected virtual bool B_DisablePhysicsOnHit => true;
    protected virtual bool B_DealDamage => true;
    protected virtual bool B_RecycleOnHit => true;
    protected ModelBlink m_Blink;
    protected virtual PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) => new ProjectilePhysicsSimulator(transform,transform.position, direction, Vector3.down, F_Speed, F_Height,F_Radius, GameLayer.Mask.I_All, OnHitTargetBreak,CanHitTarget);
    protected DamageInfo m_DamageInfo;
    public int m_sourceID => m_DamageInfo.m_detail.I_SourceID;
    protected virtual void PlayIndicator(float duration) => m_Indicator.Play(m_sourceID, duration);
    protected Vector3 m_CenterPos => F_Height > F_Radius * 2 ? transform.position + transform.forward * F_Height / 2 : transform.position + transform.forward * F_Radius;
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Trail = transform.GetComponentInChildren<TrailRenderer>();
        if (B_TargetReachBlink)
                m_Blink = new ModelBlink(transform.Find("BlinkModel"), .25f, .25f,C_BlinkColor);
    }

    public virtual void Play(DamageDeliverInfo deliverInfo ,Vector3 direction, Vector3 targetPosition )
    {
        OnPlayPreset();
        if (I_BufFApplyOnHit > 0)
            deliverInfo.AddExtraBuff(I_BufFApplyOnHit);
        m_DamageInfo=new DamageInfo(F_Damage, enum_DamageType.Basic,deliverInfo);
        m_Simulator = GetSimulator(direction, targetPosition);
        if (I_IndicatorIndex > 0)
            SpawnIndicator(targetPosition,Vector3.up, F_Duration(transform.position, targetPosition));

        if (m_Blink != null)
            m_Blink.OnReset();

        PlaySFX(deliverInfo.I_SourceID, F_Duration(transform.position, targetPosition));
    }
    
    protected virtual void OnPlayPreset()
    {
        B_SimulatePhysics = true;
        m_EntityHitted.Clear();

        if (m_Trail)
        {
            m_Trail.enabled = true;
            m_Trail.Clear();
        }

        if (E_ProjectileType == enum_ProjectileFireType.Invalid)
            Debug.LogError("Error Projectile Type Invalid:" + gameObject.name);
        if (F_Speed <= 0)
            Debug.LogError("Error Speed Less Or Equals 0:" + gameObject.name);
        if (I_ImpactIndex < 0)
            Debug.LogError("Error Impact Index Less 0:" + gameObject.name);
    }
    protected override void OnRecycle()
    {
        base.OnRecycle();
        if (m_Indicator)
        {
            m_Indicator.StopParticles();
            m_Indicator = null;
        }
    }
    protected override void Update()
    {
        base.Update();
        if (m_Simulator!=null&&B_SimulatePhysics)
            m_Simulator.Simulate(Time.deltaTime);

        if (m_Blink != null)
        {
            if (f_timeLeft < GameConst.I_ProjectileBlinkWhenTimeLeftLessThan)
            {
                float timeMultiply =2f*(1- f_timeLeft / GameConst.I_ProjectileBlinkWhenTimeLeftLessThan);
                m_Blink.Tick(Time.deltaTime*timeMultiply);
            }
        }
    }
    #region Physics
    protected virtual bool CanHitTarget(HitCheckBase hitCheck) => !m_EntityHitted.Contains(hitCheck.I_AttacherID) && GameManager.B_CanHitTarget(hitCheck, m_sourceID);
    protected bool OnHitTargetBreak(RaycastHit hitInfo, HitCheckBase hitCheck)
    {
        bool hitSource = hitInfo.point == Vector3.zero;
        Vector3 hitPoint = hitSource ? transform.position : hitInfo.point;
        Vector3 hitNormal = hitSource ? -transform.forward : hitInfo.normal;
        SpawnImpact(hitPoint, -transform.forward);
        SpawnHitMark(hitPoint,hitNormal,hitCheck);
        if ( OnHitTargetCanPenetrate(hitInfo, hitCheck)&& B_Penetrate)
            return false;

        if (B_DisablePhysicsOnHit) B_SimulatePhysics = false;
        if (B_RecycleOnHit) OnRecycle();
        return true;
    }
    
    protected virtual bool OnHitTargetCanPenetrate(RaycastHit hit, HitCheckBase hitCheck)
    {
        switch (hitCheck.m_HitCheckType)
        {
            case enum_HitCheck.Dynamic:
            case enum_HitCheck.Static:
                {
                    hitCheck.TryHit(m_DamageInfo);
                    return false;
                }
            case enum_HitCheck.Entity:
                {
                    HitCheckEntity entity = hitCheck as HitCheckEntity;
                    if (B_DealDamage && !m_EntityHitted.Contains(entity.I_AttacherID) && GameManager.B_CanDamageEntity(entity, m_sourceID))
                    {
                        entity.TryHit(m_DamageInfo, transform.forward);
                        m_EntityHitted.Add(entity.I_AttacherID);
                    }
                    return true;
                }
        }
        Debug.LogError("Invalid Item Hitted:"+hit.collider);
        return false;
    }
    #endregion
    protected virtual void SpawnIndicator(Vector3 position,Vector3 direction,float duration)
    {
        m_Indicator = GameObjectManager.SpawnIndicator(I_IndicatorIndex, position, direction);
        m_Indicator.Play(m_sourceID, duration);
    }
    protected void SpawnImpact(Vector3 hitPoint,Vector3 hitNormal)
    {
        if (I_ImpactIndex <= 0)
            return;

        GameObjectManager.SpawnParticles<SFXImpact>(I_ImpactIndex,hitPoint,hitNormal, null).Play(m_sourceID);
    }
    protected void SpawnHitMark(Vector3 hitPoint,Vector3 hitNormal,HitCheckBase hitParent)
    {
        if (I_HitMarkIndex <= 0)
            return;

        bool showhitMark = GameExpression.B_ShowHitMark(hitParent.m_HitCheckType);
        if (showhitMark)
        {
            SFXHitMark hitMark = GameObjectManager.SpawnParticles<SFXHitMark>(I_HitMarkIndex, hitPoint, hitNormal, hitParent.transform);
            hitMark.Play(m_sourceID);
            if (hitParent.m_HitCheckType == enum_HitCheck.Entity)
                (hitParent as HitCheckEntity).AttachHitMark(hitMark);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying &&GameManager.Instance&&!GameManager.Instance.B_PhysicsDebugGizmos)
            return;
        Gizmos.color = Color.yellow;
        Gizmos_Extend.DrawWireCapsule(m_CenterPos,Quaternion.LookRotation( transform.up,transform.forward), Vector3.one, F_Radius,F_Height);
    }
#endif
}
