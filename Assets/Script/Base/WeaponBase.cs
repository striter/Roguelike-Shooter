using UnityEngine;
using GameSetting;
using System;

public class WeaponBase : CObjectPoolStaticPrefabBase<enum_PlayerWeaponIdentity>
{
    #region PresetData
    public enum_PlayerAnim E_Anim = enum_PlayerAnim.Invalid;
    public bool B_AttachLeft = false;

    public float F_Damage=10;
    public float F_DamagePerEnhance=10;
    public int I_ClipAmount=10;
    public float F_RefillTime=.1f;
    public float F_RecoilPerShot=2;

    public int I_ExtraBuffApply = -1;
    #endregion
    public virtual enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Invalid;
    public EntityCharacterPlayer m_Attacher { get; private set; }
    public SWeaponInfos m_WeaponInfo { get; private set; }
    public int m_EnhanceLevel { get; private set; }
    public int m_ClipAmount { get; private set; } = 0;
    public int m_AmmoLeft { get; private set; } = 0;
    public Transform m_Muzzle { get; private set; } = null;
    public MeshRenderer m_WeaponSkin { get; private set; } = null;
    public float m_Recoil => m_Attacher.m_CharacterInfo.F_AimSpreadMultiply * F_RecoilPerShot;
    public float m_BaseDamage => F_Damage + F_DamagePerEnhance * m_EnhanceLevel;
    protected WeaponTriggerBase m_Trigger { get; private set; }
    Action<float> OnFireRecoil;
    
    TimerBase m_BulletRefillTimer=new TimerBase(),m_RefillPauseTimer=new TimerBase(GameConst.F_PlayerWeaponFireReloadPause);
    public float F_AmmoStatus => m_AmmoLeft / (float)m_ClipAmount;
    public bool m_HaveAmmoLeft => I_ClipAmount == -1 || m_AmmoLeft > 0;
    public bool B_AmmoFull => I_ClipAmount == -1||m_ClipAmount == m_AmmoLeft;

    protected int m_BaseSFXWeaponIndex { get; private set; }
    public override void OnPoolItemInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity,_OnRecycle);
        m_Muzzle = transform.FindInAllChild("Muzzle");
        m_WeaponSkin = transform.FindInAllChild("Case").GetComponent<MeshRenderer>();
        m_WeaponInfo = GameDataManager.GetWeaponProperties(_identity);
        m_ClipAmount = I_ClipAmount;
        m_AmmoLeft = m_ClipAmount;
        m_BulletRefillTimer.SetTimerDuration(F_RefillTime);
        m_BaseSFXWeaponIndex = GameExpression.GetPlayerWeaponIndex(m_WeaponInfo.m_Index);

        m_Trigger = GetComponent<WeaponTriggerBase>();
        switch (m_Trigger.m_Type)
        {
            case enum_PlayerWeaponTriggerType.Auto:
                (m_Trigger as WeaponTriggerAuto).Init(this, OnTriggerCheck, OnAutoTrigger);
                break;
            case enum_PlayerWeaponTriggerType.Store:
                (m_Trigger as WeaponTriggerStore).Init(this, OnTriggerCheck, OnStoreTrigger);
                break;
        }
    }

    public WeaponBase InitWeapon(WeaponSaveData weaponData)
    {
        m_EnhanceLevel = weaponData.m_Enhance;
        return this;
    }

    public virtual void OnAttach(EntityCharacterPlayer _attacher,Transform _attachTo)
    {
        m_Attacher = _attacher;
        transform.SetParentResetTransform(_attachTo);
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

    public DamageInfo GetWeaponDamageInfo(float damage,enum_DamageType type= enum_DamageType.Basic) => m_Attacher.m_CharacterInfo.GetDamageBuffInfo(damage, I_ExtraBuffApply, enum_DamageType.Basic);
    #region PlayerInteract
    public void Trigger(bool down)=>m_Trigger.OnSetTrigger(down);

    public virtual void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType)
    {
    }
    protected bool OnTriggerCheck() => m_HaveAmmoLeft;

    protected virtual void OnAutoTrigger() => Debug.LogError("Override This Please!");
    protected virtual void OnStoreTrigger(bool success) => Debug.LogError("Override This Please!"); 

    protected virtual void OnAmmoCost()
    {
        m_AmmoLeft--;
        m_RefillPauseTimer.Replay();
        m_BulletRefillTimer.Replay();
        m_Attacher.PlayRecoil(m_Recoil);
    }

    public void OnAttacherAnim(int index=0)=> m_Attacher.PlayAnim(m_Trigger.F_FireRate / m_Attacher.m_CharacterInfo.m_FireRateMultiply,index);

    public virtual void Tick(bool firePausing, float deltaTime)
    {
        int clipAmount = m_Attacher.m_CharacterInfo.CheckClipAmount(I_ClipAmount);
        if (m_ClipAmount != clipAmount)
        {
            m_ClipAmount = clipAmount;
            if (m_AmmoLeft > m_ClipAmount)
                m_AmmoLeft = m_ClipAmount;
        }

        switch (m_Trigger.m_Type)
        {
            default:Debug.LogError("Invalid Convertions Here!");break;
            case enum_PlayerWeaponTriggerType.Auto:
                m_Trigger.Tick(firePausing, m_Attacher.m_CharacterInfo.DoFireRateTick(deltaTime));
                break;
            case enum_PlayerWeaponTriggerType.Store:
                m_Trigger.Tick(firePausing, m_Attacher.m_CharacterInfo.DoStoreRateTick(deltaTime));
                break;
        }
        ReloadTick(m_Attacher.m_CharacterInfo.DoReloadRateTick(deltaTime));
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
