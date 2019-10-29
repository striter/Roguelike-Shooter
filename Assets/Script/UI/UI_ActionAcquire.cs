﻿using GameSetting;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_ActionAcquire : UIPageBase {

    Action<int> OnIndexSelect;
    UIC_Button m_Confirm;
    UIT_GridControllerMonoItem<UIGI_ActionItemSelect> m_Grid;
    int m_selectIndex;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Confirm =new UIC_Button( tf_Container.Find("Confirm"),OnConfirmClick);
        m_Grid = new UIT_GridControllerMonoItem<UIGI_ActionItemSelect>(tf_Container.Find("ActionGrid"));
    }
    public void Play(List<ActionBase> actions,Action<int> _OnIndexSelect, int selectAmount)
    {
        m_Grid.m_GridLayout.spacing = new UnityEngine.Vector2(m_Grid.m_GridLayout.spacing.x*2/actions.Count,m_Grid.m_GridLayout.spacing.y);
        m_Confirm.SetInteractable(false);
        m_selectIndex = -1;
        OnIndexSelect = _OnIndexSelect;
        m_Grid.ClearGrid();
        for (int i = 0; i < actions.Count; i++)
            m_Grid.AddItem(i).SetInfo(actions[i],OnItemSelected,true);
    }

    void OnItemSelected(int index)
    {
        if (m_selectIndex != -1)
            m_Grid.GetItem(m_selectIndex).SetHighlight(false);
        m_selectIndex = index;
        m_Grid.GetItem(m_selectIndex).SetHighlight(true);

        m_Confirm.SetInteractable(true);
    }

    void OnConfirmClick()
    {
        m_Confirm.SetInteractable(false);
        OnIndexSelect(m_selectIndex);
        OnCancelBtnClick();
    }
}
