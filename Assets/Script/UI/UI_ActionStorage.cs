using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;
public class UI_ActionStorage : UIPageBase {
    UIT_GridControllerGridItemScrollView<UIGI_ActionItemSelect> m_Grid;
    Action OnCancelClick;
    ScrollRect m_ScrollView;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridControllerGridItemScrollView<UIGI_ActionItemSelect>(tf_Container.Find("ScrollView/Viewport/ActionGrid"));
        m_ScrollView = tf_Container.Find("ScrollView").GetComponent<ScrollRect>();
    }
    private void Update()
    {
        m_Grid.CheckVisible(m_ScrollView.verticalNormalizedPosition,9);
    }

    public void Play(Action _OnCancelClick)
    {
        OnCancelClick = _OnCancelClick;
        m_Grid.ClearGrid();
        for(int i=0;i<ActionDataManager.m_Action.Count;i++)
        {
            int index = ActionDataManager.m_Action[i];
            ActionStorageData data = GameDataManager.m_PlayerCampData.m_StorageActions.Find(p => p.m_Index == index);
            enum_RarityLevel rarity = data.GetRarityLevel();
            ActionBase action =  ActionDataManager.CreateAction(index,rarity== enum_RarityLevel.Invalid? enum_RarityLevel.Normal:rarity);
            m_Grid.AddItem(i).SetInfo(action,OnItemClick,rarity!= enum_RarityLevel.Invalid);
        }
    }

    void OnItemClick(int index)
    {

    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        OnCancelClick();
    }
}
