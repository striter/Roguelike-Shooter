using System;
using GameSetting;
using UnityEngine;

public class HitCheckDynamic : HitCheckBase {
    public override enum_HitCheck m_HitCheckType => enum_HitCheck.Dynamic;
    public new void Attach(Func<DamageInfo, Vector3, bool> OnHit) => base.Attach(OnHit);
}
