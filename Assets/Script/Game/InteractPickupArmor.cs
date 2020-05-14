using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractPickupArmor : InteractPickupAmount {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupArmor;

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        _interactor.m_HitCheck.TryHit(new DamageInfo(_interactor.m_EntityID, enum_DamageIdentity.Environment).SetDamage(-m_Amount, enum_DamageType.Armor));
        return base.OnInteractedContinousCheck(_interactor);
    }
}
