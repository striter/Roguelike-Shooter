using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXIndicator : SFXParticles {
    public override void Play(int sourceID, float duration)
    {
        base.Play(sourceID, duration);
        Play();
    }
}
