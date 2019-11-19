using GameSetting;
using System;
using UnityEngine;

public class UI_GameResult : UIPageBase {
    Action OnButtonClick;
    UIT_TextExtend txt_Progress, txt_LevelScore, txt_KillScore, txt_DifficultyBonus, txt_FinalScore, txt_CoinsResult;
    Transform tf_Result;
    Transform tf_ProgressStatus;
    UIT_TextExtend txt_Difficulty;
    RectTransform rtf_CurrentStatus;
    Transform tf_Stage1, tf_Stage2, tf_Stage3,tf_Goal;
    protected override void Init()
    {
        base.Init();
        txt_Progress = tf_Container.Find("Progress/Data").GetComponent<UIT_TextExtend>();
        txt_LevelScore = tf_Container.Find("LevelScore/Data").GetComponent<UIT_TextExtend>();
        txt_KillScore = tf_Container.Find("KillScore/Data").GetComponent<UIT_TextExtend>();
        txt_DifficultyBonus = tf_Container.Find("DifficultyBonus/Data").GetComponent<UIT_TextExtend>();
        txt_FinalScore = tf_Container.Find("FinalScore/Data").GetComponent<UIT_TextExtend>();
        txt_CoinsResult = tf_Container.Find("CoinsResult/Data").GetComponent<UIT_TextExtend>();

        tf_ProgressStatus = tf_Container.Find("ProgressStatus");
        txt_Difficulty = tf_ProgressStatus.Find("Difficulty").GetComponent<UIT_TextExtend>();
        rtf_CurrentStatus = tf_ProgressStatus.Find("CurrentStatus").GetComponent<RectTransform>();

        tf_Stage1 = tf_ProgressStatus.Find("Stage1/Passed");
        tf_Stage2 = tf_ProgressStatus.Find("Stage2/Passed");
        tf_Stage3 = tf_ProgressStatus.Find("Stage3/Passed");
        tf_Goal = tf_ProgressStatus.Find("Goal");
    }

    public void Play(GameLevelManager level,Action _OnButtonClick)
    {
        tf_Result = tf_Container.Find("Result/" + (level.m_gameWin ? "Complete" : "Fail"));
        tf_Result.SetActivate(true);

        float progress = level.F_Progress;
        rtf_CurrentStatus.anchorMax = new Vector2(rtf_CurrentStatus.anchorMax.x,progress);
        rtf_CurrentStatus.anchorMin = new Vector2(rtf_CurrentStatus.anchorMin.x,progress);
        txt_Difficulty.formatText("UI_Result_Difficulty", level.m_GameDifficulty);
        tf_Stage1.SetActivate(level.m_GameStage > enum_StageLevel.Rookie);
        tf_Stage2.SetActivate(level.m_GameStage > enum_StageLevel.Veteran);
        tf_Stage3.SetActivate(level.m_gameWin);
        tf_Goal.SetActivate(level.m_gameWin);

        tf_Result.Find(OptionsManager.m_OptionsData.m_Region.ToString()).SetActivate(true);
        txt_Progress.text = string.Format("{0}%",Mathf.CeilToInt(progress*100f));
        txt_LevelScore.text =level.F_LevelScore.ToString();
        txt_KillScore.text =  level.F_KillScore.ToString();
        txt_DifficultyBonus.text = string.Format("x{0}", Mathf.CeilToInt(level.F_DifficultyBonus));
        txt_FinalScore.text = string.Format("{0}", Mathf.CeilToInt( level.F_FinalScore));
        txt_CoinsResult.text = string.Format("{0}", Mathf.CeilToInt(level.F_CreditGain));
        OnButtonClick = _OnButtonClick;
    }
    protected override void OnCancelBtnClick()
    {
        OnButtonClick?.Invoke();
        OnButtonClick = null;
    }
}
