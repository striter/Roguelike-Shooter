using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityCharacterPlayerMachinist : EntityCharacterPlayer {
    #region Preset
    public float F_AbilityCoolDown = 0f;
    public float F_RollSpeedMultiple;
    public float F_RollDuration;
    public int P_RollFinishClipRestore;
    public int P_RollFinishFireRateExtraMultiply;
    public int P_RollFinishFireRateRankMultiply;
    public float F_RollFinishFireRateDuration;
    #endregion
    public override enum_PlayerCharacter m_Character => enum_PlayerCharacter.Beth;
    protected TimerBase m_RollTimer,m_RollCooldown;
    protected bool m_Rolling => m_RollTimer.m_Timing;
    Vector3 m_RollDirection;
    public override bool m_AbilityAvailable => !m_RollCooldown.m_Timing;
    public override float m_AbilityCooldownScale => m_RollCooldown.m_TimeLeftScale;
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_RollTimer = new TimerBase(F_RollDuration, true);
        m_RollCooldown = new TimerBase(F_AbilityCoolDown, true);
    }

    public override void OnAbilityDown(bool down)
    {
        base.OnAbilityDown(down);
        if (m_IsDead || !down || !m_AbilityAvailable)
            return;
        Vector3 position = NavigationManager.NavMeshPosition(transform.position + TCommon.RandomXZCircle() * 2f);
        GameObjectManager.SpawnParticles(m_SpawnParticleIndex, position, Vector3.up).PlayUncontrolled(m_EntityID);
        GameObjectManager.SpawnGameCharcter(m_SpawnIndex, position, Quaternion.identity).OnCharacterBattleActivate(m_Flag, GameDataManager.DefaultGameCharacterPerk(m_HealthMultiplier,m_DamageMultiplier), m_EntityID);
        //m_RollDirection = m_MoveAxisInput == Vector2.zero?transform.forward: base.CalculateMoveDirection(m_MoveAxisInput);
        //m_Animator.BeginRoll(F_RollDuration);
        m_RollTimer.Replay();
        m_RollCooldown.Replay();
        //EnableHitbox(false);
    }
    int m_SpawnIndex=601;
    public int m_SpawnParticleIndex=19205;
    public float m_HealthMultiplierPerEnhance=0.2f;
    public float m_DamageMultiplierPerEnhance=0.2f;

    public float m_HealthMultiplier => m_HealthMultiplierPerEnhance; //* m_EnhanceLevel;
    public float m_DamageMultiplier => m_DamageMultiplierPerEnhance;//* m_EnhanceLevel;

    protected override void OnAliveTick(float deltaTime)
    {
        m_RollCooldown.Tick(deltaTime);
        if (m_Rolling)
        {
            m_RollTimer.Tick(deltaTime);
            if (!m_Rolling)
            {
                m_WeaponCurrent.AddAmmo((int)(m_WeaponCurrent.m_ClipAmount*P_RollFinishClipRestore/100f));
                m_CharacterInfo.AddExpire(new EntityExpirePreset( m_EntityID,SBuff.CreateGameBethBuff(m_CharacterInfo.m_ExtraFireRateMultiply*P_RollFinishFireRateExtraMultiply/100f+m_CharacterInfo.m_RankManager.m_Rank*P_RollFinishFireRateRankMultiply/100f,F_RollFinishFireRateDuration)));
                m_Animator.EndRoll();
                m_CharacterInfo.OnAbilityTrigger();
                EnableHitbox(true);
            }
        }
        base.OnAliveTick(deltaTime);
    }
    protected override void OnDead()
    {
        m_RollTimer.Stop();
        m_Animator.EndRoll();
        base.OnDead();
    }

    protected override bool CheckWeaponFiring() => !m_Rolling&& base.CheckWeaponFiring();
    public override float GetBaseMovementSpeed()=> (m_Rolling ? F_RollSpeedMultiple : 1) * base.GetBaseMovementSpeed();
    protected override float CalculateMovementSpeedMultiple() => m_Rolling ? 1f : base.CalculateMovementSpeedMultiple();
    protected override Vector3 CalculateMoveDirection(Vector2 moveAxisInput) => m_Rolling ? m_RollDirection : base.CalculateMoveDirection(moveAxisInput);
    protected override Quaternion GetCharacterRotation() => m_Rolling ? Quaternion.LookRotation(m_RollDirection, Vector3.up) : base.GetCharacterRotation();
}
