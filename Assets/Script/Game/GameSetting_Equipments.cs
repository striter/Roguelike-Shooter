using GameSetting;
using UnityEngine;
using System;
using System.Collections.Generic;
namespace GameSetting_Action
{
    #region Desingers Data
    public static class ActionData
    {
        #region 10000-19999
        public const int I_10001_Cost=4;
        public const float F_10001_Duration = 30f;
        public static int P_10001_ClipMultiply(enum_ActionRarity rarity) => 50 * (int)rarity;

        public const int I_10002_Cost = 4;
        public const float F_10002_Duration = 5f;
        public static float F_10002_ArmorReceive(enum_ActionRarity rarity) => 1.5f * (int)rarity;

        public const int I_10003_Cost = 4;
        public const float F_10003_Duration = 15f;
        public static float P_10003_FirerateMultiplyPerMile(enum_ActionRarity rarity) => 1.5f * ((int)rarity);
        public const int I_10003_MaxStackAmount = 40;
        
        public const int I_10005_Cost = 4;
        public static int P_10005_OnFire_DamageMultiply(enum_ActionRarity rarity) => 110 * (int)rarity;
        public static int P_10005_OnKill_Buff_MovementSpeed(enum_ActionRarity rarity) => 60 * (int)rarity;
        public static float F_10005_OnKill_Buff_Duration = 5f;

        public const int I_10006_Cost = 6;
        public static int P_10006_DamageMultiply(enum_ActionRarity rarity) => 110 * (int)rarity;
        public static int I_10006_OnKill_ArmorReceive(enum_ActionRarity rarity) => 10 + 30 * (int)rarity;

        public const int I_10007_Cost = 6;
        public static int P_10007_OnFire_DamageMultiply(enum_ActionRarity rarity) => 110 * (int)rarity;
        public static int I_10007_OnKill_HealReceive(enum_ActionRarity rarity) => 10 + 30 * (int)rarity;

        public const int I_10008_Cost = 4;
        public const float F_10008_Duration = 10f;
        public const int P_10008_MovementReduction = 20;
        public static int P_10008_FireRateAdditive(enum_ActionRarity rarity) => 60 * (int)rarity;

        public const int I_10009_Cost = 2;
        public const float F_10009_Duration = 10f;
        public static int P_10009_FireRateAdditive(enum_ActionRarity rarity) => 20 * (int)rarity;

        public const int I_10010_Cost = 4;
        public const float F_10010_Duration = 10f;
        public const float F_10010_DamageReduction = 4f;
        public static int P_10010_FireRateAdditive(enum_ActionRarity rarity) => 60 * (int)rarity;

        public const int I_10011_Cost = 4;
        public const float F_10011_Duration = 10f;
        public static float P_10011_FireRateAdditivePerHitStack(enum_ActionRarity rarity) => 1f * (int)rarity;

        public const int I_10012_Cost = 4;
        public const float F_10012_Duration = 10f;
        public static float F_10012_DamageAdditive(enum_ActionRarity rarity) => 7 * (int)rarity;

        public const int I_10013_Cost = 4;
        public static int P_10013_DamageMultiply(enum_ActionRarity rarity) => 110 * (int)rarity;
        public static float F_10013_CloakDuration(enum_ActionRarity rarity) => 5*(int)rarity;

        public const int I_10014_Cost = 4;
        public const float F_10014_Duration = 15f;
        public static float F_10014_HitFrozenDamageAdditive(enum_ActionRarity rarity) => 10 * (int)rarity;

        public const int I_10015_Cost = 1;
        public static float F_10015_FrozeDuration(enum_ActionRarity rarity) => 3f * (int)rarity;

        public const int I_10016_Cost = 2;
        public static int P_10016_FrozenDirectDamageMultiply(enum_ActionRarity rarity) => 110 * (int)rarity;

        public static int I_10017_Cost(enum_ActionRarity rarity) => 3-(int)rarity;
        public const int I_10017_EquipmentCopyTimes = 1;

