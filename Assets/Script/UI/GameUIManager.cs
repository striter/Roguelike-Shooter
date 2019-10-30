using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : UIManager {
    public static new GameUIManager Instance;
    protected override void Init()
    {
        Instance = this;
        base.Init();
        m_InGameSprites.Check();
        m_ActionSprites.Check();
        ShowTools<UI_EntityHealth>();
        ShowTools<UI_GamePlayerStatus>();
        cvs_Overlay.transform.Find("Test/SeedTest").GetComponent<Text>().text = GameManager.Instance.m_GameLevel.m_Seed;   //Test
    }

    public void OnGameFinished(GameLevelManager level, Action _OnButtonClick)
    {
        cvs_Camera.gameObject.SetActivate(false);
        ShowPage<UI_GameResult>(true).Play(level, _OnButtonClick);
    }
    AtlasLoader m_inGameSprites;
    public AtlasLoader m_InGameSprites
    {
        get
        {
            if (m_inGameSprites == null)
                m_inGameSprites = TResources.GetUIAtlas_InGame();
            return m_inGameSprites;
        }
    }
    AtlasLoader m_actionSprites;
    public AtlasLoader m_ActionSprites
    {
        get
        {
            if (m_actionSprites == null)
                m_actionSprites = TResources.GetUIAtlas_Action();
            return m_actionSprites;
        }
    }
}
