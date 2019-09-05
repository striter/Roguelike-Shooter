using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupHealth : InteractPickup {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupHealth;
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => _interactor.m_HealthManager.F_HealthScale < 1;
    public float m_healAmount { get; private set; }
    public InteractPickupHealth Play(float _healthAmount)
    {
        base.Play();
        m_healAmount = _healthAmount;
        return this;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_HitCheck.TryHit(new DamageInfo(-m_healAmount, enum_DamageType.HealthOnly, DamageDeliverInfo.Default(_interactTarget.I_EntityID)));
    }
}
