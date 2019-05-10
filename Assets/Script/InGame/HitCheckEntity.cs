using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class HitCheckEntity : HitCheckBase {
    public int I_AttacherID { get; private set; }=-1;
    public override enum_HitCheck m_HitCheckType => enum_HitCheck.Entity;
    public void Attach(int _attacherID, Func<float, bool> _OnHitCheck)
    {
        base.Attach(_OnHitCheck);
        I_AttacherID = _attacherID;
    }
}
