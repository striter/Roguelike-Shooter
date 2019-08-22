using GameSetting;
using UnityEngine;
using System;
using System.Collections.Generic;
namespace GameSetting_Action
{
    public static class ActionData
    {
        #region Cost
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

        #region Duration
        public const float F_10002_Duration = 10f;
        public const float F_10005_Duration = 30f;
        public const float F_10006_Duration = 20f;
        public const float F_10007_Duration = 20f;
        public const float F_10008_Duration = 30f;
        public const float F_10009_Duration = 30f;
        public const float F_10014_Duration = 30f;
        #endregion

        #region Expression
        public static float F_10001_ArmorAdditive(enum_ActionLevel level) => -30f * (int)level;
        public static float F_10002_ArmorDamageAdditive(enum_ActionLevel level, float currentArmor) => currentArmor * 1f;
        public static float F_10003_ArmorMultiplyAdditive(enum_ActionLevel level, float currentArmor) => -currentArmor * .3f * (int)level;
        public static float F_10004_ArmorActionAcquire(enum_ActionLevel level, float currentArmor) => currentArmor / (20f + 10f * (int)level);
        public static float F_10005_ArmorDamageReduction(enum_ActionLevel level) => .2f * (int)level;
        public static float F_10006_FireRateAdditive(enum_ActionLevel level) => .3f * (int)level;
        public static float F_10007_RecoilMultiplyAdditive(enum_ActionLevel level) => -.3f * (int)level;
        public static float F_10008_ClipMultiply(enum_ActionLevel level) => .2f * (int)level;
        public static float F_10009_BulletSpeedAdditive(enum_ActionLevel level) => .3f * (int)level;
        public static float F_10010_SingleDamageMultiply(enum_ActionLevel level) => 4 * (int)level;
        public static float F_10011_SingleDamageMultiply(enum_ActionLevel level) => 2 * (int)level;
        public static float F_10011_EntityKillActionReturn(enum_ActionLevel level) => 1 + (int)level;
        public static float F_10012_SingleDamageMultiply(enum_ActionLevel level) => 2 * (int)level;
        public static float F_10012_EntityKillHealing(enum_ActionLevel level) => -30 * (int)level;
        public static float F_10013_SingleDamageMultiply(enum_ActionLevel level) => 2 * (int)level;
        public static float F_10014_ReloadRateMultiplyAdditive(enum_ActionLevel level) => .3f * (int)level;

        public static float F_20001_ArmorTurretHealth(enum_ActionLevel level, float currentArmor) => currentArmor * (2 * (int)level);
        public static float F_20001_ArmorTurretDamage(enum_ActionLevel level, float currentArmor) => currentArmor * (1.5f * (int)level);
        public static float F_20002_DamageDealt(enum_ActionLevel level) => 50f;
        public static float F_20002_BuffIndex(enum_ActionLevel level) => 200020 + (int)level;
        public static float F_20003_Health(enum_ActionLevel level) => 400;
        public static float F_20003_FireRate(enum_ActionLevel level, float weaponFirerate) =>  weaponFirerate / (.7f * (int)level);
        public static float F_20003_DamageDealt(enum_ActionLevel level, float weaponDamage) => .7f * weaponDamage * (int)level;
        public static float F_20004_DamageDealt(enum_ActionLevel level, float weaponDamage) => weaponDamage * 2 * (int)level;

        public static float F_30001_ArmorActionAdditive(enum_ActionLevel level) => 10 * (int)level;
        public static float F_30002_ArmorDamageReturn(enum_ActionLevel level, float currentArmor) => currentArmor * (2 * (int)level);
        public static float F_30003_DamageAdditive(enum_ActionLevel level) => 20f * (int)level;
        public static float F_30004_DamageAdditive(enum_ActionLevel level) => 200 * (int)level;
        public static float F_30005_ReloadTimesHeal(enum_ActionLevel level) => 5 - 1 * (int)level;
        public static float F_30005_ReloadHealAmount(enum_ActionLevel level) => -10f;
        public static float F_30006_ReloadTimesDamageMultiply(enum_ActionLevel level) => 2;
        public static float F_30006_ReloadDamageMultiply(enum_ActionLevel level) => 2 * (int)level;
        #endregion
    }

