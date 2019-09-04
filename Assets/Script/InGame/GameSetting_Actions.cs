using GameSetting;
using UnityEngine;
using System;
using System.Collections.Generic;
namespace GameSetting_Action
{
    public static class ActionData
    {
        #region Costs
        public const int I_10001_Cost = 2;
        public const int I_10002_Cost = 2;
        public const int I_10003_Cost = 2;
        public const int I_10004_Cost = 0;
        public const int I_10005_Cost = 3;
        public const int I_10006_Cost = 1;
        public const int I_10007_Cost = 1;
        public const int I_10008_Cost = 1;
        public const int I_10009_Cost = 1;
        public const int I_10010_Cost = 1;
        public const int I_10011_Cost = 1;
        public const int I_10012_Cost = 1;
        public const int I_10013_Cost = 1;
        public const int I_10014_Cost = 1;

        public const int I_20001_Cost = 2;
        public const int I_20002_Cost = 1;
        public const int I_20003_Cost = 2;
        public const int I_20004_Cost = 1;

        public const int I_30001_Cost = 1;
        public const int I_30002_Cost = 2;
        public const int I_30003_Cost = 2;
        public const int I_30004_Cost = 2;
        public const int I_30005_Cost = 1;
        public const int I_30006_Cost = 1;
        #endregion
        #region Durations
        public const float F_10002_Duration = 30f;
        public const float F_10005_Duration = 30f;
        public const float F_10006_Duration = 30f;
        public const float F_10007_Duration = 30f;
        public const float F_10008_Duration = 30f;
        public const float F_10009_Duration = 30f;
        public const float F_10014_Duration = 30f;
        #endregion
        #region Expressions
        public static float F_10001_ArmorAdditive(enum_RarityLevel level) => 30 + 30f * (int)level;
        public static float F_10002_ArmorDamageAdditive(enum_RarityLevel level) => 1f * (int)level;
        public static int IP_10003_ArmorMultiplyAdditive(enum_RarityLevel level) => 30 + 30 * (int)level;
        public static float F_10004_ArmorActionAcquire(enum_RarityLevel level) => 60f - 10f * (int)level;
        public static int IP_10005_ArmorDamageReduction(enum_RarityLevel level) => 30 + 20 * (int)level;
        public static int IP_10006_FireRateAdditive(enum_RarityLevel level) => 30 + 30 * (int)level;
        public static int IP_10007_RecoilMultiplyAdditive(enum_RarityLevel level) => 40 + 20 * (int)level;
        public static int IP_10008_ClipMultiply(enum_RarityLevel level) => 100 * (int)level;
        public static int IP_10009_BulletSpeedAdditive(enum_RarityLevel level) => 60 * (int)level;
        public static int IP_10010_SingleDamageMultiply(enum_RarityLevel level) => 200 + 400 * (int)level;
        public static int IP_10011_SingleDamageMultiply(enum_RarityLevel level) => 200 + 200 * (int)level;
        public static float F_10011_EntityKillActionReturn(enum_RarityLevel level) => 1 + (int)level;
        public static int IP_10012_SingleDamageMultiply(enum_RarityLevel level) => 200 + 200 * (int)level;
        public static float F_10012_EntityKillHealing(enum_RarityLevel level) => 30 * (int)level;
        public static int IP_10013_SingleDamageMultiply(enum_RarityLevel level) => 200 + 200 * (int)level;
        public static int IP_10014_ReloadRateMultiplyPercentage(enum_RarityLevel level) => 60 + 40 * (int)level;

        public static float F_20001_Health(enum_RarityLevel level) => 200;
        public static float F_20001_ArmorTurretDamage(enum_RarityLevel level) => 1.5f * (int)level;
        public static float F_20001_TurretMinumumDamage(enum_RarityLevel level) => 50f;
        public static float F_20002_DamageDealt(enum_RarityLevel level) => 50f;
        public static float F_20002_BuffIndex(enum_RarityLevel level) => 200020 + (int)level;
        public static float F_20003_Health(enum_RarityLevel level) => 200;
        public static float F_20003_FireRate(enum_RarityLevel level) => 1f;
        public static float F_20003_DamageDealt(enum_RarityLevel level) => .7f * (int)level;
        public static float F_20004_DamageDealt(enum_RarityLevel level) => 2 + 2 * (int)level;

