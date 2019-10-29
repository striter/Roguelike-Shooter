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

    public void SetInfo(ActionBase actionInfo,string hexColor)
    {
        base.SetInfo(actionInfo);
        string format = "<color=#" + hexColor + ">{0}</color>";
        m_Intro.formatText(actionInfo.GetIntroLocalizeKey(), actionInfo.F_Duration, string.Format(format, actionInfo.Value1), string.Format(format, actionInfo.Value2), string.Format(format, actionInfo.Value3));
    }
}
