using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : UIManager {
    public static new BattleUIManager Instance;
    public AtlasLoader m_InGameSprites { get; private set; }
    protected override void Init()
    {
        Instance = this;
        base.Init();
        UIManager.Instance.DisplayUI();
        m_InGameSprites = TResources.GetUIAtlas_InGame();
        ShowControls<UIC_GameNumericVisualize>();
        ShowControls<UIC_GameStatus>();

        PreloadPage<UI_PlayerDetail>();
        PreloadPage<UI_Map>();
        PreloadPage<UI_Revive>();
        PreloadPage<UI_GameResult>();
    }

    public void OnGameFinished(BattleProgressManager level, Action _OnButtonClick)
    {
        cvs_Camera.gameObject.SetActivate(false);
        ShowPage<UI_GameResult>(true).Play(level.m_GameWin,_OnButtonClick);
    }
    
}
