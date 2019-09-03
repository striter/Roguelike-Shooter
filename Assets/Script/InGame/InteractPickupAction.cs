using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupAction : InteractPickup {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupAction;
    public ActionBase m_Action { get; private set; }
    Renderer m_Renderer;
    public override void Init()
    {
        base.Init();
        m_Renderer = GetComponentInChildren<Renderer>();
    }
    public InteractPickupAction Play(ActionBase _action)
    {
        m_Action = _action;
        m_Renderer.material.color = _action.m_Level.ActionRarityColor();
        return this;
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_PlayerInfo.AddStoredAction(m_Action);
    }
}
