using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerPerks : UIPage {
    UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem> m_Grid;
    UIC_EquipmentNameFormatIntro m_Selecting;
    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem>(rtf_Container.Find("EquipmentGrid"), OnItemSelect);
        m_Selecting = new UIC_EquipmentNameFormatIntro(rtf_Container.Find("Selecting"));
    }
    PlayerExpireManager m_Info;
    public void Show()
    {
        m_Info = GameManager.Instance.m_LocalPlayer.m_CharacterInfo;
        m_Grid.ClearGrid();
        m_Info.m_ExpirePerks.Traversal((int index,ExpirePlayerPerkBase perk) => {
            m_Grid.AddItem(index).SetInfo(perk);
        });
        m_Selecting.transform.SetActivate(false);
        if (m_Info.m_ExpirePerks.Count > 0)
            m_Grid.OnItemClick(m_Info.m_ExpirePerks.GetIndexKey(0));
    }

    void OnItemSelect(int index)
    {
        m_Selecting.transform.SetActivate(true);
        m_Selecting.SetInfo(m_Info.m_ExpirePerks[index]);
    }
}
