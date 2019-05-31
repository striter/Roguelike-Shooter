using UnityEngine;
using GameSetting;
using System;
using System.Collections.Generic;

public class WeaponBase : MonoBehaviour,ISingleCoroutine {
    public int I_AttacherID { get; private set; }
    public SWeapon m_WeaponInfo { get; private set; }
    public bool B_Reloading { get; private set; }
    public int I_AmmoLeft { get; private set; }
    float f_actionCheck=0;
    protected Transform tf_Muzzle;
    Action<float> OnAmmoChangeCostMana;
    Action<Vector2> OnRecoil;
    WeaponTrigger m_Trigger=null;
    WeaponAimAssist m_Assist = null;
    bool B_HaveAmmoLeft => m_WeaponInfo.m_ClipAmount == -1 || I_AmmoLeft > 0;
    public void Init(SWeapon weaponInfo)
    {
        tf_Muzzle = transform.Find("Muzzle");
        m_WeaponInfo = weaponInfo;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        m_Assist = new WeaponAimAssist(tf_Muzzle,GameConst.I_AimAssistCurveCount,GameConst.I_NormalBulletLastTime,weaponInfo);
        switch (weaponInfo.m_TriggerType)
        {
            default: Debug.LogError("Add More Convertions Here:" + weaponInfo.m_TriggerType.ToString()); m_Trigger = new TriggerSingle(m_WeaponInfo.m_FireRate, m_WeaponInfo.m_SpecialRate, FireOnce, CheckCanAction, SetActionPause, CheckCanAutoReload); break;
            case enum_TriggerType.Auto: m_Trigger = new TriggerAuto(m_WeaponInfo.m_FireRate, m_WeaponInfo.m_SpecialRate, FireOnce, CheckCanAction, SetActionPause,CheckCanAutoReload);break;
            case enum_TriggerType.Single:m_Trigger = new TriggerSingle(m_WeaponInfo.m_FireRate, m_WeaponInfo.m_SpecialRate, FireOnce, CheckCanAction, SetActionPause, CheckCanAutoReload);break;
            case enum_TriggerType.Burst:m_Trigger = new TriggerBurst(m_WeaponInfo.m_FireRate,m_WeaponInfo.m_SpecialRate, FireOnce, CheckCanAction,SetActionPause, CheckCanAutoReload);break;
            case enum_TriggerType.Pull: m_Trigger = new TriggerPull(()=> { Debug.Log("Pull"); },m_WeaponInfo.m_FireRate,m_WeaponInfo.m_SpecialRate, FireOnce, CheckCanAction, SetActionPause, CheckCanAutoReload); break;
            case enum_TriggerType.Store:m_Trigger = new TriggerStore(m_WeaponInfo.m_FireRate, m_WeaponInfo.m_SpecialRate, FireOnce, CheckCanAction, SetActionPause, CheckCanAutoReload);break;
        }
    }
    protected virtual void Start()
    {
        if (m_WeaponInfo.m_Weapon == 0)
            Debug.LogError("Please Init Entity Info!" + gameObject.name.ToString());
    }
    protected void Update()
    {
        if(m_Trigger!=null)
        m_Trigger.Tick(Time.deltaTime);

        m_Assist.Simulate();
    }
    protected virtual void OnDisable()
    {
        B_Reloading = false;
        m_Trigger.OnDisable();
        this.StopAllSingleCoroutines();
    }
    protected void SetActionPause(float pauseDuration)
    {
        f_actionCheck = Time.time + pauseDuration;
    }
    protected bool CheckCanAction()
    {
        return Time.time > f_actionCheck;
    }

    public void Attach(int _attacherID,Transform attachTarget,Action<float> _OnAmmoChangeCostMana,Action<Vector2> _OnRecoil)
    {
        I_AttacherID = _attacherID;
        transform.SetParent(attachTarget);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        OnAmmoChangeCostMana = _OnAmmoChangeCostMana;
        OnRecoil = _OnRecoil;
    }
    public bool Trigger(bool down)
    {
        if (B_Reloading)
            return false;
        if (!B_HaveAmmoLeft)
            return false;

        m_Trigger.OnSetTrigger(down);

        return true;
    }

    protected virtual bool FireOnce()
    {
        if (!B_HaveAmmoLeft)
            return false;

        I_AmmoLeft--;
        for (int i = 0; i < m_WeaponInfo.m_PelletsPerShot; i++)
        {
            Vector3 bulletDirection = Vector3.Normalize(tf_Muzzle.transform.forward*GameConst.I_BulletSpeadAtDistance+UnityEngine.Random.Range(-1f,1f)*tf_Muzzle.up*m_WeaponInfo.m_Spread+ UnityEngine.Random.Range(-1f, 1f) * tf_Muzzle.right * m_WeaponInfo.m_Spread);
            (ObjectManager.SpawnSFX(m_WeaponInfo.m_BulletType.ToSFXType(), tf_Muzzle) as SFXBullet).Play(I_AttacherID,m_WeaponInfo.m_Damage, bulletDirection, m_WeaponInfo.m_HorizontalSpeed,m_WeaponInfo.m_HorizontalDrag,m_WeaponInfo.m_VerticalAcceleration);
        }
        OnRecoil?.Invoke(m_WeaponInfo.m_RecoilPerShot);
        OnAmmoChangeCostMana?.Invoke(m_WeaponInfo.m_ManaCost);

        return true;
    }



