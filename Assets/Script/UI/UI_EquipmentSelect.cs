using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;
using UnityEngine.UI;

public class UI_EquipmentSelect : UIPage
{
    UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem> m_Grid;
    UIC_EquipmentNameFormatIntro m_Selecting;

    List<ActionPerkBase> m_Equipments;
    int m_selectIndex;
    Action<ActionPerkBase> OnEquipmentSelect;

    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem>(rtf_Container.Find("EquipmentGrid"), OnItemSelect);
        m_Selecting = new UIC_EquipmentNameFormatIntro(rtf_Container.Find("Selecting"));
        rtf_Container.Find("Confirm").GetComponent<Button>().onClick.AddListener(OnConfirm);
    }

    public void Show(List<ActionPerkBase> _equipments,Action<ActionPerkBase> OnEquipmentSelect)
    {
        m_selectIndex = -1;
        this.OnEquipmentSelect = OnEquipmentSelect;
        m_Equipments = _equipments;

        m_Grid.ClearGrid();
        m_Equipments.Traversal((int index, ActionPerkBase equipment) => {
            m_Grid.AddItem(index).SetInfo(equipment);
        });
        m_Grid.OnItemClick(0);
    }

    void OnItemSelect(int index)
    {
        m_selectIndex = index;
        m_Selecting.SetInfo(m_Equipments[m_selectIndex]);
    }
    void OnConfirm()
    {
        OnEquipmentSelect(m_Equipments[m_selectIndex]);
        OnCancelBtnClick();
    }
}
