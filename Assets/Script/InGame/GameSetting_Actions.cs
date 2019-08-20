using GameSetting;
using UnityEngine;
using System;

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
        
        public const int I_20001_Cost = 2;
        public const int I_20002_Cost = 1;
        public const int I_20003_Cost = 2;
        public const int I_20004_Cost = 1;

        public const int I_30001_Cost = 1;
        public const int I_30002_Cost = 2;
        public const int I_30003_Cost = 2;

        #endregion

        #region Duration
        public const float F_10002_Duration = 10f;
        public const float F_10005_Duration = 30f;
        public const float F_10006_Duration = 20f;
        public const float F_10007_Duration = 20f;
        public const float F_10008_Duration = 30f;
        public const float F_10009_Duration = 30f;
        #endregion

        #region Expression
        public static float F_10001_ArmorAdditive(enum_ActionLevel level) => -30f * (int)level;
        public static float F_10002_ArmorDamageAdditive(enum_ActionLevel level, float currentArmor) => currentArmor * 1f;
        public static float F_10003_ArmorMultiplyAdditive(enum_ActionLevel level, float currentArmor) => -currentArmor * .3f * (int)level;
        public static float F_10004_ArmorActionAcquire(enum_ActionLevel level, float currentArmor) => currentArmor / (20f + 10f * (int)level);
        public static float F_10005_ArmorDamageReduction(enum_ActionLevel level) => .2f * (int)level;
        public static float F_10006_FireRateAdditive(enum_ActionLevel level) => .3f * (int)level;
        public static float F_10007_RecoilReduction(enum_ActionLevel level) => .3f * (int)level;
        public static float F_10008_ClipMultiply(enum_ActionLevel level) => .2f * (int)level;
        public static float F_10009_BulletSpeedAdditive(enum_ActionLevel level) => .3f * (int)level;

        public static float F_20001_ArmorTurretHealth(enum_ActionLevel level, float currentArmor) => currentArmor * (2 * (int)level);
        public static float F_20001_ArmorTurretDamage(enum_ActionLevel level, float currentArmor) => currentArmor * (1.5f * (int)level);
        public static float F_20002_DamageDealt(enum_ActionLevel level) => 50f;
        public static float F_20002_StunDuration(enum_ActionLevel level) => 2f * (int)level;
        public static float F_20003_FireRate(enum_ActionLevel level, float weaponFirerate) => .7f * weaponFirerate * (int)level;
        public static float F_20003_DamageDealt(enum_ActionLevel level, float weaponDamage) => .7f * weaponDamage * (int)level;
        public static float F_20004_DamageDealt(enum_ActionLevel level, float weaponDamage) => weaponDamage * 2 * (int)level;

        public static float F_30001_ArmorActionAdditive(enum_ActionLevel level) => 10 * (int)level;
        public static float F_30002_ArmorDamageReturn(enum_ActionLevel level, float currentArmor) => currentArmor * (2 * (int)level);
        public static float F_30003_DamageAdditive(enum_ActionLevel level) => 20f * (int)level;
        #endregion
    }

    public static class ActionHelper
    {
        public static void PlayerAcquireSimpleEquipmentItem(EntityPlayerBase player, int equipmentIndex,float damage)
        {
            player.OnAcquireEquipment<EquipmentBase>(equipmentIndex, () => { return DamageBuffInfo.Create(0,damage); });
        }
        public static void PlayerAcquireEntityEquipmentItem(EntityPlayerBase player, int equipmentIndex, float damage,int health,float fireRate)
        {
            player.OnAcquireEquipment<EquipmentEntitySpawner>(equipmentIndex, () => { return DamageBuffInfo.Create(0, damage); }).SetOnSpawn((EntityBase entity) => {
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
            GameManager.Instance.GetEntity(targetID).m_HitCheck.TryHit(new DamageInfo(player.I_EntityID,damageAmount, damageType, DamageBuffInfo.Default()));
        }
        public static void PlayerReceiveHealing(EntityPlayerBase player,float healtAmount,enum_DamageType type= enum_DamageType.Common)
        {
            if (healtAmount >= 0)
                Debug.LogError("Howd Fk Healing Above Zero?");
            player.m_HitCheck.TryHit(new DamageInfo(player.I_EntityID,healtAmount,type, DamageBuffInfo.Default()));
        }
        public static void PlayerReceiveActionAmount(EntityPlayerBase player, float amount)
        {
            player.m_PlayerInfo.AddActionAmount(amount);
        }
    }
    #region NormalAction
    public class Action_10001_ArmorAdditive : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_Index => 10001;
        public override int I_ActionCost => ActionData.I_10001_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10001_ArmorAdditive(m_Level);
        protected override void OnActionUse(EntityPlayerBase _actionEntity) => ActionHelper.PlayerReceiveHealing(_actionEntity, GetValue1(_actionEntity), enum_DamageType.ArmorOnly);
        public Action_10001_ArmorAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10002_ArmorDamageAdditive : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_Index => 10002;
        public override int I_ActionCost => ActionData.I_10002_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10002_ArmorDamageAdditive(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override float F_DamageAdditive(EntityPlayerBase _actionEntity) => GetValue1(_actionEntity);
        public Action_10002_ArmorDamageAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10002_Duration) { }
    }
    public class Action_10003_ArmorMultiplyAdditive : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_Index => 10003;
        public override int I_ActionCost => ActionData.I_10003_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10003_ArmorMultiplyAdditive(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        protected override void OnActionUse(EntityPlayerBase _actionEntity) => ActionHelper.PlayerReceiveHealing(_actionEntity,GetValue1(_actionEntity), enum_DamageType.ArmorOnly);
        public Action_10003_ArmorMultiplyAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10004_ArmorActionReturn : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_Index => 10004;
        public override int I_ActionCost => ActionData.I_10004_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10004_ArmorActionAcquire(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        protected override void OnActionUse(EntityPlayerBase _actionEntity) => ActionHelper.PlayerReceiveActionAmount(_actionEntity,GetValue1(_actionEntity));
        public Action_10004_ArmorActionReturn(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10005_ArmorDamageReduction : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_Index => 10005;
        public override int m_EffectIndex => base.m_EffectIndex;
        public override int I_ActionCost => ActionData.I_10005_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10005_ArmorDamageReduction(m_Level);
        public override float m_DamageReduction => GetValue1(null);
        public Action_10005_ArmorDamageReduction(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10005_Duration) { }
    }

    public class Action_10006_FireRateAdditive:ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_Index => 10006;
        public override int I_ActionCost => ActionData.I_10006_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) =>ActionData.F_10006_FireRateAdditive(m_Level);
        public override float m_FireRateMultiply => GetValue1(null);
        public Action_10006_FireRateAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10006_Duration) { }
    }
    public class Action_10007_RecoilReduction : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_Index => 10007;
        public override int I_ActionCost => ActionData.I_10007_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10007_RecoilReduction(m_Level);
        public override float F_RecoilReduction(EntityPlayerBase _actionEntity) => GetValue1(null);
        public Action_10007_RecoilReduction(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10007_Duration) { }
    }
    public class Action_10008_ClipMultiply : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_Index => 10008;
        public override int I_ActionCost => ActionData.I_10008_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10008_ClipMultiply(m_Level);
        public override float F_ClipMultiply => GetValue1(null);
        public Action_10008_ClipMultiply(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10008_Duration) { }
    }
    #endregion
    #region EquipmentItem
    public class Action_20001_ArmorTurret : ActionBase
    {
        public override int m_Index => 20001;
        public override int I_ActionCost => ActionData.I_20001_Cost;
        public override enum_ActionType m_Type => enum_ActionType.Equipment;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_20001_ArmorTurretHealth(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override float GetValue2(EntityPlayerBase _actionEntity) => ActionData.F_20001_ArmorTurretDamage(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        protected override void OnActionUse(EntityPlayerBase _entity)=> ActionHelper.PlayerAcquireEntityEquipmentItem(_entity,m_Index,GetValue2(_entity),(int)GetValue1(_entity),1f);
        public Action_20001_ArmorTurret(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_20004_ExplosiveGrenade : ActionBase
    {
        public override int m_Index => 20004;
        public override int I_ActionCost => ActionData.I_20004_Cost;
        public override enum_ActionType m_Type => enum_ActionType.Equipment;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_20004_DamageDealt(m_Level, _actionEntity.m_WeaponCurrent.m_ProjectileInfo.F_Damage);
        protected override void OnActionUse(EntityPlayerBase _entity)=> ActionHelper.PlayerAcquireSimpleEquipmentItem(_entity,m_Index, GetValue1(_entity));
        public Action_20004_ExplosiveGrenade(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    #endregion
    #region LevelEquipment
    public class Action_30001_ArmorActionAdditive : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.LevelEquipment;
        public override int m_Index => 30001;
        public override int I_ActionCost => ActionData.I_30001_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => -ActionData.F_30001_ArmorActionAdditive(m_Level);
        public override void OnAddActionElse(EntityPlayerBase _actionEntity, float actionAmount) => ActionHelper.PlayerReceiveHealing(_actionEntity, GetValue1(_actionEntity), enum_DamageType.ArmorOnly);
        public Action_30001_ArmorActionAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_30002_ArmorDemageReturn : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.LevelEquipment;
        public override int m_Index => 30002;
        public override int I_ActionCost => ActionData.I_30002_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_30002_ArmorDamageReturn(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override void OnReceiveDamage(int applier, EntityPlayerBase receiver, float amount) => ActionHelper.PlayerDealtDamageToEntity(receiver,applier,GetValue1(receiver));
        public Action_30002_ArmorDemageReturn(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_30003_DamageAdditive : ActionBase
    {
        public override enum_ActionType m_Type => enum_ActionType.LevelEquipment;
        public override int m_Index => 30003;
        public override int I_ActionCost => ActionData.I_30003_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_30003_DamageAdditive(m_Level);
        public override float F_DamageAdditive(EntityPlayerBase _actionEntity) => GetValue1(null);
        public Action_30003_DamageAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    #endregion

}