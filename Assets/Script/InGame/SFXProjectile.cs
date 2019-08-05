﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TPhysics;
public class SFXProjectile : SFXBase
{
    public enum_EnermyWeaponProjectile E_ProjectileType= enum_EnermyWeaponProjectile.Invalid;
    public float F_Damage;
    public float F_Speed;
    public int I_MuzzleIndex;
    public int I_ImpactIndex;
    public int I_IndicatorIndex;
    public int I_HitMarkIndex;
    public int I_BufFApplyOnHit;
    public float F_Radius = .5f, F_Height = 1f;
    public bool B_TargetReachBlink = false;
    protected PhysicsSimulator<HitCheckBase> m_Simulator;
    protected TrailRenderer m_Trail;
    protected SFXIndicator m_Indicator;
    List<int> m_TargetHitted = new List<int>();
    public bool B_SimulatePhysics { get; protected set; }
    protected virtual float F_Duration(Vector3 startPos, Vector3 endPos) => GameConst.I_ProjectileMaxDistance / F_Speed;
    protected virtual bool B_RecycleOnHit => true;
    protected virtual bool B_DisablePhysicsOnHit => true;
    protected virtual bool B_HitMultiple => false;
    protected virtual bool B_DealDamage => true;
    protected ProjectileBlink m_Blink;
    protected DamageInfo m_DamageInfo;
    protected virtual PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) => new ProjectilePhysicsSimulator(transform,transform.position, direction, Vector3.down, F_Speed, F_Height,F_Radius, GameLayer.Physics.I_All, OnHitTargetBreak,CanHitTarget);
    protected virtual void PlayIndicator(float duration) => m_Indicator.Play(I_SourceID,duration);
    protected Vector3 m_CenterPos => F_Height > F_Radius * 2 ? transform.position + transform.forward * F_Height / 2 : transform.position + transform.forward * F_Radius;
    public override void Init(int sfxIndex)
    {
        base.Init(sfxIndex);
        m_Trail = transform.GetComponentInChildren<TrailRenderer>();
        m_DamageInfo = new DamageInfo(F_Damage, enum_DamageType.Projectile);
        if (B_TargetReachBlink)
        {
            try
            {
                m_Blink = new ProjectileBlink(transform.Find("BlinkModel").GetComponent<Renderer>().materials, .25f, .25f);
            }
            catch
            {
                Debug.LogError("Error! Blink Model Init, BlinkModel Folder Required!" + gameObject.name);
                m_Blink = null;
            }
        }
    }

    public virtual void Play(int sourceID, Vector3 direction, Vector3 targetPosition, DamageBuffInfo buffInfo)
    {
        OnPlayPreset();
        if (I_BufFApplyOnHit > 0)
            buffInfo.m_BuffAplly.Add(I_BufFApplyOnHit);
        m_DamageInfo.ResetBuff(buffInfo);

        m_Simulator = GetSimulator(direction, targetPosition);
        if (I_IndicatorIndex > 0)
            SpawnIndicator(targetPosition,Vector3.up, F_Duration(transform.position, targetPosition));

        PlaySFX(sourceID, F_Duration(transform.position, targetPosition));
    }

    protected virtual void OnPlayPreset()
    {
        B_SimulatePhysics = true;
        m_TargetHitted.Clear();

        if (m_Trail)
        {
            m_Trail.enabled = true;
            m_Trail.Clear();
        }

        if (E_ProjectileType == enum_EnermyWeaponProjectile.Invalid)
            Debug.LogError("Error Projectile Type Invalid:" + gameObject.name);
        if (F_Damage <= 0)
            Debug.LogError("Error Damage Less Or Equals 0:"+gameObject.name);
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
        if (m_Blink!=null)
            m_Blink.OnReset();
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
    protected bool OnHitTargetBreak(RaycastHit hit, HitCheckBase hitCheck)
    {
        OnHitTarget(hit, hitCheck);
        SpawnImpact(hit, hitCheck);
        if (B_DisablePhysicsOnHit)
            B_SimulatePhysics = false;
        if (B_RecycleOnHit)
            OnRecycle();

        return !B_HitMultiple;
    }
    protected virtual bool CanHitTarget(HitCheckBase hitCheck)
    {
        switch (hitCheck.m_HitCheckType)
        {
            case enum_HitCheck.Dynamic:
            case enum_HitCheck.Static:
                return hitCheck.I_AttacherID != I_SourceID;
            case enum_HitCheck.Entity:
                {
                    HitCheckEntity entity = hitCheck as HitCheckEntity;
                    return !m_TargetHitted.Contains(entity.I_AttacherID) && GameManager.B_CanHitEntity(entity, I_SourceID);
                }
        }
        return false;
    }
    protected virtual void OnHitTarget(RaycastHit hit, HitCheckBase hitCheck)
    {
        switch (hitCheck.m_HitCheckType)
        {
            case enum_HitCheck.Dynamic:
            case enum_HitCheck.Static:
                hitCheck.TryHit(m_DamageInfo);
                break;
            case enum_HitCheck.Entity:
                {
                    HitCheckEntity entity = hitCheck as HitCheckEntity;
                    if (B_DealDamage && !m_TargetHitted.Contains(entity.I_AttacherID) && GameManager.B_CanDamageEntity(entity, I_SourceID))
                    {
                        entity.TryHit(m_DamageInfo);
                        m_TargetHitted.Add(entity.I_AttacherID);
                    }
                }
                break;
        }
    }
    #endregion
    protected virtual void SpawnIndicator(Vector3 position,Vector3 direction,float duration)
    {
        m_Indicator = ObjectManager.SpawnIndicator(I_IndicatorIndex, position, direction);
        m_Indicator.Play(I_SourceID, duration );
    }
    protected void SpawnImpact(RaycastHit hitInfo, HitCheckBase hitParent)
    {
        if (I_ImpactIndex > 0)
             ObjectManager.SpawnParticles<SFXImpact>(I_ImpactIndex, hitInfo.point, hitInfo.normal, null).Play(I_SourceID);

        if (I_HitMarkIndex > 0)
        {
            bool showhitMark = GameExpression.B_ShowHitMark(hitParent.m_HitCheckType);
            if (showhitMark)
            {
                SFXHitMark hitMark= ObjectManager.SpawnParticles<SFXHitMark>(I_HitMarkIndex, hitInfo.point, hitInfo.normal, hitParent.transform);
                hitMark.Play(I_SourceID);
                if (hitParent.m_HitCheckType == enum_HitCheck.Entity)
                    (hitParent as HitCheckEntity).AttachHitMark(hitMark);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_PhysicsDebugGizmos)
            return;
        Gizmos.color = Color.yellow;
        Gizmos_Extend.DrawWireCapsule(m_CenterPos,Quaternion.LookRotation( transform.up,transform.forward), Vector3.one, F_Radius,F_Height);
    }
#endif
}
