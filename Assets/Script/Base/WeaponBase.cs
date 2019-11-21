﻿using UnityEngine;
using GameSetting;
using System;

public class WeaponBase : ObjectPoolMonoItem<enum_PlayerWeapon>
{ 
    public enum_PlayerAnim E_Anim= enum_PlayerAnim.Invalid;
    public bool B_AttachLeft=false;
    public AudioClip m_ReloadClip1, m_ReloadClip2, m_ReloadClip3;
    protected EntityCharacterPlayer m_Attacher { get; private set; }
    public SWeapon m_WeaponInfo { get; private set; }
    public ActionBase m_WeaponAction { get; private set; } = null;
    public float F_BaseDamage { get; protected set; } = 0;
    public float F_BaseRecoil => m_WeaponInfo.m_RecoilPerShot;
    public float F_BaseFirerate => m_WeaponInfo.m_FireRate;
    public bool B_Triggerable { get; private set; } = false;
    public bool B_Reloading { get; private set; } = false;
    public int I_AmmoLeft { get; private set; } = 0;
    public Transform m_Muzzle { get; private set; } = null;
    public Transform m_Case { get; private set; } = null;
    public int I_ClipAmount { get; private set; } = 0;
    public float F_Recoil => m_Attacher.m_PlayerInfo.F_RecoilMultiply * F_BaseRecoil;
    public bool B_TriggerActionable() => B_Triggerable && B_Actionable();
    public bool B_Actionable() => !B_Reloading && f_fireCheck <= 0;
    WeaponTrigger m_Trigger = null;
    Action<bool,float> OnReload;
    Action<float> OnFireRecoil;

    float f_reloadCheck, f_fireCheck;
    public float F_ReloadStatus => B_Reloading ? f_reloadCheck / m_WeaponInfo.m_ReloadTime : 1;
    public float F_AmmoStatus => I_AmmoLeft / (float)I_ClipAmount;
    bool B_HaveAmmoLeft => m_WeaponInfo.m_ClipAmount == -1 || I_AmmoLeft > 0;
    bool B_AmmoFull => m_WeaponInfo.m_ClipAmount == -1||I_ClipAmount == I_AmmoLeft;
    protected void OnFireCheck(float pauseDuration) => f_fireCheck = pauseDuration;

