using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class InteractWeapon : InteractGameBase {
    Transform tf_ModelContainer;
    public WeaponBase m_Weapon { get; private set; }
    public override enum_Interaction m_InteractType => enum_Interaction.Weapon;
    protected override bool DoCheckInteractSuccessful(EntityCharacterPlayer _interactor) => base.DoCheckInteractSuccessful(_interactor)&& m_Weapon != null;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_ModelContainer = transform.Find("Container/Model");
    }
    public InteractWeapon Play(WeaponBase weapon )
    {
        base.Play();
        m_Weapon = weapon;
        m_Weapon.transform.SetParentResetTransform(tf_ModelContainer);
        return this;
    }
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        m_Weapon = _interactTarget.ObtainWeapon(m_Weapon);
        if (!m_Weapon)
            return false;
        m_Weapon.transform.SetParentResetTransform(tf_ModelContainer);
        return true;
    }
}
