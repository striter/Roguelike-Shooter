using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampUIManager : UIManager {
    public static new CampUIManager Instance { get; private set; }
    UIC_GameProgress m_GameProgress;
    UIControlBase m_Coins, m_OverlayControl;
    Action OnCampPageExit;
    protected override void Init()
    {
        base.Init();
        Instance = this;
        m_Coins = ShowControls<UIC_CampCurrencyStatus>(false);
        if (m_firstEntry)
        {
            m_GameProgress = ShowControls<UIC_GameProgress>();
            m_firstEntry = false;
        }

        PreloadPage<UI_Armory>();
        PreloadPage<UI_CharacterSelect>();
        PreloadPage<UI_DailyTasks>();
        PreloadPage<UI_ShoppingMall>();
    }

    public T ShowCoinsPage<T>(bool animate,bool blurBG, Action OnPageExit,float bulletTime = 1f) where T : UIPage
    {
        m_OverlayControl = m_Coins;
        SetControlViewMode(m_OverlayControl, true);
        m_ControlSiblings.Traversal((UIControlBase control) => { if (control != m_OverlayControl) control.SetActivate(false); });
        OnCampPageExit = OnPageExit;
        return ShowPage<T>(animate,blurBG, bulletTime);
    }

    protected override void OnPageExit(UIPageBase page)
    {
        base.OnPageExit(page);
        if (!m_OverlayControl || m_PageOpening)
            return;
        SetControlViewMode(m_OverlayControl, false);
        m_ControlSiblings.Traversal((UIControlBase control) => { if (control != m_OverlayControl) control.SetActivate(true); });
        m_OverlayControl = null;
        OnCampPageExit?.Invoke();
        OnCampPageExit = null;
    }

    protected override void OnAdjustPageSibling()
    {
        base.OnAdjustPageSibling();
        if (m_OverlayControl)
            SetControlViewMode(m_OverlayControl, UIMessageBoxBase.m_MessageBox == null && I_PageCount == 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
}
