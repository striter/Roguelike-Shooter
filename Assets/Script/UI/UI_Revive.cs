using System;
using UnityEngine.UI;
public class UI_Revive : UIPageBase
{
    Action OnVideoFinished, OnCreditClick;
    Func<float,bool> CheckCanCreditClick;
    Button btn_Video, btn_Credit;
    Text txt_Credit;
    float m_reviveCredit;
    protected override void Init()
    {
        base.Init();
        btn_Video = tf_Container.Find("BtnVideo").GetComponent<Button>();
        btn_Video.onClick.AddListener(OnVideoBtnClick);
        btn_Credit = tf_Container.Find("BtnCredit").GetComponent<Button>();
        btn_Credit.onClick.AddListener(OnCreditBtnClick);
        txt_Credit = btn_Credit.transform.Find("Amount").GetComponent<Text>();
    }

    public void Play(Action _OnVideoFinished,Action _OnCreditClick,float credit, Func<float,bool> _CheckCanCreditClick)
    {
        m_reviveCredit = credit;
        txt_Credit.text = m_reviveCredit.ToString();
        OnVideoFinished = _OnVideoFinished;
        OnCreditClick = _OnCreditClick;
        CheckCanCreditClick = _CheckCanCreditClick;
    }

    void OnVideoBtnClick()
    {
        OnVideoFinished();
        OnCancelBtnClick();
    }

    void OnCreditBtnClick()
    {
        if (!CheckCanCreditClick(m_reviveCredit))
            return;

        OnCreditClick();
        OnCancelBtnClick();
    }
    
}
