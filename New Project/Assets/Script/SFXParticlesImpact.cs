using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXParticlesImpact : SFXParticles
{
    Transform tf_BulletDecal;
    protected override void Awake()
    {
        base.Awake();
        f_duration = GameSettings.CI_DecalLifeTime;
        tf_BulletDecal = transform.Find("BulletDecal");
    }
    public void Play(bool showDecal)
    {
        tf_BulletDecal?.SetActivate(showDecal);
        base.Play();
    }
}
