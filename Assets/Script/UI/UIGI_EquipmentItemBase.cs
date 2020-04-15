using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

public class UIGI_EquipmentItemBase : UIT_GridItem {
    Text m_Name, m_Enhance;
    public override void Init()
    {
        base.Init();
        m_Name = rtf_Container.Find("Name").GetComponent<Text>();
        m_Enhance = rtf_Container.Find("Enhance").GetComponent<Text>();
    }

    protected void Play(EquipmentSaveData equipmentData)
    {
        m_Name.text = equipmentData.GetNameLocalizeKey();
        m_Enhance.text =equipmentData.m_Rarity+ "+" + equipmentData.GetEnhanceLevel();
    }
}