        public const int I_10018_Cost = 6;
        public const float F_10018_Duration = 10f;
        public const float F_10018_PerStackHealthLoss = 5f;
        public static float P_10018_HealthStealPerStack(enum_ActionRarity rarity) => 1f * (int)rarity;
        
        public const int I_10020_Cost = 4;
        public const float F_10020_Duration = 20f;
        public const float F_10020_PerStackHealthLoss = 1f;
        public static float P_10020_MovementAdditivePerStack(enum_ActionRarity rarity) => 1f * (int)rarity;

        public const int I_10021_Cost = 2;
        public const float F_20021_Duration = 10f;
        public static float F_10021_HealthRegenPerMin(enum_ActionRarity rarity) => 3f * (int)rarity;

        public const int I_10022_Cost = 6;
        public static float F_10022_Duration(enum_ActionRarity rarity) => 4f + 2f * (int)rarity;
        public const float P_10022_HealthRegenTranslateFromDamage = 100;

        public const int I_10023_Cost = 8;
        public static float F_10023_ReviveHealthRegen(enum_ActionRarity rarity) => 30 * (int)rarity;

        public const int I_10024_Cost = 4;
        public static float F_10024_ArmorAdditive(enum_ActionRarity rarity) => 15 * (int)rarity;

        public const int I_10025_Cost = 8;
        public static float P_10025_ArmorAdditiveMultiply(enum_ActionRarity rarity) => 30* (int)rarity;

        public const int I_10026_Cost = 4;
        public const float F_10026_Duration = 10f;
        public static float F_10026_DamageAdditiveMultiplyWithArmor(enum_ActionRarity rarity) => 0.035f * (int)rarity;

        public const int I_10027_Cost = 4;
        public static float F_10027_HealthDamage(enum_ActionRarity rarity) => 15 * (int)rarity;
        public static float F_10027_ArmorAdditive(enum_ActionRarity rarity) => 45 * (int)rarity;

        public const int I_10028_Cost = 4;
        public static float P_10028_NextShotDamageMultiplyPerArmor(enum_ActionRarity rarity) => 1.1f * (int)rarity;

        public const int I_10029_Cost = 4;
        public const float F_10029_Duration = 30f;
        public static int I_10029_ClipAdditive(enum_ActionRarity rarity) => 4 * (int)rarity;

        public const int I_10030_Cost = 2;
        public const float F_10030_Duration = 10f;
        public static float P_10030_MovementSpeedAdditive(enum_ActionRarity rarity) => 30 * (int)rarity;
        
        public const int I_10032_Cost = 1;
        public static float F_10032_Duration(enum_ActionRarity rarity) => 10 * (int)rarity;

        public const int I_10033_Cost = 4;
        public static float F_10033_Duration(enum_ActionRarity rarity) => 10 + 5 * (int)rarity;

        public const int I_10034_Cost = 4;
        public static float P_10034_DamageAdditiveFirstShot(enum_ActionRarity rarity) => 110f * (int)rarity;
        public static float P_10034_DamageAdditiveShotAfterKill(enum_ActionRarity rarity) => 170f * (int)rarity;

        public const int I_10035_Cost = 4;
        public static float P_10035_DamageAdditiveNextShot(enum_ActionRarity rarity) => 220f * (int)rarity;
        #endregion
        #region 20000-29999
        public const int I_20001_Cost = 4;
        public static float F_20001_RustDamageDuration(enum_ActionRarity rarity) => 10f;
        public static float F_20001_RustDamagePerSecond(enum_ActionRarity rarity) => 40 * (int)rarity;

        public const int I_20002_Cost = 8;
        public static float F_20002_ArmorAdditiveTargetDead(enum_ActionRarity rarity) => 70 * (int)rarity;

        public const int I_20003_Cost = 2;
        public static float F_20003_FreezeDuration(enum_ActionRarity rarity) => 3 * (int)rarity;

        public const int I_20004_Cost = 2;
        public static float F_20004_FreezeDuration(enum_ActionRarity rarity) => 3 * (int)rarity;

