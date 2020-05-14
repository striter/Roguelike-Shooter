using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponItemBase : WeaponBase {
    public override enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Item;

    protected override void OnAutoTrigger()
    {
        OnAttacherAnim();
    }

    protected override void OnKeyAnim()
    {
        OnAmmoCost();
    }

}
