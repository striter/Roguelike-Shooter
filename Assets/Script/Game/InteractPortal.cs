using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
     string m_localizeKey = "";
    public override string m_ExternalLocalizeKeyJoint => "_" + m_localizeKey;
    TSpecialClasses.ParticleControlBase m_Particles;
    Action OnPortalInteract;
    public void Play( Action _OnPortalInteract, string _localizeKey)
    {
        base.Play();
        OnPortalInteract = _OnPortalInteract;
        m_localizeKey= _localizeKey;
        m_Particles = new TSpecialClasses.ParticleControlBase(transform);
        m_Particles.Play();
    }
    private void OnEnable()
    {
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    private void OnDisable()
    {
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    void OnBattleStart()
    {
        SetInteractable(false);
        m_Particles.Stop();
    }

    void OnBattleFinish()
    {
        SetInteractable(true);
        m_Particles.Play();
    }

    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        OnPortalInteract();
        return false;
    }
}
