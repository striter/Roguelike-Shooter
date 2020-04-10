using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;
using UnityEngine.UI;

public class UI_PerkSelect : UIPage
{
    UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem> m_Grid;
    UIC_EquipmentNameFormatIntro m_Selecting;
    
    int m_SelectPerkIndex;
    Action<int> OnEquipmentSelect;

    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem>(rtf_Container.Find("EquipmentGrid"), OnItemSelect);
        m_Selecting = new UIC_EquipmentNameFormatIntro(rtf_Container.Find("Selecting"));
        rtf_Container.Find("Confirm").GetComponent<Button>().onClick.AddListener(OnConfirm);
    }

    public void Show(List<int> _perks,Action<int> OnPerkSelect)
    {
        m_SelectPerkIndex = -1;
        this.OnEquipmentSelect = OnPerkSelect;

        m_Grid.ClearGrid();
        _perks.Traversal((int perk) => {
            m_Grid.AddItem(perk).SetInfo(GameDataManager.GetPerkData( perk));
        });
        m_Grid.OnItemClick(_perks[0]);
    }

    void OnItemSelect(int index)
    {
        m_SelectPerkIndex = index;
        m_Selecting.SetInfo(GameDataManager.GetPerkData(m_SelectPerkIndex));
    }
    void OnConfirm()
    {
        OnEquipmentSelect(m_SelectPerkIndex);
        OnCancelBtnClick();
    }
}
