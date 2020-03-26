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

    List<ExpirePerkBase> m_Perks;
    int m_selectIndex;
    Action<ExpirePerkBase> OnEquipmentSelect;

    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem>(rtf_Container.Find("EquipmentGrid"), OnItemSelect);
        m_Selecting = new UIC_EquipmentNameFormatIntro(rtf_Container.Find("Selecting"));
        rtf_Container.Find("Confirm").GetComponent<Button>().onClick.AddListener(OnConfirm);
    }

    public void Show(List<ExpirePerkBase> _perks,Action<ExpirePerkBase> OnPerkSelect)
    {
        m_selectIndex = -1;
        this.OnEquipmentSelect = OnPerkSelect;
        m_Perks = _perks;

        m_Grid.ClearGrid();
        m_Perks.Traversal((int index, ExpirePerkBase perk) => {
            m_Grid.AddItem(index).SetInfo(perk);
        });
        m_Grid.OnItemClick(0);
    }

    void OnItemSelect(int index)
    {
        m_selectIndex = index;
        m_Selecting.SetInfo(m_Perks[m_selectIndex]);
    }
    void OnConfirm()
    {
        OnEquipmentSelect(m_Perks[m_selectIndex]);
        OnCancelBtnClick();
    }
}
