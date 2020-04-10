using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;
using System;

public class UIGI_EquipmentItemOwned : UIGI_EquipmentItemBase {
    Text m_Equipping, m_Locked;
    Action<int> OnButtonClick;

    public override void Init()
    {
        base.Init();
        rtf_Container.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnButtonClick?.Invoke(m_Index); });
        m_Equipping = rtf_Container.Find("Equipping").GetComponent<Text>();
        m_Locked = rtf_Container.Find("Locked").GetComponent<Text>();
    }
    public void Play(EquipmentSaveData data, Action<int> OnButtonClick, bool equipping, bool locked)
    {
        this.OnButtonClick = OnButtonClick;
        m_Equipping.SetActivate(equipping);
        m_Locked.SetActivate(locked);
        Play(data);
    }
}
