using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UI_ActionBattle : UIPageBase {

    UIT_GridControllerGridItem<UIGI_ActionItemDetail> m_Grid;
    PlayerInfoManager m_Info;
    UIC_ActionEnergy m_Energy,m_PreEnergy;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridControllerGridItem<UIGI_ActionItemDetail>(tf_Container.Find("ActionGrid"));
        m_Energy = new UIC_ActionEnergy(tf_Container.Find("ActionEnergy"));
    }
    public void Show(PlayerInfoManager _info,UIC_ActionEnergy _preEnergy)
    {
        m_Info = _info;
        m_PreEnergy = _preEnergy;
        m_PreEnergy.rectTransform.SetActivate(false);

        OnActionChanged(m_Info);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionChanged);
    }
    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        m_PreEnergy.rectTransform.SetActivate(true);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionChanged);
    }

    void OnActionChanged(PlayerInfoManager info)
    {
        m_Grid.ClearGrid();
        List<ActionBase> targetList =  info.m_BattleActionPicking;
        for (int i = 0; i < targetList.Count; i++)
            m_Grid.AddItem(i).SetInfo(targetList[i], OnItemClick,  info.B_EnergyCostable(targetList[i]));
        m_Energy.TickValue(info.m_ActionEnergy,1f);
    }
    void OnItemClick(int index)
    {
        m_Info.TryUseHoldingAction(index);
    }
}
