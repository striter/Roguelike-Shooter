﻿using GameSetting;
using UnityEngine;
using System;
namespace GameSetting_Action
{
    #region Desingers Data
    public static class EquipmentConsts
    {
        public static int P_0001_ReloadSpeedAdditive(enum_EquipmentLevel rarity) => 35 * (int)rarity;
        public static int P_0002_ClipRefillRate(enum_EquipmentLevel rarity) => 3 * (int)rarity;
        public static int I_0003_ClipAdditive(enum_EquipmentLevel rarity) => 2 * (int)rarity;
        public static int P_0004_ClipMultiply(enum_EquipmentLevel rarity) => 50 * (int)rarity;
        public static float F_0005_DamageAdditive(enum_EquipmentLevel rarity) => 6.6f * (int)rarity;
        public static float F_0006_DamageAdditive(enum_EquipmentLevel rarity) => 8.6f * (int)rarity;
        public static float F_0006_Duration(enum_EquipmentLevel rarity) => 3*(int)rarity;
        public static int P_0007_SpreadReduction(enum_EquipmentLevel rarity) => 30 * (int)rarity;
        public static float F_0008_AimRangeIncrease(enum_EquipmentLevel rarity) => 1f * (int)rarity;
        public static int P_0009_DamageMultiply(enum_EquipmentLevel rarity) => 100 * (int)rarity;
        public static int P_0010_PenetrateAdditive(enum_EquipmentLevel rarity) => 20 * (int)rarity;
        public static int P_0011_FireRateAdditive(enum_EquipmentLevel rarity) => 40 * (int)rarity;
        public static int I_0012_BounceTimes(enum_EquipmentLevel rarity) => 1*(int)rarity;

        public static float F_0013_EffectRange(enum_EquipmentLevel rarity) => 8f;
        public static int P_0013_DamageMultiply(enum_EquipmentLevel rarity) => 20 * (int)rarity;
        public static float F_0014_EffectRange(enum_EquipmentLevel rarity) => 8f;
        public static int F_0014_ReductionDuration(enum_EquipmentLevel rarity) => 1 * (int)rarity;
        public static float F_0015_EffectRange(enum_EquipmentLevel rarity) => 8f;
        public static int P_0015_MovementSpeedEachEnermy(enum_EquipmentLevel rarity,int nearbyCount) => nearbyCount* 10 * (int)rarity;
        public static float F_0016_EffectRange(enum_EquipmentLevel rarity) => 8f*(int)rarity;
        public static float P_0016_ClipRefillRate(enum_EquipmentLevel rarity) => 30f;
        public static float F_0017_Duration(enum_EquipmentLevel rarity) => 1 * (int)rarity;

        public static float P_0018_MovementSpeedMultiply(enum_EquipmentLevel rarity) => 60f * (int)rarity;
        public static float F_0018_Duration(enum_EquipmentLevel rarity) => 5f;
        public static float P_0019_MovementSpeedMultiply(enum_EquipmentLevel rarity) => 40f * (int)rarity;
        public static float F_0019_Duration(enum_EquipmentLevel rarity) => 5f;
        public static float P_0020_MovementSpeedAdditive(enum_EquipmentLevel rarity) => 20 * (int)rarity;
        public static float P_0021_AimPressureReductionDecrease(enum_EquipmentLevel rarity) => 30 * (int)rarity;
        public static float P_0022_DamageMultiply(enum_EquipmentLevel rarity,float stack) => stack*3* (int)rarity;
        public const int P_0022_MaxStack = 40;
        
        public static int P_0023_MaxHealthRegen(enum_EquipmentLevel rarity) => 15 * (int)rarity;
        public static int P_0024_HealthRegenEachKill(enum_EquipmentLevel rarity) => 6 * (int)rarity;
        public static float F_0025_MaxHealthAdd(enum_EquipmentLevel rarity) => 60 * (int)rarity;
        public static int P_0026_HealthRegenAdditive(enum_EquipmentLevel rarity) => 100 * (int)rarity;
        public static int P_0027_HealthDrainAdditive(enum_EquipmentLevel rarity) => 1 * (int)rarity;
        
