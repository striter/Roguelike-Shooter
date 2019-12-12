using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ActionEquipmentSelect : UIGI_ActionBase,IGridHighlight {
    
    UIT_TextExtend m_Name;
    Transform m_Selected;

    public override void Init()
    {
        base.Init();
        m_Name = tf_Container.Find("Name").GetComponent<UIT_TextExtend>();
        m_Selected = tf_Container.Find("Selected");
    }
    public void AttachSelectButton(Action<int> OnButtonClick)=>GetComponent<Button>().onClick.AddListener(()=> { OnButtonClick(I_Index); });


    public void OnHighlight(bool highlight)
    {
        m_Selected.SetActivate(highlight);
        m_Name.color = highlight ? TCommon.GetHexColor("FE9E00FF") : TCommon.GetHexColor("FFFFFFFF");
    }

    public override void SetInfo(ActionBase action)
    {
        base.SetInfo(action);
        m_Name.localizeKey = action.GetNameLocalizeKey();
    }
}
