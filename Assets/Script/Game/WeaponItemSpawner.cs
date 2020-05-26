using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemSpawner : WeaponItemBase {
    public int m_SpawnIndex;
    public int m_SpawnParticleIndex;
    public float m_HealthMultiplierPerEnhance;
    public float m_DamageMultiplierPerEnhance;

    public float m_HealthMultiplier => m_HealthMultiplierPerEnhance * m_EnhanceLevel;
    public float m_DamageMultiplier => m_DamageMultiplierPerEnhance * m_EnhanceLevel;

    protected override void OnKeyAnim()
    {
        base.OnKeyAnim();
        Vector3 position = NavigationManager.NavMeshPosition(m_Attacher.transform.position + TCommon.RandomXZCircle() * 2f);
        GameObjectManager.SpawnParticles(m_SpawnParticleIndex,position,Vector3.up).PlayUncontrolled(m_Attacher.m_EntityID);
        GameObjectManager.SpawnGameCharcter(m_SpawnIndex,position,Quaternion.identity).OnCharacterBattleActivate(m_Attacher.m_Flag,GameDataManager.DefaultGameCharacterPerk(m_HealthMultiplier,m_DamageMultiplier),m_Attacher.m_EntityID);
    }
}
