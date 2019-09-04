using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractPickupArmor : InteractPickup {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupHealth;
    protected override bool B_CanInteract(EntityPlayerBase _interactor) => _interactor.m_HealthManager.F_HealthScale < 1;
    public float m_healAmount { get; private set; }
    public InteractPickupArmor Play(float _armorAmount)
    {
        base.Play();
        m_healAmount = _armorAmount;
        return this;
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_HitCheck.TryHit(new DamageInfo(-m_healAmount, enum_DamageType.ArmorOnly, DamageDeliverInfo.Default(_interactTarget.I_EntityID)));
    }
}
