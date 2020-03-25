using GameSetting;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameResult : UIPage {
    Action OnButtonClick;
    Image m_ResultTitle;
    UIC_Button btn_video;
    UIT_TextExtend m_Completion, m_CompletionScore, m_Difficulty, m_DifficultyMultiply, m_FinalScore;
    UIT_TextExtend m_CoinsAmount;

    protected override void Init()
    {
        base.Init();
        m_ResultTitle = rtf_Container.Find("ResultTitle").GetComponent<Image>();
        rtf_Container.Find("BtnShare").GetComponent<Button>().onClick.AddListener(OnShareBtnClick);
        btn_video = new UIC_Button(rtf_Container.Find("BtnVideo"),OnVideoBtnClick);
        Transform tf_result = rtf_Container.Find("Result");
        m_Completion = tf_result.Find("Completion").GetComponent<UIT_TextExtend>();
        m_CompletionScore = tf_result.Find("CompletionScore").GetComponent<UIT_TextExtend>();
        m_Difficulty = tf_result.Find("Difficulty").GetComponent<UIT_TextExtend>();
        m_DifficultyMultiply = tf_result.Find("DifficultyMultiply").GetComponent<UIT_TextExtend>();
        m_FinalScore = tf_result.Find("FinalScore").GetComponent<UIT_TextExtend>();
        m_CoinsAmount = tf_result.Find("RewardsGrid/Coins/Container/Amount").GetComponent<UIT_TextExtend>();
    }
    GameProgressManager m_level;
    public void Play(GameProgressManager level,Action _OnButtonClick)
    {
        m_level = level;
        m_ResultTitle.sprite = GameUIManager.Instance.m_InGameSprites[UIConvertions.GetUIGameResultTitleBG(level.m_GameWin,OptionsManager.m_OptionsData.m_Region)];
        m_Completion.text = string.Format("{0}%", (int)level.F_Completion*100f);
        m_CompletionScore.text = string.Format("{0}", (int)level.F_CompletionScore);
        m_Difficulty.text = string.Format("{0}", level.m_GameDifficulty);
        m_DifficultyMultiply.text = string.Format("x{0:N1}", level.F_DifficultyBonus);
        m_FinalScore.text = string.Format("{0}",(int)level.F_FinalScore);
        m_CoinsAmount.text = string.Format("{0}", (int)level.F_CreditGain);
        OnButtonClick = _OnButtonClick;
    }
    protected override void OnCancelBtnClick()
    {
        OnButtonClick?.Invoke();
        OnButtonClick = null;
    }

    void OnShareBtnClick()
    {

    }

    void OnVideoBtnClick()
    {
        GameDataManager.OnCreditStatus(m_level.F_CreditGain*.2f);
        m_CoinsAmount.text = string.Format("{0}", (int)(m_level.F_CreditGain * 1.2f));
        btn_video.SetInteractable(false);
    }
}
