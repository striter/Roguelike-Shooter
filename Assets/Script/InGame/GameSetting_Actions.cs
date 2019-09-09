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
        public const int I_10015_Cost = 2;
        public const int I_10016_Cost = 2;
        public const int I_10017_Cost = 1;
        public static int I_10018_Cost(enum_RarityLevel level) => 3-(int)level;

        public const int I_20001_Cost = 2;
        public const int I_20002_Cost = 1;
        public const int I_20003_Cost = 2;
        public const int I_20004_Cost = 1;
        public const int I_20005_Cost = 1;

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
        public const float F_10015_Duration = 30f;
        public const float F_10016_Duration = 30f;
        public const float F_10018_Duration = 10f;
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
        public static float F_10015_ArmorDamageReturn(enum_RarityLevel level) => 2 * (int)level;
        public static float IP_10016_DamageReducePercentage(enum_RarityLevel level) => 65 - 15 * (int)level;
        public static float IP_10016_FireRateAdditivePercentage(enum_RarityLevel level) => 60 + 60 * (int)level;
        public static float IP_10017_DamageAdditivePercentage(enum_RarityLevel level) => 200 + 200 * (int)level;
        public static float F_10017_EntityKillArmorAdditive(enum_RarityLevel level) => 40 + 30 * (int)level;

        public static float F_20001_Health(enum_RarityLevel level) => 200;
        public static float F_20001_ArmorTurretDamage(enum_RarityLevel level) => 1.5f * (int)level;
        public static float F_20001_TurretMinumumDamage(enum_RarityLevel level) => 50f;
        public static float F_20002_DamageDealt(enum_RarityLevel level) => 50f;
        public static float F_20002_BuffIndex(enum_RarityLevel level) => 200020 + (int)level;
        public static float F_20003_Health(enum_RarityLevel level) => 200;
        public static float F_20003_FireRate(enum_RarityLevel level) => 1f;
        public static float F_20003_DamageDealt(enum_RarityLevel level) => .7f * (int)level;
        public static float F_20004_DamageDealt(enum_RarityLevel level) => 2 + 2 * (int)level;
        public static float IP_20005_BaseShieldHealthPercentage(enum_RarityLevel level) => 50;
        public static float IP_20005_BonusShieldHealthPercentage(enum_RarityLevel level) => 50 * (int)level;
        public static float F_20005_MinimumShieldHealth(enum_RarityLevel level) => 50;

        public static float F_30001_ArmorActionAdditive(enum_RarityLevel level) => 10 + 10 * (int)level;
        public static float F_30002_ReceiveDamageArmorAdditive(enum_RarityLevel level) => 3 + 3 * (int)level;
        public static float F_30003_DamageAdditive(enum_RarityLevel level) => 20f * (int)level;
        public static float F_30004_DamageAdditive(enum_RarityLevel level) => 200 * (int)level;
        public static float F_30005_DamageDealtHeal(enum_RarityLevel level) => 600 - 100 * (int)level;
        public static float F_30005_HealAmount(enum_RarityLevel level) => 20f;
        public static int I_30006_ReloadTimesCount(enum_RarityLevel level) => 1;
        public static int IP_30006_ReloadDamageMultiplyPercentage(enum_RarityLevel level) => 100 * (int)level;

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
        public static void PlayerAcquireSimpleEquipmentItem(EntityCharacterPlayer player, int equipmentIndex,float damage,int buffIndex=-1)
        {
            player.OnAcquireEquipment<EquipmentBase>(equipmentIndex, () => { return DamageDeliverInfo.EquipmentInfo(player.I_EntityID,damage,buffIndex); });
        }
        public static void PlayerAcquireEntityEquipmentItem(EntityCharacterPlayer player, int equipmentIndex,int health,float fireRate, Func<DamageDeliverInfo> damage)
        {
            player.OnAcquireEquipment<EquipmentEntitySpawner>(equipmentIndex,damage).SetOnSpawn((EntityCharacterBase entity) => {
                EntityCharacterAI target = entity as EntityCharacterAI;
                target.I_MaxHealth = health;
                target.F_AttackDuration = new RangeFloat(0f, 0);
                target.F_AttackTimes = new RangeInt(1, 0);
                target.F_AttackRate = fireRate;
            });
        }
        public static void PlayerAttachShield(EntityCharacterPlayer player, int equipmentIndex, int health)
        {
            player.OnUseEquipment(equipmentIndex,(EquipmentShieldAttach attach)=> {
                attach.SetOnSpawn((SFXShield shield) => {
                    shield.m_Health.I_MaxHealth = health;
                });
            });
        }
        public static void PlayerDealtDamageToEntity(EntityCharacterPlayer player,int targetID, float damageAmount, enum_DamageType damageType= enum_DamageType.Common)
        {
            if (damageAmount < 0)
                Debug.LogError("Howd Fk Damage Below Zero?");
            GameManager.Instance.GetEntity(targetID).m_HitCheck.TryHit(new DamageInfo(damageAmount, damageType, DamageDeliverInfo.Default(player.I_EntityID)));
        }
        public static void PlayerReceiveHealing(EntityCharacterPlayer player,float heal,enum_DamageType type= enum_DamageType.Common)
        {
            Debug.Log("Player Receive Healing Amount:" + heal);
            if (heal <= 0)
                Debug.LogError("Howd Fk Healing Below Zero?");
            player.m_HitCheck.TryHit(new DamageInfo(-heal,type, DamageDeliverInfo.Default(player.I_EntityID)));
        }
        public static void PlayerReceiveActionAmount(EntityCharacterPlayer player, float amount)
        {
            Debug.Log("Player Receive Aciton Amount:" + amount);
            player.m_PlayerInfo.AddActionAmount(amount);
        }
        public static void PlayerUpgradeAction(EntityCharacterPlayer player)
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
        public ActionAfterUse(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public bool OnActionHitEntity(int damageIdentity, EntityCharacterBase _targetEntity)
        {
            if (m_fireIdentity != damageIdentity)
                return false;
            return OnActionHit(_targetEntity);
        }

        protected virtual bool OnActionHit( EntityCharacterBase _hitEntity)
        {
            return false;
        }
        public ActionAfterFire(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class ActionAfterBattle : ActionBase
    {
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterBattle;
        public override void OnAfterBattle()
        {
            base.OnAfterBattle();
            ForceExpire();
        } 
        public ActionAfterBattle(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public ActionAfterBattle_ReloadTrigger(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class ActionAfterWeaponDetach : ActionBase
    {
        public override int I_ActionCost => 0;
        public override enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.AfterWeaponSwitch;
        public override void OnWeaponDetach() => ForceExpire();
        public ActionAfterWeaponDetach(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_10001_ArmorAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_10002_ArmorDamageAdditive : ActionBase
    {
        public override int m_Index => 10002;
        protected override float F_Duration => ActionData.F_10002_Duration;
        public override int I_ActionCost => ActionData.I_10002_Cost;
        public override float Value1 => ActionData.F_10002_ArmorDamageAdditive(m_Level);
        public override float F_DamageAdditive =>  Value1* m_ActionEntity.m_HealthManager.m_CurrentArmor;
        public Action_10002_ArmorDamageAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
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
        public Action_10003_ArmorMultiplyAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_10004_ArmorActionReturn(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_10005_ArmorDamageReduction : ActionBase
    {
        public override int m_Index => 10005;
        protected override float F_Duration => ActionData.F_10005_Duration;
        public override int m_EffectIndex => base.m_EffectIndex;
        public override int I_ActionCost => ActionData.I_10005_Cost;
        public override float Value1 => ActionData.IP_10005_ArmorDamageReduction(m_Level);
        public override float m_DamageReduction=> Value1 / 100f;
        public Action_10005_ArmorDamageReduction(int _identity, enum_RarityLevel _level) : base(_identity ,_level) { }
    }
    public class Action_10006_FireRateAdditive: ActionBase
    {
        public override int m_Index => 10006;
        public override int I_ActionCost => ActionData.I_10006_Cost;
        public override float Value1 =>ActionData.IP_10006_FireRateAdditive(m_Level);
        public override float m_FireRateMultiply => Value1 / 100f;
        protected override float F_Duration => ActionData.F_10006_Duration;
        public Action_10006_FireRateAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    public class Action_10007_RecoilReduction : ActionBase
    {
        public override int m_Index => 10007;
        public override int I_ActionCost => ActionData.I_10007_Cost;
        public override float Value1 => ActionData.IP_10007_RecoilMultiplyAdditive(m_Level);
        public override float F_RecoilReduction => Value1 / 100f;
        protected override float F_Duration => ActionData.F_10007_Duration;
        public Action_10007_RecoilReduction(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    public class Action_10008_ClipMultiply : ActionBase
    {
        public override int m_Index => 10008;
        public override int I_ActionCost => ActionData.I_10008_Cost;
        public override float Value1 => ActionData.IP_10008_ClipMultiply(m_Level) ;
        public override float F_ClipMultiply => Value1 / 100f;
        protected override float F_Duration => ActionData.F_10008_Duration;
        public Action_10008_ClipMultiply(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    public class Action_10009_ProjectileSpeedMultiply : ActionBase
    {
        public override int m_Index => 10009;
        public override int I_ActionCost => ActionData.I_10009_Cost;
        public override float Value1 => ActionData.IP_10009_BulletSpeedAdditive(m_Level);
        public override float F_ProjectileSpeedMultiply => Value1 / 100f;
        protected override float F_Duration => ActionData.F_10009_Duration;
        public Action_10009_ProjectileSpeedMultiply(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    public class Action_10010_SingleDamageMultiply : ActionAfterFire
    {
        public override int m_Index => 10010;
        public override int I_ActionCost => ActionData.I_10010_Cost;
        public override float Value1 => ActionData.IP_10010_SingleDamageMultiply(m_Level) ;
        public override float m_DamageMultiply => Value1 / 100f;
        public Action_10010_SingleDamageMultiply(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_10011_SingleDamageKillActionReturn : ActionAfterFire
    {
        public override int m_Index => 10011;
        public override int I_ActionCost => ActionData.I_10011_Cost;
        public override float Value1 => ActionData.IP_10011_SingleDamageMultiply(m_Level) ;
        public override float Value2 => ActionData.F_10011_EntityKillActionReturn(m_Level);
        public override float m_DamageMultiply => Value1 / 100f;
        protected override bool OnActionHit( EntityCharacterBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerReceiveActionAmount(m_ActionEntity, Value2);
            return true;
        }
        public Action_10011_SingleDamageKillActionReturn(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_10012_SingleDamageKillHealing : ActionAfterFire
    {
        public override int m_Index => 10012;
        public override int I_ActionCost => ActionData.I_10012_Cost;
        public override float Value1 => ActionData.IP_10012_SingleDamageMultiply(m_Level) ;
        public override float Value2 => ActionData.F_10012_EntityKillHealing(m_Level);
        public override float m_DamageMultiply => Value1 / 100f;
        protected override bool OnActionHit( EntityCharacterBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.HealthOnly);
            return true;
        }
        public Action_10012_SingleDamageKillHealing(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_10013_SingleProjectileKillActionUpgrade : ActionAfterFire
    {
        public override int m_Index => 10013;
        public override int I_ActionCost => ActionData.I_10013_Cost;
        public override float Value1 => ActionData.IP_10013_SingleDamageMultiply(m_Level) ;
        public override float m_DamageMultiply => Value1 / 100f;
        protected override bool OnActionHit( EntityCharacterBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerUpgradeAction(m_ActionEntity);
            return true;
        }
        public Action_10013_SingleProjectileKillActionUpgrade(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_10014_ReloadRateMultiply : ActionBase
    {
        public override int m_Index => 10014;
        public override int I_ActionCost => ActionData.I_10014_Cost;
        public override float Value1 => ActionData.IP_10014_ReloadRateMultiplyPercentage(m_Level);
        public override float m_ReloadRateMultiply => Value1/100f;
        protected override float F_Duration => ActionData.F_10014_Duration;
        public Action_10014_ReloadRateMultiply(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    public class Action_10015_ArmorDemageReturn : ActionBase
    {
        public override int m_Index => 10015;
        public override int I_ActionCost => ActionData.I_10015_Cost;
        public override float Value1 => ActionData.F_10015_ArmorDamageReturn(m_Level);
        public override void OnReceiveDamage(int applier, float amount) => ActionHelper.PlayerDealtDamageToEntity(m_ActionEntity, applier, Value1 * m_ActionEntity.m_HealthManager.m_CurrentArmor);
        protected override float F_Duration => ActionData.F_10015_Duration;
        public Action_10015_ArmorDemageReturn(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    public class Action_10016_FireBurstDamageReduce : ActionBase
    {
        public override int m_Index => 10016;
        public override int I_ActionCost => ActionData.I_10016_Cost;
        public override float Value1 => ActionData.IP_10016_DamageReducePercentage(m_Level);
        public override float Value2 => ActionData.IP_10016_FireRateAdditivePercentage(m_Level);
        public override float m_DamageMultiply => -Value1/100f;
        public override float m_FireRateMultiply => Value2/100f;
        protected override float F_Duration => ActionData.F_10016_Duration;
        public Action_10016_FireBurstDamageReduce(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }

    public class Action_10017_SingleProjectileKillArmorAdditive : ActionAfterFire
    {
        public override int m_Index => 10017;
        public override int I_ActionCost => ActionData.I_10017_Cost;
        public override float Value1 => ActionData.IP_10017_DamageAdditivePercentage(m_Level);
        public override float Value2 => ActionData.F_10017_EntityKillArmorAdditive(m_Level);
        public override float m_DamageMultiply => Value1 / 100f;
        protected override bool OnActionHit(EntityCharacterBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.ArmorOnly);
            return true;
        }
        public Action_10017_SingleProjectileKillArmorAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    public class Action_10018_Vanish : ActionBase
    {
        public override int m_Index => 10018;
        public override int I_ActionCost => ActionData.I_10018_Cost(m_Level);
        protected override float F_Duration => ActionData.F_10018_Duration;
        public override float F_CloakDuration => ActionData.F_10018_Duration;
        public Action_10018_Vanish(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
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
            ActionHelper.PlayerAcquireEntityEquipmentItem(m_ActionEntity, m_Index,  (int)(Value1* m_ActionEntity.m_HealthManager.m_CurrentArmor), 1f, GetDamageInfo);
        }
        public DamageDeliverInfo GetDamageInfo()=> DamageDeliverInfo.EquipmentInfo(m_ActionEntity.I_EntityID, Value3+Value2 * m_ActionEntity.m_HealthManager.m_CurrentArmor, -1);
        public Action_20001_Armor_Turret_Cannon(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_20002_FireRate_FrozenGrenade(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_20003_Firerate_Turret_Minigun(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_20004_Damage_ExplosiveGrenade(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_20005_Damage_ExplosiveGrenade : ActionAfterUse
    {
        public override int m_Index => 20005;
        public override int I_ActionCost => ActionData.I_20005_Cost;
        public override float Value1 => ActionData.IP_20005_BaseShieldHealthPercentage(m_Level);
        public override float Value2 => ActionData.IP_20005_BonusShieldHealthPercentage(m_Level);
        public override float Value3 => ActionData.F_20001_TurretMinumumDamage(m_Level);
        public override void OnActionUse()
        {
            base.OnActionUse();
            float shieldHealth = Value1 + Value2;
            shieldHealth = shieldHealth > Value3 ? shieldHealth : Value3;
            ActionHelper.PlayerAttachShield(m_ActionEntity, m_Index, (int)shieldHealth);
        }
        public Action_20005_Damage_ExplosiveGrenade(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    #endregion
    #region LevelEquipment
    public class Action_30001_ArmorActionAdditive : ActionAfterBattle
    {
        public override int m_Index => 30001;
        public override int I_ActionCost => ActionData.I_30001_Cost;
        public override float Value1 => ActionData.F_30001_ArmorActionAdditive(m_Level);
        public override void OnAddActionElse(float actionAmount) => ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
        public Action_30001_ArmorActionAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_30002_ArmorActionAdditive : ActionAfterBattle
    {
        public override int m_Index => 30002;
        public override int I_ActionCost => ActionData.I_30002_Cost;
        public override float Value1 => ActionData.F_30002_ReceiveDamageArmorAdditive(m_Level);
        public override void OnReceiveDamage(int applier, float amount)
        {
            base.OnReceiveDamage(applier, amount);
            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value1, enum_DamageType.ArmorOnly);
        }
        public Action_30002_ArmorActionAdditive(int _identity, enum_RarityLevel _level) : base(_identity, _level) { }
    }
    public class Action_30003_DamageAdditive : ActionAfterBattle
    {
        public override int m_Index => 30003;
        public override int I_ActionCost => ActionData.I_30003_Cost;
        public override float Value1 => ActionData.F_30003_DamageAdditive(m_Level);
        public override float F_DamageAdditive => Value1;
        public Action_30003_DamageAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_30004_ClipOverrideDamageAdditive : ActionAfterBattle
    {
        public override int m_Index => 30004;
        public override int I_ActionCost => ActionData.I_30004_Cost;
        public override bool B_ClipOverride => true;
        public override float Value1 => ActionData.F_30004_DamageAdditive(m_Level);
        public override float F_DamageAdditive=> Value1;
        public Action_30004_ClipOverrideDamageAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    public class Action_30005_DamageHeal : ActionAfterBattle
    {
        public override int m_Index => 30005;
        public override int I_ActionCost => ActionData.I_30005_Cost;
        public override float Value1 => ActionData.F_30005_DamageDealtHeal(m_Level);
        public override float Value2 => ActionData.F_30005_HealAmount(m_Level);
        float m_damageDealt = 0;
        public override void OnDealtDemage(EntityCharacterBase receiver, float amount)
        {
            base.OnDealtDemage(receiver, amount);
            m_damageDealt += amount;
            if (m_damageDealt < Value1)
                return;
            m_damageDealt -= Value1;
            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.HealthOnly);
        }
        public Action_30005_DamageHeal(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_30006_ReloadDamageMultiply(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
    #endregion
    #region WeaponAction
    public class Action_40001_DealtDamageAddArmor : ActionAfterWeaponDetach
    {
        public override int m_Index => 40001;
        float m_TotalDamageDealt=0;
        public override float Value1 => ActionData.F_40001_DamageDealtCount(m_Level);
        public override float Value2 => ActionData.F_40001_ArmorAdditive(m_Level);
        public override void OnDealtDemage( EntityCharacterBase receiver, float amount)
        {
            base.OnDealtDemage( receiver, amount);
            m_TotalDamageDealt += amount;
            if (m_TotalDamageDealt < Value1)
                return;
            m_TotalDamageDealt -= Value1;

            ActionHelper.PlayerReceiveHealing(m_ActionEntity, Value2, enum_DamageType.ArmorOnly);
        }
        public Action_40001_DealtDamageAddArmor(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_40002_DealtDamageAddActionRandom(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_40003_FireTimesDamageAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_40007_DamageReductionCooldown(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
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
        public Action_40012_UseActionReturn(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }

    public class Action_40014_KillArmorAdditive : ActionAfterWeaponDetach
    {
        public override int m_Index => 40014;
        public override float Value1 => ActionData.F_40014_ArmorAdditive(m_Level);
        public override void OnDealtDemage(EntityCharacterBase receiver, float amount)
        {
            base.OnDealtDemage(receiver, amount);
            if (receiver.m_HealthManager.b_IsDead)
                ActionHelper.PlayerReceiveHealing(m_ActionEntity,Value1, enum_DamageType.HealthOnly);
        }
        public Action_40014_KillArmorAdditive(int _identity,enum_RarityLevel _level) : base(_identity,_level) { }
    }
        #endregion
    }