using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponTriggerAuto : WeaponTriggerBase
{
    public float F_FireRate = .1f;
    public override enum_PlayerWeaponTriggerType m_Type => enum_PlayerWeaponTriggerType.Auto;
    public override bool m_Triggering => m_TriggerTimer.m_Timing;
    protected TimerBase m_TriggerTimer { get; private set; }
    Func<bool> OnTriggerCheck;
    Action<float> OnTriggerSuccessful;
    public void Init(WeaponBase _weapon, Func<bool> _OnTriggerCheck, Action<float> _OnTriggerSuccessful)
    {
        base.Init(_weapon);
        OnTriggerCheck = _OnTriggerCheck;
        OnTriggerSuccessful = _OnTriggerSuccessful;
        m_TriggerTimer = new TimerBase(F_FireRate, true);
    }
    public void Tick(bool paused, float deltaTime)
    {
        m_TriggerTimer.Tick(deltaTime);
        if (paused || m_TriggerTimer.m_Timing)
            return;

        if (!m_TriggerDown)
            return;

        if (!OnTriggerCheck())
            return;
        OnTriggerSuccessful(m_TriggerTimer.m_TimerDuration);
        m_TriggerTimer.Replay();
    }
}
