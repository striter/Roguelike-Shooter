using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TSpecialClasses;

public class UIC_CampCurrencyStatus : UIControlBase {
    Text m_Credit;
    ValueLerpSeconds m_CreditLerp;
    protected override void Init()
    {
        base.Init();
        m_Credit = transform.Find("Credit/Data").GetComponent<Text>();
        m_CreditLerp = new ValueLerpSeconds(GameDataManager.m_GameData.f_Credits, 100f,1f,(float value)=> { m_Credit.text = string.Format("{0:N2}",value); });
        OnCampStatus();
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_GameCurrencyStatus, OnCampStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_GameCurrencyStatus, OnCampStatus);
    }
    private void Update()
    {
        m_CreditLerp.TickDelta(Time.deltaTime);
    }
    void OnCampStatus()
    {
        m_CreditLerp.ChangeValue(GameDataManager.m_GameData.f_Credits);
    }

}
