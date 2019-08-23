using UnityEngine;
using GameSetting;
using System;
using System.Collections.Generic;

public class WeaponBase : MonoBehaviour {
    public enum_TriggerType E_Trigger = enum_TriggerType.Invalid;
    public enum_PlayerAnim E_Anim= enum_PlayerAnim.Invalid;
    public bool B_AttachLeft=false;


    SWeapon m_WeaponInfo;
    List<int> m_WeaponAction;
    EntityPlayerBase m_Attacher;
    public float F_BaseSpeed { get; private set; }
    public float F_BaseDamage { get; private set; }
    public int I_MuzzleIndex { get; private set; }

    public bool B_Triggerable { get; private set; } = false;
    public bool B_Reloading { get; private set; }
    public int I_AmmoLeft { get; private set; }
    public Transform m_Muzzle { get; private set; }
    public int I_ClipAmout { get; private set; }
    public float F_BaseFirerate => m_WeaponInfo.m_FireRate;
    public float F_Speed => m_Attacher.m_PlayerInfo.F_ProjectileSpeedMuiltiply * F_BaseSpeed;
    public Vector2 F_Recoil => m_Attacher.m_PlayerInfo.F_RecoilMultiply * m_WeaponInfo.m_RecoilPerShot;
    public float F_ReloadStatus => B_Reloading ? f_reloadCheck / m_WeaponInfo.m_ReloadTime : 1;
    public float F_AmmoStatus => I_AmmoLeft / (float)I_ClipAmout;
    public bool B_TriggerActionable() => B_Triggerable && B_Actionable();
    public bool B_Actionable() => !B_Reloading && f_fireCheck <= 0;

    WeaponTrigger m_Trigger = null;
    Action<bool,float> OnReload;
    Action<Vector2> OnFireRecoil;

    float f_reloadCheck, f_fireCheck;
    bool B_HaveAmmoLeft => m_WeaponInfo.m_ClipAmount == -1 || I_AmmoLeft > 0;
    bool B_AmmoFull => m_WeaponInfo.m_ClipAmount == -1||I_ClipAmout == I_AmmoLeft;
    protected void OnFireCheck(float pauseDuration) => f_fireCheck = pauseDuration;

    public void Init(SWeapon weaponInfo)
    {
        m_Muzzle = transform.FindInAllChild("Muzzle");
        m_WeaponInfo = weaponInfo;
        SFXProjectile projectileInfo = ObjectManager.GetEquipmentData<SFXProjectile>(m_WeaponInfo.m_Index);
        F_BaseSpeed = projectileInfo.F_Speed;
        F_BaseDamage = projectileInfo.F_Damage;
        I_MuzzleIndex = projectileInfo.I_MuzzleIndex;
        I_ClipAmout = m_WeaponInfo.m_ClipAmount;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        switch (E_Trigger)
        {
            default: Debug.LogError("Add More Convertions Here:" + E_Trigger); m_Trigger = new TriggerSingle(m_WeaponInfo.m_FireRate, m_WeaponInfo.m_SpecialRate, FireOnce, B_TriggerActionable, OnFireCheck, CheckCanAutoReload); break;
            case enum_TriggerType.Auto: m_Trigger = new TriggerAuto(m_WeaponInfo.m_FireRate, m_WeaponInfo.m_SpecialRate, FireOnce, B_TriggerActionable, OnFireCheck,CheckCanAutoReload);break;
            case enum_TriggerType.Single:m_Trigger = new TriggerSingle(m_WeaponInfo.m_FireRate, m_WeaponInfo.m_SpecialRate, FireOnce, B_TriggerActionable, OnFireCheck, CheckCanAutoReload);break;
            case enum_TriggerType.Burst:m_Trigger = new TriggerBurst(m_WeaponInfo.m_FireRate,m_WeaponInfo.m_SpecialRate, FireOnce, B_TriggerActionable,OnFireCheck, CheckCanAutoReload);break;
            case enum_TriggerType.Pull: m_Trigger = new TriggerPull(()=> { Debug.Log("Pull"); },m_WeaponInfo.m_FireRate,m_WeaponInfo.m_SpecialRate, FireOnce, B_TriggerActionable, OnFireCheck, CheckCanAutoReload); break;
            case enum_TriggerType.Store:m_Trigger = new TriggerStore(m_WeaponInfo.m_FireRate, m_WeaponInfo.m_SpecialRate, FireOnce, B_TriggerActionable, OnFireCheck, CheckCanAutoReload);break;
        }
    }
    protected virtual void Start()
    {
        if (m_WeaponInfo.m_Weapon == 0)
            Debug.LogError("Please Init Entity Info!" + gameObject.name.ToString());
    }
    protected virtual void OnDisable()
    {;
        B_Reloading = false;
        f_reloadCheck = 0;
        f_fireCheck = 0;
        m_Trigger.OnDisable();
    }
    public void Attach(EntityPlayerBase _attacher,Transform _attachTo,Action<Vector2> _OnFireRecoil,Action<bool,float> _OnReload)
    {
        m_Attacher = _attacher;
        transform.SetParent(_attachTo);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        OnFireRecoil = _OnFireRecoil;
        OnReload = _OnReload;
    }
    public void Detach()
    {
        ObjectManager.RecycleWeapon(m_WeaponInfo.m_Weapon,this);
    }
    public bool Trigger(bool down)
    {
        m_Trigger.OnSetTrigger(down);
        return true;
    }

