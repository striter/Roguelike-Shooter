using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UIPage : UIPageBase {
    public AudioClip Audio_PageOpen = null;
    public AudioClip Audio_PageExit = null;
    protected override void Init()
    {
        base.Init();
        if(Audio_PageOpen)
            AudioManager.Instance.Play2DClip(-1, Audio_PageOpen);
    }
    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        if (Audio_PageExit)
            AudioManager.Instance.Play2DClip(-1, Audio_PageExit);
    }
}