    EquipmentBase m_Equipment;
    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        m_Muzzle = transform.FindInAllChild("Muzzle");
        m_Case = transform.FindInAllChild("Case");
        m_WeaponInfo = GameDataManager.GetWeaponProperties(_identity);
        I_ClipAmount = m_WeaponInfo.m_ClipAmount;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        m_Trigger = new TriggerAuto(m_WeaponInfo.m_FireRate, OnTrigger, B_TriggerActionable, OnFireCheck, CheckCanAutoReload);
        OnGetEquipmentData(GameObjectManager.GetEquipmentData<SFXEquipmentBase>(GameExpression.GetPlayerEquipmentIndex(m_WeaponInfo.m_Index)));
    }

    protected virtual void OnGetEquipmentData(SFXEquipmentBase equipment)
    {
    }

    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        B_Reloading = false;
        f_reloadCheck = 0;
        f_fireCheck = 0;
        m_Trigger.OnDisable();
    }

    public void OnSpawn(ActionBase _weaponAction)
    {
        m_WeaponAction = _weaponAction;
    }

    public void OnAttach(EntityCharacterPlayer _attacher,Transform _attachTo,Action<float> _OnFireRecoil,Action<bool,float> _OnReload)
    {
        m_Attacher = _attacher;
        transform.SetParentResetTransform(_attachTo);
        OnFireRecoil = _OnFireRecoil;
        OnReload = _OnReload;
    }

    public void OnDetach()
    {
        m_Attacher = null;
    }

    #region PlayerInteract
    public bool Trigger(bool down)
    {
        m_Trigger.OnSetTrigger(down);
        return true;
    }
    
    protected virtual bool OnTrigger()
    {
        if (!B_HaveAmmoLeft)
            return false;
        OnFireRecoil?.Invoke(F_Recoil);
        I_AmmoLeft--;
        OnTriggerSuccessful();
        return true;
    }

    protected virtual void OnTriggerSuccessful()
    {

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

    public void Tick(float deltaTime,bool _canFire)
    {
        B_Triggerable = _canFire;
        
        AmmoStatus(deltaTime);

        float fireTick = m_Attacher.m_PlayerInfo.F_FireRateTick(deltaTime);

        if (m_Trigger != null)
            m_Trigger.Tick(fireTick);

        if (f_fireCheck > 0)
            f_fireCheck -= fireTick;

        if (B_Reloading)
        {
            f_reloadCheck += m_Attacher.m_PlayerInfo.F_ReloadRateTick(deltaTime);
            if (f_reloadCheck > m_WeaponInfo.m_ReloadTime)
            {
                B_Reloading = false;
                OnReload(false, 0);
                I_AmmoLeft = I_ClipAmount;
            }
        }
    }
    void AmmoStatus(float deltaTime)
    {
        int clipAmount = m_Attacher.m_PlayerInfo.I_ClipAmount(m_WeaponInfo.m_ClipAmount);
        if (I_ClipAmount != clipAmount)
        {
            I_ClipAmount = clipAmount;
            if (I_AmmoLeft > I_ClipAmount)
                I_AmmoLeft = I_ClipAmount;
        }
    }
    
    public void ForceReload()
    {
        I_AmmoLeft = I_ClipAmount;
        B_Reloading = false;
        OnReload(false, 0);
    }
    #endregion
    internal class WeaponTrigger:ISingleCoroutine
    {
        public bool B_TriggerDown { get; protected set; }
        protected float f_fireRate { get; private set; }
        protected Func<bool> OnTriggerSuccessful { get; private set; }
        protected Func<bool> OnTriggerActionable { get; private set; }
        private Action<float> OnSetActionPause;
        private Action OnCheckAutoReload;
        public WeaponTrigger(float _fireRate, Func<bool> _OnTriggerSuccessful,Func<bool> _OnTriggerActionable, Action<float> _OnSetActionPause,Action _OnCheckAutoReload)
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
            this.StopSingleCoroutines(0);
        }
        protected void OnActionPause(float pauseDuration,bool autoReload,Action ActionAfterPause=null)
        {
            if(pauseDuration!=0)
                 OnSetActionPause(pauseDuration);

            if (autoReload)
                OnCheckAutoReload();

            if (ActionAfterPause != null)
                this.StartSingleCoroutine(0, TIEnumerators.PauseDel(pauseDuration, ActionAfterPause));
        }
    }
    
    internal class TriggerAuto : WeaponTrigger
    {
        public TriggerAuto(float _fireRate, Func<bool> _OnTriggerSuccessful, Func<bool> _OnTriggerActionable, Action<float> _OnSetActionPause, Action _OnCheckAutoReload) : base(_fireRate, _OnTriggerSuccessful, _OnTriggerActionable, _OnSetActionPause, _OnCheckAutoReload)
        {
        }
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (B_TriggerDown && OnTriggerActionable())
            {
                if (OnTriggerSuccessful())
                    OnActionPause(f_fireRate,true);
            }
        }
    }
    
    public void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType)
    {
        AudioClip targetClip=null;
        switch (eventType)
        {
            case TAnimatorEvent.enum_AnimEvent.Reload1:targetClip = m_ReloadClip1; break;
            case TAnimatorEvent.enum_AnimEvent.Reload2: targetClip = m_ReloadClip2; break;
            case TAnimatorEvent.enum_AnimEvent.Reload3: targetClip = m_ReloadClip3; break;
        }
        if(targetClip)
             GameAudioManager.Instance.PlayClip(m_Attacher.m_EntityID, targetClip, false, m_Case);
    }
}
