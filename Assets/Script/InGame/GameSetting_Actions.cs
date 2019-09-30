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
        public const int I_10001_Cost=1;
        public const float F_10001_Duration = 30f;
        public static int IP_10001_ClipMultiply(enum_RarityLevel rarity) => 25 + 25 * (int)rarity;

        public const int I_10002_Cost = 1;
        public const float F_10002_Duration = 20f;
        public static float F_10002_ArmorReceive(enum_RarityLevel rarity) => 1.2f * (int)rarity;

        public const int I_10003_Cost = 2;
        public const float F_10003_Duration = 20f;
        public static float F_10003_FirerateMultiplyPerMile(enum_RarityLevel rarity) => .06f+.06f * ((int)rarity);
        public const int I_10003_MaxStackAmount = 100;

        public const int I_10004_Cost = 2;
        public static float F_10004_Duration = 10f;
        public static float F_10004_EnergyAdditivePerMile(enum_RarityLevel rarity) => .06f + .01f * (int)rarity;

        public const int I_10005_Cost = 2;
        public static int IP_10005_OnFire_DamageMultiply(enum_RarityLevel rarity) => 80 + 80 * (int)rarity;
        public static int IP_10006_OnKill_Buff_MovementSpeed(enum_RarityLevel rarity) => 30 + 30 * (int)rarity;
        public static float F_10005_OnKill_Buff_Duration = 5f;

        public const int I_10006_Cost = 2;
        public static int IP_10006_DamageMultiply(enum_RarityLevel rarity) => 80 + 80 * (int)rarity;
        public static int I_10006_OnKill_ArmorReceive(enum_RarityLevel rarity) => 10 + 30 * (int)rarity;

        public const int I_10007_Cost = 2;
        public static int IP_10007_OnFire_DamageMultiply(enum_RarityLevel rarity) => 80 + 80 * (int)rarity;
        public static int I_10007_OnKill_HealReceive(enum_RarityLevel rarity) => 10 + 30 * (int)rarity;

        public const int I_10008_Cost = 2;
        public const int IP_10008_MovementReduction = 20;
        public static int IP_10008_FireRateAdditive(enum_RarityLevel rarity) => 36 + 36 * (int)rarity;

        public const int I_10009_Cost = 2;
        public const float F_10009_Duration = 10f;
        public static int IP_10009_FireRateAdditive(enum_RarityLevel rarity) => 24 + 24 * (int)rarity;

        public const int I_10010_Cost = 2;
        public const float F_10010_Duration = 10f;
        public const float F_10010_DamageReduction = 4f;
        public static int IP_10010_FireRateAdditive(enum_RarityLevel rarity) => 36 + 36 * (int)rarity;
        #endregion
        #region 20000-29999
        #endregion
        #region 30000-39999
        #endregion
    }
    #endregion

    #region DevelopersUse
    #region BaseClasses
    public static class ActionHelper
    {
        public static void PlayerAcquireSimpleEquipmentItem(EntityCharacterPlayer player, int equipmentIndex, float damage, enum_CharacterEffect effect = enum_CharacterEffect.Invalid, float duration = 0f)
        {
            player.OnAcquireEquipment<EquipmentBase>(equipmentIndex, () => { return DamageDeliverInfo.EquipmentInfo(player.I_EntityID, damage, effect, duration); });
        }
        public static void PlayerAcquireEntityEquipmentItem(EntityCharacterPlayer player, int equipmentIndex, int health, float fireRate, Func<DamageDeliverInfo> damage)
        {
            player.OnAcquireEquipment<EquipmentEntitySpawner>(equipmentIndex, damage).SetOnSpawn((EntityCharacterBase entity) => {
                EntityCharacterAI target = entity as EntityCharacterAI;
                target.I_MaxHealth = health;
                target.F_AttackDuration = new RangeFloat(0f, 0);
                target.F_AttackTimes = new RangeInt(1, 0);
                target.F_AttackRate = fireRate;
            });
        }
        public static void PlayerAttachShield(EntityCharacterPlayer player, int equipmentIndex, int health)
        {
            player.OnUseEquipment(equipmentIndex, (EquipmentShieldAttach attach) => {
                attach.SetOnSpawn((SFXShield shield) => {
                    shield.m_Health.I_MaxHealth = health;
                });
            });
        }
        public static void PlayerDealtDamageToEntity(EntityCharacterPlayer player, int targetID, float damageAmount, enum_DamageType damageType = enum_DamageType.Common)
        {
            if (damageAmount < 0)
                Debug.LogError("Howd Fk Damage Below Zero?");
            GameManager.Instance.GetEntity(targetID).m_HitCheck.TryHit(new DamageInfo(damageAmount, damageType, DamageDeliverInfo.Default(player.I_EntityID)));
        }
        public static void PlayerReciveBuff(EntityCharacterPlayer player, SBuff buff)
        {
            player.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Common, DamageDeliverInfo.BuffInfo(player.I_EntityID,buff)));
        }
        public static void ReceiveHealing(EntityCharacterPlayer entity, float heal, enum_DamageType type = enum_DamageType.Common)
        {
            if (heal <= 0)
                Debug.LogError("Howd Fk Healing Below Zero?");
            entity.m_HitCheck.TryHit(new DamageInfo(-heal, type, DamageDeliverInfo.Default(entity.I_EntityID)));
        }
        public static void ReceiveEnergy(EntityCharacterPlayer entity, float amount)
        {
            entity.m_PlayerInfo.AddActionAmount(amount);
        }
        public static void PlayerUpgradeAction(EntityCharacterPlayer player)
        {
            Debug.Log("Player Upgrade Current Random Action");
            player.m_PlayerInfo.UpgradeRandomHoldingAction();
        }
    }
    public class ActionAfterUse : ActionBase
    {
        public override void OnActionUse()
        {
            base.OnActionUse();
            ForceExpire();
        }
        public ActionAfterUse(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class ActionAfterBattle_ReloadTrigger : ActionBase
    {
        protected int m_reloadTimes;
        public override void OnReloadFinish()
        {
            base.OnReloadFinish();
            m_reloadTimes++;
            if (m_reloadTimes % (int)Value1 != 0)
                return;

            m_reloadTimes -= (int)Value1;
            OnReloadTrigger();
        }

        protected virtual void OnReloadTrigger()
        {
            Debug.LogError("Override This Please");
        }
        public ActionAfterBattle_ReloadTrigger(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class ActionAfterWeaponDetach : ActionBase
    {
        public override int I_ActionCost => 0;
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterWeaponSwitch;
        public override void OnWeaponDetach() => ForceExpire();
        public ActionAfterWeaponDetach(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class ActionMovementStackup : ActionBase
    {
        public ActionMovementStackup(int _identity, enum_RarityLevel _level) : base(_identity, _level) { m_maxStackup=-1; }
        public ActionMovementStackup(int _identity, enum_RarityLevel _level, float _maxStackup = -1) : base(_identity, _level) { m_maxStackup = _maxStackup; }
        protected float m_stackUp,m_maxStackup;
        public override void OnActionUse()
        {
            base.OnActionUse();
            m_stackUp = 0;
        }
        public override void OnMove(float amount)
        {
            base.OnMove(amount);
            m_stackUp += amount;
            if (m_maxStackup > 0 && m_stackUp > m_maxStackup)
                m_stackUp = m_maxStackup;
        }
        protected void ResetStackUp() => m_stackUp = 0;
    }
    public class ActionSingleBurstShotKill : ActionBase
    {
        public override float F_DamageAdditive => m_fireIdentity == -1 ? Value1 : 0f;
        protected int m_fireIdentity = -1;
        public override void OnActionUse()
        {
            base.OnActionUse();
            m_fireIdentity = -1;
        }
        public override void OnFire(int _identity)
        {
            base.OnFire(_identity);
            m_fireIdentity = _identity;
        }
        public override void OnDealtDemage(EntityCharacterBase receiver, DamageDeliverInfo deliverInfo)
        {
            base.OnDealtDemage(receiver, deliverInfo);
            if (deliverInfo.I_IdentiyID == m_fireIdentity)
                OnShotsHit(receiver);
        }
        protected virtual void OnShotsHit(EntityCharacterBase _hitEntity) {}
        public ActionSingleBurstShotKill(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    #endregion
    #region Inherted Claseses
    public class Action_10001_ClipAdditive : ActionBase
    {
        public override int m_Index => 10001;
        public override int I_ActionCost => ActionData.I_10001_Cost;
        public override float Value1 => ActionData.IP_10001_ClipMultiply(m_rarity);
        public override float F_ClipMultiply => Value1 / 100f;
        public override float F_Duration => ActionData.F_10001_Duration;
        public Action_10001_ClipAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }

    public class Action_10002_MoveArmorAdditive : ActionBase
    {
        public override int m_Index => 10002;
        public override int I_ActionCost => ActionData.I_10002_Cost;
        public override float F_Duration => ActionData.F_10002_Duration;
        public override float Value1 => ActionData.F_10002_ArmorReceive(m_rarity);
        public override void OnMove(float amount) => ActionHelper.ReceiveHealing(m_ActionEntity,Value1*amount, enum_DamageType.ArmorOnly);
        public Action_10002_MoveArmorAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }

    public class Action_10003_MoveFirerateStackup : ActionMovementStackup
    {
        public override int m_Index => 10003;
        public override int I_ActionCost => ActionData.I_10003_Cost;
        public override float F_Duration => ActionData.F_10003_Duration;
        public override float Value1 => ActionData.F_10003_FirerateMultiplyPerMile(m_rarity);
        public override float Value2 => ActionData.F_10003_FirerateMultiplyPerMile(m_rarity) * m_maxStackup;
        public override float m_FireRateMultiply => Value1*m_stackUp;
        public Action_10003_MoveFirerateStackup(int _identity, enum_RarityLevel _level) : base(_identity, _level,ActionData.I_10003_MaxStackAmount) { }
    }

    public class Action_10004_MoveEnergyAdditive : ActionBase
    {
        public override int m_Index => 10004;
        public override int I_ActionCost => ActionData.I_10004_Cost;
        public override float F_Duration => ActionData.F_10004_Duration;
        public override float Value1 => ActionData.F_10004_EnergyAdditivePerMile(m_rarity);
        public override void OnMove(float distance) =>ActionHelper.ReceiveEnergy(m_ActionEntity, Value1 *distance);
        public Action_10004_MoveEnergyAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }

    public class Action_10005_SingleShotMovementAdditive : ActionSingleBurstShotKill
    {
        public override int m_Index => 10005;
        public override float Value1 => ActionData.IP_10005_OnFire_DamageMultiply(m_rarity);
        public override float Value2 => ActionData.IP_10006_OnKill_Buff_MovementSpeed(m_rarity);
        public override float Value3 => ActionData.F_10005_OnKill_Buff_Duration;
        protected override void OnShotsHit(EntityCharacterBase _hitEntity)
        {
            if (_hitEntity.m_Health.b_IsDead)
                ActionHelper.PlayerReciveBuff(m_ActionEntity,SBuff.CreateActionBuff(m_Index,Value2/100f,Value3));
        }
        public Action_10005_SingleShotMovementAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    #endregion
    #endregion


    #region Abandoned
    //#region NormalAction
    //public class Action_10001_ArmorAdditive : ActionAfterUse
    //{
    //    public override int m_Index => 10001;
    //    public override int I_ActionCost => ActionData.I_10001_Cost;
    //    public override float Value1 => ActionData.F_10001_ArmorAdditive(m_rarity);
    //    public override void OnActionUse() {
    //        base.OnActionUse();
    //        ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
    //    }
    //    public Action_10001_ArmorAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_10002_ArmorDamageAdditive : ActionBase
    //{
    //    public override int m_Index => 10002;
    //    public override float F_Duration => ActionData.F_10002_Duration;
    //    public override int I_ActionCost => ActionData.I_10002_Cost;
    //    public override float Value1 => ActionData.F_10002_ArmorDamageAdditive(m_rarity);
    //    public override float F_DamageAdditive =>  Value1* m_ActionEntity.m_PlayerHealth.m_CurrentArmor;
    //    public Action_10002_ArmorDamageAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10003_ArmorMultiplyAdditive : ActionAfterUse
    //{
    //    public override int m_Index => 10003;
    //    public override int I_ActionCost => ActionData.I_10003_Cost;
    //    public override float Value1 => ActionData.IP_10003_ArmorMultiplyAdditive(m_rarity);
    //    public override void OnActionUse() {
    //        base.OnActionUse();
    //        ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1 / 100f * m_ActionEntity.m_PlayerHealth.m_CurrentArmor, enum_DamageType.ArmorOnly);
    //    } 
    //    public Action_10003_ArmorMultiplyAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_10004_ArmorActionReturn : ActionAfterUse
    //{
    //    public override int m_Index => 10004;
    //    public override int I_ActionCost => ActionData.I_10004_Cost;
    //    public override float Value1 => ActionData.F_10004_ArmorActionAcquire(m_rarity);
    //    public override void OnActionUse()
    //    {
    //        base.OnActionUse();
    //        ActionHelper.PlayerReceiveActionAmount(m_ActionEntity, Value1* m_ActionEntity.m_PlayerHealth.m_CurrentArmor);
    //    } 
    //    public Action_10004_ArmorActionReturn(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_10005_ArmorDamageReduction : ActionBase
    //{
    //    public override int m_Index => 10005;
    //    public override float F_Duration => ActionData.F_10005_Duration;
    //    public override int m_EffectIndex => base.m_EffectIndex;
    //    public override int I_ActionCost => ActionData.I_10005_Cost;
    //    public override float Value1 => ActionData.IP_10005_ArmorDamageReduction(m_rarity);
    //    public override float m_DamageReduction=> Value1 / 100f;
    //    public Action_10005_ArmorDamageReduction(int _identity, enum_RarityLevel _level) : base(_identity ,_level) { }
    //}
    //public class Action_10006_FireRateAdditive: ActionBase
    //{
    //    public override int m_Index => 10006;
    //    public override int I_ActionCost => ActionData.I_10006_Cost;
    //    public override float Value1 =>ActionData.IP_10006_FireRateAdditive(m_rarity);
    //    public override float m_FireRateMultiply => Value1 / 100f;
    //    public override float F_Duration => ActionData.F_10006_Duration;
    //    public Action_10006_FireRateAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10007_RecoilReduction : ActionBase
    //{
    //    public override int m_Index => 10007;
    //    public override int I_ActionCost => ActionData.I_10007_Cost;
    //    public override float Value1 => ActionData.IP_10007_RecoilMultiplyAdditive(m_rarity);
    //    public override float F_RecoilReduction => Value1 / 100f;
    //    public override float F_Duration => ActionData.F_10007_Duration;
    //    public Action_10007_RecoilReduction(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10008_ClipMultiply : ActionBase
    //{
    //    public override int m_Index => 10008;
    //    public override int I_ActionCost => ActionData.I_10008_Cost;
    //    public override float Value1 => ActionData.IP_10008_ClipMultiply(m_rarity) ;
    //    public override float F_ClipMultiply => Value1 / 100f;
    //    public override float F_Duration => ActionData.F_10008_Duration;
    //    public Action_10008_ClipMultiply(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10009_ProjectileSpeedMultiply : ActionBase
    //{
    //    public override int m_Index => 10009;
    //    public override int I_ActionCost => ActionData.I_10009_Cost;
    //    public override float Value1 => ActionData.IP_10009_BulletSpeedAdditive(m_rarity);
    //    public override float F_ProjectileSpeedMultiply => Value1 / 100f;
    //    public override float F_Duration => ActionData.F_10009_Duration;
    //    public Action_10009_ProjectileSpeedMultiply(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10010_SingleDamageMultiply : ActionAfterFire
    //{
    //    public override int m_Index => 10010;
    //    public override int I_ActionCost => ActionData.I_10010_Cost;
    //    public override float Value1 => ActionData.IP_10010_SingleDamageMultiply(m_rarity) ;
    //    public override float m_DamageMultiply => Value1 / 100f;
    //    public Action_10010_SingleDamageMultiply(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_10011_SingleDamageKillActionReturn : ActionAfterFire
    //{
    //    public override int m_Index => 10011;
    //    public override int I_ActionCost => ActionData.I_10011_Cost;
    //    public override float Value1 => ActionData.IP_10011_SingleDamageMultiply(m_rarity) ;
    //    public override float Value2 => ActionData.F_10011_EntityKillActionReturn(m_rarity);
    //    public override float m_DamageMultiply => Value1 / 100f;
    //    protected override bool OnActionHit( EntityCharacterBase _targetEntity)
    //    {
    //        if (!_targetEntity.m_Health.b_IsDead)
    //            return false;
    //        ActionHelper.PlayerReceiveActionAmount(m_ActionEntity, Value2);
    //        return true;
    //    }
    //    public Action_10011_SingleDamageKillActionReturn(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_10012_SingleDamageKillHealing : ActionAfterFire
    //{
    //    public override int m_Index => 10012;
    //    public override int I_ActionCost => ActionData.I_10012_Cost;
    //    public override float Value1 => ActionData.IP_10012_SingleDamageMultiply(m_rarity) ;
    //    public override float Value2 => ActionData.F_10012_EntityKillHealing(m_rarity);
    //    public override float m_DamageMultiply => Value1 / 100f;
    //    protected override bool OnActionHit( EntityCharacterBase _targetEntity)
    //    {
    //        if (!_targetEntity.m_Health.b_IsDead)
    //            return false;
    //        ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.HealthOnly);
    //        return true;
    //    }
    //    public Action_10012_SingleDamageKillHealing(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_10013_SingleProjectileKillActionUpgrade : ActionAfterFire
    //{
    //    public override int m_Index => 10013;
    //    public override int I_ActionCost => ActionData.I_10013_Cost;
    //    public override float Value1 => ActionData.IP_10013_SingleDamageMultiply(m_rarity) ;
    //    public override float m_DamageMultiply => Value1 / 100f;
    //    protected override bool OnActionHit( EntityCharacterBase _targetEntity)
    //    {
    //        if (!_targetEntity.m_Health.b_IsDead)
    //            return false;
    //        ActionHelper.PlayerUpgradeAction(m_ActionEntity);
    //        return true;
    //    }
    //    public Action_10013_SingleProjectileKillActionUpgrade(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_10014_ReloadRateMultiply : ActionBase
    //{
    //    public override int m_Index => 10014;
    //    public override int I_ActionCost => ActionData.I_10014_Cost;
    //    public override float Value1 => ActionData.IP_10014_ReloadRateMultiplyPercentage(m_rarity);
    //    public override float m_ReloadRateMultiply => Value1/100f;
    //    public override float F_Duration => ActionData.F_10014_Duration;
    //    public Action_10014_ReloadRateMultiply(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10015_ArmorDemageReturn : ActionBase
    //{
    //    public override int m_Index => 10015;
    //    public override int I_ActionCost => ActionData.I_10015_Cost;
    //    public override float Value1 => ActionData.F_10015_ArmorDamageReturn(m_rarity);
    //    public override void OnReceiveDamage(int applier, float amount) => ActionHelper.PlayerDealtDamageToEntity(m_ActionEntity, applier, Value1 * m_ActionEntity.m_PlayerHealth.m_CurrentArmor);
    //    public override float F_Duration => ActionData.F_10015_Duration;
    //    public Action_10015_ArmorDemageReturn(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10016_FireBurstDamageReduce : ActionBase
    //{
    //    public override int m_Index => 10016;
    //    public override int I_ActionCost => ActionData.I_10016_Cost;
    //    public override float Value1 => ActionData.IP_10016_DamageReducePercentage(m_rarity);
    //    public override float Value2 => ActionData.IP_10016_FireRateAdditivePercentage(m_rarity);
    //    public override float m_DamageMultiply => -Value1/100f;
    //    public override float m_FireRateMultiply => Value2/100f;
    //    public override float F_Duration => ActionData.F_10016_Duration;
    //    public Action_10016_FireBurstDamageReduce(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}

    //public class Action_10017_SingleProjectileKillArmorAdditive : ActionAfterFire
    //{
    //    public override int m_Index => 10017;
    //    public override int I_ActionCost => ActionData.I_10017_Cost;
    //    public override float Value1 => ActionData.IP_10017_DamageAdditivePercentage(m_rarity);
    //    public override float Value2 => ActionData.F_10017_EntityKillArmorAdditive(m_rarity);
    //    public override float m_DamageMultiply => Value1 / 100f;
    //    protected override bool OnActionHit(EntityCharacterBase _targetEntity)
    //    {
    //        if (!_targetEntity.m_Health.b_IsDead)
    //            return false;
    //        ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.ArmorOnly);
    //        return true;
    //    }
    //    public Action_10017_SingleProjectileKillArmorAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10018_Vanish : ActionBase
    //{
    //    public override int m_Index => 10018;
    //    public override int I_ActionCost => ActionData.I_10018_Cost(m_rarity);
    //    public override float F_Duration => ActionData.F_10018_Duration;
    //    public override float F_CloakDuration => ActionData.F_10018_Duration;
    //    public Action_10018_Vanish(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10019_ClipAdditive : ActionBase
    //{
    //    public override int m_Index => 10019;
    //    public override int I_ActionCost => ActionData.I_10019_Cost;
    //    public override float Value1 => ActionData.I_10019_ClipAdditiveAmount(m_rarity);
    //    public override int I_ClipAdditive => (int)Value1;
    //    public override float F_Duration => ActionData.F_10019_Duration;
    //    public Action_10019_ClipAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_10020_DamageSteal : ActionBase
    //{
    //    public override int m_Index => 10020;
    //    public override int I_ActionCost => ActionData.I_10020_Cost;
    //    public override float F_Duration => ActionData.F_10020_Duration;
    //    public override float Value1 => ActionData.IP_10020_HealthStealPercentage(m_rarity);
    //    public override void OnDealtDemage(EntityCharacterBase receiver, float amount)
    //    {
    //        base.OnDealtDemage(receiver, amount);
    //        ActionHelper.PlayerReceiveHealing(m_ActionEntity, amount * Value1, enum_DamageType.HealthOnly);
    //    }
    //    public Action_10020_DamageSteal(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //#endregion
    //#region EquipmentItem
    //public class Action_20001_Armor_Turret_Cannon : ActionAfterUse
    //{
    //    public override int m_Index => 20001;
    //    public override int I_ActionCost => ActionData.I_20001_Cost;
    //    public override float Value1 => ActionData.F_20001_Health(m_rarity);
    //    public override float Value2 => ActionData.F_20001_ArmorTurretDamage(m_rarity);
    //    public override float Value3 => ActionData.F_20001_TurretMinumumDamage(m_rarity);
    //    public override void OnActionUse() {
    //        base.OnActionUse();
    //        ActionHelper.PlayerAcquireEntityEquipmentItem(m_ActionEntity, m_Index,  (int)(Value1* m_ActionEntity.m_PlayerHealth.m_CurrentArmor), 1f, GetDamageInfo);
    //    }
    //    public DamageDeliverInfo GetDamageInfo()=> DamageDeliverInfo.DamageInfo(m_ActionEntity.I_EntityID,1f, Value3+Value2 * m_ActionEntity.m_PlayerHealth.m_CurrentArmor);
    //    public Action_20001_Armor_Turret_Cannon(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_20002_FireRate_FrozenGrenade : ActionAfterUse
    //{
    //    public override int m_Index => 20002;
    //    public override int I_ActionCost => ActionData.I_20002_Cost;
    //    public override float Value1=> ActionData.F_20002_DamageDealt(m_rarity);
    //    public override float Value2=> ActionData.F_20002_StunDuration(m_rarity);
    //    public override float Value3 => GameDataManager.GetEntityBuffProperties((int)ActionData.F_20002_StunDuration(m_rarity)).m_ExpireDuration;
    //    public override void OnActionUse()
    //    {
    //        base.OnActionUse();
    //        ActionHelper.PlayerAcquireSimpleEquipmentItem(m_ActionEntity, m_Index, Value1, enum_CharacterEffect.Stun, Value2);
    //    }
    //    public Action_20002_FireRate_FrozenGrenade(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_20003_Firerate_Turret_Minigun : ActionAfterUse
    //{
    //    public override int m_Index => 20003;
    //    public override int I_ActionCost => ActionData.I_20003_Cost;
    //    public override float Value1 => ActionData.F_20003_FireRate(m_rarity);
    //    public override float Value2 => ActionData.F_20003_DamageDealt(m_rarity) ;
    //    public override float Value3 => ActionData.F_20003_Health(m_rarity);
    //    public override void OnActionUse()
    //    {
    //        base.OnActionUse();
    //        ActionHelper.PlayerAcquireEntityEquipmentItem(m_ActionEntity, m_Index, (int)Value3, Value1 ,GetDamageInfo);
    //    }
    //    public DamageDeliverInfo GetDamageInfo() => DamageDeliverInfo.DamageInfo(m_ActionEntity.I_EntityID, 1f,Value2 * m_ActionEntity.m_WeaponCurrent.F_BaseDamage);
    //    public Action_20003_Firerate_Turret_Minigun(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_20004_Damage_ExplosiveGrenade : ActionAfterUse
    //{
    //    public override int m_Index => 20004;
    //    public override int I_ActionCost => ActionData.I_20004_Cost;
    //    public override float Value1 => ActionData.F_20004_DamageDealt(m_rarity);
    //    public override void OnActionUse() {
    //        base.OnActionUse();
    //        ActionHelper.PlayerAcquireSimpleEquipmentItem(m_ActionEntity, m_Index, Value1* m_ActionEntity.m_WeaponCurrent.F_BaseDamage);
    //    }
    //    public Action_20004_Damage_ExplosiveGrenade(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_20005_Damage_ExplosiveGrenade : ActionAfterUse
    //{
    //    public override int m_Index => 20005;
    //    public override int I_ActionCost => ActionData.I_20005_Cost;
    //    public override float Value1 => ActionData.IP_20005_BaseShieldHealthPercentage(m_rarity);
    //    public override float Value2 => ActionData.IP_20005_BonusShieldHealthPercentage(m_rarity);
    //    public override float Value3 => ActionData.F_20001_TurretMinumumDamage(m_rarity);
    //    public override void OnActionUse()
    //    {
    //        base.OnActionUse();
    //        float shieldHealth = Value1 + Value2;
    //        shieldHealth = shieldHealth > Value3 ? shieldHealth : Value3;
    //        ActionHelper.PlayerAttachShield(m_ActionEntity, m_Index, (int)shieldHealth);
    //    }
    //    public Action_20005_Damage_ExplosiveGrenade(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //#endregion
    //#region LevelEquipment
    //public class Action_30001_ArmorActionAdditive : ActionAfterBattle
    //{
    //    public override int m_Index => 30001;
    //    public override int I_ActionCost => ActionData.I_30001_Cost;
    //    public override float Value1 => ActionData.F_30001_ArmorActionAdditive(m_rarity);
    //    public override void OnAddActionElse(float actionAmount) => ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
    //    public Action_30001_ArmorActionAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_30002_ArmorActionAdditive : ActionAfterBattle
    //{
    //    public override int m_Index => 30002;
    //    public override int I_ActionCost => ActionData.I_30002_Cost;
    //    public override float Value1 => ActionData.F_30002_ReceiveDamageArmorAdditive(m_rarity);
    //    public override void OnReceiveDamage(int applier, float amount)
    //    {
    //        base.OnReceiveDamage(applier, amount);
    //        ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
    //    }
    //    public Action_30002_ArmorActionAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //public class Action_30003_DamageAdditive : ActionAfterBattle
    //{
    //    public override int m_Index => 30003;
    //    public override int I_ActionCost => ActionData.I_30003_Cost;
    //    public override float Value1 => ActionData.F_30003_DamageAdditive(m_rarity);
    //    public override float F_DamageAdditive => Value1;
    //    public Action_30003_DamageAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_30004_ClipOverrideDamageAdditive : ActionAfterBattle
    //{
    //    public override int m_Index => 30004;
    //    public override int I_ActionCost => ActionData.I_30004_Cost;
    //    public override bool B_ClipOverride => true;
    //    public override float Value1 => ActionData.F_30004_DamageAdditive(m_rarity);
    //    public override float F_DamageAdditive=> Value1;
    //    public Action_30004_ClipOverrideDamageAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_30005_DamageHeal : ActionAfterBattle
    //{
    //    public override int m_Index => 30005;
    //    public override int I_ActionCost => ActionData.I_30005_Cost;
    //    public override float Value1 => ActionData.F_30005_DamageDealtHeal(m_rarity);
    //    public override float Value2 => ActionData.F_30005_HealAmount(m_rarity);
    //    float m_damageDealt = 0;
    //    public override void OnDealtDemage(EntityCharacterBase receiver, float amount)
    //    {
    //        base.OnDealtDemage(receiver, amount);
    //        m_damageDealt += amount;
    //        if (m_damageDealt < Value1)
    //            return;
    //        m_damageDealt -= Value1;
    //        ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.HealthOnly);
    //    }
    //    public Action_30005_DamageHeal(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_30006_ReloadDamageMultiply : ActionAfterBattle_ReloadTrigger
    //{
    //    public override int m_Index => 30006;
    //    public override int I_ActionCost => ActionData.I_30006_Cost;
    //    public override float Value1 => ActionData.I_30006_ReloadTimesCount(m_rarity);
    //    public override float Value2 => ActionData.IP_30006_ReloadDamageMultiplyPercentage(m_rarity);
    //    public override float m_DamageMultiply =>  m_TriggerOn?Value2 / 100f : 0;
    //    bool m_TriggerOn = false;
    //    public override void OnReloadFinish()
    //    {
    //        m_TriggerOn = false;
    //        base.OnReloadFinish();
    //    }
    //    protected override void OnReloadTrigger() => m_TriggerOn = true;
    //    public Action_30006_ReloadDamageMultiply(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_30007_MovementStackupDamageAdditive : ActionAfterBattle
    //{
    //    public override int m_Index => 30007;
    //    public override int I_ActionCost => ActionData.I_30007_Cost;
    //    public override float Value1 => ActionData.F_MovementStackupMax(m_rarity);
    //    public override float Value2 => ActionData.F_MovementStackupDamageAdditive( m_rarity);
    //    public override float F_DamageAdditive => m_stackUpDamage;

    //    protected float m_stackUp;
    //    protected float m_stackUpDamage;
    //    protected Vector3 m_prePos;
    //    public override void OnActionUse()
    //    {
    //        base.OnActionUse();
    //        m_prePos = m_ActionEntity.transform.position;
    //        m_stackUp = 0;
    //    }
    //    public override void OnFire(int identity) => m_stackUp = 0;
    //    float timeCheck;
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        if (timeCheck > 0)
    //        {
    //            timeCheck -= deltaTime;
    //            return;
    //        }
    //        timeCheck = .5f;

    //        m_stackUp += TCommon.GetXZDistance(m_prePos, m_ActionEntity.transform.position);
    //        m_prePos = m_ActionEntity.transform.position;
    //        m_stackUpDamage = m_stackUp * Value2;
    //        if (m_stackUpDamage > Value1)
    //            m_stackUpDamage = Value1;
    //    }
    //    public Action_30007_MovementStackupDamageAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    //}
    //#endregion
    //#region WeaponAction
    //public class Action_40001_DealtDamageAddArmor : ActionAfterWeaponDetach
    //{
    //    public override int m_Index => 40001;
    //    float m_TotalDamageDealt=0;
    //    public override float Value1 => ActionData.F_40001_DamageDealtCount(m_rarity);
    //    public override float Value2 => ActionData.F_40001_ArmorAdditive(m_rarity);
    //    public override void OnDealtDemage( EntityCharacterBase receiver, float amount)
    //    {
    //        base.OnDealtDemage( receiver, amount);
    //        m_TotalDamageDealt += amount;
    //        if (m_TotalDamageDealt < Value1)
    //            return;
    //        m_TotalDamageDealt -= Value1;

    //        ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.ArmorOnly);
    //    }
    //    public Action_40001_DealtDamageAddArmor(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}

    //public class Action_40002_DealtDamageAddActionRandom : ActionAfterWeaponDetach
    //{
    //    public override int m_Index => 40002;
    //    public override float Value1 => ActionData.IP_40002_DamageDealtAddActionRate(m_rarity);
    //    public override float Value2 => ActionData.F_40002_AddActionAmount(m_rarity);
    //    public override void OnFire(int identity)
    //    {
    //        base.OnFire(identity);
    //        if (TCommon.RandomPercentage() < Value1)
    //            ActionHelper.PlayerReceiveActionAmount(m_ActionEntity, Value2);
    //    }
    //    public Action_40002_DealtDamageAddActionRandom(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_40003_FireTimesDamageAdditive : ActionAfterWeaponDetach
    //{
    //    public override int m_Index => 40003;
    //    public override float Value1=> ActionData.F_40003_FireTimesCount(m_rarity);
    //    public override float Value2 => ActionData.F_40003_DamageAdditive(m_rarity);
    //    bool m_triggerd = false;
    //    int fireCount = 0;
    //    public override float F_DamageAdditive => m_triggerd ? Value2 : 0;
    //    public override void OnFire(int identity)
    //    {
    //        base.OnFire(identity);
    //        m_triggerd = false;
    //        fireCount++;
    //        if (fireCount < Value1)
    //            return;
    //        fireCount -= (int)Value1;
    //        m_triggerd = true;
    //    }
    //    public Action_40003_FireTimesDamageAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    //public class Action_40007_DamageReductionCooldown : ActionAfterWeaponDetach
    //{
    //    public override int m_Index => 40007;
    //    public override int m_EffectIndex => m_activating ? 40002 : -1;
    //    public override float m_EffectDuration => Value1;
    //    public override float Value1 => ActionData.F_40007_DamageReductionDuration(m_rarity);
    //    public override float Value2 => ActionData.F_40007_ArmorAdditive(m_rarity);
    //    public override float Value3 => ActionData.F_40007_Cooldown(m_rarity);
    //    public override float m_DamageReduction => m_activating ? 1f : 0;
    //    float m_cooldownCheck=-1f;
    //    bool m_cooldowning => m_cooldownCheck >= 0;
    //    float m_activateCheck=-1f;
    //    bool m_activating => m_activateCheck >= 0;
    //    public override void OnTick(float deltaTime)
    //    {
    //        base.OnTick(deltaTime);
    //        if (m_cooldowning)
    //            m_cooldownCheck -= deltaTime;

    //        if (m_activating)
    //            m_activateCheck -= deltaTime;
    //    }
    //    public override void OnReceiveDamage(int applier,float amount)
    //    {
    //        base.OnReceiveDamage(applier, amount);
    //        if (m_activating||m_cooldowning)
    //            return;

    //        if (m_ActionEntity.m_PlayerHealth.m_CurrentArmor <= 0)
    //        {
    //            m_activateCheck += Value1;
    //            m_cooldownCheck += Value3;
    //        }
    //    }
    //    public Action_40007_DamageReductionCooldown(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    //}
    public class Action_40012_UseActionReturn : ActionAfterWeaponDetach
    {
        public override int m_Index => 40012;
        public override float Value1 => 0f;
        public override void OnAddActionElse(float actionAmount)
        {
            base.OnAddActionElse(actionAmount);
            ActionHelper.ReceiveEnergy(m_ActionEntity, Value1);
        }
        public Action_40012_UseActionReturn(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }

    public class Action_40014_KillArmorAdditive : ActionAfterWeaponDetach
    {
        public override int m_Index => 40014;
        public override float Value1 => 0;
        public override void OnDealtDemage(EntityCharacterBase receiver, DamageDeliverInfo deliverInfo)
        {
            base.OnDealtDemage(receiver, deliverInfo);
            if (receiver.m_Health.b_IsDead)
                ActionHelper.ReceiveHealing(m_ActionEntity, Value1, enum_DamageType.HealthOnly);
        }
        public Action_40014_KillArmorAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    //#endregion
    #endregion
}