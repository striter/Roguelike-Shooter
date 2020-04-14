using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_CharacterUpgradeItem : UIT_GridItem,IGridHighlight {
    Text m_Title, m_Amount, m_Selected;
    Action<int> OnItemClick;
    enum_CharacterUpgradeType m_Upgrade;
    public override void Init()
    {
        base.Init();
        m_Title = rtf_Container.Find("Title").GetComponent<Text>();
        m_Amount = rtf_Container.Find("Amount").GetComponent<Text>();
        m_Selected = rtf_Container.Find("Selected").GetComponent<Text>();
        GetComponentInChildren<Button>().onClick.AddListener(()=> { OnItemClick(m_Index); });
    }
    public void Play(enum_CharacterUpgradeType upgrade, int amount)
    {
        m_Upgrade = upgrade;
        m_Title.text = upgrade.ToString();
        m_Amount.text = amount.ToString();
    }

    public void AttachSelectButton(Action<int> OnButtonClick)
    {
        OnItemClick = OnButtonClick;
    }

    public void OnHighlight(bool highlight)
    {
        m_Selected.SetActivate(highlight);
    }

}
