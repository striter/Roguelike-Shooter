using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponReforge : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponReforge;
    enum_PlayerWeapon m_Weapon;
    public void Play(enum_PlayerWeapon _weapon)
    {
        base.Play();
        m_Weapon = _weapon;
    }
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
         _interactor.Reforge(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(m_Weapon)))?.DoItemRecycle();
        return false;
    }
}
