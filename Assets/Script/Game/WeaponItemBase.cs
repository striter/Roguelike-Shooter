using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponItemBase : WeaponBase {
    public override enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Item;

    protected override void OnAutoTrigger()
    {
        OnAttacherAnim(0);
    }

    public override void OnAnimEvent(TAnimatorEvent.enum_AnimEvent eventType)
    {
        base.OnAnimEvent(eventType);
        if (eventType != TAnimatorEvent.enum_AnimEvent.Fire)
            return;

        OnAmmoCost();
    }

}