        public const int I_20005_Cost = 6;
        public static float F_20005_FreezeDuration(enum_ActionRarity rarity) => 0.8f;
        public static float F_20005_Health(enum_ActionRarity rarity) => 400;
        public static float F_20005_Damage(enum_ActionRarity rarity) => 40 * (int)rarity;
        public const float F_20005_FireRate=1f;

        public const int I_20006_Cost = 1;
        public static float F_20006_FreezeDuration(enum_ActionRarity rarity) => 3 * (int)rarity;

        public static int I_20007_Cost(enum_ActionRarity rarity) => 1;
        public static float F_20007_Distance(enum_ActionRarity rarity) => 8f * (int)rarity;

        public const int I_20008_Cost = 4;
        public static float F_20008_Health(enum_ActionRarity rarity) => 400;
        public static float F_20008_Damage(enum_ActionRarity rarity) => 10 * (int)rarity;
        public const float F_20008_FireRate = 0.25f;

        public const int I_20009_Cost = 6;
        public static float F_20009_Health(enum_ActionRarity rarity) => 400;
        public static float F_20009_Damage(enum_ActionRarity rarity) => 40 * (int)rarity;
        public const float F_20009_FireRate = 1f;

        public const int I_20010_Cost = 4;
        public static float F_20010_Health(enum_ActionRarity rarity) => 400;
        public static float P_20010_PlayerHealthDrain(enum_ActionRarity rarity) =>2*(int)rarity;
        public static float P_20010_AIHealthDrain(enum_ActionRarity rarity) => 100 * (int)rarity;

        //20011 To Be Continued
        
        public const int I_20012_Cost = 4;
        public static float F_20012_Health(enum_ActionRarity rarity) => 400;
        public static float F_20012_PlayerHealthRegen(enum_ActionRarity rarity) => 1.5f * (int)rarity;
        public static float F_20012_AIHealthRegen(enum_ActionRarity rarity) => 20 * (int)rarity;

        public const int I_20013_Cost = 4;
        public static float P_20013_DamageMultiplyBase(enum_ActionRarity rarity) => 300 * (int)rarity;
        #endregion
        #region 30000-39999
        public const int I_30001_Cost = 2;
        public static float F_30001_DamagePerStack(enum_ActionRarity rarity) => 5f * (int)rarity;
        public const int I_30001_MaxStack = 20;
        
        public static float P_30002_SingleShotDamageMultiplyAfterKill(enum_ActionRarity rarity) => 70 * (int)rarity;
        
        public static float F_30003_GrenadeDamage(enum_ActionRarity rarity) => 200 * (int)rarity;
        
        public static float P_30004_ReloadSpeedMultiply(enum_ActionRarity rarity) => 50 * (int)rarity;
        
        public static float F_30005_FreezeDuration(enum_ActionRarity rarity) => .05f * (int)rarity;
        
        public static float F_30006_FreezeDuration(enum_ActionRarity rarity) => 3 * (int)rarity;
        
        public static float F_30007_DamageStackLimit(enum_ActionRarity rarity) => 35;
        public static float F_30007_FreezeDurationPerStack(enum_ActionRarity rarity) => .1f * (int)rarity;
        
        public static float F_30008_FreezeDuration(enum_ActionRarity rarity) => F_20006_FreezeDuration(rarity);
        
        public static float P_30009_EquipmentHealthAddup(enum_ActionRarity rarity) => 50 * (int)rarity;
        
        public static float F_30010_MaxHealthAddup(enum_ActionRarity rarity) => 3f * (int)rarity;
        
        public static float P_30011_MaxHealthRegen(enum_ActionRarity rarity) => 10 * (int)rarity;
        
        public static float P_30012_HealthRegenEachKill(enum_ActionRarity rarity) => 6 * (int)rarity;
        
        public static float P_30013_HealthRegenAdditive(enum_ActionRarity rarity) => 40* (int)rarity;
        
        public static float F_30014_DamageReductionDuration(enum_ActionRarity rarity) => 1*(int)rarity;
        
        public static float F_30015_DamageReflectPerArmor(enum_ActionRarity rarity) => 1 * (int)rarity;
        
