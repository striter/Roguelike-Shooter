using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class WeaponProjectileMultiShot : WeaponProjectileBase {

    public int I_PelletsPerShot=1;
    public float F_PelletSpreadAngle = 1;

    public float GetPelletSpread() => m_Attacher.m_CharacterInfo.F_SpreadMultiply * F_PelletSpreadAngle;
    public int GetPelletPerShot() => m_Attacher.m_CharacterInfo.I_Projectile_Multi_PelletsAdditive + I_PelletsPerShot;

    protected override void FireProjectileTowards(Vector3 direction, int projectileIndex, SFXProjectile projectileData, DamageInfo damageInfo)
    {
        float spreadAngle = GetPelletSpread();
        int pelletCount = GetPelletPerShot();
        
        float beginAnle = -spreadAngle * (pelletCount - 1) / 2f;
        for (int i = 0; i < pelletCount; i++)
            FireOneProjectile(projectileIndex, projectileData, damageInfo, direction.RotateDirectionClockwise(Vector3.up, beginAnle + i * spreadAngle));
    }
}
