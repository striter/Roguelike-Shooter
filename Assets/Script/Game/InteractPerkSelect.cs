using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkSelect : InteractBattleBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.PerkSelect;
    List<int> m_PerkIDs;
    TSpecialClasses.ParticleControlBase m_Particles;
    public override void OnPoolInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolInit(identity, OnRecycle);
        m_Particles = new TSpecialClasses.ParticleControlBase(transform.Find("Particles"));
    }

    public InteractPerkSelect Play(List<int> _perkID)
    {
        base.Play();
        m_PerkIDs = _perkID;
        m_Particles.Play();
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Particles.Stop();
        BattleUIManager.Instance.ShowPage<UI_PerkSelect>(true,true, .5f).Show(m_PerkIDs,_interactor.m_CharacterInfo.OnActionPerkAcquire);
        return false;
    }
}
