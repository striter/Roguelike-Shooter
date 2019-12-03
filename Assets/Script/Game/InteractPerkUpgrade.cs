﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkUpgrade : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.PerkUpgrade;
    public override bool B_InteractOnce => true;
    Animation m_Anim;
    Action OnInteract;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_Anim = GetComponent<Animation>();
    }
    public void Play(Action _OnInteract)
    {
        base.Play();
        OnInteract = _OnInteract;
        m_Anim.Play();
    }
    protected override bool B_CanInteract(EntityCharacterPlayer _interactor) => _interactor.m_WeaponCurrent.m_WeaponAction == null || _interactor.m_WeaponCurrent.m_WeaponAction.B_Upgradable;
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        OnInteract();
        _interactTarget.UpgradeActionPerk();
        m_Anim.Stop();
    }
}
