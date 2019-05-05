using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour {

    public virtual enum_WeaponType E_WeaponType => enum_WeaponType.Invalid;
    public virtual enum_AmmoType E_AmmoType => enum_AmmoType.Invalid;
    public int F_Spread = 3;
    public float F_PerBulletDamage = 10;
    public float F_FireRate = .03f;
    public int I_MagCapacity = 7;
    public float F_ReloadTime = .6f;
    public int I_AmmoLeft { get; protected set; }
    protected Transform tf_Muzzle;
    protected Transform tf_ShellThrow;
    protected Action OnWeaponStatusChanged;
    protected Action<bool> OnWeaponHitEnermy;
    protected LivingBase m_attacher;
    protected virtual void Awake()
    {

    }
    
    RaycastHit rh_hitInfo;
    protected void FireAt(Vector3 start,Vector3 direction, float spreadMultiParam=1)
    {
        Vector2 spread = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(-F_Spread, F_Spread);       //Spread Circle 
        spread *= spreadMultiParam;
        Vector3 end = start +direction * GameSettings.CI_BulletMaxDistance;
        end += tf_Muzzle.right * spread.x + tf_Muzzle.up * spread.y;
        if (Physics.Raycast(start, end - start, out rh_hitInfo, GameSettings.CI_BulletMaxDistance, GameLayersPhysics.IL_WeaponHit))      //On Hit Items
        {
            HitCheckBase hitCheck = rh_hitInfo.collider.GetComponent<HitCheckBase>();
            enum_WeaponSFX impactEffect = enum_WeaponSFX.ImpactConcrete;     //Null Impact Effect
            Transform impactAttach = null;

            if (hitCheck != null)       //Dealt Damage Check If Need Attach To Object
            {
                if (hitCheck.E_Type == enum_checkObjectType.Living)     //BroadCast EnermyHit
                {
                    bool isAlreadyDead = (hitCheck as HitCheckLiving).m_Attacher.isDead;
                    hitCheck.OnHitCheck(F_PerBulletDamage, enum_DamageType.Range, end - start, m_attacher);
                    if (!isAlreadyDead)
                        OnWeaponHitEnermy?.Invoke((hitCheck as HitCheckLiving).m_Attacher.isDead);
                }
                else
                {
                    hitCheck.OnHitCheck(F_PerBulletDamage, enum_DamageType.Range, end - start, m_attacher);
                }

                impactEffect = hitCheck.E_ImpactEffectType;
                impactAttach = hitCheck.B_AttachImpactDecal ? hitCheck.transform : null;
            }

            SFXBase hitParticle = EntityManager.SpawnWeaponSFX<SFXBase>(impactEffect, impactAttach);       //Player Hit Particles
            hitParticle.transform.position = rh_hitInfo.point;
            hitParticle.transform.rotation = Quaternion.LookRotation(rh_hitInfo.normal);
            (hitParticle as SFXParticlesImpact).Play(hitCheck ? hitCheck.B_ShowImpactDecal : false);
            end = rh_hitInfo.point;
        }

        EntityManager.SpawnWeaponSFX<SFXBullet>(enum_WeaponSFX.Bullet, null).Play(tf_Muzzle.position, end, GameSettings.CI_BulletMaxDistance);       //Fly Bullets
    }

    protected void SpawnShell()
    {
        EntityManager.SpawnWeaponSFX<SFXShell>(E_AmmoType.ToBulletShell(), tf_ShellThrow).Play();
    }
    protected void SpawnMuzzle()
    {
        EntityManager.SpawnWeaponSFX<SFXMuzzleFlash>(E_WeaponType.ToWeaponMuzzle(), tf_Muzzle).Play(); //Spawn Muzzle
    }

    protected void Attach(LivingBase _attacher, PickupInfoWeapon _info, Action _OnWeaponStatusChanged, Action<bool> _OnWeaponHitEnermy)
    {
        m_attacher = _attacher;
        I_AmmoLeft = _info.I_ClipAmmo;
        OnWeaponStatusChanged = _OnWeaponStatusChanged;
        OnWeaponHitEnermy = _OnWeaponHitEnermy;
    }
    public virtual PickupInfoWeapon Detach()
    {
        EntityManager.RecycleWeaponBase(E_WeaponType, this);
        return new PickupInfoWeapon(this);
    }
}
