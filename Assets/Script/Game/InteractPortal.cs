using GameSetting;
using UnityEngine;
using TTiles;
using System;
using LevelSetting;
using System.Collections.Generic;

public class InteractPortal : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
    enum_LevelType m_PortalEvent = enum_LevelType.Invalid;
    Action<enum_LevelType> OnPortalInteract;
    public override string GetUITitleKey() => m_PortalEvent.GetLocalizeNameKey();
    public override string GetUIIntroKey() => m_PortalEvent.GetLocalizeIntroKey();
    public override bool B_InteractOnTrigger => false;
    Dictionary<enum_ChunkPortalType, TSpecialClasses.ParticleControlBase> m_PortalParticles=new Dictionary<enum_ChunkPortalType, TSpecialClasses.ParticleControlBase>();
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        TCommon.TraversalEnum((enum_ChunkPortalType portalType) =>
        {
            m_PortalParticles.Add(portalType, new TSpecialClasses.ParticleControlBase(transform.Find(portalType.ToString())));
        });
    }

    public void Play(enum_LevelType eventType, Action<enum_LevelType> _OnPortalInteract)
    {
        base.Play();
        m_PortalEvent = eventType;
        OnPortalInteract = _OnPortalInteract;
        enum_ChunkPortalType portal = m_PortalEvent.GetPortalType();
        m_PortalParticles.Traversal((enum_ChunkPortalType type, TSpecialClasses.ParticleControlBase particles) => {
            if (type == portal)
                particles.Play();
            else
                particles.Stop();
        });

    }
    
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        OnPortalInteract(m_PortalEvent);
        m_PortalParticles.Traversal((TSpecialClasses.ParticleControlBase particles) => {
                particles.Stop();
        });
        return false;
    }
}
