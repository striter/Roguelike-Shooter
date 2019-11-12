using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;
using TTime;
public class UI_ActionStorage : UIPageBase {
    UIT_GridControllerGridItemScrollView<UIGI_ActionItemStorage> m_Grid;
    ScrollRect m_ScrollView;
    Action OnCancelClick;
    Button btn_request, btn_switchMode;
    Text txt_request, txt_switchMode;
    List<ActionStorageData> m_Data=> GameDataManager.m_GameData.m_StorageActions;
    bool m_RequestMode;
    int m_RequestIndex = -1;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridControllerGridItemScrollView<UIGI_ActionItemStorage>(tf_Container.Find("ScrollView/Viewport/ActionGrid"));
        m_ScrollView = tf_Container.Find("ScrollView").GetComponent<ScrollRect>();
        btn_request = tf_Container.Find("Request").GetComponent<Button>();
        btn_request.onClick.AddListener(OnRequestClick);
        btn_switchMode = tf_Container.Find("SwitchMode").GetComponent<Button>();
        btn_switchMode.onClick.AddListener(OnSwitchModeClick);
        txt_request = btn_request.transform.Find("Text").GetComponent<Text>();
        txt_switchMode = btn_switchMode.transform.Find("Text").GetComponent<Text>();
    }
    private void Update()
    {
        m_Grid.CheckVisible(m_ScrollView.verticalNormalizedPosition,9);
        int stampLeft = GetActionStorageRequestTimeLeft( TTimeTools.GetTimeStampNow());
        bool requestAvailable = stampLeft < 0;
        txt_request.text = requestAvailable ?  "Request Available": "Request Inbound In:\n" + stampLeft.ToString();
        btn_request.interactable = m_RequestMode && m_RequestIndex != -1 && requestAvailable;
    }

    public void Play(Action _OnCancelClick)
    {
        OnCancelClick = _OnCancelClick;
        m_Grid.ClearGrid();
        for(int i=0;i<ActionDataManager.m_UseableAction.Count;i++)
        {
            int actionIndex = ActionDataManager.m_UseableAction[i];
            ActionStorageData data =m_Data.Find(p => p.m_Index == actionIndex);
            m_Grid.AddItem(i).SetStorageInfo(actionIndex, data,OnItemClick);
        }
        SwitchMode(false);
    }

    void OnSwitchModeClick() => SwitchMode(!m_RequestMode);

    void SwitchMode(bool requestMode)
    {
        m_RequestMode = requestMode;
        txt_switchMode.text = m_RequestMode ? "Request" : "Select";
        for(int i=0;i<m_Grid.I_Count;i++)
        {
            bool highLight = false;
            if (m_RequestMode)
                highLight = m_RequestIndex == i;
            else
            {
                int dataIndex = GetValidActionStorageIndex(i);
                highLight = dataIndex != -1 && m_Data[dataIndex].GetRarityLevel()>= enum_RarityLevel.Normal;
            }

            m_Grid.GetItem(i).SetHighlight(highLight);
        }
    }

    void OnItemClick(int index)
    {
        if (!m_RequestMode)
            return;

        if (m_RequestIndex != -1)
            m_Grid.GetItem(m_RequestIndex).SetHighlight(false);
        m_RequestIndex = index;
        m_Grid.GetItem(m_RequestIndex).SetHighlight(true);
    }

    void OnRequestClick()
    {
        if (!m_RequestMode||m_RequestIndex==-1)
            return;

        m_Grid.GetItem(m_RequestIndex).UpdateInfo(ActionStorageRequest(m_RequestIndex));
        m_Grid.GetItem(m_RequestIndex).SetHighlight(false);
        m_RequestIndex = -1;
    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        GameDataManager.SaveActionStorageData();
        OnCancelClick();
    }

    ActionStorageData ActionStorageRequest(int index)
    {
        int actionIndex = ActionDataManager.m_UseableAction[index];

        int dataIndex = m_Data.FindIndex(p => p.m_Index == actionIndex);
        if (dataIndex == -1)
        {
            m_Data.Add(ActionStorageData.CreateNewData(actionIndex));
            dataIndex = m_Data.Count - 1;
        }
        ActionStorageData data = m_Data[dataIndex];
        int countSurplus= data.OnRequestCount(GameExpression.I_CampActionStorageRequestAmount.Random());
        m_Data[dataIndex] = data;
        CampManager.Instance.OnCreditStatus(countSurplus *GameConst.I_CampActionCreditGainPerRequestSurplus);
        GameDataManager.m_GameData.m_StorageRequestStamp = TTimeTools.GetTimeStampNow();
        GameDataManager.SaveActionStorageData();
        return data;
    }
    
    int GetActionStorageRequestTimeLeft(int stampNow) => GameDataManager.m_GameData.m_StorageRequestStamp + GameConst.I_CampActionStorageRequestStampDuration - stampNow;
    int GetValidActionStorageIndex(int index)
    {
        int actionIndex = ActionDataManager.m_UseableAction[index];
        int dataIndex = m_Data.FindIndex(p => p.m_Index == actionIndex);
        if (dataIndex != -1 && m_Data[dataIndex].GetRarityLevel() < enum_RarityLevel.Normal)
            dataIndex = -1;
        return dataIndex;
    }
}