        public static float F_0028_FreezeDuration(enum_EquipmentLevel rarity) => 3 * (int)rarity;
        public static float F_0029_FreezeDurationPer10Damage(enum_EquipmentLevel rarity) => 0.02f * (int)rarity;
        public static float F_0030_OffsetDuration(enum_EquipmentLevel rarity) => 5-(int)rarity;
        public static float F_0030_FreezeDuration(enum_EquipmentLevel rarity) => 1f;
        public static float F_0031_Duration(enum_EquipmentLevel rarity) =>5- (int)rarity;
        public static float F_0031_FreezeDuration(enum_EquipmentLevel rarity) => 2 * (int)rarity;
        public static float P_0032_FreezeDurationEnhance(enum_EquipmentLevel rarity) =>100*(int)rarity;

        public static float P_0033_CoinsDropAdditive(enum_EquipmentLevel rarity) => 10 * (int)rarity;
        public static float P_0034_CoinsCostDecrease(enum_EquipmentLevel rarity) => 10 * (int)rarity;
        public static float P_0035_CoinsDoubleRate(enum_EquipmentLevel rarity) => 10 * (int)rarity;
        public static float P_0036_CoinsReviveAmount(enum_EquipmentLevel rarity) => 50 - 10 * ((int)rarity );
        public static float F_0037_DamageMultiplyPer10Coins(enum_EquipmentLevel rarity) => 4 * (int)rarity;
    }

    public class ActionBuffConsts
    {
        public static RangeInt RI_10001_StartArmor = new RangeInt(20, 5);
        public const float P_10002_DamageMultiply = 5;
        public static RangeInt RI_10003_StartHealth = new RangeInt(40, 5);
    }
    #endregion

    #region Developers Use
    public static class EquipmentHelper
    {
        public static WeaponHelperBase GetCommonDevice(int actionIndex,EntityCharacterPlayer player, Func<DamageDeliverInfo> damageInfo) => WeaponHelperBase.AcquireWeaponHelper(GameExpression.GetPlayerWeaponIndex(actionIndex), player, damageInfo);
        public static WeaponHelperEntitySpawner GetEntityDevice(int actionIndex, EntityCharacterPlayer player,  Func<DamageDeliverInfo> damage, int health, float fireRate)
        {
            WeaponHelperEntitySpawner equipment = GetCommonDevice(actionIndex,player, damage) as WeaponHelperEntitySpawner;
            equipment.SetOnSpawn(health, (EntityCharacterBase entity) =>
            {
                EntityCharacterAI target = entity as EntityCharacterAI;
                target.F_AttackDuration = new RangeFloat(0f, 0);
                target.F_AttackTimes = new RangeInt(1, 0);
                target.F_AttackRate = fireRate;
            });
            return equipment;
        }
        public static WeaponHelperEntitySpawner GetBuffDevice(int actionIndex, EntityCharacterPlayer player,float health,SBuff buffApplyPlayer,  SBuff buffApplyAlly,float refreshDuration)
        {
            WeaponHelperEntitySpawner equipment = GetCommonDevice(actionIndex, player, null) as WeaponHelperEntitySpawner;
            equipment.SetOnSpawn(health,(EntityCharacterBase entity)=>{(entity as EntityDeviceBuffApllier).SetBuffApply(buffApplyPlayer,buffApplyAlly,refreshDuration);});
            return equipment;
        }
        public static void PlayerDealtDamageToEntity(EntityCharacterPlayer player, int targetID, float damageAmount, enum_DamageType damageType = enum_DamageType.Basic)
        {
            if (damageAmount < 0)
                Debug.LogError("Howd Fk Damage Below Zero?");
            if(GameManager.Instance.EntityExists(targetID))
                GameManager.Instance.GetEntity(targetID).m_HitCheck.TryHit(new DamageInfo(damageAmount, damageType, DamageDeliverInfo.Default(player.m_EntityID)));
        }
        public static void ReceiveDamage(EntityCharacterPlayer player, float damage, enum_DamageType type = enum_DamageType.Basic)
        {
            if (damage < 0)
                Debug.LogError("???????????");
            player.m_HitCheck.TryHit(new DamageInfo(damage, type,DamageDeliverInfo.Default(player.m_EntityID)));
        }

