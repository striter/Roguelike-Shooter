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
    public int m_EnhanceLevel { get; private set; }
    public int m_ClipAmount { get; private set; } = 0;
    public int m_AmmoLeft { get; private set; } = 0;
    public Transform m_Muzzle { get; private set; } = null;
    public MeshRenderer m_WeaponSkin { get; private set; } = null;
    public float m_Recoil => m_Attacher.m_CharacterInfo.F_AimSpreadMultiply * m_WeaponInfo.m_RecoilPerShot;
    public float m_BaseDamage => m_WeaponInfo.m_Damage * (1f + GameExpression.GetPlayerWeaponBaseDamageMultiplyAdditive(m_WeaponInfo.m_Rarity,m_EnhanceLevel));
    protected WeaponTrigger m_Trigger { get; private set; }
    protected virtual WeaponTrigger GetTrigger() => new WeaponTriggerAuto(m_WeaponInfo.m_FireRate, OnTriggerCheck,OnAutoTriggerSuccessful);
    Action<float> OnFireRecoil;
    
    TimerBase m_BulletRefillTimer=new TimerBase(),m_RefillPauseTimer=new TimerBase(GameConst.F_PlayerWeaponFireReloadPause);
    public float F_AmmoStatus => m_AmmoLeft / (float)m_ClipAmount;
    public bool m_HaveAmmoLeft => m_WeaponInfo.m_ClipAmount == -1 || m_AmmoLeft > 0;
    public bool B_AmmoFull => m_WeaponInfo.m_ClipAmount == -1||m_ClipAmount == m_AmmoLeft;

    protected int m_BaseSFXWeaponIndex { get; private set; }
    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity,_OnRecycle);
        m_Muzzle = transform.FindInAllChild("Muzzle");
        m_WeaponSkin = transform.FindInAllChild("Case").GetComponent<MeshRenderer>();
        m_WeaponInfo = GameDataManager.GetWeaponProperties(_identity);
        m_ClipAmount = m_WeaponInfo.m_ClipAmount;
        m_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        m_Trigger = GetTrigger();
        m_BulletRefillTimer.SetTimerDuration(m_WeaponInfo.m_RefillTime);
        m_BaseSFXWeaponIndex = GameExpression.GetPlayerWeaponIndex(m_WeaponInfo.m_Index);
    }

    public WeaponBase InitWeapon(WeaponSaveData weaponData)
    {
        m_EnhanceLevel = weaponData.m_Enhance;
        Debug.Log("Spawned Weapon:" + TDataConvert.Convert( weaponData));
        return this;
    }

    public virtual void OnAttach(EntityCharacterPlayer _attacher,Transform _attachTo,Action<float> _OnFireRecoil)
    {
        m_Attacher = _attacher;
        transform.SetParentResetTransform(_attachTo);
        OnFireRecoil = _OnFireRecoil;
    }

    public virtual void OnDetach()
    {
        m_Attacher = null;
    }


    public virtual void OnShow(bool play)
    {
        transform.SetActivate(play);

        if (play)
            return;

        m_Trigger.OnTriggerStop();
    }

    public DamageInfo GetWeaponDamageInfo(float damage) => m_Attacher.m_CharacterInfo.GetDamageBuffInfo(damage, I_ExtraBuffApply, enum_DamageType.Basic);
    #region PlayerInteract
    public void Trigger(bool down)=>m_Trigger.OnSetTrigger(down);

    public virtual void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType)
    {
    }
    protected bool OnTriggerCheck() => m_HaveAmmoLeft;
    protected virtual void OnAutoTriggerSuccessful() => OnTriggerSuccessful();

    protected void OnTriggerSuccessful()
    {
        m_AmmoLeft--;
        OnFireRecoil?.Invoke(m_Recoil);
        m_RefillPauseTimer.Replay();
        m_BulletRefillTimer.Replay();
    }

    public virtual void Tick(bool firePausing, float triggerTick,float reloadTick)
    {
        m_Trigger.Tick(firePausing,triggerTick);
        ReloadTick(reloadTick);

        int clipAmount = m_Attacher.m_CharacterInfo.CheckClipAmount(m_WeaponInfo.m_ClipAmount);
        if (m_ClipAmount != clipAmount)
        {
            m_ClipAmount = clipAmount;
            if (m_AmmoLeft > m_ClipAmount)
                m_AmmoLeft = m_ClipAmount;
        }

    }

    public void AddAmmo(int amount) => m_AmmoLeft = Mathf.Clamp(m_AmmoLeft + amount, 0, m_ClipAmount);
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

        m_AmmoLeft++;
        m_BulletRefillTimer.Replay();
    }
    
    #endregion
}
