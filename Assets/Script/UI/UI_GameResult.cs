using System;

public class UI_GameResult : UIPageBase {
    UIT_TextLocalization txt_Result, txt_LevelScore,txt_KillScore, txt_coins;
    Action OnButtonClick;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        txt_Result = tf_Container.Find("Result").GetComponent<UIT_TextLocalization>();
        txt_LevelScore = tf_Container.Find("LevelScore").GetComponent<UIT_TextLocalization>();
        txt_KillScore = tf_Container.Find("KillScore").GetComponent<UIT_TextLocalization>();
        txt_coins = tf_Container.Find("Coins").GetComponent<UIT_TextLocalization>();
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
