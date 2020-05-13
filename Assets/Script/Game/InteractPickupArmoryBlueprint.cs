using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupArmoryBlueprint : InteractPickup
{
    public override enum_Interaction m_InteractType => enum_Interaction.PickupArmoryBlueprint;
    public enum_PlayerWeaponIdentity m_Weapon { get; private set; }
    public override bool B_InteractOnTrigger => true;

    public InteractPickupArmoryBlueprint Play(enum_PlayerWeaponIdentity weapon)
    {
        base.Play();
        m_Weapon = weapon;
        return this;
    }

}
