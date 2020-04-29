using UnityEngine;
using GameSetting;
using System;

public class WeaponBase : CObjectPoolMono<enum_PlayerWeapon>
{
    #region PresetData
    public enum_PlayerAnim E_Anim = enum_PlayerAnim.Invalid;
    public bool B_AttachLeft = false;
    public int I_ExtraBuffApply = -1;
    #endregion
    protected virtual enum_PlayerWeaponType m_WeaponType => enum_PlayerWeaponType.Invalid;
    protected EntityCharacterPlayer m_Attacher { get; private set; }
    public SWeapon m_WeaponInfo { get; private set; }
    public int I_AmmoLeft { get; private set; } = 0;
    public Transform m_Muzzle { get; private set; } = null;
    public MeshRenderer m_WeaponSkin { get; private set; } = null;
    public int I_ClipAmount { get; private set; } = 0;
    public float F_Recoil => m_Attacher.m_CharacterInfo.F_SpreadMultiply * m_WeaponInfo.m_RecoilPerShot;
    protected WeaponTrigger m_Trigger { get; private set; }
    protected virtual WeaponTrigger GetTrigger() => new WeaponTriggerAuto(m_WeaponInfo.m_FireRate, OnTriggerCheck,OnAutoTriggerSuccessful);
    Action<float> OnFireRecoil;
    
    TimerBase m_BulletRefillTimer=new TimerBase(),m_RefillPauseTimer=new TimerBase(GameConst.F_PlayerWeaponFireReloadPause);
    public float F_AmmoStatus => I_AmmoLeft / (float)I_ClipAmount;
    public bool m_HaveAmmoLeft => m_WeaponInfo.m_ClipAmount == -1 || I_AmmoLeft > 0;
    public bool B_AmmoFull => m_WeaponInfo.m_ClipAmount == -1||I_ClipAmount == I_AmmoLeft;

    protected int m_BaseSFXWeaponIndex { get; private set; }
    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity,_OnRecycle);
        m_Muzzle = transform.FindInAllChild("Muzzle");
        m_WeaponSkin = transform.FindInAllChild("Case").GetComponent<MeshRenderer>();
        m_WeaponInfo = GameDataManager.GetWeaponProperties(_identity);
        I_ClipAmount = m_WeaponInfo.m_ClipAmount;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        m_Trigger = GetTrigger();
        m_BulletRefillTimer.SetTimerDuration(m_WeaponInfo.m_RefillTime);
        m_BaseSFXWeaponIndex = GameExpression.GetPlayerWeaponIndex(m_WeaponInfo.m_Index);
    }

    public void OnAttach(EntityCharacterPlayer _attacher,Transform _attachTo,Action<float> _OnFireRecoil)
    {
        m_Attacher = _attacher;
        transform.SetParentResetTransform(_attachTo);
        OnFireRecoil = _OnFireRecoil;
        OnShow(true);
    }

    public DamageInfo GetWeaponDamageInfo(float damage) => m_Attacher.m_CharacterInfo.GetDamageBuffInfo(damage, I_ExtraBuffApply, enum_DamageType.Basic);

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

        m_Trigger.OnTriggerStop();
    }

    void OnShow(bool show)=>transform.SetActivate(show);
    #region PlayerInteract
    public void Trigger(bool down)=>m_Trigger.OnSetTrigger(down);

    public virtual void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType)
    {
    }
    protected bool OnTriggerCheck() => m_HaveAmmoLeft;
    protected virtual void OnAutoTriggerSuccessful() => OnTriggerSuccessful();

    protected void OnTriggerSuccessful()
    {
        I_AmmoLeft--;
        OnFireRecoil?.Invoke(F_Recoil);
        m_RefillPauseTimer.Replay();
        m_BulletRefillTimer.Replay();
    }

    public virtual void Tick(bool firePausing, float fireTick,float reloadTick)
    {
        m_Trigger.Tick(firePausing,fireTick);
        ReloadTick(reloadTick);

        int clipAmount = m_Attacher.m_CharacterInfo.CheckClipAmount(m_WeaponInfo.m_ClipAmount);
        if (I_ClipAmount != clipAmount)
        {
            I_ClipAmount = clipAmount;
            if (I_AmmoLeft > I_ClipAmount)
                I_AmmoLeft = I_ClipAmount;
        }

    }

    public void AddAmmo(int amount) => I_AmmoLeft = Mathf.Clamp(I_AmmoLeft + amount, 0, I_ClipAmount);
    void ReloadTick(float deltaTime)
    {
        m_RefillPauseTimer.Tick(deltaTime);
        if (m_RefillPauseTimer.m_Timing)
            return;

        m_BulletRefillTimer.Tick(deltaTime);
        if (m_BulletRefillTimer.m_Timing)
            return;

        if (B_AmmoFull)
            return;

        I_AmmoLeft++;
        m_BulletRefillTimer.Replay();
    }
    
    #endregion
}
