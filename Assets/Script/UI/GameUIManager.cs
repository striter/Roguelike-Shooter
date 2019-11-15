using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : UIManager {
    public static new GameUIManager Instance;
    public AtlasLoader m_InGameSprites { get; private set; }
    UIC_CoinsStatus m_Coins;
    bool m_CoinsOverlay;
    protected override void Init()
    {
        Instance = this;
        base.Init();
        m_InGameSprites = TResources.GetUIAtlas_InGame();
        ShowControls<UIC_EntityHealth>();
        ShowControls<UIC_PlayerInteract>();
        ShowControls<UIC_PlayerStatus>().SetInGame(true);
        m_Coins = ShowControls<UIC_CoinsStatus>();
        m_CoinsOverlay = false;
        cvs_Overlay.transform.Find("Test/SeedTest").GetComponent<Text>().text = GameManager.Instance.m_GameLevel.m_Seed;   //Test
    }

    public void OnGameFinished(GameLevelManager level, Action _OnButtonClick)
    {
        cvs_Camera.gameObject.SetActivate(false);
        ShowPage<UI_GameResult>(true).Play(level, _OnButtonClick);
    }

    
    public T ShowPage<T>(bool animate, float bulletTime = 1f, bool showCoins=false) where T:UIPageBase
    {
        m_CoinsOverlay = showCoins;
        if (m_CoinsOverlay)
            SetControlViewMode(m_Coins, true);

        return base.ShowPage<T>(animate, bulletTime);
    }

    protected override void OnPageExit()
    {
        base.OnPageExit();
        if(m_CoinsOverlay)
            SetControlViewMode(m_Coins, false);
        m_CoinsOverlay = false;
    }
}
