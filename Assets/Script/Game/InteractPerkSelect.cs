using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkSelect : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.PerkSelect;
    List<int> m_PerkIDs;
    TSpecialClasses.ParticleControlBase m_Particles;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_Particles = new TSpecialClasses.ParticleControlBase(transform.Find("Particles"));
    }
    public InteractPerkSelect Play(List<int> _perkID)
    {
        base.Play();
        m_PerkIDs = _perkID;
        m_Particles.Play();
        return this;
    }


    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        m_Particles.Stop();
        GameUIManager.Instance.ShowPage<UI_PerkSelect>(true, .5f).Show(m_PerkIDs,_interactor.m_CharacterInfo.OnActionPerkAcquire);
        return false;
    }
}
