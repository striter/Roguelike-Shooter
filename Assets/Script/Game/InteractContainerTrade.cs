using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractContainerTrade : InteractContainer {
    public override enum_Interaction m_InteractType => enum_Interaction.ContainerTrade;
    protected override bool B_SelfRecycleOnInteract => true;
    public int m_TradePrice { get; private set; }
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => _interactor.m_CharacterInfo.m_Coins >= m_TradePrice;
    public void Play(int _tradePrice,InteractBase _interactItem)
    {
        base.Play();
        Attach(_interactItem);
        m_TradePrice = _tradePrice;
    }
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        _interactTarget.m_CharacterInfo.OnCoinsRemoval(m_TradePrice);
        return false;
    }
}
