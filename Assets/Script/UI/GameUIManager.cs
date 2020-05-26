﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : UIManager {
    public static new GameUIManager Instance;
    public AtlasLoader m_InGameSprites { get; private set; }
    protected override void Init()
    {
        Instance = this;
        base.Init();
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
