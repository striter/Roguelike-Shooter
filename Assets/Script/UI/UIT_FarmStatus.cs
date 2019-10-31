using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIT_FarmStatus : UIToolsBase {

    Button m_Buy, m_Exit,m_Profit;
    Text m_ProfitAmount;
    Action OnExitClick, OnBuyClick, OnProfitClick;
    protected override void Init()
    {
        base.Init();
        m_Buy = transform.Find("Buy").GetComponent<Button>();
        m_Buy.onClick.AddListener(()=> { OnBuyClick(); });
        m_Exit = transform.Find("Exit").GetComponent<Button>();
        m_Exit.onClick.AddListener(()=> {Destroy(this.gameObject); OnExitClick(); });
        m_Profit = transform.Find("Profit").GetComponent<Button>();
        m_Profit.onClick.AddListener(() => { OnProfitClick(); });
        m_ProfitAmount = m_Profit.transform.Find("Text").GetComponent<Text>();

        TBroadCaster<enum_BC_UIStatus>.Add<float>(enum_BC_UIStatus.UI_CampFarmProfitStatus, OnProfitStatus);
    }
    protected override void OnDestroy()
    {
        TBroadCaster<enum_BC_UIStatus>.Remove<float>(enum_BC_UIStatus.UI_CampFarmProfitStatus, OnProfitStatus);
        base.OnDestroy();
    }
    public void Play(Action _OnExitClick, Action _OnBuyClick,Action _OnProfitClick)
    {
        OnExitClick = _OnExitClick;
        OnBuyClick = _OnBuyClick;
        OnProfitClick = _OnProfitClick;
    }
    void OnProfitStatus(float amount)
    {
        m_ProfitAmount.text = amount.ToString();
    }
    
}
