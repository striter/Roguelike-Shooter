using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXParticlesImpact : SFXParticles {
    Transform tf_HitMark;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        tf_HitMark = transform.Find("HitMark");
    }
    public void SetHitMarkShow(bool show)
    {
        tf_HitMark.SetActivate(show);
    }
}
