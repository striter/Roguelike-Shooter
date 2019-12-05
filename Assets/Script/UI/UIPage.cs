using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UIPage : UIPageBase {
    public enum_UIVFX Audio_PageOpen= enum_UIVFX.Invalid;
    public enum_UIVFX Audio_PageExit= enum_UIVFX.Invalid;
    protected override void Init()
    {
        base.Init();
        if(Audio_PageOpen!= enum_UIVFX.Invalid)
            AudioManager.Instance.Play2DClip(-1, Audio_PageOpen);
    }
    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        if (Audio_PageExit != enum_UIVFX.Invalid)
            AudioManager.Instance.Play2DClip(-1, Audio_PageExit);
    }
}
