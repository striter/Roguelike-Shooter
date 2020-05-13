using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectileBase : WeaponBase
{
    public float F_AimSpread;

    public override enum_PlayerWeaponType m_WeaponType => enum_PlayerWeaponType.ProjectileShot;
    public float GetAimSpread() => m_Attacher.m_CharacterInfo.F_AimSpreadMultiply * F_AimSpread;
    SFXProjectile m_ProjectileData;

    protected override void OnAutoTriggerSuccessful()
    {
        base.OnAutoTriggerSuccessful();
        OnTriggerOnce(m_BaseSFXWeaponIndex, GetWeaponDamageInfo(m_BaseDamage));
    }
    RaycastHit hit;
    protected void OnTriggerOnce(int projectileIndex,DamageInfo damageInfo)
    {
        Vector3 targetDirection =  m_Attacher.tf_WeaponAim.forward;
        float aimSpread = GetAimSpread();

        SFXProjectile targetProjectile = GameObjectManager.GetSFXWeaponData<SFXProjectile>(projectileIndex);
        FireProjectileTowards(targetDirection.RotateDirectionClockwise(Vector3.up, UnityEngine.Random.Range(-aimSpread, aimSpread)), projectileIndex, targetProjectile,damageInfo);
        GameObjectManager.PlayMuzzle(m_Attacher.m_EntityID, m_Muzzle.position, targetDirection, targetProjectile.I_MuzzleIndex, targetProjectile.AC_MuzzleClip);
    }

    protected virtual void FireProjectileTowards(Vector3 direction, int projectileIndex,SFXProjectile projectileData,DamageInfo damageInfo)=> FireOneProjectile(projectileIndex, projectileData, damageInfo, direction);

    protected void FireOneProjectile(int projectilIndex, SFXProjectile projectileData,DamageInfo damageInfo, Vector3 direction)
    {
        SFXProjectile projectile = GameObjectManager.SpawnSFXWeapon<SFXProjectile>(projectilIndex, m_Muzzle.position, direction);
        projectile.F_Speed = m_Attacher.m_CharacterInfo.F_Projectile_SpeedMuiltiply*projectileData.F_Speed;
        projectile.B_Penetrate = projectileData.B_Penetrate || m_Attacher.m_CharacterInfo.B_Projectile_Penetrate;
        projectile.Play(damageInfo, direction, m_Attacher.tf_Weapon.position + direction * GameConst.I_ProjectileInvalidDistance);
    }
}
