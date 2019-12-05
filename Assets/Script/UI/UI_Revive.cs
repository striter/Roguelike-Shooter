using System;
using UnityEngine.UI;
public class UI_Revive : UIPage
{
    Action OnRevive;
    Button btn_Revive;
    protected override void Init()
    {
        base.Init();
        btn_Revive = tf_Container.Find("BtnRevive").GetComponent<Button>();
        btn_Revive.onClick.AddListener(OnVideoBtnClick);
    }

    public void Play(Action _OnVideoFinished)
    {
        OnRevive = _OnVideoFinished;
    }

    void OnVideoBtnClick()
    {
        OnRevive();
        OnCancelBtnClick();
    }
    
    
}
