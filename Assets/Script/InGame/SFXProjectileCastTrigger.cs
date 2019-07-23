using GameSetting;
using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile {
    public int I_CastIndex;
    protected override bool B_DealDamage => false;
    protected override float F_Duration(Vector3 startPos, Vector3 endPos) => Vector3.Distance(startPos, endPos) / F_Speed;
    protected bool b_trigger = false;
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        b_trigger = false;
        if (I_CastIndex <= 0)
            Debug.LogError("Cast Index Less Or Equals 0");
    }
    protected override void OnPlayFinished()
    {
        base.OnPlayFinished();
        OnCastTrigger();
    }
    protected void OnCastTrigger()
    {
        if (b_trigger)
            return;
        ObjectManager.SpawnCommonSFX<SFXCast>(I_CastIndex, transform.position, Vector3.up).Play(I_SourceID, m_DamageInfo.m_BuffApply);
        b_trigger = true;
    }
}
