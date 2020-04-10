using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentDepot : UIPage {
    public CEquipmentDepotData m_DepotData => GameDataManager.m_EquipmentDepotData;
    bool m_SelectedEquipping => m_DepotData.m_Equipping.Contains(m_SelectingEquipmentIndex);
    UIT_GridControllerGridItem<UIGI_EquipmentItemEquipping> m_EquippingGrid;
    UIT_GridControllerGridItemScrollView<UIGI_EquipmentItemOwned> m_OwnedGrid;
    UIGI_EquipmentItemSelected m_Selected;
    UIT_GridControllerMono<Text> m_AttributesEntryGrid;
    Text m_Passive;
    Text m_LeftBtnText, m_RightBtnText;

    bool m_Upgrading;
    int m_SelectingEquipmentIndex;

    protected override void Init()
    {
        base.Init();
        m_EquippingGrid = new UIT_GridControllerGridItem<UIGI_EquipmentItemEquipping>(rtf_Container.Find("EquippingGrid"));
        m_OwnedGrid = new UIT_GridControllerGridItemScrollView<UIGI_EquipmentItemOwned>(rtf_Container.Find("OwnedScrollView"),10);
        m_AttributesEntryGrid = new UIT_GridControllerMono<Text>(rtf_Container.Find("Attributes/EntryGrid"));
        m_Passive = rtf_Container.Find("Attributes/Passive").GetComponent<Text>();
        m_Selected = rtf_Container.Find("Selected").GetComponent<UIGI_EquipmentItemSelected>();
        m_Selected.Init();
        rtf_Container.Find("LeftBtn").GetComponent<Button>().onClick.AddListener(OnLeftButtonClick);
        rtf_Container.Find("RightBtn").GetComponent<Button>().onClick.AddListener(OnRightButtonClick);
        m_LeftBtnText = rtf_Container.Find("LeftBtn/Text").GetComponent<Text>();
        m_RightBtnText = rtf_Container.Find("RightBtn/Text").GetComponent<Text>();
        SetUpgrading(false);
        UpdateEquipments();
        UpdateBtnStatus();
    }

    void OnLeftButtonClick() => SetUpgrading(!m_Upgrading);

    void OnRightButtonClick()
    {
        if(m_Upgrading)
        {

        }
        else
        {
            GameDataManager.DoEquipmentEquip(m_SelectingEquipmentIndex,!m_SelectedEquipping);
            UpdateEquipments();
            UpdateBtnStatus();
        }
    }

    void SetUpgrading(bool upgrading)
    {
        m_Upgrading = upgrading;
        UpdateBtnStatus();
    }

    void OnEquipmentClick(int index)
    {
        m_SelectingEquipmentIndex = index;
        m_Selected.Play(GameDataManager.m_EquipmentDepotData.m_Equipments[m_SelectingEquipmentIndex]);
        UpdateBtnStatus();
    }


    void UpdateBtnStatus()
    {
        m_LeftBtnText.text = m_Upgrading ? "Cancel" : "Upgrade";
        m_RightBtnText.text = m_Upgrading ? "Confirm" : m_SelectedEquipping ? "Dequip" : "Equip";
    }

    void UpdateEquipments()
    {
        m_EquippingGrid.ClearGrid();
        m_DepotData.m_Equipping.Traversal((int selectIndex)=> { m_EquippingGrid.AddItem(selectIndex).Play(m_DepotData.m_Equipments[selectIndex],OnEquipmentClick); });
        m_OwnedGrid.ClearGrid();
        m_DepotData.m_Equipments.Traversal((int index, EquipmentSaveData data) => { m_OwnedGrid.AddItem(index).Play(data,OnEquipmentClick,  m_DepotData.m_Equipping.Contains(index), m_DepotData.m_Locking.Contains(index)); });

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
}
