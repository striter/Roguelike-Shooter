using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTradeContainer : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.TradeContainer;
    protected override bool B_SelfRecycleOnInteract => true;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_Model = transform.Find("Container/Model");
    }
    public void Play(int _tradePrice,InteractBase _interactItem)
    {
        base.Play();
        Attach(_interactItem);
        m_TradePrice = _tradePrice;
    }
    public InteractBase m_TradeInteract { get; private set; }
    Transform tf_Model;

    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor)=>base.OnTryInteractCheck(_interactor) &&  m_TradeInteract.TryInteract(_interactor); 

    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        return false;
    }

    protected void Attach(InteractBase _interactItem)
    {
        m_TradeInteract = _interactItem;
        m_TradeInteract.SetInteractable(false);
        m_TradeInteract.transform.position = tf_Model.position;
        m_TradeInteract.transform.rotation = tf_Model.rotation;
    }
}
