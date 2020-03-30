using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampNPCAction : InteractCamp {
    public override enum_Interaction m_InteractType => enum_Interaction.CampAction;
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        CampManager.Instance.OnActionNPCChatted();
        return true;
    }
}
