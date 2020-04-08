using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameStatus : UIControlBase {

    Transform tf_Level;
    UIT_TextExtend m_Level_Info, m_Level_Index;
    Image m_Level_Icon;
    Text m_EndlessData;

    protected override void Init()
    {
        base.Init();
        tf_Level = transform.Find("Level");
        m_Level_Icon = tf_Level.Find("Icon").GetComponent<Image>();
        m_Level_Index = tf_Level.Find("Index").GetComponent<UIT_TextExtend>();
        m_Level_Info = tf_Level.Find("Info").GetComponent<UIT_TextExtend>();

        m_EndlessData = transform.Find("EndlessData").GetComponent<Text>();
        m_EndlessData.SetActivate(false);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnLevelStart, OnLevelStart);
        TBroadCaster<enum_BC_GameStatus>.Add<int, float>(enum_BC_GameStatus.OnEndlessData, OnEndlessData);

    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnLevelStart, OnLevelStart);
        TBroadCaster<enum_BC_GameStatus>.Remove<int, float>(enum_BC_GameStatus.OnEndlessData, OnEndlessData);
    }
    void OnLevelStart()
    {
        m_Level_Icon.sprite = GameUIManager.Instance.m_InGameSprites[GameManager.Instance.m_GameLevel.GetLevelIconSprite()];
        m_Level_Index.formatText("UI_Level_Index", "<Color=#F8C64BFF>" + (GameManager.Instance.m_GameLevel.m_LevelPassed + 1) + "</Color>");
        m_Level_Info.localizeKey = GameManager.Instance.m_GameLevel.GetLevelInfoKey();
    }

    void OnEndlessData(int kills, float damage)
    {
        m_EndlessData.SetActivate(true);
        m_EndlessData.text = string.Format("Kills:{0},Damage:{1}", kills, damage);
    }
}
