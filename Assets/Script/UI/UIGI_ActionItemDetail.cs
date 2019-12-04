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
    public virtual void SetDetailInfo(ActionBase actionInfo,Action<int> _OnClick)
    {
        SetInfo(actionInfo);
        actionInfo.SetActionIntro(m_Intro);
        SetOnClick(_OnClick);
    }

    protected void SetOnClick(Action<int> _OnClick) => OnClick = _OnClick;
    
    void OnButtonClick()
    {
        OnClick?.Invoke(I_Index);
    }
}
