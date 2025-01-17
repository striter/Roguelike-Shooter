﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ActionEquipmentSelect : UIGI_ActionBase,IGridHighlight {
    
    UIT_TextExtend m_Name;
    Transform m_Selected;
    protected Button m_Button { get; private set; }
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Name = rtf_Container.Find("Name").GetComponent<UIT_TextExtend>();
        m_Selected = rtf_Container.Find("Selected");
        m_Button = GetComponent<Button>();
    }
    public void AttachSelectButton(Action<int> OnButtonClick)=> m_Button.onClick.AddListener(()=> { OnButtonClick(m_Identity); });

    public void OnHighlight(bool highlight)
    {
        m_Selected.SetActivate(highlight);
        m_Name.color = highlight ? TCommon.GetHexColor("FE9E00FF") : TCommon.GetHexColor("FFFFFFFF");
    }

    public override void SetInfo(ExpirePlayerPerkBase action)
    {
        base.SetInfo(action);
        m_Name.localizeKey = action.GetNameLocalizeKey();
    }
}
