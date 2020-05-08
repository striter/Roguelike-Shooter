using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class UIGI_ActionEquipmentPackItem : UIGI_ActionEquipmentSelect {
    Transform m_Empty;
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Empty = transform.Find("Empty");
    }
    public override void SetInfo(ExpirePlayerPerkBase action)
    {
        bool validAction = action != null;
        m_Button.interactable = validAction;
        m_Empty.SetActivate(!validAction);
        rtf_Container.SetActivate(validAction);
        if (validAction)
            base.SetInfo(action);
    }
}
