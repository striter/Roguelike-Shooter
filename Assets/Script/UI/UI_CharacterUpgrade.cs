using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UI_CharacterUpgrade : UIPage {
    Transform m_Status;
    Text m_Status_Cost, m_Status_Current, m_Status_Next;
    Button m_Status_Confirm;
    UIT_GridControlledSingleSelect<UIGI_CharacterUpgradeItem> m_UpgradeGrid;
    enum_CharacterUpgradeType m_SelectUpgrade;
    protected override void Init()
    {
        base.Init();
        m_Status = rtf_Container.Find("Status");
        m_Status_Cost = m_Status.Find("Cost").GetComponent<Text>();
        m_Status_Current = m_Status.Find("Current").GetComponent<Text>();
        m_Status_Next = m_Status.Find("Next").GetComponent<Text>();
        m_Status_Confirm = m_Status.Find("Confirm").GetComponent<Button>();
        m_Status_Confirm.onClick.AddListener(OnConfirmButtonClick);
        m_UpgradeGrid = new UIT_GridControlledSingleSelect<UIGI_CharacterUpgradeItem>(rtf_Container.Find("UpgradeGrid"),OnUpgradeItemClick);

        TCommon.TraversalEnum((enum_CharacterUpgradeType type) => { m_UpgradeGrid.AddItem((int)type).Play(type, 0); });
        m_UpgradeGrid.Sort((a, b) => a.Key - b.Key);
        m_UpgradeGrid.OnItemClick((int)enum_CharacterUpgradeType.Health);
    }

    void OnUpgradeItemClick(int index)
    {
        m_SelectUpgrade = (enum_CharacterUpgradeType)index;
        UpdateInfo();
    }

    void OnConfirmButtonClick()
    {
        GameDataManager.UpgradeCurrentCharacter(m_SelectUpgrade);
        UpdateInfo();
    }

    void UpdateInfo()
    {
        CharacterUpgradeData data = GameDataManager.m_CharacterUpgradeData.Current;
        data.m_Upgrades.Traversal((enum_CharacterUpgradeType type, int amount) => { m_UpgradeGrid.GetItem((int)type).Play(type, amount); });

        int upgradeTime = data.m_Upgrades[m_SelectUpgrade];

        float price = GameExpression.GetCharacterUpgradePrice(m_SelectUpgrade, upgradeTime);

        m_Status_Current.text = m_SelectUpgrade + ":" + GameConst.m_UpgradeValueEachTime[m_SelectUpgrade] * upgradeTime;

        bool haveNextLevel = GameDataManager.CanUpgradeItem(data, m_SelectUpgrade);
        m_Status_Next.SetActivate(haveNextLevel);
        m_Status_Cost.SetActivate(haveNextLevel);
        if (haveNextLevel)
        {
            m_Status_Next.text = (GameConst.m_UpgradeValueEachTime[m_SelectUpgrade] * (upgradeTime + 1)).ToString();
            m_Status_Cost.text = "Price:" + price;
        }

        m_Status_Confirm.interactable =haveNextLevel && GameDataManager.CanUseCredit(price);
    }

}
