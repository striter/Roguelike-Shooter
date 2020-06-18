using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerMachinist : EntityCharacterPlayer
{
    #region Preset
    public int I_ShotProjectileID;
    public float F_ShotRestoreCoolDown;
    public int I_BaseShotStorage;
    public int I_ExtraShotStorageRankStep;
    public float F_AbilityDamageBase;
    public float F_AbilityDamageExtraMultiply;
    public float F_AbilityDamageRankMultiply;
    public float F_AbilityCriticalRateMultiply;
    #endregion
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Machinist;
    public override bool m_AbilityAvailable => m_ShotStorage > 0;
    public override float m_AbilityCooldownScale => m_ShotRestoreTimer.m_TimeLeftScale;
    CharacterWeaponHelperBase m_CharacterWeaponHelper;

    int m_MaxShotStorage => I_BaseShotStorage + m_CharacterInfo.m_RankManager.m_Rank / I_ExtraShotStorageRankStep;
    TimerBase m_ShotRestoreTimer;
    public int m_ShotStorage { get; private set; } = 0;
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_ShotRestoreTimer = new TimerBase(F_ShotRestoreCoolDown, false);
    }
    protected override void OnEntityActivate(enum_EntityFlag flag)
    {
        base.OnEntityActivate(flag);
        m_ShotStorage = m_MaxShotStorage;
        m_CharacterWeaponHelper = CharacterWeaponHelperBase.AcquireCharacterWeaponHelper(I_ShotProjectileID, this, 0);
        if(m_Enhance>= enum_PlayerCharacterEnhance.Critical)
        {
            I_BaseShotStorage = 2;
        }
    }
    public int m_SpawnIndex;
    public int m_SpawnParticleIndex;
    public float m_HealthMultiplierPerEnhance;
    public float m_DamageMultiplierPerEnhance;

    public float m_HealthMultiplier => m_HealthMultiplierPerEnhance* (int)m_Enhance;
    public float m_DamageMultiplier => m_DamageMultiplierPerEnhance* (int)m_Enhance;
    public override void OnAbilityDown(bool down)
    {
        base.OnAbilityDown(down);
        if (m_IsDead || !down || !m_AbilityAvailable)
            return;

        Vector3 position = NavigationManager.NavMeshPosition(transform.position + TCommon.RandomXZCircle() * 2f);        GameObjectManager.SpawnParticles(m_SpawnParticleIndex, position, Vector3.up).PlayUncontrolled(m_EntityID);        GameObjectManager.SpawnGameCharcter(m_SpawnIndex, position, Quaternion.identity).OnCharacterBattleActivate(m_Flag, GameDataManager.DefaultGameCharacterPerk(m_HealthMultiplier, m_DamageMultiplier), m_EntityID);
        if (m_ShotStorage == m_MaxShotStorage)
            m_ShotRestoreTimer.Replay();
        m_ShotStorage -= 1;
        m_CharacterInfo.OnAbilityTrigger();
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (m_ShotStorage >= m_MaxShotStorage)
            return;
        m_ShotRestoreTimer.Tick(deltaTime);
        if (m_ShotRestoreTimer.m_Timing)
            return;
        m_ShotStorage += 1;
        m_ShotRestoreTimer.Replay();
    }

    protected override void OnDead()
    {
        base.OnDead();
        m_ShotStorage = m_MaxShotStorage;
        m_ShotRestoreTimer.Replay();
    }
}
