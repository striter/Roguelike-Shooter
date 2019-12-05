using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : UIManager {
    public static new GameUIManager Instance;
    public AtlasLoader m_InGameSprites { get; private set; }
    UIControlBase m_Coins,m_OverlayControl;
    protected override void Init()
    {
        Instance = this;
        base.Init();
        m_InGameSprites = TResources.GetUIAtlas_InGame();
    }
    protected override void InitGameControls(bool inGame)
    {
        base.InitGameControls(inGame);
        ShowControls<UIC_EntityHealth>();
        m_Coins = ShowControls<UIC_CoinsStatus>();
    }

    public void OnGameFinished(GameLevelManager level, Action _OnButtonClick)
    {
        cvs_Camera.gameObject.SetActivate(false);
        ShowPage<UI_GameResult>(true).Play(level, _OnButtonClick);
    }

    public T ShowCoinsPage<T>(bool animate, float bulletTime = 1f) where T:UIPage
    {
        m_OverlayControl = m_Coins;
        SetControlViewMode(m_OverlayControl, true);
        return ShowPage<T>(animate, bulletTime);
    }
    protected override void OnPageExit()
    {
        base.OnPageExit();
        if (!m_OverlayControl||UIPageBase.I_PageCount != 0)
            return;
        SetControlViewMode(m_OverlayControl, false);
        m_OverlayControl = null;
    }
    protected override void OnAdjustPageSibling()
    {
        base.OnAdjustPageSibling();
        if(m_OverlayControl)
            SetControlViewMode(m_OverlayControl,UIMessageBoxBase.m_MessageBox==null&&UIPageBase.I_PageCount==1);
    }
}
