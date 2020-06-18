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
    [SerializeField] GameObject m_receiveNew;
    [SerializeField] GameObject m_goid;
    [SerializeField] GameObject m_diamonds;
    [SerializeField] Text m_currencyNum;
    [SerializeField] Text m_countDown;
    [SerializeField] Text m_completion;
    [SerializeField] Image m_progressBar;
    [SerializeField] GameObject m_reward;
    int m_id;
    DailyTasksData m_data;
    /// <summary>
    /// 状态 0未完成，1已完成，2已领取
    /// </summary>
    public int m_state;
    private void Awake()
    {
        m_receiveBut.onClick.AddListener(OnClick);
        m_gameTask = GameDataManager.m_GameTaskData;
    }
    int m_timeNew = 10;
    float m_relativeTime = 0;
    float m_timer = 0;
    private void Update()
    {
        if (m_timeNew == 0)
        {
            m_countDown.text = "";
            return;
        }
        if (m_relativeTime == 0)
        {
            m_relativeTime = Time.unscaledTime;
            m_timer = Time.unscaledTime;
        }
        if (Time.unscaledTime - m_relativeTime > 1)
        {
            m_relativeTime = Time.unscaledTime;
            int time = m_timeNew - (int)(m_relativeTime - m_timer);
            if (time <= 0)
            {
                m_timeNew = 0;
            }
            m_countDown.text = string.Format("{0}：{1}：{2}", string.Format("{0:d2}", time / 3600), string.Format("{0:d2}", time % 3600 / 60), string.Format("{0:d2}", time % 60));
        }
    }
    CGameTask m_gameTask;
    public void OnPlay(DailyTasksData data,int id)
    {
        m_timeNew = 0;
        m_state = 0;
        m_id = id;
        m_data = data;
        bool isComplete = false;
        string str = "";
        float jiduNew = 0;
        m_receiveNew.SetActive(false);
        m_countDown.gameObject.SetActive(false);
        m_completion.transform.parent.gameObject.SetActive(true);
        if (id == 1088)
        {
            if (m_gameTask.m_signIn == 1)
            {
                m_receiveBut.SetActivate(true);
                m_received.SetActive(false);
                m_state = 1;
            }
            else
            {
                m_received.SetActive(true);
                m_receiveBut.SetActivate(false);
                m_state = 2;
            }
            m_taskContent.text = TLocalization.GetKeyLocalized("UI_Task_SignIn");
            m_completion.text = "1/1";
            m_goid.SetActive(true);
            m_diamonds.SetActive(false);
            m_currencyNum.text = data.m_num1.ToString();
            m_progressBar.fillAmount = 1;
        }
        else if (id == 1089)
        {
            if (m_gameTask.m_advertisement > 0 && m_gameTask.m_advertisement > m_gameTask.m_advertisementCollection)
            {
                m_receiveBut.SetActivate(true);
                m_state = 1;
            }
            else
            {
                m_receiveBut.SetActivate(false);
                m_receiveNew.SetActive(true);
                m_state = 0;
            }
            if (m_gameTask.m_advertisementCollection >= data.m_num2)
            {
                m_received.SetActive(true);
                m_receiveBut.SetActivate(false);
                m_receiveNew.SetActive(false);
                m_state = 2;
            }
            else
            {
                m_received.SetActive(false);
            }
            m_taskContent.text = TLocalization.GetKeyLocalized("UI_Task_Advertisement");
            m_goid.SetActive(false);
            m_diamonds.SetActive(true);
            m_currencyNum.text = data.m_num1.ToString();

            m_countDown.gameObject.SetActive(true);
            m_completion.transform.parent.gameObject.SetActive(false);
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
                    jiduNew= (float)m_gameTask.m_killMonsters/ data.m_num1;
                    break;
                case 1:
                    if (m_gameTask.m_PassTheGate >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_PassTheGate, data.m_num1);
                    jiduNew = (float)m_gameTask.m_PassTheGate / data.m_num1;
                    break;
                case 2:
                    if (m_gameTask.m_getGoldCoins >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_getGoldCoins, data.m_num1);
                    jiduNew = (float)m_gameTask.m_getGoldCoins / data.m_num1;
                    break;
                case 3:
                    if (m_gameTask.m_killBoss >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_killBoss, data.m_num1);
                    jiduNew = (float)m_gameTask.m_killBoss / data.m_num1;
                    break;
                case 4:
                    if (m_gameTask.m_portal >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str = string.Format("{0}/{1}", m_gameTask.m_portal, data.m_num1);
                    jiduNew = (float)m_gameTask.m_portal / data.m_num1;
                    break;
                case 6:
                    if (m_gameTask.m_getWeapons >= data.m_num1)
                    {
                        isComplete = true;
                    }
                    str =  "0/1";
                    jiduNew = 0;
                    break;
            }
            if (isComplete)
            {
                if (id % 100 == 6)
                    str = "1/1";
                else
                    str = string.Format("{0}/{1}", data.m_num1, data.m_num1);
                jiduNew = 1;
            }
            if (id < 100)
            {
                if (id == 5)
                {
                    if (m_gameTask.m_passTheGate > 0)
                    {
                        isComplete = true;
                        str = "1/1";
                        jiduNew = 1;
                    }
                    else
                    {
                        str = "0/1";
                        jiduNew = 0;
                    }
                }
                if (m_gameTask.m_goldCoinTaskState == 1)
                {
                    m_received.SetActive(true);
                    isComplete = false;
                    m_state = 2;
                }
                else
                {
                    m_received.SetActive(false);
                }
                m_goid.SetActive(true);
                m_diamonds.SetActive(false);
                m_currencyNum.text = data.m_num2.ToString();
                m_taskContent.text = TLocalization.GetKeyLocalized("UI_Task_Gold" + id);
            }
            else
            {
                if (id == 105)
                {
                    if (m_gameTask.m_passTheGateNew > 0)
                    {
                        isComplete = true;
                        str = "1/1";
                        jiduNew = 1;
                    }
                    else
                    {
                        str = "0/1";
                        jiduNew = 0;
                    }
                }
                if (m_gameTask.m_diamondMissionState == 1)
                {
                    m_received.SetActive(true);
                    isComplete = false;
                    m_state = 2;
                }
                else
                {
                    m_received.SetActive(false);
                }
                m_goid.SetActive(false);
                m_diamonds.SetActive(true);
                m_currencyNum.text = data.m_num2.ToString();
                m_taskContent.text = TLocalization.GetKeyLocalized("UI_Task_Diamonds" + id % 100);
            }
            m_completion.text = str;
            m_receiveBut.SetActivate(isComplete);
            m_progressBar.fillAmount = jiduNew;

            if (isComplete)
            {
                m_state = 1;
            }
        }
        if (m_state == 2)
        {
            m_reward.SetActive(false);
        }
        else
        {
            m_reward.SetActive(true);
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
            transform.SetAsLastSibling();
        }
        else if (m_id == 1089)
        {
            m_timeNew = 10;
            m_gameTask.m_advertisementCollection += 1;
            GameDataManager.OnDiamondsStatus(m_data.m_num1);
            if (m_gameTask.m_advertisement > 0 && m_gameTask.m_advertisement > m_gameTask.m_advertisementCollection)
                m_receiveBut.SetActivate(true);
            else
            {
                m_receiveBut.SetActivate(false);
                m_receiveNew.SetActivate(true);
            }
            if (m_gameTask.m_advertisementCollection >= m_data.m_num2)
            {
                m_receiveNew.SetActivate(false);
                m_received.SetActive(true);
                m_receiveBut.SetActivate(false);
                transform.SetAsLastSibling();
                m_timeNew = 0;
            }
            //string str = string.Format("{0}/{1}", m_gameTask.m_advertisementCollection, m_data.m_num2);
            //m_taskContent.text = string.Format(TLocalization.GetKeyLocalized("UI_Task_Advertisement"), m_data.m_num1, str);
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
            transform.SetAsLastSibling();
        }
        TGameData<CGameTask>.Save();
    }
}
