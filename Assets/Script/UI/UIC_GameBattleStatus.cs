using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameBattleStatus : UIControlBase {
    Text m_Data;
    protected override void Init()
    {
        base.Init();
        m_Data = transform.Find("Data").GetComponent<Text>();
    }

    private void Update()
    {
        m_Data.text = string.Format("{0},{1},{2}",(int)GameManager.Instance.m_GameLevel.m_TimeElapsed,GameManager.Instance.m_GameLevel.m_MinutesElapsed,GameManager.Instance.m_GameLevel.m_BattleTransmiting?"Battle":"Relax");

    }
}
