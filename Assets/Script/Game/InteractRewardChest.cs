using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using LevelSetting;
public class InteractRewardChest : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.RewardChest;
    ActionPerkBase m_Equipment;
    enum_PlayerWeapon m_Weapon;

    public void Play(ActionPerkBase _equipment, enum_PlayerWeapon weapon)
    {
        base.Play();
        m_Equipment = _equipment;
        m_Weapon = weapon;
    }
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        if (m_Weapon != enum_PlayerWeapon.Invalid)
            GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, transform.position + transform.forward*LevelConst.I_TileSize, transform.rotation).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(m_Weapon)));

        if (m_Equipment != null)
            GameObjectManager.SpawnInteract<InteractEquipment>(enum_Interaction.Equipment, transform.position + transform.forward * LevelConst.I_TileSize, transform.rotation).Play(m_Equipment);

        return false;
    }

}
