using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipmentPack : UIPage {
    UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem> m_Grid;
    UIC_ActionNameFormatIntro m_Selecting;
    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControlledSingleSelect<UIGI_ActionEquipmentPackItem>(rtf_Container.Find("EquipmentGrid"), OnItemSelect);
        m_Selecting = new UIC_ActionNameFormatIntro(rtf_Container.Find("Selecting"));
    }
    PlayerInfoManager m_Info;
    public void Show(PlayerInfoManager _info)
    {
        m_Info = _info;
        m_Grid.ClearGrid();
        List<ActionEquipment> targetList = _info.m_ActionEquipment;
        for (int i = 0; i < GameConst.I_PlayerEquipmentCount; i++)
            m_Grid.AddItem(i).SetInfo( i< targetList.Count ? targetList[i]:null);
        m_Selecting.transform.SetActivate(false);
        if (targetList.Count > 0)
            m_Grid.OnItemClick(0);
    }

    void OnItemSelect(int index)
    {
        m_Selecting.transform.SetActivate(true);
        m_Selecting.SetInfo(m_Info.m_ActionEquipment[index]);
    }
}
