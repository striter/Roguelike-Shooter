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
    public float GetSpeed() => m_Attacher.m_CharacterInfo.F_ProjectileSpeedMuiltiply * F_BaseSpeed;
    public bool GetPenetrate() => B_BasePenetrate||m_Attacher.m_CharacterInfo.F_PenetrateAdditive>Random.Range(0,1);
    public float GetSpread() => m_Attacher.m_CharacterInfo.F_SpreadMultiply * m_WeaponInfo.m_Spread;

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
        DamageDeliverInfo damageInfo = m_Attacher.m_CharacterInfo.GetDamageBuffInfo();

        Vector3 spreadDirection = m_Attacher.tf_WeaponAim.forward;
        Vector3 endPosition = m_Attacher.tf_WeaponAim.position + spreadDirection * GameConst.I_ProjectileMaxDistance;
        if (Physics.Raycast(m_Attacher.tf_WeaponAim.position, spreadDirection, out hit, GameConst.I_ProjectileMaxDistance, GameLayer.Mask.I_All) && GameManager.B_CanSFXHitTarget(hit.collider.Detect(), m_Attacher.m_EntityID))
            endPosition = hit.point;
        spreadDirection = (endPosition - m_Muzzle.position).normalized;
        spreadDirection.y = 0;

        float spread = GetSpread();
        if (m_WeaponInfo.m_PelletsPerShot == 1)
        {
            FireOneBullet(damageInfo, spreadDirection.RotateDirectionClockwise(Vector3.up,Random.Range(-spread,spread)));
        }
        else
        {
            int waveCount = m_WeaponInfo.m_PelletsPerShot;
            float beginAnle = -spread * (waveCount - 1) / 2f;
            for (int i = 0; i < waveCount; i++)
                FireOneBullet(damageInfo, spreadDirection.RotateDirectionClockwise(Vector3.up, beginAnle + i * m_WeaponInfo.m_Spread));
        }
        GameObjectManager.PlayMuzzle(m_Attacher.m_EntityID, m_Muzzle.position, spreadDirection, I_MuzzleIndex, m_MuzzleClip);
    }

    void FireOneBullet(DamageDeliverInfo damage, Vector3 direction)
    {
        SFXProjectile projectile = GameObjectManager.SpawnEquipment<SFXProjectile>(GameExpression.GetPlayerWeaponIndex(m_WeaponInfo.m_Index), m_Muzzle.position, direction);
        projectile.F_Speed = GetSpeed();
        projectile.B_Penetrate = GetPenetrate();
        projectile.PlayerCopyCount(damage, direction, m_Attacher.tf_Weapon.position + direction * GameConst.I_ProjectileMaxDistance,m_Attacher.m_CharacterInfo.I_ProjectileCopyCount,10);
    }
}
