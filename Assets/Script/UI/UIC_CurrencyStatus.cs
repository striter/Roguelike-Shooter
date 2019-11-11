using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TSpecialClasses;

public class UIC_CurrencyStatus : UIControlBase {
    Text m_Credit;
    Text m_TechPoint;
    ValueLerpSeconds m_CreditLerp,m_TechPointLerp;
    protected override void Init()
    {
        base.Init();
        m_Credit = transform.Find("Credit/Data").GetComponent<Text>();
        m_TechPoint = transform.Find("TechPoint/Data").GetComponent<Text>();
        m_CreditLerp = new ValueLerpSeconds(GameDataManager.m_PlayerCampData.f_Credits, 100f,1f);
        m_TechPointLerp = new ValueLerpSeconds(GameDataManager.m_PlayerCampData.f_TechPoints, 50f,1f);
        m_Credit.text = m_CreditLerp.m_value.ToString();
        m_TechPoint.text = m_TechPointLerp.m_value.ToString();
        OnCampStatus();
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_CampDataStatus, OnCampStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_CampDataStatus, OnCampStatus);
    }
    private void Update()
    {
        if (m_CreditLerp.TickDelta(Time.deltaTime))
            m_Credit.text = ((int)m_CreditLerp.m_value).ToString();
        if(m_TechPointLerp.TickDelta(Time.deltaTime))
            m_TechPoint.text = ((int)m_TechPointLerp.m_value).ToString();
    }
    void OnCampStatus()
    {
        m_CreditLerp.ChangeValue(GameDataManager.m_PlayerCampData.f_Credits);
        m_TechPointLerp.ChangeValue(GameDataManager.m_PlayerCampData.f_TechPoints);
    }

}
