using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractWeapon : InteractBase {
    Transform tf_ModelContainer;
    public WeaponBase m_Weapon { get; private set; }
    public override enum_Interaction m_InteractType => enum_Interaction.Weapon;
    public override void Init()
    {
        base.Init();
        tf_ModelContainer = transform.Find("Container/Model");
    }
    public InteractWeapon Play(WeaponBase weapon )
    {
        base.Play();
        m_Weapon = weapon;
        m_Weapon.transform.SetParentResetTransform(tf_ModelContainer);
        return this;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        m_Weapon = _interactTarget.ObtainWeapon(m_Weapon);
        m_Weapon.transform.SetParentResetTransform(tf_ModelContainer);
    }
}
