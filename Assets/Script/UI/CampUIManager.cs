using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampUIManager : UIManager {
    public static new CampUIManager Instance { get; private set; }
    UIControlBase m_Coins, m_OverlayControl;
    Action OnCampPageExit;
    protected override void Init()
    {
        base.Init();
        Instance = this;
    }
    protected override void InitControls(bool inGame)
    {
        base.InitControls(inGame);
        m_Coins=ShowControls<UIC_CurrencyStatus>(false);
    }

    public T ShowPage<T>(bool animate,bool blurBG, Action OnPageExit,float bulletTime = 1f) where T : UIPage
    {
        m_OverlayControl = m_Coins;
        SetControlViewMode(m_OverlayControl, true);
        OnCampPageExit = OnPageExit;
        return ShowPage<T>(animate,blurBG, bulletTime);
    }

    protected override void OnPageExit()
    {
        base.OnPageExit();
        if (!m_OverlayControl || UIPageBase.I_PageCount != 0)
            return;
        SetControlViewMode(m_OverlayControl, false);
        m_OverlayControl = null;
        OnCampPageExit?.Invoke();
        OnCampPageExit = null;
    }

    protected override void OnAdjustPageSibling()
    {
        base.OnAdjustPageSibling();
        if (m_OverlayControl)
            SetControlViewMode(m_OverlayControl, UIMessageBoxBase.m_MessageBox == null && UIPageBase.I_PageCount == 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
}
