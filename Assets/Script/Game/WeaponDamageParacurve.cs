using System;
using GameSetting;
using UnityEngine;

public class WeaponDamageParacurve : WeaponDamageBase {

    SFXProjectile projectileData;
    public override enum_PlayerWeaponBaseType m_WeaponType => enum_PlayerWeaponBaseType.Paracurve;

    public override void OnPoolInit(enum_PlayerWeaponIdentity _identity, Action<enum_PlayerWeaponIdentity, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        projectileData = GameObjectManager.GetSFXWeaponData<SFXProjectile>(m_BaseSFXWeaponIndex);
    }
    protected override void OnAutoTrigger(float animDuration)
    {
        Vector3 direction = m_Muzzle.forward;
        SFXProjectile projectile = GameObjectManager.SpawnSFXWeapon<SFXProjectile>(m_BaseSFXWeaponIndex, m_Muzzle.position, m_Muzzle.forward);
        projectile.Play(GetWeaponDamageInfo(m_BaseDamage),  direction, m_Attacher.GetAimingPosition(false));
        GameObjectManager.PlayMuzzle(m_Attacher.m_EntityID, m_Muzzle.position, direction, projectileData.I_MuzzleIndex, projectileData.AC_MuzzleClip);
        OnAmmoCost();
        OnAttackAnim(animDuration);
    }
}
