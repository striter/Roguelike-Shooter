using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionStorage : UIPageBase {
    UIT_GridControllerMonoItem<UIGI_ActionSelectItem> m_Grid;
    PlayerInfoManager m_Info;
    bool showStored;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridDefaultSingle<UIGI_ActionSelectItem>(tf_Container.Find("ActionGrid"),OnItemClick,false,false);
    }
    public void Show(bool _showStored, PlayerInfoManager _info)
    {
        m_Info = _info;
        showStored = _showStored;
        OnActionChanged(m_Info);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionChanged);
        if (!showStored)
            GameManagerBase.SetBulletTime(true,0f);
    }
    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionStatus, OnActionChanged);
        if (!showStored)
            GameManagerBase.SetBulletTime(false);
    }

    void OnActionChanged(PlayerInfoManager info)
    {
        m_Grid.ClearGrid();
        List<ActionBase> targetList = showStored ?info.m_ActionStored: info.m_ActionHolding;
        for (int i = 0; i <targetList.Count; i++)
            m_Grid.AddItem(i).SetInfo(targetList[i]);
    }
    void OnItemClick(int index)
    {
        if (showStored)
            return;

        m_Info.TryUseAction(index);
    }
}
