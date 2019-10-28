using GameSetting;
using System;
using UnityEngine.UI;

public class UIGI_ActionItemDetail : UIGI_ActionItemBase {
    UIT_TextExtend m_Intro;
    Action<int> OnClick;
    public override void Init()
    {
        base.Init();
        m_Intro = transform.Find("Intro").GetComponent<UIT_TextExtend>();
        tf_Container.Find("Button").GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    public virtual void SetInfo(ActionBase actionInfo,Action<int> _OnClick,bool costable)
    {
        base.SetInfo(actionInfo);
        m_Intro.formatText(actionInfo.GetIntroLocalizeKey(), actionInfo.F_Duration, actionInfo.Value1, actionInfo.Value2, actionInfo.Value3);
        OnClick = _OnClick;

        SetCostable(costable);
    }

    void OnButtonClick()
    {
        OnClick?.Invoke(I_Index);
    }
}
