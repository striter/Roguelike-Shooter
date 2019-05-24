using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractBase {
    public override string S_InteractKeyword => "Portal";
    Renderer m_Renderer;
    enum_TileDirection m_PortalDirection;
    protected override void Init()
    {
        base.Init();
        m_Renderer.GetComponentInChildren<Renderer>();
    }
    public void InitPortal(enum_TileDirection _portalDirection, Action<enum_TileDirection> OnPortalInteract)
    {
        m_PortalDirection = _portalDirection;
    }
}
