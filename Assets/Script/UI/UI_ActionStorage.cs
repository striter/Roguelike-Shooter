using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionStorage : UIPageBase {
    UIT_GridControllerGridItem<UIGI_ActionItemSelect> m_Grid;
    Action OnCancelClick;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridControllerGridItem<UIGI_ActionItemSelect>(tf_Container.Find("ScrollView/Viewport/ActionGrid"));   
    }

    public void Play(Action _OnCancelClick)
    {
        OnCancelClick = _OnCancelClick;

    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        OnCancelClick();
    }
}
