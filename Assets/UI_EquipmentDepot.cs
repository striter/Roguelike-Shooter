using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipmentDepot : UIPage {
    public CEquipmentDepotData m_DepotData => GameDataManager.m_EquipmentDepotData;
    UIT_GridControllerGridItem<UIGI_EquipmentItemBase> m_Equipping;
    UIT_GridControllerGridItem<UIGI_EquipmentItemOwned> m_Owned;
    protected override void Init()
    {
        base.Init();
        m_Equipping = new UIT_GridControllerGridItem<UIGI_EquipmentItemBase>(rtf_Container.Find("EquippingGrid"));
        m_Owned = new UIT_GridControllerGridItem<UIGI_EquipmentItemOwned>(rtf_Container.Find("OwnedGrid"));

        UpdateDepot();
    }

    void UpdateDepot()
    {
        m_Equipping.ClearGrid();
        m_DepotData.m_Equpping.Traversal((int selectIndex)=> { m_Equipping.AddItem(selectIndex).Play(m_DepotData.m_Equipments[selectIndex]); });
        m_Owned.ClearGrid();
        m_DepotData.m_Equipments.Traversal((int index, EquipmentSaveData data) => { m_Owned.AddItem(index).Play(data, m_DepotData.m_Equpping.Contains(index), m_DepotData.m_Locking.Contains(index)); });
    }
}
