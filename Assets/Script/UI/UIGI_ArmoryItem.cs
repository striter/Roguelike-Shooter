using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ArmoryItem : UIT_GridItem {
    Text m_Text;
    Button m_Button;
    Action<enum_PlayerWeapon> OnWeaponClick;
    public override void Init()
    {
        base.Init();
        m_Text = GetComponentInChildren<Text>();
        m_Button = GetComponentInChildren<Button>();
    }

    public void Play(Action<enum_PlayerWeapon> OnClick)
    {
        OnWeaponClick = OnClick;
        m_Text.text = ((enum_PlayerWeapon)m_Index).ToString();
    }

    void OnButtonClick() => OnWeaponClick((enum_PlayerWeapon)m_Index);
}
