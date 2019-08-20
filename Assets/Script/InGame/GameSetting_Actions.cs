using GameSetting;
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
        public static float F_10008_ClipAmountAdditive(enum_ActionLevel level) => .2f * (int)level;
        public static float F_10009_BulletSpeedAdditive(enum_ActionLevel level) => .3f * (int)level;

        public static float F_20001_ArmorTurretHealth(enum_ActionLevel level, float currentArmor) => currentArmor * (2 * (int)level);
        public static float F_20001_ArmorTurretDamage(enum_ActionLevel level, float currentArmor) => currentArmor * (1.5f * (int)level);
        public static float F_20002_DamageDealt(enum_ActionLevel level) => 50f;
        public static float F_20002_StunDuration(enum_ActionLevel level) => 2f * (int)level;

        public static float F_20004_DamageDealt(enum_ActionLevel level, float weaponDamage) => weaponDamage * 2 * (int)level;

        public static float F_30001_ArmorActionAdditive(enum_ActionLevel level) => 10 * (int)level;
        public static float F_30002_ArmorDamageReturn(enum_ActionLevel level, float currentArmor) => currentArmor * (2 * (int)level);
        public static float F_30003_DamageAdditive(enum_ActionLevel level) => 20f * (int)level;
        #endregion
    }


    #region NormalAction
    #region Armor
    public class Action_10001_ArmorAdditive : ActionBase
    {
        public override int m_Index => 10001;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int I_ActionCost => ActionData.I_10001_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10001_ArmorAdditive(m_Level);
        protected override void OnActionUse(EntityPlayerBase _actionEntity)
        {
            _actionEntity.m_HitCheck.TryHit(new DamageInfo(_actionEntity.I_EntityID, GetValue1(_actionEntity), enum_DamageType.ArmorOnly, DamageBuffInfo.Default()));
        }
        public Action_10001_ArmorAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10002_ArmorDamageAdditive : ActionBase
    {
        public override int m_Index => 10002;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int I_ActionCost => ActionData.I_10002_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10002_ArmorDamageAdditive(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override float F_DamageAdditive(EntityPlayerBase _actionEntity) => GetValue1(_actionEntity);
        public Action_10002_ArmorDamageAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10002_Duration) { }
    }
    public class Action_10003_ArmorMultiplyAdditive : ActionBase
    {
        public override int m_Index => 10003;
        public override int I_ActionCost => ActionData.I_10003_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10003_ArmorMultiplyAdditive(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        protected override void OnActionUse(EntityPlayerBase _actionEntity)
        {
            _actionEntity.m_HitCheck.TryHit(new DamageInfo(_actionEntity.I_EntityID, GetValue1(_actionEntity), enum_DamageType.ArmorOnly, DamageBuffInfo.Default()));
        }
        public Action_10003_ArmorMultiplyAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10004_ArmorActionReturn : ActionBase
    {
        public override int m_Index => 10004;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int I_ActionCost => ActionData.I_10004_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10004_ArmorActionAcquire(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        protected override void OnActionUse(EntityPlayerBase _actionEntity)
        {
            _actionEntity.m_PlayerInfo.AddActionAmount(GetValue1(_actionEntity));
        }
        public Action_10004_ArmorActionReturn(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_10005_ArmorDamageReduction : ActionBase
    {
        public override int m_Index => 10005;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override int m_EffectIndex => base.m_EffectIndex;
        public override int I_ActionCost => ActionData.I_10005_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_10005_ArmorDamageReduction(m_Level);
        public override float m_DamageReduction => GetValue1(null);
        public Action_10005_ArmorDamageReduction(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired, ActionData.F_10005_Duration) { }
    }
    #endregion
    #region FireRate
    #endregion
    #endregion
    #region EquipmentItem
    #region Armor
    public class Action_20001_ArmorTurret : ActionBase
    {
        public override int m_Index => 20001;
        public override int I_ActionCost => ActionData.I_20001_Cost;
        public override enum_ActionType m_Type => enum_ActionType.Equipment;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_20001_ArmorTurretHealth(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override float GetValue2(EntityPlayerBase _actionEntity) => ActionData.F_20001_ArmorTurretDamage(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        protected override void OnActionUse(EntityPlayerBase _entity)
        {
            float health = GetValue1(_entity);
            float damage = GetValue2(_entity);
            EquipmentEntitySpawner spawner = _entity.OnAcquireEquipment<EquipmentEntitySpawner>(m_Index, () => { return DamageBuffInfo.Create(0, damage); });
            spawner.SetOnSpawn((EntityBase entity) => {
                EntityAIBase target = entity as EntityAIBase;
                target.I_MaxHealth = (int)health;
                target.F_AttackDuration = new RangeFloat(1, 0);
                target.F_AttackTimes = new RangeInt(1, 0);
                target.F_AttackRate = 0f;
            });
        }
        public Action_20001_ArmorTurret(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_20004_ExplosiveGrenade : ActionBase
    {
        public override int m_Index => 20004;
        public override int I_ActionCost => ActionData.I_20004_Cost;
        public override enum_ActionType m_Type => enum_ActionType.Equipment;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_20004_DamageDealt(m_Level, _actionEntity.m_WeaponCurrent.m_ProjectileInfo.F_Damage);
        protected override void OnActionUse(EntityPlayerBase _entity)
        {
            _entity.OnAcquireEquipment<EquipmentBase>(m_Index, () => { return DamageBuffInfo.Create(0, GetValue1(_entity)); });
        }
        public Action_20004_ExplosiveGrenade(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    #endregion
    #endregion
    #region LevelEquipment
    #region Armor
    public class Action_30001_ArmorActionAdditive : ActionBase
    {
        public override int m_Index => 30001;
        public override int I_ActionCost => ActionData.I_30001_Cost;
        public override enum_ActionType m_Type => enum_ActionType.LevelEquipment;
        public override float GetValue1(EntityPlayerBase _actionEntity) => -ActionData.F_30001_ArmorActionAdditive(m_Level);
        public override void OnAddActionElse(EntityPlayerBase _actionEntity, float actionAmount)
        {
            _actionEntity.m_HitCheck.TryHit(new DamageInfo(_actionEntity.I_EntityID, GetValue1(_actionEntity), enum_DamageType.ArmorOnly, DamageBuffInfo.Default()));
        }
        public Action_30001_ArmorActionAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_30002_ArmorDemageReturn : ActionBase
    {
        public override int m_Index => 30002;
        public override int I_ActionCost => ActionData.I_30002_Cost;
        public override enum_ActionType m_Type => enum_ActionType.LevelEquipment;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_30002_ArmorDamageReturn(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override void OnReceiveDamage(int applier, EntityPlayerBase receiver, float amount)
        {
            GameManager.Instance.GetEntity(applier).m_HitCheck.TryHit(new DamageInfo(receiver.I_EntityID, GetValue1(receiver), enum_DamageType.Common, DamageBuffInfo.Default()));
        }

        public Action_30002_ArmorDemageReturn(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    #endregion
    #endregion

}