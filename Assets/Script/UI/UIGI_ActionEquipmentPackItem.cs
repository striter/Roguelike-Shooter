using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class UIGI_ActionEquipmentPackItem : UIGI_ActionEquipmentSelect {
    Transform m_Empty;
    public override void Init()
    {
        base.Init();
        m_Empty = transform.Find("Empty");
    }
    public override void SetInfo(EquipmentExpire action)
    {
        bool validAction = action != null;
        m_Button.interactable = validAction;
        m_Empty.SetActivate(!validAction);
        tf_Container.SetActivate(validAction);
        if (validAction)
            base.SetInfo(action);
    }
}
