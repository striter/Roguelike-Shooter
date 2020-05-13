using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_ArmoryItem : UIT_GridItem {
    Text m_Text;
    Button m_Button;
    Action<enum_PlayerWeaponIdentity> OnWeaponClick;
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Text = GetComponentInChildren<Text>();
        m_Button = GetComponentInChildren<Button>();
        m_Button.onClick.AddListener(OnButtonClick);
    }

    public void Play(Action<enum_PlayerWeaponIdentity> OnClick)
    {
        OnWeaponClick = OnClick;
        m_Text.text = ((enum_PlayerWeaponIdentity)m_Identity).ToString();
    }

    void OnButtonClick() => OnWeaponClick((enum_PlayerWeaponIdentity)m_Identity);
}
