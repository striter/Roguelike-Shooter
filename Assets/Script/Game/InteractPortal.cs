using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractGameQuadrant {
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
     string m_localizeKey = "";
    public override string m_ExternalLocalizeKeyJoint => "_" + m_localizeKey;
    TSpecialClasses.ParticleControlBase m_Particles;
    Action OnPortalInteract;
    public void Play(int chunkIndex, Action _OnPortalInteract, string _localizeKey)
    {
        base.PlayQuadrant(chunkIndex);
        OnPortalInteract = _OnPortalInteract;
        m_localizeKey= _localizeKey;
        m_Particles = new TSpecialClasses.ParticleControlBase(transform);
        m_Particles.Play();
    }
    protected override void OnQuadrantPlay()
    {
        base.OnQuadrantPlay();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnFinalBattleStart, OnFinalBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnFinalBattleFinish, OnFinalBattleFinish);
    }
    protected override void OnQuadrantStop()
    {
        base.OnQuadrantStop();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnFinalBattleStart, OnFinalBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnFinalBattleFinish, OnFinalBattleFinish);
    }

    void OnFinalBattleStart()
    {
        SetInteractable(false);
        m_Particles.Stop();
    }

    void OnFinalBattleFinish()
    {
        SetInteractable(true);
        m_Particles.Play();
    }

    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        OnPortalInteract();
        return false;
    }
}
