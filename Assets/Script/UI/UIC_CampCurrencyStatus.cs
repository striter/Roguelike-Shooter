using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TSpecialClasses;

public class UIC_CampCurrencyStatus : UIControlBase {
    Text m_Credit;
    Text m_Diamonds;
    ValueLerpSeconds m_CreditLerp;
    ValueLerpSeconds m_DiamondsLerp;
    protected override void Init()
    {
        base.Init();
        m_Credit = transform.Find("Credit/Data").GetComponent<Text>();
        m_CreditLerp = new ValueLerpSeconds(GameDataManager.m_GameData.m_Credit, 100f,1f,(float value)=> { m_Credit.text = string.Format("{0:N2}",value); });

        m_Diamonds = transform.Find("Diamonds/Data").GetComponent<Text>();
        m_DiamondsLerp = new ValueLerpSeconds(GameDataManager.m_GameData.m_Diamonds, 100f, 1f, (float value) => { m_Diamonds.text = string.Format("{0:N2}", value); });
        OnCampStatus();
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_CampCurrencyStatus, OnCampStatus);
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_CampDiamondsStatus, OnCampStatusNew);

        //GameDataManager.m_GameTaskData.RandomTask();
        GameDataManager.RandomTask();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_CampCurrencyStatus, OnCampStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_CampDiamondsStatus, OnCampStatusNew);
    }
    private void Update()
    {
        m_CreditLerp.TickDelta(Time.deltaTime);
        m_DiamondsLerp.TickDelta(Time.deltaTime);
    }
    void OnCampStatus()
    {
        m_CreditLerp.SetLerpValue(GameDataManager.m_GameData.m_Credit);
    }
    void OnCampStatusNew()
    {
        m_DiamondsLerp.SetLerpValue(GameDataManager.m_GameData.m_Diamonds);
    }

}
