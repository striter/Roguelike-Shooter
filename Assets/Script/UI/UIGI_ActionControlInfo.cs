using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class UIGI_ActionControlInfo : UIGI_ActionBase {
    UIT_TextExtend m_Name;
    public override void Init()
    {
        base.Init();
        m_Name = tf_Container.Find("Name").GetComponent<UIT_TextExtend>();
    }

    public override void SetInfo(ActionBase action)
    {
        base.SetInfo(action);
        m_Name.localizeKey = action.GetNameLocalizeKey();
    }
}
