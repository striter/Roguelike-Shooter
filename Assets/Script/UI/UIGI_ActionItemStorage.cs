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

    public void SetStorageInfo(int actionIndex,ActionStorageData storageData,Action<int> OnItemClick)
    {
        SetOnClick(OnItemClick);
        UpdateInfo(storageData, actionIndex);
    }

    public void UpdateInfo(ActionStorageData data, int actionIndex=-1)
    {
        enum_RarityLevel rarity = data.GetRarityLevel();
        bool costable = rarity != enum_RarityLevel.Invalid;
        if (actionIndex == -1) actionIndex = data.m_Index;
        if (!costable) rarity = enum_RarityLevel.Normal;
        SetInfo(ActionDataManager.CreateAction(actionIndex,rarity));
        SetCostable(costable);
        SetCount(data);
        SetHighlight(!costable);
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
