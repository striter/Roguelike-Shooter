using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionPack : UIPageBase {
    UIT_GridControllerGridItem<UIGI_ActionItemDetail> m_Grid;
    PlayerInfoManager m_Info;
    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControllerGridItem<UIGI_ActionItemDetail>(tf_Container.Find("ScrollView/Viewport/ActionGrid"));
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
        List<ActionBase> targetList = info.m_BattleAction;
        for (int i = 0; i <targetList.Count; i++)
            m_Grid.AddItem(i).SetInfo(targetList[i],null,true);
    }
}