    public static class ActionHelper
    {
        public static void PlayerAcquireSimpleEquipmentItem(EntityPlayerBase player, int equipmentIndex,float damage,int buffIndex=-1)
        {
            player.OnAcquireEquipment<EquipmentBase>(equipmentIndex, () => { return DamageDeliverInfo.EquipmentInfo(player.I_EntityID,damage,buffIndex); });
        }
        public static void PlayerAcquireEntityEquipmentItem(EntityPlayerBase player, int equipmentIndex, float damage,int health,float fireRate)
        {
            player.OnAcquireEquipment<EquipmentEntitySpawner>(equipmentIndex, () => { return DamageDeliverInfo.EquipmentInfo(player.I_EntityID , damage,-1); }).SetOnSpawn((EntityBase entity) => {
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
        public static void PlayerReceiveHealing(EntityPlayerBase player,float healtAmount,enum_DamageType type= enum_DamageType.Common)
        {
            Debug.Log("Player Receive Healing Amount:" + healtAmount);
            if (healtAmount >= 0)
                Debug.LogError("Howd Fk Healing Above Zero?");
            player.m_HitCheck.TryHit(new DamageInfo(healtAmount,type, DamageDeliverInfo.Default(player.I_EntityID)));
        }
        public static void PlayerReceiveActionAmount(EntityPlayerBase player, float amount)
        {
            Debug.Log("Player Receive Aciton Amount:" + amount);
            player.m_PlayerInfo.AddActionAmount(amount);
        }
        public static void PlayerUpgradeAction(EntityPlayerBase player)
        {
            Debug.Log("Player Upgrade Current Random Action");
        }
    }
    #region ActionBase
    public class ActionBase : ExpireBase
    {
        public enum_ActionLevel m_Level { get; private set; } = enum_ActionLevel.Invalid;
        public virtual int I_ActionCost => -1;
        public virtual bool B_ActionAble => true;
        public virtual enum_ActionExpireType m_ExpireType => enum_ActionExpireType.Invalid;
        public virtual float GetValue1(EntityPlayerBase _actionEntity) => 0;
        public virtual float GetValue2(EntityPlayerBase _actionEntity) => 0;
        public virtual float GetValue3(EntityPlayerBase _actionEntity) => 0;
        public virtual float F_DamageAdditive(EntityPlayerBase _actionEntity) => 0;
        public virtual float F_RecoilMultiplyAdditive(EntityPlayerBase _actionEntity) => 0;
        public virtual float F_ProjectileSpeedMultiply => 0;
        public virtual bool B_ClipOverride => false;
        public virtual int I_ClipAdditive => 0;
        public virtual float F_ClipMultiply => 0;
        public ActionBase(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired, float _expireDuration = 0) : base(_expireDuration, _OnActionExpired)
        {
            m_Level = _level;
            if (m_ExpireType == enum_ActionExpireType.Invalid)
                Debug.LogError("Override Type Please!");
        }
        public virtual void OnActionUse(EntityPlayerBase _actionEntity) { }
        public virtual void OnAddActionElse(EntityPlayerBase _actionEntity, float actionAmount) { }
        public virtual void OnReceiveDamage(int applier, EntityPlayerBase receiver, float amount) { }
        public virtual void OnDealtDemage(int applier, EntityBase receiver, float amount) { }
        public virtual void OnReloadFinish(EntityPlayerBase _actionEntity) { }
        public virtual void OnAfterBattle() { }
        public virtual void OnAfterFire(int identity) { }
        public bool B_Upgradable => m_Level < enum_ActionLevel.L3;
        public void Upgrade()
        {
            if (m_Level < enum_ActionLevel.L3)
                m_Level++;
        }
    }
    public class ActionAfterUse:ActionBase
    {
        public override enum_ActionExpireType m_ExpireType => enum_ActionExpireType.AfterUse;
        public override void OnActionUse(EntityPlayerBase _actionEntity) => ForceExpire();
        public ActionAfterUse(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class ActionAfterDuration : ActionBase
    {
        public override enum_ActionExpireType m_ExpireType => enum_ActionExpireType.AfterDuration;
        public ActionAfterDuration(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, 0) { }
        public ActionAfterDuration(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired,float _duration) : base(_level, _OnActionExpired,_duration) { }
    }
    public class ActionAfterFire : ActionBase
    {
        public override enum_ActionExpireType m_ExpireType => enum_ActionExpireType.AfterFire;
        protected int m_fireIdentity = -1;
        public override void OnAfterFire(int _identity)
        {
            m_fireIdentity = _identity;
            ForceExpire();
        }
        public bool OnActionHitEntity(int damageIdentity, EntityPlayerBase _actionEntity, EntityBase _targetEntity)
        {
            if (m_fireIdentity != damageIdentity)
                return false;
            return OnActionHit(_actionEntity, _targetEntity);
        }

        protected virtual bool OnActionHit(EntityPlayerBase _playerBase, EntityBase _hitEntity)
        {
            Debug.Log("Override This Please");
            return false;
        }
        public ActionAfterFire(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class ActionAfterBattle : ActionBase
    {
        public override enum_ActionExpireType m_ExpireType => enum_ActionExpireType.AfterBattle;
        public override void OnAfterBattle() => ForceExpire();
        public ActionAfterBattle(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class ActionAfterBattle_ReloadTrigger : ActionAfterBattle
    {
        protected int m_reloadTimes;
        public override void OnActionUse(EntityPlayerBase _actionEntity)
        {
            base.OnActionUse(_actionEntity);
            m_reloadTimes = 0;
        }
        public override void OnReloadFinish(EntityPlayerBase _actionEntity)
        {
            base.OnReloadFinish(_actionEntity);
            m_reloadTimes++;
            if (m_reloadTimes % (int)GetValue1(_actionEntity) != 0)
                return;

            m_reloadTimes -= (int)GetValue1(_actionEntity);
            OnReloadTrigger(_actionEntity);
        }

        protected virtual void OnReloadTrigger(EntityPlayerBase _entity)
        {
            Debug.LogError("Override This Please");
        }
        public ActionAfterBattle_ReloadTrigger(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    #endregion

    #region NormalAction
    public class Action_10001_ArmorAdditive : ActionAfterUse
    {
        public override int I_ActionCost => ActionData.I_10001_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10001_ArmorAdditive(m_Level);
        public override void OnActionUse(EntityPlayerBase _actionEntity) {
            base.OnActionUse(_actionEntity);
            ActionHelper.PlayerReceiveHealing(_actionEntity, GetValue1(_actionEntity), enum_DamageType.ArmorOnly);
        }
        public Action_10001_ArmorAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10002_ArmorDamageAdditive : ActionAfterDuration
    {
        public override int m_Index => 10002;
        public override int I_ActionCost => ActionData.I_10002_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10002_ArmorDamageAdditive(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override float F_DamageAdditive(EntityPlayerBase _actionEntity) => GetValue1(_actionEntity);
        public Action_10002_ArmorDamageAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10002_Duration) { }
    }
    public class Action_10003_ArmorMultiplyAdditive : ActionAfterUse
    {
        public override int m_Index => 10003;
        public override int I_ActionCost => ActionData.I_10003_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10003_ArmorMultiplyAdditive(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override void OnActionUse(EntityPlayerBase _actionEntity) {
            base.OnActionUse(_actionEntity);
            ActionHelper.PlayerReceiveHealing(_actionEntity, GetValue1(_actionEntity), enum_DamageType.ArmorOnly);
        } 
        public Action_10003_ArmorMultiplyAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10004_ArmorActionReturn : ActionAfterUse
    {
        public override int m_Index => 10004;
        public override int I_ActionCost => ActionData.I_10004_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10004_ArmorActionAcquire(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override void OnActionUse(EntityPlayerBase _actionEntity)
        {
            base.OnActionUse(_actionEntity);
            ActionHelper.PlayerReceiveActionAmount(_actionEntity, GetValue1(_actionEntity));
        } 
        public Action_10004_ArmorActionReturn(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10005_ArmorDamageReduction : ActionAfterDuration
    {
        public override int m_Index => 10005;
        public override int m_EffectIndex => base.m_EffectIndex;
        public override int I_ActionCost => ActionData.I_10005_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10005_ArmorDamageReduction(m_Level);
        public override float m_DamageReduction => GetValue1(null);
        public Action_10005_ArmorDamageReduction(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10005_Duration) { }
    }

    public class Action_10006_FireRateAdditive: ActionAfterDuration
    {
        public override int m_Index => 10006;
        public override int I_ActionCost => ActionData.I_10006_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) =>ActionData.F_10006_FireRateAdditive(m_Level);
        public override float m_FireRateMultiply => GetValue1(null);
        public Action_10006_FireRateAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10006_Duration) { }
    }
    public class Action_10007_RecoilReduction : ActionAfterDuration
    {
        public override int m_Index => 10007;
        public override int I_ActionCost => ActionData.I_10007_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10007_RecoilMultiplyAdditive(m_Level);
        public override float F_RecoilMultiplyAdditive(EntityPlayerBase _actionEntity) => GetValue1(null);
        public Action_10007_RecoilReduction(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10007_Duration) { }
    }
    public class Action_10008_ClipMultiply : ActionAfterDuration
    {
        public override int m_Index => 10008;
        public override int I_ActionCost => ActionData.I_10008_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10008_ClipMultiply(m_Level);
        public override float F_ClipMultiply => GetValue1(null);
        public Action_10008_ClipMultiply(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10008_Duration) { }
    }
    public class Action_10009_ProjectileSpeedMultiply : ActionAfterDuration
    {
        public override int m_Index => 10009;
        public override int I_ActionCost => ActionData.I_10009_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10009_BulletSpeedAdditive(m_Level);
        public override float F_ProjectileSpeedMultiply => GetValue1(null);
        public Action_10009_ProjectileSpeedMultiply(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10009_Duration) { }
    }
    public class Action_10010_SingleDamageMultiply : ActionAfterFire
    {
        public override int m_Index => 10010;
        public override int I_ActionCost => ActionData.I_10010_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10010_SingleDamageMultiply(m_Level);
        public override float m_DamageMultiply => GetValue1(null);
        public Action_10010_SingleDamageMultiply(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10011_SingleDamageKillActionReturn : ActionAfterFire
    {
        public override int m_Index => 10011;
        public override int I_ActionCost => ActionData.I_10011_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10011_SingleDamageMultiply(m_Level);
        public override float GetValue2(EntityPlayerBase _actionEntity) => ActionData.F_10011_EntityKillActionReturn(m_Level);
        public override float m_DamageMultiply => GetValue1(null);
        protected override bool OnActionHit(EntityPlayerBase _actionEntity, EntityBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerReceiveActionAmount(_actionEntity,GetValue2(null));
            return true;
        }
        public Action_10011_SingleDamageKillActionReturn(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10012_SingleDamageKillHealing : ActionAfterFire
    {
        public override int m_Index => 10012;
        public override int I_ActionCost => ActionData.I_10012_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10012_SingleDamageMultiply(m_Level);
        public override float GetValue2(EntityPlayerBase _actionEntity) => ActionData.F_10012_EntityKillHealing(m_Level);
        public override float m_DamageMultiply => GetValue1(null);
        protected override bool OnActionHit(EntityPlayerBase _actionEntity, EntityBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerReceiveHealing(_actionEntity, GetValue2(null), enum_DamageType.HealthOnly);
            return true;
        }
        public Action_10012_SingleDamageKillHealing(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10013_SingleProjectileKillActionUpgrade : ActionAfterFire
    {
        public override int m_Index => 10013;
        public override int I_ActionCost => ActionData.I_10013_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10013_SingleDamageMultiply(m_Level);
        public override float m_DamageMultiply => GetValue1(null);
        protected override bool OnActionHit(EntityPlayerBase _actionEntity, EntityBase _targetEntity)
        {
            if (!_targetEntity.m_HealthManager.b_IsDead)
                return false;
            ActionHelper.PlayerUpgradeAction(_actionEntity);
            return true;
        }
        public Action_10013_SingleProjectileKillActionUpgrade(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }

    public class Action_10014_ReloadRateMultiply : ActionAfterDuration
    {
        public override enum_ActionExpireType m_ExpireType => enum_ActionExpireType.AfterDuration;
        public override int m_Index => 10014;
        public override int I_ActionCost => ActionData.I_10014_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10014_ReloadRateMultiplyAdditive(m_Level);
        public override float m_ReloadRateMultiply => GetValue1(null);
        public Action_10014_ReloadRateMultiply(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10014_Duration) { }
    }
    #endregion
    #region EquipmentItem
    public class Action_20001_Armor_Turret_Cannon : ActionAfterUse
    {
        public override int m_Index => 20001;
        public override int I_ActionCost => ActionData.I_20001_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_20001_ArmorTurretHealth(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override float GetValue2(EntityPlayerBase _actionEntity) => ActionData.F_20001_ArmorTurretDamage(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override void OnActionUse(EntityPlayerBase _entity) {
            base.OnActionUse(_entity);
            ActionHelper.PlayerAcquireEntityEquipmentItem(_entity, m_Index, GetValue2(_entity), (int)GetValue1(_entity), 1f);
        }
        public Action_20001_Armor_Turret_Cannon(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_20002_FireRate_FrozenGrenade : ActionAfterUse
    {
        public override int m_Index => 20002;
        public override int I_ActionCost => ActionData.I_20002_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_20002_DamageDealt(m_Level);
        public override float GetValue2(EntityPlayerBase _actionEntity) => ActionData.F_20002_BuffIndex(m_Level);
        public override void OnActionUse(EntityPlayerBase _entity){
            base.OnActionUse(_entity);
            ActionHelper.PlayerAcquireSimpleEquipmentItem(_entity, m_Index, GetValue1(_entity), (int)GetValue2(_entity));
        }
        public Action_20002_FireRate_FrozenGrenade(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_20003_Firerate_Turret_Minigun : ActionAfterUse
    {
        public override int m_Index => 20003;
        public override int I_ActionCost => ActionData.I_20003_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_20003_FireRate(m_Level, _actionEntity.m_WeaponCurrent.F_BaseFirerate);
        public override float GetValue2(EntityPlayerBase _actionEntity) => ActionData.F_20003_DamageDealt(m_Level, _actionEntity.m_WeaponCurrent.F_BaseDamage) ;
        public override float GetValue3(EntityPlayerBase _actionEntity) => ActionData.F_20003_Health(m_Level);
        public override void OnActionUse(EntityPlayerBase _entity){
            base.OnActionUse(_entity);
            ActionHelper.PlayerAcquireEntityEquipmentItem(_entity, m_Index, GetValue2(_entity), (int)GetValue3(_entity), GetValue1(_entity));
        }
        public Action_20003_Firerate_Turret_Minigun(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_20004_Damage_ExplosiveGrenade : ActionAfterUse
    {
        public override int m_Index => 20004;
        public override int I_ActionCost => ActionData.I_20004_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_20004_DamageDealt(m_Level, _actionEntity.m_WeaponCurrent.F_BaseDamage);
        public override void OnActionUse(EntityPlayerBase _entity) {
            base.OnActionUse(_entity);
            ActionHelper.PlayerAcquireSimpleEquipmentItem(_entity, m_Index, GetValue1(_entity));
        }
        public Action_20004_Damage_ExplosiveGrenade(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    #endregion
    #region LevelEquipment
    public class Action_30001_ArmorActionAdditive : ActionAfterBattle
    {
        public override int m_Index => 30001;
        public override int I_ActionCost => ActionData.I_30001_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => -ActionData.F_30001_ArmorActionAdditive(m_Level);
        public override void OnAddActionElse(EntityPlayerBase _actionEntity, float actionAmount) => ActionHelper.PlayerReceiveHealing(_actionEntity, GetValue1(_actionEntity), enum_DamageType.ArmorOnly);
        public Action_30001_ArmorActionAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_30002_ArmorDemageReturn : ActionAfterBattle
    {
        public override int m_Index => 30002;
        public override int I_ActionCost => ActionData.I_30002_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_30002_ArmorDamageReturn(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override void OnReceiveDamage(int applier, EntityPlayerBase receiver, float amount) => ActionHelper.PlayerDealtDamageToEntity(receiver,applier,GetValue1(receiver));
        public Action_30002_ArmorDemageReturn(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_30003_DamageAdditive : ActionAfterBattle
    {
        public override int m_Index => 30003;
        public override int I_ActionCost => ActionData.I_30003_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_30003_DamageAdditive(m_Level);
        public override float F_DamageAdditive(EntityPlayerBase _actionEntity) => GetValue1(null);
        public Action_30003_DamageAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_30004_ClipOverrideDamageAdditive : ActionAfterBattle
    {
        public override int m_Index => 30004;
        public override int I_ActionCost => ActionData.I_30004_Cost;
        public override bool B_ClipOverride => true;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_30004_DamageAdditive(m_Level);
        public override float F_DamageAdditive(EntityPlayerBase _actionEntity)=> GetValue1(null);
        public Action_30004_ClipOverrideDamageAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_30005_ReloadHeal : ActionAfterBattle_ReloadTrigger
    {
        public override int m_Index => 30005;
        public override int I_ActionCost => ActionData.I_30005_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_30005_ReloadTimesHeal(m_Level);
        public override float GetValue2(EntityPlayerBase _actionEntity) => ActionData.F_30005_ReloadHealAmount(m_Level);
        protected override void OnReloadTrigger(EntityPlayerBase _entity)=> ActionHelper.PlayerReceiveHealing(_entity,GetValue2(_entity), enum_DamageType.HealthOnly);
        public Action_30005_ReloadHeal(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_30006_ReloadDamageMultiply : ActionAfterBattle_ReloadTrigger
    {
        public override int m_Index => 30006;
        public override int I_ActionCost => ActionData.I_30006_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_30006_ReloadTimesDamageMultiply(m_Level);
        public override float m_DamageMultiply =>  m_TriggerOn? ActionData.F_30006_ReloadDamageMultiply(m_Level):0;
        bool m_TriggerOn = false;
        public override void OnActionUse(EntityPlayerBase _actionEntity)=> m_TriggerOn = false;

        public override void OnReloadFinish(EntityPlayerBase _actionEntity)
        {
            m_TriggerOn = false;
            base.OnReloadFinish(_actionEntity);
        }
        protected override void OnReloadTrigger(EntityPlayerBase _entity) => m_TriggerOn = true;
        public Action_30006_ReloadDamageMultiply(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    #endregion

}