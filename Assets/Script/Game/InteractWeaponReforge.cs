using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponReforge : InteractGameQuadrant {
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponReforge;
    enum_PlayerWeapon m_Weapon;
    public InteractWeaponReforge Play(int chunkIndex,enum_PlayerWeapon _weapon)
    {
        base.PlayQuadrant(chunkIndex);
        m_Weapon = _weapon;
        return this;
    }
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
         _interactor.Reforge(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(m_Weapon)))?.DoItemRecycle();
        return false;
    }
}
