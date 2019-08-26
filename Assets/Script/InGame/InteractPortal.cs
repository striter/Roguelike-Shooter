using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractBase {
    public override string S_InteractKeyword => "Portal";
    Action OnPortalInteract;
    public override bool B_InteractaOnce => true;
    public void Play( Action _OnPortalInteract)
    {
        base.Play();
        OnPortalInteract = _OnPortalInteract;
    }
    public override bool TryInteract(EntityPlayerBase _interactor)
    {
        base.TryInteract(_interactor);
        OnPortalInteract();
        return true;
    }
}
