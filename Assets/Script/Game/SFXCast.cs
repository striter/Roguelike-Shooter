using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using TPhysics;
public class SFXCast : SFXWeaponBase {
    #region PresetInfo
    public enum_CastControllType E_CastType = enum_CastControllType.Invalid;
    public enum_CastTarget E_CastTarget = enum_CastTarget.Invalid;
    public int I_CastParticleIndex=-1;
    public bool B_CastForward = false;
    public int I_TickCount = 1;
    public float F_Tick = .5f;
    public enum_CastAreaType E_AreaType = enum_CastAreaType.Invalid;
    public Vector4 V4_CastInfo;
    public int I_ImpactIndex;
    public int F_DelayDuration;
    public int I_DelayIndicatorIndex;
    public bool B_CameraShake = false;
    #endregion
    protected DamageInfo m_DamageInfo;
    protected virtual float F_CastLength => V4_CastInfo.z;
    protected Transform CastTransform => tf_ControlledCast ? tf_ControlledCast : transform;
    protected float F_PlayDuration => I_TickCount * F_Tick;
    Transform tf_ControlledCast;
    TimerBase m_CastTickTimer = new TimerBase();
    SFXIndicator m_ControlledParticles;

    public bool m_OnceCast => F_Tick <= 0||I_TickCount<=1;
    public bool m_ControlledCast => tf_ControlledCast != null;

    public virtual void Play(DamageInfo buffInfo)
    {
        m_DamageInfo = buffInfo;
        base.PlayUncontrolled(m_DamageInfo.m_EntityID, F_PlayDuration, F_DelayDuration);
        if (I_DelayIndicatorIndex > 0)
            GameObjectManager.SpawnIndicator(I_DelayIndicatorIndex, transform.position, Vector3.up).PlayUncontrolled(m_SourceID, F_DelayDuration, 0);
    }

    public virtual void PlayControlled(int sourceID,EntityCharacterBase entity, Transform directionTrans)
    {
        base.PlayControlled(sourceID, F_DelayDuration);
        tf_ControlledCast = directionTrans;
        AttachTo(entity.tf_Weapon);
        if (I_CastParticleIndex > 0)
        {
            m_ControlledParticles = GameObjectManager.SpawnIndicator(I_CastParticleIndex, transform.position, transform.forward);
            m_ControlledParticles.PlayControlled(m_SourceID);
            m_ControlledParticles.AttachTo(entity.tf_Weapon);
        }
    }

    public void ControlledCheck(DamageInfo info)
    {
        m_DamageInfo = info;
        DoCastDealtDamage();
    }

    public virtual void StopControlled()
    {
        tf_ControlledCast = null;
        AttachTo(null);
        if (m_ControlledParticles)
        {
            m_ControlledParticles.Stop();
            m_ControlledParticles = null;
        }
        Stop();
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        if (m_ControlledCast)
            return;

        if (I_CastParticleIndex > 0)
            GameObjectManager.SpawnParticles(I_CastParticleIndex, transform.position, transform.forward).PlayUncontrolled(m_SourceID);

        if (B_CameraShake)
            GameManagerBase.Instance.SetEffect_Shake(V4_CastInfo.magnitude);

        if (m_OnceCast)
        {
            DoCastDealtDamage();
        }
        else
        {
            m_CastTickTimer.SetTimerDuration(F_Tick);
            m_CastTickTimer.Stop();
        }
    }
    
    protected override void Update()
    {
        base.Update();
        if (!B_Playing|| m_ControlledCast|| m_OnceCast)
            return;


        m_CastTickTimer.Tick(Time.deltaTime);
        if (!m_CastTickTimer.m_Timing)
        {
            DoCastDealtDamage();
            m_CastTickTimer.Replay();
        }
    }

