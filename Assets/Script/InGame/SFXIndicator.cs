using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXIndicator : SFXParticles {
    public void PlayDuration(int sourceID, Vector3 origin, Vector3 normal, float duration)
    {
        transform.position = origin;
        transform.rotation = Quaternion.LookRotation(normal);
        base.Play(sourceID,duration);
    }
}
