using GameSetting;
using System;
using TGameSave;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameResult : UIPage {
    Action OnButtonClick;
    Image m_ResultTitle;
    UIC_Button btn_video;
    [SerializeField] Text m_Level;
    [SerializeField] Text m_killMonsters;
    [SerializeField] Text m_difficulty;

    [SerializeField] Text m_goldCoin;
    protected override void Init()
    {
        base.Init();
        m_ResultTitle = rtf_Container.Find("ResultTitle").GetComponent<Image>();
        rtf_Container.Find("BtnShare").GetComponent<Button>().onClick.AddListener(OnShareBtnClick);
        btn_video = new UIC_Button(rtf_Container.Find("BtnVideo"), OnVideoBtnClick);
        //Transform tf_result = rtf_Container.Find("Result");
        //m_Completion = tf_result.Find("Completion").GetComponent<UIT_TextExtend>();
        //m_CompletionScore = tf_result.Find("CompletionScore").GetComponent<UIT_TextExtend>();
        //m_Difficulty = tf_result.Find("Difficulty").GetComponent<UIT_TextExtend>();
        //m_DifficultyMultiply = tf_result.Find("DifficultyMultiply").GetComponent<UIT_TextExtend>();
        //m_FinalScore = tf_result.Find("FinalScore").GetComponent<UIT_TextExtend>();
        //m_CoinsAmount = tf_result.Find("RewardsGrid/Coins/Container/Amount").GetComponent<UIT_TextExtend>();
    }
    public void Play(bool win,Action _OnButtonClick)
    {
        m_ResultTitle.sprite = BattleUIManager.Instance.m_InGameSprites[UIConvertions.GetUIGameResultTitleBG(win, OptionsDataManager.m_OptionsData.m_Region)];
        OnButtonClick = _OnButtonClick;

        m_Level.text = ((int)BattleManager.Instance.m_BattleProgress.m_Stage).ToString();
        m_killMonsters.text = BattleManager.Instance.m_BattleEntity.m_EnermyKilled.ToString();
        m_difficulty.text = ((int)BattleManager.Instance.m_BattleEntity.m_Difficulty).ToString();

        float stageCredit = ((int)BattleManager.Instance.m_BattleProgress.m_Stage - 1) * GameConst.F_GameResultCreditStageBase;
        float killCredit = BattleManager.Instance.m_BattleEntity.m_EnermyKilled * GameConst.F_GameResultCreditEnermyKilledBase;
        float difficultyBonus = (1f + ((int)BattleManager.Instance.m_BattleEntity.m_Difficulty - 1) * GameConst.F_GameResultCreditDifficultyBonus);
        int num = (int)(Math.Round(stageCredit + killCredit) * difficultyBonus);
        m_goldCoin.text = num.ToString();
    }
    protected override void OnCancelBtnClick()
    {
        GameDataManager.OnCGameTask();
        OnButtonClick?.Invoke();
        OnButtonClick = null;
    }

    void OnShareBtnClick()
    {

    }

    void OnVideoBtnClick()
    {
        btn_video.SetInteractable(false);
        GameDataManager.m_GameTaskData.m_advertisement++;
        Debug.Log("看广告了");
        TGameData<CGameTask>.Save();
    }
}
