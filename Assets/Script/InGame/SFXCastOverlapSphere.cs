using GameSetting;
using UnityEngine;
public class SFXCastOverlapSphere : SFXCast
{
    public bool B_DamageDistanceReduction = true;
    protected override void OnDamageEntity(HitCheckEntity hitEntity)
    {
        float distance = Vector3.Distance(transform.position, hitEntity.m_Attacher.transform.position);
        if (distance > V4_CastInfo.x)
            return;
        m_DamageInfo.ResetBaseDamage(B_DamageDistanceReduction ? GameExpression.F_SphereCastDamageReduction(F_Damage, distance, V4_CastInfo.x) : F_Damage);
        base.OnDamageEntity(hitEntity);
    }
}
