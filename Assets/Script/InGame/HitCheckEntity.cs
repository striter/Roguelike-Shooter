using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class HitCheckEntity : HitCheckBase {
    public EntityBase m_Attacher { get; private set; } = null;
    public int I_AttacherID => m_Attacher.I_EntityID;
    public override enum_HitCheck m_HitCheckType => enum_HitCheck.Entity;
    List<Transform> m_Attaches = new List<Transform>();
    public void Attach(EntityBase _attacher, Func<float, bool> _OnHitCheck)
    {
        base.Attach(_OnHitCheck);
        m_Attacher = _attacher;
    }
    public void AttachTransform(Transform attachment)
    {
        attachment.SetParent(this.transform);
        m_Attaches.Add(attachment);
    }
    public void HideAllAttaches()
    {
        m_Attaches.Traversal((Transform temp) => { temp.SetActivate(false); });
    }
}
