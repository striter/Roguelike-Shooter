using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkAcquire : InteractGameQuadrant
{
    public override enum_Interaction m_InteractType => enum_Interaction.PerkAcquire;
    ActionPerkBase m_Equipment;
    public int m_TryCount { get; private set; } = 0;
    public InteractPerkAcquire Play(int chunkIndex, ActionPerkBase _equipment)
    {
        base.PlayQuadrant(chunkIndex);
        m_Equipment = _equipment;
        m_TryCount = 0;
        m_TradePrice = GameExpression.GetEventPerkAcquireCoinsAmount(m_TryCount);
        return this;
    }


    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        bool successful = TCommon.RandomPercentage() < GameExpression.GetEventPerkAcquireSuccessRate(m_TryCount);
        if (successful)
            _interactor.m_CharacterInfo.OnActionPerkAcquire(m_Equipment);
        m_TryCount++;
        m_TradePrice = GameExpression.GetEventPerkAcquireCoinsAmount(m_TryCount);
        return !successful&&m_TryCount<GameConst.I_EventPerkAcquireTryCount;
    }
}
