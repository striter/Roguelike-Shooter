using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampDailyReward : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampDailyReward;
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        CampManager.Instance.OnDailyRewardInteract();
        return true;
    }
}
