﻿using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ActionBase : UIT_GridItem {
    private UIC_EquipmentData m_Action=null;
    protected virtual UIC_EquipmentData GetActionDataBase(Transform container)=>new UIC_EquipmentData(container);
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Action = GetActionDataBase(rtf_Container);
    }
    public virtual void SetInfo(ExpirePlayerPerkBase action)
    {
        m_Action.SetInfo(action);
    }
}
