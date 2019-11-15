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
        SetInfo(actionInfo);
        SetOnClick(_OnClick);
        SetCostable(costable);
    }

    protected void SetOnClick(Action<int> _OnClick) => OnClick = _OnClick;

    protected override void SetInfo(ActionBase actionInfo)
    {
        base.SetInfo(actionInfo);
        actionInfo.SetActionIntro( m_Intro);
    }

    void OnButtonClick()
    {
        OnClick?.Invoke(I_Index);
    }
}
