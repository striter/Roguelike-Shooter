using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupCoin : InteractPickup {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupCoin;
    public int m_CoinAmount { get; private set; }
    public InteractPickupCoin Play(int coinAmount)
    {
        base.Play();
        m_CoinAmount = coinAmount;
        return this;
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_PlayerInfo.OnCoinsReceive(m_CoinAmount);
    }
}
