using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class HitCheckStatic : HitCheckBase {
    public override enum_HitCheck m_HitCheckType => enum_HitCheck.Static;
    public void Attach(Func<DamageInfo,Vector3,bool> OnHit)=> base.Attach(OnHit);
}
