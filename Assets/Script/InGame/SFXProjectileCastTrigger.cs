using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile {
    public int I_CastIndex;
    protected override bool B_DealDamage => false;
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition, float duration)
    {
        base.Play(sourceID, direction, targetPosition, Vector3.Distance(transform.position, targetPosition) / F_Speed);
    }

    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (I_CastIndex <= 0)
            Debug.LogError("Cast Index Less Or Equals 0");
    }
    protected override void OnPlayFinished()
    {
        base.OnPlayFinished();
        ObjectManager.SpawnSFX<SFXCast>(I_CastIndex, transform.position, Vector3.up).Play(I_SourceID);
    }
}
