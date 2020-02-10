using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTradeActionBuff : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.TradeActionBuff;

    public new void Play()
    {
        base.Play();
        m_TradePrice = GameConst.IR_EventTradeBuffPrice.Random();

    }
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        _interactTarget.m_CharacterInfo.OnActionBuffAcquire(ActionDataManager.CreateRandomActionBuff());
        return false;
    }
}
