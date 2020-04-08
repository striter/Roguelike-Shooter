using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampDailyReward : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampDailyReward;
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        if (CampManager.Instance.OnAcquireDailyRewardInteract())
            GameObjectManager.PlayMuzzle(-1,transform.position,Vector3.up,10021);
        return true;
    }
}
