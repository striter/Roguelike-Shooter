using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentSwap : UIPage {
    UIC_ActionNameFormatIntro m_SelectItem, m_SwapItem;
    UIT_GridControlledSingleSelect<UIGI_ActionEquipmentSelect> m_EquipmentGrid;
    Button m_ConfirmBtn;
    PlayerInfoManager m_Info;
    ActionBase m_SwapEquipment;
    Action<bool> OnPageHide;
    protected override void Init()
    {
        base.Init();
        m_ConfirmBtn = tf_Container.Find("ConfirmBtn").GetComponent<Button>();
        m_ConfirmBtn.onClick.AddListener(OnConfirmBtnClick);
        m_SelectItem = new UIC_ActionNameFormatIntro( tf_Container.Find("SelectItem"));
        m_SwapItem = new UIC_ActionNameFormatIntro( tf_Container.Find("SwapItem"));
        m_EquipmentGrid = new UIT_GridControlledSingleSelect<UIGI_ActionEquipmentSelect>(tf_Container.Find("EquipmentGrid"),OnItemSelect);
    }

    public void Play(PlayerInfoManager _info,ActionBase _swapEquipment,Action<bool> _OnPageHide)
    {
        m_Info = _info;
        m_SwapEquipment = _swapEquipment;
        OnPageHide = _OnPageHide;
        m_EquipmentGrid.ClearGrid();
        for(int i=0;i<m_Info.m_ActionEquipment.Count;i++)
            m_EquipmentGrid.AddItem(i).SetInfo(m_Info.m_ActionEquipment[i]);
        m_EquipmentGrid.OnItemClick(0);
        m_SwapItem.SetInfo(m_SwapEquipment);
    }

    void OnItemSelect(int itemIndex)
    {
        m_SelectItem.SetInfo(m_Info.m_ActionEquipment[itemIndex]);
    }

    void OnConfirmBtnClick()
    {
        m_Info.SwapEquipment(m_EquipmentGrid.m_curSelecting,m_SwapEquipment);
        OnPageHide(true);
        base.OnCancelBtnClick();
    }

    protected override void OnCancelBtnClick()
    {
        OnPageHide(false);
        base.OnCancelBtnClick();
    }
}
