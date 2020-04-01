using GameSetting;
using TPhysics;
using UnityEngine;

public class SFXProjectileCastMeteor : SFXProjectileCastTrigger
{
    public float F_SpreadRadius=5;
    public float F_StartHeight = 20;
    public override void Play(DamageInfo damageInfo, Vector3 direction, Vector3 targetPosition)
    {
        Vector3 startPos = targetPosition + Vector3.up * F_StartHeight + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * F_SpreadRadius;
        Vector3 spreadDirection = (targetPosition - startPos).normalized;
        transform.position = startPos;
        base.Play(damageInfo,spreadDirection, targetPosition);
    }

    protected override void EDITOR_DEBUG()
    {
        base.EDITOR_DEBUG();
        if (I_IndicatorIndex <= 0)
            Debug.LogError("Meteor Indicator Index Less Or Equals 0");
    }
}
