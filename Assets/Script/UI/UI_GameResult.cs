using GameSetting;
using System;
using UnityEngine;

public class UI_GameResult : UIPageBase {
    Action OnButtonClick;

    UIT_TextExtend txt_Progress, txt_LevelScore, txt_KillScore, txt_DifficultyBonus, txt_FinalScore, txt_CoinsResult;
    Transform tf_Result;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        txt_Progress = tf_Container.Find("Progress/Data").GetComponent<UIT_TextExtend>();
        txt_LevelScore = tf_Container.Find("LevelScore/Data").GetComponent<UIT_TextExtend>();
        txt_KillScore = tf_Container.Find("KillScore/Data").GetComponent<UIT_TextExtend>();
        txt_DifficultyBonus = tf_Container.Find("DifficultyBonus/Data").GetComponent<UIT_TextExtend>();
        txt_FinalScore = tf_Container.Find("FinalScore/Data").GetComponent<UIT_TextExtend>();
        txt_CoinsResult = tf_Container.Find("CoinsResult/Data").GetComponent<UIT_TextExtend>();
    }

    public void Play(GameLevelManager level,Action _OnButtonClick)
    {
        tf_Result = tf_Container.Find("Result/" + (level.m_gameWin ? "Complete" : "Fail"));
        tf_Result.SetActivate(true);
        tf_Result.Find(OptionsManager.m_OptionsData.m_Region.ToString()).SetActivate(true);
        txt_Progress.text = string.Format("{0:000}%",(level.F_Progress*100f));
        txt_LevelScore.text =level.F_LevelScore.ToString();
        txt_KillScore.text =  level.F_KillScore.ToString();
        txt_DifficultyBonus.text = string.Format("x{0}", level.F_DifficultyBonus);
        txt_FinalScore.text =  level.F_FinalScore.ToString();
        txt_CoinsResult.text =  level.F_CreditGain.ToString();
        OnButtonClick = _OnButtonClick;
    }
    protected override void OnCancelBtnClick()
    {
        OnButtonClick();
    }
}
