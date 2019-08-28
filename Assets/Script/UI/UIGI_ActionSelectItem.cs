using UnityEngine.UI;
using GameSetting;
public class UIGI_ActionSelectItem : UIT_GridDefaultItem
{
    Text txt_Intro;
    protected override void Init()
    {
        base.Init();
        txt_Intro = tf_Container.Find("IntroText").GetComponent<Text>();
    }
    public void SetInfo(ActionBase action)
    {
        base.SetItemInfo(action.NameKey().Localize(), false);
        txt_Intro.text = string.Format(action.IntroKey().Localize(), action.m_ExpireDuration, action.Value1, action.Value2, action.Value3);
    }
}
