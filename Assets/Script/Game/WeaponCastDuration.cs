using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastDuration : WeaponCastBase {
    SFXCast m_Cast;
    public bool m_Casting => m_Cast;
    protected override void OnAutoTriggerSuccessful()
    {
        base.OnAutoTriggerSuccessful();
        SetCastAvailable(true);
        if (m_Cast)
            m_Cast.ControlledCheck(m_Attacher.m_CharacterInfo.GetDamageBuffInfo());
    }

    public override void OnPlay(bool play)
    {
        base.OnPlay(play);
        if (!play)
            SetCastAvailable(false);
    }

    void Update()
    {
        SetCastAvailable(m_Attacher != null && m_Trigger.m_TriggerDown && m_HaveAmmoLeft && !m_Attacher.m_IsDead && m_Attacher.m_weaponCanFire);
    }

    void SetCastAvailable(bool showCast)
    {
        if (m_Casting == showCast)
            return;

        if (showCast)
        {
            m_Cast = ShowCast();
            m_Cast.PlayControlled(m_Attacher.m_EntityID, m_Attacher, m_Attacher.tf_WeaponAim);
        }
        else
        {
            m_Cast.StopControlled();
            m_Cast = null;
        }
    }
}
