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

    public void Play(bool win,float levelScore,float killScore,float coin,Action _OnButtonClick)
    {
        tf_Result = tf_Container.Find("Result/" + (win ? "Complete" : "Fail"));
        tf_Result.Find(OptionsManager.m_OptionsData.m_Region.ToString()).SetActivate(true);
        txt_LevelScore.text =levelScore.ToString();
        txt_KillScore.text =  killScore.ToString();
        txt_CoinsResult.text =  coin.ToString();
        OnButtonClick = _OnButtonClick;
    }
    protected override void OnCancelBtnClick()
    {
        OnButtonClick();
    }
}
