using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractAction : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Action;
    public ActionBase m_Action { get; private set; }
    Renderer m_Renderer;
    protected override bool B_RecycleOnInteract => true;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_Renderer = GetComponentInChildren<Renderer>();
    }
    public InteractAction Play(ActionBase _action)
    {
        base.Play();
        m_Action = _action;
        m_Renderer.material.color = _action.m_ActionType.ActionTypeColor();
        return this;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.OnPickupAction(m_Action);
    }
}
