using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastBase : WeaponBase {
    protected SFXCast ShowCast(Vector3 effectPos )=> GameObjectManager.SpawnSFXWeapon<SFXCast>(m_BaseSFXWeaponIndex, effectPos, m_Attacher.transform.forward);
}
