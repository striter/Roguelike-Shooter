using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIGI_EquipmentItemEquipping : UIGI_EquipmentItemBase {
    Action<int> OnButtonClick;
    public override void OnInitItem()
    {
        base.OnInitItem();
        rtf_Container.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnButtonClick?.Invoke(m_Identity); });
    }

    public void Play(EquipmentSaveData data,Action<int> OnButtonClick)
    {
        this.OnButtonClick = OnButtonClick;
        Play(data);
    }
}
