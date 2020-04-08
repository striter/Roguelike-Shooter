using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampArmory : InteractCamp {
    public override enum_Interaction m_InteractType => enum_Interaction.Armory;
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        CampManager.Instance.OnArmoryInteract();
        return true;
    }
}
