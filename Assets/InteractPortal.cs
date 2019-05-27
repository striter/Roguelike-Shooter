using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractBase {
    public override string S_InteractKeyword => "Portal";
    Renderer m_Renderer;
    enum_TileDirection m_PortalDirection;
    Action<enum_TileDirection> OnPortalInteract;
    protected override void Init()
    {
        base.Init();
        m_Renderer=transform.GetComponentInChildren<Renderer>();
    }

    public void InitPortal(enum_TileDirection _portalDirection,enum_BigmapTileType _targetLevelType, Action<enum_TileDirection> _OnPortalInteract)
    {
        Init();
        OnPortalInteract = _OnPortalInteract;
        m_PortalDirection = _portalDirection;
        m_Renderer.material.SetColor("_Color",EnviormentManager.BigmapTileColor(_targetLevelType));
    }
    public override bool TryInteract()
    {
        OnPortalInteract(m_PortalDirection);
        return true;
    }
}
