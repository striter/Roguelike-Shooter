using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupArmoryParts : InteractPickup
{
    public override enum_Interaction m_InteractType => enum_Interaction.PickupArmoryParts;
    public enum_PlayerWeapon m_Weapon { get; private set; }
    public override bool B_InteractOnTrigger => true;

    public new InteractPickupArmoryParts Play()
    {
        base.Play();
        return this;
    }

}
