using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionPack : UIPageBase {
    UIT_GridControllerGridItem<UIGI_ActionItemDetail> m_Grid;
    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControllerGridItem<UIGI_ActionItemDetail>(tf_Container.Find("ScrollView/Viewport/ActionGrid"));
    }

    public void Show(PlayerInfoManager _info)
    {
        m_Grid.ClearGrid();
        List<ActionBase> targetList = _info.m_ActionEquiping;
        for (int i = 0; i < targetList.Count; i++)
            m_Grid.AddItem(i).SetInfo(targetList[i], null, true);
    }
}
