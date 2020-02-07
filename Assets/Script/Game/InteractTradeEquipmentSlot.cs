using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTradeEquipmentSlot : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.TradeEquipmentSlot;
    public new void Play()
    {
        base.Play();

    }

    public override bool OnCheckResponse(EntityCharacterPlayer _interactTarget)
    {
        m_TradePrice = _interactTarget.m_CharacterInfo.m_EquipmentSlot * GameConst.I_EquipmentSlotTradePricePerPlayerSlots;
        return base.OnCheckResponse(_interactTarget);
    }

    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        _interactTarget.m_CharacterInfo.AddEquipmentSlot();
        m_TradePrice = _interactTarget.m_CharacterInfo.m_EquipmentSlot * GameConst.I_EquipmentSlotTradePricePerPlayerSlots;
        return true;
    }


}
