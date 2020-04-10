using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkPickup : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.PerkPickup;
    public int m_PerkID { get; private set; }
    protected override bool B_SelfRecycleOnInteract => true;

    public InteractPerkPickup Play(int _perkID)
    {
        base.Play();
        m_PerkID = _perkID;
        return this;
    }


    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        _interactor.m_CharacterInfo.OnActionPerkAcquire(m_PerkID);
        return false;
    }
}
