using GameSetting;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class SFXBlastOverlapSphere : SFXBlast {
    SphereCollider m_Collider;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Collider = GetComponent<SphereCollider>();
        m_Collider.enabled = false;
    }
    protected override Collider[] BlastCheck()
    {
        return Physics.OverlapSphere(transform.position, m_Collider.radius/2, GameLayer.Physics.I_EntityOnly);
    }
    protected override void OnBlastEntity(HitCheckEntity hitEntity)
    {
        if (GameManager.B_CanHitTarget(hitEntity, I_SourceID))
            hitEntity.TryHit(GameExpression.F_RocketBlastDamage(f_damage, Vector3.Distance(transform.position, hitEntity.transform.position)));
    }
}