        public static float P_30016_ProjectileSpeedMultiply(enum_ActionRarity rarity) => 100 * (int)rarity;
        
        public static float F_30017_ArmorAdditive(enum_ActionRarity rarity) => 3f * (int)rarity;
        #endregion
        #region 40000-49999
        public static float F_40001_ArmorAdditive(enum_ActionRarity rarity) => 6 * (int)rarity;

        public static float F_40002_MaxHealthAdd(enum_ActionRarity rarity) => 60 * (int)rarity;

        public static float F_40003_Range(enum_ActionRarity rarity) => 10;
        public static float P_40003_PlayerDamageMultiply(enum_ActionRarity rarity) => 4.2f * (int)(rarity);
        public static float P_40003_AllyDamageMultiply(enum_ActionRarity rarity) => 42 * (int)rarity;

        public static float P_40004_MovementSpeedAdditive(enum_ActionRarity rarity) => 13 * (int)rarity;

        public static float P_40005_FastReloadRate(enum_ActionRarity rarity) => 4 * (int)rarity;

        public static int I_40006_ClipAdditive(enum_ActionRarity rarity) => 2 * (int)rarity;

        public static float F_40007_DamageAdditive(enum_ActionRarity rarity) => 4f * (int)rarity;

        public static float P_40008_MaxHealthAdditive(enum_ActionRarity rarity) => 50 * (int)rarity;

        public static float F_40009_DamageMultiplyAfterRloead(enum_ActionRarity rarity) => 40f * (int)rarity;

        public static float F_40010_Range(enum_ActionRarity rarity) => 8f;
        public static float P_40010_DamageMultiply(enum_ActionRarity rarity) => 12f * (int)rarity;

        public static float P_40011_ReloadMultiply(enum_ActionRarity rarity) => 50f * (int)rarity;

        public static float P_40012_MovementSpeedMultiply(enum_ActionRarity rarity) => 45f * (int)rarity;
        public static float F_40012_Duration(enum_ActionRarity rarity) => 5f;

        public static float P_40013_MovementSpeedMultiply(enum_ActionRarity rarity) => 30f * (int)rarity;
        public static float F_40013_Duration(enum_ActionRarity rarity) => 5f;

        public static float P_40014_RecoilReduction(enum_ActionRarity rarity) => 25f + 25f * (int)rarity;

        public static float F_40015_Range(enum_ActionRarity rarity) => 8f;
        public static float P_40015_HealthDrain(enum_ActionRarity rarity) => 2f * (int)rarity;
        
        public static float P_40017_DamageReduction(enum_ActionRarity rarity) => 20f + 15f * (int)rarity;

        public static float P_40018_DamageMultiply(enum_ActionRarity rarity) => 100f * (int)rarity;
        
        public static float F_40020_FreezeDuration(enum_ActionRarity rarity) => F_20004_FreezeDuration(rarity);

        public static float F_40021_FreezeDuration(enum_ActionRarity rarity) => 2f* (int)rarity;

        public static float P_40022_HealAdditive(enum_ActionRarity rarity) => 80f * (int)rarity;
        #endregion
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
    public class ActionStackUp
    {
        public ActionStackUp() { m_maxStackup = -1; }
        public ActionStackUp(float _maxStackup = -1) { m_maxStackup = _maxStackup; m_stack = 0; }
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
    #region Inherted Claseses