        public static void ReciveBuff(EntityCharacterPlayer player, SBuff buff)
        {
            player.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.BuffInfo(player.m_EntityID, buff)));
        }
        public static void ReceiveHealing(EntityCharacterPlayer entity, float heal, enum_DamageType type = enum_DamageType.Basic)
        {
            if (heal <= 0)
                Debug.LogError("Howd Fk Healing Below Zero?");
            entity.m_HitCheck.TryHit(new DamageInfo(-heal, type, DamageDeliverInfo.Default(entity.m_EntityID)));
        }
        public static void ReceiveEffect(EntityCharacterPlayer entity, enum_CharacterEffect effect, float duration)
        {
            entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.EquipmentInfo(entity.m_EntityID, 0, effect, duration)));
        }

    }

    public class EquipmentDevice :EquipmentBase
    {
        WeaponHelperBase m_Equipment;
        protected virtual DamageDeliverInfo GetDamageInfo() => null;
        public override void OnActivate(EntityCharacterPlayer _actionEntity, Action<EntityExpireBase> OnExpired)
        {
            base.OnActivate(_actionEntity, OnExpired);
            m_Equipment = WeaponHelperBase.AcquireWeaponHelper(GameExpression.GetPlayerEquipmentWeaponIndex(m_Index), m_Attacher, GetDamageInfo);
        }
        protected virtual void PlayDevice(EntityCharacterBase target)
        {
            m_Equipment.OnPlay(false,target);
        }
        public EquipmentDevice() { }
    }
    public class EquipmentStackupCounter
    {
        public EquipmentStackupCounter() { m_maxStackup = -1; }
        public EquipmentStackupCounter(float _maxStackup = -1) { m_maxStackup = _maxStackup; m_stack = 0; }
        public float m_stack { get; private set; }
        public float m_maxStackup { get; private set; }
        public void ResetStack() => m_stack = 0;
        public void SetStackup(int stackAmount) => m_stack = stackAmount;
        public void OnStackUp(float stackAmount)
        {
            m_stack += stackAmount;
            if (m_maxStackup > 0 && m_stack > m_maxStackup)
                m_stack = m_maxStackup;
        }
    }
    #region Equipment
    #region TypeA
    public class E0001_ReloadSpeedAdditive: EquipmentBase
    {
        public override int m_Index => 0001;
        public override float Value1 => EquipmentConsts.P_0001_ReloadSpeedAdditive(m_rarity);
        public override float m_ReloadRateMultiply => Value1 / 100f;
        public E0001_ReloadSpeedAdditive() { }
    }

    public class E0002_ClipRefillOnAttack:EquipmentBase
    {
        public override int m_Index => 0002;
        public override float Value1 => EquipmentConsts.P_0002_ClipRefillRate(m_rarity);
        public override void OnAttackDamageSet(DamageDeliverInfo info)
        {
            base.OnAttackDamageSet(info);
            if (TCommon.RandomPercentage() < Value1)
                m_Attacher.m_WeaponCurrent.ForceReload();
        }
        public E0002_ClipRefillOnAttack() { }
    }

    public class E0003_ClipAdditive : EquipmentBase
    {
        public override int m_Index => 0003;
        public override float Value1 => EquipmentConsts.I_0003_ClipAdditive(m_rarity);
        public override int I_ClipAdditive => (int)Value1;
        public E0003_ClipAdditive() { }
    }

    public class E0004_ClipMultiply : EquipmentBase
    {
        public override int m_Index => 0004;
        public override float Value1 => EquipmentConsts.P_0004_ClipMultiply(m_rarity);
        public override float F_ClipMultiply => Value1/100f;
        public E0004_ClipMultiply() { }
    }

    public class E0005_ClipMultiply : EquipmentBase
    {
        public override int m_Index => 0005;
        public override float Value1 => EquipmentConsts.F_0005_DamageAdditive(m_rarity);
        public override float m_DamageAdditive => Value1;
        public E0005_ClipMultiply() { }
    }

    //public class E0006_DamageAdditiveAfterReload:EquipmentBase
    //{
    //    public override int m_Index => 0006;
    //    public override float Value1 => EquipmentConsts.F_0006_DamageAdditive(m_rarity);
    //    public override float Value2 => EquipmentConsts.F_0006_Duration(m_rarity);
    //    public override float m_DamageAdditive => m_Timer.m_Timing?Value1:base.m_DamageAdditive;
    //    TimeCounter m_Timer=new TimeCounter();
    //    public override void OnReloadFinish()
    //    {
    //        base.OnReloadFinish();
    //        m_Timer.SetTimer(Value2);
    //    }
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        m_Timer.Tick(deltaTime);
    //    }
    //    public E0006_DamageAdditiveAfterReload() { }
    //}

    public class E0007_SpreadReduction:EquipmentBase
    {
        public override int m_Index => 0007;
        public override float Value1 => EquipmentConsts.P_0007_SpreadReduction(m_rarity)/100f;
        public override float F_SpreadReduction => Value1;
        public E0007_SpreadReduction() { }
    }

    //public class E0008_AimRangeIncrease : EquipmentBase
    //{
    //    public override int m_Index => 0008;
    //    public override float Value1 => EquipmentConsts.F_0008_AimRangeIncrease(m_rarity);
    //    public override float F_AimRangeAdditive => Value1;
    //    public E0008_AimRangeIncrease() { }
    //}

    public class E0009_DamageBurstAfterKill : EquipmentBase
    {
        public override int m_Index => 0009;
        public override float Value1 => EquipmentConsts.P_0009_DamageMultiply(m_rarity);
        public override float m_DamageMultiply => m_burstShot ? Value1 / 100f : 0f;
        bool m_burstShot = false;
        public override void OnAttackDamageSet(DamageDeliverInfo info)
        {
            base.OnAttackDamageSet(info);
            m_burstShot = false;
        }

        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            m_burstShot = receiver.m_IsDead;
        }
        public E0009_DamageBurstAfterKill() { }
    }

    public class E0010_PenetrateAdditive:EquipmentBase
    {
        public override int m_Index => 0010;
        public override float Value1 => EquipmentConsts.P_0010_PenetrateAdditive(m_rarity);
        public override float F_PenetradeAdditive => Value1/100f;
        public E0010_PenetrateAdditive() { }
    }

    public class E0011_FireRateAdditive:EquipmentBase
    {
        public override int m_Index => 0011;
        public override float Value1 => EquipmentConsts.P_0011_FireRateAdditive(m_rarity);
        public override float m_FireRateMultiply => Value1 / 100f;
        public E0011_FireRateAdditive() { }
    }

    public class E0012_ProjectileCopy:EquipmentBase
    {
        public override int m_Index => 0012;
        public override float Value1 => EquipmentConsts.I_0012_BounceTimes(m_rarity);
        public override int I_ProjectileCopyAdditive => (int)Value1;
        public E0012_ProjectileCopy() { }
    }
    #endregion
    #region TypeB
    //public class E0013_NearbyDamageMultiply:EquipmentBase
    //{
    //    public override int m_Index => 0013;
    //    public override float Value1 => EquipmentConsts.F_0013_EffectRange(m_rarity); 
    //    public override float Value2 => EquipmentConsts.P_0013_DamageMultiply(m_rarity);
    //    public override void OnBeforeDealtDamageBegin(EntityCharacterBase receiver, DamageInfo info)
    //    {
    //        base.OnBeforeDealtDamageBegin(receiver, info);
    //        if (Vector3.Distance(receiver.transform.position, m_Attacher.transform.position) > Value1)
    //            return;
    //        info.m_detail.DamageAdditive(Value2 / 100f, 0);
    //    }
    //    public E0013_NearbyDamageMultiply() { }
    //}

    public class E0014_NearbyKillDamageReduction:EquipmentBase
    {
        public override int m_Index => 0014;
        public override float Value1 => EquipmentConsts.F_0014_EffectRange(m_rarity);
        public override float Value2 => EquipmentConsts.F_0014_ReductionDuration(m_rarity);
        public override float m_DamageReduction => m_Timer.m_Timing ? 1f : base.m_DamageReduction;
        TimeCounter m_Timer = new TimeCounter();
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (receiver.m_IsDead&&Vector3.Distance(m_Attacher.transform.position,receiver.transform.position)<Value1)
                m_Timer.SetTimer(Value2);
        }
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
        }
        public E0014_NearbyKillDamageReduction() { }
    }
    public class E0015_NearbyCountMovementAdditive : EquipmentBase
    {
        public override int m_Index => 0015;
        public override float Value1 => EquipmentConsts.F_0015_EffectRange(m_rarity);
        public override float Value2 => EquipmentConsts.P_0015_MovementSpeedEachEnermy(m_rarity,m_NearbyCount);
        public override float m_MovementSpeedMultiply =>Value2/100f;
        int m_NearbyCount = 0;
        TimeCounter m_Timer = new TimeCounter();
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (receiver.m_IsDead && Vector3.Distance(m_Attacher.transform.position, receiver.transform.position) < Value1)
                m_Timer.SetTimer(Value2);
        }
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
            if (m_Timer.m_Timing)
                return;
            m_Timer.SetTimer(.5f);
            m_NearbyCount = GameManager.Instance.GetNearbyEnermyCount(m_Attacher,Value1);
        }
        public E0015_NearbyCountMovementAdditive() { }
    }

    public class E0016_NearbyKillClipAdditive : EquipmentBase
    {
        public override int m_Index => 0016;
        public override float Value1 => EquipmentConsts.F_0016_EffectRange(m_rarity);
        public override float Value2 => EquipmentConsts.P_0016_ClipRefillRate(m_rarity);
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (receiver.m_IsDead && Vector3.Distance(m_Attacher.transform.position, receiver.transform.position) < Value1&&TCommon.RandomPercentage()<=Value2)
                m_Attacher.m_WeaponCurrent.ForceReload();
        }
        public E0016_NearbyKillClipAdditive() { }
    }

    public class E0017_TakeDamageReductionDuration : EquipmentBase
    {
        public override int m_Index => 0017;
        public override float Value1 => EquipmentConsts.F_0017_Duration(m_rarity);
        public override float m_DamageReduction => m_Timer.m_Timing ? 1 : base.m_DamageReduction;
        TimeCounter m_Timer = new TimeCounter();
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
        }
        public override void OnBeforeReceiveDamage(DamageInfo info)
        {
            base.OnBeforeReceiveDamage(info);
            if (m_Timer.m_Timing)
                return;
            m_Timer.SetTimer(Value1);
        }
        public E0017_TakeDamageReductionDuration() { }
    }
    #endregion
    #region TypeC
    //public class E0018_ReceiveDamageMovementMultiplyDuration : EquipmentBase
    //{
    //    public override int m_Index => 0018;
    //    public override int m_EffectIndex => m_Timer.m_Timing ? 40005 : 0;
    //    public override float Value1 => EquipmentConsts.P_0018_MovementSpeedMultiply(m_rarity);
    //    public override float Value2 => EquipmentConsts.F_0018_Duration(m_rarity);
    //    public override float m_MovementSpeedMultiply => m_Timer.m_Timing? Value1 / 100f : 0;
    //    TimeCounter m_Timer = new TimeCounter();
    //    public override void OnReceiveDamage(DamageInfo info, float amount)
    //    {
    //        base.OnReceiveDamage(info, amount);
    //        m_Timer.SetTimer(Value2);
    //    }
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        m_Timer.Tick(deltaTime);
    //    }
    //    public E0018_ReceiveDamageMovementMultiplyDuration() { }
    //}

    //public class E0019_KillMovementMultiplyDuration : EquipmentBase
    //{
    //    public override int m_Index => 0019;
    //    public override int m_EffectIndex => m_Timer.m_Timing ? 40005 : 0;
    //    public override float Value1 => EquipmentConsts.P_0019_MovementSpeedMultiply(m_rarity);
    //    public override float Value2 => EquipmentConsts.F_0019_Duration(m_rarity);
    //    public override float m_MovementSpeedMultiply => m_Timer.m_Timing ? Value1 / 100f : 0;
    //    TimeCounter m_Timer = new TimeCounter();
    //    public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
    //    {
    //        base.OnDealtDamage(receiver, info, applyAmount);
    //        if (receiver.m_IsDead)
    //            m_Timer.SetTimer(Value2);
    //    }
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        m_Timer.Tick(deltaTime);
    //    }
    //    public E0019_KillMovementMultiplyDuration() { }
    //}

    //public class E0020_MovementSpeedMultiply : EquipmentBase
    //{
    //    public override int m_Index => 0020;
    //    public override float Value1 => EquipmentConsts.P_0020_MovementSpeedAdditive(m_rarity);
    //    public override float m_MovementSpeedMultiply => Value1 / 100f;
    //    public E0020_MovementSpeedMultiply() { }
    //}

    //public class E0021_AimPressureReduction:EquipmentBase
    //{
    //    public override int m_Index => 0021;
    //    public override float Value1 => EquipmentConsts.P_0021_AimPressureReductionDecrease(m_rarity);
    //    public override float F_AimPressureReduction => Value1 / 100f;
    //    public E0021_AimPressureReduction() { }
    //}

    //public class E0022_DamageMovementStackup : EquipmentBase
    //{
    //    public override int m_Index => 0022;
    //    public override float Value1 => EquipmentConsts.P_0022_DamageMultiply(m_rarity,m_stackUp.m_stack);
    //    public override float Value2 => EquipmentConsts.P_0022_DamageMultiply(m_rarity, EquipmentConsts.P_0022_MaxStack);
    //    public override float m_DamageMultiply => Value1/100f;
    //    EquipmentStackupCounter m_stackUp=new EquipmentStackupCounter(EquipmentConsts.P_0022_MaxStack);
    //    public override void OnAttackDamageSet(DamageDeliverInfo info)
    //    {
    //        base.OnAttackDamageSet(info);
    //        m_stackUp.ResetStack();
    //    }
    //    public override void OnMove(float distance) => m_stackUp.OnStackUp(distance);
    //    public E0022_DamageMovementStackup() { }
    //}
    #endregion
    #region TypeD
    //public class E0023_HealAddMaxHealth : EquipmentBase
    //{
    //    public override int m_Index => 0023;
    //    public override float Value1 => EquipmentConsts.P_0023_MaxHealthRegen(m_rarity);
    //    public override float m_MaxHealthAdditive => m_RecordData;
    //    public override void OnReceiveHealing(DamageInfo info, float amount)
    //    {
    //        base.OnReceiveHealing(info, amount);
    //        m_RecordData += -info.m_AmountApply * Value1 / 100f;
    //    }
    //    public E0023_HealAddMaxHealth() { }
    //}

    public class E0024_KillHealthRegen : EquipmentBase
    {
        public override int m_Index => 0024;
        public override float Value1 => EquipmentConsts.P_0024_HealthRegenEachKill(m_rarity);
        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (receiver.m_IsDead)
                EquipmentHelper.ReceiveHealing(m_Attacher, m_Attacher.m_Health.m_MaxHealth * Value1 / 100f, enum_DamageType.HealthOnly);
        }
        public E0024_KillHealthRegen() { }
    }

    public class E0025_MaxHealthAdditive : EquipmentBase
    {
        public override int m_Index => 0025;
        public override float Value1 => EquipmentConsts.F_0025_MaxHealthAdd(m_rarity);
        public override float m_MaxHealthAdditive => Value1;
        public E0025_MaxHealthAdditive() { }
    }

    public class E0026_HealRegenAdditive : EquipmentBase
    {
        public override int m_Index => 0026;
        public override float Value1 => EquipmentConsts.P_0026_HealthRegenAdditive(m_rarity);
        public override float m_HealAdditive => Value1 / 100f;
        public E0026_HealRegenAdditive() { }
    }

    public class E0027_MaxHealthAdditive : EquipmentBase
    {
        public override int m_Index => 0027;
        public override float Value1 => EquipmentConsts.P_0027_HealthDrainAdditive(m_rarity);
        public override float m_HealthDrainMultiply => Value1/100f;
        public E0027_MaxHealthAdditive() { }
    }
    #endregion
    #region TypeE    
    public class E0028_DamageKillFreezeBlast : EquipmentDevice
    {
        public override int m_Index => 0028;
        public override float Value1 => EquipmentConsts.F_0028_FreezeDuration(m_rarity);
        protected override DamageDeliverInfo GetDamageInfo() => DamageDeliverInfo.EquipmentInfo(m_Attacher.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1);

        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (!receiver.m_IsDead)
                return;

            PlayDevice(receiver);
        }

        public E0028_DamageKillFreezeBlast() { }
    }

    public class E0029_DamageFreeze : EquipmentBase
    {
        public override int m_Index => 0029;
        public override int m_EffectIndex => 41001;
        public override float Value1 => EquipmentConsts.F_0029_FreezeDurationPer10Damage(m_rarity);
        public override void OnBeforeDealtDamageMiddle(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamageMiddle(receiver, info);
            info.m_detail.EffectAdditiveOverride(enum_CharacterEffect.Freeze, info.m_AmountApply / 10f*Value1);
        }
        public E0029_DamageFreeze() { }
    }

    //public class E0030_DamageFreezeOffsetDuration : EquipmentBase
    //{
    //    public override int m_Index => 0030;
    //    public override int m_EffectIndex => !m_Timer.m_Timing? 41001:base.m_EffectIndex;
    //    public override float Value1 => EquipmentConsts.F_0030_OffsetDuration(m_rarity);
    //    public override float Value2 => EquipmentConsts.F_0030_FreezeDuration(m_rarity);
    //    TimeCounter m_Timer = new TimeCounter();
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        m_Timer.Tick(deltaTime);
    //    }
    //    public override void OnAttackDamageSet(DamageDeliverInfo info)
    //    {
    //        base.OnAttackDamageSet(info);
    //        if (!m_Timer.m_Timing)
    //        {
    //            info.EffectAdditiveOverride(enum_CharacterEffect.Freeze, Value2);
    //            m_Timer.SetTimer(Value1);
    //        }
    //    }
    //    public E0030_DamageFreezeOffsetDuration() { }
    //}

    public class E0031_FreezeMineSpawner:EquipmentDevice
    {
        public override int m_Index => 0031;
        public override float Value1 => EquipmentConsts.F_0031_Duration(m_rarity);
        public override float Value2 => EquipmentConsts.F_0031_FreezeDuration(m_rarity);
        protected override DamageDeliverInfo GetDamageInfo() => DamageDeliverInfo.EquipmentInfo(m_Attacher.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1);
        TimeCounter m_Timer = new TimeCounter();
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            m_Timer.Tick(deltaTime);
            if (!m_Timer.m_Timing)
            {
                m_Timer.SetTimer(Value2);
                PlayDevice(m_Attacher);
            }
        }
        public E0031_FreezeMineSpawner() { }
    }

    public class E0032_FreezeDuration: EquipmentBase
    {
        public override int m_Index => 0032;
        public override float Value1 => EquipmentConsts.P_0032_FreezeDurationEnhance(m_rarity);
        public override void OnBeforeDealtDamageFinal(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnBeforeDealtDamageFinal(receiver, info);
            if (info.m_detail.m_DamageEffect == enum_CharacterEffect.Freeze)
                info.m_detail.EffectAdditiveOverride( enum_CharacterEffect.Freeze,info.m_detail.m_EffectDuration*Value1/100f);
        }
        public E0032_FreezeDuration() { }
    }
    #endregion
    #region TypeF
    public class E0033_CoinsDropAdditive : EquipmentBase
    {
        public override int m_Index => 0033;
        public override float Value1 => EquipmentConsts.P_0033_CoinsDropAdditive(m_rarity);
        public override float P_CoinsDropAdditive => Value1;
        public E0033_CoinsDropAdditive() { }
    }


    public class E0034_CoinsDropAdditive : EquipmentBase
    {
        public override int m_Index => 0034;
        public override float Value1 => EquipmentConsts.P_0034_CoinsCostDecrease(m_rarity)/100f;
        public override float F_CoinsCostDecrease => Value1;
        public E0034_CoinsDropAdditive() { }
    }

    public class E0035_CoinsGainDouble : EquipmentBase
    {
        public override int m_Index => 0035;
        public override float Value1 => EquipmentConsts.P_0035_CoinsDoubleRate(m_rarity);
        public override void OnGainCoins(float coinAmount)
        {
            base.OnGainCoins(coinAmount);
            if (TCommon.RandomPercentage() <= Value1)
                m_Attacher.m_CharacterInfo.OnCoinsGain(coinAmount,false);
        }
        public E0035_CoinsGainDouble() { }
    }

    public class E0036_CoinsLifeExchange:EquipmentBase
    {
        public override int m_Index => 0036;
        public override float Value1 => EquipmentConsts.P_0036_CoinsReviveAmount(m_rarity);
        public override bool OnCheckRevive()
        {
            float coinsValue = Value1;
            if (m_Attacher.m_CharacterInfo.CanCostCoins(coinsValue))
            {
                m_Attacher.m_CharacterInfo.OnCoinsCost(coinsValue);
                return true;
            }
            return false;
        }
        public E0036_CoinsLifeExchange() { }
    }
    
    //public class E0037_CoinsDamage:EquipmentBase
    //{
    //    public override int m_Index => 0037;
    //    public override float Value1 => EquipmentConsts.F_0037_DamageMultiplyPer10Coins(m_rarity);
    //    public override float m_DamageMultiply => Value1/100f*m_Attacher.m_CharacterInfo.m_Coins/10f;
    //    public E0037_CoinsDamage() { }
    //}
    #endregion
    #endregion
    #region ActionBuff
    public class B10001_MaxArmorAdditive : ActionBuffBase
    {
        public override int m_Index => 10001;
        public override float Value1 => m_RecordData;
        public override float m_MaxArmorAdditive => Value1;
        public B10001_MaxArmorAdditive() { }
        protected override void SetDefaultData()
        {
            base.SetDefaultData();
            m_RecordData = ActionBuffConsts.RI_10001_StartArmor.Random();
        }
    }
    public class B_10002_DamageMultiply:ActionBuffBase
    {
        public override int m_Index => 10002;
        public override float Value1 => ActionBuffConsts.P_10002_DamageMultiply;
        public override float m_MaxArmorAdditive => Value1;
        public B_10002_DamageMultiply() { }
    }
    public class B10003_MaxHealthAdditive : ActionBuffBase
    {
        public override int m_Index => 10003;
        public override float Value1 => m_RecordData;
        public override float m_MaxHealthAdditive => Value1;
        public B10003_MaxHealthAdditive() { }
        protected override void SetDefaultData()
        {
            base.SetDefaultData();
            m_RecordData = ActionBuffConsts.RI_10003_StartHealth.Random();
        }
    }
    #endregion
    #endregion
}