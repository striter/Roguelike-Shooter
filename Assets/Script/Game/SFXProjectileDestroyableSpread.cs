using GameSetting;
using UnityEngine;

public class SFXProjectileDestroyableSpread : SFXProjectileDestroyable {
    
    [Range(0, 90)]
    public int I_SpreadAngleEach = 30;
    public float F_SpreadDuration = .5f;
    public int I_SpreadCount = 10;
    int i_spreadCountCheck = 0;
    float f_spreadCheck = 0;
    protected override bool B_StopParticlesOnHit => false;
    protected override bool B_StopPhysicsOnHit => false;
    protected override bool B_DealDamage => false;
    protected override float F_PlayDuration(Vector3 startPos, Vector3 endPos) => (I_SpreadCount+1) * F_SpreadDuration;
    protected override void OnPlay()
    {
        base.OnPlay();
        f_spreadCheck = F_SpreadDuration;
        i_spreadCountCheck = 0;
    }
    protected override void Update()
    {
        base.Update();
        if (!B_Playing)
            return;

        f_spreadCheck -= Time.deltaTime;
        if (f_spreadCheck > 0)
            return;
        f_spreadCheck = F_SpreadDuration;

        Vector3 splitDirection = transform.forward.RotateDirection(Vector3.up, i_spreadCountCheck * I_SpreadAngleEach);
        SFXProjectile projectile = GameObjectManager.SpawnEquipment<SFXProjectile>(GameExpression.GetEquipmentSubIndex(I_SFXIndex), m_CenterPos, Vector3.up);
        m_DamageInfo.m_detail.EntityComponentOverride(m_Health.m_EntityID);
        projectile.Play( m_DamageInfo.m_detail, splitDirection, m_CenterPos + splitDirection * 10);
        GameObjectManager.PlayMuzzle(m_sourceID,m_CenterPos,splitDirection,projectile.I_MuzzleIndex,projectile.AC_MuzzleClip);
        i_spreadCountCheck++;
    }

    protected override void EDITOR_DEBUG()
    {
        base.EDITOR_DEBUG();
        if (F_SpreadDuration <= 0)
            Debug.LogError("Spread Duration Less Or Equals 0!");
        if (I_SpreadCount <= 0)
            Debug.LogError("Spread Count Less Or Equals 0!");
    }
}
