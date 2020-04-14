using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampCharacterUpgrade : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampCharacterUpgrade;

    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor)
    {
        base.OnTryInteractCheck(_interactor);
        CampManager.Instance.OnCharacterUpgradeInteract();
        return true;
    }
}
