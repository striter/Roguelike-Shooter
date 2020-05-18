using System;
using GameSetting;
using UnityEngine;

public class WeaponTriggerStore : WeaponTriggerBase {

    public float F_FireRate = .1f;
    public float F_StoreDuration;
    public int I_StoreIndicatorIndex;
    public int I_StoreSuccesfulParticlesIndex;

    public override enum_PlayerWeaponTriggerType m_Type => enum_PlayerWeaponTriggerType.Store;
    Func<bool> OnStoreBeginCheck;
    Action<float, float> OnStoreEndCheck;
    TimerBase m_StoreTimer = new TimerBase();
    TimerBase m_TriggerTimer;
    public override bool m_Triggering => m_Storing;
    public bool m_Storing { get; private set; } = false;
    SFXIndicator m_Indicator;
    public void Init(WeaponBase weapon, Func<bool> _OnStoreBeginCheck, Action<float,float> _OnStoreEndCheck)
    {
        base.Init(weapon);
        OnStoreBeginCheck = _OnStoreBeginCheck;
        OnStoreEndCheck = _OnStoreEndCheck;
        m_StoreTimer = new TimerBase(F_StoreDuration);
        m_TriggerTimer = new TimerBase(F_FireRate);
        m_Storing = false;
    }

    public override void OnSetTrigger(bool down)
    {
        base.OnSetTrigger(down);

        if (!down)
        {
            SetStore(false);
            return;
        }

        if (down && !m_TriggerTimer.m_Timing && OnStoreBeginCheck())
        {
            SetStore(true);
            return;
        }

    }

    public override void OnTriggerStop()
    {
        base.OnTriggerStop();
        SetStore(false);
    }

    void SetStore(bool store)
    {
        if (m_Storing == store)
            return;

        m_Storing = store;
        PlayIndicator(m_Storing);
        if (m_Storing)
        {
            m_StoreTimer.Replay();
        }
        else
        {
            OnStoreEndCheck(m_TriggerTimer.m_TimerDuration, m_StoreTimer.m_TimeLeftScale);
            m_TriggerTimer.Replay();
        }

    }

    public void Tick(bool paused,float fireRateDelta,  float storeDelta)
    {
        m_TriggerTimer.Tick(fireRateDelta);
        if (!m_Storing||paused)
            return;

        m_Indicator.transform.localScale = Vector3.one * (2f - m_StoreTimer.m_TimeLeftScale);
        if (m_TriggerDown&&m_StoreTimer.m_Timing)
        {
            m_StoreTimer.Tick(storeDelta);
            if (!m_StoreTimer.m_Timing)
                GameObjectManager.SpawnParticles(I_StoreSuccesfulParticlesIndex, m_Weapon.m_Muzzle.position, m_Weapon.m_Muzzle.forward).PlayUncontrolled(m_Weapon.m_Attacher.m_EntityID).AttachTo(m_Weapon.m_Muzzle);
            return;
        }
    }

    void PlayIndicator(bool play)
    {
        if (play && !m_Indicator)
        {
            m_Indicator = GameObjectManager.SpawnIndicator(I_StoreIndicatorIndex,m_Weapon.m_Muzzle.position,m_Weapon.m_Muzzle.forward);
            m_Indicator.AttachTo(m_Weapon.m_Muzzle);
            m_Indicator.PlayControlled(m_Weapon.m_Attacher.m_EntityID);
            m_Indicator.transform.localScale = Vector3.one;
        }

        if (!play && m_Indicator)
        {
            m_Indicator.Stop();
            m_Indicator = null;
        }
    }
}
