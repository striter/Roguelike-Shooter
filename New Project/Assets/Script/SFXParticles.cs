using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXParticles : SFXBase
{

    protected ParticleSystem[] ar_particles;
    protected override void Awake()
    {
        ar_particles = GetComponentsInChildren<ParticleSystem>();
       base.Awake();
    }
    public override void Reset()
    {
        base.Reset();
        TCommon.TraversalArray(ar_particles, (ParticleSystem ps) =>
        {
            ps.Stop();
        });
    }
    public override void Play()
    {
        base.Play();
        TCommon.TraversalArray(ar_particles, (ParticleSystem ps) =>
        {
            ps.Play();
        });
    }
}