        public static float F_30001_ArmorActionAdditive(enum_RarityLevel level) => 10 + 10 * (int)level;
        public static float F_30002_ArmorDamageReturn(enum_RarityLevel level) => 2 * (int)level;
        public static float F_30003_DamageAdditive(enum_RarityLevel level) => 20f * (int)level;
        public static float F_30004_DamageAdditive(enum_RarityLevel level) => 200 * (int)level;
        public static float F_30005_ReloadTimesHeal(enum_RarityLevel level) => 5 - 1 * (int)level;
        public static float F_30005_ReloadHealAmount(enum_RarityLevel level) => 20f;
        public static int I_30006_ReloadTimesCount(enum_RarityLevel level) => 2;
        public static int IP_30006_ReloadDamageMultiplyPercentage(enum_RarityLevel level) => 200 * (int)level;

        public static float F_40001_DamageDealtCount(enum_RarityLevel level) => 2000 / Mathf.Pow(2, (int)level-1);
        public static float F_40001_ArmorAdditive(enum_RarityLevel level) => 20f;
        public static int IP_40002_DamageDealtAddActionRate(enum_RarityLevel level) => 1 * (int)level;
        public static float F_40002_AddActionAmount(enum_RarityLevel level) => 1f;
        public static float F_40003_FireTimesCount(enum_RarityLevel level) => 6 - (int)level;
        public static float F_40003_DamageAdditive(enum_RarityLevel level) => 300f;
        public static float F_40007_DamageReductionDuration(enum_RarityLevel level) => (int)level;
        public static float F_40007_ArmorAdditive(enum_RarityLevel level) => 30 * (int)level;
        public static float F_40007_Cooldown(enum_RarityLevel level) => 60f;
        public static float F_40012_ActionReturn(enum_RarityLevel level) => .2f * (int)level;
        public static float F_40014_ArmorAdditive(enum_RarityLevel level) => 10 * (int)level;
        #endregion
    }

    public static class ActionHelper
    {
        public static void PlayerAcquireSimpleEquipmentItem(EntityPlayerBase player, int equipmentIndex,float damage,int buffIndex=-1)
        {
            player.OnAcquireEquipment<EquipmentBase>(equipmentIndex, () => { return DamageDeliverInfo.EquipmentInfo(player.I_EntityID,damage,buffIndex); });
        }
        public static void PlayerAcquireEntityEquipmentItem(EntityPlayerBase player, int equipmentIndex,int health,float fireRate, Func<DamageDeliverInfo> damage)
        {
            player.OnAcquireEquipment<EquipmentEntitySpawner>(equipmentIndex,damage).SetOnSpawn((EntityBase entity) => {
                EntityAIBase target = entity as EntityAIBase;
                target.I_MaxHealth = health;
                target.F_AttackDuration = new RangeFloat(0f, 0);
                target.F_AttackTimes = new RangeInt(1, 0);
                target.F_AttackRate = fireRate;
            });
        }
        public static void PlayerDealtDamageToEntity(EntityPlayerBase player,int targetID, float damageAmount, enum_DamageType damageType= enum_DamageType.Common)
        {
            if (damageAmount < 0)
                Debug.LogError("Howd Fk Damage Below Zero?");
            GameManager.Instance.GetEntity(targetID).m_HitCheck.TryHit(new DamageInfo(damageAmount, damageType, DamageDeliverInfo.Default(player.I_EntityID)));
        }
        public static void PlayerReceiveHealing(EntityPlayerBase player,float heal,enum_DamageType type= enum_DamageType.Common)
        {
            Debug.Log("Player Receive Healing Amount:" + heal);
            if (heal <= 0)
                Debug.LogError("Howd Fk Healing Below Zero?");
            player.m_HitCheck.TryHit(new DamageInfo(-heal,type, DamageDeliverInfo.Default(player.I_EntityID)));
        }
        public static void PlayerReceiveActionAmount(EntityPlayerBase player, float amount)
        {
            Debug.Log("Player Receive Aciton Amount:" + amount);
            player.m_PlayerInfo.AddActionAmount(amount);
        }
        public static void PlayerUpgradeAction(EntityPlayerBase player)
        {
            Debug.Log("Player Upgrade Current Random Action");
            player.m_PlayerInfo.UpgradeRandomHoldingAction();
        }
    }
    #region ActionBase
    public class ActionAfterUse : ActionBase
    {
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterUse;
        public override void OnActionUse()
        {
            base.OnActionUse();
            ForceExpire();
        }
        public ActionAfterUse(enum_RarityLevel _level) : base(_level) { }
    }
    public class ActionAfterDuration : ActionBase
    {
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterDuration;
        public ActionAfterDuration(enum_RarityLevel _level) : base(_level) { }
        public ActionAfterDuration(enum_RarityLevel _level,float _duration) : base(_level,_duration) { }
    }
    public class ActionAfterFire : ActionBase
    {
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterFire;
        protected int m_fireIdentity = -1;
        public override void OnAfterFire(int _identity)
        {
            base.OnAfterFire(_identity);
            m_fireIdentity = _identity;
            ForceExpire();
        }
        public bool OnActionHitEntity(int damageIdentity, EntityBase _targetEntity)
        {
            if (m_fireIdentity != damageIdentity)
                return false;
            return OnActionHit(_targetEntity);
        }

