using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCoin : InteractBase {
    protected override bool B_RecycleOnInteract => true;
    public override enum_Interaction m_InteractType => enum_Interaction.Coin;
    public override bool B_InteractOnTrigger => true;
    int m_CoinAmount;
    public void Play(int coinAmount)
    {
        base.Play();
        m_CoinAmount = coinAmount;
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_PlayerInfo.OnCoinsReceive(m_CoinAmount);
    }
}
