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
        SetCount(storageData);
        SetHighlight(false);
    }

    public void SetInfo(ActionStorageData data)
    {
        base.SetInfo(ActionDataManager.CreateAction(data.m_Index, data.GetRarityLevel()));
        SetCount(data);
    }
    void SetCount(ActionStorageData data)
    {
        enum_RarityLevel rarity = data.GetRarityLevel();
        if (rarity == enum_RarityLevel.Normal)
            m_Count.text = data.m_Count.ToString() + "/" + GameConst.I_CampActionStorageOutstandingCount.ToString();
        else if (rarity == enum_RarityLevel.OutStanding)
            m_Count.text = data.m_Count.ToString() + "/" + GameConst.I_CampActionStorageEpicCount.ToString();
        else if (rarity == enum_RarityLevel.Epic)
            m_Count.text = "Epic!";
        else
            m_Count.text = data.m_Count.ToString() + "/" + GameConst.I_CampActionStorageNormalCount.ToString();
    }
}
