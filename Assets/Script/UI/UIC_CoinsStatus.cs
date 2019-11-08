using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TSpecialClasses;
using UnityEngine;
using UnityEngine.UI;

public class UIC_CoinsStatus : UIControlBase {

    Text m_Coins;
    DurationLerp m_CoinLerp;

    protected override void Init()
    {
        base.Init();
        m_Coins = transform.Find("CoinData/Data").GetComponent<Text>();
        m_CoinLerp = new DurationLerp(0f, 1f);
        m_Coins.text = "0";
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
    }

    private void Update()
    {
        if (m_CoinLerp.TickLerp(Time.deltaTime))
            m_Coins.text = ((int)(m_CoinLerp.m_value)).ToString();
    }

    void OnCommonStatus(EntityCharacterPlayer _player)
    {
        m_CoinLerp.ChangeValue(_player.m_PlayerInfo.m_Coins);
    }
}
