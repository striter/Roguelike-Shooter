using GameSetting;
using UnityEngine;
using System;
using System.Collections.Generic;
namespace GameSetting_Action
{
    #region Desingers Data
    public static class EquipmentData
    {
        public static int P_0001_ReloadSpeedAdditive(enum_EquipmentRarity rarity) => 50 * (int)rarity;
        public static int P_0002_ClipRefillRate(enum_EquipmentRarity rarity) => 4 * (int)rarity;
        public static int I_0003_ClipAdditive(enum_EquipmentRarity rarity) => 1 + 1 * (int)rarity;
        public static int P_0004_ClipMultiply(enum_EquipmentRarity rarity) => 1 * (int)rarity;
        public static float F_0005_DamageAdditive(enum_EquipmentRarity rarity) => 4f * (int)rarity;
        public static float F_0006_DamageAdditive(enum_EquipmentRarity rarity) => 1f * (int)rarity;
        public static float F_0006_Duration(enum_EquipmentRarity rarity) => 5*(int)rarity;
        public static int P_0007_SpreadReduction(enum_EquipmentRarity rarity) => 20 * (int)rarity;
        public static float F_0008_AimRangeIncrease(enum_EquipmentRarity rarity) => 5f * (int)rarity;
        public static int P_0009_DamageMultiply(enum_EquipmentRarity rarity) => 70 * (int)rarity;
        public static int P_0010_PenetrateAdditive(enum_EquipmentRarity rarity) => 30 * (int)rarity;
        public static int P_0011_FireRateAdditive(enum_EquipmentRarity rarity) => 50 * (int)rarity;
        public static int I_0012_BounceTimes(enum_EquipmentRarity rarity) => 1*(int)rarity;
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

    public class EquipmentTimer
    {
        float m_timeCheck = -1;
        public bool m_Timing => m_timeCheck > 0;
        public void SetTimer(float duration)=>m_timeCheck = duration;
        public void Tick(float deltaTime)
        {
            if (m_timeCheck <= 0)
                return;
            m_timeCheck -= deltaTime;
        }
    }
    #region Inherted Claseses
    
    public class E0001_ReloadSpeedAdditive: PlayerEquipmentExpire
    {
        public override int m_Index => 0001;
        public override float Value1 => EquipmentData.P_0001_ReloadSpeedAdditive(m_rarity);
        public override float m_ReloadRateMultiply => Value1 / 100f;
        public E0001_ReloadSpeedAdditive(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0002_ClipRefillOnAttack:PlayerEquipmentExpire
    {
        public override int m_Index => 0002;
        public override float Value1 => EquipmentData.P_0002_ClipRefillRate(m_rarity);
        public override void OnFire(int identity)
        {
            base.OnFire(identity);
            if (TCommon.RandomPercentage() < Value1)
                m_ActionEntity.m_WeaponCurrent.ForceReload();
        }
        public E0002_ClipRefillOnAttack(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0003_ClipAdditive : PlayerEquipmentExpire
    {
        public override int m_Index => 0003;
        public override float Value1 => EquipmentData.I_0003_ClipAdditive(m_rarity);
        public override int I_ClipAdditive => (int)Value1;
        public E0003_ClipAdditive(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0004_ClipMultiply : PlayerEquipmentExpire
    {
        public override int m_Index => 0004;
        public override float Value1 => EquipmentData.P_0004_ClipMultiply(m_rarity);
        public override float F_ClipMultiply => Value1/100f;
        public E0004_ClipMultiply(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0005_ClipMultiply : PlayerEquipmentExpire
    {
        public override int m_Index => 0005;
        public override float Value1 => EquipmentData.F_0005_DamageAdditive(m_rarity);
        public override float m_DamageAdditive => Value1;
        public E0005_ClipMultiply(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0006_DamageAdditiveAfterReload:PlayerEquipmentExpire
    {
        public override int m_Index => 0006;
        public override float Value1 => EquipmentData.F_0006_DamageAdditive(m_rarity);
        public override float Value2 => EquipmentData.F_0006_Duration(m_rarity);
        public override float m_DamageAdditive => m_Timer.m_Timing?Value1:base.m_DamageAdditive;
        EquipmentTimer m_Timer=new EquipmentTimer();
        public override void OnReloadFinish()
        {
            base.OnReloadFinish();
            m_Timer.SetTimer(Value2);
        }
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            m_Timer.Tick(deltaTime);
        }
        public E0006_DamageAdditiveAfterReload(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0007_SpreadReduction:PlayerEquipmentExpire
    {
        public override int m_Index => 0007;
        public override float Value1 => EquipmentData.P_0007_SpreadReduction(m_rarity)/100f;
        public override float F_SpreadReduction => Value1;
        public E0007_SpreadReduction(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0008_AimRangeIncrease : PlayerEquipmentExpire
    {
        public override int m_Index => 0008;
        public override float Value1 => EquipmentData.F_0008_AimRangeIncrease(m_rarity);
        public override float F_AimRangeAdditive => Value1;
        public E0008_AimRangeIncrease(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0009_DamageBurstAfterKill : PlayerEquipmentExpire
    {
        public override int m_Index => 0009;
        public override float Value1 => EquipmentData.P_0009_DamageMultiply(m_rarity);
        public override float m_DamageMultiply => m_burstShot ? Value1 / 100f : 0f;
        bool m_burstShot = false;
        public override void OnFire(int identity)
        {
            base.OnFire(identity);
            m_burstShot = false;
        }
        public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnAfterDealtDemage(receiver, info, applyAmount);
            m_burstShot = receiver.m_IsDead;
        }
        public E0009_DamageBurstAfterKill(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0010_PenetrateAdditive:PlayerEquipmentExpire
    {
        public override int m_Index => 0010;
        public override float Value1 => EquipmentData.P_0010_PenetrateAdditive(m_rarity);
        public override float F_PenetradeAdditive => Value1/100f;
        public E0010_PenetrateAdditive(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0011_FireRateAdditive:PlayerEquipmentExpire
    {
        public override int m_Index => 0011;
        public override float Value1 => EquipmentData.P_0011_FireRateAdditive(m_rarity);
        public override float m_FireRateMultiply => Value1 / 100f;
        public E0011_FireRateAdditive(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class E0012_ProjectileCopy:PlayerEquipmentExpire
    {
        public override int m_Index => 0012;
        public override float Value1 => EquipmentData.I_0012_BounceTimes(m_rarity);
        public override int I_ProjectileCopyAdditive => (int)Value1;
        public E0012_ProjectileCopy(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }
    #endregion
    #endregion
}