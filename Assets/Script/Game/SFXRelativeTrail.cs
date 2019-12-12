using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TrailRenderer))]
public class SFXRelativeTrail : SFXRelativeBase {
    TrailRenderer m_Trail;
    public override void Init()
    {
        base.Init();
        m_Trail = transform.GetComponentInChildren<TrailRenderer>();
    }
    public override void Play(SFXParticles _source)
    {
        base.Play(_source);
        m_Trail.Clear();
    }
}
