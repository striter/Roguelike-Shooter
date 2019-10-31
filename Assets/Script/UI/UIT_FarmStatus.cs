using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIT_FarmStatus : UIToolsBase {

    Button m_Buy, m_Exit;
    Action OnExitClick, OnBuyClick;
    protected override void Init()
    {
        base.Init();
        m_Buy = transform.Find("Buy").GetComponent<Button>();
        m_Buy.onClick.AddListener(OnFarmBuyClicked);
        m_Exit = transform.Find("Exit").GetComponent<Button>();
        m_Exit.onClick.AddListener(ExitFarm);
    }
    public void Play(Action _OnExitClick, Action _OnBuyClick)
    {
        OnExitClick = _OnExitClick;
        OnBuyClick = _OnBuyClick;
    }

    void OnFarmBuyClicked()
    {
        OnBuyClick();
    }
    void ExitFarm()
    {
        OnExitClick();
        Destroy(this.gameObject);
    }
}
