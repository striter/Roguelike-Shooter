using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastBase : WeaponBase {
    public override enum_PlayerWeaponType m_WeaponType => enum_PlayerWeaponType.Cast;
    protected SFXCast ShowCast(Vector3 effectPos )=> GameObjectManager.SpawnSFXWeapon<SFXCast>(m_BaseSFXWeaponIndex, effectPos, m_Attacher.transform.forward);
}
