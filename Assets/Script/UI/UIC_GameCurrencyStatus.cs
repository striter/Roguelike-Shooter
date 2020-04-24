using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TSpecialClasses;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameCurrencyStatus : UIControlBase {

    Text m_Coins,m_Keys,m_Ranks;
    ValueLerpSeconds m_CoinLerp;

    protected override void Init()
    {
        base.Init();
        m_Coins = transform.Find("CoinData/Data").GetComponent<Text>();
        m_CoinLerp = new ValueLerpSeconds(0f, 20f,1f,(float value)=> { m_Coins.text = ((int)value).ToString(); });
        m_Coins.text = "0";
        m_Keys = transform.Find("KeyData/Data").GetComponent<Text>();
        m_Ranks = transform.Find("RankData/Data").GetComponent<Text>();
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerExpireManager>(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, OnCurrencyUpdate);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerExpireManager>(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, OnCurrencyUpdate);
    }

    private void Update()
    {
        m_CoinLerp.TickDelta(Time.deltaTime);
    }

    void OnCurrencyUpdate(PlayerExpireManager _playerInfo)
    {
        m_CoinLerp.ChangeValue(_playerInfo.m_Coins);
        m_Keys.text = _playerInfo.m_Keys.ToString();
        m_Ranks.text = _playerInfo.m_RankManager.m_ExpCurRank + "|"+_playerInfo.m_RankManager.m_ExpToNextRank +","+ _playerInfo.m_RankManager.m_Rank;
    }
}
