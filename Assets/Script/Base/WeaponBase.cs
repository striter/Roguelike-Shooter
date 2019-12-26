using UnityEngine;
using GameSetting;
using System;

public class WeaponBase : ObjectPoolMonoItem<enum_PlayerWeapon>
{ 
    public enum_PlayerAnim E_Anim= enum_PlayerAnim.Invalid;
    public bool B_AttachLeft=false;
    public AudioClip m_ReloadClip1, m_ReloadClip2, m_ReloadClip3;
    protected EntityCharacterPlayer m_Attacher { get; private set; }
    public SWeapon m_WeaponInfo { get; private set; }
    public float F_BaseDamage { get; protected set; } = 0;
    public float F_BaseRecoil => m_WeaponInfo.m_RecoilPerShot;
    public float F_BaseFirerate => m_WeaponInfo.m_FireRate;
    public bool B_Reloading { get; private set; } = false;
    public int I_AmmoLeft { get; private set; } = 0;
    public Transform m_Muzzle { get; private set; } = null;
    public Transform m_Case { get; private set; } = null;
    public int I_ClipAmount { get; private set; } = 0;
    public float F_Recoil => m_Attacher.m_PlayerInfo.F_RecoilMultiply * F_BaseRecoil;
    protected WeaponTrigger m_Trigger { get; private set; }
    Action<bool,float> OnReload;
    Action<float> OnFireRecoil;

    float f_reloadCheck, f_fireCheck;
    public float F_ReloadStatus => B_Reloading ? f_reloadCheck / m_WeaponInfo.m_ReloadTime : 1;
    public float F_AmmoStatus => I_AmmoLeft / (float)I_ClipAmount;
    bool B_HaveAmmoLeft => m_WeaponInfo.m_ClipAmount == -1 || I_AmmoLeft > 0;
    bool B_AmmoFull => m_WeaponInfo.m_ClipAmount == -1||I_ClipAmount == I_AmmoLeft;
    protected void OnFireCheck(float pauseDuration) => f_fireCheck = pauseDuration;
    
    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity,_OnRecycle);
        m_Muzzle = transform.FindInAllChild("Muzzle");
        m_Case = transform.FindInAllChild("Case");
        m_WeaponInfo = GameDataManager.GetWeaponProperties(_identity);
        I_ClipAmount = m_WeaponInfo.m_ClipAmount;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        m_Trigger = new WeaponTrigger(m_WeaponInfo.m_FireRate, OnTriggerOnce, OnFireCheck, CheckCanAutoReload);
        OnGetEquipmentData(GameObjectManager.GetEquipmentData<SFXEquipmentBase>(GameExpression.GetPlayerEquipmentIndex(m_WeaponInfo.m_Index)));
    }

    protected virtual void OnGetEquipmentData(SFXEquipmentBase equipment)
    {
    }
    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        StopReload();
    }

    public void OnAttach(EntityCharacterPlayer _attacher,Transform _attachTo,Action<float> _OnFireRecoil,Action<bool,float> _OnReload)
    {
        m_Attacher = _attacher;
        transform.SetParentResetTransform(_attachTo);
        OnFireRecoil = _OnFireRecoil;
        OnReload = _OnReload;
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
        {
            CheckCanAutoReload();
            return;
        }

        B_Reloading = false;
        Trigger(false);
    }

    void OnShow(bool show)=>transform.SetActivate(show);
    #region PlayerInteract
    public void Trigger(bool down)=>m_Trigger.OnSetTrigger(down);
    
    protected bool OnTriggerOnce()
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
        if (B_Reloading || B_AmmoFull)
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
    void StopReload()
    {
        B_Reloading = false;
        f_reloadCheck = 0;
        f_fireCheck = 0;
    }

    public void AmmoTick(float deltaTime)
    {
        int clipAmount = m_Attacher.m_PlayerInfo.I_ClipAmount(m_WeaponInfo.m_ClipAmount);
        if (I_ClipAmount != clipAmount)
        {
            I_ClipAmount = clipAmount;
            if (I_AmmoLeft > I_ClipAmount)
                I_AmmoLeft = I_ClipAmount;
        }
    }

    public void ReloadTick(float deltaTime)
    {
        if (B_Reloading)
        {
            f_reloadCheck += deltaTime;
            if (f_reloadCheck > m_WeaponInfo.m_ReloadTime)
            {
                B_Reloading = false;
                OnReload(false, 0);
                I_AmmoLeft = I_ClipAmount;
            }
        }
    }
    public void FireTick(float deltaTime)
    {
        if (f_fireCheck > 0)
            f_fireCheck -= deltaTime;
        if (B_Reloading || f_fireCheck > 0)
            return;
        
        m_Trigger.Tick(deltaTime);
    }
    
    public void ForceReload()
    {
        I_AmmoLeft = I_ClipAmount;
        B_Reloading = false;
        OnReload(false, 0);
    }
    #endregion
    public class WeaponTrigger
    {
        public bool B_TriggerDown { get; protected set; }
        protected float f_fireRate { get; private set; }
        protected Func<bool> OnTriggerSuccessful { get; private set; }
        private Action<float> OnSetActionPause;
        private Action OnCheckAutoReload;
        public WeaponTrigger(float _fireRate, Func<bool> _OnTriggerSuccessful, Action<float> _OnSetActionPause,Action _OnCheckAutoReload)
        {
            f_fireRate = _fireRate;
            OnTriggerSuccessful = _OnTriggerSuccessful;
            OnSetActionPause = _OnSetActionPause;
            OnCheckAutoReload = _OnCheckAutoReload;
        }

        public virtual void OnSetTrigger(bool down)
        {
            B_TriggerDown = down;
        }

        public virtual void Tick(float deltaTime)
        {
            if (!B_TriggerDown)
                return;

            if (OnTriggerSuccessful())
                OnActionPause(f_fireRate, true);
        }

        protected void OnActionPause(float pauseDuration,bool autoReload)
        {
            if(pauseDuration!=0)
                 OnSetActionPause(pauseDuration);

            if (autoReload)
                OnCheckAutoReload();
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
            AudioManager.Instance.Play3DClip(m_Attacher.m_EntityID, targetClip, false, m_Case);
    }
}