    public bool TryReload()
    {
        if (!CheckCanAction())
            return false;
        StartReload();
        return true;
    }
    void StartReload()
    {
        B_Reloading = true;
        SetActionPause(m_WeaponInfo.m_ReloadTime);
        this.StartSingleCoroutine(1,TIEnumerators.PauseDel(m_WeaponInfo.m_ReloadTime,OnReloadFinished));
    }
    void OnReloadFinished()
    {
        Debug.Log("Reload");
        B_Reloading = false;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        OnAmmoChangeCostMana?.Invoke(0f);
    }
    void CheckCanAutoReload()
    {
        if (B_Reloading || B_HaveAmmoLeft)
            return;

        StartReload();
    }
    #region TriggerType
    class WeaponAimAssist
    {
        Transform transform;
        Transform tf_Dot;
        LineRenderer m_lineRenderer;
        AimAssistSimulator m_Simulator;
        int m_CurveCount;
        List<Vector3> m_CurvePoints = new List<Vector3>();
        public WeaponAimAssist(Transform muzzle, int _curveCount,float duration,SWeapon weaponInfo)
        {
            transform = muzzle;
            tf_Dot = transform.Find("Dot");
            m_lineRenderer = muzzle.GetComponent<LineRenderer>();
            m_lineRenderer.positionCount = _curveCount;
            m_CurveCount = _curveCount;
            m_Simulator = new AimAssistSimulator(duration / _curveCount,muzzle.position, muzzle.forward, Vector3.down, weaponInfo.m_HorizontalSpeed, -weaponInfo.m_HorizontalDrag, 0, weaponInfo.m_VerticalAcceleration, false);
            ;
        }
        public void Simulate()
        {
            m_Simulator.ResetSimulator(transform.position,transform.forward);
            m_CurvePoints.Clear();
            m_CurvePoints.Add(transform.position);
            tf_Dot.SetActivate(false);
            for (int i = 0; i < m_CurveCount; i++)
            {
                Vector3 lookDirection;float offset;
                Vector3 currentPosition = m_Simulator.Next(out lookDirection, out offset);
                RaycastHit hit;
                if (Physics.Raycast(currentPosition, lookDirection, out hit, offset,GameLayer.Physics.I_Static))
                {
                    m_CurvePoints.Add(hit.point);
                    tf_Dot.SetActivate(true);
                    tf_Dot.position = hit.point;
                    break;
                }
                m_CurvePoints.Add(currentPosition);
            }
            m_lineRenderer.positionCount = m_CurvePoints.Count;
            m_lineRenderer.SetPositions(m_CurvePoints.ToArray());
        }
    }
    internal class WeaponTrigger:ISingleCoroutine
    {
        public bool B_TriggerDown { get; protected set; }
        protected float f_fireRate { get; private set; }
        protected Func<bool> OnTriggerSuccessful { get; private set; }
        protected Func<bool> OnTriggerActionable { get; private set; }
        private Action<float> OnSetActionPause;
        private Action OnCheckAutoReload;
        public WeaponTrigger(float _fireRate,float _specialRate, Func<bool> _OnTriggerSuccessful,Func<bool> _OnTriggerActionable, Action<float> _OnSetActionPause,Action _OnCheckAutoReload)
        {
            f_fireRate = _fireRate;
            OnTriggerSuccessful = _OnTriggerSuccessful;
            OnTriggerActionable = _OnTriggerActionable;
            OnSetActionPause = _OnSetActionPause;
            OnCheckAutoReload = _OnCheckAutoReload;
        }

        public virtual void OnSetTrigger(bool down)
        {
            B_TriggerDown = down;
        }
        public virtual void Tick(float deltaTime)
        {

        }
        public virtual void OnDisable()
        {
            B_TriggerDown = false;
            this.StopSingleCoroutines(0,1);
        }
        protected void OnActionPause(float pauseDuration,bool autoReload,Action actionAfterPause=null)
        {
            if(pauseDuration!=0)
                 OnSetActionPause(pauseDuration);
            
            if(autoReload)
                this.StartSingleCoroutine(0, TIEnumerators.PauseDel(pauseDuration, OnCheckAutoReload));

            if (actionAfterPause != null)
                this.StartSingleCoroutine(1, TIEnumerators.PauseDel(pauseDuration, actionAfterPause));
        }
    }

    internal class TriggerSingle : WeaponTrigger
    {
        public TriggerSingle(float _fireRate, float _specialRate, Func<bool> _OnTriggerSuccessful, Func<bool> _OnTriggerActionable, Action<float> _OnSetActionPause, Action OnCheckAutoReload) : base(_fireRate,_specialRate, _OnTriggerSuccessful, _OnTriggerActionable,_OnSetActionPause, OnCheckAutoReload)
        {
        }
        public override void OnSetTrigger(bool down)
        {
            base.OnSetTrigger(down);
            if (B_TriggerDown && OnTriggerActionable())
            {
                OnTriggerSuccessful();
                OnActionPause(f_fireRate,true);
            }
        }
    }

