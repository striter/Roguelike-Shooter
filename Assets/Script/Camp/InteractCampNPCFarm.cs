using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampNPCFarm : InteractCamp {
    public override enum_Interaction m_InteractType => enum_Interaction.CampFarm;
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        CampManager.Instance.OnFarmNPCChatted();
        return true;
    }
}
