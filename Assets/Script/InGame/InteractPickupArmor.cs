using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractPickupArmor : InteractPickup {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupArmor;
    public float m_ArmorAmount { get; private set; }
    public InteractPickupArmor Play(float _armorAmount)
    {
        base.Play();
        m_ArmorAmount = _armorAmount;
        return this;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_HitCheck.TryHit(new DamageInfo(-m_ArmorAmount, enum_DamageType.ArmorOnly, DamageDeliverInfo.Default(_interactTarget.I_EntityID)));
    }
}
