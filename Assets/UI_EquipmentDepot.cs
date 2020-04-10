using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentDepot : UIPage {
    public CEquipmentDepotData m_DepotData => GameDataManager.m_EquipmentDepotData;
    UIT_GridControllerGridItem<UIGI_EquipmentItemEquipping> m_Equipping;
    UIT_GridControllerGridItem<UIGI_EquipmentItemOwned> m_Owned;
    UIGI_EquipmentItemSelected m_Selected;

    UIT_GridControllerMono<Text> m_EntryGrid;
    Text m_Passive;

    protected override void Init()
    {
        base.Init();
        m_Equipping = new UIT_GridControllerGridItem<UIGI_EquipmentItemEquipping>(rtf_Container.Find("EquippingGrid"));
        m_Owned = new UIT_GridControllerGridItem<UIGI_EquipmentItemOwned>(rtf_Container.Find("OwnedGrid"));
        m_EntryGrid = new UIT_GridControllerMono<Text>(rtf_Container.Find("Attributes/EntryGrid"));
        m_Passive = rtf_Container.Find("Attributes/Passive").GetComponent<Text>();
        m_Selected = rtf_Container.Find("Selected").GetComponent<UIGI_EquipmentItemSelected>();
        m_Selected.Init();
        UpdateDepot();
    }


    void OnEquipmentClick(int index)
    {
        m_Selected.Play(GameDataManager.m_EquipmentDepotData.m_Equipments[index]);
    }

    void UpdateDepot()
    {
        m_Equipping.ClearGrid();
        m_DepotData.m_Equpping.Traversal((int selectIndex)=> { m_Equipping.AddItem(selectIndex).Play(m_DepotData.m_Equipments[selectIndex],OnEquipmentClick); });
        m_Owned.ClearGrid();
        m_DepotData.m_Equipments.Traversal((int index, EquipmentSaveData data) => { m_Owned.AddItem(index).Play(data,OnEquipmentClick,  m_DepotData.m_Equpping.Contains(index), m_DepotData.m_Locking.Contains(index)); });

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
        TCommon.TraversalEnum((enum_EquipmentEntryType type) => { m_EntryGrid.AddItem((int)type).text = type + ":" + _entryData[type]; });
        m_Passive.SetActivate(_passiveData != null);
        if (_passiveData != null)
            m_Passive.text = "Passive:" + _passiveData.Value.GetPassiveLocalizeKey();
    }
}
