using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastBase : WeaponBase {
    SFXCast m_Cast;
    protected override void OnGetEquipmentData(SFXEquipmentBase equipment)
    {
        base.OnGetEquipmentData(equipment);
        SFXCast cast = equipment as SFXCast;
        F_BaseDamage = cast.F_Damage;
    }

    bool casting => m_Trigger.B_TriggerDown && !B_Reloading&&m_Attacher.m_weaponCanFire;
    void Update()
    {
        SetCastAvailable(casting);
    }
    void SetCastAvailable(bool showCast)
    {

        if (showCast)
        {
            if (m_Cast)
                return;
            m_Cast = GameObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetPlayerEquipmentIndex(m_WeaponInfo.m_Index), m_Muzzle.position, m_Attacher.transform.forward);
            m_Cast.PlayControlled(m_Attacher.m_EntityID, m_Muzzle, m_Attacher.tf_WeaponAim, m_Attacher.m_PlayerInfo.GetDamageBuffInfo());
        }
        else
        {
            if (!m_Cast)
                return;
            m_Cast.StopControlled();
            m_Cast = null;
        }
    }

    protected override void OnTriggerSuccessful()
    {
        base.OnTriggerSuccessful();
        if(m_Cast)
            m_Cast.OnControlledCheck(m_Attacher.m_PlayerInfo.GetDamageBuffInfo());
    }
}
