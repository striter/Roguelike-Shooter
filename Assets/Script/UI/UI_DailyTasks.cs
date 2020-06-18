using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyTasksData
{
    public int m_num1 = 0;
    public int m_num2 = 0;

    public DailyTasksData(int num1,int num2=0)
    {
        m_num1 = num1;
        m_num2 = num2;
    }
}
public class UI_DailyTasks : UIPage
{
    DailyTasksData[] m_goidTask = new DailyTasksData[7];
    DailyTasksData[] m_diamondsTask = new DailyTasksData[7];

    DailyTasksData SignInTask;
    DailyTasksData AdvertisementTask;

    [SerializeField] UIGI_Tasks[] m_tasksList = new UIGI_Tasks[4];
    protected override void Init()
    {
        base.Init();

        SignInTask = new DailyTasksData(1000);
        AdvertisementTask = new DailyTasksData(20,3);

        m_goidTask[0] = new DailyTasksData(100,1000);
        m_goidTask[1] = new DailyTasksData(5, 2000);
        m_goidTask[2] = new DailyTasksData(500, 2000);
        m_goidTask[3] = new DailyTasksData(5, 2000);
        m_goidTask[4] = new DailyTasksData(6, 2000);
        m_goidTask[5] = new DailyTasksData(4, 3000);
        m_goidTask[6] = new DailyTasksData(5, 3000);

        m_diamondsTask[0] = new DailyTasksData(200, 10);
        m_diamondsTask[1] = new DailyTasksData(10, 20);
        m_diamondsTask[2] = new DailyTasksData(800, 20);
        m_diamondsTask[3] = new DailyTasksData(8, 20);
        m_diamondsTask[4] = new DailyTasksData(10, 20);
        m_diamondsTask[5] = new DailyTasksData(4, 30);
        m_diamondsTask[6] = new DailyTasksData(7, 30);

    }

    public override void OnPlay(bool doAnim, Action<UIPageBase> OnPageExit)
    {
        base.OnPlay(doAnim, OnPageExit);
        GameDataManager.m_GameTaskData.RandomTask();
        int goldCoinTask = GameDataManager.m_GameTaskData.m_goldCoinTask;
        int diamondMission = GameDataManager.m_GameTaskData.m_diamondMission;

        m_tasksList[0].OnPlay(SignInTask,1088);
        m_tasksList[1].OnPlay(AdvertisementTask, 1089);
        m_tasksList[2].OnPlay(m_goidTask[goldCoinTask], goldCoinTask);
        m_tasksList[3].OnPlay(m_diamondsTask[diamondMission], diamondMission+100);

        for (int i = 0; i < m_tasksList.Length; i++)
        {
            if (m_tasksList[i].m_state == 0)
            {
                m_tasksList[i].transform.SetAsLastSibling();
            }
        }

        for (int i = 0; i < m_tasksList.Length; i++)
        {
            if (m_tasksList[i].m_state == 2)
            {
                m_tasksList[i].transform.SetAsLastSibling();
            }
        }
    }

}

