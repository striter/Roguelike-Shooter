using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBase : SimpleSingletonMono<UIManagerBase> {

    protected Canvas cvs_Overlay, cvs_Camera;
    private RectTransform tf_OverlayPage,tf_CameraPage,tf_OverlayControl,tf_CameraControl,tf_MessageBox;
    protected virtual void Init()
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

    protected virtual void OnAdjustPageSibling()
    {
        bool pageShow = UIMessageBoxBase.m_MessageBox == null;
        for (int i = 0; i < UIPageBase.m_Pages.Count; i++)
        {
            bool pageOverlay = pageShow && UIPageBase.m_Pages.Count - 1 == i;
            SetPageViewMode(UIPageBase.m_Pages[i], pageOverlay);
        }
    }

    protected virtual void OnPageExit()=> OnAdjustPageSibling();
    protected virtual void OnMessageBoxExit() => OnAdjustPageSibling();
    protected T ShowPage<T>(bool useAnim) where T : UIPageBase
    {
        T page = UIPageBase.Show<T>(tf_OverlayPage, useAnim);
        OnAdjustPageSibling();
        return page ;
    }
    protected T ShowMessageBox<T>() where T : UIMessageBoxBase
    {
        T messageBox = UIMessageBoxBase.Show<T>(tf_MessageBox);
        OnAdjustPageSibling();
        return messageBox;
    }

    Dictionary<UIControlBase, int> m_ControlSibiling = new Dictionary<UIControlBase, int>();
    protected T ShowControls<T>(bool overlayView=false)where T: UIControlBase
    {
        T control = UIControlBase.Show<T>(overlayView ? tf_OverlayControl : tf_CameraControl);
        m_ControlSibiling.Add(control,m_ControlSibiling.Count -1);
        return control;
    }

    protected void SetControlViewMode(UIControlBase control, bool overlay)
    {
        if (!m_ControlSibiling.ContainsKey(control))
        {
            Debug.LogError("?");
            return;
        }

        TCommonUI.ReparentRestretchUI(control.rectTransform, overlay ? tf_OverlayControl : tf_CameraControl);
        control.transform.SetSiblingIndex(m_ControlSibiling[control]);
    }
    protected void SetPageViewMode(UIPageBase page, bool overlay) => TCommonUI.ReparentRestretchUI(page.rectTransform, overlay ? tf_OverlayPage : tf_CameraPage);
}

