using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupEquipment : InteractPickup {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupEquipment;
    EquipmentSaveData m_Data;
    public override bool B_InteractOnTrigger => true;
    public InteractPickupEquipment Play(EquipmentSaveData data)
    {
        base.Play();
        m_Data = data;
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        UIManager.Instance.m_Indicate.NewTip(enum_UITipsType.Normal).formatKey("UI_Game_Pickup", m_Data.GetNameLocalizeKey());
        return base.OnInteractedContinousCheck(_interactor);
    }
}
