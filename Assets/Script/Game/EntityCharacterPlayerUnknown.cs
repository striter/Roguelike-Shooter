using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterPlayerUnknown : EntityCharacterPlayer {
    #region Preset
    public int P_NormalFinishPercentage = 25;
    public int P_EliteFinishPercentage = 15;
    public float F_AbilityDamagePerStack = 20;
    public int I_MaxAbilityDamageStack = 5;
    public float F_AbilityDamageStackExpireDuration = 20;
    #endregion

    public override void OnAbilityDown(bool down)
    {
        base.OnAbilityDown(down);
    }
}
