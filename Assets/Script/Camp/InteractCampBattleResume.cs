using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampBattleResume : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampBattleResume;
    public override void Init()
    {
        base.Init();
        //transform.SetActivate(GameDataManager.m_GameData.m_BattleResume);
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        CampManager.Instance.OnBattleStart(false);
        return false;
    }
}
