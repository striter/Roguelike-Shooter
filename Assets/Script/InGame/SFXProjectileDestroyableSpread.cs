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
    protected override bool B_DealDamage => false;
    public override void Play(DamageDeliverInfo damageInfo, Vector3 direction, Vector3 targetPosition)
    {
        base.Play(damageInfo, direction, targetPosition);
        if (F_SpreadDuration <= 0)
            Debug.LogError("Spread Duration Less Or Equals 0!");
        if (I_SpreadCount <= 0)
            Debug.LogError("Spread Count Less Or Equals 0!");
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        f_spreadCheck = 0;
        i_spreadCountCheck = 0;
    }
    protected override void Update()
    {
        base.Update();
        if (!B_Playing)
            return;

        f_spreadCheck += Time.deltaTime;
        if (f_spreadCheck < F_SpreadDuration)
            return;

        f_spreadCheck -= F_SpreadDuration;

        Vector3 splitDirection = transform.forward.RotateDirection(Vector3.up, i_spreadCountCheck * I_SpreadAngleEach);
        SFXProjectile projectile = GameObjectManager.SpawnEquipment<SFXProjectile>(GameExpression.GetEquipmentSubIndex(I_SFXIndex), m_CenterPos, Vector3.up);
        projectile.Play(m_DamageInfo.m_detail, splitDirection, m_CenterPos + splitDirection * 10);
        if (projectile.I_MuzzleIndex > 0)
            GameObjectManager.SpawnParticles<SFXMuzzle>(projectile.I_MuzzleIndex, m_CenterPos, splitDirection).Play(m_sourceID);

        i_spreadCountCheck++;
        if (i_spreadCountCheck >= I_SpreadCount)
        {
            m_Health.ForceDeath();
            return;
        }
    }
}
