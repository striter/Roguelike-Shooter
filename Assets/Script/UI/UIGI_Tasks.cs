using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TGameSave;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_Tasks : MonoBehaviour {

    [SerializeField] Text m_taskContent;
    [SerializeField] Button m_receiveBut;
    [SerializeField] GameObject m_received;

    int m_id;
    DailyTasksData m_data;
    private void Awake()
    {
        m_receiveBut.onClick.AddListener(OnClick);
        m_gameTask = GameDataManager.m_GameTaskData;
    }
    CGameTask m_gameTask;
    public void OnPlay(DailyTasksData data,int id)
    {
        m_id = id;
        m_data = data;
        bool isComplete = false;
        string str = "";
        if (id == 1088)
        {
            if (m_gameTask.m_signIn == 1)
            {
                m_receiveBut.SetActivate(true);
                m_received.SetActive(false);
            }
            else
            {
                m_received.SetActive(true);
                m_receiveBut.SetActivate(false);
            }
            m_taskContent.text = string.Format(TLocalization.GetKeyLocalized("UI_Task_SignIn"), data.m_num1);
        }
        else if (id == 1089)
        {
            if (m_gameTask.m_advertisement >0&& m_gameTask.m_advertisement> m_gameTask.m_advertisementCollection)
                m_receiveBut.SetActivate(true);
            else
                m_receiveBut.SetActivate(false);
            if (m_gameTask.m_advertisementCollection >= data.m_num2)
            {
                m_received.SetActive(true);
                m_receiveBut.SetActivate(false);
            }
            else
            {
                m_received.SetActive(false);
            }
            str= string.Format("{0}/{1}", m_gameTask.m_advertisementCollection, data.m_num2);
            m_taskContent.text = string.Format(TLocalization.GetKeyLocalized("UI_Task_Advertisement"), data.m_num1, str);
        }
        else 
        {
            
            switch (id%100)
            {
                case 0:
                    if (m_gameTask.m_killMonsters >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_killMonsters, data.m_num1);
                    break;
                case 1:
                    if (m_gameTask.m_PassTheGate >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_PassTheGate, data.m_num1);
                    break;
                case 2:
                    if (m_gameTask.m_getGoldCoins >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_getGoldCoins, data.m_num1);
                    break;
                case 3:
                    if (m_gameTask.m_killBoss >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_killBoss, data.m_num1);
                    break;
                case 4:
                    if (m_gameTask.m_portal >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_portal, data.m_num1);
                    break;
                case 6:
                    if (m_gameTask.m_getWeapons >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str =  data.m_num1.ToString();
                    break;
            }
            if (isComplete)
                str = string.Format("{0}/{1}", data.m_num1, data.m_num1);
            if (id < 100)
            {
                if (id == 5)
                {
                    if (m_gameTask.m_passTheGate > 0)
                    {
                        isComplete = true;
                    }
                    str = data.m_num1.ToString();
                }
                if (m_gameTask.m_goldCoinTaskState == 1)
                {
                    m_received.SetActive(true);
                    isComplete = false;
                }
                else
                {
                    m_received.SetActive(false);
                }
                m_taskContent.text = string.Format(TLocalization.GetKeyLocalized("UI_Task_Gold" + id), str, data.m_num2);
            }
            else
            {
                if (id == 105)
                {
                    if (m_gameTask.m_passTheGateNew > 0)
                    {
                        isComplete = true;
                    }
                    str = data.m_num1.ToString();
                }
                if (m_gameTask.m_diamondMissionState == 1)
                {
                    m_received.SetActive(true);
                    isComplete = false;
                }
                else
                {
                    m_received.SetActive(false);
                }
                m_taskContent.text = string.Format(TLocalization.GetKeyLocalized("UI_Task_Diamonds" + id % 100), str, data.m_num2);
            }
            m_receiveBut.SetActivate(isComplete);
        }

    }
    void OnClick()
    {
        if (m_id == 1088)
        {
            m_gameTask.m_signIn = -1;
            GameDataManager.OnCreditStatus(m_data.m_num1);
            m_received.SetActive(true);
            m_receiveBut.SetActivate(false);
        }
        else if (m_id == 1089)
        {
            m_gameTask.m_advertisementCollection += 1;
            GameDataManager.OnDiamondsStatus(m_data.m_num1);
            if (m_gameTask.m_advertisement > 0 && m_gameTask.m_advertisement > m_gameTask.m_advertisementCollection)
                m_receiveBut.SetActivate(true);
            else
                m_receiveBut.SetActivate(false);
            if (m_gameTask.m_advertisementCollection >= m_data.m_num2)
            {
                m_received.SetActive(true);
                m_receiveBut.SetActivate(false);
            }
            string str = string.Format("{0}/{1}", m_gameTask.m_advertisementCollection, m_data.m_num2);
            m_taskContent.text = string.Format(TLocalization.GetKeyLocalized("UI_Task_Advertisement"), m_data.m_num1, str);
        }
        else
        {
            if (m_id < 100)
            {
                m_gameTask.m_goldCoinTaskState = 1;
                GameDataManager.OnCreditStatus(m_data.m_num2);
            }
            else
            {
                m_gameTask.m_diamondMissionState = 1;
                GameDataManager.OnDiamondsStatus(m_data.m_num2);
            }
            m_received.SetActive(true);
            m_receiveBut.SetActivate(false);
        }
        TGameData<CGameTask>.Save();
    }
}
