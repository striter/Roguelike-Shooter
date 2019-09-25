﻿using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class CampInteractSwitchDifficulty : CampInteract {
    public override bool B_InteractOnce => false;
    public override enum_Interaction m_InteractType => enum_Interaction.CampDifficult;
    Text m_Text;
    public override void Init()
    {
        base.Init();
        m_Text = GetComponentInChildren<Text>();
        m_Text.text = ((int)GameDataManager.m_PlayerGameData.m_GameDifficulty).ToString();
    }

    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        m_Text.text = ((int)GameDataManager.SwitchGameDifficulty()).ToString();
    }
}
