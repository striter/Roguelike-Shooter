using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class HitCheckInteract : HitCheckBase {
    public override enum_HitCheck m_HitCheckType => enum_HitCheck.Interact;
    public void Init()=> base.Attach(null);
}
