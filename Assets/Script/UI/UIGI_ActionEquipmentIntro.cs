using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class UIGI_ActionEquipmentIntro : UIGI_ActionBase {
    UIT_TextExtend m_Name, m_Intro;
    public override void Init()
    {
        base.Init();
        m_Name = tf_Container.Find("Name").GetComponent<UIT_TextExtend>();
        m_Intro = tf_Container.Find("Intro").GetComponent<UIT_TextExtend>();
    }
    public override void SetInfo(ActionBase action)
    {
        base.SetInfo(action);
        m_Name.localizeKey = action.GetNameLocalizeKey();
        m_Intro.formatText(action.GetIntroLocalizeKey(), string.Format("<color=#FFDA6BFF>{0}</color>", action.F_Duration), string.Format("<color=#FFDA6BFF>{0}</color>", action.Value1), string.Format("<color=#FFDA6BFF>{0}</color>", action.Value2), string.Format("<color=#FFDA6BFF>{0}</color>", action.Value3));
    }
}
