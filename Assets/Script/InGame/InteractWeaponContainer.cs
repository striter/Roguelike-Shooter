using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class InteractWeaponContainer : InteractBase {
    Transform tf_ModelContainer;
    public WeaponBase m_Weapon { get; private set; }
    public override enum_Interaction m_InteractType => enum_Interaction.WeaponContainer;
    public override void Init()
    {
        base.Init();
        tf_ModelContainer = transform.Find("Container/Model");
    }
    public void Play(enum_PlayerWeapon _weapon,List<ActionBase> _actions)
    {
        base.Play();
        m_Weapon = ObjectManager.SpawnWeapon(_weapon, _actions, tf_ModelContainer);
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        m_Weapon = _interactTarget.ObtainWeapon(m_Weapon);
        m_Weapon.transform.SetParent(tf_ModelContainer);
        m_Weapon.transform.localPosition = Vector3.zero;
        m_Weapon.transform.localRotation = Quaternion.identity;
    }
}
