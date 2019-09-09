using UnityEngine.UI;
using GameSetting;
public class UIGI_ActionSelectItem : UIT_GridDefaultItem
{
    UIT_TextLocalization txt_Intro,txt_Level,txt_Cost,txt_Name;
    protected override void Init()
    {
        base.Init();
        if (txt_Name)
            return;
        txt_Name = txt_Default as UIT_TextLocalization;
        txt_Intro = tf_Container.Find("IntroText").GetComponent<UIT_TextLocalization>();
        txt_Level = tf_Container.Find("LevelText").GetComponent<UIT_TextLocalization>();
        txt_Cost = tf_Container.Find("CostText").GetComponent<UIT_TextLocalization>();
    }
    public void SetInfo(ActionBase action)
    {
        base.SetItemInfo("", false);
        txt_Name.localizeText = action.GetNameLocalizeKey();
        txt_Cost.text = action.m_ActionExpireType == enum_ActionExpireType.AfterWeaponSwitch ? "" : action.I_ActionCost.ToString();
        UnityEngine.Debug.Log(action.m_ExpireDuration);
        txt_Intro.formatText( action.GetIntroLocalizeKey(), action.F_Duration, action.Value1, action.Value2, action.Value3); 
        txt_Level.localizeText = action.m_Level.GetLocalizeKey();
    }
}
