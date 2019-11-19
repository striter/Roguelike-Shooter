using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBase : SimpleSingletonMono<UIManagerBase> {

    protected Canvas cvs_Overlay, cvs_Camera;
    private RectTransform tf_OverlayPage,tf_CameraPage,tf_OverlayControl,tf_CameraControl,tf_MessageBox;
    protected void Init()
    {
        cvs_Overlay = transform.Find("Overlay").GetComponent<Canvas>();
        cvs_Camera = transform.Find("Camera").GetComponent<Canvas>();
        tf_OverlayPage = cvs_Overlay.transform.Find("Page").GetComponent<RectTransform>();
        tf_CameraPage = cvs_Camera.transform.Find("Page").GetComponent<RectTransform>();
        
        tf_MessageBox = cvs_Overlay.transform.Find("MessageBox").GetComponent<RectTransform>();

        UIPageBase.OnPageExit = OnPageExit;
        UIMessageBoxBase.OnMessageBoxExit = OnMessageBoxExit;

        tf_OverlayControl = cvs_Overlay.transform.Find("Control").GetComponent<RectTransform>();
        tf_CameraControl = cvs_Camera.transform.Find("Control").GetComponent<RectTransform>();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        UIPageBase.OnPageExit = null;
        UIMessageBoxBase.OnMessageBoxExit = null;
    }

    void AdjustPageSibling()
    {
        bool pageShow = UIMessageBoxBase.m_MessageBox == null;
        for (int i = 0; i < UIPageBase.m_Pages.Count; i++)
        {
            bool pageOverlay = pageShow && UIPageBase.m_Pages.Count - 1 == i;
            SetPageViewMode(UIPageBase.m_Pages[i], pageOverlay);
        }
    }

    protected virtual void OnPageExit()=> AdjustPageSibling();
    protected virtual void OnMessageBoxExit() => AdjustPageSibling();
    protected T ShowPage<T>(bool useAnim) where T : UIPageBase
    {
        T page = UIPageBase.Show<T>(tf_OverlayPage, useAnim);
        AdjustPageSibling();
        return page ;
    }
    protected T ShowMessageBox<T>() where T : UIMessageBoxBase
    {
        T messageBox = UIMessageBoxBase.Show<T>(tf_MessageBox);
        AdjustPageSibling();
        return messageBox;
    }

    protected T ShowControls<T>(bool overlayView = false)where T: UIControlBase
    {
        return  UIControlBase.Show<T>(overlayView ? tf_OverlayControl : tf_CameraControl); ;
    }

    protected void SetControlViewMode(UIControlBase control, bool overlay) => TCommonUI.ReparentRestretchUI(control.rectTransform, overlay ? tf_OverlayControl : tf_CameraControl );
    protected void SetPageViewMode(UIPageBase page, bool overlay) => TCommonUI.ReparentRestretchUI(page.rectTransform, overlay ? tf_OverlayPage : tf_CameraPage);
}

