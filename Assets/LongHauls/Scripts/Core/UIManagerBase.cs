﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerBase : SingletonMono<UIManagerBase> {
    public float m_fittedScale { get; private set; }
    protected Canvas cvs_Overlay, cvs_Camera;
    private RectTransform tf_OverlayPage, tf_CameraPage, tf_OverlayControl, tf_CameraControl, tf_MessageBox;
    Transform m_PageStorage;
    public List<UIPageBase> m_Pages = new List<UIPageBase>();

    Dictionary<Type, UIPageBase> m_PageStored = new Dictionary<Type, UIPageBase>();

    public int I_PageCount => m_Pages.Count;
    public bool m_PageOpening => I_PageCount > 0;
    public bool CheckPageOpening<T>() where T : UIPageBase => m_Pages.Count > 0 && m_Pages.Find(p => p.GetType() == typeof(T));
    public Dictionary<UIControlBase, int> m_Controls { get; private set; } = new Dictionary<UIControlBase, int>();

    protected virtual void Init()
    {
        cvs_Overlay = transform.Find("Overlay").GetComponent<Canvas>();
        cvs_Camera = transform.Find("Camera").GetComponent<Canvas>();
        tf_OverlayPage = cvs_Overlay.transform.Find("Page").GetComponent<RectTransform>();
        tf_CameraPage = cvs_Camera.transform.Find("Page").GetComponent<RectTransform>();
        
        tf_MessageBox = cvs_Overlay.transform.Find("MessageBox").GetComponent<RectTransform>();

        tf_OverlayControl = cvs_Overlay.transform.Find("Control").GetComponent<RectTransform>();
        tf_CameraControl = cvs_Camera.transform.Find("Control").GetComponent<RectTransform>();

        m_PageStorage = transform.Find("PageStorage");

        CanvasScaler scaler = cvs_Overlay.GetComponent<CanvasScaler>();
        m_fittedScale = ((float)Screen.height / Screen.width)/(scaler.referenceResolution.y/scaler.referenceResolution.x);

        UIMessageBoxBase.OnMessageBoxExit = OnMessageBoxExit;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        UIMessageBoxBase.OnMessageBoxExit = null;
    }

    protected virtual void OnAdjustPageSibling()
    {
        bool pageShow = UIMessageBoxBase.m_MessageBox == null;
        for (int i = 0; i < m_Pages.Count; i++)
        {
            bool pageOverlay = pageShow && m_Pages.Count - 1 == i;
            SetPageViewMode(m_Pages[i], pageOverlay);
        }
    }

    protected T ShowPage<T>(bool useAnim) where T : UIPageBase
    {
        if (CheckPageOpening<T>())
            return null;

        T page=null;
        Type type = typeof(T);
        if (!m_PageStored.ContainsKey(typeof(T)))
          m_PageStored.Add(type, UIPageBase.Generate<T>(m_PageStorage));

        page = m_PageStored[type] as T;
        page.SetActivate(true);

        page.OnPlay(useAnim,OnPageExit);
        m_Pages.Add(page);
        OnAdjustPageSibling();
        return page ;
    }
    protected virtual void OnPageExit(UIPageBase page)
    {
        page.SetActivate(false);
        page.transform.SetParent(m_PageStorage);

        m_Pages.Remove(page);
        OnAdjustPageSibling();
    }

    protected virtual void OnMessageBoxExit() => OnAdjustPageSibling();
    protected T ShowMessageBox<T>() where T : UIMessageBoxBase
    {
        T messageBox = UIMessageBoxBase.Show<T>(tf_MessageBox);
        OnAdjustPageSibling();
        return messageBox;
    }

    protected T ShowControls<T>(bool overlayView=false)where T: UIControlBase
    {
        T control = UIControlBase.Show<T>(overlayView ? tf_OverlayControl : tf_CameraControl);
        m_Controls.Add(control,m_Controls.Count -1);
        return control;
    }

    protected void SetControlViewMode(UIControlBase control, bool overlay)
    {
        if (!m_Controls.ContainsKey(control))
        {
            Debug.LogError("?");
            return;
        }

        TCommonUI.ReparentRestretchUI(control.rectTransform, overlay ? tf_OverlayControl : tf_CameraControl);
        control.transform.SetSiblingIndex(m_Controls[control]);
    }
    protected void SetPageViewMode(UIPageBase page, bool overlay) => TCommonUI.ReparentRestretchUI(page.rectTransform, overlay ? tf_OverlayPage : tf_CameraPage);
}

