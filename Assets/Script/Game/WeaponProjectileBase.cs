using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileBase : WeaponBase
{
    protected override void OnAutoTriggerSuccessful()
    {
        base.OnAutoTriggerSuccessful();
        FireProjectiles(m_BaseSFXWeaponIndex, GetWeaponDamageInfo(F_BaseDamage));
    }


    RaycastHit hit;
    protected void FireProjectiles(int projectileIndex,DamageInfo damageInfo)
    {
        SFXProjectile projectileData = GameObjectManager.GetSFXWeaponData<SFXProjectile>(projectileIndex);
        Vector3 spreadDirection = m_Attacher.tf_WeaponAim.forward;
        Vector3 endPosition = m_Attacher.tf_WeaponAim.position + spreadDirection * GameConst.I_ProjectileMaxDistance;
        if (Physics.Raycast(m_Attacher.tf_WeaponAim.position, spreadDirection, out hit, GameConst.I_ProjectileMaxDistance, GameLayer.Mask.I_ProjectileMask) && GameManager.B_CanSFXHitTarget(hit.collider.Detect(), m_Attacher.m_EntityID))
            endPosition = hit.point;
        spreadDirection = (endPosition - m_Muzzle.position).normalized;
        spreadDirection.y = 0;

        float spread = GetSpread();
        if (m_WeaponInfo.m_PelletsPerShot == 1)
        {
            FireProjectile(projectileIndex,projectileData, damageInfo, spreadDirection.RotateDirectionClockwise(Vector3.up, UnityEngine.Random.Range(-spread, spread)));
        }
        else
        {
            int waveCount = m_WeaponInfo.m_PelletsPerShot;
            float beginAnle = -spread * (waveCount - 1) / 2f;
            for (int i = 0; i < waveCount; i++)
                FireProjectile(projectileIndex, projectileData,damageInfo, spreadDirection.RotateDirectionClockwise(Vector3.up, beginAnle + i * m_WeaponInfo.m_Spread));
        }
        GameObjectManager.PlayMuzzle(m_Attacher.m_EntityID, m_Muzzle.position, spreadDirection, projectileData.I_MuzzleIndex, projectileData.AC_MuzzleClip);
    }

    void FireProjectile(int projectilIndex, SFXProjectile projectileData,DamageInfo damageInfo, Vector3 direction)
    {
        SFXProjectile projectile = GameObjectManager.SpawnSFXWeapon<SFXProjectile>(projectilIndex, m_Muzzle.position, direction);
        projectile.F_Speed = m_Attacher.m_CharacterInfo.F_ProjectileSpeedMuiltiply*projectileData.F_Speed;
        projectile.B_Penetrate = projectileData.B_Penetrate || m_Attacher.m_CharacterInfo.F_PenetrateAdditive > UnityEngine.Random.Range(0, 1);

        projectile.Play(damageInfo, direction, m_Attacher.tf_Weapon.position + direction * GameConst.I_ProjectileMaxDistance);
    }
}