        protected virtual bool OnActionHit( EntityBase _hitEntity)
        {
            return false;
        }
        public ActionAfterFire(enum_RarityLevel _level) : base(_level) { }
    }
    public class ActionAfterBattle : ActionBase
    {
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterBattle;
        public override void OnAfterBattle()
        {
            base.OnAfterBattle();
            ForceExpire();
        } 
        public ActionAfterBattle(enum_RarityLevel _level) : base(_level) { }
    }
    public class ActionAfterBattle_ReloadTrigger : ActionAfterBattle
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
        public ActionAfterBattle_ReloadTrigger(enum_RarityLevel _level) : base(_level) { }
    }
    public class ActionAfterWeaponDetach : ActionBase
    {
        public override int I_ActionCost => 0;
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterWeaponSwitch;
        public override void OnWeaponDetach() => ForceExpire();
        public ActionAfterWeaponDetach(enum_RarityLevel _level) : base(_level) { }
    }
    #endregion

    #region NormalAction
    public class Action_10001_ArmorAdditive : ActionAfterUse
    {
        public override int m_Index => 10001;
        public override int I_ActionCost => ActionData.I_10001_Cost;
        public override float Value1 => ActionData.F_10001_ArmorAdditive(m_Level);
        public override void OnActionUse() {
            base.OnActionUse();
            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
        }
        public Action_10001_ArmorAdditive(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_10002_ArmorDamageAdditive : ActionAfterDuration
    {
        public override int m_Index => 10002;
        public override int I_ActionCost => ActionData.I_10002_Cost;
        public override float Value1 => ActionData.F_10002_ArmorDamageAdditive(m_Level);
        public override float F_DamageAdditive =>  Value1* m_ActionEntity.m_HealthManager.m_CurrentArmor;
        public Action_10002_ArmorDamageAdditive(enum_RarityLevel _level) : base(_level, ActionData.F_10002_Duration) { }
    }
    public class Action_10003_ArmorMultiplyAdditive : ActionAfterUse
    {
        public override int m_Index => 10003;
        public override int I_ActionCost => ActionData.I_10003_Cost;
        public override float Value1 => ActionData.IP_10003_ArmorMultiplyAdditive(m_Level);
        public override void OnActionUse() {
            base.OnActionUse();
            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1 / 100f * m_ActionEntity.m_HealthManager.m_CurrentArmor, enum_DamageType.ArmorOnly);
        } 
        public Action_10003_ArmorMultiplyAdditive(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_10004_ArmorActionReturn : ActionAfterUse
    {
        public override int m_Index => 10004;
        public override int I_ActionCost => ActionData.I_10004_Cost;
        public override float Value1 => ActionData.F_10004_ArmorActionAcquire(m_Level);
        public override void OnActionUse()
        {
            base.OnActionUse();
            ActionHelper.PlayerReceiveActionAmount(m_ActionEntity, Value1* m_ActionEntity.m_HealthManager.m_CurrentArmor);
        } 
        public Action_10004_ArmorActionReturn(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_10005_ArmorDamageReduction : ActionAfterDuration
    {
        public override int m_Index => 10005;
        public override int m_EffectIndex => base.m_EffectIndex;
        public override int I_ActionCost => ActionData.I_10005_Cost;
        public override float Value1 => ActionData.IP_10005_ArmorDamageReduction(m_Level);
        public override float m_DamageReduction=> Value1 / 100f;
        public Action_10005_ArmorDamageReduction(enum_RarityLevel _level) : base(_level, ActionData.F_10005_Duration) { }
    }
    public class Action_10006_FireRateAdditive: ActionAfterDuration
    {
        public override int m_Index => 10006;
        public override int I_ActionCost => ActionData.I_10006_Cost;
        public override float Value1 =>ActionData.IP_10006_FireRateAdditive(m_Level);
        public override float m_FireRateMultiply => Value1 / 100f;
        public Action_10006_FireRateAdditive(enum_RarityLevel _level) : base(_level, ActionData.F_10006_Duration) { }
    }
    public class Action_10007_RecoilReduction : ActionAfterDuration
    {
        public override int m_Index => 10007;
        public override int I_ActionCost => ActionData.I_10007_Cost;
        public override float Value1 => ActionData.IP_10007_RecoilMultiplyAdditive(m_Level);
        public override float F_RecoilReduction => Value1 / 100f;
        public Action_10007_RecoilReduction(enum_RarityLevel _level) : base(_level, ActionData.F_10007_Duration) { }
    }
    public class Action_10008_ClipMultiply : ActionAfterDuration
    {
        public override int m_Index => 10008;
        public override int I_ActionCost => ActionData.I_10008_Cost;
        public override float Value1 => ActionData.IP_10008_ClipMultiply(m_Level) ;
        public override float F_ClipMultiply => Value1 / 100f;
        public Action_10008_ClipMultiply(enum_RarityLevel _level) : base(_level, ActionData.F_10008_Duration) { }
    }
    public class Action_10009_ProjectileSpeedMultiply : ActionAfterDuration
    {
        public override int m_Index => 10009;
        public override int I_ActionCost => ActionData.I_10009_Cost;
        public override float Value1 => ActionData.IP_10009_BulletSpeedAdditive(m_Level);
        public override float F_ProjectileSpeedMultiply => Value1 / 100f;
        public Action_10009_ProjectileSpeedMultiply(enum_RarityLevel _level) : base(_level, ActionData.F_10009_Duration) { }
    }
    public class Action_10010_SingleDamageMultiply : ActionAfterFire
    {
        public override int m_Index => 10010;
        public override int I_ActionCost => ActionData.I_10010_Cost;
        public override float Value1 => ActionData.IP_10010_SingleDamageMultiply(m_Level) ;
        public override float m_DamageMultiply => Value1 / 100f;
        public Action_10010_SingleDamageMultiply(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_10011_SingleDamageKillActionReturn : ActionAfterFire
    {
        public override int m_Index => 10011;
        public override int I_ActionCost => ActionData.I_10011_Cost;
        public override float Value1 => ActionData.IP_10011_SingleDamageMultiply(m_Level) ;
        public override float Value2 => ActionData.F_10011_EntityKillActionReturn(m_Level);
        public override float m_DamageMultiply => Value1 / 100f;
        protected override bool OnActionHit( EntityBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerReceiveActionAmount(m_ActionEntity, Value2);
            return true;
        }
        public Action_10011_SingleDamageKillActionReturn(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_10012_SingleDamageKillHealing : ActionAfterFire
    {
        public override int m_Index => 10012;
        public override int I_ActionCost => ActionData.I_10012_Cost;
        public override float Value1 => ActionData.IP_10012_SingleDamageMultiply(m_Level) ;
        public override float Value2 => ActionData.F_10012_EntityKillHealing(m_Level);
        public override float m_DamageMultiply => Value1 / 100f;
        protected override bool OnActionHit( EntityBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.HealthOnly);
            return true;
        }
        public Action_10012_SingleDamageKillHealing(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_10013_SingleProjectileKillActionUpgrade : ActionAfterFire
    {
        public override int m_Index => 10013;
        public override int I_ActionCost => ActionData.I_10013_Cost;
        public override float Value1 => ActionData.IP_10013_SingleDamageMultiply(m_Level) ;
        public override float m_DamageMultiply => Value1 / 100f;
        protected override bool OnActionHit( EntityBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerUpgradeAction(m_ActionEntity);
            return true;
        }
        public Action_10013_SingleProjectileKillActionUpgrade(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_10014_ReloadRateMultiply : ActionAfterDuration
    {
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterDuration;
        public override int m_Index => 10014;
        public override int I_ActionCost => ActionData.I_10014_Cost;
        public override float Value1 => ActionData.IP_10014_ReloadRateMultiplyPercentage(m_Level);
        public override float m_ReloadRateMultiply => Value1/100f;
        public Action_10014_ReloadRateMultiply(enum_RarityLevel _level) : base(_level, ActionData.F_10014_Duration) { }
    }
    #endregion
    #region EquipmentItem
    public class Action_20001_Armor_Turret_Cannon : ActionAfterUse
    {
        public override int m_Index => 20001;
        public override int I_ActionCost => ActionData.I_20001_Cost;
        public override float Value1 => ActionData.F_20001_Health(m_Level);
        public override float Value2 => ActionData.F_20001_ArmorTurretDamage(m_Level);
        public override float Value3 => ActionData.F_20001_TurretMinumumDamage(m_Level);
        public override void OnActionUse() {
            base.OnActionUse();
            ActionHelper.PlayerAcquireEntityEquipmentItem(m_ActionEntity, m_Index, (int)(Value1* m_ActionEntity.m_HealthManager.m_CurrentArmor), 1f, GetDamageInfo);
        }
        public DamageDeliverInfo GetDamageInfo()=> DamageDeliverInfo.EquipmentInfo(m_ActionEntity.I_EntityID, Value3+Value2 * m_ActionEntity.m_HealthManager.m_CurrentArmor, -1);
        public Action_20001_Armor_Turret_Cannon(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_20002_FireRate_FrozenGrenade : ActionAfterUse
    {
        public override int m_Index => 20002;
        public override int I_ActionCost => ActionData.I_20002_Cost;
        public override float Value1=> ActionData.F_20002_DamageDealt(m_Level);
        public override float Value2=> ActionData.F_20002_BuffIndex(m_Level);
        public override float Value3 => GameDataManager.GetEntityBuffProperties((int)ActionData.F_20002_BuffIndex(m_Level)).m_ExpireDuration;
        public override void OnActionUse()
        {
            base.OnActionUse();
            ActionHelper.PlayerAcquireSimpleEquipmentItem(m_ActionEntity, m_Index, Value1, (int)Value2);
        }
        public Action_20002_FireRate_FrozenGrenade(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_20003_Firerate_Turret_Minigun : ActionAfterUse
    {
        public override int m_Index => 20003;
        public override int I_ActionCost => ActionData.I_20003_Cost;
        public override float Value1 => ActionData.F_20003_FireRate(m_Level);
        public override float Value2 => ActionData.F_20003_DamageDealt(m_Level) ;
        public override float Value3 => ActionData.F_20003_Health(m_Level);
        public override void OnActionUse()
        {
            base.OnActionUse();
            ActionHelper.PlayerAcquireEntityEquipmentItem(m_ActionEntity, m_Index, (int)Value3, Value1 ,GetDamageInfo);
        }
        public DamageDeliverInfo GetDamageInfo() => DamageDeliverInfo.EquipmentInfo(m_ActionEntity.I_EntityID, Value2 * m_ActionEntity.m_WeaponCurrent.F_BaseDamage, -1);
        public Action_20003_Firerate_Turret_Minigun(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_20004_Damage_ExplosiveGrenade : ActionAfterUse
    {
        public override int m_Index => 20004;
        public override int I_ActionCost => ActionData.I_20004_Cost;
        public override float Value1 => ActionData.F_20004_DamageDealt(m_Level);
        public override void OnActionUse() {
            base.OnActionUse();
            ActionHelper.PlayerAcquireSimpleEquipmentItem(m_ActionEntity, m_Index, Value1* m_ActionEntity.m_WeaponCurrent.F_BaseDamage);
        }
        public Action_20004_Damage_ExplosiveGrenade(enum_RarityLevel _level) : base(_level) { }
    }
    #endregion
    #region LevelEquipment
    public class Action_30001_ArmorActionAdditive : ActionAfterBattle
    {
        public override int m_Index => 30001;
        public override int I_ActionCost => ActionData.I_30001_Cost;
        public override float Value1 => ActionData.F_30001_ArmorActionAdditive(m_Level);
        public override void OnAddActionElse(float actionAmount) => ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
        public Action_30001_ArmorActionAdditive(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_30002_ArmorDemageReturn : ActionAfterBattle
    {
        public override int m_Index => 30002;
        public override int I_ActionCost => ActionData.I_30002_Cost;
        public override float Value1 => ActionData.F_30002_ArmorDamageReturn(m_Level);
        public override void OnReceiveDamage(int applier, float amount) => ActionHelper.PlayerDealtDamageToEntity(m_ActionEntity, applier,Value1* m_ActionEntity.m_HealthManager.m_CurrentArmor);
        public Action_30002_ArmorDemageReturn(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_30003_DamageAdditive : ActionAfterBattle
    {
        public override int m_Index => 30003;
        public override int I_ActionCost => ActionData.I_30003_Cost;
        public override float Value1 => ActionData.F_30003_DamageAdditive(m_Level);
        public override float F_DamageAdditive => Value1;
        public Action_30003_DamageAdditive(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_30004_ClipOverrideDamageAdditive : ActionAfterBattle
    {
        public override int m_Index => 30004;
        public override int I_ActionCost => ActionData.I_30004_Cost;
        public override bool B_ClipOverride => true;
        public override float Value1 => ActionData.F_30004_DamageAdditive(m_Level);
        public override float F_DamageAdditive=> Value1;
        public Action_30004_ClipOverrideDamageAdditive(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_30005_ReloadHeal : ActionAfterBattle_ReloadTrigger
    {
        public override int m_Index => 30005;
        public override int I_ActionCost => ActionData.I_30005_Cost;
        public override float Value1 => ActionData.F_30005_ReloadTimesHeal(m_Level);
        public override float Value2 => ActionData.F_30005_ReloadHealAmount(m_Level);
        protected override void OnReloadTrigger()=> ActionHelper.PlayerReceiveHealing(m_ActionEntity,Value2, enum_DamageType.HealthOnly);
        public Action_30005_ReloadHeal(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_30006_ReloadDamageMultiply : ActionAfterBattle_ReloadTrigger
    {
        public override int m_Index => 30006;
        public override int I_ActionCost => ActionData.I_30006_Cost;
        public override float Value1 => ActionData.I_30006_ReloadTimesCount(m_Level);
        public override float Value2 => ActionData.IP_30006_ReloadDamageMultiplyPercentage(m_Level);
        public override float m_DamageMultiply =>  m_TriggerOn?Value2 / 100f : 0;
        bool m_TriggerOn = false;
        public override void OnReloadFinish()
        {
            m_TriggerOn = false;
            base.OnReloadFinish();
        }
        protected override void OnReloadTrigger() => m_TriggerOn = true;
        public Action_30006_ReloadDamageMultiply(enum_RarityLevel _level) : base(_level) { }
    }
    #endregion
    #region WeaponAction
    public class Action_40001_DealtDamageAddArmor : ActionAfterWeaponDetach
    {
        public override int m_Index => 40001;
        float m_TotalDamageDealt=0;
        public override float Value1 => ActionData.F_40001_DamageDealtCount(m_Level);
        public override float Value2 => ActionData.F_40001_ArmorAdditive(m_Level);
        public override void OnDealtDemage( EntityBase receiver, float amount)
        {
            base.OnDealtDemage( receiver, amount);
            m_TotalDamageDealt += amount;
            if (m_TotalDamageDealt < Value1)
                return;
            m_TotalDamageDealt -= Value1;

            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.ArmorOnly);
        }
        public Action_40001_DealtDamageAddArmor(enum_RarityLevel _level) : base(_level) { }
    }

    public class Action_40002_DealtDamageAddActionRandom : ActionAfterWeaponDetach
    {
        public override int m_Index => 40002;
        public override float Value1 => ActionData.IP_40002_DamageDealtAddActionRate(m_Level);
        public override float Value2 => ActionData.F_40002_AddActionAmount(m_Level);
        public override void OnAfterFire(int identity)
        {
            base.OnAfterFire(identity);
            if (TCommon.RandomPercentage() < Value1)
                ActionHelper.PlayerReceiveActionAmount(m_ActionEntity, Value2);
        }
        public Action_40002_DealtDamageAddActionRandom(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_40003_FireTimesDamageAdditive : ActionAfterWeaponDetach
    {
        public override int m_Index => 40003;
        public override float Value1=> ActionData.F_40003_FireTimesCount(m_Level);
        public override float Value2 => ActionData.F_40003_DamageAdditive(m_Level);
        bool m_triggerd = false;
        int fireCount = 0;
        public override float F_DamageAdditive => m_triggerd ? Value2 : 0;
        public override void OnAfterFire(int identity)
        {
            base.OnAfterFire(identity);
            m_triggerd = false;
            fireCount++;
            if (fireCount < Value1)
                return;
            fireCount -= (int)Value1;
            m_triggerd = true;
        }
        public Action_40003_FireTimesDamageAdditive(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_40007_DamageReductionCooldown : ActionAfterWeaponDetach
    {
        public override int m_Index => 40007;
        public override int m_EffectIndex => m_activating ? 40002 : -1;
        public override float m_EffectDuration => Value1;
        public override float Value1 => ActionData.F_40007_DamageReductionDuration(m_Level);
        public override float Value2 => ActionData.F_40007_ArmorAdditive(m_Level);
        public override float Value3 => ActionData.F_40007_Cooldown(m_Level);
        public override float m_DamageReduction => m_activating ? 1f : 0;
        float m_cooldownCheck=-1f;
        bool m_cooldowning => m_cooldownCheck >= 0;
        float m_activateCheck=-1f;
        bool m_activating => m_activateCheck >= 0;
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            if (m_cooldowning)
                m_cooldownCheck -= deltaTime;

            if (m_activating)
                m_activateCheck -= deltaTime;
        }
        public override void OnReceiveDamage(int applier,float amount)
        {
            base.OnReceiveDamage(applier, amount);
            if (m_activating||m_cooldowning)
                return;

            if (m_ActionEntity.m_HealthManager.m_CurrentArmor <= 0)
            {
                m_activateCheck += Value1;
                m_cooldownCheck += Value3;
            }
        }
        public Action_40007_DamageReductionCooldown(enum_RarityLevel _level) : base(_level) { }
    }
    public class Action_40012_UseActionReturn : ActionAfterWeaponDetach
    {
        public override int m_Index => 40012;
        public override float Value1 => ActionData.F_40012_ActionReturn(m_Level);
        public override void OnAddActionElse(float actionAmount)
        {
            base.OnAddActionElse(actionAmount);
            ActionHelper.PlayerReceiveActionAmount(m_ActionEntity,Value1);
        }
        public Action_40012_UseActionReturn(enum_RarityLevel _level) : base(_level) { }
    }

    public class Action_40014_KillArmorAdditive : ActionAfterWeaponDetach
    {
        public override int m_Index => 40014;
        public override float Value1 => ActionData.F_40014_ArmorAdditive(m_Level);
        public override void OnDealtDemage(EntityBase receiver, float amount)
        {
            base.OnDealtDemage(receiver, amount);
            if (receiver.m_HealthManager.b_IsDead)
                ActionHelper.PlayerReceiveHealing(m_ActionEntity,Value1, enum_DamageType.ArmorOnly);
        }
        public Action_40014_KillArmorAdditive(enum_RarityLevel _level) : base(_level) { }
    }
        #endregion
    }