    internal class TriggerAuto : WeaponTrigger
    {
        public TriggerAuto(float _fireRate, float _specialRate, Func<bool> _OnTriggerSuccessful, Func<bool> _OnTriggerActionable, Action<float> _OnSetActionPause, Action _OnCheckAutoReload) : base(_fireRate, _specialRate, _OnTriggerSuccessful, _OnTriggerActionable, _OnSetActionPause, _OnCheckAutoReload)
        {
        }
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (B_TriggerDown && OnTriggerActionable())
            {
                B_TriggerDown=OnTriggerSuccessful();
                OnActionPause(f_fireRate, true);
            }
        }
    }

    internal class TriggerBurst : WeaponTrigger
    {
        float f_burstTime;
        float f_burstRate;
        bool b_bursting;
        int i_burstIndex;
        public TriggerBurst(float _fireRate, float _specialRate, Func<bool> _OnTriggerSuccessful, Func<bool> _OnTriggerActionable, Action<float> _OnSetActionPause, Action _OnCheckAutoReload) : base(_fireRate, _specialRate, _OnTriggerSuccessful, _OnTriggerActionable, _OnSetActionPause, _OnCheckAutoReload)
        {
            f_burstRate = _specialRate;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            b_bursting = false;
        }
        public override void OnSetTrigger(bool down)
        {
            base.OnSetTrigger(down);
            if (!b_bursting && B_TriggerDown && OnTriggerActionable())
            {
                OnActionPause(f_fireRate,true);
                b_bursting = true;
                i_burstIndex = 0;
                f_burstTime = 0;
            }
        }
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (b_bursting)
            {
                f_burstTime += deltaTime;
                if (f_burstTime > f_burstRate)
                {
                    f_burstTime -= f_burstRate;
                    i_burstIndex++;
                    OnTriggerSuccessful();
                    if (i_burstIndex >= GameConst.I_BurstFirePelletsOnceTrigger)
                    {
                        b_bursting = false;
                    }
                }
            }
        }
    }


    internal class TriggerPull : WeaponTrigger,ISingleCoroutine
    {
        float f_pullTime;
        float f_pullDuration;
        public bool B_Pulling { get; private set; } = false;
        public bool B_NeedPull { get; private set; } = false;
        Action OnPull;
        public TriggerPull(Action _OnPull, float _fireRate, float _specialRate, Func<bool> _OnTriggerSuccessful, Func<bool> _OnTriggerActionable, Action<float> _OnSetActionPause, Action _OnCheckAutoReload) : base(_fireRate, _specialRate, _OnTriggerSuccessful, _OnTriggerActionable, _OnSetActionPause, _OnCheckAutoReload)
        {
            OnPull = _OnPull;
            f_pullDuration = _specialRate;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            B_Pulling = false;
        }
        public override void OnSetTrigger(bool down)
        {
            base.OnSetTrigger(down);
            if (down && OnTriggerActionable())
            {
                if (B_NeedPull)
                {
                    OnPull();
                    OnActionPause(f_pullDuration,true);
                    B_Pulling = true;
                    f_pullTime = 0;
                }
                else
                {
                    OnTriggerSuccessful();
                    OnActionPause(f_fireRate,false,()=> { OnSetTrigger(true); });
                    B_NeedPull = true;
                }
            }
        }
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (B_Pulling&&B_NeedPull)
            {
                f_pullTime += deltaTime;
                if (f_pullTime > f_pullDuration)
                {
                    B_NeedPull = false;
                    B_Pulling = false;
                }
            }
        }
    }

    internal class TriggerStore : WeaponTrigger
    {
        float f_storeTime;
        float f_minStoreTime,f_maxStoreTime;
        public bool B_Storing { get; private set; }
        public TriggerStore(float _fireRate, float _specialRate, Func<bool> _OnTriggerSuccessful, Func<bool> _OnTriggerActionable, Action<float> _OnSetActionPause, Action _OnCheckAutoReload) : base(_fireRate, _specialRate, _OnTriggerSuccessful, _OnTriggerActionable, _OnSetActionPause, _OnCheckAutoReload)
        {
            f_minStoreTime = _fireRate;
            f_maxStoreTime = _specialRate;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            B_Storing = false;
        }

        public override void OnSetTrigger(bool down)
        {
            base.OnSetTrigger(down);
            if (OnTriggerActionable())
            {
                if (!B_TriggerDown && f_storeTime >= f_minStoreTime)
                    OnTriggerSuccessful();

                B_Storing = B_TriggerDown;
                f_storeTime = 0f;
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (B_Storing)
            {
                f_storeTime += deltaTime;

                //Release If Above Max Time
                if (f_storeTime >= f_maxStoreTime)
                {
                    OnTriggerSuccessful();
                    B_Storing = false;
                    f_storeTime = 0f;
                }
            }
        }
    }
    #endregion

}
