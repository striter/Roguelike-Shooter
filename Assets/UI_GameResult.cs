using System;

public class UI_GameResult : UIPageBase {
    UIT_TextLocalization txt_Result, txt_Score, txt_coins;
    Action OnButtonClick;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        txt_Result = tf_Container.Find("Result").GetComponent<UIT_TextLocalization>();
        txt_Score = tf_Container.Find("Score").GetComponent<UIT_TextLocalization>();
        txt_coins = tf_Container.Find("Coins").GetComponent<UIT_TextLocalization>();
    }

    public void Play(bool win,float score,float coin,Action _OnButtonClick)
    {
        txt_Result.text=win?"Victory":"Defeat";
        txt_Score.text = "Score:"+ score.ToString();
        txt_coins.text = "Coin:" + coin.ToString();
        OnButtonClick = _OnButtonClick;
    }
    protected override void OnCancelBtnClick()
    {
        OnButtonClick();
    }
}
