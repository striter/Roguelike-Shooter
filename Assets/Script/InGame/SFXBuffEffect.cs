using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBuffEffect : SFXParticles {
    protected override bool m_AutoRecycle => f_duration > 0;
    public void Refresh(float refreshDuration)
    {
        if (refreshDuration == 0) refreshDuration = 1f;
            SetLifeTime(refreshDuration );
    }
}
