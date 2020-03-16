using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCast : SFXWeaponBase {
    #region PresetInfo
    public enum_CastControllType E_CastType = enum_CastControllType.Invalid;
    public enum_CastTarget E_CastTarget = enum_CastTarget.Invalid;
    public bool B_CastForward = false;
    public float F_Damage;
    public int I_TickCount = 1;
    public float F_Tick = .5f;
    public int I_BuffApplyOnCast;
    public enum_CastAreaType E_AreaType = enum_CastAreaType.Invalid;
    public Vector4 V4_CastInfo;
    public int F_DelayDuration;
    public int I_DelayIndicatorIndex;
    public bool B_CameraShake = false;
    #endregion
    protected DamageInfo m_DamageInfo;
    protected virtual float F_CastLength => V4_CastInfo.z;
    protected Transform CastTransform => tf_ControlledCast ? tf_ControlledCast : transform;
    protected float F_PlayDuration => I_TickCount * F_Tick;
    Transform tf_ControlledCast;
    float f_blastTickChest = 0;
    public bool m_ControlledCast => tf_ControlledCast != null;
    public virtual void Play(DamageDeliverInfo buffInfo)
    {
        SetDamageInfo(buffInfo);
        base.PlayOnce(m_DamageInfo.m_detail.I_SourceID, F_PlayDuration, F_DelayDuration);

        if (I_DelayIndicatorIndex > 0)
            GameObjectManager.SpawnIndicator(I_DelayIndicatorIndex, transform.position, Vector3.up).PlayOnce(m_SourceID,0 ,F_DelayDuration);
    }

    public virtual void PlayControlled(int sourceID,EntityCharacterBase entity, Transform directionTrans)
    {
        tf_ControlledCast = directionTrans;
        AttachTo(entity.tf_Weapon);
        base.PlayLoop(sourceID, F_DelayDuration);
    }

    public void ControlledCheck(DamageDeliverInfo info)
    {
        SetDamageInfo(info);
        DoCastDealtDamage();
    }

    public virtual void StopControlled()
    {
        tf_ControlledCast = null;
        AttachTo(null);
        Stop();
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        if (m_ControlledCast)
            return;

        if (B_CameraShake)
            GameManagerBase.Instance.SetEffect_Shake(V4_CastInfo.magnitude);
        
        if (F_Tick <= 0)
            DoCastDealtDamage();
    }

    void SetDamageInfo(DamageDeliverInfo info)
    {
        if (I_BuffApplyOnCast > 0)
            info.AddExtraBuff( I_BuffApplyOnCast);
        m_DamageInfo = new DamageInfo(F_Damage, enum_DamageType.Basic, info);
    }
    protected override void Update()
    {
        base.Update();
        if (!B_Playing)
            return;
        
        if (F_Tick <= 0)
            return;
        if (f_blastTickChest > 0)
        {
            f_blastTickChest -= Time.deltaTime;
            return;
        }
        DoCastDealtDamage();
        f_blastTickChest = F_Tick;
    }

    protected virtual List<EntityBase> DoCastDealtDamage()
    {
        List<EntityBase> entityHitted = new List<EntityBase>();
        RaycastHit[] hits = OnCastCheck(GameLayer.Mask.I_All);
        for (int i = 0; i < hits.Length; i++)
        {
            HitCheckBase hitCheck = hits[i].collider.Detect();
            switch(hitCheck.m_HitCheckType)
            {
                case enum_HitCheck.Dynamic:
                case enum_HitCheck.Static:
                    break;
                case enum_HitCheck.Entity:
                    HitCheckEntity entity = hitCheck as HitCheckEntity;
                    if (entityHitted.Contains(entity.m_Attacher) || !GameManager.B_CanSFXDamageEntity(entity, m_SourceID))
                        continue;
                    entityHitted.Add(entity.m_Attacher);
                    break;
            }
            OnDealtDamage(hitCheck);
        }
        return entityHitted;
    }
    protected RaycastHit[] OnCastCheck(int layerMask)
    {
        RaycastHit[] hits=null;
        switch (E_AreaType)
        {
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
    protected virtual void OnDealtDamage(HitCheckBase hitCheck)=> hitCheck.TryHit(m_DamageInfo, Vector3.Normalize(hitCheck.transform.position - transform.position));

    protected override void EDITOR_DEBUG()
    {
        if (E_CastType == enum_CastControllType.Invalid)
            Debug.LogError("Weapon Type Invalid Detected+" + gameObject.name);
        if (E_AreaType == enum_CastAreaType.Invalid)
            Debug.LogError("Cast Type Invalid Detected:" + gameObject.name);
        if (V4_CastInfo == Vector4.zero)
            Debug.LogError("Cast Size Zero Detected:" + gameObject.name);
        if (I_TickCount <= 0)
            Debug.LogError("Tick Count Less Or Equals Zero:" + gameObject.name);
        if (I_TickCount > 1 && F_Tick <= 0)
            Debug.LogError("Tick Duration Less Or Equals Zero:" + gameObject.name);
        if (F_DelayDuration > 0 && I_DelayIndicatorIndex < 0)
            Debug.LogError("Delay Indicator Less Than Zero:" + gameObject.name);
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
