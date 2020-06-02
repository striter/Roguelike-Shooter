using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameProgress : UIControlBase
{
    [SerializeField] Button[] m_buttonList = new Button[2];
    protected override void Init()
    {
        for (int i = 0; i < m_buttonList.Length; i++)
        {
            int num = i;
            m_buttonList[i].onClick.AddListener(delegate () { OnClick(num); });
        }

        m_buttonList[1].SetActivate(GameDataManager.m_GameData.m_BattleResume);

    }

    public void OnClick(int num)
    {
        if (num == 0)
        {
            UIManager.Instance.DisplayUI();
            GameDataManager.m_BattleResume = true;
        }
        else
            CampManager.Instance.OnBattleStart(false);
        UIManager.Instance.closeControlsUI(this);
    }
}
