using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class InteractCampSwitchDifficulty : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampDifficulty;
    Text m_Text;
    public override void Init()
    {
        base.Init();
        m_Text = GetComponentInChildren<Text>();
        m_Text.text = (GameDataManager.m_GameData.m_BattleDifficulty).ToString();
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Text.text = (GameDataManager.OnCampDifficultySwitch()).ToString();
        return true;
    }
}
