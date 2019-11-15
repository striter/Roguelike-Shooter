using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
    public enum_StageLevel m_PortalStage= enum_StageLevel.Invalid;
    public override string m_ExternalLocalizeKeyJoint => "_" + m_PortalStage;
    Action OnPortalInteract;
    public void Play( Action _OnPortalInteract, enum_StageLevel _stage)
    {
        base.Play();
        OnPortalInteract = _OnPortalInteract;
        m_PortalStage = _stage;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        OnPortalInteract();
    }
}
