using System;
using UnityEngine.UI;
public class UI_Revive : UIPage
{
    Action OnRevive,OnCancel;
    Button btn_Revive;
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
    }

    void OnReviveBtnClick()
    {
        OnRevive();
        base.OnCancelBtnClick();
    }

    protected override void OnCancelBtnClick()
    {
        OnCancel();
        base.OnCancelBtnClick();
    }
}
