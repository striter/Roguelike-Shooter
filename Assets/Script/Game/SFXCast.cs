using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCast : SFXParticles {
    #region PresetInfo
    public enum_CastControllType E_CastType = enum_CastControllType.Invalid;
    public enum_CastTarget E_CastTarget =  enum_CastTarget.Invalid;
    public bool B_CastForward = false;
    public float F_Damage;
    public int I_TickCount=1;
    public float F_Tick = .5f;
    public int I_BuffApplyOnCast;
    public enum_CastAreaType E_AreaType = enum_CastAreaType.Invalid;
    public Vector4 V4_CastInfo;
    public int I_MuzzleIndex = 0;
    public int I_ImpactIndex = 0;
    public int F_DelayDuration;
    public int I_DelayIndicatorIndex;
    public bool B_CameraShake = false;
    #endregion
    protected DamageInfo m_DamageInfo;
    public int m_sourceID => m_DamageInfo.m_detail.I_SourceID;
    protected virtual float F_CastLength => V4_CastInfo.z;
    protected Transform CastTransform => tf_ControlledCast ? tf_ControlledCast : transform;
    protected float F_PlayDuration => I_TickCount * F_Tick;
    Transform tf_ControlledAttach,tf_ControlledCast;
    float f_blastTickChest = 0;
    public virtual void Play(DamageDeliverInfo buffInfo)
    {
        SetDamageInfo(buffInfo);
        if (I_DelayIndicatorIndex>0)
            GameObjectManager.SpawnIndicator(I_DelayIndicatorIndex, transform.position, Vector3.up).Play(m_sourceID,  F_DelayDuration);

        base.Play(m_DamageInfo.m_detail.I_SourceID, F_PlayDuration, F_DelayDuration);
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        if (B_CameraShake)
            GameManagerBase.Instance.SetEffect_Shake(V4_CastInfo.magnitude);

        if (I_ImpactIndex > 0)
            GameObjectManager.SpawnParticles<SFXImpact>(I_ImpactIndex,transform.position,transform.forward).Play(m_sourceID);

        if (F_Tick <= 0)
            DoBlastCheck();
    }

    public virtual void PlayControlled(int sourceID, Transform attachTrans, Transform directionTrans, bool play, DamageDeliverInfo buffInfo)
    {
        if (play)
        {
            SetDamageInfo(buffInfo);
            tf_ControlledAttach = attachTrans;
            tf_ControlledCast = directionTrans;
            base.Play(sourceID, 0f,F_DelayDuration);
        }
        else
        {
            tf_ControlledAttach = null;
            tf_ControlledCast = null;
            Stop();
        }
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

        if (tf_ControlledAttach != null)
        {
            transform.position = tf_ControlledAttach.position;
            transform.rotation = tf_ControlledAttach.rotation;
        }


        if (F_Tick <= 0)
            return;

        if (f_blastTickChest > 0)
        {
            f_blastTickChest -= Time.deltaTime;
            return;
        }
        DoBlastCheck();
        f_blastTickChest = F_Tick;
    }

    protected virtual void DoBlastCheck()
    {
        RaycastHit[] hits = OnCastCheck(GameLayer.Mask.I_Entity);
        List<int> targetHitted = new List<int>();
        for (int i = 0; i < hits.Length; i++)
        {
            HitCheckEntity entity = hits[i].collider.DetectEntity();
            if (entity!=null&&!targetHitted.Contains(entity.I_AttacherID)&&GameManager.B_CanDamageEntity(entity, m_sourceID))
            {
                targetHitted.Add(entity.I_AttacherID);
                OnDamageEntity(entity);
            }
        }
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
    protected virtual void OnDamageEntity(HitCheckEntity hitEntity)=>  hitEntity.TryHit(m_DamageInfo, Vector3.Normalize(hitEntity.transform.position - transform.position));

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
