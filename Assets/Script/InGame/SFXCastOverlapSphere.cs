using GameSetting;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class SFXCastOverlapSphere : SFXCast {
    public bool B_DamageDistanceReduction = true;
    SphereCollider m_Collider;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Collider = GetComponent<SphereCollider>();
        m_Collider.enabled = false;
    }
    protected override Collider[] OnBlastCheck()
    {
        return Physics.OverlapSphere(transform.position, m_Collider.radius, GameLayer.Physics.I_EntityOnly);
    }
    protected override void OnDamageEntity(HitCheckEntity hitEntity)
    {
        hitEntity.TryHit(B_DamageDistanceReduction? GameExpression.F_SphereCastDamageReduction(F_Damage,Vector3.Distance(transform.position,hitEntity.m_Attacher.transform.position)):F_Damage);
    }
}