    #region 30000-39999
    public class Action_30001_DamageMovementStackup : EquipmentExpire
    {
        public override int m_Index => 30001;
        public override float Value1 => ActionData.F_30001_DamagePerStack(m_rarity);
        public override float Value2 => Value1*ActionData.I_30001_MaxStack;
        public override float m_DamageAdditive => Value1 * m_stackUp.m_stack;
        ActionStackUp m_stackUp;
        public override void OnFire(int identity)
        {
            base.OnFire(identity);
            m_stackUp.ResetStack();
        }
        public override void OnMove(float distance) => m_stackUp.OnStackUp(distance);
        public Action_30001_DamageMovementStackup(int _identity, enum_EquipmentType _type) : base(_identity, _type) { m_stackUp = new ActionStackUp(ActionData.I_30001_MaxStack); }
    }
    public class Action_30002_KillDamageMultiply : EquipmentExpire
    {
        public override int m_Index => 30002;
        public override int m_EffectIndex => 41002;
        public override float Value1 => ActionData.P_30002_SingleShotDamageMultiplyAfterKill(m_rarity);
        public override float m_DamageMultiply => m_burstShot ? Value1/100f : 0f;
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
        public Action_30002_KillDamageMultiply(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }
    public class Action_30003_GrenadeLauncher : EquipmentExpire
    {
        public override int m_Index => 30003;
        public override float Value1 => ActionData.F_30003_GrenadeDamage(m_rarity);
        WeaponHelperBase m_WeaponHelper;
        DamageDeliverInfo GetDamageInfo()=> DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, Value1, enum_CharacterEffect.Invalid, 0); 
        public override void OnActivate()
        {
            base.OnActivate();
            m_WeaponHelper = WeaponHelperBase.AcquireWeaponHelper(GameExpression.GetPlayerWeaponIndex(m_Index), m_ActionEntity, GetDamageInfo);
        }
        bool m_shotGrenade = false;
        public override void OnReloadFinish()
        {
            base.OnReloadFinish();
            m_shotGrenade = true;
        }
        public override void OnFire(int identity)
        {
            base.OnFire(identity);
            if (m_shotGrenade)
                m_WeaponHelper.OnPlay( m_ActionEntity,m_ActionEntity.tf_Head.position+m_ActionEntity.tf_Head.forward*5);

            m_shotGrenade = false;
        }
        public Action_30003_GrenadeLauncher(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }
    public class Action_30004_ReloadSpeedAdditive : EquipmentExpire
    {
        public override int m_Index => 30004;
        public override float Value1 => ActionData.P_30004_ReloadSpeedMultiply(m_rarity);
        public override float m_ReloadRateMultiply =>Value1/100f;
        public Action_30004_ReloadSpeedAdditive(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }
    public class Action_30005_DamageFreeze : EquipmentExpire
    {
        public override int m_Index => 30005;
        public override int m_EffectIndex => 41001;
        public override float Value1 => ActionData.F_30005_FreezeDuration(m_rarity);
        public override void OnDealtDamageSetEffect(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnDealtDamageSetEffect(receiver, info);
            info.m_detail.EffectAdditiveOverride(enum_CharacterEffect.Freeze, Value1);
        }
        public Action_30005_DamageFreeze(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class Action_30006_DamageKillFreezeBlast : EquipmentExpire
    {
        public override int m_Index => 30006;
        public override float Value1 => ActionData.F_30006_FreezeDuration(m_rarity);
        WeaponHelperBase m_WeaponHelper;
        DamageDeliverInfo GetDamageInfo() => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1);
        public override void OnActivate()
        {
            base.OnActivate();
            m_WeaponHelper = WeaponHelperBase.AcquireWeaponHelper(GameExpression.GetPlayerWeaponIndex(m_Index), m_ActionEntity, GetDamageInfo);
        }
        public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnAfterDealtDemage(receiver, info, applyAmount);
            if (!receiver.m_IsDead)
                return;

            m_WeaponHelper.OnPlay(false, receiver);
        }
        
