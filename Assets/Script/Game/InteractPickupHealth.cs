using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupHealth : InteractPickupAmount {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupHealth;
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor)=> !m_OutOfBattle? !_interactor.m_Health.B_HealthFull:true;
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_HitCheck.TryHit(new DamageInfo(-m_Amount, enum_DamageType.HealthOnly, DamageDeliverInfo.Default(_interactTarget.m_EntityID)));
    }
}
