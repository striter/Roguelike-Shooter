using System;
using GameSetting;

public class WeaponTriggerStore : WeaponTriggerBase {

    public float F_FireRate = .1f;
    public int I_StoreIndicatorIndex;
    public float F_StoreDuration;

    public override enum_PlayerWeaponTriggerType m_Type => enum_PlayerWeaponTriggerType.Store;
    Func<bool> OnStoreBeginCheck;
    Action<float,bool> OnStoreFinish;
    TimerBase m_StoreTimer = new TimerBase();
    TimerBase m_TriggerTimer;
    public override bool m_Triggering => m_Storing;
    public bool m_Storing { get; private set; } = false;
    SFXIndicator m_Indicator;
    public void Init(WeaponBase weapon, Func<bool> _OnStoreBeginCheck, Action<float,bool> _OnStoreFinish)
    {
        base.Init(weapon);
        OnStoreBeginCheck = _OnStoreBeginCheck;
        OnStoreFinish = _OnStoreFinish;
        m_StoreTimer = new TimerBase(F_StoreDuration);
        m_TriggerTimer = new TimerBase(F_FireRate);
        m_Storing = false;
    }

    public override void OnSetTrigger(bool down)
    {
        base.OnSetTrigger(down);
        if (!down||m_TriggerTimer.m_Timing|| !OnStoreBeginCheck())
            return;

        SetStore(true);
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
        if (m_Storing)
        {
            m_StoreTimer.Replay();
        }
        else
        {
            OnStoreFinish(m_TriggerTimer.m_TimerDuration, !m_StoreTimer.m_Timing);
            m_TriggerTimer.Replay();
        }

    }

    public void Tick(bool paused,float fireRateDelta,  float storeDelta)
    {
        m_TriggerTimer.Tick(fireRateDelta);
        PlayIndicator(m_Storing&&m_StoreTimer.m_TimeLeftScale<.9f);
        if (!m_Storing)
            return;

        if (m_TriggerDown)
        {
            m_StoreTimer.Tick(storeDelta);
            return;
        }

        if (paused)
            return;
        SetStore(false);
    }

    void PlayIndicator(bool play)
    {
        if (play && !m_Indicator)
        {
            m_Indicator = GameObjectManager.SpawnIndicator(I_StoreIndicatorIndex,m_Weapon.m_Muzzle.position,m_Weapon.m_Muzzle.forward);
            m_Indicator.AttachTo(m_Weapon.m_Muzzle);
            m_Indicator.PlayControlled(m_Weapon.m_Attacher.m_EntityID);
        }

        if (!play && m_Indicator)
        {
            m_Indicator.Stop();
            m_Indicator = null;
        }
    }
}
