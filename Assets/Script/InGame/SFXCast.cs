using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCast : SFXBase,ISingleCoroutine {
    public float F_Damage;
    public int I_TickCount=1;
    public float F_Tick = .5f;
    public int I_BuffApplyOnCast;
    public enum_CastAreaType E_CastType = enum_CastAreaType.Invalid;
    public Vector3 V3_CastSize;
    public int F_DelayDuration;
    public int I_DelayIndicatorIndex;

    ParticleSystem[] m_Particles;
    protected DamageInfo m_DamageInfo;
    protected virtual float F_ParticleDuration => 5f;
    protected virtual float F_CastLength => V3_CastSize.z;
    public bool B_Casting { get; private set; } = false;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        m_Particles.Traversal((ParticleSystem particle)=> {particle.Stop();});
        m_DamageInfo = new DamageInfo(F_Damage, enum_DamageType.Area);

        if (E_CastType == enum_CastAreaType.Invalid)
            Debug.LogError("Cast Type Invalid Detected:"+gameObject.name);
        if (V3_CastSize == Vector3.zero)
            Debug.LogError("Cast Size Zero Detected:" + gameObject.name);
        if (F_Damage <= 0 && I_BuffApplyOnCast <= 0)
            Debug.LogError("None Damage Or BuffApply Detected:" + gameObject.name);
        if (I_TickCount <= 0)
            Debug.LogError("Tick Count Less Or Equals Zero:" + gameObject.name);
        if (I_TickCount > 1 && F_Tick <= 0)
            Debug.LogError("Tick Duration Less Or Equals Zero:" + gameObject.name);
        if (F_DelayDuration > 0 && I_DelayIndicatorIndex <= 0)
            Debug.LogError("Delay Indicator Less Or Equals Zero:" + gameObject.name);
    }
    public void Play(int sourceID,DamageBuffInfo buffInfo)
    {
        if (F_DelayDuration <= 0)
        {
            PlayDelayed(sourceID,buffInfo);
            return;
        }
        ObjectManager.SpawnSFX<SFXIndicator>(I_DelayIndicatorIndex, transform.position, Vector3.up).PlayDuration(sourceID, transform.position, Vector3.up, F_DelayDuration);
        this.StartSingleCoroutine(1, TIEnumerators.PauseDel(F_DelayDuration, () => { PlayDelayed(sourceID, buffInfo); }));
    }
    public virtual void PlayDelayed(int sourceID, DamageBuffInfo buffInfo)
    {
        PlaySFX(sourceID, I_TickCount * F_Tick + 5f);
        B_Casting = true;
        if (I_BuffApplyOnCast > 0)
            buffInfo.m_BuffAplly.Add(I_BuffApplyOnCast);
        m_DamageInfo.ResetBuff(buffInfo);

        this.StartSingleCoroutine(0, TIEnumerators.TickCount(OnBlast, I_TickCount, F_Tick, () => {
            m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
            B_Casting = false;
        }));
    }

    public virtual void PlayControlled(int sourceID, Transform attachTo, bool play)
    {
        B_Casting = play;
        if (play)
        {
            transform.SetParent(attachTo);
            this.StartSingleCoroutine(0, TIEnumerators.Tick(OnBlast, F_Tick));
            PlaySFX(sourceID, int.MaxValue);
        }
        else
        {
            this.StopSingleCoroutine(0);
            f_TimeCheck = Time.time + F_ParticleDuration;
            m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
        }
    }
    protected void OnDisable()
    {
        this.StopAllCoroutines();
    }
    protected virtual void OnBlast()
    {
        TCommon.Traversal(m_Particles, (ParticleSystem particle) => { particle.Play(); });
        RaycastHit[] hits = OnCastCheck();
        List<int> targetHitted = new List<int>();
        for (int i = 0; i < hits.Length; i++)
        {
            HitCheckEntity entity = hits[i].collider.DetectEntity();
            if (entity!=null&&!targetHitted.Contains(entity.I_AttacherID)&&GameManager.B_CanDamageEntity(entity, I_SourceID))
            {
                targetHitted.Add(entity.I_AttacherID);
                OnDamageEntity(entity);
            }
        }
    }
    protected RaycastHit[] OnCastCheck()
    {
        RaycastHit[] hits=null;
        switch (E_CastType)
        {
            case enum_CastAreaType.OverlapSphere:
                {
                    float radius = V3_CastSize.x;
                  hits= Physics.SphereCastAll(transform.position, radius,transform.forward,0f, GameLayer.Physics.I_EntityOnly);
                }
                break;
            case enum_CastAreaType.ForwardCapsule:
                {
                    float radius = V3_CastSize.x;
                    float castLength = F_CastLength - radius*2;
                    castLength = castLength > 0 ? castLength : 0f;
                    hits = Physics.SphereCastAll(transform.position+transform.forward * radius, radius, transform.forward, castLength, GameLayer.Physics.I_EntityOnly);
                }
                break;
            case enum_CastAreaType.ForwardBox:
                {
                   hits = Physics.BoxCastAll(transform.position + transform.forward*1f /2f, new Vector3(V3_CastSize.x / 2, V3_CastSize.y / 2, .5f), transform.forward, Quaternion.LookRotation(transform.forward, transform.up), F_CastLength - 1f, GameLayer.Physics.I_EntityOnly);
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
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_GameDebugGizmos)
            return;
        Gizmos.color = GetGizmosColor();
        Gizmos_Extend.DrawArrow(transform.position,Quaternion.LookRotation(transform.forward),new Vector3(V3_CastSize.x/10,V3_CastSize.y/10,F_CastLength / 4));
        switch (E_CastType)
        {
            case enum_CastAreaType.OverlapSphere:
                    Gizmos.DrawWireSphere(transform.position, V3_CastSize.x);
                break;
            case enum_CastAreaType.ForwardBox:
                    Gizmos_Extend.DrawWireCube(transform.position + transform.forward * F_CastLength / 2, Quaternion.LookRotation(transform.forward), V3_CastSize);
                break;
            case enum_CastAreaType.ForwardCapsule:
                Gizmos_Extend.DrawWireCapsule(transform.position+transform.forward* F_CastLength / 2,Quaternion.LookRotation(transform.up,transform.forward),Vector3.one,V3_CastSize.x, F_CastLength);
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
