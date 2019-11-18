using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UIGI_ActionItemRichIntro : UIGI_ActionItemBase {

    UIT_TextExtend m_Intro;
    public override void Init()
    {
        base.Init();
        m_Intro = transform.Find("Intro").GetComponent<UIT_TextExtend>();
    }
    public void SetCommonIntro(ActionBase actionInfo,string hexColor) => SetRichIntro(actionInfo,"FFFFFFFF", hexColor, hexColor,hexColor,hexColor);
    public void SetRichIntro(ActionBase actionInfo,string costColor,string durationColor,string value1Color,string value2Color,string value3Color)
    {
        base.SetInfo(actionInfo);
        m_Cost.color = TCommon.GetHexColor(costColor);
        m_Intro.formatText(actionInfo.GetIntroLocalizeKey(), string.Format("<color=#" + durationColor + ">{0}</color>", actionInfo.F_Duration), string.Format("<color=#" + value1Color + ">{0}</color>", actionInfo.Value1), string.Format("<color=#" + value2Color + ">{0}</color>", actionInfo.Value2), string.Format("<color=#" + value3Color + ">{0}</color>", actionInfo.Value3));
    }
}
