using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ActionItemSelect : UIGI_ActionItemDetail {
    Image m_Highlight;
    public override void Init()
    {
        base.Init();
        m_Highlight = tf_Container.Find("Highlight").GetComponent<Image>();
    }
    public override void SetDetailInfo(ActionBase actionInfo, Action<int> _OnClick)
    {
        base.SetDetailInfo(actionInfo, _OnClick);
        SetHighlight(false);
    }
    public void SetHighlight(bool highLight)
    {
        m_Highlight.SetActivate(highLight);
    }
}
