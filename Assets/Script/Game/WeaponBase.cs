using UnityEngine;
using GameSetting;
using System;

public class WeaponBase : CObjectPoolStaticPrefabBase<enum_PlayerWeaponIdentity>
{
    #region PresetData
    public enum_PlayerAnim E_Anim = enum_PlayerAnim.Invalid;
    public bool B_AttachLeft = false;

    public int I_ClipAmount = 10;
    public float F_RefillTime = .1f;

    #endregion
    public virtual enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Invalid;
    public EntityCharacterPlayer m_Attacher { get; private set; }
    public SWeaponInfos m_WeaponInfo { get; private set; }
    public int m_EnhanceLevel { get; private set; }
    public int m_ClipAmount { get; private set; } = 0;
    public int m_AmmoLeft { get; private set; } = 0;
    public Transform m_Muzzle { get; private set; } = null;
    public MeshRenderer m_WeaponSkin { get; private set; } = null;

    public WeaponTriggerBase m_Trigger { get; private set; }
    WeaponTriggerAuto m_AutoTrigger;
    WeaponTriggerStore m_StoreTrigger;

    TimerBase m_BulletRefillTimer = new TimerBase(), m_RefillPauseTimer = new TimerBase(GameConst.F_PlayerWeaponFireReloadPause);
    public float F_AmmoStatus => m_AmmoLeft / (float)m_ClipAmount;
    public bool m_HaveAmmoLeft => I_ClipAmount == -1 || m_AmmoLeft > 0;
    public bool B_AmmoFull => I_ClipAmount == -1 || m_ClipAmount == m_AmmoLeft;
    public int m_WeaponID { get; private set; } = -1;
    protected int m_BaseSFXWeaponIndex { get; private set; }
    public override void OnPoolInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
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
                m_AutoTrigger = m_Trigger as WeaponTriggerAuto;
                m_AutoTrigger.Init(this, OnTriggerTickCheck, OnAutoTrigger);
                break;
            case enum_PlayerWeaponTriggerType.Store:
                m_StoreTrigger = m_Trigger as WeaponTriggerStore;
                m_StoreTrigger.Init(this, OnTriggerTickCheck, OnStoreTrigger);
                break;
        }
    }

    public WeaponBase InitWeapon(WeaponSaveData weaponData)
    {
        m_WeaponID = GameIdentificationManager.GetWeaponID();
        m_EnhanceLevel = weaponData.m_Enhance;
        return this;
    }

    public virtual void OnAttach(EntityCharacterPlayer _attacher, Transform _attachTo)
    {
        m_Attacher = _attacher;
        transform.SetParentResetTransform(_attachTo);
    }

    public virtual void OnDetach()
    {
        m_Attacher = null;
    }


    public virtual void OnShow(bool showing)
    {
        transform.SetActivate(showing);

        if (showing)
            return;
        m_Trigger.Stop();
    }

    protected bool OnTriggerTickCheck() => m_HaveAmmoLeft;

    protected virtual void OnAutoTrigger(float animDuration) => OnAttackAnim(animDuration);
    protected virtual void OnStoreTrigger(float animDuration, float storeTimeLeft) => OnAttackAnim(animDuration);

    protected void OnAttackAnim(float animDuration,int index = 0) => m_Attacher.PlayAttackAnim(animDuration / m_Attacher.m_CharacterInfo.m_FireRateMultiply, index);
    public void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType) { if (eventType == TAnimatorEvent.enum_AnimEvent.Fire) OnKeyAnim(); }
    protected virtual void OnKeyAnim() { OnAmmoCost(); }

    protected virtual void OnAmmoCost()
    {
        m_AmmoLeft--;
        m_RefillPauseTimer.Replay();
        m_BulletRefillTimer.Replay();
    }
    #region PlayerInteract
    public void Trigger(bool down) => m_Trigger.OnSetTrigger(down);

    public virtual bool OnDamageBlockCheck(DamageInfo info) { return false; }

    public virtual void OnDealtDamage(float amountApply) { }

    public virtual void WeaponTick(bool firePausing, float deltaTime)
    {
            m_Attacher.PlayAttackTriggering(m_Trigger.m_Triggering);

        int clipAmount = m_Attacher.m_CharacterInfo.CheckClipAmount(I_ClipAmount);
        if (m_ClipAmount != clipAmount)
        {
            m_ClipAmount = clipAmount;
            if (m_AmmoLeft > m_ClipAmount)
                m_AmmoLeft = m_ClipAmount;
        }

        if (m_AutoTrigger != null)
            m_AutoTrigger.Tick(firePausing, m_Attacher.m_CharacterInfo.DoFireRateTick(deltaTime));
        if(m_StoreTrigger)
            m_StoreTrigger.Tick(firePausing, m_Attacher.m_CharacterInfo.DoFireRateTick(deltaTime), m_Attacher.m_CharacterInfo.DoStoreRateTick(deltaTime));
    }
    

    public void AddAmmo(int amount) => m_AmmoLeft = Mathf.Clamp(m_AmmoLeft + amount, 0, m_ClipAmount);
    public void ReloadTick(float deltaTime)
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
