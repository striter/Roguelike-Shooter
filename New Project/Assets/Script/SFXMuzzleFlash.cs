using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXMuzzleFlash : SFXParticles
{
    protected Light[] ar_lights;
    public ParticleSystem[] ar_particlesWorldParent;
    public float F_LightsOutTime = .3f;
    float f_lightsOutCheck=0f;
    protected override void Awake()
    {
        ar_lights = GetComponentsInChildren<Light>();
        base.Awake();
        TCommon.TraversalArray(ar_particles, (ParticleSystem ps) =>
        {
            f_duration = ps.main.duration > f_duration ? ps.main.duration : f_duration;
        });
    }
    
    public override void Reset()
    {
        base.Reset();
        TCommon.TraversalArray(ar_lights, (Light lt) =>
        {
            lt.enabled = false;
        });
        TCommon.TraversalArray(ar_particlesWorldParent, (ParticleSystem ps) =>
        {
            ps.transform.SetParent(this.transform);
            ps.transform.localPosition = Vector3.zero;
            ps.transform.localRotation = Quaternion.identity;
        });
    }
    public override void Play()
    {
        base.Play();
        f_lightsOutCheck = Time.time + F_LightsOutTime;
        transform.localRotation = Quaternion.Euler(0,0, Random.Range(0f, 360f));
        TCommon.TraversalArray(ar_lights, (Light lt) =>
        {
            lt.enabled = true;
        });
        TCommon.TraversalArray(ar_particlesWorldParent, (ParticleSystem ps) =>
        {
            ps.transform.SetParent(null);
        });
    }
    protected override void OnTickDelta(float delta)
    {
        base.OnTickDelta(delta);
        if (Time.time > f_lightsOutCheck)
        {
            TCommon.TraversalArray(ar_lights, (Light lt) => { lt.enabled = false; });
        }
    }
}
