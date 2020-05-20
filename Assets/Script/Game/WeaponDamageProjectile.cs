using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageProjectile : WeaponDamageBase
{
    public float F_AimSpread;

    public bool B_MultiShotAvailable = false;
    public int I_PelletsPerShot = 1;
    public float F_PelletSpreadAngle = 1;

    public override enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Projectile;
    public float GetAimSpread() => m_Attacher.m_CharacterInfo.F_AimSpreadMultiply * F_AimSpread;
    public float GetPelletSpread() => m_Attacher.m_CharacterInfo.F_AimSpreadMultiply * F_PelletSpreadAngle;
    public int GetPelletPerShot() =>B_MultiShotAvailable?( m_Attacher.m_CharacterInfo.I_Projectile_Multi_PelletsAdditive + I_PelletsPerShot):1;

    protected override void OnAutoTrigger(float animDuration)=> FireProjectile(false, m_BaseSFXWeaponIndex,animDuration);
    protected override void OnStoreTrigger(float animDuration, float storeTimeLeft) => FireProjectile(storeTimeLeft==0,m_BaseSFXWeaponIndex,animDuration);

    protected void FireProjectile(bool store, int projectileIndex, float animDuration,int multiShotAdditive=1) => FireProjectile(projectileIndex, animDuration, GetWeaponDamageInfo(store), GetPelletPerShot()+multiShotAdditive);
    void FireProjectile( int projectileIndex, float animDuration, DamageInfo damageInfo,int shotCount)
    {
        Vector3 baseSpreadDirection = Vector3.Normalize(  m_Attacher.GetAimingPosition(true)- m_Muzzle.position);
        baseSpreadDirection.y = 0;

        SFXProjectile targetProjectile = GameObjectManager.GetSFXWeaponData<SFXProjectile>(projectileIndex);
        GameObjectManager.PlayMuzzle(m_Attacher.m_EntityID, m_Muzzle.position, baseSpreadDirection, targetProjectile.I_MuzzleIndex, targetProjectile.AC_MuzzleClip);

        float aimSpread = GetAimSpread();
        baseSpreadDirection = baseSpreadDirection.RotateDirectionClockwise(Vector3.up, UnityEngine.Random.Range(-aimSpread,aimSpread));
        if (shotCount>1)
        {
            float spreadAngle = GetPelletSpread();
            float beginAnle = -spreadAngle * (shotCount - 1) / 2f;
            for (int i = 0; i < shotCount; i++)
                FireOneProjectile(projectileIndex, targetProjectile, damageInfo, baseSpreadDirection.RotateDirectionClockwise(Vector3.up, beginAnle + i * spreadAngle));
        }
        else
        {
            FireOneProjectile(projectileIndex, targetProjectile, damageInfo, baseSpreadDirection);
        }
        OnAttackAnim(animDuration);
        OnAmmoCost();
    }

    protected void FireOneProjectile(int projectilIndex, SFXProjectile projectileData, DamageInfo damageInfo, Vector3 direction)
    {
        SFXProjectile projectile = GameObjectManager.SpawnSFXWeapon<SFXProjectile>(projectilIndex, m_Muzzle.position, direction);
        projectile.F_Speed = m_Attacher.m_CharacterInfo.F_Projectile_SpeedMuiltiply * projectileData.F_Speed;
        projectile.B_Penetrate = projectileData.B_Penetrate || m_Attacher.m_CharacterInfo.B_Projectile_Penetrate;
        projectile.Play(damageInfo, direction, m_Attacher.tf_Weapon.position + direction * GameConst.I_ProjectileInvalidDistance);
    }

    protected override void OnKeyAnim() {
        //Do Nothing;
    }

}