    public void SetCanFire(bool _canFire)
    {
        B_Triggerable = _canFire;
    }
    RaycastHit hit;
    protected virtual bool FireOnce()
    {
        if (!B_HaveAmmoLeft)
            return false;

        DamageDeliverInfo damageInfo = m_Attacher.m_PlayerInfo.GetDamageBuffInfo();
        for (int i = 0; i < m_WeaponInfo.m_PelletsPerShot; i++)
        {
            Vector3 spreadDirection = GameExpression.V3_RangeSpreadDirection(m_Attacher.tf_Head.forward, m_WeaponInfo.m_Spread, m_Attacher.tf_Head.up, m_Attacher.tf_Head.right);
            Vector3 endPosition = m_Attacher.tf_Head.position + spreadDirection * GameConst.I_ProjectileMaxDistance;
            if (Physics.Raycast(m_Attacher.tf_Head.position, spreadDirection, out hit, GameConst.I_ProjectileMaxDistance, GameLayer.Mask.I_All) &&  GameManager.B_DoHitCheck(hit.collider.Detect(),m_Attacher.I_EntityID))
                endPosition = hit.point;
            spreadDirection = (endPosition - m_Muzzle.position).normalized;

            SFXProjectile projectile = ObjectManager.SpawnEquipment<SFXProjectile>(m_WeaponInfo.m_Index, m_Muzzle.position, spreadDirection);
            projectile.F_Speed = F_Speed;
            projectile.Play(damageInfo, spreadDirection, endPosition);
        }

        if (I_MuzzleIndex != -1)
            ObjectManager.SpawnParticles<SFXMuzzle>(I_MuzzleIndex, m_Muzzle.position, m_Muzzle.forward).Play(m_Attacher.I_EntityID);

        I_AmmoLeft--;
        OnFireRecoil?.Invoke(F_Recoil);

        return true;
    }

    void CheckCanAutoReload()
    {
        if (B_Reloading || B_HaveAmmoLeft)
            return;

        StartReload();
    }
    public bool TryReload()
    {
        if (!B_Actionable()||B_AmmoFull)
            return false;
        StartReload();
        return true;
    }
    void StartReload()
    {
        B_Reloading = true;
        f_reloadCheck = 0;
        OnReload?.Invoke(true, m_WeaponInfo.m_ReloadTime  / m_Attacher.m_PlayerInfo.F_ReloadRateTick(1f) );
    }

    protected void Update()
    {
        DataUpdate();

        float fireTick = m_Attacher.m_PlayerInfo.F_FireRateTick(Time.deltaTime);

        if (m_Trigger != null)
            m_Trigger.Tick(fireTick);

        if (f_fireCheck > 0)
            f_fireCheck -= fireTick;

        if (B_Reloading)
        {
            f_reloadCheck += m_Attacher.m_PlayerInfo.F_ReloadRateTick(Time.deltaTime);
            if (f_reloadCheck > m_WeaponInfo.m_ReloadTime)
            {
                I_AmmoLeft = I_ClipAmout;
                B_Reloading = false;
                OnReload(false, 0);
            }
        }

    }

    void DataUpdate()
    {
        int clipAmount = m_Attacher.m_PlayerInfo.I_ClipAmount(m_WeaponInfo.m_ClipAmount);
        if (I_ClipAmout != clipAmount)
        {
            I_ClipAmout = clipAmount;
            if (I_AmmoLeft > I_ClipAmout)
                I_AmmoLeft = I_ClipAmout;
        }
    }
    #region TriggerType
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
