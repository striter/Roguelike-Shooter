using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class SFXCastDangerzoneSphere : SFXCastOverlapSphere,ISingleCoroutine {
    int i_tickTime;
    public override void Play(int sourceID)
    {
        i_tickTime = 10;
        this.StartSingleCoroutine(0,TIEnumerators.TickCount(OnBlast,i_tickTime,.5f));
        PlaySFX(sourceID,i_tickTime*.5f);
    }
    protected void OnDisable()
    {
        this.StopSingleCoroutine(0);
    }
}
