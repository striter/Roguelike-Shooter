using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampNPCFarm : InteractCamp {
    public override enum_Interaction m_InteractType => enum_Interaction.CampFarm;

    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        CampManager.Instance.OnFarmNPCChatted();
    }
}
