using GameSetting;
using UnityEngine;
public class SFXCastOverlapSphere : SFXCast
{
    public float F_Radius = 5;
    public bool B_DamageDistanceReduction = true;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
    }
    protected override Collider[] OnBlastCheck()
    {
        return Physics.OverlapSphere(transform.position, F_Radius, GameLayer.Physics.I_EntityOnly);
    }
    protected override void OnDamageEntity(HitCheckEntity hitEntity)
    {
        float distance = Vector3.Distance(transform.position, hitEntity.m_Attacher.transform.position);
        if (distance > F_Radius)
            return;
        m_DamageInfo.ResetDamage(B_DamageDistanceReduction ? GameExpression.F_SphereCastDamageReduction(F_Damage, distance, F_Radius) : F_Damage);
        base.OnDamageEntity(hitEntity);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_GizmosInGame)
            return;
        Gizmos.color = GetGizmosColor();
        Gizmos.DrawWireSphere(transform.position,F_Radius);  
    }
#endif
}
