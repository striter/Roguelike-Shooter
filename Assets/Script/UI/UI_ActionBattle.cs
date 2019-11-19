using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UI_ActionBattle : UIPageBase {

    UIT_GridControllerGridItem<UIGI_ActionItemDetail> m_Grid;
    PlayerInfoManager m_Info;
    UIC_ActionEnergy m_Energy;
    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControllerGridItem<UIGI_ActionItemDetail>(tf_Container.Find("ActionGrid"));
        m_Energy = new UIC_ActionEnergy(tf_Container.Find("ActionEnergy"));
    }

    public void Show(PlayerInfoManager _info)
    {
        m_Info = _info;
        OnActionChanged(m_Info);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionChanged);
    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionChanged);
    }

    void OnActionChanged(PlayerInfoManager info)
    {
        m_Grid.ClearGrid();
        List<ActionBase> targetList =  info.m_BattleActionPicking;
        for (int i = 0; i < targetList.Count; i++)
            m_Grid.AddItem(i).SetInfo(targetList[i], OnItemClick,  info.CanCostEnergy(targetList[i].I_Cost));
        m_Energy.SetValue(info.m_ActionEnergy);
    }

    void OnItemClick(int index)
    {
        m_Info.TryUseHoldingAction(index);
    }
}
