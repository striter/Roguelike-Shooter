using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class HitCheckEntity : HitCheckBase {
    public EntityBase m_Attacher { get; private set; } = null;
    public override enum_HitCheck m_HitCheckType => enum_HitCheck.Entity;
    public override int I_AttacherID => m_Attacher.m_EntityID;
    public void Attach(EntityBase _attacher, Func<DamageInfo,Vector3, bool> _OnHitCheck)
    {
        base.Attach(_OnHitCheck);
        m_Attacher = _attacher;
    }
}
