using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractPickupArmor : InteractPickupAmount {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupArmor;
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => m_OutOfBattle?true:_interactor.m_Health.m_ArmorFull;
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_HitCheck.TryHit(new DamageInfo(-m_Amount, enum_DamageType.ArmorOnly, DamageDeliverInfo.Default(_interactTarget.m_EntityID)));
    }
}
