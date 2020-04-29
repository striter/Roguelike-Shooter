using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCastMelee : WeaponCastBase {

    public override void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType)
    {
        base.OnAnimEvent(eventType);
        if (eventType != TAnimatorEvent.enum_AnimEvent.Fire)
            return;
        ShowCast().Play(GetWeaponDamageInfo(m_WeaponInfo.m_Damage));
    }
}
