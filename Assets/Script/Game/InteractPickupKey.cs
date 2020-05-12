using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupKey : InteractPickupAmount {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupKey;
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        _interactor.m_CharacterInfo.OnKeysGain(m_Amount);
        return base.OnInteractedContinousCheck(_interactor);
    }
}
