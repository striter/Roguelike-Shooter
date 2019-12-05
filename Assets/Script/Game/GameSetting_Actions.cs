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

        public const int I_10002_Cost = 2;
        public const float F_10002_Duration = 5f;
        public static float F_10002_ArmorReceive(enum_ActionRarity rarity) => 1.5f * (int)rarity;

        public const int I_10003_Cost = 4;
        public const float F_10003_Duration = 15f;
        public static float P_10003_FirerateMultiplyPerMile(enum_ActionRarity rarity) => 6f * ((int)rarity);
        public const int I_10003_MaxStackAmount = 30;
        
        public const int I_10005_Cost = 4;
        public static int P_10005_OnFire_DamageMultiply(enum_ActionRarity rarity) => 210 * (int)rarity;
        public static int P_10005_OnKill_Buff_MovementSpeed(enum_ActionRarity rarity) => 30 + 30 * (int)rarity;
        public static float F_10005_OnKill_Buff_Duration = 5f;

        public const int I_10006_Cost = 6;
        public static int P_10006_DamageMultiply(enum_ActionRarity rarity) => 210 * (int)rarity;
        public static int I_10006_OnKill_ArmorReceive(enum_ActionRarity rarity) => 10 + 30 * (int)rarity;

        public const int I_10007_Cost = 6;
        public static int P_10007_OnFire_DamageMultiply(enum_ActionRarity rarity) => 210 * (int)rarity;
        public static int I_10007_OnKill_HealReceive(enum_ActionRarity rarity) => 10 + 30 * (int)rarity;

        public const int I_10008_Cost = 4;
        public const float F_10008_Duration = 10f;
        public const int P_10008_MovementReduction = 20;
        public static int P_10008_FireRateAdditive(enum_ActionRarity rarity) => 160 * (int)rarity;

        public const int I_10009_Cost = 2;
        public const float F_10009_Duration = 10f;
        public static int P_10009_FireRateAdditive(enum_ActionRarity rarity) => 55 * (int)rarity;

        public const int I_10010_Cost = 2;
        public const float F_10010_Duration = 10f;
        public const float F_10010_DamageReduction = 4f;
        public static int P_10010_FireRateAdditive(enum_ActionRarity rarity) => 160 * (int)rarity;

        public const int I_10011_Cost = 2;
        public const float F_10011_Duration = 10f;
        public static float P_10011_FireRateAdditivePerHitStack(enum_ActionRarity rarity) => 3f * (int)rarity;

        public const int I_10012_Cost = 4;
        public const float F_10012_Duration = 10f;
        public static float F_10012_DamageAdditive(enum_ActionRarity rarity) => 15 * (int)rarity;

        public const int I_10013_Cost = 4;
        public static int P_10013_DamageMultiply(enum_ActionRarity rarity) => 210 * (int)rarity;
        public static float F_10013_CloakDuration(enum_ActionRarity rarity) => 5*(int)rarity;

        public const int I_10014_Cost = 4;
        public const float F_10014_Duration = 15f;
        public static float F_10014_HitFrozenDamageAdditive(enum_ActionRarity rarity) => 20 * (int)rarity;

        public const int I_10015_Cost = 2;
        public static float F_10015_FrozeDuration(enum_ActionRarity rarity) => 3f * (int)rarity;

        public const int I_10016_Cost = 2;
        public static int P_10016_FrozenDirectDamageMultiply(enum_ActionRarity rarity) => 210 * (int)rarity;

        public static int I_10017_Cost(enum_ActionRarity rarity) => 3-(int)rarity;
        public const int I_10017_EquipmentCopyTimes = 1;

        public const int I_10018_Cost = 8;
        public const float F_10018_Duration = 10f;
        public const float F_10018_PerStackHealthLoss = 5f;
        public static float P_10018_HealthStealPerStack(enum_ActionRarity rarity) => 1f * (int)rarity;
        
        public const int I_10020_Cost = 4;
        public const float F_10020_Duration = 20f;
        public const float F_10020_PerStackHealthLoss = 1f;
        public static float P_10020_MovementAdditivePerStack(enum_ActionRarity rarity) => 2f * (int)rarity;

        public const int I_10021_Cost = 4;
        public const float F_20021_Duration = 10f;
        public static float F_10021_HealthRegenPerMin(enum_ActionRarity rarity) => 3f * (int)rarity;

        public const int I_10022_Cost = 8;
        public static float F_10022_Duration(enum_ActionRarity rarity) => 4f + 2f * (int)rarity;
        public const float P_10022_HealthRegenTranslateFromDamage = 100;

        public const int I_10023_Cost = 8;
        public static float F_10023_ReviveHealthRegen(enum_ActionRarity rarity) => 30 * (int)rarity;

        public const int I_10024_Cost = 4;
        public static float F_10024_ArmorAdditive(enum_ActionRarity rarity) => 30 * (int)rarity;

        public const int I_10025_Cost = 8;
        public static float P_10025_ArmorAdditiveMultiply(enum_ActionRarity rarity) => 60* (int)rarity;

        public const int I_10026_Cost = 4;
        public const float F_10026_Duration = 10f;
        public static float F_10026_DamageAdditiveMultiplyWithArmor(enum_ActionRarity rarity) => 0.2f * (int)rarity;

        public const int I_10027_Cost = 4;
        public static float F_10027_HealthDamage(enum_ActionRarity rarity) => 15 * (int)rarity;
        public static float F_10027_ArmorAdditive(enum_ActionRarity rarity) => 45 * (int)rarity;

        public const int I_10028_Cost = 2;
        public static float P_10028_NextShotDamageMultiplyPerArmor(enum_ActionRarity rarity) => 5.6f * (int)rarity;

        public const int I_10029_Cost = 4;
        public const float F_10029_Duration = 30f;
        public static int I_10029_ClipAdditive(enum_ActionRarity rarity) => 4 * (int)rarity;

        public const int I_10030_Cost = 2;
        public const float F_10030_Duration = 10f;
        public static float P_10030_MovementSpeedAdditive(enum_ActionRarity rarity) => 30 * (int)rarity;
        
        public const int I_10032_Cost = 2;
        public static float F_10032_Duration(enum_ActionRarity rarity) => 10 * (int)rarity;

        public const int I_10033_Cost = 4;
        public static float F_10033_Duration(enum_ActionRarity rarity) => 10 + 5 * (int)rarity;

        public const int I_10034_Cost = 4;
        public static float P_10034_DamageAdditiveFirstShot(enum_ActionRarity rarity) => 210f * (int)rarity;
        public static float P_10034_DamageAdditiveShotAfterKill(enum_ActionRarity rarity) => 320f * (int)rarity;

        public const int I_10035_Cost = 4;
        public static float P_10035_DamageAdditiveNextShot(enum_ActionRarity rarity) => 420f * (int)rarity;
        #endregion
        #region 20000-29999
        public const int I_20001_Cost = 4;
        public static float F_20001_RustDamageDuration(enum_ActionRarity rarity) => 10f;
        public static float F_20001_RustDamagePerSecond(enum_ActionRarity rarity) => 40 * (int)rarity;

        public const int I_20002_Cost = 4;
        public static float F_20002_ArmorAdditiveTargetDead(enum_ActionRarity rarity) => 70 * (int)rarity;

        public const int I_20003_Cost = 2;
        public static float F_20003_FreezeDuration(enum_ActionRarity rarity) => 3 * (int)rarity;

        public const int I_20004_Cost = 2;
        public static float F_20004_FreezeDuration(enum_ActionRarity rarity) => 3 * (int)rarity;

        public const int I_20005_Cost = 6;
        public static float F_20005_FreezeDuration(enum_ActionRarity rarity) => 0.8f;
        public static float F_20005_Health(enum_ActionRarity rarity) => 400 * (int)rarity;
        public static float F_20005_Damage(enum_ActionRarity rarity) => 40;
        public const float F_20005_FireRate=1f;

        public const int I_20006_Cost = 2;
        public static float F_20006_FreezeDuration(enum_ActionRarity rarity) => 3 * (int)rarity;

        public static int I_20007_Cost(enum_ActionRarity rarity) => 2;
        public static float F_20007_Distance(enum_ActionRarity rarity) => 8f * (int)rarity;

        public const int I_20008_Cost = 4;
        public static float F_20008_Health(enum_ActionRarity rarity) => 400 * (int)rarity;
        public static float F_20008_Damage(enum_ActionRarity rarity) => 40;
        public const float F_20008_FireRate = 1f;

        public const int I_20009_Cost = 6;
        public static float F_20009_Health(enum_ActionRarity rarity) => 400 * (int)rarity;
        public static float F_20009_Damage(enum_ActionRarity rarity) => 40;
        public const float F_20009_FireRate = 1f;

        public const int I_20010_Cost = 8;
        public static float F_20010_Health(enum_ActionRarity rarity) => 400;
        public static float P_20010_PlayerHealthDrain(enum_ActionRarity rarity) =>3*(int)rarity;
        public static float P_20010_AIHealthDrain(enum_ActionRarity rarity) => 100 * (int)rarity;

        //20011 To Be Continued
        
        public const int I_20012_Cost = 8;
        public static float F_20012_Health(enum_ActionRarity rarity) => 400;
        public static float F_20012_PlayerHealthRegen(enum_ActionRarity rarity) => 1.5f * (int)rarity;
        public static float F_20012_AIHealthRegen(enum_ActionRarity rarity) => 20 * (int)rarity;

        public const int I_20013_Cost = 4;
        public static float P_20013_DamageMultiplyBase(enum_ActionRarity rarity) => 300 * (int)rarity;
        #endregion
        #region 30000-39999
        public const int I_30001_Cost = 2;
        public static float F_30001_DamagePerStack(enum_ActionRarity rarity) => 2.5f * (int)rarity;
        public const int I_30001_MaxStack = 20;

        public const int I_30002_Cost = 3;
        public static float P_30002_SingleShotDamageMultiplyAfterKill(enum_ActionRarity rarity) => 30 * (int)rarity;

        public const int I_30003_Cost = 2;
        public static float F_30003_GrenadeDamage(enum_ActionRarity rarity) => 100 * (int)rarity;

        public const int I_30004_Cost = 2;
        public static float P_30004_ReloadSpeedMultiply(enum_ActionRarity rarity) => 25 * (int)rarity;

        public const int I_30005_Cost = 2;
        public static float F_30005_FreezeDuration(enum_ActionRarity rarity) => .05f * (int)rarity;

        public const int I_30006_Cost = 2;
        public static float F_30006_FreezeDuration(enum_ActionRarity rarity) => 3 * (int)rarity;

        public const int I_30007_Cost = 2;
        public static float F_30007_DamageStackLimit(enum_ActionRarity rarity) => 35;
        public static float F_30007_FreezeDurationPerStack(enum_ActionRarity rarity) => .1f * (int)rarity;

        public static int I_30008_Cost(enum_ActionRarity rarity) => 3 - (int)rarity;
        public static float F_30008_FreezeDuration(enum_ActionRarity rarity) => F_20006_FreezeDuration(rarity);

        public const int I_30009_Cost = 1;
        public static float P_30009_EquipmentHealthAddup(enum_ActionRarity rarity) => 25 * (int)rarity;

        public const int I_30010_Cost = 2;
        public static float F_30010_MaxHealthAddup(enum_ActionRarity rarity) => 3 * (int)rarity;

        public const int I_30011_Cost = 2;
        public static float P_30011_MaxHealthRegen(enum_ActionRarity rarity) => 6 * (int)rarity;

        public const int I_30012_Cost = 2;
        public static float P_30012_HealthRegenEachKill(enum_ActionRarity rarity) => 2.5f * (int)rarity;

        public const int I_30013_Cost = 1;
        public static float P_30013_HealthRegenAdditive(enum_ActionRarity rarity) => 20* (int)rarity;

        public const int I_30014_Cost = 3;
        public static float F_30014_DamageReductionDuration(enum_ActionRarity rarity) => 0.5f*(int)rarity;

        public const int I_30015_Cost = 2;
        public static float F_30015_DamageReflectPerArmor(enum_ActionRarity rarity) => 1 * (int)rarity;

        public const int I_30016_Cost = 1;
        public static float P_30016_ProjectileSpeedMultiply(enum_ActionRarity rarity) => 50 * (int)rarity;

        public const int I_30017_Cost = 2;
        public static float F_30017_ArmorAdditive(enum_ActionRarity rarity) => 2.5f * (int)rarity;
        
        public static int I_30018_Cost(enum_ActionRarity rarity)=>4-(int) rarity;
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
    public static class ActionHelper
    {
        public static EquipmentBase GetCommonDevice(int actionIndex,EntityCharacterPlayer player, Func<DamageDeliverInfo> damageInfo) => EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex(actionIndex), player, damageInfo);
        public static EquipmentEntitySpawner GetEntityDevice(int actionIndex, EntityCharacterPlayer player,  Func<DamageDeliverInfo> damage, int health, float fireRate)
        {
            EquipmentEntitySpawner equipment = GetCommonDevice(actionIndex,player, damage) as EquipmentEntitySpawner;
            equipment.SetOnSpawn(health, (EntityCharacterBase entity) =>
            {
                EntityCharacterAI target = entity as EntityCharacterAI;
                target.F_AttackDuration = new RangeFloat(0f, 0);
                target.F_AttackTimes = new RangeInt(1, 0);
                target.F_AttackRate = fireRate;
            });
            return equipment;
        }
        public static EquipmentEntitySpawner GetBuffDevice(int actionIndex, EntityCharacterPlayer player,float health,SBuff buffApplyPlayer,  SBuff buffApplyAlly,float refreshDuration)
        {
            EquipmentEntitySpawner equipment = GetCommonDevice(actionIndex, player, null) as EquipmentEntitySpawner;
            equipment.SetOnSpawn(health,(EntityCharacterBase entity)=>{(entity as EntityDeviceBuffApllier).SetBuffApply(buffApplyPlayer,buffApplyAlly,refreshDuration);});
            return equipment;
        }
        public static void PlayerAttachShield(EntityCharacterPlayer player, int equipmentIndex, int health)
        {
            EquipmentShieldAttach equipment = EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex(equipmentIndex), player,null) as EquipmentShieldAttach;
            equipment.SetOnSpawn((SFXShield shield) => {  shield.m_Health.I_MaxHealth = health; });
            equipment.Play(false,player);
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
    #region BaseClasses
    public class ActionAfterUse : ActionBase
    {
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override void OnActivate()
        {
            base.OnActivate();
            ForceExpire();
        }
        public ActionAfterUse(int _identity,enum_ActionRarity _level) : base(_identity,_level) { }
    }

    public class ActionRevive : ActionBase
    {
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        protected virtual RangeFloat m_ReviveAmount => new RangeFloat();
        public override bool OnCheckRevive(ref RangeFloat amount)
        {
            amount = m_ReviveAmount;
            ForceExpire();
            return true;
        }
        public ActionRevive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class ActionDevice : ActionAfterUse
    {
        protected virtual float m_DeviceDistance=>10;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override void OnActivate()
        {
            base.OnActivate();
            EquipmentBase device = GetDevice();
            device.Play(m_ActionEntity, m_ActionEntity.transform.position + m_ActionEntity.transform.forward * m_DeviceDistance);
        }

        public virtual EquipmentBase GetDevice()
        {
            Debug.LogError("Override This Please!");
            return null;
        }
        
        public ActionDevice(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class ActionStackUp : ActionBase
    {
        public ActionStackUp(int _identity, enum_ActionRarity _level) : base(_identity, _level) { m_maxStackup = -1; }
        public ActionStackUp(int _identity, enum_ActionRarity _level, float _maxStackup = -1) : base(_identity, _level) { m_maxStackup = _maxStackup; }
        protected float m_stackUp, m_maxStackup;
        public override void OnActivate()
        {
            base.OnActivate();
            m_stackUp = 0;
        }
        protected void ResetStack() => m_stackUp = 0;
        public void SetStackup(int stackAmount) => m_stackUp = stackAmount;
        public void OnStackUp(float stackAmount)
        {
            m_stackUp += stackAmount;
            if (m_maxStackup > 0 && m_stackUp > m_maxStackup)
                m_stackUp = m_maxStackup;
        }
    }
    public class ActionSingleBurstShotKill : ActionBase
    {
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        protected bool m_BurstShot => m_fireIdentity == -1;
        protected int m_fireIdentity = -1;
        public override void OnActivate()
        {
            base.OnActivate();
            ResetShotsCounter();
        }
        public void ResetShotsCounter()
        {
            SetDuration(0f);
            m_fireIdentity = -1;
        }

        public override void OnFire(int _identity)
        {
            base.OnFire(_identity);
            if (m_fireIdentity != -1)
                return;
            SetDuration(2f);
            m_fireIdentity = _identity;
        }
        public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnAfterDealtDemage(receiver, info, applyAmount);
            if (info.m_detail.I_IdentiyID == m_fireIdentity)
                OnShotsHit(receiver);
        }
        protected virtual void OnShotsHit(EntityCharacterBase _hitEntity) { if (_hitEntity.m_Health.b_IsDead) OnShotsKill(); }
        protected virtual void OnShotsKill() { }
        public ActionSingleBurstShotKill(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    #endregion
    #region Inherted Claseses
    #region 10000-19999
    public class Action_10001_ClipAdditive : ActionBase
    {
        public override int m_Index => 10001;
        public override int I_Cost => ActionData.I_10001_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10001_ClipMultiply(m_rarity);
        public override float F_ClipMultiply => Value1 / 100f;
        public override float F_Duration => ActionData.F_10001_Duration;
        public Action_10001_ClipAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10002_MoveArmorAdditive : ActionBase
    {
        public override int m_Index => 10002;
        public override int I_Cost => ActionData.I_10002_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10002_Duration;
        public override float Value1 => ActionData.F_10002_ArmorReceive(m_rarity);
        public override void OnMove(float amount)
        {
            if (amount > 0)
                ActionHelper.ReceiveHealing(m_ActionEntity, Value1 * amount, enum_DamageType.ArmorOnly);
        }
        public Action_10002_MoveArmorAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10003_MoveFirerateStackup : ActionStackUp
    {
        public override int m_Index => 10003;
        public override int I_Cost => ActionData.I_10003_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10003_Duration;
        public override float Value1 => ActionData.P_10003_FirerateMultiplyPerMile(m_rarity);
        public override float Value2 => ActionData.P_10003_FirerateMultiplyPerMile(m_rarity) * m_maxStackup;
        public override float m_FireRateMultiply => Value1/100f*m_stackUp;
        public override void OnMove(float distsance) => OnStackUp(distsance);
        public Action_10003_MoveFirerateStackup(int _identity, enum_ActionRarity _level) : base(_identity, _level,ActionData.I_10003_MaxStackAmount) { }
    }

    public class Action_10005_SingleShotMovementAdditive : ActionSingleBurstShotKill
    {
        public override int m_Index => 10005;
        public override int I_Cost => ActionData.I_10005_Cost;
        public override int m_EffectIndex => m_BurstShot ? 41002 : 0;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10005_OnFire_DamageMultiply(m_rarity);
        public override float Value2 => ActionData.P_10005_OnKill_Buff_MovementSpeed(m_rarity);
        public override float Value3 => ActionData.F_10005_OnKill_Buff_Duration;
        public override float m_DamageMultiply => m_BurstShot ? Value1/100f : 0f;
        protected override void OnShotsKill()=>ActionHelper.ReciveBuff(m_ActionEntity,SBuff.CreateMovementActionBuff(m_Index,40005,Value2/100f,Value3));
        public Action_10005_SingleShotMovementAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10006_SingleShotArmorAdditive : ActionSingleBurstShotKill
    {
        public override int m_Index => 10006;
        public override int I_Cost => ActionData.I_10006_Cost;
        public override int m_EffectIndex => m_BurstShot ? 41002 : 0;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10006_DamageMultiply(m_rarity);
        public override float Value2 => ActionData.I_10006_OnKill_ArmorReceive(m_rarity);
        public override float m_DamageMultiply => m_BurstShot ? Value1/100f : 0f;
        protected override void OnShotsKill() =>ActionHelper.ReceiveHealing(m_ActionEntity, Value2, enum_DamageType.ArmorOnly);
        public Action_10006_SingleShotArmorAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10007_SingleShotHealthAdditive : ActionSingleBurstShotKill
    {
        public override int m_Index => 10007;
        public override int I_Cost => ActionData.I_10007_Cost;
        public override int m_EffectIndex => m_BurstShot ? 41002 : 0;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10007_OnFire_DamageMultiply(m_rarity);
        public override float Value2 => ActionData.I_10007_OnKill_HealReceive(m_rarity);
        public override float m_DamageMultiply => m_BurstShot ? Value1/100f : 0f;
        protected override void OnShotsKill() => ActionHelper.ReceiveHealing(m_ActionEntity, Value2, enum_DamageType.HealthOnly);
        public Action_10007_SingleShotHealthAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10008_FirerateAdditiveMovementReduction : ActionBase
    {
        public override int m_Index => 10008;
        public override int I_Cost => ActionData.I_10008_Cost;
        public override int m_EffectIndex => 40010;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10008_Duration;
        public override float Value1 => ActionData.P_10008_FireRateAdditive(m_rarity);
        public override float Value2 => ActionData.P_10008_MovementReduction;
        public override float m_FireRateMultiply => Value1/100f;
        public override float m_MovementSpeedMultiply => -Value2/100f;
        public Action_10008_FirerateAdditiveMovementReduction(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10009_FirerateAdditive : ActionBase
    {
        public override int m_Index => 10009;
        public override int I_Cost => ActionData.I_10009_Cost;
        public override int m_EffectIndex => 40010;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10009_Duration;
        public override float Value1 => ActionData.P_10009_FireRateAdditive(m_rarity);
        public override float m_FireRateMultiply => Value1 / 100f;
        public Action_10009_FirerateAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10010_FirerateAdditiveDamageReduction : ActionBase
    {
        public override int m_Index => 10010;
        public override int I_Cost => ActionData.I_10010_Cost;
        public override int m_EffectIndex => 40010;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10010_Duration;
        public override float Value1 => ActionData.F_10010_DamageReduction;
        public override float Value2 => ActionData.P_10010_FireRateAdditive(m_rarity);
        public override float m_DamageAdditive => -Value1;
        public override float m_FireRateMultiply => Value2/100f;
        public Action_10010_FirerateAdditiveDamageReduction(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10011_HitStackFireRate : ActionStackUp
    {
        public override int m_Index => 10011;
        public override int I_Cost => ActionData.I_10011_Cost;
        public override int m_EffectIndex => 40010;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10011_Duration;
        public override float Value1 => ActionData.P_10011_FireRateAdditivePerHitStack(m_rarity);
        public override float m_FireRateMultiply => Value1/100f * m_stackUp;
        public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount) => OnStackUp(1);
        public Action_10011_HitStackFireRate(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10012_DamageAdditive : ActionBase
    {
        public override int m_Index => 10012;
        public override int I_Cost => ActionData.I_10012_Cost;
        public override int m_EffectIndex => 40011;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10012_Duration;
        public override float Value1 => ActionData.F_10012_DamageAdditive(m_rarity);
        public override float m_DamageAdditive => Value1;
        public Action_10012_DamageAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10013_CloakShotBurst : ActionSingleBurstShotKill
    {
        public override int m_Index => 10013;
        public override int I_Cost => ActionData.I_10013_Cost;
        public override int m_EffectIndex => m_BurstShot ? 41002 : 0;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.F_10013_CloakDuration(m_rarity);
        public override float Value2 => ActionData.P_10013_DamageMultiply(m_rarity);
        public override float m_DamageMultiply => m_BurstShot ? Value2/100f : 0;
        public override void OnActivate()
        {
            base.OnActivate();
            ActionHelper.ReceiveEffect(m_ActionEntity, enum_CharacterEffect.Cloak,Value1);
        }
        public Action_10013_CloakShotBurst(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10014_FreezeDamageApply : ActionBase
    {
        public override int m_Index => 10014;
        public override int I_Cost => ActionData.I_10014_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10014_Duration;
        public override float Value1 => ActionData.F_10014_HitFrozenDamageAdditive(m_rarity);
        public override void OnDealtDamageSetDamage(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnDealtDamageSetDamage(receiver, info);
            if (receiver.m_CharacterInfo.B_Effecting(enum_CharacterEffect.Freeze)||info.m_detail.m_DamageEffect== enum_CharacterEffect.Freeze)
                info.m_detail.DamageAdditive(0,Value1);
        }
        public Action_10014_FreezeDamageApply(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10015_FreezeNearby : ActionAfterUse
    {
        public override int m_Index => 10015;
        public override int I_Cost => ActionData.I_10015_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.F_10015_FrozeDuration(m_rarity);
        public override void OnActivate()
        {
            base.OnActivate();
            EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex( m_Index), m_ActionEntity, () => { return DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1); }).Play(false,m_ActionEntity);
        }
        public Action_10015_FreezeNearby(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10016_DamageAllFreezing : ActionAfterUse
    {
        public override int m_Index => 10016;
        public override int I_Cost => ActionData.I_10016_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10016_FrozenDirectDamageMultiply(m_rarity);

        public override void OnActivate()
        {
            base.OnActivate();
             GameManager.Instance.GetEntities(m_ActionEntity.m_Flag, false).Traversal((EntityCharacterBase entity)=> {
                 if (entity.m_CharacterInfo.B_Effecting(enum_CharacterEffect.Freeze))
                 {
                     entity.m_HitCheck.TryHit(new DamageInfo(Value1 / 100f * m_ActionEntity.m_WeaponCurrent.F_BaseDamage, enum_DamageType.Basic, DamageDeliverInfo.Default(m_ActionEntity.m_EntityID)));
                     GameObjectManager.SpawnSFX<SFXMuzzle>(10014,entity.transform.position,Vector3.up).Play(m_ActionEntity.m_EntityID);
                 }
             });
        }
        public Action_10016_DamageAllFreezing(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10018_HealthSteal : ActionBase
    {
        public override int m_Index => 10018;
        public override int I_Cost => ActionData.I_10018_Cost;
        public override int m_EffectIndex => 40006;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10018_Duration;
        public override float Value1 => ActionData.F_10018_PerStackHealthLoss;
        public override float Value2 => ActionData.P_10018_HealthStealPerStack(m_rarity);
        public override float m_HealthDrainMultiply => Value2 / 100f * (m_ActionEntity.m_Health.m_MaxHealth - m_ActionEntity.m_Health.m_CurrentHealth) / Value1;
        public Action_10018_HealthSteal(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    
    public class Action_10020_HealthLossSpeedUp : ActionBase
    {
        public override int m_Index => 10020;
        public override int I_Cost => ActionData.I_10020_Cost;
        public override int m_EffectIndex => 40005;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10020_Duration;
        public override float Value1 => ActionData.F_10020_PerStackHealthLoss;
        public override float Value2 => ActionData.P_10020_MovementAdditivePerStack(m_rarity);
        public override float m_MovementSpeedMultiply => healthLossStack * Value2/100f;
        float healthLossStack;
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            healthLossStack = (m_ActionEntity.m_Health.m_MaxHealth - m_ActionEntity.m_Health.m_CurrentHealth) / Value1;
        }
        public Action_10020_HealthLossSpeedUp(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10021_HealthRegen : ActionBase
    {
        public override int m_Index => 10021;
        public override int I_Cost => ActionData.I_10021_Cost;
        public override int m_EffectIndex => 40001;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_20021_Duration;
        public override float Value1 => ActionData.F_10021_HealthRegenPerMin(m_rarity);
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            ActionHelper.ReceiveHealing(m_ActionEntity, Value1 * deltaTime, enum_DamageType.HealthOnly);
        }
        public override void OnAfterReceiveDamage(DamageInfo info, float amount)
        {
            base.OnAfterReceiveDamage(info, amount);
            ForceExpire();
        }
        public Action_10021_HealthRegen(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10022_HealthRegenFromDamage : ActionBase
    {
        public override int m_Index => 10022;
        public override int I_Cost => ActionData.I_10022_Cost;
        public override int m_EffectIndex => 40003;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10022_Duration(m_rarity);
        public override float Value1 => ActionData.P_10022_HealthRegenTranslateFromDamage;
        public override float m_DamageReduction => 1f;
        public override void OnBeforeReceiveDamage(DamageInfo info)
        {
            base.OnBeforeReceiveDamage(info);
            ActionHelper.ReceiveHealing(m_ActionEntity, info.m_AmountApply, enum_DamageType.HealthOnly);
        }
        public Action_10022_HealthRegenFromDamage(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10023_Revive : ActionRevive
    {
        public override int m_Index => 10023;
        public override int I_Cost => ActionData.I_10023_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.F_10023_ReviveHealthRegen(m_rarity);
        protected override RangeFloat m_ReviveAmount => new RangeFloat(Value1, 0f);
        public Action_10023_Revive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10024_ArmorAdditive:ActionAfterUse
    {
        public override int m_Index => 10024;
        public override int I_Cost => ActionData.I_10024_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.F_10024_ArmorAdditive(m_rarity);
        public override void OnActivate()
        {
            base.OnActivate();
            ActionHelper.ReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
        }
        public Action_10024_ArmorAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_10025_ArmorAdditiveMultiply : ActionAfterUse
    {
        public override int m_Index => 10025;
        public override int I_Cost => ActionData.I_10025_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10025_ArmorAdditiveMultiply(m_rarity);
        public override void OnActivate()
        {
            base.OnActivate();
            ActionHelper.ReceiveHealing(m_ActionEntity, Value1/100f * m_ActionEntity.m_Health.m_CurrentArmor, enum_DamageType.ArmorOnly);
        }
        public Action_10025_ArmorAdditiveMultiply(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_10026_DamageAdditiveArmorMultiply : ActionBase
    {
        public override int m_Index => 10026;
        public override int I_Cost => ActionData.I_10026_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10026_Duration;
        public override float Value1 => ActionData.F_10026_DamageAdditiveMultiplyWithArmor(m_rarity);
        public override float m_DamageAdditive=> Value1 * m_ActionEntity.m_Health.m_CurrentArmor; 
        public Action_10026_DamageAdditiveArmorMultiply(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10027_HealthDamageArmorAdditive:ActionAfterUse
    {
        public override int m_Index => 10027;
        public override int I_Cost => ActionData.I_10027_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.F_10027_HealthDamage(m_rarity);
        public override float Value2 => ActionData.F_10027_ArmorAdditive(m_rarity);
        public override void OnActivate()
        {
            base.OnActivate();
            ActionHelper.ReceiveDamage(m_ActionEntity,Value1, enum_DamageType.HealthOnly);
            ActionHelper.ReceiveHealing(m_ActionEntity, Value2, enum_DamageType.ArmorOnly);
        }
        public Action_10027_HealthDamageArmorAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10028_ArmorSingleShot : ActionSingleBurstShotKill
    {
        public override int m_Index => 10028;
        public override int I_Cost => ActionData.I_10028_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10028_NextShotDamageMultiplyPerArmor(m_rarity);
        public override float m_DamageMultiply => m_BurstShot ? Value1 * m_ActionEntity.m_Health.m_CurrentArmor / 100f : 0;
        public Action_10028_ArmorSingleShot(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10029_ClipAdditive : ActionBase
    {
        public override int m_Index => 10029;
        public override int I_Cost => ActionData.I_10029_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10029_Duration;
        public override float Value1 => ActionData.I_10029_ClipAdditive(m_rarity);
        public override int I_ClipAdditive => (int)Value1;
        public Action_10029_ClipAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10030_MovementAdditive : ActionBase
    {
        public override int m_Index => 10030;
        public override int I_Cost => ActionData.I_10030_Cost;
        public override int m_EffectIndex => 40005;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10030_Duration;
        public override float Value1 => ActionData.P_10030_MovementSpeedAdditive(m_rarity);
        public override float m_MovementSpeedMultiply => Value1/100f;
        public Action_10030_MovementAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    
    public class Action_10032_RecoilReduction : ActionBase
    {
        public override int m_Index => 10032;
        public override int I_Cost => ActionData.I_10032_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10032_Duration(m_rarity);
        public override float F_RecoilReduction => 1f;
        public Action_10032_RecoilReduction(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10033_AimStrictReduction : ActionBase
    {
        public override int m_Index => 10033;
        public override int I_Cost => ActionData.I_10033_Cost;
        public override int m_EffectIndex => 40005;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float F_Duration => ActionData.F_10033_Duration(m_rarity);
        public override float F_AimStrictReduction => 1f;
        public Action_10033_AimStrictReduction(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10034_ShotsKillTwice : ActionSingleBurstShotKill
    {
        public override int m_Index => 10034;
        public override int I_Cost => ActionData.I_10034_Cost;
        public override int m_EffectIndex => m_BurstShot ? 41002 : 0;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10034_DamageAdditiveFirstShot(m_rarity);
        public override float Value2 => ActionData.P_10034_DamageAdditiveShotAfterKill(m_rarity);
        public override float m_DamageMultiply 
        {
            get
            {
                if(m_BurstShot)
                    switch (killCount)
                    {
                        case 0:return Value1/100f;
                        case 1:return Value2/100f;
                    }
                return 0;
            }
        }
        int killCount=0;
        public override void OnActivate()
        {
            base.OnActivate();
            killCount = 0;
        }
        protected override void OnShotsKill()
        {
            base.OnShotsKill();
            killCount++;
            if (killCount == 1)
                ResetShotsCounter();
        }
        public Action_10034_ShotsKillTwice(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_10035_ImmediateReloadShotsBurst : ActionBase
    {
        public override int m_Index => 10035;
        public override int I_Cost => ActionData.I_10035_Cost;
        public override int m_EffectIndex => 41002;
        public override enum_ActionType m_ActionType => enum_ActionType.Basic;
        public override float Value1 => ActionData.P_10035_DamageAdditiveNextShot(m_rarity);
        public override float m_DamageMultiply =>  Value1 / 100f ;
        public override void OnActivate()
        {
            base.OnActivate();
            m_ActionEntity.m_WeaponCurrent.ForceReload();
        }
        public override void OnFire(int identity)
        {
            base.OnFire(identity);
            ForceExpire();
        }
        public Action_10035_ImmediateReloadShotsBurst(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    #endregion
    #region 20000-29999
    public class Action_20001_RustShot : ActionDevice
    {
        public override int m_Index => 20001;
        public override int I_Cost => ActionData.I_20001_Cost;
        public override float Value1 => ActionData.F_20001_RustDamageDuration(m_rarity);
        public override float Value2 => ActionData.F_20001_RustDamagePerSecond(m_rarity);
        public override EquipmentBase GetDevice()=>ActionHelper.GetCommonDevice(m_Index, m_ActionEntity, () => DamageDeliverInfo.BuffInfo(m_ActionEntity.m_EntityID, SBuff.CreateActionDOTBuff(m_Index, Value1, 1f, Value2, enum_DamageType.Basic)));
        public Action_20001_RustShot(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20002_MarkShotArmorAdditive : ActionDevice
    {
        public override int m_Index => 20002;
        public override int I_Cost => ActionData.I_20002_Cost;
        public override float Value1 => ActionData.F_20002_ArmorAdditiveTargetDead(m_rarity);
        public override EquipmentBase GetDevice() => ActionHelper.GetCommonDevice(m_Index, m_ActionEntity, () => DamageDeliverInfo.DamageHitInfo(m_ActionEntity.m_EntityID, OnHitEntitiy));
        void OnHitEntitiy(EntityBase receiver)
        {
            GameObjectManager.SpawnSFX<SFXMarkup>(60001, receiver.transform.position, Vector3.up).Play(m_ActionEntity.m_EntityID, receiver, OnMarkupDead);
        }
        void OnMarkupDead()
        {
            ActionHelper.ReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
        }
        public Action_20002_MarkShotArmorAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20003_FreezeShot : ActionDevice
    {
        public override int m_Index => 20003;
        public override int I_Cost => ActionData.I_20003_Cost;
        public override float Value1 => ActionData.F_20003_FreezeDuration(m_rarity);
        public override EquipmentBase GetDevice() => ActionHelper.GetCommonDevice(m_Index,m_ActionEntity, () => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1));
        public Action_20003_FreezeShot(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20004_FreezeGrenade : ActionDevice
    {
        public override int m_Index => 20004;
        public override int I_Cost => ActionData.I_20004_Cost;
        public override float Value1 => ActionData.F_20004_FreezeDuration(m_rarity);
        public override EquipmentBase GetDevice()=> ActionHelper.GetCommonDevice(m_Index, m_ActionEntity, () => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1));
        public Action_20004_FreezeGrenade(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20005_FreezeTurret : ActionDevice
    {
        public override int m_Index => 20005;
        public override int I_Cost => ActionData.I_20005_Cost;
        public override float Value1 => ActionData.F_20005_FreezeDuration(m_rarity);
        public override float Value2 => ActionData.F_20005_Health(m_rarity);
        public override float Value3 => ActionData.F_20005_Damage(m_rarity);
        public override EquipmentBase GetDevice() => ActionHelper.GetEntityDevice(m_Index, m_ActionEntity, () => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, Value3, enum_CharacterEffect.Freeze, Value1), (int)Value2, 1);
        public Action_20005_FreezeTurret(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20006_FreezeMine : ActionDevice
    {
        public override int m_Index => 20006;
        public override int I_Cost => ActionData.I_20006_Cost;
        public override float Value1 => ActionData.F_20006_FreezeDuration(m_rarity);
        public override EquipmentBase GetDevice() => ActionHelper.GetCommonDevice(m_Index, m_ActionEntity, () => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1));

        public Action_20006_FreezeMine(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20007_TeleportDevice : ActionDevice
    {
        public override int m_Index => 20007;
        public override int I_Cost => ActionData.I_20007_Cost(m_rarity);
        public override float Value1 => ActionData.F_20007_Distance(m_rarity);
        protected override float m_DeviceDistance => Value1;
        public override EquipmentBase GetDevice() => ActionHelper.GetCommonDevice(m_Index, m_ActionEntity, () => DamageDeliverInfo.Default(m_ActionEntity.m_EntityID));
        public Action_20007_TeleportDevice(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20008_MinigunTurret : ActionDevice
    {
        public override int m_Index => 20008;
        public override int I_Cost => ActionData.I_20008_Cost;
        public override float Value1 => ActionData.F_20008_Health(m_rarity);
        public override float Value2 => ActionData.F_20008_Damage(m_rarity);
        public override float Value3 => ActionData.F_20008_FireRate;
        public override EquipmentBase GetDevice()=>ActionHelper.GetEntityDevice( m_Index, m_ActionEntity, () => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, Value2, enum_CharacterEffect.Invalid, 0), (int)Value1, Value3); 
        public Action_20008_MinigunTurret(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20009_BlastTurret : ActionDevice
    {
        public override int m_Index => 20009;
        public override int I_Cost => ActionData.I_20009_Cost;
        public override float Value1 => ActionData.F_20009_Health(m_rarity);
        public override float Value2 => ActionData.F_20009_Damage(m_rarity);
        public override float Value3 => ActionData.F_20009_FireRate;
        public override EquipmentBase GetDevice() => ActionHelper.GetEntityDevice(m_Index, m_ActionEntity, () => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, Value2, enum_CharacterEffect.Invalid, 0), (int)Value1, Value3);
        public Action_20009_BlastTurret(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20010_HealthDrainDevice : ActionDevice
    {
        public override int m_Index => 20010;
        public override int I_Cost => ActionData.I_20010_Cost;
        public override float Value1 => ActionData.F_20010_Health(m_rarity);
        public override float Value2 => ActionData.P_20010_PlayerHealthDrain(m_rarity);
        public override float Value3 => ActionData.P_20010_AIHealthDrain(m_rarity);
        public override EquipmentBase GetDevice()=>ActionHelper.GetBuffDevice(m_Index,m_ActionEntity,Value1, SBuff.CreateActionHealthDrainBuff(m_Index, Value2 / 100f, .5f), SBuff.CreateActionHealthDrainBuff(m_Index, (Value2 + Value3) / 100f, .5f), .4f);
        public Action_20010_HealthDrainDevice(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_20012_HealthRegenDevice : ActionDevice
    {
        public override int m_Index => 20012;
        public override int I_Cost => ActionData.I_20012_Cost;
        public override float Value1 => ActionData.F_20012_Health(m_rarity);
        public override float Value2 => ActionData.F_20012_PlayerHealthRegen(m_rarity);
        public override float Value3 => ActionData.F_20012_AIHealthRegen(m_rarity);
        public override EquipmentBase GetDevice() => ActionHelper.GetBuffDevice(m_Index, m_ActionEntity, Value1, SBuff.CreateActionHealthBuff(m_Index, Value2, 1f, .5f), SBuff.CreateActionHealthBuff(m_Index, Value2 + Value3, 1f, .5f), .4f);
        public Action_20012_HealthRegenDevice(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_20013_Grenade:ActionDevice
    {
        public override int m_Index => 20013;
        public override int I_Cost => ActionData.I_20013_Cost;
        public override float Value1 => ActionData.P_20013_DamageMultiplyBase(m_rarity);
        public override EquipmentBase GetDevice() => ActionHelper.GetCommonDevice(m_Index,m_ActionEntity, () => DamageDeliverInfo.DamageInfo(m_ActionEntity.m_EntityID, 0, Value1 / 100f * m_ActionEntity.m_WeaponCurrent.F_BaseDamage));
        public Action_20013_Grenade(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    #endregion
    #region 30000-39999
    public class Action_30001_DamageMovementStackup : ActionStackUp
    {
        public override int m_Index => 30001;
        public override int I_Cost => ActionData.I_30001_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.F_30001_DamagePerStack(m_rarity);
        public override float Value2 => Value1*ActionData.I_30001_MaxStack;
        public override float m_DamageAdditive => Value1 * m_stackUp;

        public override void OnFire(int identity)
        {
            base.OnFire(identity);
            ResetStack();
        }
        public override void OnMove(float distance) => OnStackUp(distance);
        public Action_30001_DamageMovementStackup(int _identity, enum_ActionRarity _level) : base(_identity, _level, ActionData.I_30001_MaxStack) { }
    }
    public class Action_30002_KillDamageMultiply : ActionBase
    {
        public override int m_Index => 30002;
        public override int I_Cost => ActionData.I_30002_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
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
            m_burstShot = receiver.m_Health.b_IsDead;
        }
        public Action_30002_KillDamageMultiply(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_30003_GrenadeLauncher : ActionBase
    {
        public override int m_Index => 30003;
        public override int I_Cost => ActionData.I_30003_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.F_30003_GrenadeDamage(m_rarity);
        EquipmentBase m_ActionEquipment;
        DamageDeliverInfo GetDamageInfo()=> DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, Value1, enum_CharacterEffect.Invalid, 0); 
        public override void OnActivate()
        {
            base.OnActivate();
            m_ActionEquipment = EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex(m_Index), m_ActionEntity, GetDamageInfo);
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
                m_ActionEquipment.Play( m_ActionEntity,m_ActionEntity.tf_Head.position+m_ActionEntity.tf_Head.forward*5);

            m_shotGrenade = false;
        }
        public Action_30003_GrenadeLauncher(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_30004_ReloadSpeedAdditive : ActionBase
    {
        public override int m_Index => 30004;
        public override int I_Cost => ActionData.I_30004_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.P_30004_ReloadSpeedMultiply(m_rarity);
        public override float m_ReloadRateMultiply =>Value1/100f;
        public Action_30004_ReloadSpeedAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    public class Action_30005_DamageFreeze : ActionBase
    {
        public override int m_Index => 30005;
        public override int I_Cost => ActionData.I_30005_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override int m_EffectIndex => 41001;
        public override float Value1 => ActionData.F_30005_FreezeDuration(m_rarity);
        public override void OnDealtDamageSetEffect(EntityCharacterBase receiver, DamageInfo info)
        {
            base.OnDealtDamageSetEffect(receiver, info);
            info.m_detail.EffectAdditiveOverride(enum_CharacterEffect.Freeze, Value1);
        }
        public Action_30005_DamageFreeze(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30006_DamageKillFreezeBlast : ActionBase
    {
        public override int m_Index => 30006;
        public override int I_Cost => ActionData.I_30006_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.F_30006_FreezeDuration(m_rarity);
        EquipmentBase m_Equipment;
        DamageDeliverInfo GetDamageInfo() => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1);
        public override void OnActivate()
        {
            base.OnActivate();
            m_Equipment = EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex(m_Index), m_ActionEntity, GetDamageInfo);
        }
        public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnAfterDealtDemage(receiver, info, applyAmount);
            if (!receiver.m_Health.b_IsDead)
                return;

            m_Equipment.Play(false, receiver);
        }

        public Action_30006_DamageKillFreezeBlast(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30007_DamageLimitFreeze : ActionBase
    {
        public override int m_Index => 30007;
        public override int I_Cost => ActionData.I_30007_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
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
        public Action_30007_DamageLimitFreeze(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30008_FreezeTrapSpawnByActionUse : ActionBase
    {
        public override int m_Index => 30008;
        public override int I_Cost => ActionData.I_30008_Cost(m_rarity);
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.F_30008_FreezeDuration(m_rarity);
        EquipmentBase m_Equipment;
        DamageDeliverInfo GetDamageInfo() => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID,0, enum_CharacterEffect.Freeze,Value1);
        public override void OnActivate()
        {
            base.OnActivate();
            m_Equipment = EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex(m_Index), m_ActionEntity, GetDamageInfo);
        }
        public override void OnUseActionElse(ActionBase targetAction)
        {
            base.OnUseActionElse(targetAction);
            m_Equipment.Play(false,m_ActionEntity);
        }
        public Action_30008_FreezeTrapSpawnByActionUse(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30009_AllyActivateHealthAdditive : ActionBase
    {
        public override int m_Index => 30009;
        public override int I_Cost => ActionData.I_30009_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.P_30009_EquipmentHealthAddup(m_rarity);
        public override float F_AllyHealthMultiplierAdditive => Value1/100f;
        public Action_30009_AllyActivateHealthAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30010_UseActionAddMaxHealth : ActionBase
    {
        public override int m_Index => 30010;
        public override int I_Cost => ActionData.I_30010_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.F_30010_MaxHealthAddup(m_rarity);
        public override void OnUseActionElse(ActionBase targetAction)
        {
            base.OnUseActionElse(targetAction);
            m_ActionEntity.m_Health.AddMaxHealth(Value1);
        }
        public Action_30010_UseActionAddMaxHealth(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30011_ReceiveHealingAddMaxHealth : ActionBase
    {
        public override int m_Index => 30011;
        public override int I_Cost => ActionData.I_30011_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.P_30011_MaxHealthRegen(m_rarity);
        public override void OnReceiveHealing(DamageInfo info, float amount)
        {
            base.OnReceiveHealing(info, amount);
            if(info.m_Type== enum_DamageType.HealthOnly)
                 m_ActionEntity.m_Health.AddMaxHealth (-info.m_AmountApply*Value1/100f);
        }
        public Action_30011_ReceiveHealingAddMaxHealth(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30012_KillHealthRegen : ActionBase
    {
        public override int m_Index => 30012;
        public override int I_Cost => ActionData.I_30012_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.P_30012_HealthRegenEachKill(m_rarity);
        public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnAfterDealtDemage(receiver, info, applyAmount);
            if(receiver.m_Health.b_IsDead)
                ActionHelper.ReceiveHealing(m_ActionEntity,m_ActionEntity.m_Health.m_MaxHealth*Value1/100f, enum_DamageType.HealthOnly);
        }
        public Action_30012_KillHealthRegen(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30013_HealRegenAdditive : ActionBase
    {
        public override int m_Index => 30013;
        public override int I_Cost => ActionData.I_30013_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.P_30013_HealthRegenAdditive(m_rarity);
        public override float m_HealAdditive => Value1 / 100f;
        public Action_30013_HealRegenAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30014_ArmorDamageDamageReductionDuration : ActionBase
    {
        public override int m_Index => 30014;
        public override int I_Cost => ActionData.I_30014_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
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
        public Action_30014_ArmorDamageDamageReductionDuration(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30015_ArmorDamageReflection : ActionBase
    {
        public override int m_Index => 30015;
        public override int I_Cost => ActionData.I_30015_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override int m_EffectIndex => 40007;
        public override float Value1 => ActionData.F_30015_DamageReflectPerArmor(m_rarity);
        public override void OnBeforeReceiveDamage(DamageInfo info)
        {
            base.OnBeforeReceiveDamage(info);
            if (m_ActionEntity.m_Health.m_CurrentArmor > 0)
                ActionHelper.PlayerDealtDamageToEntity(m_ActionEntity, info.m_detail.I_SourceID, Value1 * m_ActionEntity.m_Health.m_CurrentArmor, enum_DamageType.Basic);
        }
        public Action_30015_ArmorDamageReflection(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30016_ProjectileSpeed : ActionBase
    {
        public override int m_Index => 30016;
        public override int I_Cost => ActionData.I_30016_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.P_30016_ProjectileSpeedMultiply(m_rarity);
        public override float F_ProjectileSpeedMultiply => Value1/100f;
        public Action_30016_ProjectileSpeed(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30017_ArmorAdditiveActionUse : ActionBase
    {
        public override int m_Index => 30017;
        public override int I_Cost => ActionData.I_30017_Cost;
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override float Value1 => ActionData.F_30017_ArmorAdditive(m_rarity);
        public override void OnUseActionElse(ActionBase targetAction)
        {
            base.OnUseActionElse(targetAction);
            ActionHelper.ReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
        }
        public Action_30017_ArmorAdditiveActionUse(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }

    public class Action_30018_ProjectilePenetrate : ActionBase
    {
        public override int m_Index => 30018;
        public override int I_Cost => ActionData.I_30018_Cost(m_rarity);
        public override enum_ActionType m_ActionType => enum_ActionType.Equipment;
        public override bool B_ProjectilePenetrade => true;
        public Action_30018_ProjectilePenetrate(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    }
    #endregion
    //#region 40000-49999 Unused
    //public class Action_40001_UseActionArmorAdditive : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40001;
    //    public override float Value1 => ActionData.F_40001_ArmorAdditive(m_rarity);
    //    public override void OnUseActionElse(ActionBase targetAction)
    //    {
    //        base.OnUseActionElse(targetAction);
    //        ActionHelper.ReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
    //    }
    //    public Action_40001_UseActionArmorAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40002_MaxHealthAdditive : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40002;
    //    public override float Value1 => ActionData.F_40002_MaxHealthAdd(m_rarity);
    //    public override float m_MaxHealthAdditive => Value1;
    //    public Action_40002_MaxHealthAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40003_AllyDamageMultiply : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40003;
    //    public override float Value1 => ActionData.F_40003_Range(m_rarity);
    //    public override float Value2 => ActionData.P_40003_PlayerDamageMultiply(m_rarity);
    //    public override float Value3 => ActionData.P_40003_AllyDamageMultiply(m_rarity);
    //    public override float m_DamageMultiply => Value2/100f;
    //    DamageDeliverInfo allyInfo => DamageDeliverInfo.BuffInfo(m_ActionEntity.m_EntityID, SBuff.CreateActionDamageMultiplyBuff(m_Index, (Value2+Value3)/100f, .5f));
    //    float f_buffCheck;
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        if (f_buffCheck > 0f)
    //        {
    //            f_buffCheck -= deltaTime;
    //            return;
    //        }
    //        f_buffCheck = .4f;

    //        List<EntityCharacterBase> allies= GameManager.Instance.GetEntities(m_ActionEntity.m_Flag, true);
    //        for (int i = 0; i < allies.Count; i++)
    //        {
    //            if (allies[i].m_EntityID == m_ActionEntity.m_EntityID)
    //                continue ;

    //            if (Vector3.Distance(m_ActionEntity.transform.position, allies[i].transform.position) < Value1)
    //                allies[i].m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, allyInfo));
    //        }
    //    }
    //    public Action_40003_AllyDamageMultiply(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40004_MovementSpeedMultiply : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40004;
    //    public override float Value1 => ActionData.P_40004_MovementSpeedAdditive(m_rarity);
    //    public override float m_MovementSpeedMultiply => Value1 / 100f;
    //    public Action_40004_MovementSpeedMultiply(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40005_ClipRefill : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40005;
    //    public override float Value1 => ActionData.P_40005_FastReloadRate(m_rarity);
    //    public override void OnFire(int identity)
    //    {
    //        base.OnFire(identity);
    //        if (TCommon.RandomPercentage() < Value1)
    //            m_ActionEntity.m_WeaponCurrent.ForceReload();
    //    }
    //    public Action_40005_ClipRefill(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40006_ClipAdditive : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40006;
    //    public override float Value1 => ActionData.I_40006_ClipAdditive(m_rarity);
    //    public override int I_ClipAdditive => ActionData.I_40006_ClipAdditive(m_rarity);
    //    public Action_40006_ClipAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40007_DamageAdditive : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40007;
    //    public override float Value1 => ActionData.F_40007_DamageAdditive(m_rarity);
    //    public override float m_DamageAdditive => Value1;
    //    public Action_40007_DamageAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40008_EquipmentHealthAdditive : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40008;
    //    public override float Value1 => ActionData.P_40008_MaxHealthAdditive(m_rarity);
    //    public override float F_AllyHealthMultiplierAdditive => Value1 / 100f;
    //    public Action_40008_EquipmentHealthAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40009_ReloadDamageMultiply : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40009;
    //    public override float Value1 => ActionData.F_40009_DamageMultiplyAfterRloead(m_rarity);
    //    public override float m_DamageMultiply => damageShot ? Value1/100f : 0;
    //    bool damageShot = false;
    //    public override void OnReloadFinish()
    //    {
    //        base.OnReloadFinish();
    //        damageShot = true;
    //    }
    //    public override void OnFire(int identity)
    //    {
    //        base.OnFire(identity);
    //        damageShot = false;
    //    }
    //    public Action_40009_ReloadDamageMultiply(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40010_EnrangeExtraDamage : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40010;
    //    public override float Value1 => ActionData.F_40010_Range(m_rarity);
    //    public override float Value2 => ActionData.P_40010_DamageMultiply(m_rarity);
    //    public override void OnDealtDamageSetDamage(EntityCharacterBase receiver, DamageInfo info)
    //    {
    //        base.OnDealtDamageSetDamage(receiver, info);
    //        if (Vector3.Distance(receiver.transform.position, m_ActionEntity.transform.position) < Value1)
    //            info.m_detail.DamageAdditive(0, Value1 / 100f);
    //    }
    //    public Action_40010_EnrangeExtraDamage(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40011_ReloadRateMultiply : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40011;
    //    public override float Value1 => ActionData.P_40011_ReloadMultiply(m_rarity);
    //    public override float m_ReloadRateMultiply => Value1 / 100f;
    //    public Action_40011_ReloadRateMultiply(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40012_ReceiveDamageMovementMultiplyDuration : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40012;
    //    public override int m_EffectIndex => m_effecting ? 40005 : 0;
    //    public override float Value1 => ActionData.P_40012_MovementSpeedMultiply(m_rarity);
    //    public override float Value2 => ActionData.F_40012_Duration(m_rarity);
    //    public override float m_MovementSpeedMultiply => m_effecting? Value1 / 100f : 0;
    //    bool m_effecting => f_durationCheck > 0f;
    //    float f_durationCheck;
    //    public override void OnAfterReceiveDamage(DamageInfo info, float amount)
    //    {
    //        base.OnAfterReceiveDamage(info, amount);
    //        f_durationCheck = Value2;
    //    }
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        if (f_durationCheck > 0f)
    //            f_durationCheck -= deltaTime;
    //    }
    //    public Action_40012_ReceiveDamageMovementMultiplyDuration(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40013_KillMovementMultiplyDuration : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40013;
    //    public override int m_EffectIndex => m_effecting ? 40005 : 0;
    //    public override float Value1 => ActionData.P_40013_MovementSpeedMultiply(m_rarity);
    //    public override float Value2 => ActionData.F_40013_Duration(m_rarity);
    //    public override float m_MovementSpeedMultiply =>m_effecting ? Value1 / 100f : 0;
    //    bool m_effecting => f_durationCheck > 0f;
    //    float f_durationCheck=0f;
    //    public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
    //    {
    //        base.OnAfterDealtDemage(receiver, info, applyAmount);
    //        if(receiver.m_Health.b_IsDead)
    //            f_durationCheck = Value2;
    //    }
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        if (f_durationCheck > 0f)
    //            f_durationCheck -= deltaTime;
    //    }
    //    public Action_40013_KillMovementMultiplyDuration(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40014_RecoilReduction : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40014;
    //    public override float Value1 => ActionData.P_40014_RecoilReduction(m_rarity);
    //    public override float F_RecoilReduction => Value1 / 100f;
    //    public Action_40014_RecoilReduction(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40015_EnrangeExtraHealthDrain : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40015;
    //    public override float Value1 => ActionData.F_40015_Range(m_rarity);
    //    public override float Value2 => ActionData.P_40015_HealthDrain(m_rarity);
    //    public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
    //    {
    //        base.OnAfterDealtDemage(receiver, info, applyAmount);
    //        if (Vector3.Distance(receiver.transform.position, m_ActionEntity.transform.position) < Value1)
    //            ActionHelper.ReceiveHealing(m_ActionEntity, Value2 / 100f * applyAmount, enum_DamageType.HealthOnly);
    //    }
    //    public Action_40015_EnrangeExtraHealthDrain(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}
    
    //public class Action_40017_ArmorDamageReduction : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40017;
    //    public override float Value1 => ActionData.P_40017_DamageReduction(m_rarity);
    //    public override float m_DamageReduction => m_ActionEntity.m_Health.m_CurrentArmor > 0 ? Value1/100f : 0;
    //    public Action_40017_ArmorDamageReduction(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40018_KillDamageAdditiveBurstShot : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40018;
    //    public override float Value1 => ActionData.P_40018_DamageMultiply(m_rarity);
    //    public override float m_DamageMultiply => burstShot ? Value1 / 100f : 0f;
    //    bool burstShot = false;
    //    public override void OnFire(int identity)
    //    {
    //        base.OnFire(identity);
    //        burstShot = false;
    //    }
    //    public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
    //    {
    //        base.OnAfterDealtDemage(receiver, info, applyAmount);
    //        burstShot = receiver.m_Health.b_IsDead;
    //    }
    //    public Action_40018_KillDamageAdditiveBurstShot(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}
    
    //public class Action_40020_KillFreezeGrenade : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40020;
    //    public override float Value1 => ActionData.F_40020_FreezeDuration(m_rarity);
    //    DamageDeliverInfo GetDamageInfo() => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1);
    //    public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
    //    {
    //        base.OnAfterDealtDemage(receiver, info, applyAmount);
    //        if (receiver.m_Health.b_IsDead)
    //            EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex(m_Index),receiver,GetDamageInfo).Play(receiver,receiver.tf_Model.position+TCommon.RandomXZSphere(.5f));
    //    }
    //    public Action_40020_KillFreezeGrenade(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40021_KillFreeze : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40021;
    //    public override float Value1 => ActionData.F_40021_FreezeDuration(m_rarity);
    //    int m_fireIdentity = -1;
    //    bool identitySet = false;
    //    public override void OnFire(int identity)
    //    {
    //        base.OnFire(identity);
    //        if (identitySet)
    //        {
    //            m_fireIdentity = identity;
    //            identitySet = false;
    //        }
    //    }
    //    public override void OnAfterDealtDemage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
    //    {
    //        base.OnAfterDealtDemage(receiver, info, applyAmount);
    //        if (receiver.m_Health.b_IsDead)
    //        {
    //            identitySet = true;
    //            return;
    //        }

    //        if (info.m_detail.I_IdentiyID == m_fireIdentity)
    //        {
    //            receiver.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.EquipmentInfo(m_ActionEntity.m_EntityID, 0, enum_CharacterEffect.Freeze, Value1)));
    //            m_fireIdentity = -1;
    //        }
    //    }
    //    public Action_40021_KillFreeze(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }
    //}

    //public class Action_40022_HealAdditive : ActionWeaponPerkNormal
    //{
    //    public override int m_Index => 40022;
    //    public override float Value1 => ActionData.P_40022_HealAdditive(m_rarity);
    //    public override float m_HealAdditive => Value1 / 100f;
    //    public Action_40022_HealAdditive(int _identity, enum_ActionRarity _level) : base(_identity, _level) { }

    //}
    //#endregion
    #endregion
    #endregion
}