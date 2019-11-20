using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractContainer : InteractGameBase {

    public InteractBase m_TradeInteract { get; private set; }
    Transform tf_Model;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_Model = transform.Find("Container/Model");
    }
    public override bool TryInteract(EntityCharacterPlayer _interactor)
    {
        if (!B_CanInteract(_interactor) || !m_TradeInteract.TryInteract(_interactor))
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
    protected void Detach()
    {
        m_TradeInteract.SetInteractable(true);
    }
}
