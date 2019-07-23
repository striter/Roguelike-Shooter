using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileCastMeteor : SFXProjectileCastTrigger
{
    public int I_IndicatorIndex;
    public float F_SpreadRadius=5;
    public float F_StartHeight = 20;
    SFXIndicator m_CastIndicator;
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (I_IndicatorIndex <= 0)
            Debug.LogError("Indicator Index Less Or Equals 0");
    }
    public override void Play(int sourceID, Vector3 direction, Vector3 targetPosition,DamageBuffInfo buffInfo)
    {
        Vector3 startPos = targetPosition + Vector3.up * F_StartHeight + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * F_SpreadRadius;
        Vector3 spreadDirection = (targetPosition - startPos).normalized;
        transform.position = startPos;
        m_CastIndicator = ObjectManager.SpawnCommonIndicator(I_IndicatorIndex, targetPosition, Vector3.up);
        m_CastIndicator.Play(sourceID, F_Duration(transform.position, targetPosition));
        base.Play(sourceID, spreadDirection, targetPosition,buffInfo);
    }
    protected override void OnPlayFinished()
    {
        base.OnPlayFinished();
        m_CastIndicator.ForceStop();
    }
}
