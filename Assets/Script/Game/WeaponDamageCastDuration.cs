using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageCastDuration : WeaponDamageCast {
    SFXCast m_Cast;
    public bool m_Casting => m_Cast;
    protected override void OnAutoTrigger()
    {
        SetCastAvailable(true);
        m_Cast.ControlledCheck(GetWeaponDamageInfo(m_BaseDamage));
        OnAmmoCost();
    }

    public override void OnShow(bool play)
    {
        base.OnShow(play);
        if (!play)
            SetCastAvailable(false);
    }

    public override void Tick(bool firePausing, float deltaTime)
    {
        base.Tick(firePausing, deltaTime);
        SetCastAvailable(m_Trigger.m_TriggerDown && m_HaveAmmoLeft && !m_Attacher.m_IsDead && !m_Attacher.m_weaponFirePause);
    }

    void SetCastAvailable(bool showCast)
    {
        if (m_Casting == showCast)
            return;

        if (showCast)
        {
            m_Cast = ShowCast(m_BaseSFXWeaponIndex, m_Muzzle.position);
            m_Cast.PlayControlled(m_Attacher.m_EntityID, m_Attacher, m_Attacher.tf_WeaponAim);
        }
        else
        {
            m_Cast.StopControlled();
            m_Cast = null;
        }
    }
}
