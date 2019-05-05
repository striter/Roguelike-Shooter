using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZoneParticlesFire : DangerZoneParticles
{
    Light m_light;
    public AnimationCurve LightCurve;
    public bool B_ForceDeactivate = false;
    protected float m_lightStartIntensity;
    protected override void Awake()
    {
        m_light = GetComponentInChildren<Light>();
        m_lightStartIntensity = m_light.intensity;
        base.Awake();
    }

    protected override void OnSetActivate(bool activate)
    {
        base.OnSetActivate(B_Activate&&!B_ForceDeactivate);
        m_light.enabled = B_Activate && !B_ForceDeactivate;
    }
    public void SetForceActivate(bool forceDeactivate)
    {
        B_ForceDeactivate = forceDeactivate;
        OnSetActivate(B_Activate);
    }
    protected override void Update()
    {
        base.Update();
        if(B_Activate&&!B_ForceDeactivate)
            m_light.intensity=LightCurve.Evaluate(Time.time%1)*m_lightStartIntensity;
    }
}
