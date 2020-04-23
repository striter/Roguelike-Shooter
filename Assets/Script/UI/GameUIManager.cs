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
    }
    protected override void InitControls(bool inGame)
    {
        base.InitControls(inGame);
        ShowControls<UIC_GameNumericVisualize>();
        ShowControls<UIC_GameCurrencyStatus>();
    }

    public void OnGameFinished(GameProgressManager level, Action _OnButtonClick)
    {
        cvs_Camera.gameObject.SetActivate(false);
        ShowPage<UI_GameResult>(true).Play(level, _OnButtonClick);
    }
    
}
