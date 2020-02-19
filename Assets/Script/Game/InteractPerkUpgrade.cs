using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkUpgrade : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.PerkUpgrade;
    public override bool DnCheckInteractResponse(EntityCharacterPlayer _interactor)=> base.DnCheckInteractResponse(_interactor)&&_interactor.m_CharacterInfo.CanPerkUpgrade();
    public new InteractPerkUpgrade Play()
    {
        base.Play();
        return this;
    }
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        _interactor.m_CharacterInfo.OnPerkUpgrade();
        return false;
    }
}
