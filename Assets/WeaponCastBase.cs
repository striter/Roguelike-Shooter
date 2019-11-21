using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastBase : WeaponBase {
    protected override void OnGetEquipmentData(SFXEquipmentBase equipment)
    {
        base.OnGetEquipmentData(equipment);
        SFXCast cast = equipment as SFXCast;
        F_BaseDamage = cast.F_Damage;
    }

    protected override void OnTriggerSuccessful()
    {
        base.OnTriggerSuccessful();
        DamageDeliverInfo damageInfo = m_Attacher.m_PlayerInfo.GetDamageBuffInfo();

        SFXCast cast = GameObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetPlayerEquipmentIndex(m_WeaponInfo.m_Index), m_Muzzle.position, m_Attacher.transform.forward);
        cast.Play(damageInfo);
    }
}
