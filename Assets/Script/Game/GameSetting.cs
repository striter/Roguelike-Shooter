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

        public const float F_PlayerWeaponFireReloadPause = 0f; //武器恢复间隔时间
        public const float F_PlayerAutoAimRangeBase = 16f; //自动锁定敌人范围
        public const float F_PlayerDamageAdjustmentRange = .1f;
        public const int I_PlayerRotationSmoothParam = 10;     //Camera Smooth Param For Player 10 is suggested

        public const int I_PlayerReviveBuffIndex = 40004;
        public static readonly RangeInt RI_GameFinalBattleEnermySpawnCheck = new RangeInt(10, 5); //BOSS关小怪刷新检测时间
        public const float F_FinalBattleEnermySpawnEliteHealthScaleOffset = .1f; //BOSS血量减少百分比会判断刷新小怪

        public static readonly List<EliteBuffCombine> L_GameEliteBuff = new List<EliteBuffCombine>() { new EliteBuffCombine(2010, 12010, 32010), new EliteBuffCombine(2020, 12020, 32020), new EliteBuffCombine(2030, 12030, 32030), new EliteBuffCombine(2040, 12040, 32040), new EliteBuffCombine(2050, 12050, 32050), new EliteBuffCombine(2060, 12060, 32060) };
        public const float F_EliteBuffTimerDurationWhenFullHealth=15f; //触发tick值
        public const float F_EliteBuffTimerTickRateMultiplyHealthLoss = 2f; //每秒加几tick=(1+血量损失比例* X)

        public static readonly Dictionary<enum_CampFarmItemStatus, int> DP_FarmGeneratePercentage = new Dictionary<enum_CampFarmItemStatus, int>() { { enum_CampFarmItemStatus.Progress1, 60 }, { enum_CampFarmItemStatus.Progress2, 30 }, { enum_CampFarmItemStatus.Progress3, 6 }, { enum_CampFarmItemStatus.Progress4, 3 }, { enum_CampFarmItemStatus.Progress5, 1 } };   //Farm生成等级百分比
        public static readonly Dictionary<enum_CampFarmItemStatus, float> GetFarmCreditPerSecond = new Dictionary<enum_CampFarmItemStatus, float> { { enum_CampFarmItemStatus.Progress1, .1f / 60f }, { enum_CampFarmItemStatus.Progress2, .2f / 60f }, { enum_CampFarmItemStatus.Progress3, .3f / 60f }, { enum_CampFarmItemStatus.Progress4, .5f / 60f }, { enum_CampFarmItemStatus.Progress5, 1f / 60f } };      //Farm 等级,每秒Credit

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

        public const int I_EnermySpawnDelay = 2;        //Enermy Spawn Delay Time 
        public const float F_EnermySpawnOffsetEach = .5f;       //Enermy Spawn Offset Each

        public const float F_PickupAcceleration = 10f; //拾取物的飞行加速速度
        public const int I_HealthPickupAmount = 25;
        public const int I_ArmorPickupAmount = 25;
        public const int I_HealthPackAmount = 50;

        public static readonly RangeInt IR_EventTradeBuffPrice = new RangeInt(20, 5);
        public static readonly Dictionary<enum_ChunkEventType, int> D_ChunkEventPercentage = new Dictionary<enum_ChunkEventType, int>() { { enum_ChunkEventType.RewardChest, 25 },{ enum_ChunkEventType.WeaponReforge, 25 }, { enum_ChunkEventType.PerkAcquire, 25 }, { enum_ChunkEventType.PerkUpgrade, 25 } };
        public static readonly RangeInt IR_EventMedicPrice = new RangeInt(10, 5);
        public const int I_EventEquipmentTradePrice = 10;
        public const int I_EventPerkAcquireTryCount = 3;
        public static readonly Dictionary<enum_WeaponRarity, RangeInt> D_EventWeaponTradePrice = new Dictionary<enum_WeaponRarity, RangeInt>() { { enum_WeaponRarity.Ordinary, new RangeInt(5, 5) }, { enum_WeaponRarity.Advanced, new RangeInt(10, 5) }, { enum_WeaponRarity.Rare, new RangeInt(20, 5) }, { enum_WeaponRarity.Legend, new RangeInt(30, 5) } };
        public static readonly Dictionary<enum_WeaponRarity, int> D_EventWeaponReforgeRate = new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 25 }, { enum_WeaponRarity.Advanced, 25 }, { enum_WeaponRarity.Rare, 25 }, { enum_WeaponRarity.Legend, 25 } };


        public const int I_EquipmentSlotTradePricePerPlayerSlots = 20;
        
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
        public static int GetPlayerWeaponIndex(int weaponIndex) =>weaponIndex * 10;
        public static int GetPlayerEquipmentWeaponIndex(int equipmentIndex) => 100000 + equipmentIndex * 10;
        public static int GetAIWeaponIndex(int entityIndex, int weaponIndex = 0, int subWeaponIndex = 0) => entityIndex * 100 + weaponIndex * 10 + subWeaponIndex;
        public static int GetWeaponSubIndex(int weaponIndex) => weaponIndex + 1;

        public static float F_PlayerSensitive(int sensitiveTap) => sensitiveTap / 5f;
        public static float F_GameVFXVolume(int vfxVolumeTap) => vfxVolumeTap / 10f;
        public static float F_GameMusicVolume(int musicVolumeTap) => musicVolumeTap / 10f;

        public static bool B_ShowHitMark(enum_HitCheck check) => check != enum_HitCheck.Invalid;

        public static float F_SphereCastDamageReduction(float weaponDamage, float distance, float radius) => weaponDamage * (1 - (distance / radius));       //Rocket Blast Damage

        public static SBuff GetEnermyGameBuff(enum_StageLevel stage,int difficulty) => SBuff.CreateGameEnermyBuff(difficulty, ((int)stage - 1) * .2f+ (difficulty - 1)*.2f );
        public static float GetEnermyMaxHealthMultiplier(enum_StageLevel stage, int difficulty) => 1f + ((int)stage - 1) * .2f + (difficulty - 1) * .05f;

        public static int GetRankupExp(int rank) => 100 + 10* rank;
        public static int GetEnermyKillExp(bool isElite,enum_StageLevel stage)
        {
            if (!isElite)
                return 25;
            switch (stage)
            {
                default: Debug.LogError("Invalid Convertions Here!") ; return 0;
                case enum_StageLevel.Rookie: return 200;
                case enum_StageLevel.Veteran: return 300;
                case enum_StageLevel.Ranger: return 400;
            }
        }

        public static float GetResultCompletion(bool win, enum_StageLevel _stage, int _battleLevelEntered) => win ? 1f : (.33f * ((int)_stage - 1) +.066f*_battleLevelEntered);
        public static float GetResultLevelScore(enum_StageLevel _stage, int _levelPassed) => 200 * ((int)_stage - 1) + 20 * (_levelPassed - 1);
        public static float GetResultDifficultyBonus(int _difficulty) =>1f+ _difficulty * .05f;
        public static float GetResultRewardCredits(float _totalScore) => _totalScore;

        public static RangeInt GetEventTradePrice(enum_Interaction interactType, enum_EquipmentRarity actionRarity= enum_EquipmentRarity.Invalid,enum_WeaponRarity weaponRarity= enum_WeaponRarity.Invalid)
        {
            switch (interactType)
            {
                default: Debug.LogError("No Coins Can Phrase Here!"); return new RangeInt(0, -1);
                case enum_Interaction.PickupHealth:
                    return new RangeInt(10, 0);
                case enum_Interaction.Equipment:
                    switch (actionRarity)
                    {
                        default: Debug.LogError("Invalid Level!"); return new RangeInt(0, -1);
                        case enum_EquipmentRarity.Normal: return new RangeInt(8, 4);
                        case enum_EquipmentRarity.OutStanding: return new RangeInt(16, 8);
                        case enum_EquipmentRarity.Epic: return new RangeInt(24, 12);
                    }
                case enum_Interaction.Weapon:
                    switch (weaponRarity)
                    {
                        default:Debug.LogError("Invalid Weapon Rarity");return new RangeInt(0, -1);
                        case enum_WeaponRarity.Ordinary:
                            return new RangeInt(1, 5);
                        case enum_WeaponRarity.Advanced:
                            return new RangeInt(8, 4);
                        case enum_WeaponRarity.Rare:
                            return new RangeInt(16, 8);
                        case enum_WeaponRarity.Legend:
                            return new RangeInt(24,12);
                    }
            }
        }
        public static int GetEventPerkAcquireSuccessRate(int tryCount) => 20 * (tryCount + 1);
        public static int GetEventPerkAcquireCoinsAmount(int tryCount) => 5 * (tryCount + 1);


        public static float GetLevelObjectHealth(enum_TileObjectType objectType)
        {
            switch(objectType)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return -1;
                case enum_TileObjectType.Main:
                case enum_TileObjectType.Sub1:
                case enum_TileObjectType.Sub2:
                    return 1000;
                case enum_TileObjectType.SubUnbreakable:
                    return 2000;
                case enum_TileObjectType.SubBreakable:
                    return 10;
                case enum_TileObjectType.Object1x1A:
                case enum_TileObjectType.Object1x1B:
                case enum_TileObjectType.Object1x1C:
                case enum_TileObjectType.Object1x1D:
                    return 100;
                case enum_TileObjectType.Object2x1A:
                case enum_TileObjectType.Object2x1B:
                    return 200;
                case enum_TileObjectType.Object2x2A:
                case enum_TileObjectType.Object2x2B:
                    return 400;
                case enum_TileObjectType.Object3x2A:
                case enum_TileObjectType.Object3x2B:
                    return 600;
                case enum_TileObjectType.Object3x3A:
                case enum_TileObjectType.Object3x3B:
                    return 900;
            }
        }

        public static int GetActionRemovePrice(enum_StageLevel stage, int removeTimes) => 10 * (removeTimes + 1) ;
        public static int GetActionUpgradePrice(enum_StageLevel stage, int upgradeTimes) => 10 * (upgradeTimes + 1) ;
        public static StageInteractGenerateData GetInteractGenerate(enum_StageLevel level)
        {
            switch (level)
            {
                default: return new StageInteractGenerateData();
                case enum_StageLevel.Rookie:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_EquipmentRarity, int>() { { enum_EquipmentRarity.Normal, 80 }, { enum_EquipmentRarity.OutStanding, 15 }, { enum_EquipmentRarity.Epic,5 } },     //Trade Equipment
                        new Dictionary<enum_EquipmentRarity, int>() { { enum_EquipmentRarity.Normal, 50 }, { enum_EquipmentRarity.OutStanding, 30 }, { enum_EquipmentRarity.Epic, 20 } },    //Rankup Equipment
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 100 }, { enum_WeaponRarity.Rare, 0 }, { enum_WeaponRarity.Legend, 0 } },        //Trade Weapon
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 50 }, { enum_WeaponRarity.Advanced, 50 }, { enum_WeaponRarity.Rare, 0 }, { enum_WeaponRarity.Legend, 0 } },        //Reward Weapon
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),       //Normal Pickups
                        new Dictionary<enum_WeaponRarity, float> {{ enum_WeaponRarity.Ordinary, 6 },{ enum_WeaponRarity.Advanced,3} }),          //Normal Weapon Pikcups
                        PickupGenerateData.Create(10, 100, 50, new RangeInt(10, 5),     //Elite Pickups
                        new Dictionary<enum_WeaponRarity, float> { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 100 } })        //Elite Weapon Pickups
                        );
                case enum_StageLevel.Veteran:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_EquipmentRarity, int>() { { enum_EquipmentRarity.Normal, 50 }, { enum_EquipmentRarity.OutStanding, 45 }, { enum_EquipmentRarity.Epic, 5 } },
                        new Dictionary<enum_EquipmentRarity, int>() { { enum_EquipmentRarity.Normal, 0 }, { enum_EquipmentRarity.OutStanding, 100 }, { enum_EquipmentRarity.Epic, 0 } },
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 0 }, { enum_WeaponRarity.Rare, 100 }, { enum_WeaponRarity.Legend, 0 } },
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced,50 }, { enum_WeaponRarity.Rare, 50 }, { enum_WeaponRarity.Legend, 0 } },
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),
                        new Dictionary<enum_WeaponRarity, float> {{ enum_WeaponRarity.Advanced, 3.8f },{ enum_WeaponRarity.Rare,1.9f} }),
                        PickupGenerateData.Create(10, 100, 100, new RangeInt(10, 5),
                        new Dictionary<enum_WeaponRarity, float> { { enum_WeaponRarity.Rare, 100 } })
                        );
                case enum_StageLevel.Ranger:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_EquipmentRarity, int>() { { enum_EquipmentRarity.Normal,5} , { enum_EquipmentRarity.OutStanding,65 } , { enum_EquipmentRarity.Epic, 30 } },
                        new Dictionary<enum_EquipmentRarity, int>() { { enum_EquipmentRarity.Normal, 0 }, { enum_EquipmentRarity.OutStanding, 0 }, { enum_EquipmentRarity.Epic, 100 } },
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 0 }, { enum_WeaponRarity.Rare, 0 }, { enum_WeaponRarity.Legend, 100 } },
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 0 }, { enum_WeaponRarity.Rare, 50 }, { enum_WeaponRarity.Legend, 50 } },
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),
                        new Dictionary<enum_WeaponRarity, float> {{ enum_WeaponRarity.Rare, 2.8f },{ enum_WeaponRarity.Legend, 2.8f} }),
                        PickupGenerateData.Create(0, 0, 0, new RangeInt(10, 5),
                        new Dictionary<enum_WeaponRarity, float> { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 0 } })
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

        public static List<int> GetEnermyIDList(this SEnermyGenerate enermyGenerate,Dictionary<enum_EnermyType,List<int>> m_EnermyIDs,System.Random random)
        {
            List<int> enermyID = new List<int>();
            for (int i = 0; i < enermyGenerate.m_FighterCount; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.Fighter].RandomItem(random));
            for (int i = 0; i < enermyGenerate.m_ShooterRCount; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.Shooter_Rookie].RandomItem(random));
            for (int i = 0; i < enermyGenerate.m_ShooterVCount; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.Shooter_Veteran].RandomItem(random));
            for (int i = 0; i < enermyGenerate.m_CasterCount; i++)
                enermyID.Add(m_EnermyIDs[enum_EnermyType.AOECaster].RandomItem(random));
            return enermyID;
        }
    }

    public static class UIConst
    {
        public const int I_AmmoCountToSlider = 200;      //Ammo UI,While Clip Above This Will Turn To Be Slider大于此值变成白条
        public const int I_PlayerDyingMinValue = 10;
        public const int I_PlayerDyingMaxValue = 50;
        
        public const float F_MapAnimateTime = 1.6f;

        public const float F_UIDamageStartOffset = 20f; //血显示区域范围

        public const float F_UIActionBattlePressDuration = .3f; //查看卡片说明长按时间
    }

    public static class UIExpression
    {
        public static Color TipsColor(this enum_UITipsType type)
        {
            switch(type)
            {
                case enum_UITipsType.Normal:
                    return Color.green;
                case enum_UITipsType.Warning:
                    return Color.yellow;
                case enum_UITipsType.Error:
                    return Color.white;
                default:
                    return Color.magenta;
            }
        }

        public static float GetUIWeaponDamageValue(float uiDamage) => Mathf.InverseLerp(0,100,uiDamage);   //武器数据查看ui标准
        public static float GetUIWeaponRPMValue(float uiRPM) =>  Mathf.InverseLerp(0,400,uiRPM);
        public static float GetUIWeaponStabilityValue(float uiStability) => Mathf.InverseLerp(0, 100, uiStability);
        public static float GetUIWeaponSpeedValue(float uiSpeed) =>  Mathf.InverseLerp(0, 100, uiSpeed);
        public static float GetUIDamageScale(float damage) => ((damage / 50 / 10 ) + .9f )/ 2;  //伤害显示比例缩放，默认是两倍大小
    }

    public static class GameEnumConvertions
    {
        public static enum_EquipmentRarity ToRarity(this enum_StageLevel stageLevel) => (enum_EquipmentRarity)stageLevel;
        public static int ToLayer(this enum_HitCheck layerType)
        {
            switch (layerType)
            {
                default: Debug.LogError("Null Layer Can Be Transferd From:" + layerType.ToString()); return 0;
                case enum_HitCheck.Entity: return GameLayer.I_Entity;
                case enum_HitCheck.Static: return GameLayer.I_Static;
                case enum_HitCheck.Dynamic: return GameLayer.I_Dynamic;
                case enum_HitCheck.Interact: return GameLayer.I_Interact;
            }
        }
    }

    public static class UIConvertions
    {
        public static string GetInteractIcon(this enum_Interaction type)
        {
            switch(type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return "";
                case enum_Interaction.Portal:
                    return "InteractIcon_" + type;
            }
        }
        public static string GetNumericVisualizeIcon(this enum_Interaction type)
        {
            switch (type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return "";
                case enum_Interaction.PickupCoin:
                    return "NumericIcon_Coin";
                case enum_Interaction.PickupArmor:
                    return "NumericIcon_Armor";
                case enum_Interaction.PickupHealth:
                case enum_Interaction.PickupHealthPack:
                    return "NumericIcon_Health";
            }
        } 
        public static Color GetVisualizeAmountColor(this enum_Interaction type)
        {
            switch (type)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return Color.magenta;
                case enum_Interaction.PickupCoin:
                    return TCommon.GetHexColor("FFCC1FFF");
                case enum_Interaction.PickupArmor:
                    return TCommon.GetHexColor("1FF2FFFF");
                case enum_Interaction.PickupHealth:
                case enum_Interaction.PickupHealthPack:
                    return TCommon.GetHexColor("FFA54EFF");
            }
        }

        public static string GetMapEventSprite(this enum_ChunkEventType type) => "map_icon_event_" + type;

        public static string GetAbilityBackground(bool cooldowning)=>cooldowning?"control_ability_bottom_cooldown":"control_ability_bottom_activate";
        public static string GetAbilitySprite(enum_PlayerCharacter character) => "control_ability_" + character;
        public static string GetSpriteName(this enum_PlayerWeapon weapon) => ((int)weapon).ToString();

        public static string GetUIInteractBackground(this enum_WeaponRarity rarity) => "interact_" + rarity;
        public static string GetUIStatusShadowBackground(this enum_WeaponRarity rarity) => "weapon_shadow_" + rarity;
        public static string GetUIGameControlBackground(this enum_WeaponRarity rarity) => "control_" + rarity;
        public static string GetUITextColor(this enum_WeaponRarity rarity)
        {
            switch (rarity)
            {
                default: return "FFFFFFFF";
                case enum_WeaponRarity.Ordinary: return "E3E3E3FF";
                case enum_WeaponRarity.Advanced: return "6F8AFFFF";
                case enum_WeaponRarity.Rare: return "C26FFFFF";
                case enum_WeaponRarity.Legend: return "FFCC1FFF";
            }
        }

        public static string GetUIGameResultTitleBG(bool win, enum_Option_LanguageRegion language) => "result_title_" + (win ? "win_" : "fail_") + language;
    }

    public static class LocalizationKeyJoint
    {
        public static string GetNameLocalizeKey(this EntityExpirepRreset buff) => "Buff_Name_" + buff.m_Index;
        public static string GetNameLocalizeKey(this ActionPerkBase action) => "Action_Name_" + action.m_Index;
        public static string GetIntroLocalizeKey(this ActionPerkBase action) => "Action_Intro_" + action.m_Index;
        public static string GetLocalizeKey(this enum_StageLevel stage) => "Game_Stage_" + stage;
        public static string GetLocalizeKey(this enum_LevelStyle style) => "Game_Style_" + style;
        public static string GetMapLocalizeNameKey(this enum_ChunkEventType type) => "UI_Map_" + type + "_Name";
        public static string GetMapLocalizeIntroKey(this enum_ChunkEventType type) => "UI_Map_" + type + "_Intro";
        public static string GetLocalizeNameKey(this enum_PlayerWeapon weapon) => "Weapon_Name_" + weapon;
        public static string GetTitleLocalizeKey(this InteractBase interact) => "UI_Interact_" + interact.m_InteractType+interact.m_ExternalLocalizeKeyJoint;
        public static string GetBottomLocalizeKey(this InteractBase interact) => "UI_Interact_" + interact.m_InteractType + interact.m_ExternalLocalizeKeyJoint + "_Bottom";
        public static string GetIntroLocalizeKey(this InteractBase interact) => "UI_Interact_" + interact.m_InteractType +interact.m_ExternalLocalizeKeyJoint+ "_Intro";
        public static string GetLocalizeKey(this enum_EquipmentRarity rarity) => "UI_Rarity_" + rarity;
        public static string GetLocalizeKey(this enum_Option_FrameRate frameRate) => "UI_Option_" + frameRate;
        public static string GetLocalizeKey(this enum_Option_JoyStickMode joystick) => "UI_Option_" + joystick;
        public static string GetLocalizeKey(this enum_Option_LanguageRegion region) => "UI_Option_" + region;
        public static string GetLocalizeKey(this enum_CampFarmItemStatus status) => "UI_Farm_" + status;
        public static string SetActionIntro(this ActionPerkBase actionInfo, UIT_TextExtend text) => text.formatText(actionInfo.GetIntroLocalizeKey() , actionInfo.Value1, actionInfo.Value2, actionInfo.Value3);
    }

    public static class GameLayer
    {
        public static readonly int I_Static = LayerMask.NameToLayer("static");
        public static readonly int I_Entity = LayerMask.NameToLayer("entity");
        public static readonly int I_Dynamic = LayerMask.NameToLayer("dynamic");
        public static readonly int I_Interact = LayerMask.NameToLayer("interact");
        public static class Mask
        {
            public static readonly int I_All = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity | 1 << GameLayer.I_Dynamic;
            public static readonly int I_StaticEntity = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity;
            public static readonly int I_Entity = (1 << GameLayer.I_Entity);
            public static readonly int I_Interact = (1 << GameLayer.I_Interact);
            public static readonly int I_Static = (1 << GameLayer.I_Static);
        }

        public static readonly int I_EntityDetect = LayerMask.NameToLayer("entityDetect");
        public static readonly int I_InteractDetect = LayerMask.NameToLayer("interactDetect");
        public static readonly int I_MovementDetect = LayerMask.NameToLayer("movementDetect");
    }
    #endregion

    #region For Developers Use

    #region BroadCastEnum
    enum enum_BC_GameStatus
    {
        Invalid = -1,

        OnEntityActivate,
        OnEntityRecycle,

        OnCharacterHealthWillChange,
        OnCharacterHealthChange,
        OnCharacterDead,
        OnCharacterRevive,

        OnPlayerRankUp,
        
        OnBattleStart,
        OnBattleFinish,
        OnFinalBattleStart,
        OnFinalBattleFinish,

        OnStageFinished,

        OnGameLoad,
        OnGameBegin,
        OnGameFinish,
        OnGameExit,

        OnCampStart,
    }

    enum enum_BC_UIStatus
    {
        Invalid = -1,
        UI_PlayerCommonStatus,
        UI_PlayerInteractStatus,
        UI_PlayerInteractPickup, 
        UI_PlayerHealthStatus,
        UI_PlayerBuffStatus,
        UI_PlayerPerkStatus,
        UI_PlayerWeaponStatus,

        UI_ChunkTeleportUnlock,

        UI_CampDataStatus,

        UI_PageOpen,
        UI_PageClose,
    }
    #endregion

    #region GameEnum
    public enum enum_StageLevel { Invalid = -1, Rookie = 1, Veteran = 2, Ranger = 3 }
    
    public enum enum_EntityController { Invalid = -1, None = 1, Player = 2, AI = 3, Device = 4, }

    public enum enum_EntityFlag { Invalid = -1, None = 0, Player = 1, Enermy = 2, Neutal = 3, }

    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, Interact = 4, }
    
    public enum enum_LevelStyle { Invalid = -1, Forest = 1, Desert = 2, Frost = 3, Horde = 4, Undead = 5, }

    public enum enum_EnermyType { Invalid = -1, Fighter = 1, Shooter_Rookie = 2, Shooter_Veteran = 3, AOECaster = 4, Elite = 5, }

    public enum enum_Interaction { Invalid = -1,
        GameBegin,Bonfire, TradeContainer, PickupCoin, PickupHealth,PickupHealthPack,PickupExp, PickupArmor,RewardChest,PerkUpgrade,PerkAcquire,WeaponReforge, Equipment, Weapon,Teleport, Portal, GameEnd,
        CampBegin,CampStage, CampDifficult,CampFarm,CampAction,CampEnd, }
    
    public enum enum_ProjectileFireType { Invalid = -1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, }

    public enum enum_CastTarget { Invalid=-1,Head=1,Weapon=2,Feet=3}

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, ForwardTrapezium = 4, }

    public enum enum_HealthChangeMessage { Invalid = -1, Default = 0, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Basic = 1, ArmorOnly = 2, HealthOnly = 3,}

    public enum enum_CharacterEffect { Invalid = -1, Freeze = 1, Cloak = 2, Scan = 3, }

    public enum enum_ExpireType { Invalid = -1, PresetBuff = 1,ActionBuff=2, ActionPerk = 3, }

    public enum enum_ExpireRefreshType { Invalid = -1, AddUp = 1, Refresh = 2,RefreshIdentity=3, }

    public enum enum_EquipmentRarity { Invalid = -1, Normal = 1, OutStanding = 2, Epic = 3, }

    public enum enum_EffectAttach { Invalid = -1,  Head = 1, Feet = 2, WeaponModel = 3,}

    public enum enum_PlayerCharacter {Invalid=-1,Beth=10001, }

    public enum enum_InteractCharacter { Invalid=-1,Trader=20001,Trainer=20002, }

    public enum enum_WeaponRarity { Invalid = -1, Ordinary = 1, Advanced = 2, Rare = 3, Legend = 4 }

    public enum enum_PlayerWeapon
    {
        Invalid = -1,
        //Laser
        RailPistol = 101,
        Railgun = 102,
        //Snipe Rifle
        M82A1 = 201,
        Kar98 = 202,
        //Submachine Gun
        UZI = 301,
        UMP45 = 302,
        //Assult Rifle
        SCAR = 401,
        M16A4 = 402,
        AKM = 403,
        //Pistol
        P92 = 501,
        DE = 502,
        //Shotgun
        XM1014 = 601,
        S686 = 602,
        //Heavy Weapon
        Crossbow = 701,
        RocketLauncher = 702,
        Minigun = 703,
        //Special
        Flamer = 801,
        Driller = 802,
        Bouncer = 803,
        Tesla=804,
    }

    public enum enum_PlayerAnim
    {
        Invalid = -1,
        Rifle_1001 = 1001,
        Pistol_1002 = 1002,
        Crossbow_1003 = 1003,
        Heavy_1004 = 1004,
    }

    public enum enum_EnermyAnim
    {
        Invalid = -1,
        Axe_Dual_Pound_10 = 10,
        Spear_R_Stick_20 = 20,
        Sword_R_Swipe_30 = 30,
        Sword_R_Slash_31 = 31,
        Dagger_Dual_Twice_40 = 40,
        Staff_L_Cast_110 = 110,
        Staff_Dual_Cast_111 = 111,
        Staff_R_Cast_Loop_112 = 112,
        Staff_R_Cast_113 = 113,
        Bow_Shoot_130 = 130,
        CrossBow_Shoot_131 = 131,
        Bow_UpShoot_133 = 133,
        Rifle_HipShot_140 = 140,
        Rifle_AimShot_141 = 141,
        Throwable_Hips_150 = 150,
        Throwable_R_ThrowHip_151 = 151,
        Throwable_R_ThrowBack_152 = 152,
        Throwable_R_Summon_153 = 153,
        Heavy_HipShot_161 = 161,
        Heavy_Mortal_162 = 162,
        Heavy_Shield_Spear_163 = 163,
        Heavy_Remote_164 = 164,
    }

    public enum enum_GameVFX
    {
        Invalid=-1,

        EntityDamage,
        PlayerDamage,

        PlayerRevive,
    }

    public enum enum_GameMusic { Invalid=-1,StyledStart=1,  Relax,StyledEnd=10, Fight}

    public enum enum_CampMusic { Invalid=-1, Relax = 0,}

    public enum enum_Option_FrameRate { Invalid = -1, Normal = 45, High = 60, }

    public enum enum_Option_ScreenEffect { Invalid=-1,Normal,High}

    public enum enum_CampFarmItemStatus { Invalid=-1, Empty = 0, Locked=1 , Decayed = 2, Progress1=10,Progress2,Progress3,Progress4,Progress5}

    public enum enum_UITipsType { Invalid=-1,Normal=0,Warning=1,Error=2}

    #endregion

    #region Structs
    #region Default Readonly
    public struct PickupGenerateData
    {
        public int m_HealthRate { get; private set; }
        public int m_ArmorRate { get; private set; }
        public int m_CoinRate { get; private set; }
        public RangeInt m_CoinRange { get; private set; }
        public Dictionary<enum_WeaponRarity, float> m_WeaponRate { get; private set; }

        public bool CanGenerateHealth() => TCommon.RandomPercentage() <= m_HealthRate;
        public bool CanGenerateArmor() => TCommon.RandomPercentage() <= m_ArmorRate;
        public bool CanGenerateCoins(float baseCreditRate, out int amount)
        {
            amount = -1;
            if (TCommon.RandomPercentage() <= (baseCreditRate + m_CoinRate))
            {
                amount = m_CoinRange.Random();
                return true;
            }
            return false;
        }
        public static PickupGenerateData Create(int healthRate, int armorRate, int coinRate, RangeInt coinAmount, Dictionary<enum_WeaponRarity, float> _weaponRate) => new PickupGenerateData() { m_HealthRate = healthRate, m_ArmorRate = armorRate, m_CoinRate = coinRate, m_CoinRange = coinAmount, m_WeaponRate=_weaponRate };
    }

    public struct StageInteractGenerateData
    {
        public Dictionary<enum_EquipmentRarity,int> m_EventEquipment { get; private set; }
        public Dictionary<enum_EquipmentRarity, int> m_RankupEquipment { get; private set; }
        public Dictionary<enum_WeaponRarity, int> m_TradeWeapon { get; private set; }
        public Dictionary<enum_WeaponRarity, int> m_RewardWeapon { get; private set; }
        public PickupGenerateData m_NormalPickupData { get; private set; }
        public PickupGenerateData m_ElitePickupData { get; private set; }
        public static StageInteractGenerateData Create(Dictionary<enum_EquipmentRarity, int>  _tradeEquipmentRate, Dictionary<enum_EquipmentRarity, int> _rankupEquipment, Dictionary<enum_WeaponRarity,int> _tradeWeaponRate, Dictionary<enum_WeaponRarity, int> _rewardWeaponRate, PickupGenerateData _normalGenerate,PickupGenerateData _eliteGenerate) => new StageInteractGenerateData() {m_EventEquipment=_tradeEquipmentRate,m_RankupEquipment= _rankupEquipment, m_TradeWeapon=_tradeWeaponRate, m_RewardWeapon = _rewardWeaponRate, m_NormalPickupData=_normalGenerate,m_ElitePickupData=_eliteGenerate};
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

        public void DataRecorrect()
        {
        }

        public void UnlockDifficulty()
        {
            if (m_GameDifficulty != m_DifficultyUnlocked)
                return;

            m_DifficultyUnlocked++;
            m_GameDifficulty++;
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
        public enum_StageLevel m_Stage;
        public float m_Coins;
        public int m_Exp;
        public int m_Rank;
        public List<EquipmentSaveData> m_Equipments;
        public List<ActionSaveData> m_ActionBuffs;
        public float m_Health;
        public float m_Armor;
        public WeaponSaveData m_weapon1, m_weapon2;
        public bool m_weaponEquipingFirst;
        public enum_PlayerCharacter m_character;
        public CBattleSave()
        {
            m_Coins = 0;
            m_Exp = 0;
            m_Rank = 0;
            m_Health = -1;
            m_Armor = -1;
            m_Equipments = new List<EquipmentSaveData>();
            m_ActionBuffs = new List<ActionSaveData>();
            m_Stage = enum_StageLevel.Rookie;
            m_GameSeed = DateTime.Now.ToLongTimeString();
            m_character = GameDataManager.m_GameData.m_CharacterSelected;
            m_weapon1 = WeaponSaveData.CreateNew(enum_PlayerWeapon.P92);
            m_weapon2 = WeaponSaveData.CreateNew(enum_PlayerWeapon.Invalid);
            m_weaponEquipingFirst = true;
        }
        public void Adjust(EntityCharacterPlayer _player, GameProgressManager _level)
        {
            m_Coins = _player.m_CharacterInfo.m_Coins;
            m_Exp = _player.m_CharacterInfo.m_Exp;
            m_Rank = _player.m_CharacterInfo.m_Rank;
            m_Health = _player.m_Health.m_CurrentHealth;
            m_Armor = _player.m_Health.m_CurrentArmor;
            m_weapon1 = WeaponSaveData.Create(_player.m_Weapon1);
            m_weapon2 = WeaponSaveData.Create(_player.m_Weapon2);
            m_weaponEquipingFirst = _player.m_weaponEquipingFirst;
            m_Equipments = EquipmentSaveData.Create(_player.m_CharacterInfo.m_ExpirePerks);
            m_ActionBuffs = ActionSaveData.Create(_player.m_CharacterInfo.m_ExpireBuffs);
            m_GameSeed = _level.m_GameSeed;
            m_Stage = _level.m_GameStage;
        }

        void ISave.DataRecorrect()
        {
        }
    }

    public class CGameOptions : ISave
    {
        public enum_Option_JoyStickMode m_JoyStickMode;
        public enum_Option_FrameRate m_FrameRate;
        public enum_Option_ScreenEffect m_ScreenEffect;
        public enum_Option_LanguageRegion m_Region;
        public int m_MusicVolumeTap;
        public int m_VFXVolumeTap;
        public int m_SensitiveTap;

        public CGameOptions()
        {
            m_JoyStickMode = enum_Option_JoyStickMode.Retarget;
            m_FrameRate = enum_Option_FrameRate.High;
            m_ScreenEffect =  enum_Option_ScreenEffect.High;
            m_Region = enum_Option_LanguageRegion.CN;
            m_SensitiveTap = 5;
            m_MusicVolumeTap = 10;
            m_VFXVolumeTap = 10;
        }


        void ISave.DataRecorrect()
        {
        }
    }
    
    public struct ActionSaveData:IXmlPhrase
    {
        public int m_Index { get; private set; }
        public float m_RecordData { get; private set; }
        public string ToXMLData() => m_Index +"," + m_RecordData;
        public ActionSaveData(string xmlData)
        {
            string[] split = xmlData.Split(',');
            m_Index = int.Parse(split[0]);
            m_RecordData = float.Parse(split[1]);
        }
        public static ActionSaveData Default(int index) => new ActionSaveData() { m_Index = index, m_RecordData = -1 };
        public static ActionSaveData Create(ActionBase action) => new ActionSaveData() { m_Index = action.m_Index, m_RecordData = action.m_RecordData };
        public static List<ActionSaveData> Create(List<ActionBase> actions)
        {
            List<ActionSaveData> data = new List<ActionSaveData>();
            actions.Traversal((ActionBase equipment) => { data.Add(Create(equipment)); });
            return data;
        }
        public static List<ActionSaveData> Create(List<ActionBuffBase> actions)
        {
            List<ActionSaveData> data = new List<ActionSaveData>();
            actions.Traversal((ActionBuffBase equipment) => { data.Add(Create(equipment)); });
            return data;
        }
    }

    public struct EquipmentSaveData : IXmlPhrase
    {
        public ActionSaveData m_ActionData { get; private set; }
        public enum_EquipmentRarity m_Rarity { get; private set; }

        public string ToXMLData() =>m_ActionData.ToXMLData()+"," + m_Rarity;
        public EquipmentSaveData(string xmlData)
        {
            string[] split = xmlData.Split(',');
            m_ActionData = new ActionSaveData(xmlData);
            m_Rarity = (enum_EquipmentRarity)Enum.Parse(typeof(enum_EquipmentRarity), split[2]);
        }

        public static EquipmentSaveData Default(int index, enum_EquipmentRarity rarity) => new EquipmentSaveData {m_ActionData=ActionSaveData.Default(index),m_Rarity=rarity };
        public static EquipmentSaveData Create(ActionPerkBase equipment) =>  new EquipmentSaveData { m_ActionData=ActionSaveData.Create(equipment), m_Rarity=equipment.m_Rarity};
        public static List<EquipmentSaveData> Create(List<ActionPerkBase> equipments)
        {
            List<EquipmentSaveData> data = new List<EquipmentSaveData>();
            equipments.Traversal((ActionPerkBase equipment) => { data.Add(Create(equipment)); });
            return data;
        }
    }


    public struct WeaponSaveData : IXmlPhrase
    {
        public enum_PlayerWeapon m_Weapon { get; private set; }
        public string ToXMLData() => m_Weapon.ToString();
        public WeaponSaveData(string xmlData)
        {
            m_Weapon = (enum_PlayerWeapon)Enum.Parse(typeof(enum_PlayerWeapon), xmlData);
        }
        public static WeaponSaveData Create(WeaponBase weapon) => new WeaponSaveData() { m_Weapon =weapon!=null? weapon.m_WeaponInfo.m_Weapon: enum_PlayerWeapon.Invalid};
        public static WeaponSaveData CreateNew(enum_PlayerWeapon weapon) => new WeaponSaveData() { m_Weapon = weapon };
    }


    public struct CampFarmPlotData : IXmlPhrase
    {
        public int m_StartStamp { get; private set; }
        public enum_CampFarmItemStatus m_Status { get; private set; }
        public string ToXMLData() => m_StartStamp.ToString() + "," + m_Status.ToString();
        public CampFarmPlotData(string xmlData)
        {
            string[] split = xmlData.Split(',');
            m_StartStamp = int.Parse(split[0]);
            m_Status = (enum_CampFarmItemStatus)Enum.Parse(typeof(enum_CampFarmItemStatus), split[1]);
        }

        public static CampFarmPlotData Create(enum_CampFarmItemStatus _status) => new CampFarmPlotData { m_StartStamp = -1, m_Status = _status };
        public static CampFarmPlotData SaveData(CampFarmPlot _plot) => new CampFarmPlotData { m_StartStamp = _plot.m_StartStamp, m_Status = _plot.m_Status };
    }
    #endregion

    #region ExcelData
    public struct SWeapon : ISExcel
    {
        int index;
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
        public enum_PlayerWeapon m_Weapon => (enum_PlayerWeapon)index;
        public enum_WeaponRarity m_Rarity => (enum_WeaponRarity)i_rarity;
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
        //100-999
        public static SBuff SystemSubEntityDOTInfo(float damageTickTime, float damagePerTick) => new SBuff(){ index = 100,i_addType = (int)enum_ExpireRefreshType.Refresh,f_expireDuration = 0,f_damageTickTime = damageTickTime,f_damagePerTick = damagePerTick, i_damageType = (int)enum_DamageType.Basic};
        public static readonly SBuff m_PlayerReviveBuff = new SBuff() { index = 101,i_effect= GameConst.I_PlayerReviveBuffIndex, i_addType = (int)enum_ExpireRefreshType.Refresh, f_expireDuration = GameConst.F_PlayerReviveBuffDuration, f_damageReduce = 1f };
        public static readonly SBuff m_PlayerBoneFireHealBuff = new SBuff() { index = 102, i_addType = (int)enum_ExpireRefreshType.Refresh, f_expireDuration = 5f, f_damageTickTime = .1f, f_damagePerTick = -1f, i_damageType = (int)enum_DamageType.HealthOnly };
        public static readonly SBuff m_EnermyOuttaBattleDamageReduction = new SBuff() { index = 103, i_effect = GameConst.I_PlayerReviveBuffIndex, i_addType = (int)enum_ExpireRefreshType.Refresh,f_expireDuration=.5f, f_damageReduce = 1f };
        //1000-9999
        public static SBuff CreateGameEnermyBuff(int difficulty, float damageMultiply)
        {
            SBuff buff = new SBuff();
            buff.index = 1000 + difficulty;
            buff.i_addType = (int)enum_ExpireRefreshType.Refresh;
            buff.f_damageMultiply = damageMultiply;
            return buff;
        }
        //100000-999999
        public static SBuff CreateMovementActionBuff(int actionIndex,int effectIndex, float movementMultiply, float duration)
        {
            SBuff buff = new SBuff();
            buff.index = actionIndex * 10;
            buff.i_effect = effectIndex;
            buff.i_addType = (int)enum_ExpireRefreshType.Refresh;
            buff.f_movementSpeedMultiply = movementMultiply;
            buff.f_expireDuration = duration;
            return buff;
        }
        public static SBuff CreateActionHealthBuff(int actionIndex, float healthPerTick, float healthTick, float duration)
        {
            SBuff buff = new SBuff();
            buff.index = actionIndex * 10;
            buff.i_effect = 40008;
            buff.i_addType = (int)enum_ExpireRefreshType.RefreshIdentity;
            buff.i_damageType = (int)enum_DamageType.HealthOnly;
            buff.f_damageTickTime = healthTick;
            buff.f_damagePerTick = -healthPerTick;
            buff.f_expireDuration = duration;
            return buff;
        }
        public static SBuff CreateActionHealthDrainBuff(int actionIndex, float healthDrainAmount, float duration)
        {
            SBuff buff = new SBuff();
            buff.index = actionIndex * 10;
            buff.i_effect = 40009;
            buff.i_addType = (int)enum_ExpireRefreshType.RefreshIdentity;
            buff.f_healthDrainMultiply = healthDrainAmount;
            buff.f_expireDuration = duration;
            return buff;
        }
        public static SBuff CreateActionDamageMultiplyBuff(int actionIndex, float damageMultiply, float duration)
        {
            SBuff buff = new SBuff();
            buff.index = actionIndex * 10;
            buff.i_addType = (int)enum_ExpireRefreshType.Refresh;
            buff.f_damageMultiply = damageMultiply;
            buff.f_expireDuration = duration;
            return buff;
        }
        public static SBuff CreateActionDOTBuff(int actionIndex, float duration, float damageTickTime, float damagePerTick, enum_DamageType damageType)
        {
            SBuff buff = new SBuff();
            buff.index = actionIndex * 10;
            buff.i_addType = (int)enum_ExpireRefreshType.Refresh;
            buff.f_expireDuration = duration;
            buff.f_damageTickTime = damageTickTime;
            buff.f_damagePerTick = damagePerTick;
            buff.i_damageType = (int)damageType;
            return buff;
        }
    }

    public struct SEnermyGenerate : ISExcel
    {
        bool b_isFinal;
        int i_fighterCount;
        int i_shooterRCount;
        int i_shooterVCount;
        int i_casterCount;

        public bool m_IsFinal => b_isFinal;
        public int m_FighterCount => i_fighterCount;
        public int m_ShooterRCount => i_shooterRCount;
        public int m_ShooterVCount => i_shooterVCount;
        public int m_CasterCount => i_casterCount;

        public void InitOnValueSet()
        {
        }
    }
    #endregion
    #endregion

    #region Class
    #region GameBase
    public class GameChunk
    {
        public enum_ChunkType m_ChunkType { get; private set; }
        public List<int> m_Triggers { get; private set; } = new List<int>();
        public GameChunk(enum_ChunkType _chunkType)
        {
            m_ChunkType = _chunkType;
        }
        public virtual bool GetChunkMapIconShow => false;
        public virtual string GetChunkMapNameKey => "";
        public virtual string GetChunkMapIntroKey => "";
        public virtual string GetChunkMapLocationSprite => "";
        public virtual Vector3 GetChunkMapLocationPosition => Vector3.zero;
        public virtual bool OnMapDoubleClick() { return false; }
    }
    public class GameChunkBattle:GameChunk
    {
        public bool m_Triggered { get; private set; }
        public List<int> m_BattleEnermyCommands { get; private set; } = new List<int>();
        public bool m_IsFinal => m_ChunkType == enum_ChunkType.Final;
        public GameChunkBattle(enum_ChunkType chunkType) : base(chunkType)
        {
            m_Triggered = false;
        }
        public void Trigger()
        {
            m_Triggered = true;
        } 
    }
    public class GameChunkTeleport : GameChunk
    {
        public InteractTeleport m_ChunkTeleport { get; private set; }
        public bool m_Enable { get; private set; }
        public override bool GetChunkMapIconShow => m_Enable;
        public override string GetChunkMapNameKey => "UI_Map_Teleport_Name";
        public override string GetChunkMapIntroKey => "UI_Map_Teleport_Intro";
        public override Vector3 GetChunkMapLocationPosition =>m_ChunkTeleport.transform.position;
        public override string GetChunkMapLocationSprite => "map_icon_teleport";

        public GameChunkTeleport() : base( enum_ChunkType.Teleport)
        {
        }
        public void OnSpawnTeleport(InteractTeleport _ChunkTeleport)
        {
            m_ChunkTeleport = _ChunkTeleport;
            m_Enable = false;
            m_ChunkTeleport.SetPlay(m_Enable);
        }

        public void OnChunkTrigger()
        {
            m_Enable = true;
            m_ChunkTeleport.SetPlay(m_Enable);
        }

        public override bool OnMapDoubleClick()
        {
            GameManager.Instance.m_LocalPlayer.Teleport(NavigationManager.NavMeshPosition( m_ChunkTeleport.transform.position),m_ChunkTeleport.transform.rotation);
            return true;
        }
    }

    public class GameChunkEvent:GameChunk
    {
        public enum_ChunkEventType m_EventType;
        public Vector3 m_EventPos;
        public InteractGameBase m_EventInteract { get; private set; }
        public override bool GetChunkMapIconShow => true;
        public override string GetChunkMapLocationSprite => m_EventType.GetMapEventSprite();
        public override Vector2 GetChunkMapLocationPosition => m_EventPos;
        public override string GetChunkMapNameKey => m_EventType.GetMapLocalizeNameKey();
        public override string GetChunkMapIntroKey => m_EventType.GetMapLocalizeIntroKey();
        public GameChunkEvent() : base( enum_ChunkType.Event)
        {
        }
        public void OnSpawnEvent(enum_ChunkEventType _eventType, Vector3 _eventPos, InteractGameBase _eventInteract)
        {
            m_EventType = _eventType;
            m_EventPos = _eventPos;
            m_EventInteract = _eventInteract;
        }
    }
    
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
            if (damageInfo.m_Type == enum_DamageType.ArmorOnly)
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
        public void OnActivate(float startHealth, float startArmor)
        {
            base.OnActivate(startHealth);
            m_BaseArmor = startArmor;
            m_CurrentArmor = m_BaseArmor;
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

            float finalAmount = damageInfo.m_AmountApply;
            if (damageInfo.m_AmountApply > 0)    //Damage
            {
                if (damageReduction <= 0)
                    return false;
                finalAmount *= damageReduction;
                switch (damageInfo.m_Type)
                {
                    case enum_DamageType.ArmorOnly:
                        {
                            if (m_CurrentArmor <= 0)
                                return false;
                            DamageArmor(finalAmount);
                            OnHealthChanged(enum_HealthChangeMessage.DamageArmor);
                        }
                        break;
                    case enum_DamageType.HealthOnly:
                        {
                            DamageHealth(finalAmount);
                            OnHealthChanged(enum_HealthChangeMessage.DamageHealth);
                        }
                        break;
                    case enum_DamageType.Basic:
                        {
                            float healthDamage = finalAmount - m_CurrentArmor;
                            DamageArmor(finalAmount);
                            if (healthDamage > 0)
                                DamageHealth(healthDamage);
                            OnHealthChanged(healthDamage >= 0 ? enum_HealthChangeMessage.DamageHealth : enum_HealthChangeMessage.DamageArmor);
                        }
                        break;
                    default:
                        Debug.LogError("Error! Invalid Type:" + damageInfo.m_Type.ToString());
                        break;
                }
            }
            else if (damageInfo.m_AmountApply < 0)    //Healing
            {
                switch (damageInfo.m_Type)
                {
                    case enum_DamageType.ArmorOnly:
                        {
                            if (m_ArmorFull)
                                break;
                            DamageArmor(finalAmount);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveArmor);
                        }
                        break;
                    case enum_DamageType.HealthOnly:
                        {
                            finalAmount *= healEnhance;
                            if (m_HealthFull || finalAmount > 0)
                                break;
                            DamageHealth(finalAmount);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveHealth);
                        }
                        break;
                    case enum_DamageType.Basic:
                        {
                            float armorReceive = finalAmount - m_CurrentHealth + m_MaxHealth;
                            DamageHealth(finalAmount);
                            if (armorReceive > 0)
                            {
                                OnHealthChanged(enum_HealthChangeMessage.ReceiveHealth);
                                return true;
                            }

                            DamageArmor(armorReceive);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveArmor);
                        }
                        break;
                    default:
                        Debug.LogError("Error! Invalid Healing Type:" + damageInfo.m_Type.ToString());
                        break;
                }
            }
            if (finalAmount != 0)
                TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterHealthChange, damageInfo, m_Entity, finalAmount);
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
                if(m_CurrentHealth>m_MaxHealth)
                    OnSetHealth(m_MaxHealth);
                changed = true;
            }

            if (maxArmorAdd != m_MaxArmorAdditive)
            {
                m_MaxArmorAdditive = maxArmorAdd;
                OnSetArmor(m_MaxArmor);
                changed = true;
            }
            if(changed)
                OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        
        public void OnBattleFinishResetArmor()=>base.OnSetStatus(m_CurrentHealth, m_MaxArmor);
    }

    public class DamageInfo
    {
        public float m_AmountApply => (m_baseDamage + m_detail.m_DamageAdditive) * (1 + m_detail.m_DamageMultiply);
        private float m_baseDamage;
        public enum_DamageType m_Type { get; private set; }
        public DamageDeliverInfo m_detail { get; private set; }
        public DamageInfo(float damage, enum_DamageType type, DamageDeliverInfo detail)
        {
            m_baseDamage = damage;
            m_detail = detail;
            m_Type = type;
        }
        public void ResetBaseDamage(float damage)
        {
            m_baseDamage = damage;
        }
    }
    public class DamageDeliverInfo
    {
        public int I_IdentiyID { get; private set; } = -1;
        public int I_SourceID { get; private set; } = -1;
        public float m_DamageMultiply { get; private set; } = 0;
        public float m_DamageAdditive { get; private set; } = 0;
        public List<SBuff> m_BaseBuffApply { get; private set; } = new List<SBuff>();
        public enum_CharacterEffect m_DamageEffect = enum_CharacterEffect.Invalid;
        public float m_EffectDuration = 0;
        public Action<EntityBase> m_OnHitAction = null;

        public void AddExtraBuff(int commonBuffIndex) => m_BaseBuffApply.Add(GameDataManager.GetPresetBuff(commonBuffIndex));
        public static DamageDeliverInfo Default(int sourceID) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageMultiply = 0f, m_DamageAdditive = 0f };
        public static DamageDeliverInfo BuffInfo(int sourceID, SBuff buffApply) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageMultiply = 0f, m_DamageAdditive = 0f, m_BaseBuffApply = new List<SBuff>() { buffApply } };
        public static DamageDeliverInfo BuffInfo(int sourceID, int commonBuffIndex) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageMultiply = 0f, m_DamageAdditive = 0f, m_BaseBuffApply = new List<SBuff>() { GameDataManager.GetPresetBuff(commonBuffIndex) } };
        public static DamageDeliverInfo EquipmentInfo(int sourceID, float _damageAdditive, enum_CharacterEffect _effect, float _duration) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageAdditive = _damageAdditive, m_DamageEffect = _effect, m_EffectDuration = _duration };
        public static DamageDeliverInfo DamageInfo(int sourceID, float _damageEnhanceMultiply, float _damageAdditive) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageMultiply = _damageEnhanceMultiply, m_DamageAdditive = _damageAdditive, };
        public static DamageDeliverInfo DamageHitInfo(int sourceID, Action<EntityBase> OnHitEntity) => new DamageDeliverInfo() { I_SourceID = sourceID, I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), m_OnHitAction = OnHitEntity };
        public void EntityComponentOverride(int overrideID) => I_SourceID = overrideID;
        public void EffectAdditiveOverride(enum_CharacterEffect damageEffect, float effectDuration)
        {
            if (m_DamageEffect != damageEffect)
                m_EffectDuration = 0;

            m_EffectDuration += effectDuration;
            m_DamageEffect = damageEffect;
        }
        public void DamageAdditive(float damageMultiply,float damageAdditive)
        {
            m_DamageMultiply += damageMultiply;
            m_DamageAdditive += damageAdditive;
        }
        public void InfoAdditive(DamageDeliverInfo info)
        {
            m_DamageAdditive += info.m_DamageAdditive;
            m_DamageMultiply += info.m_DamageMultiply;
            m_DamageEffect = info.m_DamageEffect;
            m_EffectDuration = info.m_EffectDuration;
        }
        public void DamageReset()
        {
            m_DamageMultiply = 0f;
            m_DamageAdditive = 0f;
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
        protected float F_FireRateMultiply { get; private set; } = 1f;
        protected float F_ReloadRateMultiply { get; private set; } = 1f;
        protected float F_DamageMultiply { get; private set; } = 0f;
        protected float F_HealthDrainMultiply { get; private set; } = 0f;
        public float F_FireRateTick(float deltaTime) => deltaTime * F_FireRateMultiply;
        public float F_ReloadRateTick(float deltaTime) => deltaTime * F_ReloadRateMultiply;
        public float F_MovementSpeed => m_Entity.m_baseMovementSpeed * F_MovementSpeedMultiply;

        protected Dictionary<enum_CharacterEffect, EffectCounterBase> m_Effects = new Dictionary<enum_CharacterEffect, EffectCounterBase>();
        public bool B_Effecting(enum_CharacterEffect type) => m_Effects[type].m_Effecting;
        protected void ResetEffect(enum_CharacterEffect type) => m_Effects[type].Reset();
        public void OnSetEffect(enum_CharacterEffect type, float duration) => m_Effects[type].OnSet(duration);

        Func<DamageDeliverInfo> m_DamageBuffOverride;
        public void AddDamageOverride(Func<DamageDeliverInfo> _damageOverride) => m_DamageBuffOverride = _damageOverride;
        public virtual DamageDeliverInfo GetDamageBuffInfo()
        {
            DamageDeliverInfo deliver = DamageDeliverInfo.DamageInfo(m_Entity.m_EntityID, F_DamageMultiply, 0f);
            if (m_DamageBuffOverride != null) deliver.InfoAdditive(m_DamageBuffOverride());
            return deliver;
        }
        Func<DamageInfo, bool> OnReceiveDamage;
        Action OnExpireInfoChange;

        bool b_expireUpdated = false;
        public void UpdateEntityInfo() => b_expireUpdated = false;

        public CharacterInfoManager(EntityCharacterBase _attacher, Func<DamageInfo, bool> _OnReceiveDamage, Action _OnExpireChange)
        {
            m_Entity = _attacher;
            OnReceiveDamage = _OnReceiveDamage;
            OnExpireInfoChange = _OnExpireChange;
            TCommon.TraversalEnum((enum_CharacterEffect effect) => { m_Effects.Add(effect, new EffectCounterBase(enum_ExpireRefreshType.AddUp)); });
        }

        public virtual void OnActivate()
        {
            Reset();
            m_DamageBuffOverride = null;
        }
        public virtual void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
        {
            if (amountApply <= 0)
                return;

            if ( damageInfo.m_detail.I_SourceID == m_Entity.m_EntityID)
            {
                if (F_HealthDrainMultiply > 0)
                    m_Entity.m_HitCheck.TryHit(new DamageInfo(-amountApply * F_HealthDrainMultiply, enum_DamageType.HealthOnly, DamageDeliverInfo.Default(m_Entity.m_EntityID)));
                m_Expires.Traversal((EntityExpireBase expire) => { expire.OnDealtDamage(damageEntity,damageInfo,amountApply); });
            }
        }
        public virtual void OnDead() => Reset();
        public virtual void OnRecycle() => Reset();
        public virtual void OnRevive() => Reset(); 
        protected virtual void Reset()
        {
            m_Effects.Traversal((enum_CharacterEffect type) => { m_Effects[type].Reset(); });
            m_Expires.Traversal((EntityExpireBase expire) => { if (expire.m_ExpireType == enum_ExpireType.PresetBuff) RemoveExpire(expire); }, true);
            UpdateExpireInfo();
            UpdateExpireEffect();
        }

        public virtual void Tick(float deltaTime) {
            m_Expires.Traversal((EntityExpireBase expire) => { expire.OnTick(deltaTime); },true);
            m_Effects.Traversal((enum_CharacterEffect type) => { m_Effects[type].Tick(deltaTime); });

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
            EntityExpirepRreset buff = new EntityExpirepRreset(sourceID, buffInfo, OnReceiveDamage, RemoveExpire);
            switch (buff.m_RefreshType)
            {
                case enum_ExpireRefreshType.AddUp:
                    AddExpire(buff);
                    break;
                case enum_ExpireRefreshType.Refresh:
                    {
                        EntityExpirepRreset buffRefresh = m_Expires.Find(p => p.m_Index == buff.m_Index) as EntityExpirepRreset;
                        if (buffRefresh != null)
                            buffRefresh.BuffRefresh();
                        else
                            AddExpire(buff);
                    }
                    break;
                case enum_ExpireRefreshType.RefreshIdentity:
                    {
                        EntityExpirepRreset buffRefresh = m_Expires.Find(p => p.m_Index == buff.m_Index && (p.m_ExpireType == enum_ExpireType.PresetBuff && (p as EntityExpirepRreset).I_SourceID == buff.I_SourceID)) as EntityExpirepRreset;
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
            F_HealthDrainMultiply += expire.m_HealthDrainMultiply;
        }
        protected virtual void AfterInfoSet()
        {
            if (F_DamageReceiveMultiply < 0) F_DamageReceiveMultiply = 0;
            if (F_MovementSpeedMultiply < 0) F_MovementSpeedMultiply = 0;
            if (F_HealthDrainMultiply < 0) F_HealthDrainMultiply = 0;
            if (F_HealReceiveMultiply < 0) F_HealReceiveMultiply = 0;
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
        public int I_ClipAmount(int baseClipAmount) => baseClipAmount == 0 ? 0 : (int)(( baseClipAmount + I_ClipAdditive) * F_ClipMultiply);
        public float F_SpreadMultiply { get; private set; } = 1f;
        public float F_AimMovementStrictMultiply { get; private set; } = 1f;
        public float F_ProjectileSpeedMuiltiply { get; private set; } = 1f;
        public float F_PenetrateAdditive { get; private set; } = 0;
        public float F_AimRangeAdditive { get; private set; } = 0;
        public int I_ProjectileCopyCount { get; private set; } = 0;
        public float F_MaxHealthAdditive { get; private set; } = 0;
        public float F_MaxArmorAdditive { get; private set; } = 0;
        public float F_AllyHealthMultiplierAdditive { get; private set; } = 0f;
        public float P_CoinsDropBase { get; private set; } = 0f;
        public float F_CoinsCostMultiply { get; private set; } = 0f;
        protected int I_ClipAdditive { get; private set; } = 0;
        protected float F_ClipMultiply { get; private set; } = 1f;
        protected float F_DamageAdditive = 0f;
        
        protected Vector3 m_prePos;
        
        public List<ActionBase> m_PlayerExpires { get; private set; } = new List<ActionBase>();
        public List<ActionPerkBase> m_ExpirePerks { get; private set; } = new List<ActionPerkBase>();
        public List<ActionBuffBase> m_ExpireBuffs { get; private set; } = new List<ActionBuffBase>();
        public float m_Coins { get; private set; } = 0;
        public int m_Exp { get; private set; } = 0;
        public int m_Rank { get; private set; } = 0;

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
            OnPlayerMove(TCommon.GetXZDistance(m_prePos, m_Entity.transform.position));
            m_prePos = m_Entity.transform.position;
        }

        public void SetInfoData(CBattleSave m_saveData)
        {
            m_Coins = m_saveData.m_Coins;
            m_Rank = m_saveData.m_Rank;
            m_Exp = m_saveData.m_Exp;
            ActionDataManager.CreatePlayerEquipments(m_saveData.m_Equipments).Traversal((ActionPerkBase action) => { AddExpire(action); });
            ActionDataManager.CreateActionBuffs(m_saveData.m_ActionBuffs).Traversal((ActionBuffBase action) => { AddExpire(action); });
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
        }

        #region Action
        #region Interact
        public void OnActionPerkAcquire(ActionPerkBase equipment)=>AddExpire(equipment);
        public void OnActionBuffAcquire(ActionBuffBase actionBuff) => AddExpire(actionBuff);

        public bool CanPerkUpgrade() => m_ExpirePerks.Any(p=>p.m_UpgradeAble);
        public void OnPerkUpgrade()
        {
            m_ExpirePerks.TraversalRandomBreak((ActionPerkBase perk)=> {
                if (!perk.m_UpgradeAble)
                    return false;
                perk.Upgrade();
                return true;
            });
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
        }

        public bool CheckRevive()
        {
            for (int i = 0; i < m_PlayerExpires.Count; i++)
            {
                if (m_PlayerExpires[i].OnCheckRevive())
                    return true;
            }
            return false;
        }
        #endregion
        #region List Update
        protected override void AddExpire(EntityExpireBase expire)
        {
            base.AddExpire(expire);
            ActionBase targetExpire = expire as ActionBase;
            if (targetExpire == null)
                return;
            m_PlayerExpires.Add(targetExpire);

            if (targetExpire.m_ExpireType== enum_ExpireType.ActionPerk)
            {
                m_ExpirePerks.Add(targetExpire as ActionPerkBase);
                TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
            }
            else if (targetExpire.m_ExpireType == enum_ExpireType.ActionBuff)
            {
                m_ExpireBuffs.Add(targetExpire as ActionBuffBase);
                TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerBuffStatus, this);
            }

            targetExpire.OnActivate(m_Player, RemoveExpire);
        }

        protected override void RemoveExpire(EntityExpireBase expire)
        {
            base.RemoveExpire(expire);
            ActionBase targetExpire = expire as ActionBase;
            if (targetExpire == null)
                return;
            m_PlayerExpires.Remove(targetExpire);
            if (targetExpire.m_ExpireType== enum_ExpireType.ActionPerk)
            {
                m_ExpirePerks.Remove(targetExpire as ActionPerkBase);
                TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerPerkStatus, this);
            }
            else if (targetExpire.m_ExpireType == enum_ExpireType.ActionBuff)
            {
                m_ExpireBuffs.Remove(targetExpire as ActionBuffBase);
                TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerBuffStatus, this);
            }
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
            I_ProjectileCopyCount = 0;
            F_AimRangeAdditive = 0;
            F_SpreadMultiply = 1f;
            F_ProjectileSpeedMuiltiply = 1f;
            F_AimMovementStrictMultiply = 1f;
            F_AllyHealthMultiplierAdditive = 1f;
            F_MaxHealthAdditive = 0f;
            F_MaxArmorAdditive = 0;
            P_CoinsDropBase = 0f;
            F_CoinsCostMultiply = 1f;
        }
        protected override void OnSetExpireInfo(EntityExpireBase expire)
        {
            base.OnSetExpireInfo(expire);
            ActionBase action = expire as ActionBase;
            if (action == null)
                return;

            F_DamageAdditive += action.m_DamageAdditive;
            F_SpreadMultiply -= action.F_SpreadReduction;
            F_AimMovementStrictMultiply -= action.F_AimPressureReduction;
            
            I_ClipAdditive += action.I_ClipAdditive;
            F_ClipMultiply += action.F_ClipMultiply;

            F_ProjectileSpeedMuiltiply += action.F_ProjectileSpeedMultiply;
            F_PenetrateAdditive += action.F_PenetradeAdditive;
            I_ProjectileCopyCount += action.I_ProjectileCopyAdditive;
            F_AimRangeAdditive += action.F_AimRangeAdditive;
            F_MaxHealthAdditive += action.m_MaxHealthAdditive;
            F_MaxArmorAdditive += action.m_MaxArmorAdditive;
            F_AllyHealthMultiplierAdditive += action.F_AllyHealthMultiplierAdditive;
            P_CoinsDropBase += action.P_CoinsDropAdditive;
            F_CoinsCostMultiply -= action.F_CoinsCostDecrease;
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
        public override DamageDeliverInfo GetDamageBuffInfo()
        {
            ResetEffect(enum_CharacterEffect.Cloak);
            float randomDamageMultiply = UnityEngine.Random.Range(-GameConst.F_PlayerDamageAdjustmentRange, GameConst.F_PlayerDamageAdjustmentRange);
            DamageDeliverInfo info = DamageDeliverInfo.DamageInfo(m_Entity.m_EntityID, F_DamageMultiply + randomDamageMultiply, F_DamageAdditive);
            m_PlayerExpires.Traversal((ActionBase action) => { action.OnAttackDamageSet(info); });
            return info;
        }

        public void OnPlayerMove(float distance) => m_PlayerExpires.Traversal((ActionBase action) => { action.OnMove(distance); });
        public void OnReloadFinish() => m_PlayerExpires.Traversal((ActionBase action) => { action.OnReloadFinish(); });

        public void OnEntityActivate(EntityBase targetEntity)
        {
            if (targetEntity.m_ControllType != enum_EntityController.AI || targetEntity.m_Flag != m_Entity.m_Flag || targetEntity.m_EntityID == m_Entity.m_EntityID)
                return;

            EntityCharacterBase ally = (targetEntity as EntityCharacterBase);
            if (F_AllyHealthMultiplierAdditive > 0)
                ally.m_Health.SetHealthMultiplier(F_AllyHealthMultiplierAdditive);
        }

        public void OnWillDealtDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity) { m_PlayerExpires.Traversal((ActionBase action) => { action.OnBeforeDealtDamageBegin(damageEntity, damageInfo); action.OnBeforeDealtDamageMiddle(damageEntity, damageInfo); action.OnBeforeDealtDamageFinal(damageEntity, damageInfo); }); }

        public void OnWillReceiveDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity) { m_PlayerExpires.Traversal((ActionBase action) => { action.OnBeforeReceiveDamage(damageInfo); }); }

        public override void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
        {
            if (damageInfo.m_detail.I_SourceID <= 0)
                return;
            base.OnCharacterHealthChange(damageInfo, damageEntity, amountApply);
            
            if (damageEntity.m_EntityID == m_Player.m_EntityID)
            {
                if (amountApply > 0)
                    m_PlayerExpires.Traversal((ActionBase action) => { action.OnReceiveDamage(damageInfo, amountApply); });
                else
                    m_PlayerExpires.Traversal((ActionBase action) => { action.OnReceiveHealing(damageInfo, amountApply); });
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
        public void OnCoinsGain(float coinAmount,bool isPickup)
        {
            if(isPickup)
                m_PlayerExpires.Traversal((ActionBase action) => { action.OnGainCoins(coinAmount); });
            m_Coins += coinAmount;
        }
        #endregion
        #region ExpInfo
        public void OnExpGain(int amount)
        {
            m_Exp += amount;
            int expRankup = GameExpression.GetRankupExp(m_Rank);
            if (m_Exp < expRankup)
                return;
            m_Exp -= expRankup;
            m_Rank += 1;
            TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnPlayerRankUp);
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
    
    public class EntityExpirepRreset : EntityExpireBase
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
        public override float m_HealthDrainMultiply => m_buffInfo.m_HealthDrainMultiply;
        public float m_ExpireDuration { get; private set; } = 0;
        public float f_expireCheck { get; private set; }
        public float f_expireLeftScale => f_expireCheck / m_ExpireDuration;
        public int I_SourceID { get; private set; }
        SBuff m_buffInfo;
        Func<DamageInfo, bool> OnDOTDamage;
        float f_dotCheck;
        public EntityExpirepRreset(int sourceID, SBuff _buffInfo, Func<DamageInfo, bool> _OnDOTDamage, Action<EntityExpireBase> _OnExpired)
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
                OnDOTDamage(new DamageInfo(m_buffInfo.m_DamagePerTick, m_buffInfo.m_DamageType, DamageDeliverInfo.Default(I_SourceID)));
            }
        }

        public override void OnDealtDamage(EntityCharacterBase receiver, DamageInfo info, float applyAmount)
        {
            base.OnDealtDamage(receiver, info, applyAmount);
            if (m_buffInfo.m_ExtraBuffApply <= 0)
                return;
            receiver.m_CharacterInfo.AddBuff(info.m_detail.I_SourceID, GameDataManager.GetPresetBuff(m_buffInfo.m_ExtraBuffApply));
        }
    }

    public class ActionBase:EntityExpireBase
    {
        public int m_Identity { get; private set; } = -1;
        public EntityCharacterPlayer m_Attacher { get; private set; }
        public virtual float m_RecordData { get; protected set; }
        public ActionBase() { }
        protected virtual void OnManagerSetData(int identity, ActionSaveData data) { m_RecordData = data.m_RecordData; }
        public virtual void OnActivate(EntityCharacterPlayer _actionEntity, Action<EntityExpireBase> OnExpired) { m_Attacher = _actionEntity; OnActivate(OnExpired); }

        public virtual bool B_ActionAble => true;
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
        public virtual int I_ProjectileCopyAdditive => 0;
        public virtual float m_MaxHealthAdditive => 0;
        public virtual float m_MaxArmorAdditive => 0;
        public virtual float F_AllyHealthMultiplierAdditive => 0;
        public virtual float P_CoinsDropAdditive => 0f;
        public virtual float F_CoinsCostDecrease => 0f;


        #region Interact
        public virtual void OnBeforeReceiveDamage(DamageInfo info) { }
        public virtual void OnReceiveDamage(DamageInfo info, float amount) { }
        public virtual void OnAttackDamageSet(DamageDeliverInfo info) { }
        public virtual void OnBeforeDealtDamageBegin(EntityCharacterBase receiver, DamageInfo info) { }
        public virtual void OnBeforeDealtDamageMiddle(EntityCharacterBase receiver, DamageInfo info) { }
        public virtual void OnBeforeDealtDamageFinal(EntityCharacterBase receiver, DamageInfo info) { }
        public virtual void OnReceiveHealing(DamageInfo info, float applyAmount) { }
        public virtual void OnGainCoins(float coinAmount) { }
        public virtual void OnReloadFinish() { }
        public virtual void OnMove(float distsance) { }
        public virtual bool OnCheckRevive() { return false; }
        #endregion
    }

    public class ActionPerkBase : ActionBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.ActionPerk;
        public enum_EquipmentRarity m_Rarity { get; private set; }

        public ActionPerkBase() { }
        public void OnManagerSetData(int identity, EquipmentSaveData _data)
        {
            m_Rarity = _data.m_Rarity;
            base.OnManagerSetData(identity, _data.m_ActionData);
        }
        public bool m_UpgradeAble => m_Rarity < enum_EquipmentRarity.Epic;
        public void Upgrade()
        {
            if (!m_UpgradeAble)
                return;

            m_Rarity++;
        }
    }

    public class ActionBuffBase:ActionBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.ActionBuff;
        public ActionBuffBase() { }
        public new void OnManagerSetData(int identity, ActionSaveData data)
        {
            base.OnManagerSetData(identity, data);
            if (m_RecordData < 0)
                SetDefaultData();
        }

        protected virtual void SetDefaultData()
        {
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
    
    #region Equipment
    public class WeaponHelperBase
    {
        public virtual bool B_TargetAlly => false;
        public int I_Index { get; private set; } = -1;
        public virtual bool B_LoopAnim => false;
        protected EntityCharacterBase m_Entity;
        protected Transform attacherHead => m_Entity.tf_Head;
        protected Func<DamageDeliverInfo> GetDamageDeliverInfo;
        public WeaponHelperBase(int equipmentIndex, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo)
        {
            I_Index = equipmentIndex;
            m_Entity = _controller;
            GetDamageDeliverInfo = _GetBuffInfo;
        }
        protected virtual Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target) => _target.tf_Head.position;
        public void OnPlay(bool _preAim, EntityCharacterBase _target) => OnPlay(_target,GetTargetPosition(_preAim, _target));
        public virtual void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {

        }

        public virtual void OnStopPlay()
        {

        }
        public static WeaponHelperBase AcquireWeaponHelper(int weaponIndex, EntityCharacterBase _entity, Func<DamageDeliverInfo> GetDamageBuffInfo)
        {
            SFXWeaponBase weaponInfo = GameObjectManager.GetEquipmentData<SFXWeaponBase>(weaponIndex);
            SFXProjectile projectile = weaponInfo as SFXProjectile;
            if (projectile)
            {
                switch (projectile.E_ProjectileType)
                {
                    default: Debug.LogError("Invalid Type:" + projectile.E_ProjectileType); break;
                    case enum_ProjectileFireType.Single: return new WeaponHelperBarrageRange(weaponIndex,projectile, _entity, GetDamageBuffInfo); 
                    case enum_ProjectileFireType.MultipleFan: return new WeaponHelperBarrageMultipleFan(weaponIndex,projectile, _entity, GetDamageBuffInfo); 
                    case enum_ProjectileFireType.MultipleLine: return new WeaponHelperBarrageMultipleLine(weaponIndex,projectile, _entity, GetDamageBuffInfo); 
                }
            }

            SFXCast cast = weaponInfo as SFXCast;
            if (cast)
            {
                switch (cast.E_CastType)
                {
                    default: Debug.LogError("Invalid Type:" + cast.E_CastType); break;
                    case enum_CastControllType.CastFromOrigin: return new WeaponHelperCaster(weaponIndex,cast, _entity, GetDamageBuffInfo);
                    case enum_CastControllType.CastControlledForward: return new WeaponHelperCasterControlled(weaponIndex,cast, _entity, GetDamageBuffInfo);
                    case enum_CastControllType.CastAtTarget: return new WeaponHelperCasterTarget(weaponIndex,cast, _entity, GetDamageBuffInfo);
                }
            }

            SFXBuffApply buffApply = weaponInfo as SFXBuffApply;
            if (buffApply)
                return new WeaponHelperBuffApply(weaponIndex,buffApply, _entity, GetDamageBuffInfo);

            SFXSubEntitySpawner entitySpawner = weaponInfo as SFXSubEntitySpawner;
            if (entitySpawner)
                return new WeaponHelperEntitySpawner(weaponIndex,entitySpawner, _entity, GetDamageBuffInfo);

            return null;
        }
    }

    public class WeaponHelperCaster : WeaponHelperBase
    {
        protected enum_CastTarget m_CastAt { get; private set; }
        protected bool m_castForward { get; private set; }
        public WeaponHelperCaster(int equipmentIndex,SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            m_CastAt = _castInfo.E_CastTarget;
            m_castForward = _castInfo.B_CastForward;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Transform castAt = GetCastAt(m_Entity);
            GameObjectManager.SpawnEquipment<SFXCast>(I_Index, NavigationManager.NavMeshPosition(  castAt.position), m_castForward?castAt.forward:Vector3.up).Play(GetDamageDeliverInfo());
        }
        protected Transform GetCastAt(EntityCharacterBase character)
        {
            switch(m_CastAt)
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
        public WeaponHelperCasterTarget(int equipmentIndex,SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex,_castInfo, _controller, _GetBuffInfo)
        {
        }
        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target)
        {
            Transform castAt = GetCastAt(_target);
            Vector3 castPos = NavigationManager.NavMeshPosition(castAt.position + TCommon.RandomXZSphere(m_Entity.F_AttackSpread));
            castPos.y = castAt.transform.position.y;
            return castPos;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            GameObjectManager.SpawnEquipment<SFXCast>(I_Index, _calculatedPosition,m_castForward?m_Entity.tf_Weapon.forward:Vector3.up).Play(GetDamageDeliverInfo());
        }
    }

    public class WeaponHelperCasterControlled : WeaponHelperCaster
    {
        public override bool B_LoopAnim => true;
        SFXCast m_Cast;
        public WeaponHelperCasterControlled(int equipmentIndex,SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex,_castInfo, _controller, _GetBuffInfo)
        {
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            if (!m_Cast)
            {
                m_Cast = GameObjectManager.SpawnEquipment<SFXCast>(I_Index, m_Entity.tf_Weapon.position, m_Entity.tf_Weapon.forward);
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

        public WeaponHelperBarrageRange(int equipmentIndex,SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
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
            Vector3 targetPosition =  preAim ? _target.m_PrecalculatedTargetPos(startDistance / f_projectileSpeed) : _target.tf_Head.position;

            if (preAim && Mathf.Abs(TCommon.GetAngle(m_Entity.tf_Weapon.forward, TCommon.GetXZLookDirection(m_Entity.tf_Weapon.position, targetPosition), Vector3.up)) > 90)    //Target Positioned Back, Return Target
                targetPosition = _target.tf_Head.position;

            if (TCommon.GetXZDistance(m_Entity.tf_Weapon.position, targetPosition) > m_Entity.F_AttackSpread)      //Target Outside Spread Sphere,Add Spread
                targetPosition += TCommon.RandomXZSphere(m_Entity.F_AttackSpread);
            return targetPosition;
        }

        protected void FireBullet(Vector3 startPosition, Vector3 direction, Vector3 targetPosition)
        {
            GameObjectManager.SpawnEquipment<SFXProjectile>(I_Index, startPosition, direction).Play(GetDamageDeliverInfo(),direction, targetPosition);
        }
        protected void SpawnMuzzle(Vector3 startPosition, Vector3 direction) => GameObjectManager.PlayMuzzle(m_Entity.m_EntityID,startPosition,direction,i_muzzleIndex,m_MuzzleClip);
    }
    public class WeaponHelperBarrageMultipleLine : WeaponHelperBarrageRange
    {
        public WeaponHelperBarrageMultipleLine(int equipmentIndex,SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex,projectileInfo, _controller, _GetBuffInfo)
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
        public WeaponHelperBarrageMultipleFan(int equipmentIndex,SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex,projectileInfo, _controller, _GetBuffInfo)
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
        public WeaponHelperBuffApply(int equipmentIndex,SFXBuffApply buffApplyinfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            m_buffInfo = GameDataManager.GetPresetBuff(buffApplyinfo.I_BuffIndex);
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            if (!m_Effect || !m_Effect.B_Playing)
                m_Effect = GameObjectManager.SpawnEquipment<SFXBuffApply>(I_Index, m_Entity.tf_Weapon.position, Vector3.up);

            m_Effect.Play(m_Entity.m_EntityID, m_buffInfo, m_Entity.tf_Weapon, _target);
        }
    }
    public class WeaponHelperEntitySpawner : WeaponHelperBase
    {
        bool m_SpawnAtTarget;
        public WeaponHelperEntitySpawner(int equipmentIndex,SFXSubEntitySpawner spawner, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            startHealth = 0;
            m_SpawnAtTarget = spawner.B_SpawnAtTarget;
        }
        Action<EntityCharacterBase> OnSpawn;
        float startHealth;
        public void SetOnSpawn(float _startHealth,Action<EntityCharacterBase> _OnSpawn)
        {
            OnSpawn = _OnSpawn;
            startHealth = _startHealth;
        }
        public override void OnPlay(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Vector3 spawnPosition = (m_SpawnAtTarget ? _target.transform.position : m_Entity.transform.position) + TCommon.RandomXZSphere(m_Entity.F_AttackSpread);
            GameObjectManager.SpawnEquipment<SFXSubEntitySpawner>(I_Index, spawnPosition, Vector3.up).Play(m_Entity,_target.transform.position, startHealth, GetDamageDeliverInfo, OnSpawn);
        }
    }
    #endregion
    #endregion
    #region For UI Usage
    public enum enum_UI_ActionUpgradeType
    {
        Invalid = -1,
        Upgradeable = 1,
        LackOfCoins = 2,
        MaxLevel = 3,
    }
    
    public class UIC_RarityLevel
    {
        class RarityLevel
        {
            public Image m_HighLight { get; private set; }
            public Image m_BackGround { get; private set; }
            public RarityLevel(Transform trans)
            {
                m_HighLight = trans.Find("HighLight").GetComponent<Image>();
                m_BackGround = trans.Find("Background").GetComponent<Image>();
            }
            public void SetHighlight(bool show)
            {
                m_HighLight.SetActivate(show);
                m_BackGround.SetActivate(!show);
            }
        }
        public Transform transform { get; private set; }
        ObjectPoolListComponent<int,Transform> m_Grid;
        Dictionary<int, RarityLevel> m_Levels = new Dictionary<int, RarityLevel>();
        public UIC_RarityLevel(Transform _transform)
        {
            transform = _transform;
            m_Grid = new ObjectPoolListComponent<int, Transform>(transform,"GridItem");
            m_Grid.ClearPool();
            TCommon.TraversalEnum((enum_EquipmentRarity rarity) => { m_Levels.Add((int)rarity,new RarityLevel( m_Grid.AddItem((int)rarity))); });
        }
        public void SetRarity(enum_EquipmentRarity level)
        {
            m_Levels.Traversal((int index, RarityLevel rarity) => rarity.SetHighlight(index <= (int)level));
        }
    }

    public class UIC_Button
    {
        Button m_Button;
        Transform m_Show;
        Transform m_Hide;
        public UIC_Button(Transform _transform, UnityEngine.Events.UnityAction OnButtonClick)
        {
            m_Button = _transform.GetComponent<Button>();
            m_Button.onClick.AddListener(OnButtonClick);
            m_Show = _transform.Find("Show");
            m_Hide = _transform.Find("Hide");
            SetInteractable(true);
        }
        public void SetInteractable(bool interactable)
        {
            m_Hide.SetActivate(!interactable);
            m_Show.SetActivate(interactable);
            m_Button.interactable = interactable;
        }
    }
    public class UIC_EquipmentData
    {
        public Transform transform { get; private set; }

        Image m_Image;
        UIC_RarityLevel m_Rarity;
        public UIC_EquipmentData(Transform _transform)
        {
            transform = _transform;
            m_Image = transform.Find("Mask/Image").GetComponent<Image>();
            m_Rarity = new UIC_RarityLevel(transform.Find("Rarity"));
        }
        public virtual void SetInfo(ActionPerkBase equipmentInfo)
        {
            m_Image.sprite = GameUIManager.Instance.m_ActionSprites[equipmentInfo.m_Index.ToString()];
            m_Rarity.SetRarity(equipmentInfo.m_Rarity);
        }
    }

    public class UIC_EquipmentNameData: UIC_EquipmentData
{
        UIT_TextExtend m_Name;
        public UIC_EquipmentNameData(Transform _transform):base(_transform)
        {
            m_Name = transform.Find("Name").GetComponent<UIT_TextExtend>();
        }
        public override void SetInfo(ActionPerkBase equipmentInfo)
        {
            base.SetInfo(equipmentInfo);
            m_Name.localizeKey = equipmentInfo.GetNameLocalizeKey();
        }
    }

    public class UIC_EquipmentNameFormatIntro : UIC_EquipmentNameData
    {
        UIT_TextExtend  m_Intro;

        public UIC_EquipmentNameFormatIntro(Transform _transform) : base(_transform)
        {

            m_Intro = transform.Find("Intro").GetComponent<UIT_TextExtend>();
        }
        public override void SetInfo(ActionPerkBase equipmentInfo)
        {
            base.SetInfo(equipmentInfo);
            m_Intro.formatText(equipmentInfo.GetIntroLocalizeKey(), string.Format("<color=#FFDA6BFF>{0}</color>", equipmentInfo.Value1), string.Format("<color=#FFDA6BFF>{0}</color>", equipmentInfo.Value2), string.Format("<color=#FFDA6BFF>{0}</color>", equipmentInfo.Value3));
        }
    }
    
    public class UIC_WeaponData
    {
        public Transform transform { get; private set; }
        UIT_TextExtend m_Name;
        Image m_Background;
        Image m_Image;
        Transform tf_AmmoStatus;
        Text m_Clip, m_Total;
        public UIC_WeaponData(Transform _transform)
        {
            transform = _transform;
            m_Background = transform.Find("Background").GetComponent<Image>();
            m_Image = transform.Find("Image").GetComponent<Image>();
            m_Name = transform.Find("NameStatus/Name").GetComponent<UIT_TextExtend>();
            tf_AmmoStatus = transform.Find("NameStatus/AmmoStatus");
            m_Clip = tf_AmmoStatus.Find("Clip").GetComponent<Text>();
            m_Total = tf_AmmoStatus.Find("Total").GetComponent<Text>();
        }

        public void UpdateInfo(WeaponBase weapon)
        {
            m_Background.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Rarity.GetUIGameControlBackground()];
            m_Image.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Weapon.GetSpriteName()];
            m_Name.autoLocalizeText = weapon.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();
            m_Name.color = TCommon.GetHexColor( weapon.m_WeaponInfo.m_Rarity.GetUITextColor());
        }
        public void UpdateAmmoInfo(int ammoLeft,int clipAmount)
        {
            m_Clip.text = ammoLeft.ToString();
            m_Total.text = clipAmount.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(tf_AmmoStatus as RectTransform);
        }
    }

    class UIC_MapBase
    {
        protected RectTransform rectTransform { get; private set; }
        protected RectTransform m_Map_Origin { get; private set; }
        protected RawImage m_Map_Origin_Base { get; private set; }
        protected RawImage m_Map_Origin_Base_Fog { get; private set; }
        protected UIGI_MapEntityLocation m_Player { get; private set; }
        protected float m_MapScale { get; private set; }
        protected float m_MapAngle { get; private set; }
        public UIC_MapBase(Transform transform, float mapScale)
        {
            rectTransform = transform as RectTransform;
            m_Map_Origin = transform.Find("Origin") as RectTransform;
            m_Map_Origin_Base = m_Map_Origin.Find("Base").GetComponent<RawImage>();
            m_Map_Origin_Base_Fog = m_Map_Origin_Base.transform.Find("Fog").GetComponent<RawImage>();
            m_Player = m_Map_Origin_Base.transform.Find("Player").GetComponent<UIGI_MapEntityLocation>();
            m_Player.Init();
            ChangeMapScale(mapScale);
        }

        public virtual void DoMapInit()
        {
            m_Player.Play(GameManager.Instance.m_LocalPlayer);
            m_Map_Origin_Base.texture = GameLevelManager.Instance.m_MapTexture;
            m_Map_Origin_Base.SetNativeSize();
            m_Map_Origin_Base_Fog.texture = GameLevelManager.Instance.m_FogTexture;
        }
        protected void UpdateMap(float mapAngle)
        {
            m_MapAngle = mapAngle;
            m_Map_Origin.localRotation = Quaternion.Euler(0, 0, mapAngle);
            m_Player.Tick();
        }
        
        protected void ChangeMapScale(float mapScale)
        {
            m_MapScale = mapScale;
            m_Map_Origin_Base.rectTransform.localScale = Vector3.one * m_MapScale;
        }
    }

    #endregion
    #endregion
}
