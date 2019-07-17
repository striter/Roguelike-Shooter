using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCastOverlapSphereDelay : SFXCastOverlapSphere {
    public int F_DelayDuration;
    public int I_IndicatorIndex;
    public override void Play(int sourceID, DamageBuffInfo buffInfo)
    {
        if(I_IndicatorIndex>0)
        ObjectManager.SpawnSFX<SFXIndicator>(I_IndicatorIndex,transform.position,Vector3.up).PlayDuration(sourceID,transform.position,Vector3.up,F_DelayDuration);
        this.StartSingleCoroutine(1,TIEnumerators.PauseDel(F_DelayDuration,()=> { base.Play(sourceID, buffInfo); }));
    }
}
