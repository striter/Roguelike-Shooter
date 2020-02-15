using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipmentPack : UIPage {
    UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem> m_Grid;
    UIC_EquipmentNameFormatIntro m_Selecting;
    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem>(rtf_Container.Find("EquipmentGrid"), OnItemSelect);
        m_Selecting = new UIC_EquipmentNameFormatIntro(rtf_Container.Find("Selecting"));
    }
    PlayerInfoManager m_Info;
    public void Show(PlayerInfoManager _info)
    {
        m_Info = _info;
        m_Grid.ClearGrid();
        _info.m_ExpireEquipments.Traversal((int index,EquipmentBase equipment) => {
            m_Grid.AddItem(index).SetInfo(equipment);
        });
        m_Selecting.transform.SetActivate(false);
        if (_info.m_ExpireEquipments.Count > 0)
            m_Grid.OnItemClick(0);
    }

    void OnItemSelect(int index)
    {
        m_Selecting.transform.SetActivate(true);
        m_Selecting.SetInfo(m_Info.m_ExpireEquipments[index]);
    }
}
