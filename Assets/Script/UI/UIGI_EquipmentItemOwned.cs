using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;
using System;

public class UIGI_EquipmentItemOwned : UIGI_EquipmentItemBase {
    Text m_Equipping, m_Locked,m_Selected;
    Action<int> OnButtonClick;

    public override void Init()
    {
        base.Init();
        rtf_Container.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnButtonClick?.Invoke(m_Index); });
        m_Equipping = rtf_Container.Find("Equipping").GetComponent<Text>();
        m_Locked = rtf_Container.Find("Locked").GetComponent<Text>();
        m_Selected = rtf_Container.Find("Selected").GetComponent<Text>();
    }
    public void Play(EquipmentSaveData data, Action<int> OnButtonClick, bool equipping, bool locked,bool selected,bool deconstruct)
    {
        this.OnButtonClick = OnButtonClick;
        m_Equipping.SetActivate(equipping);
        m_Locked.SetActivate(locked);
        m_Selected.SetActivate(selected||deconstruct);
        m_Selected.text = selected ? "Selcted" : deconstruct ? "Deconstruct" : "";
        Play(data);
    }
}
