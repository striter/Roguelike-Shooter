using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponTriggerAuto : WeaponTriggerBase {
    public override enum_PlayerWeaponTriggerType m_Type => enum_PlayerWeaponTriggerType.Auto;
    protected Func<bool> OnTriggerCheck;
    Action OnTriggerSuccessful;
    public void Init(WeaponBase _weapon, Func<bool> _OnTriggerCheck, Action _OnTriggerSuccessful)
    {
        base.Init(_weapon);
        OnTriggerCheck = _OnTriggerCheck;
        OnTriggerSuccessful = _OnTriggerSuccessful;
    }
    public override void Tick(bool paused, float deltaTime)
    {
        base.Tick(paused, deltaTime);
        if (paused || m_TriggerTimer.m_Timing)
            return;

        if (!m_TriggerDown)
            return;

        if (!OnTriggerCheck())
            return;
        OnTriggerSuccessful();
        m_TriggerTimer.Replay();
    }
}
