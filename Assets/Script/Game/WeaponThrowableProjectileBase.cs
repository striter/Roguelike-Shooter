using System;
using GameSetting;
using UnityEngine;

public class WeaponThrowableProjectileBase : WeaponBase {

    SFXProjectile projectileData;
    public override enum_PlayerWeaponType m_WeaponType => enum_PlayerWeaponType.ThrowableProjectle;

    public override void OnPoolItemInit(enum_PlayerWeapon _identity, Action<enum_PlayerWeapon, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        projectileData = GameObjectManager.GetSFXWeaponData<SFXProjectile>(m_BaseSFXWeaponIndex);
    }
    protected override void OnAutoTriggerSuccessful()
    {
        base.OnAutoTriggerSuccessful();
        Vector3 direction = m_Muzzle.forward;
        SFXProjectile projectile = GameObjectManager.SpawnSFXWeapon<SFXProjectile>(m_BaseSFXWeaponIndex, m_Muzzle.position, m_Muzzle.forward);
        projectile.Play(GetWeaponDamageInfo(m_BaseDamage),  direction, m_Attacher.GetAimingPosition(false));
        GameObjectManager.PlayMuzzle(m_Attacher.m_EntityID, m_Muzzle.position, direction, projectileData.I_MuzzleIndex, projectileData.AC_MuzzleClip);
    }
}
