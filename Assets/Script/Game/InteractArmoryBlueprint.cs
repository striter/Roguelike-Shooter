using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractArmoryBlueprint : InteractPickup {
    public override enum_Interaction m_InteractType => enum_Interaction.ArmoryBlueprint;
    protected override bool B_SelfRecycleOnInteract => true;
    public enum_PlayerWeapon m_Weapon { get; private set; }
    public void Play(enum_PlayerWeapon weapon,bool moveTowardsPlayer)
    {
        base.Play(moveTowardsPlayer);
        m_Weapon = weapon;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        UIManager.Instance.m_Indicate.NewTip(enum_UITipsType.Normal).formatKey("UI_Game_Blueprint",m_Weapon.GetLocalizeNameKey());
        return base.OnInteractedContinousCheck(_interactor);
    }
}
