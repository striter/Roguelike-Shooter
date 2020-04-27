using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIGI_EquipmentItemSelected : UIGI_EquipmentItemBase {

    UIT_GridControllerMono<Text> m_EntryGrid;
    Text m_EnhanceRequirementLeft,m_EnhanceDetail;

    public override void Init()
    {
        base.Init();
        m_EntryGrid =new UIT_GridControllerMono<Text>( rtf_Container.Find("EntryGrid"));
        m_EnhanceRequirementLeft = rtf_Container.Find("EnhanceRequirementLeft").GetComponent<Text>();
        m_EnhanceDetail = rtf_Container.Find("EnhanceDetail").GetComponent<Text>();
    }

    public void Play(EquipmentSaveData data,int enhanceReceive)
    {
        base.Play(data);
        m_EntryGrid.ClearGrid();
        data.m_Entries.Traversal((int index,EquipmentEntrySaveData entryData) =>
        {
            m_EntryGrid.AddItem(index).text=entryData.m_Type+":"+entryData.m_Value;
        });
        m_EntryGrid.AddItem(m_EntryGrid.m_Count).text = "Passive" + data.GetPassiveLocalizeKey();
        m_EnhanceRequirementLeft.text = "Next Require:" + data.GetEnhanceRequireNextLevel();

        m_EnhanceDetail.SetActivate(enhanceReceive > 0);
        if (enhanceReceive <= 0)
            return;

        int levelOffset= GameDataManager.GetEnhanceLevel(data.m_Enhance+enhanceReceive,data.m_Rarity)-data.GetEnhanceLevel();
        m_EnhanceDetail.text = enhanceReceive <= 0 ? "" : "+" + enhanceReceive.ToString()+(levelOffset>0?(", Upgrade:"+levelOffset):"");

    }
}
