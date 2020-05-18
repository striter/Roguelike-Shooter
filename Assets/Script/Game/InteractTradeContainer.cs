using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTradeContainer : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.TradeContainer;
    protected override bool B_SelfRecycleOnInteract => true;
    public InteractGameBase m_TradeInteract { get; private set; }
    Transform tf_Model;
    public override void OnPoolInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolInit(identity, OnRecycle);
        tf_Model = transform.Find("Container/Model");
    }
    public InteractTradeContainer Play(float _tradePrice, InteractGameBase _interactItem)
    {
        base.Play();
        m_TradeInteract = _interactItem;
        m_TradeInteract.SetInteractable(false);
        m_TradeInteract.transform.position = tf_Model.position;
        m_TradeInteract.transform.rotation = tf_Model.rotation;
        SetTradePrice(_tradePrice);
        return this;
    }

    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor)=>base.OnTryInteractCheck(_interactor) &&  m_TradeInteract.TryInteract(_interactor); 
}
