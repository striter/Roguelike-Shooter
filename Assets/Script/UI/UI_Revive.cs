using System;
using UnityEngine.UI;
public class UI_Revive : UIPageBase
{
    Action OnVideoFinished, OnCreditClick;
    Button btn_Video, btn_Credit;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        btn_Video = tf_Container.Find("BtnVideo").GetComponent<Button>();
        btn_Video.onClick.AddListener(OnVideoBtnClick);
        btn_Credit = tf_Container.Find("BtnCredit").GetComponent<Button>();
        btn_Credit.onClick.AddListener(OnCreditBtnClick);
    }

    public void Play(Action _OnVideoFinished,Action _OnCreditClick)
    {
        OnVideoFinished = _OnVideoFinished;
        OnCreditClick = _OnCreditClick;
    }

    void OnVideoBtnClick()
    {
        OnVideoFinished();
    }

    void OnCreditBtnClick()
    {
        OnCreditClick();
    }
    
}
