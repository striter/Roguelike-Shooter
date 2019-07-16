using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile {
    public int I_CastIndex;
    protected override bool B_DealDamage => false;
    protected override float F_Duration(Vector3 startPos, Vector3 endPos) => Vector3.Distance(startPos, endPos) / F_Speed;

    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (I_CastIndex <= 0)
            Debug.LogError("Cast Index Less Or Equals 0");
    }
    protected override void OnPlayFinished()
    {
        base.OnPlayFinished();
        ObjectManager.SpawnSFX<SFXCast>(I_CastIndex, transform.position, Vector3.up).Play(I_SourceID,m_DamageInfo.m_BuffApply);
    }
}
