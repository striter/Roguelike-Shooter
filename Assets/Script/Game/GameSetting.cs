using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using LevelSetting;
namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
        #region Battle
        public static readonly RangeInt RI_EnermyGenerateDuration = new RangeInt(10, 30);
        public const float F_EnermyGenerateTickMultiplierPerMinute =.04f;
        public const float F_EnermyGenerateTickMultiplierTransmiting = 2f;

        public const float F_EnermyEliteGenerateBase = 10f;
        public const float F_EnermyEliteGeneratePerMinuteMultiplier = 0.5f;
        public const float F_SqrEnermyGenerateMinDistance = 400f; // 20*20

        public const float F_SignalTowerTransmitDuration = 60f;

        public const float F_BlastShakeMultiply = .5f;
        public const float F_DamageImpactMultiply = 1f;

        public const float F_EntityDeadFadeTime = 3f;
        public const float F_PlayerReviveCheckAfterDead = 1.5f;
        public const float F_PlayerReviveBuffDuration = 6f; //复活无敌时间

        public const float F_AimMovementReduction = .6f;
        public const float F_MovementReductionDuration = .1f;
        public const int I_ProjectileMaxDistance = 100;
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
        #endregion
        #region Interacts
        public static readonly RangeInt RI_GameEventGenerate = new RangeInt(16, 4);

        public static readonly RangeInt RI_EnermyCoinsGenerate = new RangeInt(1,3);
        public const float F_EnermyKeyGenerate = 2.5f;

        public const float F_PickupMaxSpeed = 100f;
        public const float F_PickupAcceleration = 50f; //拾取物的飞行加速速度
        public const int I_HealthPickupAmount = 25;
        public const int I_ArmorPickupAmount = 25;
        public const int I_HealthPackAmount = 50;

        public const int I_DangerzoneDamage = 50;
        public const float F_DangerzoneResetDuration = 2f;

        public static Dictionary<enum_GameEventType, float> D_GameEventRate = new Dictionary<enum_GameEventType, float>() { {enum_GameEventType.CoinsSack,10f }, { enum_GameEventType.HealthpackTrade, 10f }, { enum_GameEventType.WeaponTrade, 15f }, { enum_GameEventType.WeaponReforge, 5f }, { enum_GameEventType.WeaponVendor, 10f }, { enum_GameEventType.WeaponRecycle, 2.5f }, { enum_GameEventType.PerkLottery, 15f }, { enum_GameEventType.PerkSelect, 10f }, { enum_GameEventType.PerkShrine, 10f }, { enum_GameEventType.BloodShrine, 5f }, { enum_GameEventType.HealShrine, 5f }, { enum_GameEventType.SafeBox, 2.5f }, };

        public static RangeInt RI_CoinsSackAmount = new RangeInt(3, 9);
        public const int I_EventMedpackPrice = 25;

        public const int I_EventWeaponTradePrice = 15;
        public static Dictionary<enum_Rarity, float> D_EventWeaponTradeRate = new Dictionary<enum_Rarity, float>() { { enum_Rarity.Ordinary, 30 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 25 },{ enum_Rarity.Epic, 15 } };

        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponReforgeRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 25 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 25 } };
        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponRecyclePrice = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 20 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 50 }, { enum_Rarity.Epic, 75 } };

        public const int I_EventWeaponVendorMachinePrice = 10;
        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponVendorMachineRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 35 }, { enum_Rarity.Advanced, 50 }, { enum_Rarity.Rare, 10 }, { enum_Rarity.Epic, 5 } };
        public const int I_EventWeaponVendotTryCount = 5;
        public const int I_EventPerkLotteryPrice = 15;
        public static readonly Dictionary<enum_Rarity, int> D_EventPerkLotteryRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 40 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 20 }, { enum_Rarity.Epic, 10 } };
        public const int I_EventPerkSelectPrice = 15;
        public static readonly Dictionary<enum_Rarity, int> D_EventPerkSelectRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 40 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 20 }, { enum_Rarity.Epic, 10 } };
        public const int I_PerkShrineTryCountMax = 5;
        public static readonly Dictionary<enum_Rarity, int> D_PerkShrineRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 20 }, { enum_Rarity.Advanced, 10 }, { enum_Rarity.Rare, 5 }, { enum_Rarity.Epic, 3 } };
        public const int I_BloodShrineTryCountMax = 5;
        public const int I_BloodShrineCoinsRate = 50;
        public const int I_BloodShrineCoinsAmount = 15;
        public const int I_HealShrineTryCountMax = 5;
        public const float F_HealShrineHealthReceive = 30f;

        public static readonly RangeInt I_EventSafeCoinsAmount = new RangeInt(10,10);
        public static readonly RangeInt RI_EventSafeWeaponCount = new RangeInt(1, 1);
        public static readonly Dictionary<enum_Rarity, int> D_EventSafeWeaponRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 40 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 10 } };
        public static readonly RangeInt RI_EventSafePerkCount = new RangeInt(1, 1);
        public static readonly Dictionary<enum_Rarity, int> D_EventSafePerkRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 40 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 10 } };

        #endregion        
        public static class AI
        {
            public const float F_AIMovementCheckParam = .3f; //AI检查玩家频率
            public const float F_AITargetCheckParam = .5f;      //AI Target Duration .5f is Suggested
            public const float F_AIReTargetCheckParam = 3f;       //AI Retarget Duration,3f is suggested
            public const float F_AITargetCalculationParam = .5f;       //AI Target Param Calculation Duration, 1 is suggested;
            public const float F_AIMaxRepositionDuration = .5f;
            public const float F_AIDeadImpactPerDamageValue = 0.1f;   //0.05f;
            public const int I_AIBattleIdlePercentage = 50;
            public static readonly RangeFloat RF_AIBattleIdleDuration = new RangeFloat(1f, 2f);
        }
        #region Cultivate
        public static readonly Dictionary<enum_Rarity, float> m_ArmoryBlueprintGameDropRarities = new Dictionary<enum_Rarity, float>() { { enum_Rarity.Ordinary, 10f }, { enum_Rarity.Advanced, 5f }, { enum_Rarity.Rare, 3f }, { enum_Rarity.Epic, 2f } };
        public static readonly Dictionary<enum_Rarity, float> m_ArmoryBlueprintUnlockPrice = new Dictionary<enum_Rarity, float>() { { enum_Rarity.Ordinary, 1000 }, { enum_Rarity.Advanced, 1500f }, { enum_Rarity.Rare, 3000f }, { enum_Rarity.Epic, 5000f } };
        
        #region Equipment
        public static readonly Dictionary<enum_Rarity, float> m_EquipmentGameDropRarities = new Dictionary<enum_Rarity, float>() { { enum_Rarity.Ordinary, 10f }, { enum_Rarity.Advanced, 5f }, { enum_Rarity.Rare, 3f }, { enum_Rarity.Epic, 2f } };
        public const int m_EquipmentEnhanceMaxLevel = 15;
        public const float m_EquipmentEnhanceCoinsCostMultiply = .2f;
        public static readonly Dictionary<enum_Rarity, Dictionary<int, int>> m_EquipmentGenerateEntryCount = new Dictionary<enum_Rarity, Dictionary<int, int>>()  {{ enum_Rarity.Ordinary, new Dictionary<int, int>(){ { 0,50},{ 1,50} } } ,{ enum_Rarity.Advanced, new Dictionary<int, int>(){ { 1,50},{ 2,50} } } ,{ enum_Rarity.Rare, new Dictionary<int, int>(){ { 1,35},{ 2,35},{ 3,30} } } , { enum_Rarity.Epic, new Dictionary<int, int>(){ { 2,50},{ 3,50} } } ,  };

        public static readonly Dictionary<enum_CharacterUpgradeType, Dictionary<enum_Rarity, float>> m_EquipmentEntryStart_Main = new Dictionary<enum_CharacterUpgradeType, Dictionary<enum_Rarity, float>>() {
            { enum_CharacterUpgradeType.Health,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,50},{ enum_Rarity.Advanced,70},{ enum_Rarity.Rare,90},{ enum_Rarity.Epic,110} } },
            { enum_CharacterUpgradeType.Armor,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,30},{ enum_Rarity.Advanced,50},{ enum_Rarity.Rare, 70},{ enum_Rarity.Epic,90} } },
            { enum_CharacterUpgradeType.MovementSpeed,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,10},{ enum_Rarity.Advanced,12},{ enum_Rarity.Rare, 14},{ enum_Rarity.Epic,16} } },
            { enum_CharacterUpgradeType.CriticalRate,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,5},{ enum_Rarity.Advanced,10},{ enum_Rarity.Rare, 15},{ enum_Rarity.Epic,20} } },
            { enum_CharacterUpgradeType.FireRate,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,10},{ enum_Rarity.Advanced,20},{ enum_Rarity.Rare, 30},{ enum_Rarity.Epic,40} } },
            { enum_CharacterUpgradeType.Damage,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,10},{ enum_Rarity.Advanced,20},{ enum_Rarity.Rare, 30},{ enum_Rarity.Epic,40} } } };

        public static readonly Dictionary<enum_CharacterUpgradeType, Dictionary<enum_Rarity, float>> m_EquipmentEntryUpgrade_Main = new Dictionary<enum_CharacterUpgradeType, Dictionary<enum_Rarity, float>>() {
            { enum_CharacterUpgradeType.Health,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,10},{ enum_Rarity.Advanced,14},{ enum_Rarity.Rare, 18},{ enum_Rarity.Epic,22} } },
            { enum_CharacterUpgradeType.Armor,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,10},{ enum_Rarity.Advanced,14},{ enum_Rarity.Rare, 18},{ enum_Rarity.Epic,22} } },
            { enum_CharacterUpgradeType.MovementSpeed,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,2},{ enum_Rarity.Advanced,3},{ enum_Rarity.Rare, 4},{ enum_Rarity.Epic,5} } },
            { enum_CharacterUpgradeType.CriticalRate,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,1},{ enum_Rarity.Advanced,2},{ enum_Rarity.Rare, 3},{ enum_Rarity.Epic,4} } },
            { enum_CharacterUpgradeType.FireRate,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,1},{ enum_Rarity.Advanced,1.2f},{ enum_Rarity.Rare, 1.4f},{ enum_Rarity.Epic,1.6f} } },
            { enum_CharacterUpgradeType.Damage,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,2},{ enum_Rarity.Advanced,4},{ enum_Rarity.Rare, 5},{ enum_Rarity.Epic,8} } } };

        public static readonly Dictionary<enum_CharacterUpgradeType, Dictionary<enum_Rarity, RangeFloat>> m_EquipmentEntryStart_Sub = new Dictionary<enum_CharacterUpgradeType, Dictionary<enum_Rarity, RangeFloat>>() {
            { enum_CharacterUpgradeType.Health,new Dictionary<enum_Rarity, RangeFloat>(){ { enum_Rarity.Ordinary,new RangeFloat(25,25)},{ enum_Rarity.Advanced, new RangeFloat(35,35)},{ enum_Rarity.Rare, new RangeFloat(45,45)},{ enum_Rarity.Epic, new RangeFloat(55,55)} } },
            { enum_CharacterUpgradeType.Armor,new Dictionary<enum_Rarity, RangeFloat>(){ { enum_Rarity.Ordinary, new RangeFloat(15,15)},{ enum_Rarity.Advanced, new RangeFloat(25,25)},{ enum_Rarity.Rare, new RangeFloat(35,35)},{ enum_Rarity.Epic, new RangeFloat(45,45)} } },
            { enum_CharacterUpgradeType.MovementSpeed,new Dictionary<enum_Rarity, RangeFloat>(){ { enum_Rarity.Ordinary, new RangeFloat(5,5)},{ enum_Rarity.Advanced, new RangeFloat(6,6)},{ enum_Rarity.Rare, new RangeFloat(7,7)},{ enum_Rarity.Epic, new RangeFloat(8,8)} } },
            { enum_CharacterUpgradeType.CriticalRate,new Dictionary<enum_Rarity, RangeFloat>(){ { enum_Rarity.Ordinary, new RangeFloat(2.5f,2.5f)},{ enum_Rarity.Advanced, new RangeFloat(5,5)},{ enum_Rarity.Rare, new RangeFloat(7.5f,7.5f)},{ enum_Rarity.Epic, new RangeFloat(10,10)} } },
            { enum_CharacterUpgradeType.FireRate,new Dictionary<enum_Rarity, RangeFloat>(){ { enum_Rarity.Ordinary, new RangeFloat(5,5)},{ enum_Rarity.Advanced, new RangeFloat(10,10)},{ enum_Rarity.Rare, new RangeFloat(15,15)},{ enum_Rarity.Epic, new RangeFloat(20,20)} } },
            { enum_CharacterUpgradeType.Damage,new Dictionary<enum_Rarity, RangeFloat>(){ { enum_Rarity.Ordinary, new RangeFloat(5,5)},{ enum_Rarity.Advanced, new RangeFloat(10,10)},{ enum_Rarity.Rare, new RangeFloat(15,15)},{ enum_Rarity.Epic, new RangeFloat(20, 20) } } } };
        
        public static readonly Dictionary<enum_CharacterUpgradeType, Dictionary<enum_Rarity, float>> m_EquipmentEntryUpgrade_Sub = new Dictionary<enum_CharacterUpgradeType, Dictionary<enum_Rarity, float>>() {
            { enum_CharacterUpgradeType.Health,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,5},{ enum_Rarity.Advanced,7},{ enum_Rarity.Rare, 9},{ enum_Rarity.Epic,11} } },
            { enum_CharacterUpgradeType.Armor,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,5},{ enum_Rarity.Advanced,7},{ enum_Rarity.Rare, 9},{ enum_Rarity.Epic,11} } },
            { enum_CharacterUpgradeType.MovementSpeed,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,2},{ enum_Rarity.Advanced,3},{ enum_Rarity.Rare, 4},{ enum_Rarity.Epic,5} } },
            { enum_CharacterUpgradeType.CriticalRate,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,1},{ enum_Rarity.Advanced,2},{ enum_Rarity.Rare, 3},{ enum_Rarity.Epic,4} } },
            { enum_CharacterUpgradeType.FireRate,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,1},{ enum_Rarity.Advanced,1.2f},{ enum_Rarity.Rare, 1.4f},{ enum_Rarity.Epic,1.6f} } },
            { enum_CharacterUpgradeType.Damage,new Dictionary<enum_Rarity, float>(){ { enum_Rarity.Ordinary,2},{ enum_Rarity.Advanced,4},{ enum_Rarity.Rare, 5},{ enum_Rarity.Epic,8} } } };
        #endregion

        public const int m_MaxCharacterUpgradeTime = 5;

        public static readonly Dictionary<enum_CharacterUpgradeType, int> m_UpgradeValueEachTime = new Dictionary<enum_CharacterUpgradeType, int>() { { enum_CharacterUpgradeType.Armor, 30 }, { enum_CharacterUpgradeType.Health, 30 }, { enum_CharacterUpgradeType.FireRate, 8 }, { enum_CharacterUpgradeType.MovementSpeed, 5 }, { enum_CharacterUpgradeType.CriticalRate,5 },{ enum_CharacterUpgradeType.Damage,8} };
        #endregion
    }

    public static class GameExpression
    {
        public static int GetPlayerRankUpExp(int curRank) => 50 + curRank * 50;
        public static int GetPlayerWeaponIndex(int weaponIndex) =>weaponIndex * 10;
        public static int GetPlayerExtraWeaponIndex(int weaponIndex) => weaponIndex * 10+5;
        public static int GetPlayerPerkSFXWeaponIndex(int equipmentIndex) => 100000 + equipmentIndex * 10;
        public static int GetAIWeaponIndex(int entityIndex, int weaponIndex = 0, int subWeaponIndex = 0) => entityIndex * 100 + weaponIndex * 10 + subWeaponIndex;
        public static int GetWeaponSubIndex(int weaponIndex) => weaponIndex + 1;
        
        public static float F_GameVFXVolume(int vfxVolumeTap) => vfxVolumeTap / 10f;
        public static float F_GameMusicVolume(int musicVolumeTap) => musicVolumeTap / 10f;

        public static bool B_ShowHitMark(enum_HitCheck check) => check != enum_HitCheck.Invalid;

        public static float GetEnermyMaxHealthMultiplier(int minutePassed, enum_GameDifficulty difficulty) => minutePassed * .2f + ((int)difficulty - 1) * .25f;
        public static float GetEnermyDamageMultilier(int minutesPassed, enum_GameDifficulty difficulty) => minutesPassed * .1f + ((int)difficulty - 1) * .25f;

        public static float GetResultCompletion(bool win, enum_GameStage _stage, int _battleLevelEntered) => win ? 1f : (.33f * ((int)_stage - 1) +.066f*_battleLevelEntered);
        public static float GetResultLevelScore(enum_GameStage _stage, int _levelPassed) => 200 * ((int)_stage - 1) + 20 * (_levelPassed - 1);
        public static float GetResultDifficultyBonus(enum_GameDifficulty _difficulty) =>1f+ (int)_difficulty * .05f;
        public static float GetResultRewardCredits(float _totalScore) => _totalScore;
        #region Interacts

        public static int GetPerkShrinePrice(int tryCount) =>5+ 5 * tryCount;
        public static int GetBloodShrinePrice(int tryCount) => 5 + 3 * tryCount;
        public static float GetBloodShrineHealthCostMultiple(int count) => .1f + .05f * count;
        public static int GetHealShrinePrice(int tryCount) => 5 + 2 * tryCount;
        public static RangeInt GetEventTradePrice(enum_Interaction interactType,enum_Rarity perkRarity= enum_Rarity.Invalid,enum_Rarity weaponRarity= enum_Rarity.Invalid)
        {
            switch (interactType)
            {
                default: Debug.LogError("No Coins Can Phrase Here!"); return new RangeInt(0, -1);
                case enum_Interaction.PickupHealthPack:
                    return new RangeInt(10, 0);
                case enum_Interaction.PerkPickup:
                    switch (perkRarity)
                    {
                        default: Debug.LogError("Invalid Level!"); return new RangeInt(0, -1);
                        case enum_Rarity.Ordinary:return new RangeInt( 10,0);
                        case enum_Rarity.Advanced: return new RangeInt(10, 0);
                        case enum_Rarity.Rare: return new RangeInt(10, 0);
                        case enum_Rarity.Epic: return new RangeInt(10, 0);
                    }
                case enum_Interaction.PickupWeapon:
                    switch (weaponRarity)
                    {
                        default:Debug.LogError("Invalid Weapon Rarity");return new RangeInt(0, -1);
                        case enum_Rarity.Ordinary:
                            return new RangeInt(1, 5);
                        case enum_Rarity.Advanced:
                            return new RangeInt(8, 4);
                        case enum_Rarity.Rare:
                            return new RangeInt(16, 8);
                        case enum_Rarity.Epic:
                            return new RangeInt(24,12);
                    }
            }
        }
        #endregion
        public static float GetLevelObjectHealth(enum_TileObjectType objectType)
        {
            switch(objectType)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return -1;
                case enum_TileObjectType.Dangerzone:
                    return -1;
                case enum_TileObjectType.Static1x1A:
                case enum_TileObjectType.Static1x1B:
                case enum_TileObjectType.Static1x1C:
                case enum_TileObjectType.Static1x1D:
                    return 100;
                case enum_TileObjectType.Static2x1A:
                case enum_TileObjectType.Static2x1B:
                    return 200;
                case enum_TileObjectType.Static2x2A:
                case enum_TileObjectType.Static2x2B:
                    return 400;
                case enum_TileObjectType.Static3x2A:
                case enum_TileObjectType.Static3x2B:
                    return 600;
                case enum_TileObjectType.Static3x3A:
                case enum_TileObjectType.Static3x3B:
                    return 900;
            }
        }
        
        #region Cultivate
        public static int GetEquipmentEnhanceRequirement(enum_Rarity rarity, int level) => (1000 + 500 * (int)rarity) + (500 + (int)rarity * 250) * level;
        public static int GetEquipmentDeconstruct(enum_Rarity rarity, int level) => (500 + 250 * (int)rarity) + (250 + (int)rarity * 125) * level;

        public static int GetCharacterUpgradePrice(enum_CharacterUpgradeType upgrade,int curTime)
        {
            switch (upgrade)
            {
                default:Debug.LogError("Invlaid Convertions Here!");return 0;
                case enum_CharacterUpgradeType.Armor:return 500+curTime * 1000;
                case enum_CharacterUpgradeType.Health: return 500+curTime * 1000;
                case enum_CharacterUpgradeType.MovementSpeed: return 1000+curTime * 1500;
                case enum_CharacterUpgradeType.Damage: return 1000+curTime * 500;
                case enum_CharacterUpgradeType.CriticalRate: return 500+ curTime * 200;
                case enum_CharacterUpgradeType.FireRate: return 500+curTime * 200;
            }
        }
        #endregion
    }

    public static class LocalizationKeyJoint
    {
        public static string GetNameLocalizeKey(this EntityExpirePreset buff) => "Buff_Name_" + buff.m_Index;
        public static string GetNameLocalizeKey(this ExpirePlayerPerkBase action) => "Perk_Name_" + action.m_Index;
        public static string GetIntroLocalizeKey(this ExpirePlayerPerkBase action) => "Perk_Intro_" + action.m_Index;
        public static string GetNameLocalizeKey(this enum_PlayerCharacter character) => "Character_Name_" + character;
        public static string GetIntroLocalizeKey(this enum_PlayerCharacter character) => "Character_Intro_" + character;
        public static string GetAbilityLocalizeKey(this enum_PlayerCharacter character) => "Character_Ability_" + character;
        public static string GetNameLocalizeKey(this EquipmentSaveData equipment) => "Equipment_Name_" + equipment.m_Index;
        public static string GetPassiveLocalizeKey(this EquipmentSaveData upgrade) => "Equipment_Passive_" + upgrade.m_Index;
        public static string GetPassiveLocalizeKey(this ExpirePlayerUpgradeCombine upgrade) => "Equipment_Passive_" + upgrade.m_Index;
        public static string GetLocalizeKey(this EquipmentEntrySaveData entry) => "Equipment_Entry_" + entry.m_Type;
        public static string GetLocalizeKey(this enum_GameStage stage) => "Game_Stage_" + stage;
        public static string GetLocalizeKey(this enum_GameStyle style) => "Game_Style_" + style;
        public static string GetLocalizeNameKey(this enum_GamePortalType type) => "UI_Level_" + type + "_Name";
        public static string GetLocalizeIntroKey(this enum_GamePortalType type) => "UI_Level_" + type + "_Intro";
        public static string GetLocalizeNameKey(this enum_PlayerWeapon weapon) => "Weapon_Name_" + weapon;
        public static string GetTitleLocalizeKey(this enum_Interaction interact) => "UI_Interact_" + interact+"_Title";
        public static string GetIntroLocalizeKey(this enum_Interaction interact) => "UI_Interact_" + interact + "_Intro";
        public static string GetLocalizeKey(this enum_Option_FrameRate frameRate) => "UI_Option_" + frameRate;
        public static string GetLocalizeKey(this enum_Option_JoyStickMode joystick) => "UI_Option_" + joystick;
        public static string GetLocalizeKey(this enum_Option_LanguageRegion region) => "UI_Option_" + region;
        public static string SetActionIntro(this ExpirePlayerPerkBase actionInfo, UIT_TextExtend text) => text.formatText(actionInfo.GetIntroLocalizeKey() , actionInfo.Value1, actionInfo.Value2, actionInfo.Value3);
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
        public virtual bool OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1, float healEnhance = 1)
        {
            if (damageInfo.m_DamageType == enum_DamageType.Armor)
                return false;

            if (damageInfo.m_AmountApply < 0)
            {
                DamageHealth(damageInfo.m_AmountApply * healEnhance);
                OnHealthChanged?.Invoke(enum_HealthChangeMessage.ReceiveHealth);
            }
            else
            {
                DamageHealth(damageInfo.m_AmountApply * damageReduction);
                OnHealthChanged?.Invoke(enum_HealthChangeMessage.DamageHealth);
            }
            
            return true;
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
        public void OnActivate(float baseHealth, float startArmor,float startHealth)
        {
            base.OnActivate(baseHealth);
            OnSetHealth(startHealth);
            m_BaseArmor = startArmor;
            OnSetArmor(startArmor);
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

        public override bool OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1, float healEnhance = 1)
        {
            TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterHealthWillChange, damageInfo, m_Entity);
            
            if(damageInfo.m_DamageType== enum_DamageType.HealthPenetrate)
            {
                damageReduction = 1;
                healEnhance = 1;
            }

            float basicAmount = damageInfo.m_AmountApply;
            if (damageInfo.m_AmountApply > 0)    //Damage
            {
                if (damageReduction <= 0)
                    return false;
                basicAmount *= damageReduction;
                switch (damageInfo.m_DamageType)
                {
                    case enum_DamageType.Basic:
                        {
                            float healthDamage = basicAmount - m_CurrentArmor;
                            DamageArmor(basicAmount);
                            if (healthDamage > 0)
                                DamageHealth(healthDamage);
                            OnHealthChanged(healthDamage >= 0 ? enum_HealthChangeMessage.DamageHealth : enum_HealthChangeMessage.DamageArmor);
                        }
                        break;
                    case enum_DamageType.Armor:
                        {
                            if (m_CurrentArmor <= 0)
                                return false;
                            DamageArmor(basicAmount);
                            OnHealthChanged(enum_HealthChangeMessage.DamageArmor);
                        }
                        break;
                    case enum_DamageType.HealthPenetrate:
                    case enum_DamageType.Health:
                        {
                            DamageHealth(basicAmount);
                            OnHealthChanged(enum_HealthChangeMessage.DamageHealth);
                        }
                        break;
                    default:
                        Debug.LogError("Error! Invalid Type:" + damageInfo.m_DamageType.ToString());
                        break;
                }
            }
            else if (damageInfo.m_AmountApply < 0)    //Healing
            {
                switch (damageInfo.m_DamageType)
                {
                    case enum_DamageType.Basic:
                        {
                            basicAmount *= healEnhance;
                            float armorReceive = basicAmount - m_CurrentHealth + m_MaxHealth;
                            DamageHealth(basicAmount);
                            if (armorReceive > 0)
                            {
                                OnHealthChanged(enum_HealthChangeMessage.ReceiveHealth);
                                return true;
                            }

                            DamageArmor(armorReceive);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveArmor);
                        }
                        break;
                    case enum_DamageType.Armor:
                        {
                            if (m_ArmorFull)
                                break;
                            DamageArmor(basicAmount);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveArmor);
                        }
                        break;
                    case enum_DamageType.HealthPenetrate:
                    case enum_DamageType.Health:
                        {
                            basicAmount *= healEnhance;
                            if (m_HealthFull || basicAmount > 0)
                                break;
                            DamageHealth(basicAmount);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveHealth);
                        }
                        break;
                    default:
                        Debug.LogError("Error! Invalid Healing Type:" + damageInfo.m_DamageType.ToString());
                        break;
                }
            }
            if (basicAmount != 0)
                TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterHealthChange, damageInfo, m_Entity, basicAmount);
            return true;
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
        public void OnMaxChange(float maxHealthAdd,float maxArmorAdd)
        {
            bool changed = false;
            if (maxHealthAdd != m_MaxHealthAdditive)
            {
                m_MaxHealthAdditive = maxHealthAdd;
                if (m_CurrentHealth < m_MaxHealth)
                    OnSetHealth(m_MaxHealth);
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
        public static int m_TotalDamageCount = 0;
        public int m_DamageIdentity { get; private set; } = m_TotalDamageCount++;
        
        public int m_SourceID { get; private set; } = -1;
        public float m_DamageBase { get; private set; } = 0;
        public float m_DamageMultiply { get; private set; } = 0;
        public float m_DamageCriticalMultipy { get; private set; } = 0;
        public enum_DamageType m_DamageType { get; private set; } = enum_DamageType.Invalid;

        public List<SBuff> m_BaseBuffApply { get; private set; } = new List<SBuff>();
        public void AddExtraBuff(int presetBuffID) => m_BaseBuffApply.Add(GameDataManager.GetPresetBuff(presetBuffID));

        public void AddExtraDamage(float damageMultiply, float damageAdditive)
        {
            m_DamageMultiply += damageMultiply;
            damageAdditive += damageAdditive;
        }

        public DamageInfo(int sourceID, float damage,float _damageMultiply,float _criticalHitRate, float _critcalHitMultiply, enum_DamageType type,int extraBuffID)
        {
            m_SourceID = sourceID;
            m_DamageBase = damage;
            m_DamageMultiply = _damageMultiply;
            m_DamageCriticalMultipy =   _criticalHitRate> TCommon.RandomLength(1f) ? _critcalHitMultiply:0;
            m_DamageType = type;
            if(extraBuffID>0)
                AddExtraBuff(extraBuffID);
        }

        public DamageInfo(int sourceID,float damage,enum_DamageType type)
        {
            m_SourceID = sourceID;
            m_DamageBase = damage;
            m_DamageType = type;
        }

        public DamageInfo(int sourceID, int buffInfo)
        {
            m_SourceID = sourceID;
            AddExtraBuff(buffInfo);
        }
        public DamageInfo(int sourceID,SBuff buffInfo)
        {
            m_SourceID = sourceID;
            m_BaseBuffApply.Add(buffInfo);
        }

        public float m_AmountApply => m_DamageBase * m_BaseDamageMultiply;
        public float m_BaseDamageMultiply=> (1 + m_DamageMultiply + m_DamageCriticalMultipy);
        public bool m_CritcalHitted => m_DamageCriticalMultipy != 0;
    }
    #endregion

    #region WeaponTriggers
    public class WeaponTrigger
    {
        public bool m_TriggerDown { get; protected set; }
        public virtual void OnSetTrigger(bool down)
        {
            m_TriggerDown = down;
        }

        public virtual void OnTriggerStop()
        {
            m_TriggerDown = false;
        }

        public virtual void Tick(bool paused,float deltaTime)
        {

        }
    }
    public class WeaponTriggerAuto : WeaponTrigger
    {
        protected Func<bool> OnTriggerCheck;
        Action OnTriggerSuccessful;
        TimerBase m_TriggerTimer = new TimerBase();
        public WeaponTriggerAuto(float _fireRate, Func<bool> _OnTriggerCheck,Action _OnTriggerSuccessful)
        {
            OnTriggerCheck = _OnTriggerCheck;
            OnTriggerSuccessful = _OnTriggerSuccessful;
            m_TriggerTimer.SetTimerDuration(_fireRate);
        }
        public override void Tick(bool paused,float deltaTime)
        {
            base.Tick(paused,deltaTime);
            m_TriggerTimer.Tick(deltaTime);
            if (paused|| m_TriggerTimer.m_Timing)
                return;

            if (!m_TriggerDown)
                return;

            if (!OnTriggerCheck())
                return;
            OnTriggerSuccessful();
                m_TriggerTimer.Replay();
        }
    }

    public class WeaponTriggerStoring:WeaponTrigger
    {
         Func<bool> OnStoreBeginCheck;
        Action<bool> OnStoreFinish;
        TimerBase m_StoreTimer = new TimerBase();
        public bool m_Storing { get; private set; } = false;
        public WeaponTriggerStoring(float _storeDuration, Func<bool> _OnStoreBeginCheck, Action<bool> _OnStoreFinish)
        {
            OnStoreBeginCheck = _OnStoreBeginCheck;
            OnStoreFinish = _OnStoreFinish;
            m_StoreTimer.SetTimerDuration(_storeDuration);
            m_Storing = false;
        }

        public override void OnSetTrigger(bool down)
        {
            base.OnSetTrigger(down);
            if (!down || !OnStoreBeginCheck())
                return;

            SetStore(true);
        }

        public override void OnTriggerStop()
        {
            base.OnTriggerStop();
            SetStore(false);
        }

        void SetStore(bool store)
        {
            if (m_Storing == store)
                return;

            m_Storing = store;
            if (m_Storing)
                m_StoreTimer.Replay();
            else
                OnStoreFinish(!m_StoreTimer.m_Timing);
        }

        public override void Tick(bool paused, float deltaTime)
        {
            base.Tick(paused,deltaTime);

            if (!m_Storing)
                return;

            if ( m_TriggerDown)
            {
                m_StoreTimer.Tick(deltaTime);
                return;
            }


            if (paused)
                return;
            SetStore(false);
        }

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
        public float m_CriticalRate { get; private set; } = 0f;
        public float DoFireRateTick(float deltaTime) => deltaTime * m_FireRateMultiply;
        public float DoReloadRateTick(float deltaTime) => deltaTime * m_ReloadRateMultiply;
        public float GetMovementSpeed => m_Entity.m_baseMovementSpeed * m_MovementSpeedMultiply;

        public float m_ExtraFireRateMultiply => m_FireRateMultiply - 1;
        public float m_ExtraCriticalHitMultiply => m_CriticalDamageMultiply - 1;
        public float m_ExtraMovemendSpeedMultiply => m_MovementSpeedMultiply-1f;

        public virtual DamageInfo GetDamageBuffInfo(float baseDamage, int buff = 0, enum_DamageType type = enum_DamageType.Basic)
        {
            DamageInfo info = new DamageInfo(m_Entity.m_EntityID, baseDamage + m_DamageAdditive, m_DamageMultiply, m_CriticalRate, m_CriticalDamageMultiply, type, buff);
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
                    case enum_ExpireType.Upgrades:
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
                case enum_ExpireType.Upgrades:
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
                                    buffRefresh.BuffRefresh();
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

        public virtual void Tick(float deltaTime) {
            m_Expires.Traversal((EntityExpireBase expire) => {
                if (expire.m_Expired)
                {
                    RemoveExpire(expire);
                    return;
                }
                expire.OnTick(deltaTime);
            },true);

            if (b_expireUpdated)
                return;
            b_expireUpdated = true;
            UpdateExpireInfo();
            UpdateExpireEffect();
            OnExpireInfoChange?.Invoke();
        }

        public void OnWillDealtDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity) => m_Expires.Traversal((EntityExpireBase interact) => { interact.OnBeforeDealtDamage(damageEntity, damageInfo); });
        public void OnDealtDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity, float applyAmount) => m_Expires.Traversal((EntityExpireBase interact) => { interact.OnDealtDamage(damageEntity, damageInfo, applyAmount); });
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
            m_CriticalRate = 0f;
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
            m_CriticalRate += expire.m_CriticalRateAdditive;
            m_CriticalDamageMultiply += expire.m_CriticalHitMultiplyAdditive;
        }
        protected virtual void AfterInfoSet()
        {
            if (m_DamageReceiveMultiply < 0) m_DamageReceiveMultiply = 0;
            if (m_MovementSpeedMultiply < 0) m_MovementSpeedMultiply = 0;
            if (m_HealReceiveMultiply < 0) m_HealReceiveMultiply = 0;
            if (m_CriticalDamageMultiply < 0) m_CriticalDamageMultiply = 0;
            if (m_CriticalRate < 0) m_CriticalRate = 0;
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

        public float F_SpreadMultiply { get; private set; } = 1f;
        public float F_AimMovementStrictMultiply { get; private set; } = 1f;
        public float F_ProjectileSpeedMuiltiply { get; private set; } = 1f;
        public float F_PenetrateAdditive { get; private set; } = 0;
        public float F_AimRangeAdditive { get; private set; } = 0;
        public float F_CoinsCostMultiply { get; private set; } = 0f;
        public int I_ClipAdditive { get; private set; } = 0;
        public float F_ClipMultiply { get; private set; } = 1f;
        public int CheckClipAmount(int baseClipAmount) => baseClipAmount == 0 ? 0 : (int)((baseClipAmount + I_ClipAdditive) * F_ClipMultiply);

        public List<ExpirePlayerBase> m_ExpireInteracts { get; private set; } = new List<ExpirePlayerBase>();
        public Dictionary<int, ExpirePlayerPerkBase> m_ExpirePerks { get; private set; } = new Dictionary<int, ExpirePlayerPerkBase>();
        public ExpirePlayerUpgradeCombine m_Upgrade { get; private set; }
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
        public void SetInfoData(CGameProgressSave _battleSave)
        {
            m_Coins = 0;
            m_Keys = _battleSave.m_Keys;
            m_RankManager.OnExpSet(_battleSave.m_TotalExp);
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, this);

            AddExpire(GameDataManager.CreateUpgradeCombination(_battleSave.m_Equipments, _battleSave.m_Upgrade));
            _battleSave.m_Perks.Traversal((PerkSaveData perkData) => { AddExpire(GameDataManager.CreatePlayerPerk(perkData)); });
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
        }
        public bool CheckRevive() => m_ExpirePerks.Any(p => p.Value.OnCheckRevive());
        public void OnAbilityTrigger() => m_ExpireInteracts.Traversal((ExpirePlayerBase interact) => { interact.OnAbilityTrigger(); });

        public void OnActionPerkAcquire(int perkID)
        {
            if (m_ExpirePerks.ContainsKey(perkID))
                m_ExpirePerks[perkID].OnStackUp();
            else
                AddExpire(GameDataManager.CreatePlayerPerk(PerkSaveData.New(perkID)));
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
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
                case enum_ExpireType.Upgrades:
                    {
                        if (m_Upgrade != null)
                            Debug.LogError("Can't Add Extra Equipment Upgrade!");
                        m_Upgrade = expire as ExpirePlayerUpgradeCombine;
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
                case enum_ExpireType.Upgrades:
                    Debug.LogError("Can't Remove Preset Upgrade!");
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
            F_PenetrateAdditive = 0;
            F_AimRangeAdditive = 0;
            F_SpreadMultiply = 1f;
            F_ProjectileSpeedMuiltiply = 1f;
            F_AimMovementStrictMultiply = 1f;
            F_MaxHealthAdditive = 0f;
            F_MaxArmorAdditive = 0;
            F_CoinsCostMultiply = 1f;
        }
        protected override void OnSetExpireInfo(EntityExpireBase expire)
        {
            base.OnSetExpireInfo(expire);
            switch(expire.m_ExpireType)
            {
                default:  Debug.LogError("Invalid Convertions Here!");  break;
                case enum_ExpireType.PresetBuff:break;
                case enum_ExpireType.Upgrades:
                    {
                        ExpirePlayerUpgradeCombine equipment = expire as ExpirePlayerUpgradeCombine;
                        F_MaxHealthAdditive += equipment.m_MaxHealthAdditive;
                        F_MaxArmorAdditive += equipment.m_MaxArmorAdditive;
                    }
                    break;
                case enum_ExpireType.Perk:
                    {
                        ExpirePlayerPerkBase perk = expire as ExpirePlayerPerkBase;
                        F_SpreadMultiply -= perk.F_SpreadReduction;
                        F_AimMovementStrictMultiply -= perk.F_AimPressureReduction;

                        I_ClipAdditive += perk.I_ClipAdditive;
                        F_ClipMultiply += perk.F_ClipMultiply;

                        F_ProjectileSpeedMuiltiply += perk.F_ProjectileSpeedMultiply;
                        F_PenetrateAdditive += perk.F_PenetradeAdditive;
                        F_AimRangeAdditive += perk.F_AimRangeAdditive;
                        F_MaxHealthAdditive += perk.m_MaxHealthAdditive;
                        F_MaxArmorAdditive += perk.m_MaxArmorAdditive;
                        F_CoinsCostMultiply -= perk.F_Discount;
                    }
                    break;
            }
        }
        protected override void AfterInfoSet()
        {
            base.AfterInfoSet();
            if (F_AimMovementStrictMultiply < 0) F_AimMovementStrictMultiply = 0;
            if (F_SpreadMultiply < 0) F_SpreadMultiply = 0;
            if (F_CoinsCostMultiply < 0) F_CoinsCostMultiply = 0;
        }
        #endregion
        public void RefreshEffects() => m_BuffEffects.Traversal((int expire, SFXEffect effect) => { effect.Play(m_Entity); });
        #region Coin/Key Info
        public bool CanCostCoins(float price)=> m_Coins >= price* F_CoinsCostMultiply;
        public void OnCoinsCost(float price)
        {
            m_Coins -= price * F_CoinsCostMultiply;
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCurrencyUpdate,this);
        }
        public void OnCoinsGain(float coinAmount)
        {
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
            if (m_RankManager.OnExpGainCheckLevelOffset(exp) != 0)
                Debug.Log("Rank Up!" + m_RankManager.m_Rank);
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
        public override int m_EffectIndex => m_buffInfo.m_EffectIndex;
        public override enum_ExpireRefreshType m_RefreshType => m_buffInfo.m_AddType;
        public override float m_DamageMultiply => m_buffInfo.m_DamageMultiply;
        public override float m_DamageReduction => m_buffInfo.m_DamageReduction;
        public override int m_Index => m_buffInfo.m_Index;
        public override float m_FireRateMultiply => m_buffInfo.m_FireRateMultiply;
        public override float m_MovementSpeedMultiply => m_buffInfo.m_MovementSpeedMultiply;
        public override float m_ReloadRateMultiply => m_buffInfo.m_ReloadRateMultiply;
        public float m_ExpireDuration { get; private set; } = 0;
        public float f_expireCheck { get; private set; }
        public float f_expireLeftScale => f_expireCheck / m_ExpireDuration;
        public int I_SourceID { get; private set; }
        SBuff m_buffInfo;
        float f_dotCheck;
        public EntityExpirePreset(int sourceID, SBuff _buffInfo)
        {
            I_SourceID = sourceID;
            m_buffInfo = _buffInfo;
            SetDuration(_buffInfo.m_ExpireDuration);
        }
        protected void SetDuration(float duration)
        {
            m_ExpireDuration = duration;
            BuffRefresh();
        }
        public void BuffRefresh() => f_expireCheck = m_ExpireDuration;
        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            if (m_ExpireDuration <= 0)
                return;
            f_expireCheck -= deltaTime;
            if (f_expireCheck <= 0)
                m_Expired = true;

            if (m_buffInfo.m_DamageTickTime <= 0)
                return;
            f_dotCheck += deltaTime;
            if (f_dotCheck > m_buffInfo.m_DamageTickTime)
            {
                f_dotCheck -= m_buffInfo.m_DamageTickTime;
                m_Attacher.m_HitCheck.TryHit(new DamageInfo(I_SourceID,m_buffInfo.m_DamagePerTick, m_buffInfo.m_DamageType));
            }
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
    }

    public class ExpirePlayerUpgradeCombine: ExpirePlayerBase
    {
        public override enum_ExpireType m_ExpireType =>  enum_ExpireType.Upgrades;
        public bool m_HavePassive => m_Index != GameDataManager.m_DefaultEquipmentCombinationIdentity;
        public List<EquipmentSaveData> m_EquipmentData { get; private set; }
        public CharacterUpgradeData m_CharacterData { get; private set; }
        public Dictionary<enum_CharacterUpgradeType, float> m_UpgradeDatas { get; private set; } = new Dictionary<enum_CharacterUpgradeType, float>();

        public override float m_CriticalRateAdditive => m_UpgradeDatas[ enum_CharacterUpgradeType.CriticalRate]/100f;
        public override float m_MovementSpeedMultiply => m_UpgradeDatas[ enum_CharacterUpgradeType.MovementSpeed]/100f;
        public override float m_FireRateMultiply => m_UpgradeDatas[ enum_CharacterUpgradeType.FireRate]/100f;
        public override float m_MaxArmorAdditive => m_UpgradeDatas[ enum_CharacterUpgradeType.Armor];
        public override float m_MaxHealthAdditive => m_UpgradeDatas[ enum_CharacterUpgradeType.Health];
        public override float m_DamageAdditive => m_UpgradeDatas[ enum_CharacterUpgradeType.Damage];
        public ExpirePlayerUpgradeCombine(List<EquipmentSaveData> equipmentData, CharacterUpgradeData characterUpgrade)
        {
            m_CharacterData = characterUpgrade;
            m_EquipmentData = equipmentData;

            TCommon.TraversalEnum((enum_CharacterUpgradeType upgrade) => { m_UpgradeDatas.Add(upgrade, 0); });
            m_CharacterData.m_Upgrades.Traversal((enum_CharacterUpgradeType upgrade, int amount) => {
                m_UpgradeDatas[upgrade] = GameConst.m_UpgradeValueEachTime[upgrade] * amount;
            });
            m_EquipmentData.Traversal((EquipmentSaveData data) => {
                data.m_Entries.Traversal((EquipmentEntrySaveData entry) =>
                {
                    m_UpgradeDatas[entry.m_Type] += entry.m_Value;
                });
            });
        }
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

        public virtual float F_SpreadReduction => 0;
        public virtual float F_AimPressureReduction => 0;
        public virtual float F_ProjectileSpeedMultiply => 0;
        public virtual bool B_ClipOverride => false;
        public virtual int I_ClipAdditive => 0;
        public virtual float F_ClipMultiply => 0;
        public virtual float F_PenetradeAdditive => 0;
        public virtual float F_AimRangeAdditive => 0;
        public virtual float F_AllyHealthMultiplierAdditive => 0;
        public virtual float F_Discount => 0f;
    }

    public class ExpireEnermyPerkBase:EntityExpireBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.EnermyElite;
        public virtual float m_MaxHealthMultiplierAdditive => m_GameBaseMaxHealthMultiplier;
        public override float m_DamageMultiply => m_GameBaseMaxHealthMultiplier;

        protected float m_GameBaseMaxHealthMultiplier = 0;
        protected float m_GameBaseDamageMultiplier = 0;

        public ExpireEnermyPerkBase(float baseMaxHealthMultiplier,float baseDamageMultiplier)
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
    public class WeaponHelperBase
    {
        public virtual bool B_TargetAlly => false;
        public int I_Index { get; private set; } = -1;
        public virtual bool B_LoopAnim => false;
        protected EntityCharacterBase m_Entity;
        protected Transform attacherHead => m_Entity.tf_Head;
        protected Func<DamageInfo> GetDamageDeliverInfo;
        public WeaponHelperBase(int equipmentIndex, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo)
        {
            I_Index = equipmentIndex;
            m_Entity = _controller;
            GetDamageDeliverInfo = _GetBuffInfo;
        }
        protected virtual Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target) => _target.tf_Head.position;
        public void OnPlay(bool _preAim, EntityCharacterBase _target) => OnPlay(_target, GetTargetPosition(_preAim, _target));
        public virtual void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {

        }

        public virtual void OnStopPlay()
        {

        }
        public static WeaponHelperBase AcquireWeaponHelper(int weaponIndex, EntityCharacterBase _entity, Func<DamageInfo> GetDamageBuffInfo)
        {
            SFXWeaponBase weaponInfo = GameObjectManager.GetSFXWeaponData<SFXWeaponBase>(weaponIndex);
            SFXProjectile projectile = weaponInfo as SFXProjectile;
            if (projectile)
            {
                switch (projectile.E_ProjectileType)
                {
                    default: Debug.LogError("Invalid Type:" + projectile.E_ProjectileType); break;
                    case enum_ProjectileFireType.Single: return new WeaponHelperBarrageRange(weaponIndex, projectile, _entity, GetDamageBuffInfo);
                    case enum_ProjectileFireType.MultipleFan: return new WeaponHelperBarrageMultipleFan(weaponIndex, projectile, _entity, GetDamageBuffInfo);
                    case enum_ProjectileFireType.MultipleLine: return new WeaponHelperBarrageMultipleLine(weaponIndex, projectile, _entity, GetDamageBuffInfo);
                }
            }

            SFXCast cast = weaponInfo as SFXCast;
            if (cast)
            {
                switch (cast.E_CastType)
                {
                    default: Debug.LogError("Invalid Type:" + cast.E_CastType); break;
                    case enum_CastControllType.CastFromOrigin: return new WeaponHelperCaster(weaponIndex, cast, _entity, GetDamageBuffInfo);
                    case enum_CastControllType.CastControlledForward: return new WeaponHelperCasterControlled(weaponIndex, cast, _entity, GetDamageBuffInfo);
                    case enum_CastControllType.CastAtTarget: return new WeaponHelperCasterTarget(weaponIndex, cast, _entity, GetDamageBuffInfo);
                }
            }

            SFXBuffApply buffApply = weaponInfo as SFXBuffApply;
            if (buffApply)
                return new WeaponHelperBuffApply(weaponIndex, buffApply, _entity, GetDamageBuffInfo);

            SFXSubEntitySpawner entitySpawner = weaponInfo as SFXSubEntitySpawner;
            if (entitySpawner)
                return new WeaponHelperEntitySpawner(weaponIndex, entitySpawner, _entity, GetDamageBuffInfo);

            return null;
        }
    }

    public class WeaponHelperCaster : WeaponHelperBase
    {
        protected enum_CastTarget m_CastAt { get; private set; }
        protected bool m_castForward { get; private set; }
        public WeaponHelperCaster(int equipmentIndex, SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            m_CastAt = _castInfo.E_CastTarget;
            m_castForward = _castInfo.B_CastForward;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Transform castAt = GetCastAt(m_Entity);
            GameObjectManager.SpawnSFXWeapon<SFXCast>(I_Index, NavigationManager.NavMeshPosition(castAt.position), m_castForward ? castAt.forward : Vector3.up).Play(GetDamageDeliverInfo());
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

    public class WeaponHelperCasterTarget : WeaponHelperCaster
    {
        public WeaponHelperCasterTarget(int equipmentIndex, SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo) : base(equipmentIndex, _castInfo, _controller, _GetBuffInfo)
        {
        }
        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target)
        {
            Transform castAt = GetCastAt(_target);
            Vector3 castPos = NavigationManager.NavMeshPosition(castAt.position + TCommon.RandomXZSphere() * m_Entity.F_AttackSpread);
            castPos.y = castAt.transform.position.y;
            return castPos;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            GameObjectManager.SpawnSFXWeapon<SFXCast>(I_Index, _calculatedPosition, m_castForward ? m_Entity.tf_Weapon.forward : Vector3.up).Play(GetDamageDeliverInfo());
        }
    }

    public class WeaponHelperCasterControlled : WeaponHelperCaster
    {
        public override bool B_LoopAnim => true;
        SFXCast m_Cast;
        public WeaponHelperCasterControlled(int equipmentIndex, SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo) : base(equipmentIndex, _castInfo, _controller, _GetBuffInfo)
        {
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            if (!m_Cast)
            {
                m_Cast = GameObjectManager.SpawnSFXWeapon<SFXCast>(I_Index, m_Entity.tf_Weapon.position, m_Entity.tf_Weapon.forward);
                m_Cast.PlayControlled(m_Entity.m_EntityID, m_Entity, attacherHead);
            }

            if (m_Cast)
                m_Cast.ControlledCheck(GetDamageDeliverInfo());
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
    public class WeaponHelperBarrageRange : WeaponHelperBase
    {
        protected float f_projectileSpeed { get; private set; }
        protected RangeInt m_CountExtension { get; private set; }
        protected float m_OffsetExtension { get; private set; }
        int i_muzzleIndex;
        AudioClip m_MuzzleClip;

        public WeaponHelperBarrageRange(int equipmentIndex, SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            i_muzzleIndex = projectileInfo.I_MuzzleIndex;
            m_MuzzleClip = projectileInfo.AC_MuzzleClip;
            f_projectileSpeed = projectileInfo.F_Speed;
            m_CountExtension = projectileInfo.RI_CountExtension;
            m_OffsetExtension = projectileInfo.F_OffsetExtension;
        }

        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Vector3 startPosition = m_Entity.tf_Weapon.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            SpawnMuzzle(startPosition, direction);
            FireBullet(startPosition, direction, _calculatedPosition);

        }
        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target)
        {
            preAim = preAim && f_projectileSpeed > 10f;     //Case Aim Some Shit Positions

            float startDistance = TCommon.GetXZDistance(m_Entity.tf_Weapon.position, _target.tf_Head.position);
            Vector3 targetPosition = preAim ? _target.m_PrecalculatedTargetPos(startDistance / f_projectileSpeed) : _target.tf_Head.position;

            if (preAim && Mathf.Abs(TCommon.GetAngle(m_Entity.tf_Weapon.forward, TCommon.GetXZLookDirection(m_Entity.tf_Weapon.position, targetPosition), Vector3.up)) > 90)    //Target Positioned Back, Return Target
                targetPosition = _target.tf_Head.position;

            if (TCommon.GetXZDistance(m_Entity.tf_Weapon.position, targetPosition) > m_Entity.F_AttackSpread)      //Target Outside Spread Sphere,Add Spread
                targetPosition += TCommon.RandomXZSphere() * m_Entity.F_AttackSpread;
            return targetPosition;
        }

        protected void FireBullet(Vector3 startPosition, Vector3 direction, Vector3 targetPosition)
        {
            GameObjectManager.SpawnSFXWeapon<SFXProjectile>(I_Index, startPosition, direction).Play(GetDamageDeliverInfo(), direction, targetPosition);
        }
        protected void SpawnMuzzle(Vector3 startPosition, Vector3 direction) => GameObjectManager.PlayMuzzle(m_Entity.m_EntityID, startPosition, direction, i_muzzleIndex, m_MuzzleClip);
    }
    public class WeaponHelperBarrageMultipleLine : WeaponHelperBarrageRange
    {
        public WeaponHelperBarrageMultipleLine(int equipmentIndex, SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo) : base(equipmentIndex, projectileInfo, _controller, _GetBuffInfo)
        {
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Vector3 startPosition = m_Entity.tf_Weapon.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            int waveCount = m_CountExtension.Random();
            float distance = TCommon.GetXZDistance(startPosition, _calculatedPosition);
            Vector3 lineBeginPosition = startPosition - attacherHead.right * m_OffsetExtension * ((waveCount - 1) / 2f);
            SpawnMuzzle(startPosition, direction);
            for (int i = 0; i < waveCount; i++)
                FireBullet(lineBeginPosition + attacherHead.right * m_OffsetExtension * i, direction, m_Entity.tf_Weapon.position + direction * distance);
        }
    }

    public class WeaponHelperBarrageMultipleFan : WeaponHelperBarrageRange
    {
        public WeaponHelperBarrageMultipleFan(int equipmentIndex, SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo) : base(equipmentIndex, projectileInfo, _controller, _GetBuffInfo)
        {
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
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
                FireBullet(m_Entity.tf_Weapon.position, fanDirection, m_Entity.tf_Weapon.position + fanDirection * distance);
            }
        }
    }

    public class WeaponHelperBuffApply : WeaponHelperBase
    {
        public override bool B_TargetAlly => true;
        SBuff m_buffInfo;
        SFXBuffApply m_Effect;
        public WeaponHelperBuffApply(int equipmentIndex, SFXBuffApply buffApplyinfo, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            m_buffInfo = GameDataManager.GetPresetBuff(buffApplyinfo.I_BuffIndex);
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            if (!m_Effect || !m_Effect.B_Playing)
                m_Effect = GameObjectManager.SpawnSFXWeapon<SFXBuffApply>(I_Index, m_Entity.tf_Weapon.position, Vector3.up);

            m_Effect.Play(m_Entity.m_EntityID, m_buffInfo, m_Entity.tf_Weapon, _target);
        }
    }
    public class WeaponHelperEntitySpawner : WeaponHelperBase
    {
        bool m_SpawnAtTarget;
        public WeaponHelperEntitySpawner(int equipmentIndex, SFXSubEntitySpawner spawner, EntityCharacterBase _controller, Func<DamageInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            startHealth = 0;
            m_SpawnAtTarget = spawner.B_SpawnAtTarget;
        }
        Action<EntityCharacterBase> OnSpawn;
        float startHealth;
        public void SetOnSpawn(float _startHealth, Action<EntityCharacterBase> _OnSpawn)
        {
            OnSpawn = _OnSpawn;
            startHealth = _startHealth;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Vector3 spawnPosition = (m_SpawnAtTarget ? _target.transform.position : m_Entity.transform.position) + TCommon.RandomXZSphere() * m_Entity.F_AttackSpread;
            GameObjectManager.SpawnSFXWeapon<SFXSubEntitySpawner>(I_Index, spawnPosition, Vector3.up).Play(m_Entity, _target.transform.position, startHealth, OnSpawn);
        }
    }
    #endregion
    #endregion
}
