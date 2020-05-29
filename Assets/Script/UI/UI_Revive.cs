using System;
using UnityEngine.UI;
public class UI_Revive : UIPage
{
    Action OnRevive,OnCancel;
    Button btn_Revive;
    bool m_Canceled = false;
    protected override void Init()
    {
        base.Init();
        btn_Revive = rtf_Container.Find("BtnRevive").GetComponent<Button>();
        btn_Revive.onClick.AddListener(OnReviveBtnClick);
    }

    public void Play(Action _OnRevive,Action _OnCancel)
    {
        OnRevive = _OnRevive;
        OnCancel = _OnCancel;
        m_Canceled = false;
    }

    void OnReviveBtnClick()
    {
        m_Canceled = false;
        base.OnCancelBtnClick();
    }

    protected override void OnCancelBtnClick()
    {
        m_Canceled = true;
        base.OnCancelBtnClick();
    }

    public override void OnStop()
    {
        base.OnStop();
        if (m_Canceled)
            OnCancel();
        else
            OnRevive();
    }
}
