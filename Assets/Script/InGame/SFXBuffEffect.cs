using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBuffEffect : SFXParticles {
    public override void Play(int sourceID, float duration = -1)
    {
        if(duration==-1)
            Debug.LogError("Buff Duration Can't Less Or Equals 0");
        base.Play(sourceID, duration);
    }
    public void Refresh(float refreshDuration)
    {
        f_TimeCheck = Time.time+ refreshDuration + GameConst.F_ParticlesMaxStopTime;
    }
}
