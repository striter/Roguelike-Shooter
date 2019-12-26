using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastBase : WeaponBase {
    SFXCast m_Cast;
    public bool m_Casting => m_Cast;
    protected override void OnGetEquipmentData(SFXEquipmentBase equipment)
    {
        base.OnGetEquipmentData(equipment);
        SFXCast cast = equipment as SFXCast;
        F_BaseDamage = cast.F_Damage;
    }


    protected override void OnTriggerSuccessful()
    {
        base.OnTriggerSuccessful();
        SetCastAvailable(true);
        if (m_Cast)
            m_Cast.ControlledCheck(m_Attacher.m_PlayerInfo.GetDamageBuffInfo());
    }

    public override void OnPlay(bool play)
    {
        base.OnPlay(play);
        if (!play)
            SetCastAvailable(false);
    }

    void Update()
    {
        SetCastAvailable(m_Attacher!=null&&m_Trigger.B_TriggerDown && !B_Reloading && !m_Attacher.m_IsDead && m_Attacher.m_weaponCanFire);
    }

    void SetCastAvailable(bool showCast)
    {
        if (m_Casting == showCast)
            return;

        if (showCast)
        {
            m_Cast = GameObjectManager.SpawnEquipment<SFXCast>(GameExpression.GetPlayerWeaponIndex(m_WeaponInfo.m_Index), m_Muzzle.position, m_Muzzle.forward);
            m_Cast.PlayControlled(m_Attacher.m_EntityID, m_Attacher, m_Attacher.tf_WeaponAim);
        }
        else
        {
            m_Cast.StopControlled();
            m_Cast = null;
        }
    }
}