    protected virtual List<EntityBase> DoCastDealtDamage()
    {
        List<EntityBase> entityHitted = new List<EntityBase>();
        RaycastHit[] hits = OnCastCheck(GameLayer.Mask.I_Entity);
        for (int i = 0; i < hits.Length; i++)
        {
            HitCheckBase hitCheck = hits[i].collider.Detect();
            switch (hitCheck.m_HitCheckType)
            {
                default:
                    continue;
                case enum_HitCheck.Entity:
                    HitCheckEntity entity = hitCheck as HitCheckEntity;
                    if (entityHitted.Contains(entity.m_Attacher) || !GameManager.B_CanSFXDamageEntity(entity, m_SourceID))
                        continue;
                    OnHitEntityDealtDamage(entity, hits[i].point, Vector3.Normalize(transform.position - hitCheck.transform.position));
                    entityHitted.Add(entity.m_Attacher);
                    break;
            }
        }
        return entityHitted;
    }

    public virtual void OnHitEntityDealtDamage(HitCheckEntity entity,Vector3 hitPoint,Vector3 hitNormal)
    {
        entity.TryHit(m_DamageInfo, -hitNormal);
        SpawnImpact(hitPoint, hitNormal);
    }

    protected RaycastHit[] OnCastCheck(int layerMask)
    {
        RaycastHit[] hits=null;
        switch (E_AreaType)
        {
            default:
                Debug.LogError("Invalid Convertions Here!");
                break;
            case enum_CastAreaType.OverlapSphere:
                {
                    float radius = V4_CastInfo.x;
                    hits= Physics.SphereCastAll(CastTransform.position, radius, CastTransform.forward,0f, layerMask);
                }
                break;
            case enum_CastAreaType.ForwardCapsule:
                {
                    float radius = V4_CastInfo.x;
                    float castLength = F_CastLength - radius*2;
                    castLength = castLength > 0 ? castLength : 0f;
                    hits = Physics.SphereCastAll(CastTransform.position+ CastTransform.forward * radius, radius, CastTransform.forward, castLength, layerMask);
                }
                break;
            case enum_CastAreaType.ForwardBox:
                {
                    hits = Physics_Extend.BoxCastAll(CastTransform.position, CastTransform.forward,CastTransform.up, new Vector3(V4_CastInfo.x,V4_CastInfo.y,F_CastLength), layerMask);
                }
                break;
            case enum_CastAreaType.ForwardTrapezium:
                {
                    hits = Physics_Extend.TrapeziumCastAll(CastTransform.position,CastTransform.forward,CastTransform.up,V4_CastInfo, layerMask);
                }
                break;
        }
        return hits;
    }   


    protected void SpawnImpact(Vector3 hitPoint,Vector3 hitNormal)
    {
        if (I_ImpactIndex <= 0)
            return;

        GameObjectManager.SpawnSFX<SFXParticles>(I_ImpactIndex, hitPoint, hitNormal).PlayUncontrolled(m_SourceID);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying &&GameManager.Instance&&!GameManager.Instance.B_PhysicsDebugGizmos)
            return;
        Gizmos.color = EDITOR_GizmosColor();
        Gizmos_Extend.DrawArrow(CastTransform.position,Quaternion.LookRotation(CastTransform.forward),new Vector3(V4_CastInfo.x/10,V4_CastInfo.y/10,F_CastLength / 4));
        switch (E_AreaType)
        {
            case enum_CastAreaType.OverlapSphere:
                    Gizmos.DrawWireSphere(CastTransform.position, V4_CastInfo.x);
                break;
            case enum_CastAreaType.ForwardBox:
                    Gizmos_Extend.DrawWireCube(CastTransform.position + CastTransform.forward * F_CastLength / 2, Quaternion.LookRotation(CastTransform.forward), new Vector3(V4_CastInfo.x,V4_CastInfo.y,F_CastLength));
                break;
            case enum_CastAreaType.ForwardCapsule:
                Gizmos_Extend.DrawWireCapsule(CastTransform.position+ CastTransform.forward* F_CastLength / 2,Quaternion.LookRotation(CastTransform.up, CastTransform.forward),Vector3.one,V4_CastInfo.x, F_CastLength);
                break;
            case enum_CastAreaType.ForwardTrapezium:
                Gizmos_Extend.DrawTrapezium(CastTransform.position+ CastTransform.forward*F_CastLength/2,Quaternion.LookRotation(CastTransform.up,CastTransform.forward),V4_CastInfo);
                break;
        }
    }
#endif
}
