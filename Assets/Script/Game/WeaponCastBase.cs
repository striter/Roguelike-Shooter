using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastBase : WeaponBase {
    protected SFXCast ShowCast()=> GameObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetPlayerWeaponIndex(m_WeaponInfo.m_Index), m_Muzzle.position, m_Attacher.transform.forward);

}
