using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class SFXRelativeLight : SFXRelativeBase {
    Light m_Light;
    public override void Init()
    {
        if(!m_Light)
        m_Light = GetComponent<Light>();
    }
    private void Start()
    {
        OptionsManager.event_OptionChanged += OnOptionChange;
    }
    private void OnDestroy()
    {
        OptionsManager.event_OptionChanged -= OnOptionChange;
    }
    void OnOptionChange()
    {
        if (m_Light.enabled && !OptionsManager.B_AdditionalLight)
            m_Light.enabled = false;
    }
    public override void Play()
    {
        if (!OptionsManager.B_AdditionalLight)
            return;
        m_Light.enabled = true;
    }

    public override void Stop()
    {
        if (!OptionsManager.B_AdditionalLight)
            return;
        m_Light.enabled = false;
    }
}
