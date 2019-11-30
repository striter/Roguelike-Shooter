using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UI_ActionBattle : UIPageBase {

    UIT_GridControllerGridItem<UIGI_ActionItemSelect> m_Grid;
    PlayerInfoManager m_Info;
    UIC_Button btn_Confirm;
    int m_selectIndex = -1;
    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControllerGridItem<UIGI_ActionItemSelect>(tf_Container.Find("ActionGrid"));
        btn_Confirm =new UIC_Button( tf_Container.Find("ConfirmBtn"),OnConfirmBtnClick);
        btn_Confirm.SetInteractable(false);
    }

    public void Show(PlayerInfoManager _info)
    {
        m_Info = _info;
        OnBattleActionChanged(m_Info);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerBattleActionStatus, OnBattleActionChanged);
    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerBattleActionStatus, OnBattleActionChanged);
    }

    void OnBattleActionChanged(PlayerInfoManager info)
    {
        m_selectIndex = -1;
        btn_Confirm.SetInteractable(false);
        m_Grid.ClearGrid();
        List<ActionBase> targetList =  info.m_BattleActionPicking;
        for (int i = 0; i < targetList.Count; i++)
            m_Grid.AddItem(i).SetInfo(targetList[i], OnItemClick,  info.CanCostEnergy(targetList[i].I_Cost));
    }

    void OnItemClick(int index)
    {
        if (m_selectIndex != -1)
            m_Grid.GetItem(m_selectIndex).SetHighlight(false);
        m_selectIndex = index;
        m_Grid.GetItem(m_selectIndex).SetHighlight(true);
        btn_Confirm.SetInteractable(true);
    }
    void OnConfirmBtnClick()
    {
        if (m_selectIndex < 0)
            return;
        m_Info.TryUsePickingAction(m_selectIndex);
    }

}
