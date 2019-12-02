using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
    public bool m_StagePortal = false;
    public override string m_ExternalLocalizeKeyJoint => "_" + (m_StagePortal?"Stage":"Level");
    TSpecialClasses.ParticleControlBase m_Particles;
    Action OnPortalInteract;
    public void Play( Action _OnPortalInteract, bool stagePortal)
    {
        base.Play();
        OnPortalInteract = _OnPortalInteract;
        m_StagePortal= stagePortal;
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

    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        OnPortalInteract();
    }
}
