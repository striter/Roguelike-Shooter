using GameSetting;
using System;

public class UI_GameResult : UIPageBase {
    UIT_TextExtend txt_Result, txt_LevelScore,txt_KillScore, txt_coins;
    Action OnButtonClick;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        txt_Result = tf_Container.Find("Result").GetComponent<UIT_TextExtend>();
        txt_LevelScore = tf_Container.Find("LevelScore").GetComponent<UIT_TextExtend>();
        txt_KillScore = tf_Container.Find("KillScore").GetComponent<UIT_TextExtend>();
        txt_coins = tf_Container.Find("Coins").GetComponent<UIT_TextExtend>();
    }

    public void Play(bool win,float levelScore,float killScore,float coin,Action _OnButtonClick)
    {
        txt_Result.text=win?"Victory":"Defeat";
        txt_LevelScore.text = "Level Score:"+ levelScore.ToString();
        txt_KillScore.text = "Kill Score" + killScore.ToString();
        txt_coins.text = "Coin:" + coin.ToString();
        OnButtonClick = _OnButtonClick;
    }
    protected override void OnCancelBtnClick()
    {
        OnButtonClick();
    }
}
