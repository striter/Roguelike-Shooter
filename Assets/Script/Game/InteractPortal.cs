using GameSetting;
using UnityEngine;
using TTiles;
using System;
using LevelSetting;

public class InteractPortal : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
    bool m_StagePortal = false;
    enum_ChunkType m_PortalLevel = enum_ChunkType.Invalid;
    enum_ChunkEventType m_PortalEvent = enum_ChunkEventType.Invalid;
    Action<bool,enum_ChunkType, enum_ChunkEventType> OnPortalInteract;
    public override bool B_InteractOnTrigger => false;
    public void Play(bool stagePortal, enum_ChunkType levelType,enum_ChunkEventType eventType, Action<bool,enum_ChunkType,enum_ChunkEventType> _OnPortalInteract)
    {
        base.Play();
        m_StagePortal = stagePortal;
        m_PortalLevel = levelType;
        m_PortalEvent = eventType;
        OnPortalInteract = _OnPortalInteract;
    }
    
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        OnPortalInteract(m_StagePortal, m_PortalLevel,m_PortalEvent);
        return false;
    }
}
