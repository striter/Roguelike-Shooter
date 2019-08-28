﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIGI_ActionHoldItem : UIT_GridItem {
    Text m_Name;
    Button m_Button;
    Action<int> OnClick;
    protected override void Init()
    {
        base.Init();
        if (m_Name)
            return;
        m_Name = tf_Container.Find("Name").GetComponent<Text>();
        m_Button = tf_Container.Find("Button").GetComponent<Button>();
        m_Button.onClick.AddListener(()=> { OnClick(I_Index); });
    }
    public void SetInfo(ActionBase actionInfo,Action<int> _OnClick)
    {
        OnClick = _OnClick;
        m_Name.text = actionInfo.GetNameLocalizeKey().Localize();
    }
}