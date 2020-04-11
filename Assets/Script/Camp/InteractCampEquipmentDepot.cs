using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampEquipmentDepot : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampEquipmentDepot;
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        CampManager.Instance.OnEquipmentDepotInteract();
        return true;
    }
}
