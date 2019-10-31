using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIT_CampStatus : UIToolsBase {
    Text m_Credit;
    
    protected override void Init()
    {
        base.Init();
        m_Credit = transform.Find("Credit").GetComponent<Text>();
        OnCampStatus();
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_CampStatus, OnCampStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_CampStatus, OnCampStatus);
    }

    void OnCampStatus()
    {
        m_Credit.text = GameDataManager.m_PlayerCampData.f_Credits.ToString();
    }

}
