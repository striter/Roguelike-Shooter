﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using LevelSetting;
using GameSetting_CharacterPlayerPerks_10000;

namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
        #region Battle
        public static readonly RangeInt RI_EnermyGenerateDuration = new RangeInt(15, 25);
        public const float F_EnermyGenerateTickMultiplierPerMinute =.01f;
        public const float F_EnermyGenerateTickMultiplierTransmiting = 0.5f;

        public const float F_EnermyMaxHealthMultiplierPerMinutePassed=.1f;
        public const float F_EnermyMaxHealthMultiplierPerDifficultyAboveNormal = .25f;
        public const float F_EnermyDamageMultiplierPerMinutePassed = .1f;
        public const float F_EnermyDamageMultiplierPerDifficultyAboveNormal = .25f;

        public const float F_EnermyEliteGenerateBase = 5f;
        public const float F_EnermyEliteGeneratePerMinuteMultiplier = 0.5f;
        public const float F_SqrEnermyGenerateMinDistance = 100f; // 20*20

        public const float F_SignalTowerTransmitDuration = 20f;
        public const float F_SignalTowerSquareTransmitDistance = 100f;      //10*10

        public const float F_BlastShakeMultiply = .5f;
        public const float F_DamageImpactMultiply = 1f;

        public const float F_EntityDeadFadeTime = 3f;
        public const float F_PlayerReviveCheckAfterDead = 1.5f;
        public const float F_PlayerReviveBuffDuration = 6f; //复活无敌时间

        public const float F_AimMovementReduction = .5f;
        public const float F_MovementReductionDuration = .1f;
        public const int I_ProjectileInvalidDistance = 100;
        public const int I_ProjectileParacurveInvalidDistance = 15;
        public const int I_ProjectileBlinkWhenTimeLeftLessThan = 3;
        public const float F_AimAssistDistance = 100f;
        public const short I_BoltLastTimeAfterHit = 5;

        public const float F_PlayerArmorRegenDuration = 5f;
        public const float F_PlayerArmorRegenPerSec = 5f;
        public const float F_PlayerWeaponFireReloadPause = 1f; //武器恢复间隔时间
        public const float F_PlayerAutoAimRangeBase = 16f; //自动锁定敌人范围
        public const int I_PlayerRotationSmoothParam = 10;     //Camera Smooth Param For Player 10 is suggested
        public const int I_PlayerEnermyKillExpGain = 20;

        public const float F_EliteBuffTimerDurationWhenFullHealth = 15f; //触发tick值
        public const float F_EliteBuffTimerTickRateMultiplyHealthLoss = 2f; //每秒加几tick=(1+血量损失比例* X)
        public static readonly List<int> L_GameEliteIndexes = new List<int>() { 107, 207, 306, 407, 507 };
        public static readonly List<EliteBuffCombine> L_GameEliteBuff = new List<EliteBuffCombine>() { new EliteBuffCombine(211, 12010, 32010), new EliteBuffCombine(213, 12030, 32030), new EliteBuffCombine(214, 12040, 32040), new EliteBuffCombine(215, 12050, 32050), new EliteBuffCombine(216, 12060, 32060) };

        public static readonly List<int> m_GameDebuffID = new List<int>() { 103, 104, 105 };
        #endregion
        #region Interacts
        public static readonly RangeInt RI_GameEventGenerate = new RangeInt(25, 6);

        public const float F_TradePriceMultiplyAdditivePerMin = .05f;

        public static readonly RangeInt RI_EnermyCoinsGenerate = new RangeInt(1,5);
        public const float F_EnermyKeyGenerate = 2.5f;

        public const float F_PickupMaxSpeed = 100f;
        public const float F_PickupAcceleration = 50f; //拾取物的飞行加速速度
        public const int I_HealthPickupAmount = 25;
        public const int I_ArmorPickupAmount = 25;
        public const int I_HealthPackAmount = 50;

        public const int I_DangerzoneDamage = 50;
        public const float F_DangerzoneResetDuration = 2f;

        public static Dictionary<enum_BattleEvent, float> D_GameEventRate = new Dictionary<enum_BattleEvent, float>() { {enum_BattleEvent.CoinsSack,17.5f }, { enum_BattleEvent.HealthpackTrade, 7.5f }, { enum_BattleEvent.WeaponTrade, 0f }, { enum_BattleEvent.WeaponReforge, 5f }, { enum_BattleEvent.WeaponVendor, 25f }, { enum_BattleEvent.WeaponRecycle, 0f }, { enum_BattleEvent.PerkLottery, 22.5f }, { enum_BattleEvent.PerkSelect, 5f }, { enum_BattleEvent.PerkShrine, 10f }, { enum_BattleEvent.BloodShrine, 5f }, { enum_BattleEvent.HealShrine, 0f }, { enum_BattleEvent.SafeBox, 2.5f }, };

        public static RangeInt RI_CoinsSackAmount = new RangeInt(6, 4);
        public const int I_EventMedpackPrice = 0;

        public const int I_EventWeaponTradePrice = 12;
        public static Dictionary<enum_Rarity, float> D_EventWeaponTradeRate = new Dictionary<enum_Rarity, float>() { { enum_Rarity.Ordinary, 30 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 25 },{ enum_Rarity.Epic, 15 } };

        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponReforgeRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 25 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 25 } };
        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponRecyclePrice = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 20 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 50 }, { enum_Rarity.Epic, 75 } };

        public const int I_EventWeaponVendorMachinePrice = 12;
        public static readonly Dictionary<enum_Rarity, float> D_EventWeaponVendorMachineRate = new Dictionary<enum_Rarity, float>() { { enum_Rarity.Ordinary, 30 }, { enum_Rarity.Advanced, 40 }, { enum_Rarity.Rare, 20 }, { enum_Rarity.Epic, 10 } };
        public const int I_EventWeaponVendorTryCount = 0;
        public const int I_EventPerkLotteryPrice = 12;
        public static readonly Dictionary<enum_Rarity, int> D_EventPerkLotteryRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 55 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 10 }, { enum_Rarity.Epic, 5 } };
        public const int I_EventPerkSelectPrice = 15;
        public static readonly Dictionary<enum_Rarity, int> D_EventPerkSelectRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 55 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 10 }, { enum_Rarity.Epic, 5 } };
        public const int I_PerkShrineTradePrice = 2;
        public const int I_PerkShrineTryCountMax = 10086;
        public static readonly Dictionary<enum_Rarity, int> D_PerkShrineRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 15 }, { enum_Rarity.Advanced, 8 }, { enum_Rarity.Rare, 5 }, { enum_Rarity.Epic, 2 } };
        public const int I_BloodShrineTryCountMax = 99999;
        public static readonly RangeInt RI_BloodShrintCoinsAmount = new RangeInt(10, 20);
        public const int I_HealShrineTradePrice = 10;
        public const int I_HealShrineTryCountMax = 5;
        public const float F_HealShrineHealthReceive = 30f;

        public static readonly RangeInt I_EventSafeCoinsAmount = new RangeInt(10,10);
        public static readonly RangeInt RI_EventSafeWeaponCount = new RangeInt(1, 1);
        public static readonly Dictionary<enum_Rarity, float> D_EventSafeWeaponRate = new Dictionary<enum_Rarity, float>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 40 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 10 } };
        public static readonly RangeInt RI_EventSafePerkCount = new RangeInt(1, 1);
        public static readonly Dictionary<enum_Rarity, int> D_EventSafePerkRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 40 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 10 } };

        public static readonly Dictionary<enum_BattleStage, Dictionary<int, float>> m_StageWeaponEnhanceLevel = new Dictionary<enum_BattleStage, Dictionary<int, float>>()
        {
            {enum_BattleStage.Rookie,new Dictionary<int, float>(){ {1,20 },{2,5 } } },
            {enum_BattleStage.Militia,new Dictionary<int, float>(){ { 1,30},{2,10 },{3,5 } } },
            {enum_BattleStage.Veteran,new Dictionary<int, float>(){ {1,35 },{2,25 },{3,15 },{4,5 } } },
            {enum_BattleStage.Elite,new Dictionary<int, float>(){ {1,20 },{2,30 },{3,25f },{ 4,10},{ 5,2.5f} } },
            {enum_BattleStage.Ranger,new Dictionary<int, float>(){ {1,15 },{2,35 },{3,25 },{ 4,15} ,{ 5,5} } },
        };
        #endregion        
        public static class AI
        {
            public const float F_AIMovementCheckParam = .3f; //AI检查玩家频率
            public const float F_AITargetCheckParam = .5f;      //AI Target Duration .5f is Suggested
            public const float F_AIReTargetCheckParam = 3f;       //AI Retarget Duration,3f is suggested
            public const float F_AITargetCalculationParam = .5f;       //AI Target Param Calculation Duration, 1 is suggested;
            public const float F_AIMaxRepositionDuration = .5f;
            public const float F_AIDeadImpactPerDamageValue = 0.2f;   //0.05f;
            public const int I_AIBattleIdlePercentage = 50;
            public static readonly RangeFloat RF_AIBattleIdleDuration = new RangeFloat(1f, 2f);
        }
        #region Cultivate
        public static readonly Dictionary<enum_Rarity, float> m_ArmoryBlueprintGameDropRarities = new Dictionary<enum_Rarity, float>() { { enum_Rarity.Ordinary, 10f }, { enum_Rarity.Advanced, 5f }, { enum_Rarity.Rare, 3f }, { enum_Rarity.Epic, 2f } };
        public static readonly Dictionary<enum_Rarity, int> m_ArmoryBlueprintUnlockPrice = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 1000 }, { enum_Rarity.Advanced, 1500 }, { enum_Rarity.Rare, 3000 }, { enum_Rarity.Epic, 5000} };

        public static readonly Dictionary<enum_PlayerCharacter, enum_PlayerWeaponIdentity> m_CharacterStartWeapon = new Dictionary<enum_PlayerCharacter, enum_PlayerWeaponIdentity>() { { enum_PlayerCharacter.Beth,enum_PlayerWeaponIdentity.UMP45},{ enum_PlayerCharacter.Vampire,enum_PlayerWeaponIdentity.PoisonWand},{ enum_PlayerCharacter.Railer,enum_PlayerWeaponIdentity.RailPistol}, { enum_PlayerCharacter.Machinist, enum_PlayerWeaponIdentity.Flamer } };
        public static readonly Dictionary<enum_Rarity, int> m_CharacterRandomStartWeaponRarities = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 50 }, { enum_Rarity.Advanced, 50 } };

        public static readonly Dictionary<enum_PlayerCharacter, float> m_CharacterUnlockCost = new Dictionary<enum_PlayerCharacter, float>() {
            { enum_PlayerCharacter.Beth,5000 },{ enum_PlayerCharacter.Railer,6000},{ enum_PlayerCharacter.Vampire,7000},{ enum_PlayerCharacter.Machinist,9999}
        };

        public static readonly Dictionary<enum_PlayerCharacterEnhance, int> m_CharacterEnhanceCost = new Dictionary<enum_PlayerCharacterEnhance, int>() {
            { enum_PlayerCharacterEnhance.Health,250 },
            { enum_PlayerCharacterEnhance.Armor, 500 },
            { enum_PlayerCharacterEnhance.MovementSpeed, 1000 },
            { enum_PlayerCharacterEnhance.StartWeapon, 2000 },
            { enum_PlayerCharacterEnhance.StageCoin, 3000 },
            { enum_PlayerCharacterEnhance.Critical, 4000 },
            { enum_PlayerCharacterEnhance.Ability, 5000 },
            { enum_PlayerCharacterEnhance.DropWeapon, 6000 } };

        public const int I_PlayerEnhanceMaxHealthAdditive = 50;
        public const int I_PlayerEnhanceMaxArmorAddtive = 30;
        public const float F_PlayerEnhanceMovementSpeedAdditive = .5f;
        public const int I_PlayerEnhanceStartWeaponEnhanceAdditive = 1;
        public const int I_PlayerEnhanceStageStartCoin = 20;
        public const float F_PlayerEnhanceCriticalRateAdditive = .05f;
        public const int I_PlayerEnhanceDropWeaponEnhanceAdditive = 1;

        public const float F_GameResultCreditStageBase = 500f;
        public const float F_GameResultCreditEnermyKilledBase = 1f;
        public const float F_GameResultCreditDifficultyBonus = .2f;
        #endregion
    }

    public static class GameExpression
    {
        public static int GetPlayerRankUpExp(int curRank) => 100 + curRank * 50;
        public static int GetPlayerWeaponIndex(int weaponIndex) =>weaponIndex * 10;
        public static int GetPlayerExtraWeaponIndex(int weaponIndex) => weaponIndex * 10+5;
        public static int GetPlayerPerkSFXWeaponIndex(int weaponIndex) => 100000 + weaponIndex * 10;
        public static int GetAIWeaponIndex(int entityIndex, int weaponIndex = 0, int subWeaponIndex = 0) => entityIndex * 100 + weaponIndex * 10 + subWeaponIndex;
        public static int GetWeaponSubIndex(int weaponIndex) => weaponIndex + 1;

        public static float F_GameVFXVolume(int vfxVolumeTap) => vfxVolumeTap / 10f;
        public static float F_GameMusicVolume(int musicVolumeTap) => musicVolumeTap / 10f;

        public static float GetEnermyMaxHealthMultiplier(int minutePassed, enum_BattleDifficulty difficulty) => minutePassed * GameConst.F_EnermyMaxHealthMultiplierPerMinutePassed + ((int)difficulty - 1) * GameConst.F_EnermyMaxHealthMultiplierPerDifficultyAboveNormal;
        public static float GetEnermyDamageMultilier(int minutesPassed, enum_BattleDifficulty difficulty) => minutesPassed * GameConst.F_EnermyDamageMultiplierPerMinutePassed + ((int)difficulty - 1) * GameConst.F_EnermyDamageMultiplierPerDifficultyAboveNormal;

        #region Interacts
        public static float GetPerkShrinePriceMultiply(int tryCount) => 1f * tryCount;
        public static float GetHealShrinePriceMultiply(int tryCount) => 1f + .1f * tryCount;
        public static float GetBloodShrineHealthCostMultiple(int count) => 10 + 5 * count;
        #endregion
    }

    public static class GameLayer
    {
        public static readonly int I_Static = LayerMask.NameToLayer("static");
        public static readonly int I_Entity = LayerMask.NameToLayer("entity");
        public static readonly int I_Dynamic = LayerMask.NameToLayer("dynamic");
        public static readonly int I_Interact = LayerMask.NameToLayer("interact");
        public static class Mask
        {
            public static readonly int I_ProjectileMask = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity;
            public static readonly int I_All = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity | 1 << GameLayer.I_Dynamic;
            public static readonly int I_Static = (1 << GameLayer.I_Static);
            public static readonly int I_Entity = (1 << GameLayer.I_Entity);
            public static readonly int I_Interact = (1 << GameLayer.I_Interact);
        }

        public static readonly int I_EntityDetect = LayerMask.NameToLayer("entityDetect");
        public static readonly int I_InteractDetect = LayerMask.NameToLayer("interactDetect");
        public static readonly int I_MovementDetect = LayerMask.NameToLayer("movementDetect");
    }
    #endregion

    #region For Developers Use
    #region Structs
    public struct EliteBuffCombine
    {
        public int m_BuffIndex;
        public int m_MuzzleIndex;
        public int m_IndicatorIndex;
        public EliteBuffCombine(int _buffIndex, int _muzzleIndex, int _indicatorIndex)
        {
            m_BuffIndex = _buffIndex;
            m_IndicatorIndex = _indicatorIndex;
            m_MuzzleIndex = _muzzleIndex;
        }
    }

    #endregion

    #region GameBase
    public class HealthBase
    {
        public float m_CurrentHealth { get; private set; }
        public float m_BaseHealth { get; private set; }
        public virtual float F_TotalEHP => m_CurrentHealth;
        public float F_HealthBaseScale => m_CurrentHealth / m_BaseHealth;
        public float F_HealthMaxScale => m_CurrentHealth / m_MaxHealth;
        public bool m_HealthFull => m_CurrentHealth >= m_MaxHealth;
        public virtual float m_MaxHealth => m_BaseHealth;
        protected void DamageHealth(float health)
        {
            m_CurrentHealth -= health;
            if (m_CurrentHealth < 0)
                m_CurrentHealth = 0;
            else if (m_HealthFull)
                m_CurrentHealth = m_MaxHealth;
        }

        protected Action<enum_HealthChangeMessage> OnHealthChanged;
        public HealthBase(Action<enum_HealthChangeMessage> _OnHealthChanged)
        {
            OnHealthChanged = _OnHealthChanged;
        }
        public void OnActivate(float startHealth)
        {
            this.m_BaseHealth = startHealth;
            m_CurrentHealth = m_MaxHealth;
        }
        public void OnSetHealth(float reviveHealth)=> m_CurrentHealth = reviveHealth;
        public virtual float OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1, float healEnhance = 1)
        {
            float amountApply = 0;
            if (damageInfo.m_DamageType == enum_DamageType.Armor)
                return amountApply;

            if (amountApply < 0)
            {
                amountApply = amountApply * healEnhance;
                DamageHealth(amountApply);
                OnHealthChanged?.Invoke(enum_HealthChangeMessage.ReceiveHealth);
            }
            else if(amountApply>0)
            {
                amountApply = amountApply * damageReduction;
                DamageHealth(amountApply);
                OnHealthChanged?.Invoke(enum_HealthChangeMessage.DamageHealth);
            }
            return amountApply;
        }
    }

    public class EntityHealth : HealthBase
    {
        public float m_CurrentArmor { get; private set; }
        public float m_BaseArmor { get; private set; }
        public override float F_TotalEHP => m_CurrentArmor + base.F_TotalEHP;
        float m_HealthMultiplier = 1f;
        public override float m_MaxHealth => base.m_MaxHealth * m_HealthMultiplier;
        public virtual float m_MaxArmor => m_BaseArmor;
        public float F_ArmorMaxScale => m_CurrentArmor / m_MaxArmor;
        public bool m_ArmorFull => m_CurrentArmor >= m_MaxArmor;
        protected EntityCharacterBase m_Entity;
        protected void DamageArmor(float amount)
        {
            m_CurrentArmor -= amount;
            if (m_CurrentArmor < 0)
                m_CurrentArmor = 0;
            if (m_CurrentArmor > m_MaxArmor)
                m_CurrentArmor = m_MaxArmor;
        }
        protected void OnSetArmor(float amount) => m_CurrentArmor = amount;

        public EntityHealth(EntityCharacterBase entity, Action<enum_HealthChangeMessage> _OnHealthChanged) : base(_OnHealthChanged)
        {
            m_Entity = entity;
            m_HealthMultiplier = 1f;
        }
        public virtual void OnActivate(float baseHealth, float startArmor,float startHealth)
        {
            base.OnActivate(baseHealth);
            OnSetHealth(startHealth);
            m_BaseArmor = startArmor;
            OnSetArmor(startArmor);
            m_HealthMultiplier = 1f;
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        public void OnSetStatus(float setHealth, float setArmor)
        {
            base.OnSetHealth(setHealth);
            m_CurrentArmor = setArmor;
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }

        public void OnHealthMultiplierChange(float healthMultiplier)
        {
            m_HealthMultiplier = healthMultiplier;
            OnActivate(base.m_BaseHealth);
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }

        public override float OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1, float healEnhance = 1)
        {
            float amountApply = damageInfo.GetAmountApply();
            if (amountApply == 0)
                return amountApply;
            enum_HealthChangeMessage message = enum_HealthChangeMessage.Invalid;
            if (damageInfo.m_TrueDamage)
            {
                damageReduction = 1;
                healEnhance = 1;
            }

            if (amountApply > 0)    //Damage
            {
                if (damageReduction <= 0)
                    return 0;
                amountApply *= damageReduction;
                switch (damageInfo.m_DamageType)
                {
                    case enum_DamageType.Basic:
                        {
                            float healthDamage = amountApply - m_CurrentArmor;
                            DamageArmor(amountApply);
                            if (healthDamage > 0)
                                DamageHealth(healthDamage);
                            message = healthDamage >= 0 ? enum_HealthChangeMessage.DamageHealth : enum_HealthChangeMessage.DamageArmor;
                        }
                        break;
                    case enum_DamageType.Armor:
                        {
                            if (m_CurrentArmor <= 0)
                                return 0;
                            DamageArmor(amountApply);
                            message = enum_HealthChangeMessage.DamageArmor;
                        }
                        break;
                    case enum_DamageType.Health:
                        {
                            DamageHealth(amountApply);
                            message = enum_HealthChangeMessage.DamageHealth;
                        }
                        break;
                    default:
                        Debug.LogError("Error! Invalid Type:" + damageInfo.m_DamageType.ToString());
                        break;
                }
            }
            else if (amountApply < 0)    //Healing
            {
                switch (damageInfo.m_DamageType)
                {
                    case enum_DamageType.Basic:
                        {
                            amountApply *= healEnhance;
                            float armorReceive = amountApply - m_CurrentHealth + m_MaxHealth;
                            DamageHealth(amountApply);
                            if (armorReceive > 0)
                            {
                                message = enum_HealthChangeMessage.ReceiveHealth;
                                break;
                            }

                            DamageArmor(armorReceive);
                            message = enum_HealthChangeMessage.ReceiveArmor;
                        }
                        break;
                    case enum_DamageType.Armor:
                        {
                            if (m_ArmorFull)
                                break;
                            DamageArmor(amountApply);
                            message = enum_HealthChangeMessage.ReceiveArmor;
                        }
                        break;
                    case enum_DamageType.Health:
                        {
                            amountApply *= healEnhance;
                            if (m_HealthFull || amountApply > 0)
                                break;
                            DamageHealth(amountApply);
                            message = enum_HealthChangeMessage.ReceiveHealth;
                        }
                        break;
                    default:
                        Debug.LogError("Error! Invalid Healing Type:" + damageInfo.m_DamageType.ToString());
                        break;
                }
            }
            if (message != enum_HealthChangeMessage.Invalid)
                OnHealthChanged(message);

            return amountApply;
        }

    }

    public class EntityPlayerHealth:EntityHealth
    {
        public float m_MaxHealthAdditive { get; private set; }
        public float m_MaxArmorAdditive { get; private set; }
        public override float m_MaxHealth => base.m_MaxHealth + m_MaxHealthAdditive;
        public override float m_MaxArmor => base.m_MaxArmor + m_MaxArmorAdditive;
        public EntityPlayerHealth(EntityCharacterBase entity, Action<enum_HealthChangeMessage> _OnHealthChanged) : base(entity,_OnHealthChanged)
        {
        }
        public override void OnActivate(float baseHealth, float startArmor, float startHealth)
        {
            base.OnActivate(baseHealth, startArmor, startHealth);
            m_MaxHealthAdditive = 0;
            m_MaxArmorAdditive = 0;
        }
        public void OnMaxChange(float maxHealthAdd,float maxArmorAdd)
        {
            bool changed = false;
            if (maxHealthAdd != m_MaxHealthAdditive)
            {
                float maxHealthDelta = m_MaxHealthAdditive - maxHealthAdd;
                OnSetHealth(m_CurrentHealth-maxHealthDelta);
                m_MaxHealthAdditive = maxHealthAdd;
                changed = true;
            }

            if (maxArmorAdd != m_MaxArmorAdditive)
            {
                float maxArmorDelta = m_MaxArmorAdditive - maxArmorAdd;
                OnSetArmor(m_CurrentArmor - maxArmorDelta);
                m_MaxArmorAdditive = maxArmorAdd;
                changed = true;
            }

            if(changed)
                OnHealthChanged(enum_HealthChangeMessage.Default);
        }
    }

    public class DamageInfo
    {
        public int m_EntityID { get; private set; } = -1;
        public enum_DamageIdentity m_IdentityType { get; private set; } = enum_DamageIdentity.Invalid;
        public int m_IdentityID { get; private set; } = -1;
        
        public float m_DamageBase { get; private set; } = 0;
        public float m_DamageMultiply { get; private set; } = 0;
        public float m_DamageCriticalMultipy { get; private set; } = 0;
        public enum_DamageType m_DamageType { get; private set; } = enum_DamageType.Invalid;
        public bool m_TrueDamage { get; private set; } = false;

        public List<SBuff> m_BaseBuffApply { get; private set; } = new List<SBuff>();

        public DamageInfo(int entityID, enum_DamageIdentity identity= enum_DamageIdentity.Default, int identityID=-1)
        {
            m_EntityID = entityID;
            m_IdentityType = identity;
            m_IdentityID = identityID;
        }

        public DamageInfo SetDamage(float damage, enum_DamageType type = enum_DamageType.Basic, bool trueDamage = false)
        {
            m_DamageBase = damage;
            m_DamageType = type;
            m_TrueDamage = trueDamage;
            return this;
        }

        public DamageInfo SetDamageMultiply(float _damageMultiply)
        {
            m_DamageMultiply = _damageMultiply;
            return this;
        }

        public DamageInfo SetDamageCritical(float _criticalHitRate, float _criticalHitMultiply)
        {
            m_DamageCriticalMultipy = _criticalHitRate > TCommon.RandomLength(1f) ? _criticalHitMultiply : 0;
            return this;
        }

        public DamageInfo AddPresetBuff(int presetBuffID)
        {
            if(presetBuffID>0)
            m_BaseBuffApply.Add(GameDataManager.GetPresetBuff(presetBuffID));
            return this;
        }
        public DamageInfo AddBuffInfo(SBuff buff)
        {
            m_BaseBuffApply.Add(buff);
            return this;
        }

        public DamageInfo AddExtraDamage(float damageMultiply)
        {
            m_DamageMultiply += damageMultiply;
            return this;
        }

        public float GetAmountApply() => m_DamageBase * (1 + m_DamageMultiply + m_DamageCriticalMultipy);
        public bool m_CritcalHitted => m_DamageCriticalMultipy != 0;
        public bool m_IsDamage => m_DamageBase > 0;
        public bool m_IsHealing => m_DamageBase < 0;
    }
    #endregion

    #region Entity Expire Manager
    public class CharacterExpireManager
    {
        protected EntityCharacterBase m_Entity { get; private set; }
        public List<EntityExpireBase> m_Expires { get; private set; } = new List<EntityExpireBase>();
        protected Dictionary<int, SFXEffect> m_BuffEffects { get; private set; } = new Dictionary<int, SFXEffect>();
        public float m_DamageReceiveMultiply { get; private set; } = 1f;
        public float m_HealReceiveMultiply { get; private set; } = 1f;
        public float m_MovementSpeedMultiply { get; private set; } = 1f;
        public float m_FireRateMultiply { get; private set; } = 1f;
        public float m_ReloadRateMultiply { get; private set; } = 1f;
        public float m_DamageMultiply { get; private set; } = 0f;
        public float m_DamageAdditive { get; private set; } = 0f;
        public float m_CriticalDamageMultiply { get; private set; } = 1f;
        public float m_CriticalRateAdditive { get; private set; } = 0f;

        public float DoFireRateTick(float deltaTime) => deltaTime * m_FireRateMultiply;
        public float DoReloadRateTick(float deltaTime) => deltaTime * m_ReloadRateMultiply;

        public float GetCharacterMovementSpeed() => m_Entity.GetBaseMovementSpeed() * m_MovementSpeedMultiply;
        public float GetCharacterCritcalRate() => m_Entity.GetBaseCriticalRate() + m_CriticalRateAdditive;

        public float m_ExtraFireRateMultiply => m_FireRateMultiply - 1;
        public float m_ExtraCriticalHitMultiply => m_CriticalDamageMultiply - 1;
        public float m_ExtraMovemendSpeedMultiply => m_MovementSpeedMultiply-1f;

        public DamageInfo GetDamageInfo(float baseDamage,float criticalAdditive=0, int buff = -1, enum_DamageType damageType = enum_DamageType.Basic,enum_DamageIdentity identityType= enum_DamageIdentity.Default,int identityID=-1)
        {
            DamageInfo info = new DamageInfo(m_Entity.m_EntityID, identityType, identityID).SetDamage(baseDamage + m_DamageAdditive, damageType).SetDamageMultiply(m_DamageMultiply).SetDamageCritical(criticalAdditive+GetCharacterCritcalRate(), m_CriticalDamageMultiply).AddPresetBuff(buff);
            m_Expires.Traversal((EntityExpireBase interact) => { interact.OnAttackSetDamage(info); });
            return info;
        } 

        Action OnExpireInfoChange;
        bool b_expireUpdated = false;
        public void UpdateEntityInfo() => b_expireUpdated = false;

        public CharacterExpireManager(EntityCharacterBase _attacher, Action _OnExpireChange)
        {
            m_Entity = _attacher;
            OnExpireInfoChange = _OnExpireChange;
        }

        public virtual void OnDead()
        {
            m_Expires.Traversal((EntityExpireBase expire) =>
            {
                switch (expire.m_ExpireType)
                {
                    default: Debug.LogError("Invalid Convertions Here!"); return;
                    case enum_ExpireType.Perk:
                        break;
                    case enum_ExpireType.PresetBuff:
                    case enum_ExpireType.EnermyElite:
                        RemoveExpire(expire);
                        break;
                }
            }, true);
            UpdateExpireInfo();
            UpdateExpireEffect();

        }
        public virtual void OnRecycle()
        {
            m_Expires.Clear();
            UpdateExpireInfo();
            UpdateExpireEffect();
        } 


        public virtual void AddExpire(EntityExpireBase expire)
        {
            switch (expire.m_ExpireType)
            {
                default:Debug.LogError("Invalid Convertions Here!");break;
                case enum_ExpireType.Perk:
                case enum_ExpireType.EnermyElite:
                    m_Expires.Add(expire.OnActivate(m_Entity));
                    break;
                case enum_ExpireType.PresetBuff:
                    EntityExpirePreset buff = expire as EntityExpirePreset;
                    switch (buff.m_RefreshType)
                    {
                        default:
                            Debug.LogError("Invalid Convertions Here!");
                            break;
                        case enum_ExpireRefreshType.AddUp:
                            m_Expires.Add(expire.OnActivate(m_Entity));
                            break;
                        case enum_ExpireRefreshType.Refresh:
                            {
                                EntityExpirePreset buffRefresh = m_Expires.Find(p => p.m_Index == buff.m_Index) as EntityExpirePreset;
                                if (buffRefresh != null)
                                    buffRefresh.RefreshTimer();
                                else
                                    m_Expires.Add(expire.OnActivate(m_Entity));
                            }
                            break;
                    }
                    break;
            }
            UpdateEntityInfo();
        }

        protected virtual void RemoveExpire(EntityExpireBase expire)
        {
            m_Expires.Remove(expire);
            UpdateEntityInfo();
        }

        public virtual void Tick(float deltaTime)
        {
            m_Expires.Traversal((EntityExpireBase expire) => {
                expire.OnTick(deltaTime);
                if (expire.m_Expired)
                    RemoveExpire(expire);
            }, true);

            if (b_expireUpdated)
                return;
            b_expireUpdated = true;
            UpdateExpireInfo();
            UpdateExpireEffect();
            OnExpireInfoChange?.Invoke();
        }

        public void OnWillDealtDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity) => m_Expires.Traversal((EntityExpireBase interact) => { interact.OnBeforeDealtDamage(damageEntity, damageInfo); });
        public void OnDealtDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity, float applyAmount) => m_Expires.Traversal((EntityExpireBase interact) => { interact.OnDealtDamage(damageEntity, damageInfo, applyAmount); });
        public void OnKillEnermy(EntityCharacterBase damageEntity) => m_Expires.Traversal((EntityExpireBase interact) => { });
        public void OnWillReceiveDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity) => m_Expires.Traversal((EntityExpireBase interact) => { interact.OnBeforeReceiveDamage(damageInfo); });
        public void OnAfterReceiveDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply) => m_Expires.Traversal((EntityExpireBase interact) => { interact.OnReceiveDamage(damageInfo, amountApply); });
        public void OnReceiveHealing(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply) => m_Expires.Traversal((EntityExpireBase interact) => { interact.OnReceiveHealing(damageInfo, amountApply); });
        #region ExpireInfo
        void UpdateExpireInfo()
        {
            OnResetInfo();
            m_Expires.Traversal(OnSetExpireInfo);
            AfterInfoSet();
        }
        protected virtual void OnResetInfo()
        {
            m_DamageReceiveMultiply = 1f;
            m_HealReceiveMultiply = 1f;
            m_MovementSpeedMultiply = 1f;
            m_FireRateMultiply = 1f;
            m_ReloadRateMultiply = 1f;
            m_DamageMultiply = 0f;
            m_DamageAdditive = 0f;
            m_CriticalRateAdditive = 0f;
            m_CriticalDamageMultiply = 1f;
        }
        protected virtual void OnSetExpireInfo(EntityExpireBase expire)
        {
            m_DamageReceiveMultiply -= expire.m_DamageReduction;
            m_HealReceiveMultiply += expire.m_HealAdditive;
            m_MovementSpeedMultiply += expire.m_MovementSpeedMultiply;
            m_FireRateMultiply += expire.m_FireRateMultiply;
            m_ReloadRateMultiply += expire.m_ReloadRateMultiply;
            m_DamageMultiply += expire.m_DamageMultiply;
            m_DamageAdditive += expire.m_DamageAdditive;
            m_CriticalRateAdditive += expire.m_CriticalRateAdditive;
            m_CriticalDamageMultiply += expire.m_CriticalHitMultiplyAdditive;
        }
        public  void OnAccelerate(float movementSpeedMultiply)
        {
            m_MovementSpeedMultiply += movementSpeedMultiply;
        }
        protected virtual void AfterInfoSet()
        {
            if (m_DamageReceiveMultiply < 0) m_DamageReceiveMultiply = 0;
            if (m_MovementSpeedMultiply < 0) m_MovementSpeedMultiply = 0;
            if (m_HealReceiveMultiply < 0) m_HealReceiveMultiply = 0;
            if (m_CriticalDamageMultiply < 0) m_CriticalDamageMultiply = 0;
            if (m_CriticalRateAdditive < 0) m_CriticalRateAdditive = 0;
        }
        #endregion

        void UpdateExpireEffect()
        {
            m_Expires.Traversal((EntityExpireBase expire) =>
            {
                if (expire.m_EffectIndex <= 0)
                    return;
                if (m_BuffEffects.ContainsKey(expire.m_EffectIndex))
                    return;
                SFXEffect effect = GameObjectManager.SpawnBuffEffect(expire.m_EffectIndex);
                effect.Play(m_Entity);
                m_BuffEffects.Add(expire.m_EffectIndex, effect);
            });

            m_BuffEffects.Traversal((int effectIndex) => {
                if (m_Expires.Any(p => p.m_EffectIndex == effectIndex))
                    return;

                m_BuffEffects[effectIndex].Stop();
                m_BuffEffects.Remove(effectIndex);
            },true);
        }
    }

    public class PlayerExpireManager : CharacterExpireManager
    {
        public float F_MaxHealthAdditive { get; private set; } = 0;
        public float F_MaxArmorAdditive { get; private set; } = 0;

        public float F_AimSpreadMultiply { get; private set; } = 1f;
        public float F_AimMovementStrictMultiply { get; private set; } = 1f;
        public int I_ClipAdditive { get; private set; } = 0;
        public float F_ClipMultiply { get; private set; } = 1f;
        public float F_Projectile_SpeedMuiltiply { get; private set; } = 1f;
        public bool B_Projectile_Penetrate { get; private set; } = false;
        public int I_Projectile_Multi_PelletsAdditive { get; private set; } = 0;
        public float F_Projectile_Store_TickMultiply { get; private set; } = 1f;
        public float F_Cast_Melee_SizeMultiply { get; private set; } = 1f;
        public int CheckClipAmount(int baseClipAmount) => baseClipAmount == 0 ? 0 : (int)((baseClipAmount + I_ClipAdditive) * F_ClipMultiply);
        public float DoStoreRateTick(float deltaTime) => deltaTime * F_Projectile_Store_TickMultiply;

        public List<ExpirePlayerBase> m_ExpireInteracts { get; private set; } = new List<ExpirePlayerBase>();
        public List<ExpirePlayerBase> m_SkillAcceleration =new List<ExpirePlayerBase> ();
        public Dictionary<int, ExpirePlayerPerkBase> m_ExpirePerks { get; private set; } = new Dictionary<int, ExpirePlayerPerkBase>();
        public float m_Coins { get; private set; } = 0;
        public int m_Keys { get; private set; } = 0;
        public ExpRankBase m_RankManager { get; private set; }

        public PlayerExpireManager(EntityCharacterPlayer _attacher ,Action _OnExpireChange) : base(_attacher, _OnExpireChange)
        {
            m_RankManager = new ExpRankBase(GameExpression.GetPlayerRankUpExp);
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            UpdateEntityInfo();
        }

        #region Interact
        public void SetInfoData(CBattleSave _battleSave)
        {
            m_Coins = 0;
            m_Keys = _battleSave.m_Keys;
            m_RankManager.OnExpSet(_battleSave.m_TotalExp);
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, this);

            _battleSave.m_Perks.Traversal((PerkSaveData perkData) => { AddExpire(GameDataManager.CreatePlayerPerk(perkData)); });
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
        }
        public bool CheckRevive() => m_ExpirePerks.Any(p => p.Value.OnCheckRevive());
        public void OnAbilityTrigger() => m_ExpireInteracts.Traversal((ExpirePlayerBase expire) => { expire.OnAbilityTrigger(); });
        public void OnKilledEnermy(DamageInfo info, EntityCharacterBase target)
        {
            m_ExpireInteracts.Traversal((ExpirePlayerBase expire) => { expire.OnKillEnermy(info,target); });
            OnExpReceived(GameConst.I_PlayerEnermyKillExpGain);
        }
        public void OnKilledEnermys(DamageInfo info, EntityCharacterBase target)
        {
            if (info.m_IdentityType == enum_DamageIdentity.PlayerAbility && BattleManager.Instance.m_LocalPlayer.m_Character == enum_PlayerCharacter.Vampire)
            {
                Debug.Log("加速加速");
                if (m_SkillAcceleration.Count == 0)
                {
                    ExpirePlayerPerkBase expire = GameDataManager.CreatePlayerPerk(PerkSaveData.New(10099));
                    base.AddExpire(expire);
                    m_SkillAcceleration.Add(expire as ExpirePlayerBase);
                }

                m_SkillAcceleration.Traversal((ExpirePlayerBase expire) => { expire.OnKillEnermy(info, target); });
            }
        }

        public void OnActionPerkAcquire(int perkID)
        {
            if (m_ExpirePerks.ContainsKey(perkID))
                m_ExpirePerks[perkID].OnStackUp();
            else
                AddExpire(GameDataManager.CreatePlayerPerk(PerkSaveData.New(perkID)));
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
        }
        public override void OnRecycle()
        {
            base.OnRecycle();
            m_ExpirePerks.Clear();
            m_ExpireInteracts.Clear();
        }
        #endregion
        #region Expire Update
        public override void AddExpire(EntityExpireBase expire)
        {
            base.AddExpire(expire);
            if (expire.m_ExpireType.IsInteractExpire())
                m_ExpireInteracts.Add(expire as ExpirePlayerBase);

            switch (expire.m_ExpireType)
            {
                case enum_ExpireType.Perk:
                    {
                        ExpirePlayerPerkBase targetExpire = expire as ExpirePlayerPerkBase;
                        m_ExpirePerks.Add(targetExpire.m_Index, targetExpire);
                        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
                    }
                    break;
            }
        }

        protected override void RemoveExpire(EntityExpireBase expire)
        {
            base.RemoveExpire(expire);
            if(expire.m_ExpireType.IsInteractExpire())
                m_ExpireInteracts.Remove(expire as ExpirePlayerBase);

            switch (expire.m_ExpireType)
            {
                case enum_ExpireType.Perk:
                    {
                        m_ExpirePerks.Remove(expire.m_Index);
                        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
                    }
                    break;
            }
        }
        #endregion
        #region Data Update
        protected override void OnResetInfo()
        {
            base.OnResetInfo();
            I_ClipAdditive = 0;
            F_ClipMultiply = 1f;
            F_AimSpreadMultiply = 1f;
            B_Projectile_Penetrate = false;
            F_Projectile_SpeedMuiltiply = 1f;
            F_AimMovementStrictMultiply = 1f;
            F_MaxHealthAdditive = 0f;
            F_MaxArmorAdditive = 0;
            I_Projectile_Multi_PelletsAdditive = 0;
            F_Projectile_Store_TickMultiply = 1f;
            F_Cast_Melee_SizeMultiply = 1f;
        }
        protected override void OnSetExpireInfo(EntityExpireBase expire)
        {
            base.OnSetExpireInfo(expire);
            switch(expire.m_ExpireType)
            {
                default:  Debug.LogError("Invalid Convertions Here!");  break;
                case enum_ExpireType.PresetBuff:break;
                case enum_ExpireType.Perk:
                    {
                        ExpirePlayerPerkBase perk = expire as ExpirePlayerPerkBase;
                        F_AimSpreadMultiply -= perk.F_SpreadReduction;
                        F_AimMovementStrictMultiply -= perk.F_AimPressureReduction;

                        I_ClipAdditive += perk.I_ClipAdditive;
                        F_ClipMultiply += perk.F_ClipMultiply;

                        F_MaxHealthAdditive += perk.m_MaxHealthAdditive;
                        F_MaxArmorAdditive += perk.m_MaxArmorAdditive;
                        F_Projectile_SpeedMuiltiply += perk.F_Projectile_SpeedMultiply;
                        B_Projectile_Penetrate |= perk.B_Projectile_Penetrate;
                        I_Projectile_Multi_PelletsAdditive += perk.I_Projectile_Multi_PelletsAdditive;
                        F_Projectile_Store_TickMultiply += perk.F_Projectile_Store_TickMultiply;
                        F_Cast_Melee_SizeMultiply += perk.F_Cast_Melee_SizeMultiply;
                    }
                    break;
            }
        }
        protected override void AfterInfoSet()
        {
            base.AfterInfoSet();
            if (F_AimMovementStrictMultiply < 0) F_AimMovementStrictMultiply = 0;
            if (F_AimSpreadMultiply < 0) F_AimSpreadMultiply = 0;
            if (F_Projectile_Store_TickMultiply < .1f) F_Projectile_Store_TickMultiply = .1f;
            if (F_Cast_Melee_SizeMultiply < .1f) F_Cast_Melee_SizeMultiply = .1f;
        }
        #endregion
        public void RefreshEffects() => m_BuffEffects.Traversal((int expire, SFXEffect effect) => { effect.Play(m_Entity); });
        #region Coin/Key Info
        public bool CanCostCoins(float price)=> m_Coins >= price;
        public void OnCoinsCost(float price)
        {
            m_Coins -= price;
            BattleUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().OnDeductMoney(price);
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCurrencyUpdate,this);
        }
        public void OnCoinsGain(float coinAmount)
        {
            GameDataManager.m_getGoldCoins += (int)coinAmount;
            Debug.Log("拾取了金币"+ coinAmount);
            m_Coins += coinAmount;
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, this);
        } 

        public bool CanCostKeys(int keyCount) => m_Keys >=keyCount;
        public void OnKeyCost(int keyAmount)
        {
            m_Keys--;
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, this);
        }
        public void OnKeysGain(int keyAmount)
        {
            m_Keys += keyAmount;
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, this);
        }

        public void OnExpReceived(int exp)
        {
            m_RankManager.OnExpGainCheckLevelOffset(exp);
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, this);
        }
        #endregion
    }
    
    #endregion

    #region ExpireBases
    public class EntityExpireBase
    {
        public virtual int m_EffectIndex => -1;
        public virtual int m_Index => -1;
        public virtual enum_ExpireType m_ExpireType => enum_ExpireType.Invalid;
        public virtual enum_ExpireRefreshType m_RefreshType => enum_ExpireRefreshType.Invalid;
        public virtual float m_MovementSpeedMultiply => 0;
        public virtual float m_FireRateMultiply => 0;
        public virtual float m_ReloadRateMultiply => 0;
        public virtual float m_DamageMultiply => 0;
        public virtual float m_DamageAdditive => 0;
        public virtual float m_DamageReduction => 0;
        public virtual float m_CriticalRateAdditive => 0;
        public virtual float m_CriticalHitMultiplyAdditive => 0f;
        public virtual float m_HealAdditive => 0;

        public bool m_Expired { get; protected set; } = false;

        public EntityCharacterBase m_Attacher { get; private set; }
        public virtual EntityExpireBase OnActivate(EntityCharacterBase _actionEntity) { m_Attacher = _actionEntity;return this; }
        public virtual void OnTick(float deltaTime) { }
        public virtual void OnAttackSetDamage(DamageInfo info) { }
        public virtual void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info) { }
        public virtual void OnBeforeReceiveDamage(DamageInfo info) { }
        public virtual void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount) { }
        public virtual void OnReceiveDamage(DamageInfo info, float amount) { }
        public virtual void OnReceiveHealing(DamageInfo info, float applyAmount) { }
    }

    public class EntityExpirePreset : EntityExpireBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.PresetBuff;
        public override int m_Index => m_buffInfo.m_Index;
        public override int m_EffectIndex => m_buffInfo.m_EffectIndex;
        public override enum_ExpireRefreshType m_RefreshType => m_buffInfo.m_RefreshType;
        public override float m_DamageMultiply => m_buffInfo.m_DamageMultiply;
        public override float m_DamageReduction => m_buffInfo.m_DamageReduction;
        public override float m_FireRateMultiply => m_buffInfo.m_FireRateMultiply;
        public override float m_MovementSpeedMultiply => m_buffInfo.m_MovementSpeedMultiply;
        public override float m_ReloadRateMultiply => m_buffInfo.m_ReloadRateMultiply;
        public int I_SourceID { get; private set; }
        SBuff m_buffInfo;
        TimerBase m_DotTimer;
        TimerBase m_ExpireTimer;
        public EntityExpirePreset(int sourceID, SBuff _buffInfo)
        {
            I_SourceID = sourceID;
            m_buffInfo = _buffInfo;
            m_DotTimer = new TimerBase(_buffInfo.m_DotTick);
            m_ExpireTimer = new TimerBase(_buffInfo.m_ExpireDuration);
        }

        public void RefreshTimer() => m_ExpireTimer.Replay();

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            if (!m_ExpireTimer.m_Timing)
                return;
            m_ExpireTimer.Tick(deltaTime);
            if (!m_ExpireTimer.m_Timing)
            {
                m_Expired = true;
                return;
            }
            m_DotTimer.Tick(deltaTime);
            if (m_DotTimer.m_Timing)
                return;
            m_DotTimer.Replay();
            m_Attacher.m_HitCheck.TryHit(new DamageInfo(I_SourceID, enum_DamageIdentity.Expire).SetDamage(m_buffInfo.m_DotPercentage * m_Attacher.m_Health.m_MaxHealth, m_buffInfo.m_DotType));
        }
    }
    
    public class ExpirePlayerBase: EntityExpireBase
    {
        public new EntityCharacterPlayer m_Attacher { get; private set; }

        public override EntityExpireBase OnActivate(EntityCharacterBase _actionEntity)
        {
            if (_actionEntity.m_ControllType != enum_EntityType.Player)
                Debug.LogError("Invalid Expire Type Attached!");
            m_Attacher = _actionEntity as EntityCharacterPlayer;
            return base.OnActivate(_actionEntity);
        }
        public virtual float m_MaxHealthAdditive => 0;
        public virtual float m_MaxArmorAdditive => 0;

        public virtual float Value1 => 0;
        public virtual float Value2 => 0;
        public virtual float Value3 => 0;

        public virtual bool OnCheckRevive() { return false; }
        public virtual void OnAbilityTrigger() { }
        public virtual void OnKillEnermy(DamageInfo info, EntityCharacterBase target) { }
    }

    public class ExpirePlayerPerkBase: ExpirePlayerBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.Perk;
        public virtual enum_Rarity m_Rarity { get; private set; } = enum_Rarity.Invalid;
        public virtual int m_MaxStack=>-1;
        public virtual bool m_DataHidden => false;

        public int m_Stack { get; private set; } = 0;
        public virtual float m_RecordData { get; protected set; }
        public ExpirePlayerPerkBase(PerkSaveData data) { m_Stack = data.m_PerkStack; m_RecordData = data.m_RecordData; }
        
        public void OnStackUp() => m_Stack++;

        public virtual float F_AimPressureReduction => 0;
        public virtual float F_SpreadReduction => 0;
        public virtual int I_ClipAdditive => 0;
        public virtual float F_ClipMultiply => 0;
        public virtual float F_Projectile_SpeedMultiply => 0;
        public virtual bool B_Projectile_Penetrate => false;
        public virtual int I_Projectile_Multi_PelletsAdditive => 0;
        public virtual float F_Projectile_Store_TickMultiply => 0f;
        public virtual float F_Cast_Melee_SizeMultiply => 0f;
    }

    public class ExpireBattleCharacterBase:EntityExpireBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.EnermyElite;
        public virtual float m_MaxHealthMultiplierAdditive => m_GameBaseMaxHealthMultiplier;
        public override float m_DamageMultiply => m_GameBaseMaxHealthMultiplier;

        protected float m_GameBaseMaxHealthMultiplier = 0;
        protected float m_GameBaseDamageMultiplier = 0;

        public ExpireBattleCharacterBase(float baseMaxHealthMultiplier,float baseDamageMultiplier)
        {
            m_GameBaseDamageMultiplier = baseDamageMultiplier;
            m_GameBaseMaxHealthMultiplier = baseMaxHealthMultiplier;
        }
    }
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
    #endregion
    
    #region GameEffects
    public class ModelBlink:ICoroutineHelperClass
    {
        Transform transform;
        Renderer[] m_renderers;
        MaterialPropertyBlock m_block=new MaterialPropertyBlock();
        float f_simulate;
        float f_blinkRate; 
        float f_blinkTime;
        Color c_blinkColor;
        public ModelBlink(Transform BlinkModel, float _blinkRate, float _blinkTime,Color _blinkColor)
        {
            if (BlinkModel == null)
                Debug.LogError("Error! Blink Model Init, BlinkModel Folder Required!");

            transform = BlinkModel;
            m_renderers = BlinkModel.GetComponentsInChildren<Renderer>();
            f_blinkRate = _blinkRate;
            f_blinkTime = _blinkTime;
            c_blinkColor = _blinkColor;
            f_simulate = 0f;
            OnReset();
        }
        public void SetShow(bool show) => transform.SetActivate(true);
        public void OnReset()
        {
            f_simulate = 0f;
            this.StopSingleCoroutine(0);
            m_block.SetColor("_Color", TCommon.ColorAlpha(c_blinkColor, 0));
            m_renderers.Traversal((Renderer render) => { render.SetPropertyBlock(m_block); });
            SetShow(false);
        }
        
        public void Tick(float deltaTime)
        {
            f_simulate += deltaTime;
            if (f_simulate > f_blinkRate)
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
                    m_block.SetColor("_Color", TCommon.ColorAlpha(c_blinkColor, value));
                    m_renderers.Traversal((Renderer render) => { render.SetPropertyBlock(m_block); });
                }, 1, 0, f_blinkTime));
                f_simulate -= f_blinkRate;
            }
        }
    }
    #endregion

    #region WeaponHelper
    public class CharacterWeaponHelperBase
    {
        public virtual bool B_TargetAlly => false;
        public int I_Index { get; private set; } = -1;
        public virtual bool B_LoopAnim => false;
        protected Transform attacherHead => m_Entity.tf_Head;
        protected EntityCharacterBase m_Entity { get; private set; }
        protected float m_Spread { get; private set; }
        public CharacterWeaponHelperBase(int weaponIndex, EntityCharacterBase _controller,float spread)
        {
            I_Index = weaponIndex;
            m_Entity = _controller;
            m_Spread = spread;
        }
        protected virtual Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target) => _target.tf_Head.position;
        public void OnPlay(bool _preAim, EntityCharacterBase _target,DamageInfo info) => OnPlay(_target, GetTargetPosition(_preAim, _target), info);
        public virtual void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition,DamageInfo info)
        {

        }

        public virtual void OnStopPlay()
        {

        }
        public static CharacterWeaponHelperBase AcquireCharacterWeaponHelper(int weaponIndex, EntityCharacterBase _entity,float spread)
        {
            SFXDamageBase weaponInfo = GameObjectManager.GetSFXWeaponData<SFXDamageBase>(weaponIndex);
            SFXProjectile projectile = weaponInfo as SFXProjectile;
            if (projectile)
            {
                switch (projectile.E_ProjectileType)
                {
                    default: Debug.LogError("Invalid Type:" + projectile.E_ProjectileType); break;
                    case enum_ProjectileFireType.Single: return new CharacterWeaponHelperBarrageRange(weaponIndex, projectile, _entity, spread);
                    case enum_ProjectileFireType.MultipleFan: return new CharacterWeaponHelperBarrageMultipleFan(weaponIndex, projectile, _entity, spread);
                    case enum_ProjectileFireType.MultipleLine: return new CharacterWeaponHelperBarrageMultipleLine(weaponIndex, projectile, _entity, spread);
                }
            }

            SFXCast cast = weaponInfo as SFXCast;
            if (cast)
            {
                switch (cast.E_CastType)
                {
                    default: Debug.LogError("Invalid Type:" + cast.E_CastType); break;
                    case enum_CastControllType.CastFromOrigin: return new CharacterWeaponHelperCaster(weaponIndex, cast, _entity, spread);
                    case enum_CastControllType.CastControlledForward: return new CharacterWeaponHelperCasterControlled(weaponIndex, cast, _entity, spread);
                    case enum_CastControllType.CastAtTarget: return new CharacterWeaponHelperCasterTarget(weaponIndex, cast, _entity, spread);
                }
            }

            SFXDamageApply buffApply = weaponInfo as SFXDamageApply;
            if (buffApply)
                return new CharacterWeaponHelperDamageApply(weaponIndex, buffApply, _entity, spread);

            SFXSubEntitySpawner entitySpawner = weaponInfo as SFXSubEntitySpawner;
            if (entitySpawner)
                return new CharacterWeaponHelperEntitySpawner(weaponIndex, entitySpawner, _entity, spread);

            return null;
        }
    }

    public class CharacterWeaponHelperCaster : CharacterWeaponHelperBase
    {
        protected enum_CastTarget m_CastAt { get; private set; }
        protected bool m_castForward { get; private set; }
        public CharacterWeaponHelperCaster(int weaponIndex, SFXCast _castInfo, EntityCharacterBase _controller, float spread) : base(weaponIndex, _controller,spread)
        {
            m_CastAt = _castInfo.E_CastTarget;
            m_castForward = _castInfo.B_CastForward;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition, DamageInfo info)
        {
            Transform castAt = GetCastAt(m_Entity);
            GameObjectManager.SpawnSFXWeapon<SFXCast>(I_Index, castAt.position, m_castForward ? castAt.forward : Vector3.up).Play(info);
        }
        protected Transform GetCastAt(EntityCharacterBase character)
        {
            switch (m_CastAt)
            {
                default:
                    Debug.LogError("Invalid Phrase Here");
                    return null;
                case enum_CastTarget.Feet:
                    return character.transform;
                case enum_CastTarget.Head:
                    return character.tf_Head;
                case enum_CastTarget.Weapon:
                    return character.tf_Weapon;
            }
        }
    }


    public class CharacterWeaponHelperCasterTarget : CharacterWeaponHelperCaster
    {
        public CharacterWeaponHelperCasterTarget(int weaponIndex, SFXCast _castInfo, EntityCharacterBase _controller,float spread) : base(weaponIndex, _castInfo, _controller,spread)
        {
        }
        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target)
        {
            Transform castAt = GetCastAt(_target);
            Vector3 castPos = NavigationManager.NavMeshPosition(castAt.position + TCommon.RandomXZSphere() * m_Spread);
            castPos.y = castAt.transform.position.y;
            return castPos;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition, DamageInfo info)
        {
            GameObjectManager.SpawnSFXWeapon<SFXCast>(I_Index, _calculatedPosition, m_castForward ? m_Entity.tf_Weapon.forward : Vector3.up).Play(info);
        }
    }

    public class CharacterWeaponHelperCasterControlled : CharacterWeaponHelperCaster
    {
        public override bool B_LoopAnim => true;
        SFXCast m_Cast;
        public CharacterWeaponHelperCasterControlled(int weaponIndex, SFXCast _castInfo, EntityCharacterBase _controller, float spread) : base(weaponIndex, _castInfo, _controller,spread)
        {
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition, DamageInfo info)
        {
            if (!m_Cast)
            {
                m_Cast = GameObjectManager.SpawnSFXWeapon<SFXCast>(I_Index, m_Entity.tf_Weapon.position, m_Entity.tf_Weapon.forward);
                m_Cast.PlayControlled(m_Entity.m_EntityID, m_Entity, attacherHead);
            }

            if (m_Cast)
                m_Cast.ControlledCheck(info);
        }

        public override void OnStopPlay()
        {
            base.OnStopPlay();
            if (m_Cast)
            {
                m_Cast.StopControlled();
                m_Cast = null;
            }
        }
    }
    public class CharacterWeaponHelperBarrageRange : CharacterWeaponHelperBase
    {
        protected float f_projectileSpeed { get; private set; }
        protected RangeInt m_CountExtension { get; private set; }
        protected float m_OffsetExtension { get; private set; }
        int i_muzzleIndex;
        AudioClip m_MuzzleClip;

        public CharacterWeaponHelperBarrageRange(int weaponIndex, SFXProjectile projectileInfo, EntityCharacterBase _controller, float spread) : base(weaponIndex, _controller,spread)
        {
            i_muzzleIndex = projectileInfo.I_MuzzleIndex;
            m_MuzzleClip = projectileInfo.AC_MuzzleClip;
            f_projectileSpeed = projectileInfo.F_Speed;
            m_CountExtension = projectileInfo.RI_CountExtension;
            m_OffsetExtension = projectileInfo.F_OffsetExtension;
        }

        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition, DamageInfo info)
        {
            Vector3 startPosition = m_Entity.tf_Weapon.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            SpawnMuzzle(startPosition, direction);
            FireBullet(startPosition, direction, _calculatedPosition,info);
        }

        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target)
        {
            preAim = preAim && f_projectileSpeed > 10f;     //Case Aim Some Shit Positions

            float startDistance = TCommon.GetXZDistance(m_Entity.tf_Weapon.position, _target.tf_Head.position);
            Vector3 targetPosition = preAim ? _target.m_PrecalculatedTargetPos(startDistance / f_projectileSpeed) : _target.tf_Head.position;

            if (preAim && Mathf.Abs(TCommon.GetAngle(m_Entity.tf_Weapon.forward, TCommon.GetXZLookDirection(m_Entity.tf_Weapon.position, targetPosition), Vector3.up)) > 90)    //Target Positioned Back, Return Target
                targetPosition = _target.tf_Head.position;

            if (TCommon.GetXZDistance(m_Entity.tf_Weapon.position, targetPosition) > m_Spread)      //Target Outside Spread Sphere,Add Spread
                targetPosition += TCommon.RandomXZSphere() * m_Spread;
            return targetPosition;
        }

        protected void FireBullet(Vector3 startPosition, Vector3 direction, Vector3 targetPosition,DamageInfo info)
        {
            GameObjectManager.SpawnSFXWeapon<SFXProjectile>(I_Index, startPosition, direction).Play(info, direction, targetPosition);
        }
        protected void SpawnMuzzle(Vector3 startPosition, Vector3 direction) => GameObjectManager.PlayMuzzle(m_Entity.m_EntityID, startPosition, direction, i_muzzleIndex, m_MuzzleClip);
    }
    public class CharacterWeaponHelperBarrageMultipleLine : CharacterWeaponHelperBarrageRange
    {
        public CharacterWeaponHelperBarrageMultipleLine(int weaponIndex, SFXProjectile projectileInfo, EntityCharacterBase _controller, float spread) : base(weaponIndex, projectileInfo, _controller,spread)
        {
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition, DamageInfo info)
        {
            Vector3 startPosition = m_Entity.tf_Weapon.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            int waveCount = m_CountExtension.Random();
            float distance = TCommon.GetXZDistance(startPosition, _calculatedPosition);
            Vector3 lineBeginPosition = startPosition - attacherHead.right * m_OffsetExtension * ((waveCount - 1) / 2f);
            SpawnMuzzle(startPosition, direction);
            for (int i = 0; i < waveCount; i++)
                FireBullet(lineBeginPosition + attacherHead.right * m_OffsetExtension * i, direction, m_Entity.tf_Weapon.position + direction * distance,info);
        }
    }

    public class CharacterWeaponHelperBarrageMultipleFan : CharacterWeaponHelperBarrageRange
    {
        public CharacterWeaponHelperBarrageMultipleFan(int weaponIndex, SFXProjectile projectileInfo, EntityCharacterBase _controller, float spread) : base(weaponIndex, projectileInfo, _controller,spread)
        {
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition, DamageInfo info)
        {
            Vector3 startPosition = m_Entity.tf_Weapon.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            int waveCount = m_CountExtension.Random();
            float beginAnle = -m_OffsetExtension * (waveCount - 1) / 2f;
            float distance = TCommon.GetXZDistance(m_Entity.tf_Weapon.position, _calculatedPosition);
            SpawnMuzzle(startPosition, direction);
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 fanDirection = direction.RotateDirectionClockwise(Vector3.up, beginAnle + i * m_OffsetExtension);
                FireBullet(m_Entity.tf_Weapon.position, fanDirection, m_Entity.tf_Weapon.position + fanDirection * distance,info);
            }
        }
    }

    public class CharacterWeaponHelperDamageApply : CharacterWeaponHelperBase
    {
        public override bool B_TargetAlly => true;
        public CharacterWeaponHelperDamageApply(int weaponIndex, SFXDamageApply buffApplyinfo, EntityCharacterBase _controller, float spread) : base(weaponIndex, _controller,spread)
        {
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition, DamageInfo info)
        {
            GameObjectManager.SpawnSFXWeapon<SFXDamageApply>(I_Index, m_Entity.tf_Weapon.position, Vector3.up).Play(m_Entity.m_EntityID, info, _target);
        }

    }
    public class CharacterWeaponHelperEntitySpawner : CharacterWeaponHelperBase
    {
        bool m_SpawnAtTarget;
        public CharacterWeaponHelperEntitySpawner(int weaponIndex, SFXSubEntitySpawner spawner, EntityCharacterBase _controller, float spread) : base(weaponIndex, _controller,spread)
        {
            m_SpawnAtTarget = spawner.B_SpawnAtTarget;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition, DamageInfo info)
        {
            Vector3 spawnPosition = (m_SpawnAtTarget ? _target.transform.position : m_Entity.transform.position) + TCommon.RandomXZSphere() * m_Spread;
            GameObjectManager.SpawnSFXWeapon<SFXSubEntitySpawner>(I_Index, spawnPosition, Vector3.up).Play(m_Entity, _target.transform.position);
        }
    }
    #endregion
    #endregion
}
