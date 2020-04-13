using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentDepot : UIPage {
    public CEquipmentDepotData m_DepotData => GameDataManager.m_EquipmentDepotData;
    UIT_GridControllerGridItem<UIGI_EquipmentItemEquipping> m_EquippingGrid;
    UIT_GridControllerGridItemScrollView<UIGI_EquipmentItemOwned> m_OwnedGrid;
    UIGI_EquipmentItemSelected m_SelectedEquipment;
    UIT_GridControllerMono<Text> m_AttributesEntryGrid;
    Text m_Passive;
    Transform m_Btns;
    Button m_LeftBtn, m_RightBtn, m_LockBtn;
    Text m_LeftBtnText, m_RightBtnText,m_LockBtnText;

    bool m_Upgrading;
    int m_SelectedEquipmentIndex;
    List<int> m_SelectedDeconstructIndexes=new List<int>();

    protected override void Init()
    {
        base.Init();
        m_EquippingGrid = new UIT_GridControllerGridItem<UIGI_EquipmentItemEquipping>(rtf_Container.Find("EquippingGrid"));
        m_OwnedGrid = new UIT_GridControllerGridItemScrollView<UIGI_EquipmentItemOwned>(rtf_Container.Find("OwnedScrollView"),10);
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
            UpdateBtnStatus();
        }
        UpdateSelectedEquipment();
        UpdateOwnedEquipments();
    }

    void SetUpgrading(bool upgrading)
    {
        m_Upgrading = upgrading;
        m_SelectedDeconstructIndexes.Clear();
        UpdateOwnedEquipments();
        UpdateBtnStatus();
    }
    #endregion
    #region Update
    void UpdateWhole()
    {
        UpdateEquippingEquipments();
        UpdateOwnedEquipments();
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
        m_LeftBtnText.text = m_Upgrading ? "Cancel" : GameDataManager.CanEnhanceEquipment(m_SelectedEquipmentIndex)?"Enhance":"Max Level";
        m_RightBtnText.text = m_Upgrading ? GameDataManager.CanEnhanceEquipment(m_SelectedEquipmentIndex) ? "Enhance" : "Max Level" : GameDataManager.CheckEquipmentEquiping(m_SelectedEquipmentIndex) ? "Dequip" : "Equip";
        m_LockBtnText.text = GameDataManager.CheckEquipmentLocking(m_SelectedEquipmentIndex) ? "Unlock" : "Lock";

        m_RightBtn.interactable = !m_Upgrading || GameDataManager.CanEnhanceEquipment(m_SelectedEquipmentIndex);
    }

    void UpdateOwnedEquipments()
    {
        m_OwnedGrid.ClearGrid();
        m_DepotData.m_Equipments.Traversal((int index, EquipmentSaveData data) => { m_OwnedGrid.AddItem(index).Play(data, OnEquipmentClick, m_DepotData.m_Equipping.Contains(index), m_DepotData.m_Locking.Contains(index), m_SelectedEquipmentIndex == index, m_SelectedDeconstructIndexes.Contains(index)); });
    }

    void UpdateEquippingEquipments()
    {
        m_EquippingGrid.ClearGrid();
        m_DepotData.m_Equipping.Traversal((int selectIndex) => { m_EquippingGrid.AddItem(selectIndex).Play(m_DepotData.m_Equipments[selectIndex], OnEquipmentClick); });
    }

    void UpdateTotalAttributes()
    {
        Dictionary<enum_EquipmentEntryType, float> _entryData = new Dictionary<enum_EquipmentEntryType, float>();
        TCommon.TraversalEnum((enum_EquipmentEntryType type) => { _entryData.Add(type, 0); });
        EquipmentSaveData? _passiveData = null;
        List<EquipmentSaveData> equippingData = m_DepotData.GetSelectedEquipments();
        equippingData.Traversal((int index, EquipmentSaveData data) =>
        {
            if ((equippingData.FindAll(p => p.m_Index == data.m_Index).Count == 2))
                _passiveData = data;

            data.m_Entries.Traversal((EquipmentEntrySaveData entryData) =>
            {
                _entryData[entryData.m_Type] += entryData.m_Value;
            });
        });
        m_AttributesEntryGrid.ClearGrid();
        TCommon.TraversalEnum((enum_EquipmentEntryType type) => { m_AttributesEntryGrid.AddItem((int)type).text = type + ":" + _entryData[type]; });
        m_Passive.SetActivate(_passiveData != null);
        if (_passiveData != null)
            m_Passive.text = "Passive:" + _passiveData.Value.GetPassiveLocalizeKey();
    }
    #endregion
}
