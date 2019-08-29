﻿using UnityEngine.UI;
using GameSetting;
public class UIGI_ActionSelectItem : UIT_GridDefaultItem
{
    UIT_TextLocalization txt_Intro;
    Text txt_Level;
    protected override void Init()
    {
        base.Init();
        txt_Intro = tf_Container.Find("IntroText").GetComponent<UIT_TextLocalization>();
        txt_Level = tf_Container.Find("LevelText").GetComponent<Text>();
    }
    public void SetInfo(ActionBase action)
    {
        base.SetItemInfo(action.GetNameLocalizeKey(), false);
        txt_Intro.formatText( action.GetIntroLocalizeKey(), action.m_ExpireDuration, action.Value1, action.Value2, action.Value3); 
        txt_Level.text = action.m_Level.GetLocalizeKey();
    }
}
