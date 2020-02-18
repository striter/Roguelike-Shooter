using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649

public class UI_BonfireSelection : UIPage ,TReflection.UI.IUIPropertyFill{
    Action<bool> OnSelect;
    Button m_BtnTop, m_BtnDown;

    protected override void Init()
    {
        base.Init();
        TReflection.UI.UIPropertyFill(this,rtf_Container);
        m_BtnTop.onClick.AddListener(OnTopClick);
        m_BtnDown.onClick.AddListener(OnDownClick);
    }

    public void Play(Action<bool> OnSelect)
    {
        this.OnSelect = OnSelect;
    }

    void OnTopClick()
    {
        OnSelect(true);
        OnCancelBtnClick();
    }
    void OnDownClick()
    {
        OnSelect(false);
        OnCancelBtnClick();
    } 
}
