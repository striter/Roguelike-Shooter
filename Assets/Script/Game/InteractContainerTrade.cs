using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractContainerTrade : InteractContainer {
    public override enum_Interaction m_InteractType => enum_Interaction.ContainerTrade;
    protected override bool B_RecycleOnInteract => true;
    public int m_TradePrice { get; private set; }
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => _interactor.m_PlayerInfo.m_Coins >= m_TradePrice;
    public void Play(int _tradePrice,InteractBase _interactItem)
    {
        base.Play();
        Attach(_interactItem);
        m_TradePrice = _tradePrice;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_PlayerInfo.OnCoinsRemoval(m_TradePrice);
    }
}
