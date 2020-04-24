using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentDepot : UIPage {

    enum enum_EquipmentDepotSorting {Invalid=-1, Rarity,Level,Time }

    public CEquipmentDepotData m_DepotData => GameDataManager.m_EquipmentDepotData;
    UIT_GridControllerGridItem<UIGI_EquipmentItemEquipping> m_EquippingGrid;
    UIT_GridControllerGridItemScrollView<UIGI_EquipmentItemOwned> m_OwnedGrid;
    UIGI_EquipmentItemSelected m_SelectedEquipment;
    UIT_GridControllerMono<Text> m_AttributesEntryGrid;
    Text m_Passive;
    Transform m_Btns;
    Button m_LeftBtn, m_RightBtn, m_LockBtn;
    Text m_LeftBtnText, m_RightBtnText,m_LockBtnText;

    Button m_SortRarity, m_SortTime, m_SortLevel;

    bool m_Upgrading;
    int m_SelectedEquipmentIndex;
    List<int> m_SelectedDeconstructIndexes=new List<int>();
    enum_EquipmentDepotSorting m_OwnedSorting = enum_EquipmentDepotSorting.Invalid;
    protected override void Init()
    {
        base.Init();
        m_EquippingGrid = new UIT_GridControllerGridItem<UIGI_EquipmentItemEquipping>(rtf_Container.Find("EquippingGrid"));
        m_OwnedGrid = new UIT_GridControllerGridItemScrollView<UIGI_EquipmentItemOwned>(rtf_Container.Find("OwnedScrollView"),25);
        m_AttributesEntryGrid = new UIT_GridControllerMono<Text>(rtf_Container.Find("Attributes/EntryGrid"));
        m_Passive = rtf_Container.Find("Attributes/Passive").GetComponent<Text>();
        m_SelectedEquipment = rtf_Container.Find("Selected").GetComponent<UIGI_EquipmentItemSelected>();
        m_SelectedEquipment.Init();
        m_Btns = rtf_Container.Find("Btns");
        m_LeftBtn = m_Btns.Find("LeftBtn").GetComponent<Button>();
        m_LeftBtn.onClick.AddListener(OnLeftButtonClick);
        m_RightBtn = m_Btns.Find("RightBtn").GetComponent<Button>();
        m_RightBtn.onClick.AddListener(OnRightButtonClick);
        m_LockBtn = m_Btns.Find("LockBtn").GetComponent<Button>();
        m_LockBtn.onClick.AddListener(OnLockButtonClick);
        m_LeftBtnText = m_Btns.Find("LeftBtn/Text").GetComponent<Text>();
        m_RightBtnText = m_Btns.Find("RightBtn/Text").GetComponent<Text>();
        m_LockBtnText = m_Btns.Find("LockBtn/Text").GetComponent<Text>();
        m_SortRarity = rtf_Container.Find("SortRarity").GetComponent<Button>();
        m_SortRarity.onClick.AddListener(() => { m_OwnedSorting = enum_EquipmentDepotSorting.Rarity;UpdateOwnedEquipments(true); });
        m_SortLevel = rtf_Container.Find("SortLevel").GetComponent<Button>();
        m_SortLevel.onClick.AddListener(() => { m_OwnedSorting = enum_EquipmentDepotSorting.Level; UpdateOwnedEquipments(true); });
        m_SortTime = rtf_Container.Find("SortTime").GetComponent<Button>();
        m_SortTime.onClick.AddListener(() => { m_OwnedSorting = enum_EquipmentDepotSorting.Time; UpdateOwnedEquipments(true); });

        m_Upgrading = false;
        m_SelectedEquipmentIndex = -1;
        UpdateWhole();
    }
    #region Interact
    void OnLeftButtonClick() => SetUpgrading(!m_Upgrading);

    void OnRightButtonClick()
    {
        if(m_Upgrading)
        {
            if (m_SelectedDeconstructIndexes.Count < 0)
                return;
            m_Upgrading = false;
            m_SelectedEquipmentIndex= GameDataManager.DoEnhanceEquipment(m_SelectedEquipmentIndex, m_SelectedDeconstructIndexes);
            m_SelectedDeconstructIndexes.Clear();
            UpdateWhole();
        }
        else
        {
            GameDataManager.DoEquipmentEquip(m_SelectedEquipmentIndex);
            UpdateWhole();
        }
    }

    void OnLockButtonClick()
    {
        GameDataManager.DoEquipmentLock(m_SelectedEquipmentIndex);
        UpdateWhole();
    }

    void OnEquipmentClick(int equipmentIndex)
    {
        if(m_Upgrading)
        {
            if (m_SelectedEquipmentIndex == equipmentIndex||GameDataManager.CheckEquipmentLocking(equipmentIndex))
                return;

            if (m_SelectedDeconstructIndexes.Contains(equipmentIndex))
                m_SelectedDeconstructIndexes.Remove(equipmentIndex);
            else
                m_SelectedDeconstructIndexes.Add(equipmentIndex);
        }
        else
        {
            m_SelectedEquipmentIndex = equipmentIndex;
        }
        UpdateBtnStatus();
        UpdateSelectedEquipment();
        UpdateOwnedEquipments(false);
    }

    void SetUpgrading(bool upgrading)
    {
        m_Upgrading = upgrading;
        m_SelectedDeconstructIndexes.Clear();
        UpdateOwnedEquipments(false);
        UpdateBtnStatus();
    }
    #endregion
    #region Update
    void UpdateWhole()
    {
        UpdateEquippingEquipments();
        UpdateOwnedEquipments(true);
        UpdateTotalAttributes();
        UpdateBtnStatus();
        UpdateSelectedEquipment();
    }

    void UpdateSelectedEquipment()
    {
        m_Btns.SetActivate(m_SelectedEquipmentIndex >= 0);
        if(m_SelectedEquipmentIndex>=0)
            m_SelectedEquipment.Play(GameDataManager.m_EquipmentDepotData.m_Equipments[m_SelectedEquipmentIndex], GameDataManager.GetDeconstructIncome(m_SelectedDeconstructIndexes));
    } 

    void UpdateBtnStatus()
    {
        m_Btns.SetActivate(m_SelectedEquipmentIndex>=0);
        if (m_SelectedEquipmentIndex < 0)
            return;

        float deconstructCost = 0;
        bool canEnhanceTarget= GameDataManager.CanEnhanceEquipment(m_SelectedEquipmentIndex);
        bool canDeconstruct = GameDataManager.CanDeconstructEquipments(m_SelectedDeconstructIndexes,ref deconstructCost);

        m_LeftBtnText.text = m_Upgrading ? "Cancel" : GameDataManager.CanEnhanceEquipment(m_SelectedEquipmentIndex)?"Enhance":"Max Level";
        m_RightBtnText.text = m_Upgrading ? canEnhanceTarget ? canDeconstruct? "Enhance\n"+ deconstructCost : "Lack of Credits" : "Max Level" : GameDataManager.CheckEquipmentEquiping(m_SelectedEquipmentIndex) ? "Dequip" : "Equip";
        m_LockBtnText.text = GameDataManager.CheckEquipmentLocking(m_SelectedEquipmentIndex) ? "Unlock" : "Lock";

        m_RightBtn.interactable = !m_Upgrading || (canEnhanceTarget&&canDeconstruct);
    }

    void UpdateOwnedEquipments(bool refreshGrid)
    {
        if (!refreshGrid)
        {
            m_DepotData.m_Equipments.Traversal((int equipmentIndex, EquipmentSaveData data) => {
                m_OwnedGrid.GetOrAddItem(GetSorting(equipmentIndex, data)).Play(m_DepotData.m_Equipping.Contains(equipmentIndex), m_DepotData.m_Locking.Contains(equipmentIndex), m_SelectedEquipmentIndex == equipmentIndex, m_SelectedDeconstructIndexes.Contains(equipmentIndex));
            });
            return;
        }
        m_OwnedGrid.ClearGrid();
        m_DepotData.m_Equipments.Traversal((int equipmentIndex, EquipmentSaveData data) => {
            m_OwnedGrid.AddItem(GetSorting(equipmentIndex,data)).Play(equipmentIndex, data, OnEquipmentClick, m_DepotData.m_Equipping.Contains(equipmentIndex), m_DepotData.m_Locking.Contains(equipmentIndex), m_SelectedEquipmentIndex == equipmentIndex, m_SelectedDeconstructIndexes.Contains(equipmentIndex));
        });
        m_OwnedGrid.Sort((a,b)=>a.Key-b.Key);
    }

    int GetSorting(int equipmentIndex, EquipmentSaveData data)
    {
        switch (m_OwnedSorting)
        {
            default:
                return equipmentIndex;
            case enum_EquipmentDepotSorting.Level:
                return data.GetEnhanceLevel() * 100000 + (int)data.m_Rarity * 1000 + equipmentIndex;
            case enum_EquipmentDepotSorting.Rarity:
                return (int)data.m_Rarity * 100000 + data.GetEnhanceLevel() * 1000 + equipmentIndex;
            case enum_EquipmentDepotSorting.Time:
                return data.m_AcquireStamp * 1000 + equipmentIndex;
        }
    }
    
    void UpdateEquippingEquipments()
    {
        m_EquippingGrid.ClearGrid();
        m_DepotData.m_Equipping.Traversal((int selectIndex) => { m_EquippingGrid.AddItem(selectIndex).Play(m_DepotData.m_Equipments[selectIndex], OnEquipmentClick); });
    }

    void UpdateTotalAttributes()
    {
        Dictionary<enum_CharacterUpgradeType, float> _entryData = new Dictionary<enum_CharacterUpgradeType, float>();
        TCommon.TraversalEnum((enum_CharacterUpgradeType type) => { _entryData.Add(type, 0); });

        ExpirePlayerUpgradeCombine upgrade = GameDataManager.CreateUpgradeCombination(m_DepotData.GetSelectedEquipments(),CharacterUpgradeData.Default());
        m_AttributesEntryGrid.ClearGrid();
        TCommon.TraversalEnum((enum_CharacterUpgradeType type) => { m_AttributesEntryGrid.AddItem((int)type).text = type + ":" + upgrade.m_UpgradeDatas[type]; });
        m_Passive.SetActivate(upgrade.m_HavePassive);
        if (upgrade.m_HavePassive)
            m_Passive.text = "Passive:" + upgrade.GetPassiveLocalizeKey();
    }
    #endregion
}
