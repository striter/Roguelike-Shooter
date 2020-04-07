using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractWeaponBlueprint : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponBlueprint;
    protected override bool B_SelfRecycleOnInteract => true;
    public enum_PlayerWeapon m_Weapon { get; private set; }
    public void Play(enum_PlayerWeapon weapon)
    {
        base.Play();
        m_Weapon = weapon;
    }
}
