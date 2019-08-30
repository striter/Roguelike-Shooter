using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTrade : InteractBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Trade;
    protected override bool B_RecycleOnInteract => true;
    protected override bool B_InteractaOnce => true;
    Transform tf_Model;
    public override void Init()
    {
        base.Init();
        tf_Model = transform.Find("Container/Model");
    }
    public int m_TradePrice { get; private set; }
    public InteractBase m_InteractTarget { get; private set; }
    public void Play(int _tradePrice,InteractBase _interactItem)
    {
        m_TradePrice = _tradePrice;
        m_InteractTarget = _interactItem;
        m_InteractTarget.SetInteractable(false);
        m_InteractTarget.transform.SetParentResetTransform(tf_Model);
    }
    protected override bool B_CanInteract(EntityPlayerBase _interactor) => _interactor.m_PlayerInfo.m_Coins >= m_TradePrice;
    public override bool TryInteract(EntityPlayerBase _interactor)
    {
        if (!B_CanInteract(_interactor)||!m_InteractTarget.TryInteract(_interactor))
            return false;
        return base.TryInteract(_interactor);
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        m_InteractTarget.transform.SetParent(transform.parent);
        m_InteractTarget.SetInteractable(true);
        m_InteractTarget = null;
    }
}
