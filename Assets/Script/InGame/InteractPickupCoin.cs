using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupCoin : InteractPickupAmount {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupCoin;
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_PlayerInfo.OnCoinsReceive((int)m_Amount);
    }
}
