using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBuffEffect : SFXParticles
{
    public void Refresh(float refreshDuration)
    {
        SetLifeTime(refreshDuration);
    }
}
