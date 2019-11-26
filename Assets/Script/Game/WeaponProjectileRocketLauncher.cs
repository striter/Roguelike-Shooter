using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileRocketLauncher : WeaponProjectileBase {

    protected override void OnGetEquipmentData(SFXEquipmentBase equipment)
    {
        base.OnGetEquipmentData(equipment);
        SFXCast cast = GameObjectManager.GetEquipmentData<SFXCast>(GameExpression.GetPlayerSubEquipmentIndex((int)m_WeaponInfo.m_Weapon));
        F_BaseDamage = cast.F_Damage;
    }
}
