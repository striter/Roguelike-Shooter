using UnityEngine.UI;
using GameSetting;
public class UIGI_ActionSelectItem : UIT_GridDefaultItem
{
    UIT_TextExtend txt_Intro,txt_Level,txt_Cost,txt_Name;
    public override void Init()
    {
        base.Init();
        txt_Name = txt_Default as UIT_TextExtend;
        txt_Intro = tf_Container.Find("IntroText").GetComponent<UIT_TextExtend>();
        txt_Level = tf_Container.Find("LevelText").GetComponent<UIT_TextExtend>();
        txt_Cost = tf_Container.Find("CostText").GetComponent<UIT_TextExtend>();
    }
    public void SetInfo(ActionBase action)
    {
        base.SetItemInfo("", false);
        txt_Name.localizeKey = action.GetNameLocalizeKey();
        txt_Cost.text = action.m_ActionType == enum_ActionType.WeaponPerk ? "" : action.I_Cost.ToString();
        txt_Intro.formatText( action.GetIntroLocalizeKey(), action.F_Duration, action.Value1, action.Value2, action.Value3); 
        txt_Level.localizeKey = action.m_rarity.GetLocalizeKey();
    }
}
