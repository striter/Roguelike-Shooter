using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastBase : WeaponBase {

    public override float F_BaseDamage => GameObjectManager.GetSFXWeaponData<SFXCast>(m_BaseSFXWeaponIndex).F_Damage;
    protected SFXCast ShowCast()=> GameObjectManager.SpawnSFXWeapon<SFXCast>(m_BaseSFXWeaponIndex, m_Muzzle.position, m_Attacher.transform.forward);

}
