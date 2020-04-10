using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

public class UIGI_EquipmentItemOwned : UIGI_EquipmentItemBase {
    Text m_Equipping, m_Locked;
    public override void Init()
    {
        base.Init();
        m_Equipping = rtf_Container.Find("Equipping").GetComponent<Text>();
        m_Locked = rtf_Container.Find("Locked").GetComponent<Text>();
    }
    public void Play(EquipmentSaveData data,bool locked,bool selected)
    {
        base.Play(data);
        m_Equipping.SetActivate(selected);
        m_Locked.SetActivate(locked);
    }
}
