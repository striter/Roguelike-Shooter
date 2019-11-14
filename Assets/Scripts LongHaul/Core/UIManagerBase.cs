using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBase : SimpleSingletonMono<UIManagerBase> {

    protected Canvas cvs_Overlay, cvs_Camera;
    private RectTransform tf_Page, tf_OverlayControl,tf_CameraControl,tf_MessageBox;
    protected virtual void Init()
    {
        cvs_Overlay = transform.Find("Overlay").GetComponent<Canvas>();
        cvs_Camera = transform.Find("Camera").GetComponent<Canvas>();
        tf_Page = cvs_Overlay.transform.Find("Page").GetComponent<RectTransform>();

        tf_OverlayControl = cvs_Overlay.transform.Find("Control").GetComponent<RectTransform>();
        tf_CameraControl = cvs_Camera.transform.Find("Control").GetComponent<RectTransform>();
        tf_MessageBox = cvs_Overlay.transform.Find("MessageBox").GetComponent<RectTransform>();
    }

    protected T ShowPage<T>(bool useAnim) where T : UIPageBase => UIPageBase.Show<T>(tf_Page,useAnim);
    protected T ShowControls<T>() where T : UIControlBase => UIControlBase.Show<T>(tf_CameraControl);
    protected T ShowMessageBox<T>() where T : UIMessageBoxBase => UIMessageBoxBase.Show<T>(tf_MessageBox);

    protected void SetControlViewMode(UIControlBase control, bool overlay) => TCommonUI.ReparentRestretchUI(control.rectTransform, overlay ? tf_OverlayControl : tf_CameraControl );
    protected void SetPageViewMode(bool overlay)
    {
        if (overlay)
        {
            tf_Page.ReparentRestretchUI(cvs_Overlay.transform);
            tf_Page.SetSiblingIndex(1);
        }
        else
        {
            tf_Page.ReparentRestretchUI(cvs_Camera.transform);
            tf_Page.SetAsLastSibling();
        }
    }
}

