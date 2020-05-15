using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemSpawner : WeaponItemBase {
    public int m_SpawnIndex;
    public float m_HealthMultiplierPerEnhance;
    public float m_DamageMultiplierPerEnhance;

    public float m_HealthMultiplier => m_HealthMultiplierPerEnhance * m_EnhanceLevel;
    public float m_DamageMultiplier => m_DamageMultiplierPerEnhance * m_EnhanceLevel;

    protected override void OnKeyAnim()
    {
        base.OnKeyAnim();
        GameObjectManager.SpawnGameCharcter(m_SpawnIndex,NavigationManager.NavMeshPosition( m_Attacher.transform.position+TCommon.RandomXZCircle()*2f),Quaternion.identity).OnCharacterGameActivate(m_Attacher.m_Flag,GameDataManager.DefaultGameCharacterPerk(m_HealthMultiplier,m_DamageMultiplier),m_Attacher.m_EntityID);
    }
}
