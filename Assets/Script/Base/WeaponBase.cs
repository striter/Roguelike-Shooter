﻿using UnityEngine;
using GameSetting;
using System;

public class WeaponBase : CObjectPoolMono<enum_PlayerWeapon>
{ 
    public enum_PlayerAnim E_Anim= enum_PlayerAnim.Invalid;
    public bool B_AttachLeft=false;
    protected EntityCharacterPlayer m_Attacher { get; private set; }
    public SWeapon m_WeaponInfo { get; private set; }
    public float F_BaseDamage { get; protected set; } = 0;
    public float F_BaseRecoil => m_WeaponInfo.m_RecoilPerShot;
    public float F_BaseFirerate => m_WeaponInfo.m_FireRate;
    public int I_AmmoLeft { get; private set; } = 0;
    public Transform m_Muzzle { get; private set; } = null;
    public Transform m_Case { get; private set; } = null;
    public int I_ClipAmount { get; private set; } = 0;
    public float F_Recoil => m_Attacher.m_CharacterInfo.F_SpreadMultiply * F_BaseRecoil;
    protected WeaponTrigger m_Trigger { get; private set; }
    Action<float> OnFireRecoil;
    
    TimeCounter m_ReloadTimer=new TimeCounter(),m_ReloadPauseTimer=new TimeCounter(GameConst.F_PlayerWeaponFireReloadPause);
    public float F_AmmoStatus => I_AmmoLeft / (float)I_ClipAmount;
    public bool m_HaveAmmoLeft => m_WeaponInfo.m_ClipAmount == -1 || I_AmmoLeft > 0;
    public bool B_AmmoFull => m_WeaponInfo.m_ClipAmount == -1||I_ClipAmount == I_AmmoLeft;
    
    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity,_OnRecycle);
        m_Muzzle = transform.FindInAllChild("Muzzle");
        m_Case = transform.FindInAllChild("Case");
        m_WeaponInfo = GameDataManager.GetWeaponProperties(_identity);
        I_ClipAmount = m_WeaponInfo.m_ClipAmount;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        m_Trigger = new WeaponTrigger(m_WeaponInfo.m_FireRate, OnTriggerOnce);
        m_ReloadTimer.SetTimer(m_WeaponInfo.m_ReloadTime);
        OnGetEquipmentData(GameObjectManager.GetEquipmentData<SFXWeaponBase>(GameExpression.GetPlayerWeaponIndex(m_WeaponInfo.m_Index)));
    }

    protected virtual void OnGetEquipmentData(SFXWeaponBase equipment)
    {
    }

    public void OnAttach(EntityCharacterPlayer _attacher,Transform _attachTo,Action<float> _OnFireRecoil)
    {
        m_Attacher = _attacher;
        transform.SetParentResetTransform(_attachTo);
        OnFireRecoil = _OnFireRecoil;
        OnShow(true);
    }

    public virtual void OnDetach()
    {
        m_Attacher = null;
        OnShow(true);
    }

    public virtual void OnPlay(bool play)
    {
        OnShow(play);

        if (play)
            return;
        
        Trigger(false);
    }

    void OnShow(bool show)=>transform.SetActivate(show);
    #region PlayerInteract
    public void Trigger(bool down)=>m_Trigger.OnSetTrigger(down);
    
    protected bool OnTriggerOnce()
    {
        if (!m_HaveAmmoLeft)
            return false;
        OnFireRecoil?.Invoke(F_Recoil);
        m_ReloadPauseTimer.Reset();
        I_AmmoLeft--;
        OnTriggerSuccessful();
        return true;
    }
    protected virtual void OnTriggerSuccessful()
    {
    }
    
    public void Tick(float fireTick,float reloadTick)
    {
        m_Trigger.Tick(fireTick);
        ReloadTick(reloadTick);

        int clipAmount = m_Attacher.m_CharacterInfo.I_ClipAmount(m_WeaponInfo.m_ClipAmount);
        if (I_ClipAmount != clipAmount)
        {
            I_ClipAmount = clipAmount;
            if (I_AmmoLeft > I_ClipAmount)
                I_AmmoLeft = I_ClipAmount;
        }

    }
    void ReloadTick(float deltaTime)
    {
        m_ReloadPauseTimer.Tick(deltaTime);
        if (m_ReloadPauseTimer.m_Timing)
            return;

        m_ReloadTimer.Tick(deltaTime);
        if (m_ReloadTimer.m_Timing)
            return;

        if (B_AmmoFull)
            return;

        I_AmmoLeft++;
        m_ReloadTimer.Reset();
    }
    
    public void ForceReload()
    {
        I_AmmoLeft = I_ClipAmount;
    }
    #endregion
    public class WeaponTrigger
    {
        public bool B_TriggerDown { get; protected set; }
        protected Func<bool> OnTriggerSuccessful { get; private set; }
        TimeCounter m_TriggerTimer = new TimeCounter();
        public WeaponTrigger(float _fireRate, Func<bool> _OnTriggerSuccessful)
        {
            OnTriggerSuccessful = _OnTriggerSuccessful;
            m_TriggerTimer.SetTimer(_fireRate);
        }

        public virtual void OnSetTrigger(bool down)
        {
            B_TriggerDown = down;
        }

        public virtual void Tick(float deltaTime)
        {
            m_TriggerTimer.Tick(deltaTime);
            if (m_TriggerTimer.m_Timing)
                return;


            if (!B_TriggerDown)
                return;

            if (OnTriggerSuccessful())
                m_TriggerTimer.Reset();
        }
        
    }
    
    public void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType)
    {
    }
}
