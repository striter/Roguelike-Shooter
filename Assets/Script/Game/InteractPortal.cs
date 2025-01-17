﻿using GameSetting;
using UnityEngine;
using TTiles;
using System;
using LevelSetting;
using System.Collections.Generic;

public class InteractPortal : InteractBattleBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
    enum_BattlePortalTye m_PortalEvent = enum_BattlePortalTye.Invalid;
    Action<enum_BattlePortalTye> OnPortalInteract;
    public override string GetUITitleKey() => m_PortalEvent.GetLocalizeNameKey();
    public override string GetUIIntroKey() => m_PortalEvent.GetLocalizeIntroKey();
    public override bool B_InteractOnTrigger => false;
    Dictionary<enum_ChunkPortalType, TSpecialClasses.ParticleControlBase> m_PortalParticles=new Dictionary<enum_ChunkPortalType, TSpecialClasses.ParticleControlBase>();
    public override void OnPoolInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolInit(identity, OnRecycle);
        TCommon.TraversalEnum((enum_ChunkPortalType portalType) =>
        {
            m_PortalParticles.Add(portalType, new TSpecialClasses.ParticleControlBase(transform.Find(portalType.ToString())));
        });
    }

    public InteractPortal Play(enum_BattlePortalTye eventType, Action<enum_BattlePortalTye> _OnPortalInteract)
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
        return this;
    }
    
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        OnPortalInteract(m_PortalEvent);
        m_PortalParticles.Traversal((TSpecialClasses.ParticleControlBase particles) => {
                particles.Stop();
        });

        GameDataManager.m_PassTheGate++;
        if ((int)BattleManager.Instance.m_BattleProgress.m_Stage - 1 == GameDataManager.m_passThe)
        {
            GameDataManager.m_passTheGate++;
            GameDataManager.m_passTheGateNew++;
        }
        Debug.Log("通过关卡"+(int)BattleManager.Instance.m_BattleProgress.m_Stage);
        GameDataManager.OnCGameTask();
        return false;
    }
}
