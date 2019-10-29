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
    public override void SetInfo(ActionBase actionInfo, Action<int> _OnClick, bool costable)
    {
        base.SetInfo(actionInfo, _OnClick, costable);
        SetHighlight(false);
    }
    public void SetHighlight(bool highLight)
    {
        m_Highlight.SetActivate(highLight);
    }
}
