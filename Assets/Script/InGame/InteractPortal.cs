using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
    protected override bool B_InteractaOnce => true;
    Action OnPortalInteract;
    public void Play( Action _OnPortalInteract)
    {
        base.Play();
        OnPortalInteract = _OnPortalInteract;
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        OnPortalInteract();
    }
}
