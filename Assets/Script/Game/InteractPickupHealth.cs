using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupHealth : InteractPickupAmount {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupHealth;

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        _interactor.m_HitCheck.TryHit(new DamageInfo(_interactor.m_EntityID, -m_Amount, enum_DamageType.Health));
        return base.OnInteractedContinousCheck(_interactor);
    }
}
