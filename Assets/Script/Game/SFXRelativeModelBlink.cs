using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXRelativeModelBlink : SFXRelativeBase {
    public bool B_BlinkDelay;
    public Color C_BlinkColor = Color.red;
    bool m_blinking;
    protected ModelBlink m_Blink;
    public override void Init()
    {
        base.Init();
        m_Blink = new ModelBlink(transform, .25f, .25f, C_BlinkColor);
    }
    public override void Play(SFXParticles _source)
    {
        base.Play(_source);
        m_Blink.OnReset();
        m_blinking = B_BlinkDelay;
        m_Blink.SetShow(m_blinking);
    }
    public override void OnPlay()
    {
        base.OnPlay();
        m_blinking = !B_BlinkDelay;
        m_Blink.SetShow(m_blinking);
    }
    public override void OnStop()
    {
        base.OnStop();
        m_blinking = !B_BlinkDelay;
        m_Blink.SetShow(m_blinking);
    }

    private void Update()
    {
        if (!m_blinking)
            return;

        float blinkScale = B_BlinkDelay ? m_SFXSource.f_delayLeftScale : m_SFXSource.f_playTimeLeft/GameConst.I_ProjectileBlinkWhenTimeLeftLessThan;
            float timeMultiply = 2f * (1 - blinkScale);
            m_Blink.Tick(Time.deltaTime * timeMultiply);
    }

}
