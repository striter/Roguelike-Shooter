using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_ActionItemStorage : UIGI_ActionItemSelect {
    Text m_Count;
    public override void Init()
    {
        base.Init();
        m_Count = transform.Find("Count").GetComponent<Text>();
    }

    public void SetInfo(int actionIndex,ActionStorageData storageData,Action<int> OnItemClick)
    {
        SetInfo(ActionDataManager.CreateAction(actionIndex, storageData.GetRarityLevel()), OnItemClick, true);
        m_Count.text = storageData.m_Count.ToString();
        SetHighlight(false);
    }

    public void SetInfo(ActionStorageData data)
    {
        base.SetInfo(ActionDataManager.CreateAction(data.m_Index, data.GetRarityLevel()));
        m_Count.text = data.m_Count.ToString();
    }
}
