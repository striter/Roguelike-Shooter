using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using GameSetting_Action;
public class InteractWeaponContainer : InteractBase {
    Transform tf_ModelContainer;
    WeaponBase m_Weapon;
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
    public override bool TryInteract(EntityPlayerBase _interactor)
    {
        m_Weapon=_interactor.ObtainWeapon(m_Weapon);
        m_Weapon.transform.SetParent(tf_ModelContainer);
        m_Weapon.transform.localPosition = Vector3.zero;
        m_Weapon.transform.localRotation = Quaternion.identity;
        return base.TryInteract(_interactor);
    }
}
