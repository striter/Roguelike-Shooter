using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageCast : WeaponDamageBase {
    public override enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Cast;
    protected SFXCast ShowCast(int castIndex,Vector3 effectPos )=> GameObjectManager.SpawnSFXWeapon<SFXCast>(castIndex, effectPos, m_Attacher.transform.forward);
}
