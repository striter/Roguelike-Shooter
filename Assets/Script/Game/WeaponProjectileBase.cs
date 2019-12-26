using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileBase : WeaponBase
{
    public float F_BaseSpeed { get; private set; } = -1;
    public bool B_BasePenetrate { get; private set; } = false;
    public int I_MuzzleIndex { get; private set; } = -1;
    AudioClip m_MuzzleClip;
    public float F_Speed => m_Attacher.m_PlayerInfo.F_ProjectileSpeedMuiltiply * F_BaseSpeed;
    public bool B_Penetrate => m_Attacher.m_PlayerInfo.B_ProjectilePenetrate || B_BasePenetrate;

    protected override void OnGetEquipmentData(SFXWeaponBase equipment)
    {
        base.OnGetEquipmentData(equipment);
        SFXProjectile projectileInfo = equipment as SFXProjectile;
        F_BaseDamage = projectileInfo.F_Damage;
        F_BaseSpeed = projectileInfo.F_Speed;
        B_BasePenetrate = projectileInfo.B_Penetrate;
        I_MuzzleIndex = projectileInfo.I_MuzzleIndex;
        m_MuzzleClip = projectileInfo.AC_MuzzleClip;
    }

    RaycastHit hit;
    protected override void OnTriggerSuccessful()
    {
        base.OnTriggerSuccessful();
        DamageDeliverInfo damageInfo = m_Attacher.m_PlayerInfo.GetDamageBuffInfo();

        Vector3 spreadDirection = m_Attacher.tf_WeaponAim.forward;
        Vector3 endPosition = m_Attacher.tf_WeaponAim.position + spreadDirection * GameConst.I_ProjectileMaxDistance;
        if (Physics.Raycast(m_Attacher.tf_WeaponAim.position, spreadDirection, out hit, GameConst.I_ProjectileMaxDistance, GameLayer.Mask.I_All) && GameManager.B_CanSFXHitTarget(hit.collider.Detect(), m_Attacher.m_EntityID))
            endPosition = hit.point;
        spreadDirection = (endPosition - m_Muzzle.position).normalized;
        spreadDirection.y = 0;

        if (m_WeaponInfo.m_PelletsPerShot == 1)
        {
            FireOneBullet(damageInfo, spreadDirection.RotateDirection(Vector3.up,Random.Range(-m_WeaponInfo.m_Spread, m_WeaponInfo.m_Spread)));
        }
        else
        {
            int waveCount = m_WeaponInfo.m_PelletsPerShot;
            float beginAnle = -m_WeaponInfo.m_Spread * (waveCount - 1) / 2f;
            for (int i = 0; i < waveCount; i++)
                FireOneBullet(damageInfo, spreadDirection.RotateDirection(Vector3.up, beginAnle + i * m_WeaponInfo.m_Spread));
        }
        GameObjectManager.PlayMuzzle(m_Attacher.m_EntityID, m_Muzzle.position, spreadDirection, I_MuzzleIndex, m_MuzzleClip);
    }

    void FireOneBullet(DamageDeliverInfo damage, Vector3 direction)
    {
        SFXProjectile projectile = GameObjectManager.SpawnEquipment<SFXProjectile>(GameExpression.GetPlayerWeaponIndex(m_WeaponInfo.m_Index), m_Muzzle.position, direction);
        projectile.F_Speed = F_Speed;
        projectile.B_Penetrate = B_Penetrate;
        projectile.Play(damage, direction, m_Attacher.tf_Weapon.position + direction * GameConst.I_ProjectileMaxDistance);
    }
}
