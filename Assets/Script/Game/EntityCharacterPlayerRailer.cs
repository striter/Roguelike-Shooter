using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerRailer : EntityCharacterPlayer {
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
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Railer;
    public override bool m_AbilityAvailable => m_ShotStorage > 0;
    public override float m_AbilityCooldownScale => m_ShotRestoreTimer.m_TimeLeftScale;
    WeaponHelperBase m_CharacterWeaponHelper;

    int m_MaxShotStorage => I_BaseShotStorage + m_CharacterInfo.m_RankManager.m_Rank / I_ExtraShotStorageRankStep;
    TimerBase m_ShotRestoreTimer;
    public int m_ShotStorage { get; private set; } = 0;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_ShotRestoreTimer = new TimerBase(F_ShotRestoreCoolDown,false);
    }
    DamageInfo GetAbilityDamageInfo() => new DamageInfo(m_EntityID, F_AbilityDamageBase + F_AbilityDamageExtraMultiply * m_CharacterInfo.m_DamageAdditive + m_CharacterInfo.m_RankManager.m_Rank * F_AbilityDamageRankMultiply, 0f, m_CharacterInfo.m_CriticalRate * F_AbilityCriticalRateMultiply, m_CharacterInfo.m_CriticalDamageMultiply, enum_DamageType.Basic, -1);
    protected override void OnEntityActivate(enum_EntityFlag flag, float startHealth = 0)
    {
        base.OnEntityActivate(flag, startHealth);
        m_ShotStorage = m_MaxShotStorage;
        m_CharacterWeaponHelper = WeaponHelperBase.AcquireWeaponHelper(I_ShotProjectileID,this,GetAbilityDamageInfo);
    }

    public override void OnAbilityDown(bool down)
    {
        base.OnAbilityDown(down);
        if (m_IsDead || !down||!m_AbilityAvailable)
            return;

        m_CharacterWeaponHelper.OnPlay(null, GetAimingPosition(true));
        if (m_ShotStorage == m_MaxShotStorage)
            m_ShotRestoreTimer.Replay();
        m_ShotStorage -=1;
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
