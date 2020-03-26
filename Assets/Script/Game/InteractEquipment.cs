using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractEquipment : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Equipment;
    public int m_PerkID { get; private set; }
    protected override bool B_SelfRecycleOnInteract => true;

    public InteractEquipment Play(int _perkID)
    {
        base.Play();
        m_PerkID = _perkID;
        return this;
    }


    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        _interactor.m_CharacterInfo.OnActionPerkAcquire(m_PerkID);
        return false;
    }
}
