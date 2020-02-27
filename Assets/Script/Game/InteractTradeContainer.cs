using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTradeContainer : InteractGameQuadrant {
    public override enum_Interaction m_InteractType => enum_Interaction.TradeContainer;
    protected override bool B_SelfRecycleOnInteract => true;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_Model = transform.Find("Container/Model");
    }
    public void Play(int chunkIndex, int _tradePrice,InteractBase _interactItem)
    {
        base.PlayQuadrant(chunkIndex);
        Attach(_interactItem);
        m_TradePrice = _tradePrice;
    }
    public InteractBase m_TradeInteract { get; private set; }
    Transform tf_Model;

    protected override void OnQuadrantShow(bool show)
    {
        base.OnQuadrantShow(show);
        m_TradeInteract.SetActivate(show);
    }

    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        return false;
    }

    public override bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!DoCheckInteractSuccessful(_interactor) || !m_TradeInteract.TryInteract(_interactor))
            return false;
        return base.TryInteract(_interactor);
    }

    protected void Attach(InteractBase _interactItem)
    {
        m_TradeInteract = _interactItem;
        m_TradeInteract.SetInteractable(false);
        m_TradeInteract.transform.position = tf_Model.position;
        m_TradeInteract.transform.rotation = tf_Model.rotation;
    }
}
