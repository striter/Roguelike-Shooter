using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIC_CampStatus : UIControlBase {
    Text m_Credit;
    Text m_TechPoint;
    protected override void Init()
    {
        base.Init();
        m_Credit = transform.Find("Credit").GetComponent<Text>();
        m_TechPoint = transform.Find("TechPoint").GetComponent<Text>();
        OnCampStatus();
        TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_CampDataStatus, OnCampStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_CampDataStatus, OnCampStatus);
    }

    void OnCampStatus()
    {
        m_Credit.text = GameDataManager.m_PlayerCampData.f_Credits.ToString();
        m_TechPoint.text = GameDataManager.m_PlayerCampData.f_TechPoints.ToString();
    }

}
