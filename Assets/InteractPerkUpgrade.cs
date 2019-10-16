using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkUpgrade : InteractBase {
    public override enum_Interaction m_InteractType => enum_Interaction.PerkUpgrade;
    public override bool B_InteractOnce => true;
    ActionBase m_invalidPerk;
    Action OnInteract;
    public void Play(Action _OnInteract,ActionBase _invalidPerk)
    {
        base.Play();
        OnInteract = _OnInteract;
        m_invalidPerk = _invalidPerk;
    }
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => _interactor.m_WeaponCurrent.m_WeaponAction == null || _interactor.m_WeaponCurrent.m_WeaponAction.B_Upgradable;
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        OnInteract();
        _interactTarget.UpgradeWeaponPerk(m_invalidPerk);
        m_invalidPerk = null;
    }
}
