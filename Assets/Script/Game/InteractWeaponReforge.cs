using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponReforge : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponReforge;
    public enum_PlayerWeapon m_Weapon { get; private set; }
    public InteractWeaponReforge Play(enum_PlayerWeapon weapon)
    {
        base.Play();
        m_Weapon = weapon;
        return this;
    }
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        _interactor.GameWeaponReforge(GameObjectManager.SpawnWeapon(WeaponSaveData.New(m_Weapon)));
        return false;
    }
}
