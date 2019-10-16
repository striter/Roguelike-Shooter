using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;
using UnityEngine.UI;

public class UIGI_ActionItemDetail : UIGI_ActionItemBase {
    UIT_TextExtend m_Intro;
    Action<int> OnClick;
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        m_Intro = tf_Container.Find("Intro").GetComponent<UIT_TextExtend>();
        tf_Container.Find("Button").GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    public void SetInfo(PlayerInfoManager info, ActionBase actionInfo,Action<int> _OnClick)
    {
        base.SetInfo(info,actionInfo);
        m_Intro.formatText(actionInfo.GetIntroLocalizeKey(), actionInfo.F_Duration, actionInfo.Value1, actionInfo.Value2, actionInfo.Value3);
        OnClick = _OnClick;
    }

    void OnButtonClick()
    {
        OnClick?.Invoke(I_Index);
    }
}
