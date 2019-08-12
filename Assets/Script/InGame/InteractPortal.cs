using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractBase {
    public override string S_InteractKeyword => "Portal";
    Action OnPortalInteract;
    public void InitPortal( Action _OnPortalInteract)
    {
        Init();
        OnPortalInteract = _OnPortalInteract;
    }
    public override bool TryInteract()
    {
        OnPortalInteract();
        return true;
    }
}
