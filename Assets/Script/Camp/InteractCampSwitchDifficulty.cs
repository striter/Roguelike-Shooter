using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class InteractCampSwitchDifficulty : InteractCamp {
    public override enum_Interaction m_InteractType => enum_Interaction.CampDifficult;
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
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        m_Text.text = (GameDataManager.OnCampDifficultySwitch()).ToString();
        return true;
    }
}
