using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileBase : WeaponBase
{
    public float F_AimSpread;

    protected override enum_PlayerWeaponType m_WeaponType => enum_PlayerWeaponType.ProjectileShot;
    public float GetAimSpread() => m_Attacher.m_CharacterInfo.F_AimSpreadMultiply * F_AimSpread;
    protected override void OnAutoTriggerSuccessful()
    {
        base.OnAutoTriggerSuccessful();
        OnTriggerOnce(m_BaseSFXWeaponIndex, GetWeaponDamageInfo(m_WeaponInfo.m_Damage));
    }

    RaycastHit hit;
    protected void OnTriggerOnce(int projectileIndex,DamageInfo damageInfo)
    {
        Vector3 spreadDirection = m_Attacher.tf_WeaponAim.forward;
        Vector3 endPosition = m_Attacher.tf_WeaponAim.position + spreadDirection * GameConst.I_ProjectileMaxDistance;
        if (Physics.Raycast(m_Attacher.tf_WeaponAim.position, spreadDirection, out hit, GameConst.I_ProjectileMaxDistance, GameLayer.Mask.I_ProjectileMask) && GameManager.B_CanSFXHitTarget(hit.collider.Detect(), m_Attacher.m_EntityID))
            endPosition = hit.point;
        spreadDirection = (endPosition - m_Muzzle.position).normalized;
        spreadDirection.y = 0;

        float aimSpread = GetAimSpread();

        SFXProjectile projectileData = GameObjectManager.GetSFXWeaponData<SFXProjectile>(projectileIndex);
        FireProjectileTowards(spreadDirection.RotateDirectionClockwise(Vector3.up, UnityEngine.Random.Range(-aimSpread, aimSpread)), projectileIndex, projectileData,damageInfo);
        GameObjectManager.PlayMuzzle(m_Attacher.m_EntityID, m_Muzzle.position, spreadDirection, projectileData.I_MuzzleIndex, projectileData.AC_MuzzleClip);
    }

    protected virtual void FireProjectileTowards(Vector3 direction, int projectileIndex,SFXProjectile projectileData,DamageInfo damageInfo)=> FireOneProjectile(projectileIndex, projectileData, damageInfo, direction);

    protected void FireOneProjectile(int projectilIndex, SFXProjectile projectileData,DamageInfo damageInfo, Vector3 direction)
    {
        SFXProjectile projectile = GameObjectManager.SpawnSFXWeapon<SFXProjectile>(projectilIndex, m_Muzzle.position, direction);
        projectile.F_Speed = m_Attacher.m_CharacterInfo.F_Projectile_SpeedMuiltiply*projectileData.F_Speed;
        projectile.B_Penetrate = projectileData.B_Penetrate || m_Attacher.m_CharacterInfo.B_Projectile_Penetrate;
        projectile.Play(damageInfo, direction, m_Attacher.tf_Weapon.position + direction * GameConst.I_ProjectileMaxDistance);
    }
}