        public Action_30006_DamageKillFreezeBlast(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class Action_30007_DamageLimitFreeze : EquipmentExpire
    {
        public override int m_Index => 30007;
        public override int m_EffectIndex => 41001;
        public override float Value1 => ActionData.F_30007_DamageStackLimit(m_rarity);
        public override float Value2 => ActionData.F_30007_FreezeDurationPerStack(m_rarity);
        public override void OnDealtDamageSetEffect(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnDealtDamageSetEffect(receiver, info);
            float amount = info.m_AmountApply;
            if (amount < Value1)
                return;

            info.m_detail.EffectAdditiveOverride(enum_CharacterEffect.Freeze, Value2 * Mathf.Ceil(amount / Value1));
        }
        public Action_30007_DamageLimitFreeze(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class Action_30009_AllyActivateHealthAdditive : EquipmentExpire
    {
        public override int m_Index => 30009;
        public override float Value1 => ActionData.P_30009_EquipmentHealthAddup(m_rarity);
        public override float F_AllyHealthMultiplierAdditive => Value1/100f;
        public Action_30009_AllyActivateHealthAdditive(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }
    
    public class Action_30011_ReceiveHealingAddMaxHealth : EquipmentExpire
    {
        public override int m_Index => 30011;
        public override float Value1 => ActionData.P_30011_MaxHealthRegen(m_rarity);
        public override void OnReceiveHealing(DamageInfo info, float amount)
        {
            base.OnReceiveHealing(info, amount);
            if(info.m_Type== enum_DamageType.HealthOnly)
                 m_ActionEntity.m_Health.AddMaxHealth (-info.m_AmountApply*Value1/100f);
        }
        public Action_30011_ReceiveHealingAddMaxHealth(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class Action_30012_KillHealthRegen : EquipmentExpire
    {
        public override int m_Index => 30012;
        public override float Value1 => ActionData.P_30012_HealthRegenEachKill(m_rarity);
        public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnAfterDealtDemage(receiver, info, applyAmount);
            if(receiver.m_IsDead)
                EquipmentHelper.ReceiveHealing(m_ActionEntity,m_ActionEntity.m_Health.m_BaseHealth * Value1/100f, enum_DamageType.HealthOnly);
        }
        public Action_30012_KillHealthRegen(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class Action_30013_HealRegenAdditive : EquipmentExpire
    {
        public override int m_Index => 30013;
        public override float Value1 => ActionData.P_30013_HealthRegenAdditive(m_rarity);
        public override float m_HealAdditive => Value1 / 100f;
        public Action_30013_HealRegenAdditive(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class Action_30014_ArmorDamageDamageReductionDuration : EquipmentExpire
    {
        public override int m_Index => 30014;
        public override int m_EffectIndex => m_Effecting? 40004:0;
        public override float Value1 => ActionData.F_30014_DamageReductionDuration(m_rarity);
        public override float m_DamageReduction => m_Effecting ? 1 : 0;
        float m_counter = 0;
        bool m_Effecting => m_counter > 0;
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            if (m_Effecting) m_counter -= deltaTime;
        }
        public override void OnBeforeReceiveDamage(DamageInfo info)
        {
            base.OnBeforeReceiveDamage(info);
            if (m_Effecting)
                return;

            if (m_ActionEntity.m_Health.m_CurrentArmor > 0)
                m_counter = Value1;
        }
        public Action_30014_ArmorDamageDamageReductionDuration(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class Action_30015_ArmorDamageReflection : EquipmentExpire
    {
        public override int m_Index => 30015;
        public override int m_EffectIndex => 40007;
        public override float Value1 => ActionData.F_30015_DamageReflectPerArmor(m_rarity);
        public override void OnBeforeReceiveDamage(DamageInfo info)
        {
            base.OnBeforeReceiveDamage(info);
            if (m_ActionEntity.m_Health.m_CurrentArmor > 0)
                EquipmentHelper.PlayerDealtDamageToEntity(m_ActionEntity, info.m_detail.I_SourceID, Value1 * m_ActionEntity.m_Health.m_CurrentArmor, enum_DamageType.Basic);
        }
        public Action_30015_ArmorDamageReflection(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }

    public class Action_30016_ProjectileSpeed : EquipmentExpire
    {
        public override int m_Index => 30016;
        public override float Value1 => ActionData.P_30016_ProjectileSpeedMultiply(m_rarity);
        public override float F_ProjectileSpeedMultiply => Value1/100f;
        public Action_30016_ProjectileSpeed(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }
    
    public class Action_30018_ProjectilePenetrate : EquipmentExpire
    {
        public override int m_Index => 30018;
        public override bool B_ProjectilePenetrade => true;
        public Action_30018_ProjectilePenetrate(int _identity, enum_EquipmentType _type) : base(_identity, _type) { }
    }
    #endregion
    #endregion
    #endregion
}