using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class InteractCampSwitchDifficulty : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampDifficulty;
    Text m_Text;
    protected override void Awake()
    {
        base.Awake();
        m_Text = GetComponentInChildren<Text>();
    }
    private void Start()
    {
        m_Text.text = (GameDataManager.m_GameData.m_GameDifficulty).ToString();
    }
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Text.text = (GameDataManager.OnCampDifficultySwitch()).ToString();
        return true;
    }
}
