using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCast : SFXParticles,ISingleCoroutine {
    public enum_CastControllType E_CastType = enum_CastControllType.Invalid;
    public float F_Damage;
    public int I_TickCount=1;
    public float F_Tick = .5f;
    public int I_BuffApplyOnCast;
    public enum_CastAreaType E_AreaType = enum_CastAreaType.Invalid;
    public Vector4 V4_CastInfo;
    public int F_DelayDuration;
    public int I_DelayIndicatorIndex;
    public bool B_CameraShake = false;
    public int I_ShakeAmount = 0;
    protected DamageInfo m_DamageInfo;
    public int m_sourceID => m_DamageInfo.m_detail.I_SourceID;
    protected virtual float F_ParticleDuration => 5f;
    protected virtual float F_CastLength => V4_CastInfo.z;
    public bool B_Casting { get; private set; } = false;
    protected override bool B_PlayOnAwake => false;
    protected Transform CastTransform => tf_ControlledCast ? tf_ControlledCast : transform;
    Transform tf_ControlledAttach,tf_ControlledCast;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        if (E_CastType == enum_CastControllType.Invalid)
            Debug.LogError("Weapon Type Invalid Detected+"+gameObject.name);
        if (E_AreaType == enum_CastAreaType.Invalid)
            Debug.LogError("Cast Type Invalid Detected:"+gameObject.name);
        if (V4_CastInfo == Vector4.zero)
            Debug.LogError("Cast Size Zero Detected:" + gameObject.name);
        if (I_TickCount <= 0)
            Debug.LogError("Tick Count Less Or Equals Zero:" + gameObject.name);
        if (I_TickCount > 1 && F_Tick <= 0)
            Debug.LogError("Tick Duration Less Or Equals Zero:" + gameObject.name);
        if (F_DelayDuration > 0 && I_DelayIndicatorIndex < 0)
            Debug.LogError("Delay Indicator Less Than Zero:" + gameObject.name);
    }
    public void Play(DamageDeliverInfo buffInfo)
    {
        SetDamageInfo(buffInfo);
        if (F_DelayDuration <= 0)
        {
            PlayDelayed();
            return;
        }
        if (I_DelayIndicatorIndex>0)
            ObjectManager.SpawnIndicator(I_DelayIndicatorIndex, transform.position, Vector3.up).Play(m_sourceID,  F_DelayDuration);
        this.StartSingleCoroutine(1, TIEnumerators.PauseDel(F_DelayDuration, PlayDelayed));
    }
    public virtual void PlayDelayed()
    {
        PlaySFX(m_DamageInfo.m_detail.I_SourceID, I_TickCount * F_Tick + 5f);
        B_Casting = true;
        if (B_CameraShake)
            TPSCameraController.Instance.AddShake(I_ShakeAmount);

        PlayParticles();
        this.StartSingleCoroutine(0, TIEnumerators.TickCount(DoBlastCheck, I_TickCount, F_Tick, () => {
            StopParticles();
            B_Casting = false;
        }));
    }
    public virtual void PlayControlled(int sourceID, Transform attachTrans, Transform directionTrans, bool play, DamageDeliverInfo buffInfo)
    {
        B_Casting = play;
        if (play)
        {
            SetDamageInfo(buffInfo);
            PlayParticles();
            tf_ControlledAttach = attachTrans;
            tf_ControlledCast = directionTrans;
            this.StartSingleCoroutine(0, TIEnumerators.Tick(DoBlastCheck, F_Tick));
            PlaySFX(sourceID, int.MaxValue);
        }
        else
        {
            StopParticles();
            tf_ControlledAttach = null;
            tf_ControlledCast = null;
            this.StopSingleCoroutine(0);
        }
    }
    void SetDamageInfo(DamageDeliverInfo info)
    {
        if (I_BuffApplyOnCast > 0)
            info.m_BuffAplly.Add(I_BuffApplyOnCast);
        m_DamageInfo = new DamageInfo(F_Damage, enum_DamageType.Common, info);
    }
    protected override void Update()
    {
        base.Update();
        if (tf_ControlledAttach != null)
        {
            transform.position = tf_ControlledAttach.position;
            transform.rotation = tf_ControlledAttach.rotation;
        }
    }
    protected void OnDisable()
    {
        this.StopAllCoroutines();
    }
    protected virtual void DoBlastCheck()
    {
        RaycastHit[] hits = OnCastCheck();
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
    protected RaycastHit[] OnCastCheck()
    {
        RaycastHit[] hits=null;
        switch (E_AreaType)
        {
            case enum_CastAreaType.OverlapSphere:
                {
                    float radius = V4_CastInfo.x;
                    hits= Physics.SphereCastAll(CastTransform.position, radius, CastTransform.forward,0f, GameLayer.Mask.I_EntityOnly);
                }
                break;
            case enum_CastAreaType.ForwardCapsule:
                {
                    float radius = V4_CastInfo.x;
                    float castLength = F_CastLength - radius*2;
                    castLength = castLength > 0 ? castLength : 0f;
                    hits = Physics.SphereCastAll(CastTransform.position+ CastTransform.forward * radius, radius, CastTransform.forward, castLength, GameLayer.Mask.I_EntityOnly);
                }
                break;
            case enum_CastAreaType.ForwardBox:
                {
                    hits = Physics_Extend.BoxCastAll(CastTransform.position, CastTransform.forward,CastTransform.up, new Vector3(V4_CastInfo.x,V4_CastInfo.y,F_CastLength), GameLayer.Mask.I_EntityOnly);
                }
                break;
            case enum_CastAreaType.ForwardTrapezium:
                {
                    hits = Physics_Extend.TrapeziumCastAll(CastTransform.position,CastTransform.forward,CastTransform.up,V4_CastInfo,GameLayer.Mask.I_EntityOnly);
                }
                break;
        }
        return hits;
    }   
    protected virtual void OnDamageEntity(HitCheckEntity hitEntity)
    {
        hitEntity.TryHit(m_DamageInfo);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_PhysicsDebugGizmos)
            return;
        Gizmos.color = GetGizmosColor();
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
                Gizmos_Extend.DrawWireCapsule(CastTransform.position+transform.forward* F_CastLength / 2,Quaternion.LookRotation(CastTransform.up, CastTransform.forward),Vector3.one,V4_CastInfo.x, F_CastLength);
                break;
            case enum_CastAreaType.ForwardTrapezium:
                Gizmos_Extend.DrawTrapezium(CastTransform.position+transform.forward*F_CastLength/2,Quaternion.LookRotation(CastTransform.up,CastTransform.forward),V4_CastInfo);
                break;
        }
    }
    protected Color GetGizmosColor()
    {
        Color color = Color.green;
        if (B_Casting)
            color = Color.red;
        if (!UnityEditor.EditorApplication.isPlaying)
            color = Color.yellow;
        return color;
    }
#endif
}
