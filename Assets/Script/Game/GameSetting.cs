using UnityEngine;
using TExcel;
using System.Collections.Generic;
using System;
using TTiles;
using TPhysics;
using System.Linq;
using GameSetting_Action;
using UnityEngine.UI;
using TGameSave;
using LevelSetting;
using TSpecialClasses;
#pragma warning disable 0649
namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
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

        public const float F_PlayerWeaponFireReloadPause = 1f; //武器恢复间隔时间
        public const float F_PlayerAutoAimRangeBase = 16f; //自动锁定敌人范围
        public const float F_PlayerDamageAdjustmentRange = 0f;     //Test 不方便数据测试 临时关闭
        public const int I_PlayerRotationSmoothParam = 10;     //Camera Smooth Param For Player 10 is suggested

        public const int I_PlayerReviveBuffIndex = 40004;
        public static readonly RangeInt RI_GameFinalBattleEnermySpawnCheck = new RangeInt(10, 5); //BOSS关小怪刷新检测时间
        public const float F_FinalBattleEnermySpawnEliteHealthScaleOffset = .1f; //BOSS血量减少百分比会判断刷新小怪

        public static readonly List<EliteBuffCombine> L_GameEliteBuff = new List<EliteBuffCombine>() { new EliteBuffCombine(2010, 12010, 32010), new EliteBuffCombine(2020, 12020, 32020), new EliteBuffCombine(2030, 12030, 32030), new EliteBuffCombine(2040, 12040, 32040), new EliteBuffCombine(2050, 12050, 32050), new EliteBuffCombine(2060, 12060, 32060) };
        public const float F_EliteBuffTimerDurationWhenFullHealth=15f; //触发tick值
        public const float F_EliteBuffTimerTickRateMultiplyHealthLoss = 2f; //每秒加几tick=(1+血量损失比例* X)

        public const int I_DangerzoneDamage = 50;
        public const float F_DangerzoneResetDuration = 2f;

        public const float F_PickupMaxSpeed = 100f;
        public const float F_PickupAcceleration = 50f; //拾取物的飞行加速速度
        public const int I_HealthPickupAmount = 25;
        public const int I_ArmorPickupAmount = 25;
        public const int I_HealthPackAmount = 50;

        public static readonly Dictionary<enum_Rarity, RangeInt> D_EventWeaponTradePrice = new Dictionary<enum_Rarity, RangeInt>() { { enum_Rarity.Ordinary, new RangeInt(5, 5) }, { enum_Rarity.Advanced, new RangeInt(10, 5) }, { enum_Rarity.Rare, new RangeInt(20, 5) }, { enum_Rarity.Epic, new RangeInt(30, 5) } };

        public static readonly Dictionary<enum_Rarity, int> D_BattleFinishPerkGenerate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 60 }, { enum_Rarity.Advanced, 25 }, { enum_Rarity.Rare, 10 }, { enum_Rarity.Epic, 5 } };


        public const int I_WeaponVendorMachineNormalPrice = 10;
        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponVendorMachineNormalRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 35 }, { enum_Rarity.Advanced, 50 }, { enum_Rarity.Rare, 10 }, { enum_Rarity.Epic, 5 } };
        public const int I_WeaponVendorMachineRarePrice = 15;
        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponVendorMachineRareRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 5 }, { enum_Rarity.Advanced, 20 }, { enum_Rarity.Rare, 45 }, { enum_Rarity.Epic, 30 } };
        public const int I_EventPerkRarePrice = 10;
        public static readonly Dictionary<enum_Rarity, int> D_EventPerkRareRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Rare, 70 }, { enum_Rarity.Epic, 30 } };
        public const int I_EventPerkRareSelectPrice = 15;
        public static readonly Dictionary<enum_Rarity, int> D_EventPerkRareSelectRate = new Dictionary<enum_Rarity, int>() { {enum_Rarity.Advanced,20 }, { enum_Rarity.Rare, 50 }, { enum_Rarity.Epic, 30 } };
        public static readonly Dictionary<enum_Rarity, int> D_EventPerkFillRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 60 }, { enum_Rarity.Advanced , 30 }, { enum_Rarity.Rare, 8 },{ enum_Rarity.Epic,2} };
        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponReforgeRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 25 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 25 } };
        public const int I_EventWeaponReforgeSecondPrice = 10;
        public static readonly Dictionary<enum_Rarity, int> D_EventWeaponRecyclePrice = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 20 }, { enum_Rarity.Advanced, 30 }, { enum_Rarity.Rare, 50 }, { enum_Rarity.Epic, 75 } };
        public const int I_EventSafeCrackPrice = 20;
        public const int I_EventSafeCoinsCount = 5;
        public const int I_EventSafeCoinsAmount = 7;
        public static readonly RangeInt RI_EventSafeWeaponCount = new RangeInt(1, 1);
        public static readonly Dictionary<enum_Rarity, int> D_EventSafeWeaponRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 40 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 10 } };
        public static readonly RangeInt RI_EventSafePerkCount = new RangeInt(1, 1);
        public static readonly Dictionary<enum_Rarity, int> D_EventSafePerkRate = new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 40 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 10 } };

        public static class AI
        {
            public const float F_AIShowDistance = 30f;
            public const float F_AIIdleTargetDistance = 12;
            public const int F_AIIdleTargetAngle = 60;
            public const float F_AITargetIndicateRange = 5f;
            public static readonly float F_AIPatrolRange = 5f; //AI idle巡逻范围
            public static readonly RangeFloat RF_AIPatrolDuration = new RangeFloat(1f, 3f); //AI idle巡逻时间
            public const float F_AIMovementCheckParam = .3f; //AI检查玩家频率
            public const float F_AITargetCheckParam = .5f;      //AI Target Duration .5f is Suggested
            public const float F_AIReTargetCheckParam = 3f;       //AI Retarget Duration,3f is suggested
            public const float F_AITargetCalculationParam = .5f;       //AI Target Param Calculation Duration, 1 is suggested;
            public const float F_AIMaxRepositionDuration = .5f;
            public const float F_AIDeadImpact = 0.03f;   //.003f;
            public const int I_AIIdlePercentage = 50;
            public static readonly RangeFloat RF_AIBattleIdleDuration = new RangeFloat(1f, 2f);
        }

        public static readonly Dictionary<enum_CampFarmItemStatus, int> DP_FarmGeneratePercentage = new Dictionary<enum_CampFarmItemStatus, int>() { { enum_CampFarmItemStatus.Progress1, 60 }, { enum_CampFarmItemStatus.Progress2, 30 }, { enum_CampFarmItemStatus.Progress3, 6 }, { enum_CampFarmItemStatus.Progress4, 3 }, { enum_CampFarmItemStatus.Progress5, 1 } };   //Farm生成等级百分比
        public static readonly Dictionary<enum_CampFarmItemStatus, float> GetFarmCreditPerSecond = new Dictionary<enum_CampFarmItemStatus, float> { { enum_CampFarmItemStatus.Progress1, .1f / 60f }, { enum_CampFarmItemStatus.Progress2, .2f / 60f }, { enum_CampFarmItemStatus.Progress3, .3f / 60f }, { enum_CampFarmItemStatus.Progress4, .5f / 60f }, { enum_CampFarmItemStatus.Progress5, 1f / 60f } };      //Farm 等级,每秒Credit

        public const int I_RewardLevelRate = 40;
        public static readonly List<enum_LevelType> m_NormalLevelsPool = new List<enum_LevelType>() { enum_LevelType.EliteBattle, enum_LevelType.WeaponReforge, enum_LevelType.Bonefire, enum_LevelType.WeaponRecycle, enum_LevelType.WeaponVendorNormal, enum_LevelType.PerkFill, enum_LevelType.PerkRare, };
        public static readonly List<enum_LevelType> m_RewardLevelsPool = new List<enum_LevelType>() { enum_LevelType.PerkRareSelect, enum_LevelType.WeaponVendorRare, enum_LevelType.SafeCrack };

        public const int I_CampFarmPlot4UnlockDifficulty = 3;
        public const int I_CampFarmPlot5UnlockDifficulty = 10;
        public const int I_CampFarmPlot6UnlockTechPoints = 3000;
        public const int I_CampFarmItemAcquire = 50;
        public const int I_CampFarmDecayDuration = 20;
        public const float F_CampFarmItemTickAmount = 0.05f;

        public const int I_CampActionStorageNormalCount = 10;
        public const int I_CampActionStorageOutstandingCount = 30;
        public const int I_CampActionStorageEpicCount = 60;

        public const int I_CampActionCreditGainPerRequestSurplus = 100;
        public const int I_CampActionStorageRequestStampDuration = 30;//36000 //10 hours
    }

    public static class GameExpression
    {
        public static float GetCameraSmoothInterpolate(float widthParam, float widthSize)
        {
            float halfWidth = widthSize / 2;
            float interpolateParam = Mathf.InverseLerp(0, halfWidth, Mathf.Abs(widthParam));
            interpolateParam = Mathf.Clamp(interpolateParam,0,.5f)*(.5f+ Mathf.Lerp(.8f,.4f,interpolateParam)*.5f);
            return Mathf.Sign(widthParam) * interpolateParam * halfWidth;
        } 

        public static int GetPlayerWeaponIndex(int weaponIndex) =>weaponIndex * 10;
        public static int GetPlayerExtraWeaponIndex(int weaponIndex) => weaponIndex * 10+5;
        public static int GetPlayerPerkSFXWeaponIndex(int equipmentIndex) => 100000 + equipmentIndex * 10;
        public static int GetAIWeaponIndex(int entityIndex, int weaponIndex = 0, int subWeaponIndex = 0) => entityIndex * 100 + weaponIndex * 10 + subWeaponIndex;
        public static int GetWeaponSubIndex(int weaponIndex) => weaponIndex + 1;

        public static float F_PlayerSensitive(int sensitiveTap) => sensitiveTap / 5f;
        public static float F_GameVFXVolume(int vfxVolumeTap) => vfxVolumeTap / 10f;
        public static float F_GameMusicVolume(int musicVolumeTap) => musicVolumeTap / 10f;

        public static bool B_ShowHitMark(enum_HitCheck check) => check != enum_HitCheck.Invalid;

        public static SBuff GetEnermyGameBuff(enum_Stage stage,int difficulty) => SBuff.CreateGameEnermyBuff(difficulty, ((int)stage - 1) * .2f+ (difficulty - 1)*.2f );
        public static float GetEnermyMaxHealthMultiplier(enum_Stage stage, int difficulty) => 1f + ((int)stage - 1) * .2f + (difficulty - 1) * .05f;

        public static int GetRankupExp(int rank) => 100 + 10* rank;
        public static int GetEnermyKillExp(bool isElite,enum_Stage stage)
        {
            if (!isElite)
                return 25;
            switch (stage)
            {
                default: Debug.LogError("Invalid Convertions Here!") ; return 0;
                case enum_Stage.Rookie: return 200;
                case enum_Stage.Veteran: return 300;
                case enum_Stage.Ranger: return 400;
            }
        }
        
        public static float GetResultCompletion(bool win, enum_Stage _stage, int _battleLevelEntered) => win ? 1f : (.33f * ((int)_stage - 1) +.066f*_battleLevelEntered);
        public static float GetResultLevelScore(enum_Stage _stage, int _levelPassed) => 200 * ((int)_stage - 1) + 20 * (_levelPassed - 1);
        public static float GetResultDifficultyBonus(int _difficulty) =>1f+ _difficulty * .05f;
        public static float GetResultRewardCredits(float _totalScore) => _totalScore;

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
                case enum_Interaction.WeaponPickup:
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

        public static float GetLevelObjectHealth(enum_TileObjectType objectType)
        {
            switch(objectType)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return -1;
                case enum_TileObjectType.Block:
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

        public static int GetActionRemovePrice(enum_Stage stage, int removeTimes) => 10 * (removeTimes + 1) ;
        public static int GetActionUpgradePrice(enum_Stage stage, int upgradeTimes) => 10 * (upgradeTimes + 1) ;
        public static StageInteractGenerateData GetInteractGenerate(enum_Stage level)
        {
            switch (level)
            {
                default: return new StageInteractGenerateData();
                case enum_Stage.Rookie:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 25 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 25 } },        //Trade Perk
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 0 }, { enum_Rarity.Advanced, 100 }, { enum_Rarity.Rare, 0 }, { enum_Rarity.Epic, 0 } },        //Trade Weapon
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 50 }, { enum_Rarity.Advanced, 50 }, { enum_Rarity.Rare, 0 }, { enum_Rarity.Epic, 0 } },        //Reward Weapon
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),       //Normal Pickups
                        new Dictionary<enum_Rarity, float> {{ enum_Rarity.Ordinary, 6 },{ enum_Rarity.Advanced,3} }),          //Normal Weapon Pikcups
                        PickupGenerateData.Create(10, 100, 50, new RangeInt(10, 5),     //Elite Pickups
                        new Dictionary<enum_Rarity, float> { { enum_Rarity.Ordinary, 0 }, { enum_Rarity.Advanced, 100 } })        //Elite Weapon Pickups
                        );
                case enum_Stage.Veteran:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 25 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 25 } },        //Trade Perk
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 0 }, { enum_Rarity.Advanced, 0 }, { enum_Rarity.Rare, 100 }, { enum_Rarity.Epic, 0 } },
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 0 }, { enum_Rarity.Advanced,50 }, { enum_Rarity.Rare, 50 }, { enum_Rarity.Epic, 0 } },
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),
                        new Dictionary<enum_Rarity, float> {{ enum_Rarity.Advanced, 3.8f },{ enum_Rarity.Rare,1.9f} }),
                        PickupGenerateData.Create(10, 100, 100, new RangeInt(10, 5),
                        new Dictionary<enum_Rarity, float> { { enum_Rarity.Rare, 100 } })
                        );
                case enum_Stage.Ranger:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 25 }, { enum_Rarity.Advanced, 25 }, { enum_Rarity.Rare, 25 }, { enum_Rarity.Epic, 25 } },        //Trade Perk
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 0 }, { enum_Rarity.Advanced, 0 }, { enum_Rarity.Rare, 0 }, { enum_Rarity.Epic, 100 } },
                        new Dictionary<enum_Rarity, int>() { { enum_Rarity.Ordinary, 0 }, { enum_Rarity.Advanced, 0 }, { enum_Rarity.Rare, 50 }, { enum_Rarity.Epic, 50 } },
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),
                        new Dictionary<enum_Rarity, float> {{ enum_Rarity.Rare, 2.8f },{ enum_Rarity.Epic, 2.8f} }),
                        PickupGenerateData.Create(0, 0, 0, new RangeInt(10, 5),
                        new Dictionary<enum_Rarity, float> { { enum_Rarity.Ordinary, 0 }, { enum_Rarity.Advanced, 0 } })
                        );
            }
        }
        
        public static bool CanGenerateprofit(this enum_CampFarmItemStatus status)
        {
            switch(status)
            {
                default: return false;
                case enum_CampFarmItemStatus.Progress1:
                case enum_CampFarmItemStatus.Progress2:
                case enum_CampFarmItemStatus.Progress3:
                case enum_CampFarmItemStatus.Progress4:
                case enum_CampFarmItemStatus.Progress5:
                    return true;
            }
        }

    }

    public static class LocalizationKeyJoint
    {
        public static string GetNameLocalizeKey(this EntityExpirePreset buff) => "Buff_Name_" + buff.m_Index;
        public static string GetNameLocalizeKey(this ExpirePerkBase action) => "Action_Name_" + action.m_Index;
        public static string GetIntroLocalizeKey(this ExpirePerkBase action) => "Action_Intro_" + action.m_Index;
        public static string GetLocalizeKey(this enum_Stage stage) => "Game_Stage_" + stage;
        public static string GetLocalizeKey(this enum_GameStyle style) => "Game_Style_" + style;
        public static string GetLocalizeNameKey(this enum_LevelType type) => "UI_Level_" + type + "_Name";
        public static string GetLocalizeIntroKey(this enum_LevelType type) => "UI_Level_" + type + "_Intro";
        public static string GetLocalizeNameKey(this enum_PlayerWeapon weapon) => "Weapon_Name_" + weapon;
        public static string GetTitleLocalizeKey(this enum_Interaction interact) => "UI_Interact_" + interact+"_Title";
        public static string GetIntroLocalizeKey(this enum_Interaction interact) => "UI_Interact_" + interact + "_Intro";
        public static string GetLocalizeKey(this enum_Option_FrameRate frameRate) => "UI_Option_" + frameRate;
        public static string GetLocalizeKey(this enum_Option_JoyStickMode joystick) => "UI_Option_" + joystick;
        public static string GetLocalizeKey(this enum_Option_LanguageRegion region) => "UI_Option_" + region;
        public static string GetLocalizeKey(this enum_CampFarmItemStatus status) => "UI_Farm_" + status;
        public static string SetActionIntro(this ExpirePerkBase actionInfo, UIT_TextExtend text) => text.formatText(actionInfo.GetIntroLocalizeKey() , actionInfo.Value1, actionInfo.Value2, actionInfo.Value3);
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
    #region Default Readonly

    public struct GameLevelPortalData
    {
        public enum_LevelType m_PortalMain { get; private set; }
        public enum_LevelType m_PortalExtra { get; private set; }
        public GameLevelPortalData(enum_LevelType _mainType,enum_LevelType _subType)
        {
            m_PortalMain = _mainType;
            m_PortalExtra = _subType;
        }
        
    }


    public struct PickupGenerateData
    {
        public int m_HealthRate { get; private set; }
        public int m_ArmorRate { get; private set; }
        public int m_CoinRate { get; private set; }
        public RangeInt m_CoinRange { get; private set; }
        public Dictionary<enum_Rarity, float> m_WeaponRate { get; private set; }

        public bool CanGenerateHealth() => TCommon.RandomPercentage() <= m_HealthRate;
        public bool CanGenerateArmor() => TCommon.RandomPercentage() <= m_ArmorRate;
        public bool CanGenerateCoins( out int amount)
        {
            amount = -1;
            if (TCommon.RandomPercentage() <=  m_CoinRate)
            {
                amount = m_CoinRange.Random();
                return true;
            }
            return false;
        }
        public static PickupGenerateData Create(int healthRate, int armorRate, int coinRate, RangeInt coinAmount, Dictionary<enum_Rarity, float> _weaponRate) => new PickupGenerateData() { m_HealthRate = healthRate, m_ArmorRate = armorRate, m_CoinRate = coinRate, m_CoinRange = coinAmount, m_WeaponRate=_weaponRate };
    }

    public struct StageInteractGenerateData
    {
        public Dictionary<enum_Rarity,int> m_TradePerk { get; private set; }
        public Dictionary<enum_Rarity, int> m_TradeWeapon { get; private set; }
        public Dictionary<enum_Rarity, int> m_RewardWeapon { get; private set; }
        public PickupGenerateData m_NormalPickupData { get; private set; }
        public PickupGenerateData m_ElitePickupData { get; private set; }
        public static StageInteractGenerateData Create(Dictionary<enum_Rarity, int>  _tradePerkRate,Dictionary<enum_Rarity,int> _tradeWeaponRate, Dictionary<enum_Rarity, int> _rewardWeaponRate, PickupGenerateData _normalGenerate,PickupGenerateData _eliteGenerate) => new StageInteractGenerateData() { m_TradePerk=_tradePerkRate, m_TradeWeapon =_tradeWeaponRate, m_RewardWeapon = _rewardWeaponRate, m_NormalPickupData=_normalGenerate,m_ElitePickupData=_eliteGenerate};
    }

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

    #region SaveData
    public class CGameSave : ISave
    {
        public float f_Credits;
        public float f_TechPoints;
        public int m_GameDifficulty;
        public int m_DifficultyUnlocked;
        public enum_PlayerCharacter m_CharacterSelected;
        public int m_StorageRequestStamp;
        public CGameSave()
        {
            f_Credits = 0;
            f_TechPoints = 0;
            m_GameDifficulty = 1;
            m_DifficultyUnlocked = 1;
            m_StorageRequestStamp = -1;
            m_CharacterSelected = enum_PlayerCharacter.Beth;
        }

        public void UnlockDifficulty()
        {
            if (m_GameDifficulty != m_DifficultyUnlocked)
                return;

            m_DifficultyUnlocked++;
            m_GameDifficulty++;
        }

        public void DataRecorrect()
        {
        }
    }

    public class CFarmSave : ISave
    {
        public int m_OffsiteProfitStamp;
        public List<CampFarmPlotData> m_PlotStatus;
        public CFarmSave()
        {
            m_OffsiteProfitStamp = TTime.TTimeTools.GetTimeStampNow();
            m_PlotStatus = new List<CampFarmPlotData>() { CampFarmPlotData.Create( enum_CampFarmItemStatus.Empty), CampFarmPlotData.Create(enum_CampFarmItemStatus.Empty), CampFarmPlotData.Create(enum_CampFarmItemStatus.Empty), CampFarmPlotData.Create(enum_CampFarmItemStatus.Locked), CampFarmPlotData.Create(enum_CampFarmItemStatus.Locked), CampFarmPlotData.Create(enum_CampFarmItemStatus.Locked) };
        }
        public void Save(CampFarmManager manager)
        {
            m_PlotStatus.Clear();
            m_OffsiteProfitStamp = manager.m_LastProfitStamp; 
            for (int i=0;i< manager.m_Plots.Count; i++)
                m_PlotStatus.Add(CampFarmPlotData.SaveData(manager.m_Plots[i]));
        }

        public void UnlockPlot(int difficulty)
        {
            if (difficulty >=GameConst.I_CampFarmPlot4UnlockDifficulty && m_PlotStatus[3].m_Status == enum_CampFarmItemStatus.Locked)
                m_PlotStatus[3] = CampFarmPlotData.Create(enum_CampFarmItemStatus.Empty);
            if (difficulty >= GameConst.I_CampFarmPlot5UnlockDifficulty && m_PlotStatus[4].m_Status == enum_CampFarmItemStatus.Locked)
                m_PlotStatus[4] = CampFarmPlotData.Create(enum_CampFarmItemStatus.Empty);
        }

        void ISave.DataRecorrect()
        {
            if(m_PlotStatus.Count!=6)
                m_PlotStatus= new List<CampFarmPlotData>() { CampFarmPlotData.Create(enum_CampFarmItemStatus.Empty), CampFarmPlotData.Create(enum_CampFarmItemStatus.Empty), CampFarmPlotData.Create(enum_CampFarmItemStatus.Empty), CampFarmPlotData.Create(enum_CampFarmItemStatus.Locked), CampFarmPlotData.Create(enum_CampFarmItemStatus.Locked), CampFarmPlotData.Create(enum_CampFarmItemStatus.Locked) };
        }
    }

    public class CBattleSave : ISave
    {
        public string m_GameSeed;
        public enum_Stage m_Stage;
        public int m_LevelPassed;
        public PlayerSaveData m_PlayerData;
        public CBattleSave()
        {
            m_Stage = enum_Stage.Rookie;
            m_GameSeed = DateTime.Now.ToLongTimeString();
            m_PlayerData = new PlayerSaveData( enum_PlayerCharacter.Assassin, enum_PlayerWeapon.DE);
        }
        public void Adjust(EntityCharacterPlayer _player, GameProgressManager _level)
        {
            m_GameSeed = _level.m_GameSeed;
            m_Stage = _level.m_StageIndex;
            m_LevelPassed = _level.m_LevelPassed;
            m_PlayerData = new PlayerSaveData(_player);
        }

        void ISave.DataRecorrect()
        {
        }
    }

    public class CGameOptions : ISave
    {
        public enum_Option_JoyStickMode m_JoyStickMode;
        public enum_Option_FrameRate m_FrameRate;
        public enum_Option_Effect m_Effect;
        public enum_Option_Bloom m_Bloom;
        public enum_Option_LanguageRegion m_Region;
        public bool m_ShadowOff;
        public int m_MusicVolumeTap;
        public int m_VFXVolumeTap;
        public int m_SensitiveTap;

        public CGameOptions()
        {
            m_JoyStickMode = enum_Option_JoyStickMode.Retarget;
            m_FrameRate = enum_Option_FrameRate.High;
            m_Effect = enum_Option_Effect.High;
            m_Bloom = enum_Option_Bloom.High;
            m_Region = enum_Option_LanguageRegion.CN;
            m_ShadowOff = false;
            m_SensitiveTap = 5;
            m_MusicVolumeTap = 10;
            m_VFXVolumeTap = 10;
        }


        void ISave.DataRecorrect()
        {
        }
    }

    public struct WeaponSaveData : IXmlPhrase
    {
        public enum_PlayerWeapon m_Weapon { get; private set; }
        public static WeaponSaveData Create(WeaponBase weapon) => new WeaponSaveData() { m_Weapon = weapon != null ? weapon.m_WeaponInfo.m_Weapon : enum_PlayerWeapon.Invalid };
        public static WeaponSaveData CreateNew(enum_PlayerWeapon weapon) => new WeaponSaveData() { m_Weapon = weapon };
    }

    public struct MercenarySaveData:IXmlPhrase
    {
        public enum_MercenaryCharacter m_MercenaryCharacter { get; private set; }
        public WeaponSaveData m_Weapon { get; private set; }
        public float m_Health { get;private set; }
        public MercenarySaveData(EntityCharacterMercenary _mercenary)
        {
            m_Weapon = WeaponSaveData.Create(_mercenary.m_Weapon);
            m_MercenaryCharacter = _mercenary.m_Character;
            m_Health = _mercenary.m_Health.m_CurrentHealth;

        }
    }

    public struct PlayerSaveData:IXmlPhrase
    {
        public float m_Health { get; private set; }
        public float m_Coins { get; private set; }
        public WeaponSaveData m_Weapon1 { get; private set; }
        public WeaponSaveData m_Weapon2 { get; private set; }
        public enum_PlayerCharacter m_Character { get; private set; }
        public List<PerkSaveData> m_Perks { get; private set; }
        public PlayerSaveData(enum_PlayerCharacter player,enum_PlayerWeapon weapon)
        {
            m_Coins = 0;
            m_Health = -1;
            m_Perks = new List<PerkSaveData>();
            m_Character = player;
            m_Weapon1 = WeaponSaveData.CreateNew(weapon);
            m_Weapon2 = WeaponSaveData.CreateNew(enum_PlayerWeapon.Invalid);
        }

        public PlayerSaveData(EntityCharacterPlayer _player)
        {
            m_Character = _player.m_Character;
            m_Coins = _player.m_CharacterInfo.m_Coins;
            m_Health = _player.m_Health.m_CurrentHealth;
            m_Weapon1 = WeaponSaveData.Create(_player.m_Weapon1);
            m_Weapon2 = WeaponSaveData.Create(_player.m_Weapon2);
            m_Perks = PerkSaveData.Create(_player.m_CharacterInfo.m_ExpirePerks.Values.ToList());
        }
    }

    public struct PerkSaveData:IXmlPhrase
    {
        public int m_Index { get; private set; }
        public int m_PerkStack { get; private set; }
        public float m_RecordData { get; private set; }
        public static PerkSaveData New(int index) => new PerkSaveData() { m_Index = index, m_PerkStack = 1, m_RecordData = -1 };
        public static PerkSaveData Create(ExpirePerkBase perk) => new PerkSaveData() { m_Index = perk.m_Index,m_PerkStack=perk.m_Stack, m_RecordData = perk.m_RecordData };
        public static List<PerkSaveData> Create(List<ExpirePerkBase> perks)
        {
            List<PerkSaveData> data = new List<PerkSaveData>();
            perks.Traversal((ExpirePerkBase perk) => { data.Add(Create(perk)); });
            return data;
        }
    }

    public struct CampFarmPlotData : IXmlPhrase
    {
        public int m_StartStamp { get; private set; }
        public enum_CampFarmItemStatus m_Status { get; private set; }

        public static CampFarmPlotData Create(enum_CampFarmItemStatus _status) => new CampFarmPlotData { m_StartStamp = -1, m_Status = _status };
        public static CampFarmPlotData SaveData(CampFarmPlot _plot) => new CampFarmPlotData { m_StartStamp = _plot.m_StartStamp, m_Status = _plot.m_Status };
    }
    #endregion

    #region ExcelData
    public struct SWeapon : ISExcel
    {
        int index;
        bool b_hidden;
        int i_rarity;
        float f_fireRate;
        int i_clipAmount;
        float f_spread;
        float f_bulletRefill;
        int i_PelletsPerShot;
        float f_weight;
        float f_recoil;

        float f_UIDamage;
        float f_UIRPM;
        float f_UIStability;
        float f_UISpeed;

        public int m_Index => index;
        public bool m_Hidden => b_hidden;
        public enum_PlayerWeapon m_Weapon => (enum_PlayerWeapon)index;
        public enum_Rarity m_Rarity => (enum_Rarity)i_rarity;
        public float m_FireRate => f_fireRate;
        public int m_ClipAmount => i_clipAmount;
        public float m_Spread => f_spread;
        public float m_BulletRefillTime => f_bulletRefill;
        public int m_PelletsPerShot => i_PelletsPerShot;
        public float m_Weight => f_weight;
        public float m_RecoilPerShot =>f_recoil;

        public float m_UIDamage => f_UIDamage;
        public float m_UIRPM => f_UIRPM;
        public float m_UIStability => f_UIStability;
        public float m_UISpeed => f_UISpeed;

        public void InitOnValueSet()
        {
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
        float f_healthDrainMultiply;
        float f_damageMultiply;
        float f_damageReduce;
        int i_extraBuffApply;
        float f_damageTickTime;
        float f_damagePerTick;
        int i_damageType;
        public int m_Index => index;
        public enum_ExpireRefreshType m_AddType => (enum_ExpireRefreshType)i_addType;
        public float m_ExpireDuration => f_expireDuration;
        public int m_EffectIndex => i_effect;
        public float m_MovementSpeedMultiply => f_movementSpeedMultiply;
        public float m_FireRateMultiply => f_fireRateMultiply;
        public float m_ReloadRateMultiply => f_reloadRateMultiply;
        public float m_HealthDrainMultiply => f_healthDrainMultiply;
        public float m_DamageMultiply => f_damageMultiply;
        public float m_DamageReduction => f_damageReduce;
        public int m_ExtraBuffApply => i_extraBuffApply;
        public float m_DamageTickTime => f_damageTickTime;
        public float m_DamagePerTick => f_damagePerTick;
        public enum_DamageType m_DamageType => (enum_DamageType)i_damageType;
        public void InitOnValueSet()
        {
            f_movementSpeedMultiply /= 100f;
            f_fireRateMultiply /= 100f;
            f_reloadRateMultiply /= 100f;
            f_damageMultiply /= 100f;
            f_damageReduce /= 100f;
            f_healthDrainMultiply /= 100f;
        }
        //Normally In Excel 0-99
       //1000-9999
        public static SBuff CreateGameEnermyBuff(int difficulty, float damageMultiply)
        {
            SBuff buff = new SBuff();
            buff.index = 1000 + difficulty;
            buff.i_addType = (int)enum_ExpireRefreshType.Refresh;
            buff.f_damageMultiply = damageMultiply;
            return buff;
        }
    }

    public struct SEnermyGenerate : ISExcel
    {
        bool b_isFinal;
        int i_MeleeCount;
        int i_E2Count;
        int i_E3Count;
        int i_E4Count;
        int i_E5Count;
        int i_E6Count;
        int i_EliteCount;

        public bool m_IsFinal => b_isFinal;

        public void InitOnValueSet()
        {
        }

        public static SEnermyGenerate operator +(SEnermyGenerate data1,SEnermyGenerate data2)
        {
            data1.i_MeleeCount += data2.i_MeleeCount;
            data1.i_E2Count += data2.i_E2Count;
            data1.i_E3Count += data2.i_E3Count;
            data1.i_E4Count += data2.i_E4Count;
            data1.i_E5Count += data2.i_E5Count;
            data1.i_E6Count += data2.i_E6Count;
            data1.i_EliteCount += data2.i_EliteCount;
            return data1;
        }
        
        public List<int> GetEnermyIDList(Dictionary<enum_EnermyType, int> m_EnermyIDs)
        {
            List<int> enermyID = new List<int>();
            for (int i = 0; i < i_MeleeCount; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.Melee]);
            for (int i = 0; i < i_E2Count; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.E2]);
            for (int i = 0; i < i_E3Count; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.E3]);
            for (int i = 0; i < i_E4Count; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.E4]);
            for (int i = 0; i < i_E5Count; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.E5]);
            for (int i = 0; i < i_E6Count; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.E6]);
            for (int i = 0; i < i_EliteCount; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.Elite]);
            return enermyID;
        }
    }
    #endregion
    #endregion

    #region Class
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
        public bool m_ArmorFull => m_CurrentArmor >= m_MaxHealth;
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
        public void SetHealthMultiplier(float healthMultiplier)
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

        public void OnBattleFinishResetArmor() => base.OnSetStatus(m_CurrentHealth, m_MaxArmor);
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


        public DamageInfo(int sourceID, float damage,float _damageMultiply,float _critcalHitMultiply, enum_DamageType type,int extraBuffID)
        {
            m_SourceID = sourceID;
            m_DamageBase = damage;
            m_DamageMultiply = _damageMultiply;
            m_DamageCriticalMultipy = _critcalHitMultiply;
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
        public virtual enum_WeaponTrigger m_TriggerType => enum_WeaponTrigger.Invalid;
        public bool m_TriggerDown { get; protected set; }
        public virtual void OnSetTrigger(bool down)
        {
            m_TriggerDown = down;
        }

        public virtual void Tick(bool paused,float deltaTime)
        {

        }
    }
    public class WeaponTriggerAuto : WeaponTrigger
    {
        public override enum_WeaponTrigger m_TriggerType => enum_WeaponTrigger.Auto;
        protected Func<bool> OnTriggerCheck;
        Action OnTriggerSuccessful;
        TimeCounter m_TriggerTimer = new TimeCounter();
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
        public override enum_WeaponTrigger m_TriggerType => enum_WeaponTrigger.Auto;
         Func<bool> OnStoreBeginCheck;
        Action<bool> OnStoreFinish;
        TimeCounter m_StoreTimer = new TimeCounter();
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

            m_Storing = true;
            m_StoreTimer.Replay();
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

            OnStoreFinish(! m_StoreTimer.m_Timing);
            m_Storing = false;
        }
    }
    #endregion

    #region Entity Info Manager
    public class CharacterInfoManager
    {
        protected EntityCharacterBase m_Entity { get; private set; }
        public List<EntityExpireBase> m_Expires { get; private set; } = new List<EntityExpireBase>();
        protected Dictionary<int, SFXEffect> m_BuffEffects { get; private set; } = new Dictionary<int, SFXEffect>();
        public float F_DamageReceiveMultiply { get; private set; } = 1f;
        public float F_HealReceiveMultiply { get; private set; } = 1f;
        public float F_MovementSpeedMultiply { get; private set; } = 1f;
        public float F_FireRateMultiply { get; private set; } = 1f;
        protected float F_ReloadRateMultiply { get; private set; } = 1f;
        protected float F_HealthDrainMultiply { get; private set; } = 0f;
        protected float F_DamageMultiply { get; private set; } = 0f;
        public float F_CriticalHitRate { get; private set; } = 0f;
        public float F_CriticalHitMultiply { get; private set; } = 1f;
        public float F_FireRateTick(float deltaTime) => deltaTime * F_FireRateMultiply;
        public float F_ReloadRateTick(float deltaTime) => deltaTime * F_ReloadRateMultiply;
        public float F_MovementSpeed => m_Entity.m_baseMovementSpeed * F_MovementSpeedMultiply;
        
        public bool CheckCriticalHit() => TCommon.RandomLength(1f) < F_CriticalHitRate;
        public float GetCritalDamageMultiply() =>   F_CriticalHitMultiply;
        public virtual DamageInfo GetDamageBuffInfo(float baseDamage, int buff = 0, enum_DamageType type = enum_DamageType.Basic) =>new DamageInfo(m_Entity.m_EntityID, baseDamage, F_DamageMultiply, CheckCriticalHit() ? GetCritalDamageMultiply() : 0f, type, buff);

        Func<DamageInfo, bool> OnReceiveDamage;
        Action OnExpireInfoChange;

        bool b_expireUpdated = false;
        public void UpdateEntityInfo() => b_expireUpdated = false;

        public CharacterInfoManager(EntityCharacterBase _attacher, Func<DamageInfo, bool> _OnReceiveDamage, Action _OnExpireChange)
        {
            m_Entity = _attacher;
            OnReceiveDamage = _OnReceiveDamage;
            OnExpireInfoChange = _OnExpireChange;
        }

        public virtual void OnActivate()=>Reset();
        public virtual void OnDead() => Reset();
        public virtual void OnRecycle() => Reset();
        public virtual void OnRevive() => Reset();

        public virtual void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
        {
            if (amountApply <= 0)
                return;

            if ( damageInfo.m_SourceID == m_Entity.m_EntityID)
            {
                if (F_HealthDrainMultiply > 0)
                    m_Entity.m_HitCheck.TryHit(new DamageInfo(m_Entity.m_EntityID ,- amountApply * F_HealthDrainMultiply, enum_DamageType.Health));
                m_Expires.Traversal((EntityExpireBase expire) => { expire.OnDealtDamage(damageEntity,damageInfo,amountApply); });
            }
        }
        protected virtual void Reset()
        {
            m_Expires.Traversal((EntityExpireBase expire) => { if (expire.m_ExpireType == enum_ExpireType.Preset) RemoveExpire(expire); }, true);
            UpdateExpireInfo();
            UpdateExpireEffect();
        }

        public virtual void Tick(float deltaTime) {
            m_Expires.Traversal((EntityExpireBase expire) => { expire.OnTick(deltaTime); });

            if (b_expireUpdated)
                return;
            UpdateExpireInfo();
            UpdateExpireEffect();
            OnExpireInfoChange?.Invoke();
            b_expireUpdated = true;
        }

        protected virtual void AddExpire(EntityExpireBase expire)
        {
            m_Expires.Add(expire);
            UpdateEntityInfo();
        }
        protected virtual void RemoveExpire(EntityExpireBase expire)
        {
            m_Expires.Remove(expire);
            UpdateEntityInfo();
        }
        public void AddBuff(int sourceID, SBuff buffInfo)
        {
            EntityExpirePreset buff = new EntityExpirePreset(sourceID, buffInfo, OnReceiveDamage, RemoveExpire);
            switch (buff.m_RefreshType)
            {
                case enum_ExpireRefreshType.AddUp:
                    AddExpire(buff);
                    break;
                case enum_ExpireRefreshType.Refresh:
                    {
                        EntityExpirePreset buffRefresh = m_Expires.Find(p => p.m_Index == buff.m_Index) as EntityExpirePreset;
                        if (buffRefresh != null)
                            buffRefresh.BuffRefresh();
                        else
                            AddExpire(buff);
                    }
                    break;
            }
        }
        #region ExpireInfo
        void UpdateExpireInfo()
        {
            OnResetInfo();
            m_Expires.Traversal(OnSetExpireInfo);
            AfterInfoSet();
        }
        protected virtual void OnResetInfo()
        {
            F_DamageReceiveMultiply = 1f;
            F_HealReceiveMultiply = 1f;
            F_MovementSpeedMultiply = 1f;
            F_FireRateMultiply = 1f;
            F_ReloadRateMultiply = 1f;
            F_DamageMultiply = 0f;
            F_CriticalHitRate = 0f;
            F_CriticalHitMultiply = 1f;
            F_HealthDrainMultiply = 0f;
        }
        protected virtual void OnSetExpireInfo(EntityExpireBase expire)
        {
            F_DamageMultiply += expire.m_DamageMultiply;
            F_DamageReceiveMultiply -= expire.m_DamageReduction;
            F_HealReceiveMultiply += expire.m_HealAdditive;
            F_MovementSpeedMultiply += expire.m_MovementSpeedMultiply;
            F_FireRateMultiply += expire.m_FireRateMultiply;
            F_ReloadRateMultiply += expire.m_ReloadRateMultiply;
            F_CriticalHitRate += expire.m_CriticalChangeAdditive;
            F_CriticalHitMultiply += expire.m_CriticalHitMultiplyAdditive;
            F_HealthDrainMultiply += expire.m_HealthDrainMultiply;
        }
        protected virtual void AfterInfoSet()
        {
            if (F_DamageReceiveMultiply < 0) F_DamageReceiveMultiply = 0;
            if (F_MovementSpeedMultiply < 0) F_MovementSpeedMultiply = 0;
            if (F_HealthDrainMultiply < 0) F_HealthDrainMultiply = 0;
            if (F_HealReceiveMultiply < 0) F_HealReceiveMultiply = 0;
            if (F_CriticalHitMultiply < 0) F_CriticalHitMultiply = 0;
            if (F_CriticalHitRate < 0) F_CriticalHitRate = 0;
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
                if (m_Expires.Find(p => p.m_EffectIndex == effectIndex) != null)
                    return;

                m_BuffEffects[effectIndex].Stop();
                m_BuffEffects.Remove(effectIndex);
            },true);
        }
    }

    public class PlayerInfoManager : CharacterInfoManager
    {
        EntityCharacterPlayer m_Player;
        public int I_ClipAmount(int baseClipAmount) => baseClipAmount == 0 ? 0 : (int)((baseClipAmount + I_ClipAdditive) * F_ClipMultiply);
        public float F_SpreadMultiply { get; private set; } = 1f;
        public float F_AimMovementStrictMultiply { get; private set; } = 1f;
        public float F_ProjectileSpeedMuiltiply { get; private set; } = 1f;
        public float F_PenetrateAdditive { get; private set; } = 0;
        public float F_AimRangeAdditive { get; private set; } = 0;
        public float F_MaxHealthAdditive { get; private set; } = 0;
        public float F_MaxArmorAdditive { get; private set; } = 0;
        public float F_AllyHealthMultiplierAdditive { get; private set; } = 0f;
        public float F_CoinsCostMultiply { get; private set; } = 0f;
        protected int I_ClipAdditive { get; private set; } = 0;
        protected float F_ClipMultiply { get; private set; } = 1f;
        protected float F_DamageAdditive = 0f;

        protected Vector3 m_prePos;

        public Dictionary<int, ExpirePerkBase> m_ExpirePerks { get; private set; } = new Dictionary<int, ExpirePerkBase>();
        public float m_Coins { get; private set; } = 0;

        public PlayerInfoManager(EntityCharacterPlayer _attacher, Func<DamageInfo, bool> _OnReceiveDamage, Action _OnExpireChange) : base(_attacher, _OnReceiveDamage, _OnExpireChange)
        {
            m_Player = _attacher;
        }

        public override void OnActivate()
        {
            base.OnActivate();
            m_prePos = m_Entity.transform.position;
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            UpdateEntityInfo();
            OnMove(TCommon.GetXZDistance(m_prePos, m_Entity.transform.position));
            m_prePos = m_Entity.transform.position;
        }

        public void SetInfoData(PlayerSaveData _saveData)
        {
            m_Coins = _saveData.m_Coins;
            _saveData.m_Perks.Traversal((PerkSaveData perkData) => { AddExpire(PerkDataManager.CreatePerk(perkData)); });
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
        }

        #region Perks
        #region Interact
        public bool CheckRevive() => m_ExpirePerks.Any(p => p.Value.OnCheckRevive());
        public void OnActionPerkAcquire(int perkID)
        {
            if (m_ExpirePerks.ContainsKey(perkID))
                m_ExpirePerks[perkID].OnStackUp();
            else
                AddExpire(PerkDataManager.CreatePerk(PerkSaveData.New(perkID)));

            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
        }


        #endregion
        #region List Update
        protected override void AddExpire(EntityExpireBase expire)
        {
            base.AddExpire(expire);
            ExpirePerkBase targetExpire = expire as ExpirePerkBase;
            if (targetExpire == null)
                return;

            m_ExpirePerks.Add(targetExpire.m_Index,targetExpire);
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
            targetExpire.OnActivate(m_Player, RemoveExpire);
        }

        protected override void RemoveExpire(EntityExpireBase expire)
        {
            base.RemoveExpire(expire);
            ExpirePerkBase targetExpire = expire as ExpirePerkBase;
            if (targetExpire == null)
                return;
            m_ExpirePerks.Remove(targetExpire.m_Index);
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
        }

        #endregion
        #region Data Update
        protected override void OnResetInfo()
        {
            base.OnResetInfo();
            F_DamageAdditive = 0f;
            I_ClipAdditive = 0;
            F_ClipMultiply = 1f;
            F_PenetrateAdditive = 0;
            F_AimRangeAdditive = 0;
            F_SpreadMultiply = 1f;
            F_ProjectileSpeedMuiltiply = 1f;
            F_AimMovementStrictMultiply = 1f;
            F_AllyHealthMultiplierAdditive = 1f;
            F_MaxHealthAdditive = 0f;
            F_MaxArmorAdditive = 0;
            F_CoinsCostMultiply = 1f;
        }
        protected override void OnSetExpireInfo(EntityExpireBase expire)
        {
            base.OnSetExpireInfo(expire);
            ExpirePerkBase action = expire as ExpirePerkBase;
            if (action == null)
                return;

            F_DamageAdditive += action.m_DamageAdditive;
            F_SpreadMultiply -= action.F_SpreadReduction;
            F_AimMovementStrictMultiply -= action.F_AimPressureReduction;

            I_ClipAdditive += action.I_ClipAdditive;
            F_ClipMultiply += action.F_ClipMultiply;

            F_ProjectileSpeedMuiltiply += action.F_ProjectileSpeedMultiply;
            F_PenetrateAdditive += action.F_PenetradeAdditive;
            F_AimRangeAdditive += action.F_AimRangeAdditive;
            F_MaxHealthAdditive += action.m_MaxHealthAdditive;
            F_MaxArmorAdditive += action.m_MaxArmorAdditive;
            F_AllyHealthMultiplierAdditive += action.F_AllyHealthMultiplierAdditive;
            F_CoinsCostMultiply -= action.F_Discount;
        }
        protected override void AfterInfoSet()
        {
            base.AfterInfoSet();
            if (F_AllyHealthMultiplierAdditive < .1f) F_AllyHealthMultiplierAdditive = .1f;
            if (F_AimMovementStrictMultiply < 0) F_AimMovementStrictMultiply = 0;
            if (F_SpreadMultiply < 0) F_SpreadMultiply = 0;
            if (F_CoinsCostMultiply < 0) F_CoinsCostMultiply = 0;
        }
        #endregion
        #region Action Helpers
        public override DamageInfo GetDamageBuffInfo(float baseDamage,int buff=0,enum_DamageType type= enum_DamageType.Basic)
        {
            float randomDamageMultiply = UnityEngine.Random.Range(-GameConst.F_PlayerDamageAdjustmentRange, GameConst.F_PlayerDamageAdjustmentRange);
            DamageInfo info= new DamageInfo(m_Entity.m_EntityID, baseDamage + F_DamageAdditive, F_DamageMultiply+ randomDamageMultiply, CheckCriticalHit() ? GetCritalDamageMultiply() : 0f, type, buff);
            m_ExpirePerks.Traversal((ExpirePerkBase action) => { action.OnAttack(info); });
            return info;
        }

        public void OnMove(float distance) => m_ExpirePerks.Traversal((ExpirePerkBase perk) => { perk.OnMove(distance); });
        public void OnAbilityTrigger() => m_ExpirePerks.Traversal((ExpirePerkBase perk) => { perk.OnAbilityTrigger(); });
        public void OnLevelFinish() => m_ExpirePerks.Traversal((ExpirePerkBase perk) => { perk.OnLevelFinish(); });

        public void OnEntityActivate(EntityBase targetEntity)
        {
            if (targetEntity.m_ControllType != enum_EntityType.AIWeaponHelper || targetEntity.m_Flag != m_Entity.m_Flag || targetEntity.m_EntityID == m_Entity.m_EntityID)
                return;

            EntityCharacterBase ally = (targetEntity as EntityCharacterBase);
            if (F_AllyHealthMultiplierAdditive > 0)
                ally.m_Health.SetHealthMultiplier(F_AllyHealthMultiplierAdditive);
        }

        public void OnWillDealtDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity)=> m_ExpirePerks.Traversal((ExpirePerkBase action) => { action.OnBeforeDealtDamage(damageEntity, damageInfo); });
        public void OnWillReceiveDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity) => m_ExpirePerks.Traversal((ExpirePerkBase action) => { action.OnBeforeReceiveDamage(damageInfo); });
        public override void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
        {
            base.OnCharacterHealthChange(damageInfo, damageEntity, amountApply);
            
            if (damageEntity.m_EntityID == m_Player.m_EntityID)
            {
                if (amountApply > 0)
                    m_ExpirePerks.Traversal((ExpirePerkBase action) => { action.OnAfterReceiveDamage(damageInfo, amountApply); });
                else
                    m_ExpirePerks.Traversal((ExpirePerkBase action) => { action.OnReceiveHealing(damageInfo, amountApply); });
            }
        }
        #endregion
        #endregion

        public void RefreshEffects() => m_BuffEffects.Traversal((int expire, SFXEffect effect) => { effect.Play(m_Entity); });
        #region CoinInfo
        public bool CanCostCoins(float price)
        {
            return m_Coins >= price* F_CoinsCostMultiply;
        }
        public void OnCoinsCost(float price)
        {
            m_Coins -= price * F_CoinsCostMultiply;
        }
        public void OnCoinsGain(float coinAmount)
        {
            m_Coins += coinAmount;
        }
        #endregion
    }

    #endregion

    #region Expires

    public class EffectCounterBase
    {
        public enum_ExpireRefreshType m_Type { get; private set; }
        public float m_duration { get; private set; }
        public bool m_Effecting => m_duration > 0;
        public EffectCounterBase(enum_ExpireRefreshType type)
        {
            m_Type = type;
            Reset();
        }
        public void OnSet(float _duration)
        {
            if (_duration <= 0)
                return;
            switch (m_Type)
            {
                case enum_ExpireRefreshType.AddUp:
                        m_duration += _duration;
                    break;
                case enum_ExpireRefreshType.Refresh:
                    if (m_duration < _duration)
                        m_duration = _duration;
                    break;
            }
        }
        public void Reset()
        {
            m_duration = 0;
        }
        public void Tick(float deltaTime)
        {
            if (m_duration > 0)
            {
                m_duration -= deltaTime;
                if (m_duration < 0)
                    m_duration = 0;
            }
        }
    }

    public class EntityExpireBase
    {
        public virtual int m_EffectIndex => -1;
        public virtual int m_Index => -1;
        public virtual enum_ExpireType m_ExpireType => enum_ExpireType.Invalid;
        public virtual enum_ExpireRefreshType m_RefreshType => enum_ExpireRefreshType.Invalid;
        public virtual float m_MovementSpeedMultiply => 0;
        public virtual float m_FireRateMultiply => 0;
        public virtual float m_ReloadRateMultiply => 0;
        public virtual float m_HealthDrainMultiply => 0;
        public virtual float m_DamageMultiply => 0;
        public virtual float m_DamageReduction => 0;
        public virtual float m_CriticalChangeAdditive => 0;
        public virtual float m_CriticalHitMultiplyAdditive => 0f;
        public virtual float m_HealAdditive => 0;
        private Action<EntityExpireBase> OnExpired;
        bool forceExpire = false;
        protected void OnActivate( Action<EntityExpireBase> _OnExpired)
        {
            OnExpired = _OnExpired;
            forceExpire = false;
        }

        public void DoExpire() => forceExpire = true;
        public virtual void OnTick(float deltaTime)
        {
            if (forceExpire) OnExpired(this);
        }
        public virtual void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount) { }
    }
    
    public class EntityExpirePreset : EntityExpireBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.Preset;
        public override int m_EffectIndex => m_buffInfo.m_EffectIndex;
        public override enum_ExpireRefreshType m_RefreshType => m_buffInfo.m_AddType;
        public override float m_DamageMultiply => m_buffInfo.m_DamageMultiply;
        public override float m_DamageReduction => m_buffInfo.m_DamageReduction;
        public override int m_Index => m_buffInfo.m_Index;
        public override float m_FireRateMultiply => m_buffInfo.m_FireRateMultiply;
        public override float m_MovementSpeedMultiply => m_buffInfo.m_MovementSpeedMultiply;
        public override float m_ReloadRateMultiply => m_buffInfo.m_ReloadRateMultiply;
        public override float m_HealthDrainMultiply => m_buffInfo.m_HealthDrainMultiply;
        public float m_ExpireDuration { get; private set; } = 0;
        public float f_expireCheck { get; private set; }
        public float f_expireLeftScale => f_expireCheck / m_ExpireDuration;
        public int I_SourceID { get; private set; }
        SBuff m_buffInfo;
        Func<DamageInfo, bool> OnDOTDamage;
        float f_dotCheck;
        public EntityExpirePreset(int sourceID, SBuff _buffInfo, Func<DamageInfo, bool> _OnDOTDamage, Action<EntityExpireBase> _OnExpired)
        {
            base.OnActivate(_OnExpired);
            I_SourceID = sourceID;
            m_buffInfo = _buffInfo;
            OnDOTDamage = _OnDOTDamage;
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
                DoExpire();

            if (m_buffInfo.m_DamageTickTime <= 0)
                return;
            f_dotCheck += deltaTime;
            if (f_dotCheck > m_buffInfo.m_DamageTickTime)
            {
                f_dotCheck -= m_buffInfo.m_DamageTickTime;
                OnDOTDamage(new DamageInfo(I_SourceID,m_buffInfo.m_DamagePerTick, m_buffInfo.m_DamageType));
            }
        }

        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (m_buffInfo.m_ExtraBuffApply <= 0)
                return;
            receiver.m_CharacterInfo.AddBuff(info.m_SourceID, GameDataManager.GetPresetBuff(m_buffInfo.m_ExtraBuffApply));
        }
    }

    public class ExpirePerkBase:EntityExpireBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.Perk;
        public virtual enum_Rarity m_Rarity { get; private set; } = enum_Rarity.Invalid;
        public virtual int m_MaxStack=>-1;
        public virtual bool m_DataHidden => false;

        public int m_Stack { get; private set; } = 0;
        public virtual float m_RecordData { get; protected set; }
        public ExpirePerkBase(PerkSaveData data) { m_Stack = data.m_PerkStack; m_RecordData = data.m_RecordData; }
        
        public EntityCharacterPlayer m_Attacher { get; private set; }
        public virtual void OnActivate(EntityCharacterPlayer _actionEntity, Action<EntityExpireBase> OnExpired) { m_Attacher = _actionEntity; OnActivate(OnExpired); }
        public void OnStackUp() => m_Stack++;

        public virtual float Value1 => 0;
        public virtual float Value2 => 0;
        public virtual float Value3 => 0;
        public virtual float m_DamageAdditive => 0;
        public virtual float F_SpreadReduction => 0;
        public virtual float F_AimPressureReduction => 0;
        public virtual float F_ProjectileSpeedMultiply => 0;
        public virtual bool B_ClipOverride => false;
        public virtual int I_ClipAdditive => 0;
        public virtual float F_ClipMultiply => 0;
        public virtual float F_PenetradeAdditive => 0;
        public virtual float F_AimRangeAdditive => 0;
        public virtual float m_MaxHealthAdditive => 0;
        public virtual float m_MaxArmorAdditive => 0;
        public virtual float F_AllyHealthMultiplierAdditive => 0;
        public virtual float F_Discount => 0f;

        #region Interact
        public virtual void OnBeforeReceiveDamage(DamageInfo info) { }
        public virtual void OnAfterReceiveDamage(DamageInfo info, float amount) { }
        public virtual void OnAttack(DamageInfo info) { }
        public virtual void OnBeforeDealtDamage(EntityCharacterBase receiver, DamageInfo info) { }
        public virtual void OnReceiveHealing(DamageInfo info, float applyAmount) { }
        public virtual void OnMove(float distsance) { }
        public virtual bool OnCheckRevive() { return false; }
        public virtual void OnAbilityTrigger() { }
        public virtual void OnLevelFinish() { }
        #endregion
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
    #endregion
}
