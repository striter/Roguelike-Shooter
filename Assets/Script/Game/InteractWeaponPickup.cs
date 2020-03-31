﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class InteractWeaponPickup : InteractGameBase {
    Transform tf_ModelContainer;
    public WeaponBase m_Weapon { get; private set; }
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponPickup;
    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor) => base.OnTryInteractCheck(_interactor)&& m_Weapon != null;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_ModelContainer = transform.Find("Container/Model");
    }
    public InteractWeaponPickup Play(WeaponSaveData data )
    {
        base.Play();
        m_Weapon = GameObjectManager.SpawnWeapon(data,tf_ModelContainer);
        m_Weapon.transform.localPosition = Vector3.zero;
        m_Weapon.transform.localRotation = Quaternion.identity;
        return this;
    }
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractedCheck(_interactTarget);
        m_Weapon = _interactTarget.ObtainWeapon(m_Weapon);
        if (!m_Weapon)
            return false;
        m_Weapon.transform.SetParentResetTransform(tf_ModelContainer);
        return true;
    }
}