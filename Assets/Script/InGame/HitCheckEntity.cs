using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class HitCheckEntity : HitCheckBase {
    public EntityBase m_Attacher { get; private set; } = null;
    public override enum_HitCheck m_HitCheckType => enum_HitCheck.Entity;
    public void Attach(EntityBase _attacher, Func<DamageInfo, bool> _OnHitCheck)
    {
        base.Attach(_attacher.I_EntityID,_OnHitCheck);
        m_Attacher = _attacher;
    }
    List<SFXBase> m_Attaches = new List<SFXBase>();
    public void AttachTransform(SFXBase attachment)
    {
        attachment.transform.SetParent(this.transform);
        m_Attaches.Add(attachment);
    }
    public void HideAllAttaches()
    {
        m_Attaches.Traversal((SFXBase temp) => { temp.ForceStop(); });
    }
}
