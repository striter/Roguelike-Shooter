using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXBuffEffect : SFXParticles {
    float f_buffRefresh;
    public override void Play(int sourceID, float duration = -1)
    {
        if(duration==-1)
            Debug.LogError("Buff Duration Can't Less Or Equals 0");
        base.Play(sourceID, duration);
        f_buffRefresh = duration;
    }
    public void Refresh()
    {
        f_TimeCheck = Time.time + f_buffRefresh;
    }
}
