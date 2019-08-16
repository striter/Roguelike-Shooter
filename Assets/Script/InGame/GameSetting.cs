﻿using UnityEngine;
using TExcel;
using System.Collections.Generic;
using System;
using TTiles;
using TPhysics;
using System.Linq;
#pragma warning disable 0649
namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
        public const int I_TotalStageCount = 3;
        public const float F_EntityDeadFadeTime = 2f;
        public const float F_MaxActionAmount = 3f;

        public const int I_ProjectileMaxDistance = 100;
        public const int I_ProjectileBlinkWhenTimeLeftLessThan = 3;
        public const float F_AimAssistDistance = 100f;
        public const short I_BoltLastTimeAfterHit = 5;
        public const float F_LaserRayStartPause = .5f;      //Laser Start Pause
        public const float F_ParticlesMaxStopTime = 4f;

        public const int I_BurstFirePelletsOnceTrigger = 3;       //Times While Burst Fire
        public const int I_ProjectileSpreadAtDistance = 100;       //Meter,  Bullet Spread In A Circle At End Of This Distance

        public const float F_LevelTileSize = 2f;        //Cube Size For Level Tiles
        
        public const float F_PlayerCameraSmoothParam = 1f;     //Camera Smooth Param For Player .2 is suggested
        public const float F_PlayerFallSpeed = 9.8f;       //Player Fall Speed(Not Acceleration)

        public const float F_EnermyAICheckTime = .3f;       //AI Check Offset Time, 0.3 is suggested;
        public const int I_EnermyCountWaveFinish = 0;       //When Total Enermy Count Reaches This Amount,Wave Finish
        public const int I_EnermySpawnDelay = 2;        //Enermy Spawn Delay Time 

    }

    public static class GameExpression
    {
        public static Vector3 V3_TileAxisOffset(TileAxis axis) => new Vector3(axis.X * GameConst.F_LevelTileSize, 0, axis.Y * GameConst.F_LevelTileSize);
        public static float F_BigmapYaw(Vector3 direction) => TCommon.GetAngle(direction, Vector3.forward, Vector3.up);         //Used For Bigmap Direction
        public static bool B_ShowHitMark(enum_HitCheck check) => check!= enum_HitCheck.Invalid;
        public static float F_SphereCastDamageReduction(float weaponDamage, float distance, float radius) => weaponDamage * (1 - (distance / radius));       //Rocket Blast Damage
        public static Vector3 V3_RangeSpreadDirection(Vector3 aimDirection, float spread, Vector3 up, Vector3 right) => (aimDirection * GameConst.I_ProjectileSpreadAtDistance + up * UnityEngine.Random.Range(-spread, spread) + right * UnityEngine.Random.Range(-spread, spread)).normalized;

        public static int GetEnermyWeaponIndex(int enermyIndex, int weaponIndex = 0, int subWeaponIndex = 0) => enermyIndex * 100 + weaponIndex * 10 + subWeaponIndex;
        public static int GetEnermyWeaponSubIndex(int weaponIndex) => weaponIndex + 1;

        public static float F_ActionAmountReceive(float damageApply) => damageApply * .1f;
        
    }
    public static class ActionData
    {
        #region Armor
        public const int I_Armor1_Cost = 2;
        public const int I_Armor2_Cost = 2;
        public const int I_Armor3_Cost = 3;
        public const int I_Armor4_Cost = 0;
        public const int I_Armor5_Cost = 3;

        public const float F_Armor2_Duration = 10f;
        public const float F_Armor5_Duration = 30f;

        public static float F_Armor1_Additive(enum_ActionLevel level) => -30f * (int)level;
        public static float F_Armor2_DamageAdditive(enum_ActionLevel level, float currentArmor) => currentArmor * .1f;
        public static float F_Armor3_MultiplyAdditive(enum_ActionLevel level, float currentArmor) => -currentArmor * (int)level * .3f;
        public static float F_Armor4_ActionReturn(enum_ActionLevel level, float currentArmor) => currentArmor / (20f + (int)level * 10f);
        public static float F_Armor5_DamageReduction(enum_ActionLevel level) => 20f * (int)level;
        #endregion
    }

    public static class UIConst
    {
        public const int I_SporeManagerUnitTime = 60;//Seconds
        public const int I_SporeManagerContainersMaxAmount = 9;  //Max Amount Of SporeManager Container
        public const int I_SporeManagerContainerStartFreeSlot = 3;    //Free Slot For New Player
        public const int I_SporeManagerContainerTickTime = 60;       //Seconds
        public const int I_SporeManagerContainerStartRandomEnd = 30;      // Start From 0 
        public const int I_SporeManagerAutoSave = 5;      //Per Seconds Auto Save Case Game Crush
        public const int I_SporeManagerHybridMaxLevel = 40;      //Spore Level Equals Won't Hybrid
        
        public const float F_IAmmoLineLength = 200;
    }

    public static class UIExpression
    {
        public static float F_SporeManagerProfitPerMinute(int level) => 10 * Mathf.Pow(1.3f, level - 1);     //Coins Profit Per Unit Time
        public static float F_SporeManagerCoinsMaxAmount(int level) => 100 * Mathf.Pow(10 * Mathf.Pow(1.3f, level - 1), 1.03f) + 400 * Mathf.Pow(1.4f, level - 1);
        //Coins Max Amount Each Level
        public static float F_SporeManagerChestCoinRequirement(int maxLevel) => 200 * Mathf.Pow(1.4f, maxLevel - 1);       //Coin Requirement Per Chest
        public static float F_SporeManagerChestBlueRequirement(int maxLevel) => 100 * Mathf.Pow(1.05f, maxLevel - 1);       //Blue Requirement Per Chest

        public static Color BigmapTileColor(enum_TileLocking levelLocking, enum_TileType levelType)
        {
            Color color;
            switch (levelType)
            {
                default: color = TCommon.ColorAlpha(Color.blue, .5f); break;
                case enum_TileType.Battle: color = TCommon.ColorAlpha(Color.red, .5f); break;
                case enum_TileType.Reward: color = TCommon.ColorAlpha(Color.green, .5f); break;
                case enum_TileType.Start: color = TCommon.ColorAlpha(Color.blue, .5f); break;
                case enum_TileType.End: color = TCommon.ColorAlpha(Color.black, .5f); break;
            }
            switch (levelLocking)
            {
                case enum_TileLocking.Unlockable: color = TCommon.ColorAlpha(color, .2f); break;
            }
            return color;
        }
    }

    public static class Enum_Relative
    {
        public static enum_LevelGenerateType ToPrefabType(this enum_TileType type)
        {
            switch (type)
            {
                default: Debug.LogError("Please Edit This Please:" + type.ToString()); return enum_LevelGenerateType.Invalid;
                case enum_TileType.Battle:
                case enum_TileType.End:
                    return enum_LevelGenerateType.Big;
                case enum_TileType.Reward:
                case enum_TileType.Start:
                    return enum_LevelGenerateType.Small;
            }
        }
        public static int ToLayer(this enum_HitCheck layerType)
        {
            switch (layerType)
            {
                default: Debug.LogError("Null Layer Can Be Transferd From:" + layerType.ToString()); return 0;
                case enum_HitCheck.Entity: return GameLayer.I_Entity;
                case enum_HitCheck.Static: return GameLayer.I_Static;
                case enum_HitCheck.Dynamic: return GameLayer.I_Dynamic;
            }
        }
    }
    #endregion

    #region For Developers Use

    #region BroadCastEnum
    enum enum_BC_GameStatusChanged
    {
        Invalid = -1,
        OnEntitySpawn,
        OnEntityDamage,
        OnEntityDead,
        OnEntityRecycle,

        PlayerInfoChanged,
        LevelStatusChange,

        OnStageStart,       //Total Stage Start
        OnStageFinish,
        OnLevelStart,       //Change Between Each Level
        OnLevelFinish,
        OnBattleStart,      //Battle Against Entity
        OnBattleFinish,
        OnWaveStart,     //Battle Wave
        OnWaveFinish,
    }
    #endregion

    #region GameEnum
    public enum enum_EntityFlag { Invalid=-1, Player=1,Enermy=2,}

    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, }

    public enum enum_BattleDifficulty { Invalid = -1,Peaceful=0, Eazy = 1, Normal = 2, Hard = 3,Final=4, }

    public enum enum_TileLocking { Invalid = -1, Locked = 0, Unlockable = 1, Unlocked = 2, }

    public enum enum_Style { Invalid = -1, Forest = 1, Desert = 2, Iceland = 3, Horde = 4, Undead = 5, }

    public enum enum_TileType { Invalid = -1, Start = 0, Battle = 1, End = 2, Reward = 3, }

    public enum enum_LevelItemType { Invalid = -1, LargeMore, LargeLess, MediumMore, MediumLess, SmallMore, SmallLess, ManmadeMore, ManmadeLess, NoCollisionMore, NoCollisionLess,BorderLinear,BorderOblique,Portal, }

    public enum enum_LevelTileType { Invaid = -1, Empty, Main,Border, Item, Portal, }

    public enum enum_LevelTileOccupy { Invalid = -1, Inner, Outer, Border, }

    public enum enum_LevelGenerateType { Invalid = -1, Big = 1, Small = 2 }

    public enum enum_EntityType { Invalid = -1,Hidden=0, Fighter = 1, Shooter_Rookie = 2,Shooter_Veteran=3, AOECaster = 4, Elite = 5 }

    public enum enum_Interaction { Invalid = -1, }      //To Be Continued

    public enum enum_TriggerType { Invalid = -1, Single = 1, Auto = 2, Burst = 3, Pull = 4, Store = 5, }

    public enum enum_ProjectileFireType { Invalid=-1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, CastSelfDetonate=4,}

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, ForwardTrapezium = 4, }

    public enum enum_HealthMessageType { Invalid = -1, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Common = 1, ArmorOnly = 2,HealthOnly = 3, }

    public enum enum_ExpireRefreshType { Invalid = -1, AddUp = 1, Refresh = 2 }

    public enum enum_ActionLevel { Invalid=-1, L1=1,L2=2,L3=3, }

    public enum enum_ActionType { Invalid = -1, Action = 1, Item = 2, Equipment = 3, }

    public enum enum_PlayerWeapon
    {
        Invalid = -1,
        //Laser
        LaserPistol = 1001,
        Railgun = 1002,
        //Snipe Rifle
        M82A1 = 2001,
        Kar98 = 2002,
        //Submachine Gun
        UZI = 3001,
        UMP45 = 3002,
        //Assult Rifle
        SCAR = 4001,
        M16A4 = 4002,
        AKM = 4003,
        //Pistol
        P92 = 5001,
        DE = 5002,
        //Shotgun
        XM1014 = 6001,
        S686 = 6002,
        //Heavy Weapon
        Crossbow = 7001,
        RocketLauncher = 7002,
    }

    public enum enum_PlayerAnim
    {
        Invalid=-1,
        Rifle_1001=1001,
        Pistol_L_1002=1002,
        Crossbow_1003=1003,
        Heavy_1004=1004,
    }

    public enum enum_EnermyAnim
    {
        Invalid = -1,
        Axe_Dual_Pound_10 = 10,
        Spear_R_Stick_20 = 20,
        Sword_R_Swipe_30 = 30,
        Sword_R_Slash_31 = 31,
        Dagger_Dual_Twice_40=40,
        Staff_L_Cast_110 = 110,
        Staff_Dual_Cast_111 = 111,
        Staff_R_Cast_Loop_112 = 112,
        Staff_R_Cast_113=113,
        Bow_Shoot_130 = 130,
        CrossBow_Shoot_131=131,
        Bow_UpShoot_133=133,
        Rifle_HipShot_140 = 140,
        Rifle_AimShot_141=141,
        Throwable_Hips_150 = 150,
        Throwable_R_ThrowHip_151=151,
        Throwable_R_ThrowBack_152=152,
        Heavy_HipShot_161=161,
        Heavy_Mortal_162=162,
        Heavy_Shield_Spear_163=163,
        Heavy_Remote_164=164,
    }
    #endregion

    #region GameLayer
    public static class GameLayer
    {
        public static readonly int I_Static = LayerMask.NameToLayer("static");
        public static readonly int I_Entity = LayerMask.NameToLayer("entity");
        public static readonly int I_Dynamic = LayerMask.NameToLayer("dynamic");
        public static readonly int I_DynamicDetect = LayerMask.NameToLayer("dynamicDetect");
        public static readonly int I_MovementDetect = LayerMask.NameToLayer("movementDetect");
        public static class Mask
        {
            public static readonly int I_All = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity | 1 << GameLayer.I_Dynamic;
            public static readonly int I_StaticEntity = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity;
            public static readonly int I_EntityOnly = (1 << I_Entity);
            public static readonly int I_Static = (1 << GameLayer.I_Static);
        }
    }
    #endregion

    #region GameSave
    public class CPlayerSave : ISave        //Save Outta Game
    {
        public float f_blue;
        public CPlayerSave()
        {
            f_blue = 100;
        }
    }
    #endregion

    #region GameClass/Structs
    #region GameBase
    public class HealthBase
    {
        public float m_CurrentHealth { get; private set; }
        public float m_MaxHealth { get; private set; }
        
        public bool b_IsDead => m_CurrentHealth <= 0;
        protected void DamageHealth(float health)
        {
            m_CurrentHealth -= health;
            if (m_CurrentHealth < 0)
                m_CurrentHealth = 0;
            else if (m_CurrentHealth > m_MaxHealth)
                m_CurrentHealth = m_MaxHealth;
        }

        protected Action<enum_HealthMessageType> OnHealthChanged;
        protected Action OnDead;
        public HealthBase(float _maxHealth, Action<enum_HealthMessageType> _OnHealthChanged, Action _OnDead)
        {
            m_MaxHealth = _maxHealth;
            OnHealthChanged = _OnHealthChanged;
            OnDead = _OnDead;
        }
        public virtual void OnActivate()
        {
            m_CurrentHealth = m_MaxHealth;
        }
        public virtual bool OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1)
        {
            if (b_IsDead)
                return false;

            m_CurrentHealth -= damageInfo.m_AmountApply * damageReduction;
            OnHealthChanged?.Invoke( enum_HealthMessageType.DamageHealth);

            if (b_IsDead)
                OnDead();

            return true;
        }
    }

    public class EntityHealth:HealthBase
    {
        public float m_CurrentArmor { get; private set; }
        public float m_DefaultArmor { get; private set; }
        
        public float F_TotalHealth => m_MaxHealth + m_DefaultArmor;
        public float F_HealthScale =>Mathf.Clamp01( m_CurrentHealth/m_MaxHealth);
        public float F_ArmorScale => Mathf.Clamp01(m_CurrentArmor/ m_DefaultArmor);
        public float F_EHPScale => Mathf.Clamp01((m_CurrentArmor+m_CurrentHealth)/(m_DefaultArmor+m_MaxHealth));
        protected EntityBase m_Entity;
        protected void DamageArmor(float amount)
        {
            m_CurrentArmor -= amount;
            if (m_CurrentArmor < 0)
                m_CurrentArmor = 0;
        }

        public EntityHealth(EntityBase entity, Action<enum_HealthMessageType> _OnHealthChanged, Action _OnDead) :base(entity.I_MaxHealth,_OnHealthChanged,_OnDead)
        {
            m_Entity = entity;
            m_DefaultArmor = entity.I_DefaultArmor;
            m_CurrentArmor = m_DefaultArmor;
        }
        public override void OnActivate()
        {
            base.OnActivate();
        }
        public override bool OnReceiveDamage(DamageInfo damageInfo,float damageMultiply=1)
        {
            if (b_IsDead)
                return false;

            if (damageInfo.m_AmountApply > 0)    //Damage
            {
                float damageReceive = damageInfo.m_AmountApply*damageMultiply;

                switch (damageInfo.m_damageType)
                {
                    case enum_DamageType.ArmorOnly:
                        {
                            if (F_ArmorScale == 0)
                                return false;
                            DamageArmor(damageReceive);
                            OnHealthChanged(enum_HealthMessageType.DamageArmor);
                        }
                        break;
                    case enum_DamageType.HealthOnly:
                        {
                            DamageHealth(damageReceive);
                            OnHealthChanged(enum_HealthMessageType.DamageHealth);
                        }
                        break;
                    case enum_DamageType.Common:
                        {
                            float healthDamage = damageReceive-m_CurrentArmor ;
                            DamageArmor(damageReceive);
                            if (healthDamage > 0)
                                DamageHealth(healthDamage);
                            OnHealthChanged(healthDamage >= 0 ? enum_HealthMessageType.DamageHealth : enum_HealthMessageType.DamageArmor);
                        }
                        break;
                }

                TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnEntityDamage, damageInfo.I_SourceID, m_Entity, damageReceive);
                if (b_IsDead)
                    OnDead();
            }
            else if (damageInfo.m_AmountApply < 0)    //Healing
            {
                float amountHeal = damageInfo.m_AmountApply;
                switch (damageInfo.m_damageType)
                {
                    case enum_DamageType.ArmorOnly:
                        {
                            DamageArmor(amountHeal);
                            OnHealthChanged(enum_HealthMessageType.ReceiveArmor);
                        }
                        break;
                    case enum_DamageType.HealthOnly:
                        {
                            if (F_HealthScale==1)
                                return false;
                            DamageHealth(amountHeal);
                            OnHealthChanged( enum_HealthMessageType.ReceiveHealth);
                        }
                        break;
                    case enum_DamageType.Common:
                        {
                            float armorReceive = amountHeal  - m_CurrentHealth+m_MaxHealth;
                            DamageHealth(amountHeal);
                            if (armorReceive>0)
                            {
                                OnHealthChanged(enum_HealthMessageType.ReceiveHealth);
                                return true;
                            }
                            
                            DamageArmor(armorReceive);
                            OnHealthChanged(enum_HealthMessageType.ReceiveArmor);
                        }
                        break;
                }
            }
            
            return true;
        }
    }

    public class DamageInfo
    {
        public int I_SourceID { get; private set; }
        public float m_AmountApply => m_baseDamage  *(1+  m_BuffApply.F_DamageMultiply)+ m_BuffApply.F_DamageAdditive;
        private float m_baseDamage;
        public DamageBuffInfo m_BuffApply { get; private set; }
        public enum_DamageType m_damageType { get; private set; }
        public DamageInfo(int sourceID,float damage,enum_DamageType type,DamageBuffInfo buffInfo)
        {
            I_SourceID = sourceID;
            m_baseDamage = damage;
            m_BuffApply = buffInfo;
            m_damageType = type;
        }
        public DamageInfo(int sourceID, int buffApply)
        {
            I_SourceID = sourceID;
            m_baseDamage = 0;
            m_BuffApply =  DamageBuffInfo.Create(new List<int>() { buffApply });
            m_damageType =  enum_DamageType.Common;
        }

        public void ResetDamage(float damage)
        {
            m_baseDamage = damage;
        }
    }
    #endregion

    #region BuffClass
    public class EntityInfoManager
    {
        public List<ExpireBase> m_Buffs { get; private set; } = new List<ExpireBase>();
        protected EntityBase m_Entity { get; private set; }
        List< SFXBuffEffect> m_BuffEffects = new List<SFXBuffEffect>();
        public float F_MaxHealth => m_Entity.I_MaxHealth;

        public float F_DamageReceiveMultiply { get; private set; } = 1f;
        public float F_MovementSpeedMultiply { get; private set; } = 1f;
        protected float F_FireRateMultiply { get; private set; } = 1f;
        protected float F_ReloadRateMultiply { get; private set; } = 1f;
        protected float F_DamageMultiply { get; private set; } = 0f;

        public float F_FireRateTick(float deltaTime) => deltaTime * F_FireRateMultiply;
        public float F_ReloadRateTick(float deltaTime) => deltaTime * F_ReloadRateMultiply;
        public float F_MovementSpeed => m_Entity.F_MovementSpeed * F_MovementSpeedMultiply;
        public virtual DamageBuffInfo GetDamageInfo() => DamageBuffInfo.Create(F_DamageMultiply, 0f);
        Func<DamageInfo, bool> OnReceiveDamage;
        Action OnInfoChange;
        public EntityInfoManager(EntityBase _attacher,Func<DamageInfo, bool> _OnReceiveDamage,Action _OnInfoChange)
        {
            m_Entity = _attacher;
            OnReceiveDamage = _OnReceiveDamage;
            OnInfoChange = _OnInfoChange;
        }
        public virtual void OnDeactivate()
        {
            m_Buffs.Clear();
            OnInfoChanged();
        }
        public virtual void Tick(float deltaTime) => m_Buffs.Traversal((ExpireBase buff) => { buff.OnTick(deltaTime); });
        public void AddBuff(int sourceID,int buffIndex)
        {
            ExpireBuff buff = GetBuff(sourceID,buffIndex);
            switch (buff.m_RefreshType)
            {
                case enum_ExpireRefreshType.AddUp:
                    {
                        m_Buffs.Add(buff);
                    }
                    break;
                case enum_ExpireRefreshType.Refresh:
                    {
                        ExpireBase buffRefresh = m_Buffs.Find(p =>p.m_Index == buffIndex);
                        if (buffRefresh != null)
                            buffRefresh.ExpireRefresh();
                        else
                            m_Buffs.Add(buff);
                    }
                    break;
            }
            OnInfoChanged();
        }

        protected void OnExpired(ExpireBase buff)
        {
            m_Buffs.Remove(buff);
            OnInfoChanged();
        }
        public ExpireBuff GetBuff(int sourceID,int buffIndex) => new ExpireBuff(sourceID, DataManager.GetEntityBuffProperties(buffIndex), OnReceiveDamage, OnExpired);
        public virtual void OnInfoChanged()
        {
            //Calculate All Buff Infos
            F_DamageMultiply = 0f;
            F_DamageReceiveMultiply = 1f;
            F_MovementSpeedMultiply = 1f;
            F_FireRateMultiply = 1f;
            F_ReloadRateMultiply = 1f;

            m_Buffs.Traversal((ExpireBase expire) => {
                F_DamageMultiply += expire.m_DamageMultiply;
                F_DamageReceiveMultiply -= expire.m_DamageReduction;
                F_MovementSpeedMultiply += expire.m_MovementSpeedMultiply;
                F_FireRateMultiply += expire.m_FireRateMultiply;
                F_ReloadRateMultiply += expire.m_ReloadRateMultiply;
            });

            F_DamageReceiveMultiply = F_DamageReceiveMultiply < 0 ? 0 : F_DamageReceiveMultiply;

            //Do Effect Removal Check
            for (int i = 0; i < m_BuffEffects.Count; i++)
            {
                ExpireBase expire = m_Buffs.Find(p => p.m_EffectIndex == m_BuffEffects[i].I_SFXIndex);
                if (expire == null)
                {
                    m_BuffEffects[i].StopParticles();
                    m_BuffEffects.RemoveAt(i);
                }
            }

            //Refresh Or Add Effects
            for (int i = 0; i < m_Buffs.Count; i++)
            {
                if (m_Buffs[i].m_EffectIndex <= 0)
                    return;

                SFXBuffEffect particle = m_BuffEffects.Find(p=>p.I_SFXIndex==m_Buffs[i].m_EffectIndex);

                if (particle)
                    particle.Refresh(m_Buffs[i].m_ExpireDuration);
                else
                {
                    particle = ObjectManager.SpawnBuffEffect(m_Buffs[i].m_EffectIndex, m_Entity);
                    particle.Play(m_Entity.I_EntityID, m_Buffs[i].m_ExpireDuration);
                    m_BuffEffects.Add(particle );
                }
            }
            OnInfoChange();
        }
    }

    public class ExpireBase
    {
        public virtual int m_EffectIndex => -1;
        public virtual int m_Index => -1;
        public virtual enum_ExpireRefreshType m_RefreshType => enum_ExpireRefreshType.Invalid;
        public virtual float m_MovementSpeedMultiply => 0;
        public virtual float m_FireRateMultiply => 0;
        public virtual float m_ReloadRateMultiply => 0;
        public virtual float m_DamageMultiply => 0;
        public virtual float m_DamageReduction => 0;
        public virtual float m_ExpireDuration => 0;

        protected Action<ExpireBase> OnExpired;
        float f_expireCheck;
        public ExpireBase(Action<ExpireBase> _OnExpired)
        {
            OnExpired = _OnExpired;
            ExpireRefresh();
        }
        public void ExpireRefresh()
        {
            f_expireCheck = m_ExpireDuration;
        }
        public virtual void OnTick(float deltaTime)
        {
            if (m_ExpireDuration <= 0)
                return;

            f_expireCheck -= deltaTime;
            if (f_expireCheck <= 0)
                OnExpired(this);
        }
    }

    public class ExpireBuff:ExpireBase
    {
        public override int m_EffectIndex => m_buffInfo.m_EffectIndex;
        public override enum_ExpireRefreshType m_RefreshType => m_buffInfo.m_AddType;
        public override float m_DamageMultiply => m_buffInfo.m_DamageMultiply;
        public override float m_DamageReduction => m_buffInfo.m_DamageReduction;
        public override int m_Index => m_buffInfo.m_Index;
        public override float m_FireRateMultiply => m_buffInfo.m_FireRateMultiply;
        public override float m_MovementSpeedMultiply => m_buffInfo.m_MovementSpeedMultiply;
        public override float m_ReloadRateMultiply => m_buffInfo.m_ReloadRateMultiply;
        public override float m_ExpireDuration => m_buffInfo.m_ExpireDuration;
        SBuff m_buffInfo;
        Func<DamageInfo, bool> OnDOTDamage;
        int I_SourceID;
        float  f_dotCheck;
        public ExpireBuff(int sourceID,SBuff _buffInfo, Func<DamageInfo, bool> _OnDOTDamage, Action<ExpireBase> _OnBuffExpired) :base(_OnBuffExpired)
        {
            I_SourceID = sourceID;
            m_buffInfo = _buffInfo;
            OnDOTDamage = _OnDOTDamage;
        }
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
            if (m_buffInfo.m_DamageTickTime <= 0)
                return;
            f_dotCheck += deltaTime;
            if (f_dotCheck > m_buffInfo.m_DamageTickTime)
            {
                f_dotCheck -= m_buffInfo.m_DamageTickTime;
                OnDOTDamage(new DamageInfo(I_SourceID, m_buffInfo.m_DamagePerTick, m_buffInfo.m_DamageType, DamageBuffInfo.Default()));
            }
        }
    }
    
    public struct DamageBuffInfo
    {
        public float F_DamageMultiply { get; private set; }
        public float F_DamageAdditive { get; private set; }
        public List<int> m_BuffAplly { get; private set; }
        public static DamageBuffInfo Default()
        {
            DamageBuffInfo info = new DamageBuffInfo()
            {
                F_DamageMultiply = 0f,
                F_DamageAdditive = 0f,
                m_BuffAplly = new List<int>()
            };
            return info;
        }
        public static DamageBuffInfo Create(List<int> _buffApply)
        {
        DamageBuffInfo info = new DamageBuffInfo()
        {
            F_DamageMultiply = 0f,
            F_DamageAdditive = 0f,
            m_BuffAplly = _buffApply
            };
            return info;
        }
        public static  DamageBuffInfo Create(  float _damageEnhanceMultiply,float _damageAdditive=0f)
        {
            DamageBuffInfo info = new DamageBuffInfo()
            {
                F_DamageAdditive = _damageAdditive,
                F_DamageMultiply = _damageEnhanceMultiply,
                m_BuffAplly = new List<int>(),
            };
            return info;
        }
    }
    #endregion
    #region Action
    public class PlayerInfoManager : EntityInfoManager
    {
        List<ActionBase> m_ActionStored = new List<ActionBase>();
        List<ActionBase> m_ActionInPool = new List<ActionBase>();
        List<ActionBase> m_ActionHodling = new List<ActionBase>();

        List<ActionBase> m_ActionEquiping = new List<ActionBase>();

        public float F_ActionAmount { get; private set; }
        EntityPlayerBase m_Player;
        public PlayerInfoManager(EntityPlayerBase _attacher, Func<DamageInfo, bool> _OnReceiveDamage, Action _OnInfoChange) : base(_attacher, _OnReceiveDamage, _OnInfoChange)
        {
            m_Player = _attacher;
            F_ActionAmount = GameConst.F_MaxActionAmount;
            TBroadCaster<enum_BC_GameStatusChanged>.Add<int, EntityBase, float>(enum_BC_GameStatusChanged.OnEntityDamage, OnEntityApplyDamage);
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            TBroadCaster<enum_BC_GameStatusChanged>.Remove<int, EntityBase, float>(enum_BC_GameStatusChanged.OnEntityDamage, OnEntityApplyDamage);
        }
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            m_ActionEquiping.Traversal((ActionBase action) => { action.OnTick(deltaTime); });
        }
        public override DamageBuffInfo GetDamageInfo()
        {
            float damageAdditive = 0;
            m_ActionEquiping.Traversal((ActionBase action) =>{  damageAdditive += action.F_DamageAdditive(m_Player);});
            return DamageBuffInfo.Create(F_DamageMultiply, damageAdditive);
        }
        public void TestUseAction(int index)
        {
            ActionBase targetAction = DataManager.GetAction(index, enum_ActionLevel.L3, OnActionExpired);
            m_ActionEquiping.Add(targetAction);
            targetAction.OnActionUse(m_Player);
        }
        void OnActionExpired(ExpireBase action)
        {
            m_ActionEquiping.Remove(action as ActionBase);
            if(action.m_ExpireDuration>0)
                OnInfoChanged();
        }
        void OnEntityApplyDamage(int applierID, EntityBase damageEntity, float amountApply)
        {
            if (applierID != m_Entity.I_EntityID)
                return;
            AddActionAmount(GameExpression.F_ActionAmountReceive(amountApply));
        }

        public void AddActionAmount(float amount)
        {
            F_ActionAmount += amount;
            if (F_ActionAmount > GameConst.F_MaxActionAmount)
                F_ActionAmount = GameConst.F_MaxActionAmount;
        }
    }
    public class ActionBase : ExpireBase
    {
        public enum_ActionLevel m_Level { get; private set; } = enum_ActionLevel.Invalid;
        protected virtual int I_ActionCost => -1;
        public virtual bool B_ActionAble => true;
        public virtual enum_ActionType m_Type => enum_ActionType.Invalid;
        public virtual float GetValue1(EntityPlayerBase _actionEntity) => 0;
        public virtual float GetValue2(EntityPlayerBase _actionEntity) => 0;
        public virtual float F_DamageAdditive(EntityPlayerBase _actionEntity) => 0;

        public ActionBase(enum_ActionLevel _level,Action<ExpireBase> _OnActionExpired) :base(_OnActionExpired)
        {
            m_Level = _level;
        }
        public virtual void OnActionUse(EntityPlayerBase _entity)
        {
        }
        public virtual void OnFireOnce(EntityPlayerBase _entity)
        {
        }
        public virtual void OnEnermyKilled(EntityPlayerBase _entity)
        {
        }

        public bool B_Upgradable => m_Level < enum_ActionLevel.L3;
        public void Upgrade()
        {
            if (m_Level < enum_ActionLevel.L3)
                m_Level++;
        }
    }

    #region Armor
    public class Action_Armor1_Additive : ActionBase
    {
        public override int m_Index => 10001;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        protected override int I_ActionCost =>ActionData.I_Armor1_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_Armor1_Additive(m_Level);
        public override void OnActionUse(EntityPlayerBase _actionEntity) {
            _actionEntity.m_HitCheck.TryHit(new DamageInfo(_actionEntity.I_EntityID, GetValue1(_actionEntity), enum_DamageType.ArmorOnly, DamageBuffInfo.Default()));
            OnExpired(this);
        }
        public Action_Armor1_Additive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) {}
    }
    public class Action_Armor2_DamageAdditive : ActionBase
    {
        public override int m_Index => 10002;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        public override float m_ExpireDuration => ActionData.F_Armor2_Duration;
        protected override int I_ActionCost => ActionData.I_Armor2_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_Armor2_DamageAdditive(m_Level,_actionEntity.m_HealthManager.m_CurrentArmor);
        public override float F_DamageAdditive(EntityPlayerBase _actionEntity) => GetValue1(_actionEntity);
        public Action_Armor2_DamageAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired){}
    }
    public class Action_Armor3_MultiplyAdditive : ActionBase
    {
        public override int m_Index => 10003;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        protected override int I_ActionCost => ActionData.I_Armor3_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_Armor3_MultiplyAdditive(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override void OnActionUse(EntityPlayerBase _actionEntity) {
            _actionEntity.m_HitCheck.TryHit(new DamageInfo(_actionEntity.I_EntityID, GetValue1(_actionEntity), enum_DamageType.ArmorOnly, DamageBuffInfo.Default()));
            OnExpired(this);
        } 
        public Action_Armor3_MultiplyAdditive(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired){}
    }
    public class Action_Armor4_ActionReturn : ActionBase
    {
        public override int m_Index => 10004;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        protected override int I_ActionCost => ActionData.I_Armor4_Cost;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_Armor4_ActionReturn(m_Level, _actionEntity.m_HealthManager.m_CurrentArmor);
        public override void OnActionUse(EntityPlayerBase _actionEntity)
        {
            (_actionEntity.m_EntityInfo as PlayerInfoManager).AddActionAmount(GetValue1(_actionEntity));
            OnExpired(this);
        } 
        public Action_Armor4_ActionReturn(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    public class Action_Armor5_DamageReduction : ActionBase
    {
        public override int m_Index => 10005;
        public override int m_EffectIndex => base.m_EffectIndex;
        public override enum_ActionType m_Type => enum_ActionType.Action;
        protected override int I_ActionCost => ActionData.I_Armor5_Cost;
        public override float m_ExpireDuration => ActionData.F_Armor5_Duration;
        public override float GetValue1(EntityPlayerBase _actionEntity) => ActionData.F_Armor5_DamageReduction(m_Level);
        public override float m_DamageReduction => GetValue1(null);
        public Action_Armor5_DamageReduction(enum_ActionLevel _level, Action<ExpireBase> _OnActionExpired) : base(_level, _OnActionExpired) { }
    }
    
    #endregion
    #endregion
    #region Physics
    public static class HitCheckDetect_Extend
    {
        public static HitCheckBase Detect(this Collider other)
        {
            HitCheckBase hitCheck = other.GetComponent<HitCheckBase>();
            if (hitCheck == null)
                Debug.LogWarning("Null Hit Check Attached:" + other.gameObject);

            return hitCheck;
        }
        public static HitCheckEntity DetectEntity(this Collider other)
        {
            HitCheckBase hitCheck = other.GetComponent<HitCheckBase>();
            if (hitCheck == null)
                Debug.LogWarning("Null Hit Check Attached:" + other.gameObject);
            if (hitCheck.m_HitCheckType == enum_HitCheck.Entity)
                return hitCheck as HitCheckEntity;
            return null;
        }
    }
    public class ProjectilePhysicsSimulator : PhysicsSimulatorCapsule<HitCheckBase> 
    {
        protected Vector3 m_VerticalDirection;
        float m_horizontalSpeed;
        public ProjectilePhysicsSimulator(Transform _transform, Vector3 _startPos, Vector3 _horizontalDirection, Vector3 _verticalDirection, float _horizontalSpeed, float _height, float _radius, int _hitLayer, Func<RaycastHit,HitCheckBase,bool> _onTargetHit,Predicate<HitCheckBase> _canHitTarget):base(_transform,_startPos, _horizontalDirection,_height,_radius,_hitLayer,_onTargetHit,_canHitTarget)
        {
            m_VerticalDirection = _verticalDirection.normalized;
            m_horizontalSpeed = _horizontalSpeed;
        }
        public override Vector3 GetSimulatedPosition(float elapsedTime)=> m_startPos + m_Direction * Expressions.SpeedShift(m_horizontalSpeed, elapsedTime); 
    }
    public class ProjectilePhysicsLerpSimulator : PhysicsSimulatorCapsule<HitCheckBase>
    {
        bool b_lerpFinished;
        Action OnLerpFinished;
        Vector3 m_endPos;
        float f_totalTime;
        public ProjectilePhysicsLerpSimulator(Transform _transform, Vector3 _startPos,Vector3 _endPos,Action _OnLerpFinished, float _duration, float _height, float _radius, int _hitLayer, Func<RaycastHit,HitCheckBase, bool> _onTargetHit, Predicate<HitCheckBase> _canHitTarget) : base(_transform, _startPos,_endPos-_startPos , _height, _radius, _hitLayer, _onTargetHit,_canHitTarget)
        {
            m_endPos = _endPos;
            OnLerpFinished = _OnLerpFinished;
            f_totalTime= _duration;
            b_lerpFinished = false;
        }
        public override void Simulate(float deltaTime)
        {
            base.Simulate(deltaTime);
            if (!b_lerpFinished && m_simulateTime > f_totalTime)
            {
                OnLerpFinished?.Invoke();
                b_lerpFinished = true;
            }
         }
        public override Vector3 GetSimulatedPosition(float elapsedTime) =>b_lerpFinished?m_endPos:Vector3.Lerp(m_startPos, m_endPos, elapsedTime / f_totalTime);
    }
    #endregion

    #region GameEffects
    public class ModelBlink:ISingleCoroutine
    {
        Material[] m_materials;
        float f_simulate;
        float f_blinkRate;
        float f_blinkTime;
        public ModelBlink(Transform BlinkModel, float _blinkRate, float _blinkTime)
        {
            if (BlinkModel == null)
                Debug.LogError("Error! Blink Model Init, BlinkModel Folder Required!");

            Renderer[] renderers=BlinkModel.GetComponentsInChildren<Renderer>();
            m_materials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                m_materials[i] = renderers[i].material;
            f_blinkRate = _blinkRate;
            f_blinkTime = _blinkTime;
            f_simulate = 0f;
            OnReset();
        }
        public void OnReset()
        {
            f_simulate = 0f;
            this.StopSingleCoroutine(0);
            m_materials.Traversal((Material material) => { material.SetColor("_Color", TCommon.ColorAlpha(Color.red, 0)); }); 
        }
        public void Tick(float deltaTime)
        {
            f_simulate += deltaTime;
            if (f_simulate > f_blinkRate)
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
                    m_materials.Traversal((Material material) => { material.SetColor("_Color", TCommon.ColorAlpha(Color.red, value)); }); }, 1, 0, f_blinkTime));
                f_simulate -= f_blinkRate;
            }
        }
    }
    
    #endregion

    #region BigmapTile
    public class SBigmapTileInfo : ITileAxis
    {
        public TileAxis m_TileAxis => m_Tile;
        protected TileAxis m_Tile { get; private set; }
        public enum_TileType m_TileType { get; private set; } = enum_TileType.Invalid;
        public enum_TileLocking m_TileLocking { get; private set; } = enum_TileLocking.Invalid;
        public Dictionary<enum_TileDirection, TileAxis> m_Connections { get; protected set; } = new Dictionary<enum_TileDirection, TileAxis>();

        public SBigmapTileInfo(TileAxis _tileAxis, enum_TileType _tileType, enum_TileLocking _tileLocking)
        {
            m_Tile = _tileAxis;
            m_TileType = _tileType;
            m_TileLocking = _tileLocking;
        }
        public void ResetTileType(enum_TileType _tileType)
        {
            m_TileType = _tileType;
        }
        public void SetTileLocking(enum_TileLocking _lockType)
        {
            if(m_TileLocking!= enum_TileLocking.Unlocked)
               m_TileLocking = _lockType;
        }
    }

    public class SBigmapLevelInfo : SBigmapTileInfo
    {
        public LevelBase m_Level { get; private set; } = null;
        public SBigmapLevelInfo(SBigmapTileInfo tile) : base(tile.m_TileAxis, tile.m_TileType,tile.m_TileLocking)
        {
            m_Connections = tile.m_Connections;
        }
        public Dictionary<LevelItemBase, int> GenerateMap(LevelBase levelSpawned,SLevelGenerate innerData,SLevelGenerate outerData, Dictionary<enum_LevelItemType,List<LevelItemBase>> _levelItemPrefabs,System.Random seed)
        {
            m_Level = levelSpawned;
            m_Level.transform.localRotation = Quaternion.Euler(0, seed.Next(360), 0);
            m_Level.transform.localPosition = Vector3.zero;
            m_Level.transform.localScale = Vector3.one;
            m_Level.SetActivate(false);
            return m_Level.GenerateTileItems(innerData,outerData, _levelItemPrefabs, m_TileType,seed, m_TileType== enum_TileType.End);        //Add Portal For Level End
        }
        public void StartLevel()
        {
            m_Level.ShowAllItems();
            m_Level.SetActivate(true);
        }
    }
    #endregion

    #region LevelTile
    public class LevelTile 
    {
        public TileAxis m_TileAxis;
        public Vector3 m_Offset => GameExpression.V3_TileAxisOffset(m_TileAxis);
        public virtual enum_LevelTileType E_TileType => enum_LevelTileType.Empty;
        public enum_LevelTileOccupy E_Occupation { get; private set; } = enum_LevelTileOccupy.Invalid;

        public LevelTile(TileAxis _axis ,enum_LevelTileOccupy _occupy) 
        {
            m_TileAxis = _axis;
            E_Occupation = _occupy;
        }

        public LevelTile(LevelTile tile)
        {
            m_TileAxis = tile.m_TileAxis;
            E_Occupation = tile.E_Occupation;
        }
    }
    public class LevelTilePortal : LevelTileItem
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Portal;
        public LevelTilePortal(LevelTile current,List<int> _subTileIndex,int prefabIndex) : base(current, prefabIndex, enum_LevelItemType.Portal, enum_TileDirection.Top,_subTileIndex)
        {
        }
    }
    public class LevelTileSub : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Item;
        public int m_ParentTileIndex { get; private set; }
        public LevelTileSub(LevelTile current, int _parentIndex) : base(current)
        {
            m_ParentTileIndex = _parentIndex;
        }
    }
    public class LevelTileItem : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Main;
        public int m_LevelItemListIndex { get; private set; }
        public enum_LevelItemType m_LevelItemType { get; private set; }
        public enum_TileDirection m_ItemDirection { get; private set; }
        public List<int> m_subTiles { get; private set; }
        
        public LevelTileItem(LevelTile current, int levelItemListIndex, enum_LevelItemType levelItemType,enum_TileDirection _ItemDirection, List<int> _subTiles) : base(current)
        {
            m_LevelItemListIndex = levelItemListIndex;
            m_LevelItemType = levelItemType;
            m_subTiles = _subTiles;
            m_ItemDirection = _ItemDirection;
        }
    }
    class LevelTileBorder : LevelTileItem
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Border;
        public LevelTileBorder(LevelTile current, int levelItemListIndex, enum_LevelItemType levelItemType, enum_TileDirection _ItemDirection) : base(current,levelItemListIndex,levelItemType,_ItemDirection,null)
        {
        }
    }
    #endregion

    #region ExcelStruct
    public struct SWeapon : ISExcel
    {
        int index;
        string s_name;
        int i_muzzleIndex;
        int i_projectileIndex;
        float f_fireRate;
        float f_specialRate;
        int i_clipAmount;
        float f_spread;
        float f_reloadTime;
        int i_PelletsPerShot;
        float f_stunAfterShot;
        float f_recoilHorizontal;
        float f_recoilVertical;
        public enum_PlayerWeapon m_Weapon => (enum_PlayerWeapon)index;
        public string m_Name => s_name;
        public int m_ProjectileSFX => i_projectileIndex;
        public int m_MuzzleSFX => i_muzzleIndex;
        public float m_FireRate => f_fireRate;
        public float m_SpecialRate => f_specialRate;
        public int m_ClipAmount => i_clipAmount;
        public float m_Spread => f_spread;
        public float m_ReloadTime => f_reloadTime;
        public int m_PelletsPerShot => i_PelletsPerShot;
        public float m_stunAfterShot => f_stunAfterShot;
        public Vector2 m_RecoilPerShot => new Vector2(f_recoilHorizontal, f_recoilVertical);
        public void InitOnValueSet()
        {
        }
    }
    public struct SLevelGenerate : ISExcel
    {
        string em_defines;
        RangeInt ir_length;
        RangeInt ir_smallLess;
        RangeInt ir_smallMore;
        RangeInt ir_mediumLess;
        RangeInt ir_mediumMore;
        RangeInt ir_largeLess;
        RangeInt ir_largeMore;
        RangeInt ir_manmadeLess;
        RangeInt ir_manmadeMore;
        RangeInt ir_noCollisionLess;
        RangeInt ir_noCollisionMore;
        public bool m_IsInner;
        public enum_Style m_LevelStyle;
        public enum_LevelGenerateType m_LevelPrefabType;
        public Dictionary<enum_LevelItemType, RangeInt> m_ItemGenerate;
        public RangeInt m_Length => ir_length;
        public void InitOnValueSet()
        {
            string[] defineSplit = em_defines.Split('_');
            if (defineSplit.Length != 3)
                Debug.LogError("Please Corret Format Of DefineSplit:" +em_defines   );
            m_LevelStyle = (enum_Style)(int.Parse(defineSplit[0]));
            m_LevelPrefabType = (enum_LevelGenerateType)(int.Parse(defineSplit[1]));
            m_IsInner = int.Parse(defineSplit[2]) == 1;
            m_ItemGenerate = new Dictionary<enum_LevelItemType, RangeInt>(); 
            m_ItemGenerate.Add(enum_LevelItemType.LargeLess, ir_largeLess);
            m_ItemGenerate.Add(enum_LevelItemType.LargeMore, ir_largeMore);
            m_ItemGenerate.Add(enum_LevelItemType.MediumLess, ir_mediumLess);
            m_ItemGenerate.Add(enum_LevelItemType.MediumMore, ir_mediumMore);
            m_ItemGenerate.Add(enum_LevelItemType.SmallLess,ir_smallLess);
            m_ItemGenerate.Add(enum_LevelItemType.SmallMore, ir_smallMore);
            m_ItemGenerate.Add(enum_LevelItemType.ManmadeLess, ir_manmadeLess);
            m_ItemGenerate.Add(enum_LevelItemType.ManmadeMore, ir_manmadeMore);
            m_ItemGenerate.Add(enum_LevelItemType.NoCollisionLess, ir_noCollisionLess);
            m_ItemGenerate.Add(enum_LevelItemType.NoCollisionMore, ir_noCollisionMore);
        }
    }
    public struct SGenerateEntity:ISExcel
    {
        string em_defines;
        float f_eliteChance;
        RangeInt ir_fighter;
        RangeInt ir_shooterRookie;
        RangeInt ir_shooterVeteran;
        RangeInt ir_aoeCaster;
        RangeInt ir_elite;
        public int m_waveCount;
        public enum_BattleDifficulty m_Difficulty;
        public float m_EliteChance => f_eliteChance;
        public Dictionary<enum_EntityType, RangeInt> m_EntityGenerate;
        public void InitOnValueSet()
        {
            string[] defineSplit = em_defines.Split('_');
            m_Difficulty = (enum_BattleDifficulty)(int.Parse(defineSplit[0]));
            m_waveCount = (int.Parse(defineSplit[1]));
            m_EntityGenerate = new Dictionary<enum_EntityType, RangeInt>();
            m_EntityGenerate.Add(enum_EntityType.Fighter,ir_fighter);
            m_EntityGenerate.Add(enum_EntityType.Shooter_Rookie, ir_shooterRookie);
            m_EntityGenerate.Add(enum_EntityType.Shooter_Veteran, ir_shooterVeteran);
            m_EntityGenerate.Add(enum_EntityType.AOECaster, ir_aoeCaster);
            m_EntityGenerate.Add(enum_EntityType.Elite, ir_elite);
        }
    }
    public struct SBuff : ISExcel
    {
        int index;
        int i_addType;
        float f_expireDuration;
        int i_effect;
        float f_movementSpeedMultiply;
        float f_fireRateMultiply;
        float f_reloadRateMultiply;
        float f_damageMultiply;
        float f_damageReduce;
        float f_damageTickTime;
        float f_damagePerTick;
        int i_damageType;
        public int m_Index => index;
        public enum_ExpireRefreshType m_AddType => (enum_ExpireRefreshType)i_addType;
        public float m_ExpireDuration => f_expireDuration;
        public int m_EffectIndex => i_effect;
        public float m_MovementSpeedMultiply => f_movementSpeedMultiply ;
        public float m_FireRateMultiply => f_fireRateMultiply;
        public float m_ReloadRateMultiply => f_reloadRateMultiply;
        public float m_DamageMultiply => f_damageMultiply;
        public float m_DamageReduction => f_damageReduce ;
        public float m_DamageTickTime => f_damageTickTime;
        public float m_DamagePerTick => f_damagePerTick;
        public enum_DamageType m_DamageType =>(enum_DamageType)i_damageType;
        public void InitOnValueSet()
        {
            f_movementSpeedMultiply = f_movementSpeedMultiply > 0 ? f_movementSpeedMultiply/100f : 0;
            f_fireRateMultiply = f_fireRateMultiply > 0 ? f_fireRateMultiply / 100f : 0;
            f_reloadRateMultiply = f_reloadRateMultiply > 0 ? f_reloadRateMultiply / 100f : 0;
            f_damageMultiply = f_damageMultiply > 0 ? f_damageMultiply / 100f : 0;
            f_damageReduce = f_damageReduce > 0 ? f_damageReduce / 100f : 0;
        }
    }
    #endregion
    #endregion

    #region For UI Usage     
    class CSporeManagerSave : ISave     //Locked=-1 Spare=1
    {
        public float f_coin;
        public int i_maxLevel;
        public int i_previousTimeStamp;
        public double d_timePassed;
        protected Dictionary<int, int> d_sporeContainerInfo;
        public int I_SlotCount => d_sporeContainerInfo.Count;
        public int this[int slotIndex]
        {
            set
            {
                if (!d_sporeContainerInfo.ContainsKey(slotIndex))
                    Debug.LogError("Invalid Slot Index:" + slotIndex);

                d_sporeContainerInfo[slotIndex] = value;
            }
            get
            {
                if (!d_sporeContainerInfo.ContainsKey(slotIndex))
                    Debug.LogError("Invalid Slot Index:" + slotIndex);

                return d_sporeContainerInfo[slotIndex];
            }
        }
        public void AddSlotValue(int slotIndex, int value = 1)
        {

            if (!d_sporeContainerInfo.ContainsKey(slotIndex))
                Debug.LogError("Invalid Slot Index:" + slotIndex);

            d_sporeContainerInfo[slotIndex] += value;
        }

        public int GetSpareSlot()
        {
            int spareSlot = -1;
            foreach (int slot in d_sporeContainerInfo.Keys)
            {
                if (d_sporeContainerInfo[slot] == 0)
                {
                    spareSlot = slot;
                    break;
                }
            }
            return spareSlot;
        }
        public void UnlockNewSlot()
        {
            foreach (int slot in d_sporeContainerInfo.Keys)
                if (d_sporeContainerInfo[slot] == -1)     //If Got New Slot Unlock it
                {
                    d_sporeContainerInfo[slot] = 0;
                    break;
                }
        }
        public CSporeManagerSave()
        {
            f_coin = 1000f;
            i_maxLevel = 1;
            d_timePassed = 0;
            i_previousTimeStamp = TTime.TTime.GetTimeStamp(DateTime.Now);
            d_sporeContainerInfo = new Dictionary<int, int>() { };
            for (int i = 1; i <= UIConst.I_SporeManagerContainersMaxAmount; i++)
                d_sporeContainerInfo.Add(i, i <= UIConst.I_SporeManagerContainerStartFreeSlot ? 0 : -1);
        }
    }

    public struct SSporeLevel : ISExcel
    {
        int maxLevel;
        float offset0;
        float offset1;
        float offset2;
        float offset3;
        float offset4;
        float offset5;
        float offset6;
        float offset7;
        float offset8;
        float offset9;
        bool addSlot;
        public int MaxLevel => maxLevel;
        public float F_OffSet0 => offset0;
        public float F_OffSet1 => offset1;
        public float F_OffSet2 => offset2;
        public float F_OffSet3 => offset3;
        public float F_OffSet4 => offset4;
        public float F_OffSet5 => offset5;
        public float F_OffSet6 => offset6;
        public float F_OffSet7 => offset7;
        public float F_OffSet8 => offset8;
        public float F_OffSet9 => offset9;
        public bool B_AddSlot => addSlot;
        public void InitOnValueSet()
        {

        }
    }

    public class SSporeLevelRate
    {
        SSporeLevel m_BaseInfo;
        public float F_CoinChestPrice { get; private set; }
        public float F_BlueChestPrice { get; private set; }
        public float F_MaxCoinsAmount { get; private set; }
        public int I_Level => m_BaseInfo.MaxLevel;
        public bool B_AddSlot => m_BaseInfo.B_AddSlot;
        public List<float> l_sporeRates;
        public SSporeLevelRate(SSporeLevel rate)
        {
            m_BaseInfo = rate;
            l_sporeRates = new List<float>() { m_BaseInfo.F_OffSet0, m_BaseInfo.F_OffSet1, m_BaseInfo.F_OffSet2, m_BaseInfo.F_OffSet3, m_BaseInfo.F_OffSet4, m_BaseInfo.F_OffSet5, m_BaseInfo.F_OffSet6, m_BaseInfo.F_OffSet7, m_BaseInfo.F_OffSet8, m_BaseInfo.F_OffSet9 };
            float count = 0;
            for (int i = 0; i < l_sporeRates.Count; i++)
                count += l_sporeRates[i];
            if (I_Level != -1 && count != 100)
                Debug.LogError("Spore Rate Total Unmatch 100! Line:" + I_Level);

            F_CoinChestPrice = UIExpression.F_SporeManagerChestCoinRequirement(I_Level);
            F_BlueChestPrice = UIExpression.F_SporeManagerChestBlueRequirement(I_Level);
            F_MaxCoinsAmount = UIExpression.F_SporeManagerCoinsMaxAmount(I_Level);
        }

        public int AcquireNewSpore()
        {
            int random = UnityEngine.Random.Range(1, 101);
            float count = 0;
            int offset = 0;
            for (int i = 0; i < l_sporeRates.Count; i++)
            {
                count += l_sporeRates[i];
                if (random < count)
                {
                    offset = i;
                    break;
                }
            }
            return I_Level - offset;
        }
    }
    #endregion

    #endregion
}
