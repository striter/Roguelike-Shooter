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
#pragma warning disable 0649
namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
        public const float F_Gravity = 9.8f;
        public const float F_BlastShakeMultiply = .5f;
        public const float F_DamageImpactMultiply = 1f;

        public const float F_EntityDeadFadeTime = 3f;
        public const float F_PlayerReviveCheckAfterDead = 1.5f;
        public const float F_PlayerReviveBuffDuration = 6f; //复活无敌时间
        
        public const float I_MaxArmor = 99999;

        public const float F_AimMovementReduction = .6f;
        public const float F_MovementReductionDuration = .1f;
        public const int I_ProjectileMaxDistance = 100;
        public const int I_ProjectileBlinkWhenTimeLeftLessThan = 3;
        public const float F_AimAssistDistance = 100f;
        public const short I_BoltLastTimeAfterHit = 5;

        public const int I_BurstFirePelletsOnceTrigger = 3;       //Times While Burst Fire //似乎已经没用？

        public const float F_PlayerDamageAdjustmentRange = .1f;
        public const int I_PlayerRotationSmoothParam = 10;     //Camera Smooth Param For Player 10 is suggested

        public const float F_AIDamageTranslate = 0;   //.003f;
        public const float F_AIMovementCheckParam = .3f;
        public const float F_AITargetCheckParam = 3f;       //AI Retarget Duration,3 is suggested
        public const float F_AITargetCalculationParam = .5f;       //AI Target Param Calculation Duration, 1 is suggested;
        public const float F_AIMaxRepositionDuration = .5f;
        public const int I_AIIdlePercentage = 50;

        public const int I_EnermyCountWaveFinish = 0;       //When Total Enermy Count Reaches This Amount,Wave Finish
        public const int I_EnermySpawnDelay = 2;        //Enermy Spawn Delay Time 
        public const float F_EnermySpawnOffsetEach = .5f;       //Enermy Spawn Offset Each

        public const float F_PickupAcceleration = 800f; //拾取物的飞行加速速度
        public const int I_HealthPickupAmount = 25;
        public const int I_ArmorPickupAmount = 25;
        public const int I_HealthTradeAmount = 50;

        public const float F_LevelTileSize = 2f;        //Cube Size For Level Tiles

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
        public static int GetPlayerEquipmentIndex(int equipmentIndex) => equipmentIndex * 10;
        public static int GetPlayerSubEquipmentIndex(int equipmentIndex) => equipmentIndex * 10 + 1;
        public static int GetAIEquipmentIndex(int entityIndex, int weaponIndex = 0, int subWeaponIndex = 0) => entityIndex * 100 + weaponIndex * 10 + subWeaponIndex;
        public static int GetActionMuzzleIndex(enum_ActionType type) => 10010 + (int)type;
        public const int I_PlayerReviveBuffIndex = 40004;

        public static float F_PlayerSensitive(int sensitiveTap) => sensitiveTap / 5f;
        public static float F_GameVFXVolume(int vfxVolumeTap) => vfxVolumeTap / 10f;
        public static float F_GameMusicVolume(int musicVolumeTap) => musicVolumeTap / 10f;

        public static Vector3 V3_TileAxisOffset(TileAxis axis) => new Vector3(axis.X * GameConst.F_LevelTileSize, 0, axis.Y * GameConst.F_LevelTileSize);
        public static float F_BigmapYaw(Vector3 direction) => TCommon.GetAngle(direction, Vector3.forward, Vector3.up);         //Used For Bigmap Direction
        public static bool B_ShowHitMark(enum_HitCheck check) => check != enum_HitCheck.Invalid;

        public static float F_SphereCastDamageReduction(float weaponDamage, float distance, float radius) => weaponDamage * (1 - (distance / radius));       //Rocket Blast Damage

        public static int GetEquipmentSubIndex(int weaponIndex) => weaponIndex + 1;
        public static SBuff GetEnermyGameDifficultyBuffIndex(int difficulty) => SBuff.CreateEnermyChallengeDifficultyBuff(difficulty, .04f * (difficulty - 1));
        public static float GetAIBaseHealthMultiplier(int gameDifficulty) => 0.99f + 0.01f * gameDifficulty;
        public static float GetAIMaxHealthMultiplier(enum_StageLevel stageDifficulty) => (int)stageDifficulty ;

        public static float GetActionEnergyRevive(float damageApply) => damageApply * .0025f;    //伤害转换成能量的比率

        public static float GetAIIdleDuration() => UnityEngine.Random.Range(1f, 2f);

        public static float GetResultCompletion(bool win, enum_StageLevel _stage, int _battleLevelEntered) => win ? 1f : (.33f * ((int)_stage - 1) +.066f*_battleLevelEntered);
        public static float GetResultLevelScore(enum_StageLevel _stage, int _levelPassed) => 200 * ((int)_stage - 1) + 20 * (_levelPassed - 1);
        public static float GetResultDifficultyBonus(int _difficulty) =>1f+ _difficulty * .05f;
        public static float GetResultRewardCredits(float _totalScore) => _totalScore;

        public static RangeInt GetTradePrice(enum_Interaction interactType, enum_ActionRarity actionRarity= enum_ActionRarity.Invalid,enum_WeaponRarity weaponRarity= enum_WeaponRarity.Invalid)
        {
            switch (interactType)
            {
                default: Debug.LogError("No Coins Can Phrase Here!"); return new RangeInt(0, -1);
                case enum_Interaction.PickupHealth:
                    return new RangeInt(10, 0);
                case enum_Interaction.Action:
                    switch (actionRarity)
                    {
                        default: Debug.LogError("Invalid Level!"); return new RangeInt(0, -1);
                        case enum_ActionRarity.Normal: return new RangeInt(8, 4);
                        case enum_ActionRarity.OutStanding: return new RangeInt(16, 8);
                        case enum_ActionRarity.Epic: return new RangeInt(24, 12);
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

        public static List<enum_LevelType> GetRandomLevels(System.Random seed)
        {
            switch (seed.Next(1, 9))        //1-4
            {
                default:
                    return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.End };
                case 1: return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.BattleActionAcquire, enum_LevelType.Battle, enum_LevelType.Trade, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.ActionAdjustment, enum_LevelType.End };
                case 2: return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.Battle, enum_LevelType.BattlePerkUpgrade, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.BattleActionAcquire, enum_LevelType.Battle, enum_LevelType.Trade, enum_LevelType.Battle, enum_LevelType.End };
                case 3: return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.BattlePerkUpgrade, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.BattleActionAcquire, enum_LevelType.Battle, enum_LevelType.ActionAdjustment, enum_LevelType.End };
                case 4: return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.BattlePerkUpgrade, enum_LevelType.Battle, enum_LevelType.ActionAdjustment, enum_LevelType.Battle, enum_LevelType.Trade, enum_LevelType.Battle, enum_LevelType.End };
                case 5: return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.Battle, enum_LevelType.BattleActionAcquire, enum_LevelType.Battle, enum_LevelType.BattlePerkUpgrade, enum_LevelType.Battle, enum_LevelType.ActionAdjustment, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.End };
                case 6: return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.BattleActionAcquire, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.Trade, enum_LevelType.Battle, enum_LevelType.BattlePerkUpgrade, enum_LevelType.End };
                case 7: return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.BattleActionAcquire, enum_LevelType.Battle, enum_LevelType.ActionAdjustment, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.Trade, enum_LevelType.End };
                case 8: return new List<enum_LevelType>() { enum_LevelType.Start, enum_LevelType.Battle, enum_LevelType.BattlePerkUpgrade, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.Battle, enum_LevelType.Trade, enum_LevelType.Battle, enum_LevelType.ActionAdjustment, enum_LevelType.End };
            }
        }

        public static int GetActionRemovePrice(enum_StageLevel stage, int removeTimes) => 10 * (removeTimes + 1) ;
        public static int GetActionUpgradePrice(enum_StageLevel stage, int upgradeTimes) => 10 * (upgradeTimes + 1) ;
        public static enum_ActionRarity GetBattleTradeActionRarity(this enum_StageLevel stageLevel) => stageLevel.ToRarity() == enum_ActionRarity.Epic ? enum_ActionRarity.Epic : (stageLevel + 1).ToRarity();
        public static StageInteractGenerateData GetInteractGenerate(enum_StageLevel level)
        {
            switch (level)
            {
                default: return new StageInteractGenerateData();
                case enum_StageLevel.Rookie:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 00 }, { enum_WeaponRarity.Advanced, 100 }, { enum_WeaponRarity.Rare, 0 }, { enum_WeaponRarity.Legend, 0 } },
                        new Dictionary<enum_ActionRarity, int>() { { enum_ActionRarity.Normal, 60 }, { enum_ActionRarity.OutStanding, 40 }, { enum_ActionRarity.Epic, 0 } },
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),
                        new Dictionary<enum_ActionRarity, float> { { enum_ActionRarity.Normal, 13.6f }, { enum_ActionRarity.OutStanding, 4.5f }, { enum_ActionRarity.Epic, 0 } },
                        new Dictionary<enum_WeaponRarity, float> {{ enum_WeaponRarity.Ordinary, 4.5f },{ enum_WeaponRarity.Advanced,2.27f} }),
                        PickupGenerateData.Create(10, 100, 50, new RangeInt(10, 5),
                        new Dictionary<enum_ActionRarity, float> { { enum_ActionRarity.Normal, 40 }, { enum_ActionRarity.OutStanding, 55 }, { enum_ActionRarity.Epic, 5 } },
                        new Dictionary<enum_WeaponRarity, float> { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 100 } })
                        );
                case enum_StageLevel.Veteran:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 0 }, { enum_WeaponRarity.Rare, 100 }, { enum_WeaponRarity.Legend, 0 } },
                        new Dictionary<enum_ActionRarity, int>() { { enum_ActionRarity.Normal, 00 }, { enum_ActionRarity.OutStanding, 70 }, { enum_ActionRarity.Epic, 30 } },
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),
                        new Dictionary<enum_ActionRarity, float> { { enum_ActionRarity.Normal, 0 }, { enum_ActionRarity.OutStanding, 13.6f }, { enum_ActionRarity.Epic, 4.5f } },
                        new Dictionary<enum_WeaponRarity, float> {{ enum_WeaponRarity.Advanced, 4.5f },{ enum_WeaponRarity.Rare,2.27f} }),
                        PickupGenerateData.Create(10, 100, 100, new RangeInt(10, 5),
                        new Dictionary<enum_ActionRarity, float> { { enum_ActionRarity.Normal, 0 }, { enum_ActionRarity.OutStanding, 80 }, { enum_ActionRarity.Epic, 20 } },
                        new Dictionary<enum_WeaponRarity, float> { { enum_WeaponRarity.Rare, 100 } })
                        );
                case enum_StageLevel.Ranger:
                    return StageInteractGenerateData.Create(
                        new Dictionary<enum_WeaponRarity, int>() { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 0 }, { enum_WeaponRarity.Rare, 0 }, { enum_WeaponRarity.Legend, 100 } },
                        new Dictionary<enum_ActionRarity, int>() { { enum_ActionRarity.Normal, 0 }, { enum_ActionRarity.OutStanding, 0 }, { enum_ActionRarity.Epic, 100 } },
                        PickupGenerateData.Create(10, 10, 30, new RangeInt(2, 3),
                        new Dictionary<enum_ActionRarity, float> { { enum_ActionRarity.Normal, 0 }, { enum_ActionRarity.OutStanding, 0 }, { enum_ActionRarity.Epic, 13.6f } },
                        new Dictionary<enum_WeaponRarity, float> {{ enum_WeaponRarity.Rare, 4.5f },{ enum_WeaponRarity.Legend,2.27f} }),
                        PickupGenerateData.Create(0, 0, 0, new RangeInt(10, 5),
                        new Dictionary<enum_ActionRarity, float> { { enum_ActionRarity.Normal, 0 }, { enum_ActionRarity.OutStanding, 0 }, { enum_ActionRarity.Epic, 0 } },
                        new Dictionary<enum_WeaponRarity, float> { { enum_WeaponRarity.Ordinary, 0 }, { enum_WeaponRarity.Advanced, 0 } })
                        );
            }
        }

        public static readonly Dictionary<enum_CampFarmItemStatus, int> GetFarmGeneratePercentage = new Dictionary< enum_CampFarmItemStatus,int>() { {  enum_CampFarmItemStatus.Progress1 ,60},{enum_CampFarmItemStatus.Progress2 ,30},{enum_CampFarmItemStatus.Progress3 ,6},{enum_CampFarmItemStatus.Progress4,3},{ enum_CampFarmItemStatus.Progress5,1} };   //Farm生成等级百分比
        public static readonly Dictionary<enum_CampFarmItemStatus, float> GetFarmCreditPerSecond = new Dictionary<enum_CampFarmItemStatus, float> { { enum_CampFarmItemStatus.Progress1, .1f / 60f}, { enum_CampFarmItemStatus.Progress2, .2f / 60f }, { enum_CampFarmItemStatus.Progress3, .3f / 60f }, { enum_CampFarmItemStatus.Progress4, .5f / 60f }, { enum_CampFarmItemStatus.Progress5,1f / 60f } };      //Farm 等级,每秒Credit
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

    public static class UIConst
    {
        public const int I_AmmoCountToSlider = 200;      //Ammo UI,While Clip Above This Will Turn To Be Slider大于此值变成白条
        public const int I_PlayerDyingMinValue = 10;
        public const int I_PlayerDyingMaxValue = 50;

        public const float F_UIMaxArmor = 100f;
        public const float F_MapAnimateTime = 1.6f;

        public const float F_UIDamageStartOffset = 20f; //血显示区域范围

        public const float F_UIActionBattlePressDuration = .3f; //查看卡片说明长按时间
    }

    public static class UIExpression
    {
        public static Color ActionTypeColor(this enum_ActionType type)
        {
            switch (type)
            {
                case enum_ActionType.WeaponAbility:
                    return Color.yellow;
                case enum_ActionType.Equipment:
                    return Color.green;
                default:
                    return Color.magenta;
            }
        }
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

    }

    public static class GameEnumConvertions
    {
        public static enum_ActionRarity ToRarity(this enum_StageLevel stageLevel) => (enum_ActionRarity)stageLevel;
        public static enum_LevelGenerateType ToPrefabType(this enum_LevelType type)
        {
            switch (type)
            {
                default: Debug.LogError("Please Edit This Please:" + type.ToString()); return enum_LevelGenerateType.Invalid;
                case enum_LevelType.Battle:
                case enum_LevelType.End:
                    return enum_LevelGenerateType.Big;
                case enum_LevelType.BattleActionAcquire:
                case enum_LevelType.BattlePerkUpgrade:
                case enum_LevelType.ActionAdjustment:
                case enum_LevelType.Trade:
                case enum_LevelType.Start:
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
                case enum_HitCheck.Interact: return GameLayer.I_Interact;
            }
        }
    }

    public static class UIConvertions
    {
        public static string GetUISprite(this enum_LevelType type)
        {
            switch (type)
            {
                default: return "level_Invalid";
                case enum_LevelType.Start:
                case enum_LevelType.Battle:
                case enum_LevelType.Trade:
                case enum_LevelType.ActionAdjustment:
                case enum_LevelType.End:
                    return "level_" + type;
                case enum_LevelType.BattleActionAcquire:
                case enum_LevelType.BattlePerkUpgrade:
                    return "level_Reward";
            }
        }
        public static string GetIconSprite(this enum_ActionType type)
        {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!"+type.ToString());  return "";
                case enum_ActionType.WeaponAbility: return "action_icon_basic";
                case enum_ActionType.Equipment: return "action_icon_equipment";
            }
        }
        public static string GetNameBGSprite(this enum_ActionType type)
        {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!" + type.ToString()); return "";
                case enum_ActionType.WeaponAbility: return "action_bottom_basic";
                case enum_ActionType.Equipment: return "action_bottom_equipment";
            }
        }
        public static string GetCostBGSprite(this enum_ActionType type)
        {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!" + type.ToString()); return "";
                case enum_ActionType.WeaponAbility: return "action_cost_basic";
                case enum_ActionType.Equipment: return "action_cost_equipment";
            }
        }

        public static string GetMainSprite(InteractBase interact)
        {
            string spriteName = "control_main_fire";
            if (interact != null&&interact.B_InteractEnable)
                switch (interact.m_InteractType)
                {
                    case enum_Interaction.Invalid: Debug.LogError("Invalid Pharse Here!"); break;
                    default: spriteName = "control_main_pickup"; break;
                    case enum_Interaction.CampFarm:
                    case enum_Interaction.ActionAdjustment:
                        spriteName = "control_main_chat"; break;
                }
            return spriteName;
        }
        public static string GetAbilityBackground(bool cooldowning)=>cooldowning?"control_ability_bottom_cooldown":"control_ability_bottom_activate";
        public static string GetAbilitySprite(enum_PlayerCharacter character) => "control_ability_" + character;
        public static string GetSpriteName(this enum_PlayerWeapon weapon) => ((int)weapon).ToString();
        public static string GetUIBGColor(this enum_ActionType type)
        {
            switch(type)
            {
                default: return "000000FF";
                case enum_ActionType.WeaponAbility:
                    return "FAC108FF";
                case enum_ActionType.Equipment:
                    return "B0FE00FF";
            }
        }


        public static string GetUIInteractBackground(this enum_WeaponRarity rarity) => "interact_" + rarity;
        public static string GetUIStatusShadowBackground(this enum_WeaponRarity rarity) => "weapon_shadow_" + rarity;
        public static string GetUIGameControlBackground(this enum_WeaponRarity rarity) => "gamecontrol_" + rarity;
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
        public static string GetNameLocalizeKey(this BuffBase buff) => "Buff_Name_" + buff.m_Index;
        public static string GetNameLocalizeKey(this ActionBase action) => "Action_Name_" + action.m_Index;
        public static string GetIntroLocalizeKey(this ActionBase action) => "Action_Intro_" + action.m_Index;
        public static string GetLocalizeKey(this enum_StageLevel stage) => "Game_Stage_" + stage;
        public static string GetLocalizeKey(this enum_Style style) => "Game_Style_" + style;
        public static string GetLocalizeNameKey(this enum_PlayerWeapon weapon) => "Weapon_Name_" + weapon;
        public static string GetNameLocalizeKey(this InteractBase interact) => "UI_Interact_" + interact.m_InteractType+interact.m_ExternalLocalizeKeyJoint;
        public static string GetIntroLocalizeKey(this InteractBase interact) => "UI_Interact_" + interact.m_InteractType +interact.m_ExternalLocalizeKeyJoint+ "_Intro";
        public static string GetLocalizeKey(this enum_LevelType type) => "UI_TileType_" + type;
        public static string GetLocalizeKey(this enum_ActionRarity rarity) => "UI_Rarity_" + rarity;
        public static string GetLocalizeKey(this enum_Option_FrameRate frameRate) => "UI_Option_" + frameRate;
        public static string GetLocalizeKey(this enum_Option_JoyStickMode joystick) => "UI_Option_" + joystick;
        public static string GetLocalizeKey(this enum_Option_LanguageRegion region) => "UI_Option_" + region;
        public static string GetLocalizeKey(this enum_CampFarmItemStatus status) => "UI_Farm_" + status;
        public static string GetInteractTitleKey(this enum_ActionType type) => "UI_Interact_Action_" + type;
        public static string SetActionIntro(this ActionBase actionInfo, UIT_TextExtend text) => text.formatText(actionInfo.GetIntroLocalizeKey() ,actionInfo.F_Duration, actionInfo.Value1, actionInfo.Value2, actionInfo.Value3);
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
        OnEntityDeactivate,

        OnCharacterHealthWillChange,
        OnCharacterHealthChange,
        OnCharacterDead,
        OnCharacterRevive,

        OnStageBeginLoad,
        OnStageStart,       //Total Stage Start
        OnStageFinish,

        OnChangeLevel,       //Change Between Each Level
        
        OnBattleStart,      //Battle Against Entity
        OnBattleFinish,
        OnWaveStart,     //Battle Wave
        OnWaveFinish,

        OnGameStart,
        OnGameFinish,
        OnGameExit,

        OnCampStart,
    }

    enum enum_BC_UIStatus
    {
        Invalid = -1,
        UI_PlayerCommonStatus,
        UI_PlayerInteractStatus,
        UI_PlayerHealthStatus,
        UI_PlayerExpireListStatus,
        UI_PlayerWeaponStatus,

        UI_CampDataStatus,

        UI_PageOpen,
        UI_PageClose,
    }
    #endregion

    #region GameEnum
    public enum enum_StageLevel { Invalid = -1, Rookie = 1, Veteran = 2, Ranger = 3 }

    public enum enum_BattleDifficulty { Invalid = -1, Peaceful = 0, Eazy = 1, Normal = 2, Hard = 3, End = 4, BattleReward = 10, }

    public enum enum_EntityController { Invalid = -1, None = 1, Player = 2, AI = 3, Device = 4, }

    public enum enum_EntityFlag { Invalid = -1, None = 0, Player = 1, Enermy = 2, Neutal = 3, }

    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, Interact = 4, }
    
    public enum enum_Style { Invalid = -1, Forest = 1, Desert = 2, Iceland = 3, Horde = 4, Undead = 5, }

    public enum enum_LevelType { Invalid = -1, Start = 0, Battle = 1, End = 2, Trade = 11, ActionAdjustment = 12, BattleActionAcquire = 13,BattlePerkUpgrade=14, }

    public enum enum_LevelItemType { Invalid = -1, LargeMore, LargeLess, MediumMore, MediumLess, SmallMore, SmallLess, ManmadeMore, ManmadeLess, NoCollisionMore, NoCollisionLess, BorderLinear, BorderOblique, }

    public enum enum_LevelGenerateType { Invalid = -1, Big = 1, Small = 2 }

    public enum enum_LevelItemTileType { Invaid = -1, Empty, Main, Border, Item, Interact, }

    public enum enum_LevelItemTileOccupy { Invalid = -1, Inner, Outer, Border, }

    public enum enum_EnermyType { Invalid = -1, Fighter = 1, Shooter_Rookie = 2, Shooter_Veteran = 3, AOECaster = 4, Elite = 5, SubHidden = 99 }

    public enum enum_Interaction { Invalid = -1,
        GameBegin,Bonfire, ContainerTrade, ContainerBattle, PickupCoin, PickupHealth,PickupHealthPack, PickupArmor, Action, Weapon,PerkUpgrade, ActionAdjustment, Portal, GameEnd,
        CampBegin,CampStage, CampDifficult,CampFarm,CampAction,CampEnd, }
    
    public enum enum_ProjectileFireType { Invalid = -1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, CastSelfDetonate = 4, }

    public enum enum_CastTarget { Invalid=-1,Head=1,Weapon=2,Feet=3}

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, ForwardTrapezium = 4, }

    public enum enum_HealthChangeMessage { Invalid = -1, Default = 0, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Basic = 1, ArmorOnly = 2, HealthOnly = 3,}

    public enum enum_CharacterEffect { Invalid = -1, Freeze = 1, Cloak = 2, Scan = 3, }

    public enum enum_ExpireType { Invalid = -1, Buff = 1, Action = 2, }

    public enum enum_ExpireRefreshType { Invalid = -1, AddUp = 1, Refresh = 2,RefreshIdentity=3, }

    public enum enum_ActionRarity { Invalid = -1, Normal = 1, OutStanding = 2, Epic = 3, }

    public enum enum_ActionType { Invalid = -1, WeaponAbility = 1,Equipment=2,}

    public enum enum_EffectAttach { Invalid = -1,  Head = 1, Feet = 2, WeaponModel = 3,}

    public enum enum_PlayerCharacter {Invalid=-1,Beth=10001,Jason=10002,Charles=10003 }

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
        Pistol_L_1002 = 1002,
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

    public enum enum_GameAudioSFX
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
        public Dictionary<enum_ActionRarity, float> m_ActionRate { get; private set; }
        public Dictionary<enum_WeaponRarity, float> m_WeaponRate { get; private set; }

        public bool CanGenerateHealth(enum_EnermyType entityType) => TCommon.RandomPercentage() <= m_HealthRate;
        public bool CanGenerateArmor(enum_EnermyType entityType) => TCommon.RandomPercentage() <= m_ArmorRate;
        public int GetCoinGenerate(enum_EnermyType entityType) => TCommon.RandomPercentage() <= m_CoinRate ? m_CoinRange.Random() : -1;
        public static PickupGenerateData Create(int healthRate, int armorRate, int coinRate, RangeInt coinAmount, Dictionary<enum_ActionRarity, float> _actionRate, Dictionary<enum_WeaponRarity, float> _weaponRate) => new PickupGenerateData() { m_HealthRate = healthRate, m_ArmorRate = armorRate, m_CoinRate = coinRate, m_CoinRange = coinAmount,m_ActionRate=_actionRate,m_WeaponRate=_weaponRate };
    }

    public struct StageInteractGenerateData
    {
        public PickupGenerateData m_NormalPickupData { get; private set; }
        public PickupGenerateData m_ElitePickupData { get; private set; }
        Dictionary<enum_WeaponRarity, int> m_TradeWeapon;
        Dictionary<enum_ActionRarity, int> m_TradeAction;
        public enum_WeaponRarity GetTradeWeaponRarity(System.Random seed) => TCommon.RandomPercentage(m_TradeWeapon, enum_WeaponRarity.Invalid, seed);
        public enum_ActionRarity GetTradeActionRarity(System.Random seed) => TCommon.RandomPercentage(m_TradeAction, enum_ActionRarity.Invalid ,seed);
        public static StageInteractGenerateData Create(Dictionary<enum_WeaponRarity,int> _weaponRate,  Dictionary<enum_ActionRarity, int> _actionRate, PickupGenerateData _normalGenerate,PickupGenerateData _eliteGenerate) => new StageInteractGenerateData() { m_TradeWeapon=_weaponRate,m_TradeAction=_actionRate,m_NormalPickupData=_normalGenerate,m_ElitePickupData=_eliteGenerate};
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
        public enum_PlayerWeapon m_WeaponSelected;
        public int m_StorageRequestStamp;
        public CGameSave()
        {
            f_Credits = 0;
            f_TechPoints = 0;
            m_GameDifficulty = 1;
            m_DifficultyUnlocked = 1;
            m_StorageRequestStamp = -1;
            m_CharacterSelected = enum_PlayerCharacter.Beth;
            m_WeaponSelected = enum_PlayerWeapon.UZI;
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
        public int m_coins;
        public List<ActionSaveData> m_actionEquipment;
        public float m_curHealth;
        public float m_maxHealthAdditive;
        public float m_curArmor;
        public WeaponSaveData m_weapon1, m_weapon2;
        public enum_PlayerCharacter m_character;
        public CBattleSave()
        {
            m_coins = 0;
            m_curHealth = -1;
            m_curArmor = -1;
            m_maxHealthAdditive = -1;
            m_actionEquipment = new List<ActionSaveData>();
            m_Stage = enum_StageLevel.Rookie;
            m_GameSeed = DateTime.Now.ToLongTimeString().ToString();
            m_character = GameDataManager.m_GameData.m_CharacterSelected;
            m_weapon1 = WeaponSaveData.CreateNew(enum_PlayerWeapon.UMP45);
            m_weapon2 = WeaponSaveData.CreateNew(enum_PlayerWeapon.Invalid);
        }
        public void Adjust(EntityCharacterPlayer _player, GameLevelManager _level)
        {
            m_coins = _player.m_PlayerInfo.m_Coins;
            m_curHealth = _player.m_Health.m_CurrentHealth;
            m_maxHealthAdditive = _player.m_Health.m_MaxHealthAdditive;
            m_curArmor = _player.m_Health.m_CurrentArmor;

            m_weapon1 = WeaponSaveData.Create(_player.m_Weapon1);
            m_weapon2 = WeaponSaveData.Create(_player.m_Weapon2);
            m_actionEquipment = ActionSaveData.Create(_player.m_PlayerInfo.m_ActionEquipment);

            m_GameSeed = _level.m_Seed;
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

    public struct ActionSaveData : IXmlPhrase
    {
        public bool m_IsNull => m_Index <= 0;
        public int m_Index { get; private set; }
        public enum_ActionRarity m_Level { get; private set; }

        public string ToXMLData() => m_Index.ToString() + "," + m_Level.ToString();
        public ActionSaveData(string xmlData)
        {
            string[] split = xmlData.Split(',');
            m_Index = int.Parse(split[0]);
            m_Level = (enum_ActionRarity)Enum.Parse(typeof(enum_ActionRarity), split[1]);
        }

        public static ActionSaveData Create(int index, enum_ActionRarity level) => new ActionSaveData { m_Index = index, m_Level = level };
        public static ActionSaveData Create(ActionBase action) => action == null ? new ActionSaveData() { m_Index = -1, m_Level = 0 } : new ActionSaveData { m_Index = action.m_Index, m_Level = action.m_rarity };
        public static List<ActionSaveData> Create(List<ActionBase> actions)
        {
            List<ActionSaveData> infos = new List<ActionSaveData>();
            actions.Traversal((ActionBase action) => { infos.Add(Create(action)); });
            return infos;
        }
    }

    public struct WeaponSaveData : IXmlPhrase
    {
        public enum_PlayerWeapon m_Weapon { get; private set; }
        public float m_WeaponEnergy { get; private set; }
        public ActionSaveData m_WeaponAction { get; private set; }
        public string ToXMLData() => m_Weapon + ";" + m_WeaponEnergy + ";" + m_WeaponAction.ToXMLData();
        public WeaponSaveData(string xmlData)
        {
            string[] split = xmlData.Split(';');
            m_Weapon = (enum_PlayerWeapon)Enum.Parse(typeof(enum_PlayerWeapon), split[0]);
            m_WeaponEnergy = float.Parse(split[1]);
            m_WeaponAction = new ActionSaveData(split[2]);
        }
        public static WeaponSaveData Create(WeaponBase weapon) => new WeaponSaveData() { m_Weapon =weapon!=null? weapon.m_WeaponInfo.m_Weapon: enum_PlayerWeapon.Invalid, m_WeaponEnergy = weapon!=null?weapon.m_ActionEnergy:0, m_WeaponAction =  ActionSaveData.Create(weapon != null ? weapon.m_WeaponAction:null)};
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
        float f_reloadTime;
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
        public float m_ReloadTime => f_reloadTime;
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
                Debug.LogError("Please Corret Format Of DefineSplit:" + em_defines);
            m_LevelStyle = (enum_Style)(int.Parse(defineSplit[0]));
            m_LevelPrefabType = (enum_LevelGenerateType)(int.Parse(defineSplit[1]));
            m_IsInner = int.Parse(defineSplit[2]) == 1;
            m_ItemGenerate = new Dictionary<enum_LevelItemType, RangeInt>();
            m_ItemGenerate.Add(enum_LevelItemType.LargeLess, ir_largeLess);
            m_ItemGenerate.Add(enum_LevelItemType.LargeMore, ir_largeMore);
            m_ItemGenerate.Add(enum_LevelItemType.MediumLess, ir_mediumLess);
            m_ItemGenerate.Add(enum_LevelItemType.MediumMore, ir_mediumMore);
            m_ItemGenerate.Add(enum_LevelItemType.SmallLess, ir_smallLess);
            m_ItemGenerate.Add(enum_LevelItemType.SmallMore, ir_smallMore);
            m_ItemGenerate.Add(enum_LevelItemType.ManmadeLess, ir_manmadeLess);
            m_ItemGenerate.Add(enum_LevelItemType.ManmadeMore, ir_manmadeMore);
            m_ItemGenerate.Add(enum_LevelItemType.NoCollisionLess, ir_noCollisionLess);
            m_ItemGenerate.Add(enum_LevelItemType.NoCollisionMore, ir_noCollisionMore);
        }
    }

    public struct SGenerateEntity : ISExcel
    {
        string em_defines;
        float f_eliteChance;
        RangeInt ir_fighter;
        RangeInt ir_shooterRookie;
        RangeInt ir_shooterVeteran;
        RangeInt ir_aoeCaster;
        RangeInt ir_elite;
        public enum_BattleDifficulty m_Difficulty;
        public int m_waveCount;
        public float m_EliteChance => f_eliteChance;
        public Dictionary<enum_EnermyType, RangeInt> m_EntityGenerate;
        public void InitOnValueSet()
        {
            string[] defineSplit = em_defines.Split('_');
            m_Difficulty = (enum_BattleDifficulty)(int.Parse(defineSplit[0]));
            m_waveCount = (int.Parse(defineSplit[1]));
            m_EntityGenerate = new Dictionary<enum_EnermyType, RangeInt>();
            m_EntityGenerate.Add(enum_EnermyType.Fighter, ir_fighter);
            m_EntityGenerate.Add(enum_EnermyType.Shooter_Rookie, ir_shooterRookie);
            m_EntityGenerate.Add(enum_EnermyType.Shooter_Veteran, ir_shooterVeteran);
            m_EntityGenerate.Add(enum_EnermyType.AOECaster, ir_aoeCaster);
            m_EntityGenerate.Add(enum_EnermyType.Elite, ir_elite);
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
        public float m_DamageTickTime => f_damageTickTime;
        public float m_DamagePerTick => f_damagePerTick;
        public enum_DamageType m_DamageType => (enum_DamageType)i_damageType;
        public void InitOnValueSet()
        {
            f_movementSpeedMultiply = f_movementSpeedMultiply > 0 ? f_movementSpeedMultiply / 100f : 0;
            f_fireRateMultiply = f_fireRateMultiply > 0 ? f_fireRateMultiply / 100f : 0;
            f_reloadRateMultiply = f_reloadRateMultiply > 0 ? f_reloadRateMultiply / 100f : 0;
            f_damageMultiply = f_damageMultiply > 0 ? f_damageMultiply / 100f : 0;
            f_damageReduce = f_damageReduce > 0 ? f_damageReduce / 100f : 0;
            f_healthDrainMultiply = f_healthDrainMultiply > 0 ? f_healthDrainMultiply / 100f : 0;
        }
        //Normally In Excel 0-99
        //100-999
        public static SBuff SystemSubEntityDOTInfo(float damageTickTime, float damagePerTick) => new SBuff(){ index = 100,i_addType = (int)enum_ExpireRefreshType.Refresh,f_expireDuration = 0,f_damageTickTime = damageTickTime,f_damagePerTick = damagePerTick, i_damageType = (int)enum_DamageType.Basic};
        public static SBuff SystemPlayerReviveInfo(float duration,int effect) => new SBuff() { index = 101,i_effect= effect, i_addType = (int)enum_ExpireRefreshType.Refresh, f_expireDuration = duration, f_damageReduce = 1f };
        public static SBuff SystemPlayerBonfireHealInfo() => new SBuff() { index = 102, i_addType = (int)enum_ExpireRefreshType.Refresh, f_expireDuration = 0.5f, f_damageTickTime = .1f, f_damagePerTick = -2f, i_damageType = (int)enum_DamageType.HealthOnly };
        //1000-9999
        public static SBuff CreateEnermyChallengeDifficultyBuff(int difficulty, float damageMultiply)
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

    #endregion
    #endregion

    #region Class
    #region GameBase
    public class HealthBase
    {
        public float m_CurrentHealth { get; private set; }
        public float m_BaseHealth { get; private set; }
        public virtual float m_MaxHealth => m_BaseHealth;
        public virtual float F_TotalEHP => m_CurrentHealth;
        public bool B_HealthFull => m_CurrentHealth >= m_MaxHealth;
        public float F_HealthBaseScale => m_CurrentHealth / m_BaseHealth;
        public float F_HealthMaxScale => m_CurrentHealth / m_MaxHealth;
        public bool b_IsDead => m_CurrentHealth <= 0;
        protected void DamageHealth(float health)
        {
            m_CurrentHealth -= health;
            if (m_CurrentHealth < 0)
                m_CurrentHealth = 0;
            else if (m_CurrentHealth > m_MaxHealth)
                m_CurrentHealth = m_MaxHealth;
        }

        protected Action<enum_HealthChangeMessage> OnHealthChanged;
        protected Action OnDead;
        public HealthBase(Action<enum_HealthChangeMessage> _OnHealthChanged, Action _OnDead)
        {
            OnHealthChanged = _OnHealthChanged;
            OnDead = _OnDead;
        }
        public void OnSetHealth(float startHealth, bool restoreHealth)
        {
            m_BaseHealth = startHealth;
            if (restoreHealth)
                m_CurrentHealth = m_MaxHealth;
        }
        public void OnSetHealth(float reviveHealth)
        {
            m_CurrentHealth = reviveHealth;
        }
        public virtual bool OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1, float healEnhance = 1)
        {
            if (b_IsDead || damageInfo.m_Type == enum_DamageType.ArmorOnly)
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

            if (b_IsDead)
                OnDead();

            return true;
        }
    }
    public class EntityHealth : HealthBase
    {
        public float m_CurrentArmor { get; private set; }
        public float m_DefaultArmor { get; private set; }
        public override float F_TotalEHP => m_CurrentArmor + base.F_TotalEHP;
        float m_HealthMultiplier = 1f;
        public override float m_MaxHealth => base.m_MaxHealth * m_HealthMultiplier;
        protected EntityCharacterBase m_Entity;
        protected void DamageArmor(float amount)
        {
            m_CurrentArmor -= amount;
            if (m_CurrentArmor < 0)
                m_CurrentArmor = 0;
            if (m_CurrentArmor > GameConst.I_MaxArmor)
                m_CurrentArmor = GameConst.I_MaxArmor;
        }

        public EntityHealth(EntityCharacterBase entity, Action<enum_HealthChangeMessage> _OnHealthChanged, Action _OnDead) : base(_OnHealthChanged, _OnDead)
        {
            m_Entity = entity;
            m_HealthMultiplier = 1f;
        }
        public void OnActivate(float startHealth, float startArmor, bool restoreHealth)
        {
            base.OnSetHealth(startHealth, restoreHealth);
            m_DefaultArmor = startArmor;
            m_CurrentArmor = m_DefaultArmor;
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        public void OnSetHealth(float setHealth, float setArmor)
        {
            base.OnSetHealth(setHealth);
            m_CurrentArmor = setArmor;
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        public void SetHealthMultiplier(float healthMultiplier)
        {
            m_HealthMultiplier = healthMultiplier;
            OnSetHealth(m_BaseHealth, true);
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        public override bool OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1, float healEnhance = 1)
        {
            if (b_IsDead)
                return false;

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
                            DamageArmor(finalAmount);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveArmor);
                        }
                        break;
                    case enum_DamageType.HealthOnly:
                        {
                            finalAmount *= healEnhance;
                            if (B_HealthFull||finalAmount>0)
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
            if (b_IsDead)
                OnDead();
            return true;
        }
    }

    public class EntityPlayerHealth:EntityHealth
    {
        public float m_MaxHealthAdditive { get; private set; }
        public override float m_MaxHealth => base.m_MaxHealth + m_MaxHealthAdditive;
        public EntityPlayerHealth(EntityCharacterBase entity, Action<enum_HealthChangeMessage> _OnHealthChanged, Action _OnDead) : base(entity,_OnHealthChanged, _OnDead)
        {
        }
        public void AddMaxHealth(float maxHealthAdd)
        {
            m_MaxHealthAdditive += maxHealthAdd;
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }

        public void SetInfoData(float startHealth,float startArmor,float maxHealthAdditive)
        {
            m_MaxHealthAdditive = maxHealthAdditive;
            OnSetHealth(startHealth,startArmor);
        }
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
    }
    #endregion

    #region Entity Info Manager
    public class CharacterInfoManager
    {
        protected EntityCharacterBase m_Entity { get; private set; }
        public List<ExpireBase> m_Expires { get; private set; } = new List<ExpireBase>();
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

            if (F_HealthDrainMultiply > 0 && damageInfo.m_detail.I_SourceID == m_Entity.m_EntityID)
                m_Entity.m_HitCheck.TryHit(new DamageInfo(-amountApply * F_HealthDrainMultiply, enum_DamageType.HealthOnly, DamageDeliverInfo.Default(m_Entity.m_EntityID)));
        }
        public virtual void OnDead() => Reset();
        public virtual void OnRevive() => Reset(); 
        protected virtual void Reset()
        {
            m_Effects.Traversal((enum_CharacterEffect type) => { m_Effects[type].Reset(); });
            m_Expires.Traversal((ExpireBase expire) => { if (expire.m_ExpireType == enum_ExpireType.Buff) OnExpireElapsed(expire); }, true);
            UpdateEntityInfo();
        }

        public virtual void Tick(float deltaTime) {
            m_Expires.Traversal((ExpireBase expire) => { expire.OnTick(deltaTime); },true);
            m_Effects.Traversal((enum_CharacterEffect type) => { m_Effects[type].Tick(deltaTime); });

            if (b_expireUpdated)
                return;
            UpdateExpireInfo();
            UpdateExpireEffect();
            OnExpireInfoChange?.Invoke();
            b_expireUpdated = true;
        }

        protected virtual void AddExpire(ExpireBase expire)
        {
            m_Expires.Add(expire);
            UpdateEntityInfo();
        }
        void RefreshExpire(ExpireBase expire)
        {
            expire.ExpireRefresh();
        }
        protected virtual void OnExpireElapsed(ExpireBase expire)
        {
            m_Expires.Remove(expire);
            UpdateEntityInfo();
        }
        public void AddBuff(int sourceID, SBuff buffInfo)
        {
            BuffBase buff = new BuffBase(sourceID, buffInfo, OnReceiveDamage, OnExpireElapsed);
            switch (buff.m_RefreshType)
            {
                case enum_ExpireRefreshType.AddUp:
                    AddExpire(buff);
                    break;
                case enum_ExpireRefreshType.Refresh:
                    {
                        ExpireBase buffRefresh = m_Expires.Find(p => p.m_Index == buff.m_Index);
                        if (buffRefresh != null)
                            RefreshExpire(buffRefresh);
                        else
                            AddExpire(buff);
                    }
                    break;
                case enum_ExpireRefreshType.RefreshIdentity:
                    {
                        ExpireBase buffRefresh = m_Expires.Find(p => p.m_Index == buff.m_Index && (p.m_ExpireType == enum_ExpireType.Buff && (p as BuffBase).I_SourceID == buff.I_SourceID));
                        if (buffRefresh != null)
                            RefreshExpire(buffRefresh);
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
        protected virtual void OnSetExpireInfo(ExpireBase expire)
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
            m_Expires.Traversal((ExpireBase expire) =>
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
        public int I_ClipAmount(int baseClipAmount) => baseClipAmount == 0 ? 0 : (int)(((B_OneOverride ? 1 : baseClipAmount) + I_ClipAdditive) * F_ClipMultiply);
        public float F_RecoilMultiply { get; private set; } = 1f;
        public float F_AimMovementStrictMultiply { get; private set; } = 1f;
        public float F_ProjectileSpeedMuiltiply { get; private set; } = 1f;
        public bool B_ProjectilePenetrate { get; private set; } = false;
        public float F_AllyHealthMultiplierAdditive { get; private set; } = 0f;
        protected bool B_OneOverride { get; private set; } = false;
        protected int I_ClipAdditive { get; private set; } = 0;
        protected float F_ClipMultiply { get; private set; } = 1f;
        protected float F_DamageAdditive = 0f;

        protected Vector3 m_prePos;

        public List<ActionBase> m_ActionPlaying { get; private set; } = new List<ActionBase>();
        public List<ActionBase> m_ActionEquipment { get; private set; } = new List<ActionBase>();
        Action OnPlayerActionChange;
        public int m_Coins { get; private set; } = 0;

        public PlayerInfoManager(EntityCharacterPlayer _attacher, Func<DamageInfo, bool> _OnReceiveDamage, Action _OnExpireChange, Action _OnPlayerActionChange) : base(_attacher, _OnReceiveDamage, _OnExpireChange)
        {
            m_Player = _attacher;
            OnPlayerActionChange = _OnPlayerActionChange;
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

        public void SetInfoData(int coins, List<ActionBase> _actionEquiping)
        {
            m_Coins = coins;
            _actionEquiping.Traversal((ActionBase action) => { AddExpire(action); });
        }

        public void UpgradeEquipment(int index)
        {
            m_ActionEquipment[index].Upgrade();
        }
        public void RemoveEquipment(int index)
        {
            m_ActionEquipment[index].ForceExpire();
        }
        #region Action
        #region Interact
        public bool CheckRevive(ref RangeFloat reviveAmount)
        {
            for (int i = 0; i < m_ActionPlaying.Count; i++)
            {
                if (m_ActionPlaying[i].OnCheckRevive(ref reviveAmount))
                    return true;
            }
            return false;
        }
        #endregion
        #region List Update
        public void OnUseAction(ActionBase targetAction)
        {
            if(targetAction.m_ActionType== enum_ActionType.WeaponAbility)
                m_ActionPlaying.Traversal((ActionBase action) => { action.OnUseWeaponAbility(targetAction); });
            GameObjectManager.PlayMuzzle(m_Player.m_EntityID, m_Player.transform.position, Vector3.up, GameExpression.GetActionMuzzleIndex(targetAction.m_ActionType));
            AddExpire(targetAction);
        }


        protected override void AddExpire(ExpireBase expire)
        {
            base.AddExpire(expire);
            if (expire.m_ExpireType != enum_ExpireType.Action)
                return;
            ActionBase targetAction = expire as ActionBase;
            m_ActionPlaying.Add(targetAction);
            if (targetAction.m_ActionType == enum_ActionType.Equipment)
                m_ActionEquipment.Add(targetAction);

            targetAction.Activate(m_Player, OnExpireElapsed);
            targetAction.OnActivate();

            OnPlayerActionChange();
        }

        protected override void OnExpireElapsed(ExpireBase expire)
        {
            base.OnExpireElapsed(expire);
            if (expire.m_ExpireType != enum_ExpireType.Action)
                return;
            ActionBase targetAction = expire as ActionBase;
            m_ActionPlaying.Remove(targetAction);
            if (targetAction.m_ActionType == enum_ActionType.Equipment)
                m_ActionPlaying.Remove(targetAction);
            OnPlayerActionChange();
        }
        #endregion
        #region Data Update
        protected override void OnResetInfo()
        {
            base.OnResetInfo();
            F_DamageAdditive = 0f;
            B_OneOverride = false;
            I_ClipAdditive = 0;
            F_ClipMultiply = 1f;
            B_ProjectilePenetrate = false;
            F_AllyHealthMultiplierAdditive = 1f;
            F_RecoilMultiply = 1f;
            F_ProjectileSpeedMuiltiply = 1f;
            F_AimMovementStrictMultiply = 1f;
        }
        protected override void OnSetExpireInfo(ExpireBase expire)
        {
            base.OnSetExpireInfo(expire);
            ActionBase action = expire as ActionBase;
            if (action == null)
                return;

            F_DamageAdditive += action.m_DamageAdditive;
            F_RecoilMultiply -= action.F_RecoilReduction;
            F_AimMovementStrictMultiply -= action.F_AimStrictReduction;
            F_ProjectileSpeedMuiltiply += action.F_ProjectileSpeedMultiply;
            F_ClipMultiply += action.F_ClipMultiply;
            B_OneOverride |= action.B_ClipOverride;
            I_ClipAdditive += action.I_ClipAdditive;
            B_ProjectilePenetrate |= action.B_ProjectilePenetrade;
            F_AllyHealthMultiplierAdditive += action.F_AllyHealthMultiplierAdditive;
        }
        protected override void AfterInfoSet()
        {
            base.AfterInfoSet();
            if (F_AllyHealthMultiplierAdditive < .1f) F_AllyHealthMultiplierAdditive = .1f;
            if (F_AimMovementStrictMultiply < 0) F_AimMovementStrictMultiply = 0;
            if (F_RecoilMultiply < 0) F_RecoilMultiply = 0;
        }
        #endregion
        #region Action Helpers
        public override DamageDeliverInfo GetDamageBuffInfo()
        {
            ResetEffect(enum_CharacterEffect.Cloak);
            float randomDamageMultiply = UnityEngine.Random.Range(-GameConst.F_PlayerDamageAdjustmentRange, GameConst.F_PlayerDamageAdjustmentRange);
            DamageDeliverInfo info = DamageDeliverInfo.DamageInfo(m_Entity.m_EntityID, F_DamageMultiply + randomDamageMultiply, F_DamageAdditive);
            m_ActionPlaying.Traversal((ActionBase action) => { action.OnFire(info.I_IdentiyID); });
            return info;
        }

        public void OnPlayerMove(float distance) => m_ActionPlaying.Traversal((ActionBase action) => { action.OnMove(distance); });
        public void OnReloadFinish() => m_ActionPlaying.Traversal((ActionBase action) => { action.OnReloadFinish(); });

        public void OnEntityActivate(EntityBase targetEntity)
        {
            if (targetEntity.m_Controller != enum_EntityController.AI || targetEntity.m_Flag != m_Entity.m_Flag || targetEntity.m_EntityID == m_Entity.m_EntityID)
                return;

            EntityCharacterBase ally = (targetEntity as EntityCharacterBase);
            if (F_AllyHealthMultiplierAdditive > 0)
                ally.m_Health.SetHealthMultiplier(F_AllyHealthMultiplierAdditive);
        }

        public void OnWillDealtDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity) { m_ActionPlaying.Traversal((ActionBase action) => { action.OnDealtDamageSetEffect(damageEntity, damageInfo); action.OnDealtDamageSetDamage(damageEntity, damageInfo); }); }

        public void OnWillReceiveDamage(DamageInfo damageInfo, EntityCharacterBase damageEntity) { m_ActionPlaying.Traversal((ActionBase action) => { action.OnBeforeReceiveDamage(damageInfo); }); }

        public override void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
        {
            base.OnCharacterHealthChange(damageInfo, damageEntity, amountApply);
            if (damageInfo.m_detail.I_SourceID <= 0)
                return;

            if (damageInfo.m_detail.I_SourceID == m_Player.m_EntityID)
            {
                if(amountApply>0)
                    m_ActionPlaying.Traversal((ActionBase action) => { action.OnAfterDealtDemage(damageEntity, damageInfo, amountApply); });
            }

            if (damageEntity.m_EntityID == m_Player.m_EntityID)
            {
                if (amountApply > 0)
                    m_ActionPlaying.Traversal((ActionBase action) => { action.OnAfterReceiveDamage(damageInfo, amountApply); });
                else
                    m_ActionPlaying.Traversal((ActionBase action) => { action.OnReceiveHealing(damageInfo, amountApply); });
            }
        }
        #endregion
        #endregion

        public void RefreshEffects() => m_BuffEffects.Traversal((int expire, SFXEffect effect) => { effect.Play(m_Entity); });
        #region CoinInfo
        public void OnCoinsReceive(int coinAmount)
        {
            m_Coins += coinAmount;
        }
        public void OnCoinsRemoval(int coinAmount)
        {
            m_Coins -= coinAmount;
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

    public class ExpireBase
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
        private Action<ExpireBase> OnExpired;
        public float m_ExpireDuration { get; private set; } = 0;
        public float f_expireCheck { get; private set; }
        public float f_expireLeftScale => f_expireCheck / m_ExpireDuration;
        bool forceExpire = false;
        protected void OnActivate(float _ExpireDuration, Action<ExpireBase> _OnExpired)
        {
            OnExpired = _OnExpired;
            SetDuration(_ExpireDuration);
            forceExpire = false;
        }

        protected void SetDuration(float duration)
        {
            m_ExpireDuration = duration;
            ExpireRefresh();
        }
        public void ExpireRefresh()=> f_expireCheck = m_ExpireDuration;
        public void ForceExpire() => forceExpire = true;
        public virtual void OnTick(float deltaTime)
        {
            if (forceExpire) OnExpired(this);

            if (m_ExpireDuration <= 0) return;
            f_expireCheck -= deltaTime;
            if (f_expireCheck <= 0) OnExpired(this);
        }
    }
    
    public class BuffBase : ExpireBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.Buff;
        public override int m_EffectIndex => m_buffInfo.m_EffectIndex;
        public override enum_ExpireRefreshType m_RefreshType => m_buffInfo.m_AddType;
        public override float m_DamageMultiply => m_buffInfo.m_DamageMultiply;
        public override float m_DamageReduction => m_buffInfo.m_DamageReduction;
        public override int m_Index => m_buffInfo.m_Index;
        public override float m_FireRateMultiply => m_buffInfo.m_FireRateMultiply;
        public override float m_MovementSpeedMultiply => m_buffInfo.m_MovementSpeedMultiply;
        public override float m_ReloadRateMultiply => m_buffInfo.m_ReloadRateMultiply;
        public override float m_HealthDrainMultiply => m_buffInfo.m_HealthDrainMultiply;
        public int I_SourceID { get; private set; }
        SBuff m_buffInfo;
        Func<DamageInfo, bool> OnDOTDamage;
        float f_dotCheck;
        public BuffBase(int sourceID, SBuff _buffInfo, Func<DamageInfo, bool> _OnDOTDamage, Action<ExpireBase> _OnExpired)
        {
            I_SourceID = sourceID;
            m_buffInfo = _buffInfo;
            OnDOTDamage = _OnDOTDamage;
            base.OnActivate(_buffInfo.m_ExpireDuration, _OnExpired);
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
                OnDOTDamage(new DamageInfo(m_buffInfo.m_DamagePerTick, m_buffInfo.m_DamageType, DamageDeliverInfo.Default(I_SourceID)));
            }
        }
    }

    public class ActionBase : ExpireBase
    {
        public override enum_ExpireType m_ExpireType => enum_ExpireType.Action;
        public EntityCharacterPlayer m_ActionEntity { get; private set; }
        public enum_ActionRarity m_rarity { get; private set; } = enum_ActionRarity.Invalid;
        public int m_Identity { get; private set; } = -1;
        public virtual int I_Cost => -1;
        public virtual bool B_ActionAble => true;
        public virtual enum_ActionType m_ActionType => enum_ActionType.Invalid;
        public virtual float Value1 => 0;
        public virtual float Value2 => 0;
        public virtual float Value3 => 0;
        public virtual float m_DamageAdditive => 0;
        public virtual float F_RecoilReduction => 0;
        public virtual float F_AimStrictReduction => 0;
        public virtual float F_ProjectileSpeedMultiply => 0;
        public virtual bool B_ClipOverride => false;
        public virtual int I_ClipAdditive => 0;
        public virtual float F_ClipMultiply => 0;
        public virtual bool B_ProjectilePenetrade => false;
        public virtual float F_AllyHealthMultiplierAdditive => 0;
        public virtual float F_Duration => 0;
        protected ActionBase(int _identity,enum_ActionRarity _level)
        {
            m_Identity = _identity;
            m_rarity = _level;
        }
        public void Activate(EntityCharacterPlayer _actionEntity, Action<ExpireBase> OnExpired) { m_ActionEntity = _actionEntity; OnActivate(F_Duration, OnExpired); }
        public bool B_Upgradable => m_rarity < enum_ActionRarity.Epic;
        public void Upgrade()
        {
            if (m_rarity < enum_ActionRarity.Epic)
                m_rarity++;
        }
        
        #region Interact
        public virtual void OnActivate() { }
        public virtual void OnUseWeaponAbility(ActionBase targetAction) { }
        public virtual void OnBeforeReceiveDamage(DamageInfo info) { }
        public virtual void OnAfterReceiveDamage(DamageInfo info, float amount) { }
        public virtual void OnDealtDamageSetEffect(EntityCharacterBase receiver,DamageInfo info) { }
        public virtual void OnDealtDamageSetDamage(EntityCharacterBase receiver, DamageInfo info) { }
        public virtual void OnAfterDealtDemage(EntityCharacterBase receiver,DamageInfo info, float applyAmount) { }
        public virtual void OnReceiveHealing(DamageInfo info, float applyAmount)  {}
        public virtual void OnReloadFinish() { }
        public virtual void OnFire(int identity) { }
        public virtual void OnMove(float distsance) { }
        public virtual bool OnCheckRevive(ref RangeFloat amount) { return false; }
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
    public class ModelBlink:ISingleCoroutine
    {
        Transform transform;
        Material[] m_materials;
        float f_simulate;
        float f_blinkRate; 
        float f_blinkTime;
        Color c_blinkColor;
        public ModelBlink(Transform BlinkModel, float _blinkRate, float _blinkTime,Color _blinkColor)
        {
            if (BlinkModel == null)
                Debug.LogError("Error! Blink Model Init, BlinkModel Folder Required!");

            transform = BlinkModel;
            Renderer[] renderers=BlinkModel.GetComponentsInChildren<Renderer>();
            m_materials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                m_materials[i] = renderers[i].material;
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
            m_materials.Traversal((Material material) => { material.SetColor("_Color", TCommon.ColorAlpha(c_blinkColor, 0)); });
            SetShow(false);
        }
        
        public void Tick(float deltaTime)
        {
            f_simulate += deltaTime;
            if (f_simulate > f_blinkRate)
            {
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
                    m_materials.Traversal((Material material) => { material.SetColor("_Color", TCommon.ColorAlpha(c_blinkColor, value)); }); }, 1, 0, f_blinkTime));
                f_simulate -= f_blinkRate;
            }
        }
    }
    
    #endregion

    #region BigmapTile
    public class SBigmapLevelInfo
    {
        public LevelBase m_Level { get; private set; } = null;
        public enum_LevelType m_LevelType { get; private set; } = enum_LevelType.Invalid;

        public SBigmapLevelInfo(enum_LevelType _tileType) 
        {
            m_LevelType = _tileType;
        }
        public void SetMap(LevelBase levelSpawned)
        {
            m_Level = levelSpawned;
            SetLevelShow(false);
        }
        public void SetLevelShow(bool show)
        {
            if(m_Level)
            m_Level.SetActivate(show);
        }
        public void ResetTileType(enum_LevelType _tileType)
        {
            m_LevelType = _tileType;
        }
    }
    #endregion

    #region LevelTile
    public class LevelTile 
    {
        public TileAxis m_TileAxis;
        public Vector3 m_Offset => GameExpression.V3_TileAxisOffset(m_TileAxis);
        public virtual enum_LevelItemTileType E_TileType => enum_LevelItemTileType.Empty;
        public enum_LevelItemTileOccupy E_Occupation { get; private set; } = enum_LevelItemTileOccupy.Invalid;

        public LevelTile(TileAxis _axis ,enum_LevelItemTileOccupy _occupy) 
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

    public class LevelTileSub : LevelTile
    {
        public override enum_LevelItemTileType E_TileType => enum_LevelItemTileType.Item;
        public int m_ParentTileIndex { get; private set; }
        public LevelTileSub(LevelTile current, int _parentIndex) : base(current)
        {
            m_ParentTileIndex = _parentIndex;
        }
    }
    public class LevelTileItem : LevelTile
    {
        public override enum_LevelItemTileType E_TileType => enum_LevelItemTileType.Main;
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
        public override enum_LevelItemTileType E_TileType => enum_LevelItemTileType.Border;
        public LevelTileBorder(LevelTile current, int levelItemListIndex, enum_LevelItemType levelItemType, enum_TileDirection _ItemDirection) : base(current,levelItemListIndex,levelItemType,_ItemDirection,null)
        {
        }
    }
    class LevelTileInteract : LevelTile
    {
        public override enum_LevelItemTileType E_TileType => enum_LevelItemTileType.Interact;

        public LevelTileInteract(LevelTile current) : base(current.m_TileAxis,current.E_Occupation)
        {
        }
    }
    #endregion
    
    #region Equipment
    public class EquipmentBase
    {
        public virtual bool B_TargetAlly => false;
        public int I_Index { get; private set; } = -1;
        public virtual bool B_LoopAnim => false;
        protected EntityCharacterBase m_Entity;
        protected Transform attacherFeet => m_Entity.transform;
        protected Transform attacherHead => m_Entity.tf_Head;
        protected Func<DamageDeliverInfo> GetDamageDeliverInfo;
        protected CharacterInfoManager m_Info
        {
            get
            {
                if (m_Entity == null)
                    Debug.LogError("Null Entity Controlling?");
                return m_Entity.m_CharacterInfo;
            }
        }
        public EquipmentBase(int equipmentIndex, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo)
        {
            I_Index = equipmentIndex;
            m_Entity = _controller;
            GetDamageDeliverInfo = _GetBuffInfo;
        }
        protected virtual Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target) => _target.tf_Head.position;
        public void Play(bool _preAim, EntityCharacterBase _target) => Play(_target,GetTargetPosition(_preAim, _target));
        public virtual void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {

        }
        public virtual void OnSetAttack(bool play)
        {
        }
        public virtual void Tick(float deltaTime)
        {

        }
        public static EquipmentBase AcquireEquipment(int weaponIndex, EntityCharacterBase _entity, Func<DamageDeliverInfo> GetDamageBuffInfo)
        {
            SFXEquipmentBase weaponInfo = GameObjectManager.GetEquipmentData<SFXEquipmentBase>(weaponIndex);
            SFXProjectile projectile = weaponInfo as SFXProjectile;
            if (projectile)
            {
                switch (projectile.E_ProjectileType)
                {
                    default: Debug.LogError("Invalid Type:" + projectile.E_ProjectileType); break;
                    case enum_ProjectileFireType.Single: return new EquipmentBarrageRange(weaponIndex,projectile, _entity, GetDamageBuffInfo); 
                    case enum_ProjectileFireType.MultipleFan: return new EquipmentBarrageMultipleFan(weaponIndex,projectile, _entity, GetDamageBuffInfo); 
                    case enum_ProjectileFireType.MultipleLine: return new EquipmentBarrageMultipleLine(weaponIndex,projectile, _entity, GetDamageBuffInfo); 
                }
            }

            SFXCast cast = weaponInfo as SFXCast;
            if (cast)
            {
                switch (cast.E_CastType)
                {
                    default: Debug.LogError("Invalid Type:" + cast.E_CastType); break;
                    case enum_CastControllType.CastFromOrigin: return new EquipmentCaster(weaponIndex,cast, _entity, GetDamageBuffInfo);
                    case enum_CastControllType.CastSelfDetonate: return new EquipmentCasterSelfDetonateAnimLess(weaponIndex,cast, _entity, GetDamageBuffInfo, _entity.tf_Model.Find("BlinkModel")); 
                    case enum_CastControllType.CastControlledForward: return new EquipmentCasterControlled(weaponIndex,cast, _entity, GetDamageBuffInfo);
                    case enum_CastControllType.CastAtTarget: return new EquipmentCasterTarget(weaponIndex,cast, _entity, GetDamageBuffInfo);
                }
            }

            SFXBuffApply buffApply = weaponInfo as SFXBuffApply;
            if (buffApply)
                return new BuffApply(weaponIndex,buffApply, _entity, GetDamageBuffInfo);

            SFXSubEntitySpawner entitySpawner = weaponInfo as SFXSubEntitySpawner;
            if (entitySpawner)
                return new EquipmentEntitySpawner(weaponIndex,entitySpawner, _entity, GetDamageBuffInfo);

            SFXShield shield = weaponInfo as SFXShield;
            if (shield)
                return new EquipmentShieldAttach(weaponIndex,shield, _entity, GetDamageBuffInfo);

            return null;
        }
    }
    public class EquipmentCaster : EquipmentBase
    {
        protected enum_CastTarget m_CastAt { get; private set; }
        protected bool m_castForward { get; private set; }
        public EquipmentCaster(int equipmentIndex,SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            m_CastAt = _castInfo.E_CastTarget;
            m_castForward = _castInfo.B_CastForward;
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Transform castAt = GetCastAt(m_Entity);
            GameObjectManager.SpawnEquipment<SFXCast>(I_Index, castAt.position, m_castForward?castAt.forward:Vector3.up).Play(GetDamageDeliverInfo());
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
    public class EquipmentCasterTarget : EquipmentCaster
    {
        public EquipmentCasterTarget(int equipmentIndex,SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex,_castInfo, _controller, _GetBuffInfo)
        {
        }
        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target)
        {
            Transform castAt = GetCastAt(_target);
            return LevelManager.NavMeshPosition(castAt.position + TCommon.RandomXZSphere(m_Entity.F_AttackSpread)) + new Vector3(0, castAt.position.y, 0);
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            GameObjectManager.SpawnEquipment<SFXCast>(I_Index, _calculatedPosition,m_castForward?m_Entity.tf_Weapon.forward:Vector3.up).Play(GetDamageDeliverInfo());
        }
    }
    public class EquipmentCasterSelfDetonateAnimLess : EquipmentCaster
    {
        ModelBlink m_Blink;
        float timeElapsed;
        bool b_activating;
        public EquipmentCasterSelfDetonateAnimLess(int equipmentIndex,SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo, Transform _blinkModels) : base(equipmentIndex,_castInfo, _controller, _GetBuffInfo)
        {
            m_Blink = new ModelBlink(_blinkModels, .25f, .25f,Color.red);
            timeElapsed = 0;
            b_activating = false;
        }
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (!b_activating||m_Entity.m_Health.b_IsDead)
                return;
            timeElapsed += deltaTime;
            float timeMultiply = 2f * (timeElapsed / 2f);
            m_Blink.Tick(Time.deltaTime * timeMultiply);
            if (timeElapsed > 2f)
            {
                GameObjectManager.SpawnEquipment<SFXCast>(I_Index, attacherHead.position, attacherHead.forward).Play(GetDamageDeliverInfo());
                m_Entity.m_HitCheck.TryHit(new DamageInfo(m_Entity.m_Health.F_TotalEHP, enum_DamageType.Basic,DamageDeliverInfo.Default(-1)));
                b_activating = false;
            }
        }
        public override void OnSetAttack(bool play)
        {
            base.OnSetAttack(play);
            if (!play || b_activating)
                return;

            timeElapsed = 0;
            b_activating = true;
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            return;
        }
    }
    public class EquipmentCasterControlled : EquipmentCaster
    {
        public override bool B_LoopAnim => true;
        SFXCast m_Cast;
        public EquipmentCasterControlled(int equipmentIndex,SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex,_castInfo, _controller, _GetBuffInfo)
        {
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            if (!m_Cast)
            {
                m_Cast = GameObjectManager.SpawnEquipment<SFXCast>(I_Index, m_Entity.tf_Weapon.position, m_Entity.tf_Weapon.forward);
                m_Cast.PlayControlled(m_Entity.m_EntityID, m_Entity, attacherHead, DamageDeliverInfo.Default(m_Entity.m_EntityID));
            }

            if (m_Cast)
                m_Cast.OnControlledCheck(GetDamageDeliverInfo());
        }

        public override void OnSetAttack(bool play)
        {
            if (m_Cast)
            {
                m_Cast.StopControlled();
                m_Cast = null;
            }
        }
    }
    public class EquipmentBarrageRange : EquipmentBase
    {
        protected float f_projectileSpeed { get; private set; }
        protected RangeInt m_CountExtension { get; private set; }
        protected float m_OffsetExtension { get; private set; }
        int i_muzzleIndex;
        AudioClip m_MuzzleClip;

        public EquipmentBarrageRange(int equipmentIndex,SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            i_muzzleIndex = projectileInfo.I_MuzzleIndex;
            m_MuzzleClip = projectileInfo.AC_MuzzleClip;
            f_projectileSpeed = projectileInfo.F_Speed;
            m_CountExtension = projectileInfo.RI_CountExtension;
            m_OffsetExtension = projectileInfo.F_OffsetExtension;
        }

        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
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
    public class EquipmentBarrageMultipleLine : EquipmentBarrageRange
    {
        public EquipmentBarrageMultipleLine(int equipmentIndex,SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex,projectileInfo, _controller, _GetBuffInfo)
        {
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
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
    public class EquipmentBarrageMultipleFan : EquipmentBarrageRange
    {
        public EquipmentBarrageMultipleFan(int equipmentIndex,SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex,projectileInfo, _controller, _GetBuffInfo)
        {
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Vector3 startPosition = m_Entity.tf_Weapon.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            int waveCount = m_CountExtension.Random();
            float beginAnle = -m_OffsetExtension * (waveCount - 1) / 2f;
            float distance = TCommon.GetXZDistance(m_Entity.tf_Weapon.position, _calculatedPosition);
            SpawnMuzzle(startPosition, direction);
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 fanDirection = direction.RotateDirection(Vector3.up, beginAnle + i * m_OffsetExtension);
                FireBullet(m_Entity.tf_Weapon.position, fanDirection, m_Entity.tf_Weapon.position + fanDirection * distance);
            }
        }
    }
    public class BuffApply : EquipmentBase
    {
        public override bool B_TargetAlly => true;
        SBuff m_buffInfo;
        SFXBuffApply m_Effect;
        public BuffApply(int equipmentIndex,SFXBuffApply buffApplyinfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            m_buffInfo = GameDataManager.GetPresetBuff(buffApplyinfo.I_BuffIndex);
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            if (!m_Effect || !m_Effect.B_Playing)
                m_Effect = GameObjectManager.SpawnEquipment<SFXBuffApply>(I_Index, m_Entity.tf_Weapon.position, Vector3.up);

            m_Effect.Play(m_Entity.m_EntityID, m_buffInfo, m_Entity.tf_Weapon, _target);
        }
    }
    public class EquipmentEntitySpawner : EquipmentBase
    {
        public EquipmentEntitySpawner(int equipmentIndex,SFXSubEntitySpawner spawner, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
            startHealth = 0;
        }
        Action<EntityCharacterBase> OnSpawn;
        float startHealth;
        public void SetOnSpawn(float _startHealth,Action<EntityCharacterBase> _OnSpawn)
        {
            OnSpawn = _OnSpawn;
            startHealth = _startHealth;
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            GameObjectManager.SpawnEquipment<SFXSubEntitySpawner>(I_Index, m_Entity.tf_Weapon.position, Vector3.up).Play(m_Entity.m_EntityID, m_Entity.m_Flag,GetDamageDeliverInfo, startHealth, OnSpawn);
        }
    }
    public class EquipmentShieldAttach : EquipmentBase
    {
        public override bool B_TargetAlly => true;
        public EquipmentShieldAttach(int equipmentIndex,SFXShield shield, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(equipmentIndex, _controller, _GetBuffInfo)
        {
        }
        Action<SFXShield> OnSpawn;
        public void SetOnSpawn(Action<SFXShield> _OnSpawn)
        {
            OnSpawn = _OnSpawn;
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            SFXShield shield = GameObjectManager.SpawnEquipment<SFXShield>(I_Index, m_Entity.tf_Model.position, Vector3.up);
            OnSpawn?.Invoke(shield);
            shield.Attach(_target);
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


    public class UIC_Numeric
    {
        static readonly AtlasLoader m_InGameSprites = TResources.GetUIAtlas_Numeric();
        public static Sprite GetNumeric(char numeric) => m_InGameSprites["numeric_" + numeric];
        public Transform transform { get; private set; }
        UIT_GridControllerMono<Image> m_Grid;
        int currentAmount=-1;
        public UIC_Numeric(Transform _transform)
        {
            transform = _transform;
            m_Grid = new UIT_GridControllerMono<Image>(transform);
        }

        public void SetAmount(int _amount)
        {
            if (currentAmount == _amount)
                return;
            currentAmount = _amount;

            m_Grid.ClearGrid();
            SetNumeric( _amount.ToString());
        }
        public void SetNumeric(string _numeric)
        {
            for (int i = 0; i < _numeric.Length; i++)
                m_Grid.GetOrAddItem(i).sprite = GetNumeric(_numeric[i]);
        }
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
        ObjectPoolSimple<int,Transform> m_Grid;
        Dictionary<int, RarityLevel> m_Levels = new Dictionary<int, RarityLevel>();
        public UIC_RarityLevel(Transform _transform)
        {
            transform = _transform;
            m_Grid = new ObjectPoolSimple<int, Transform>(transform.Find("GridItem").gameObject,transform,(Transform trans,int identity)=>trans,(Transform trans)=>trans);
            m_Grid.ClearPool();
            TCommon.TraversalEnum((enum_ActionRarity rarity) => { m_Levels.Add((int)rarity,new RarityLevel( m_Grid.AddItem((int)rarity))); });
        }
        public void SetRarity(enum_ActionRarity level)
        {
            m_Levels.Traversal((int index, RarityLevel rarity) => rarity.SetHighlight(index <= (int)level));
        }
    }
    public class UI_WeaponActionHUD
    {
        public Transform transform { get; private set; }
        UIT_TextExtend m_WeaponActionName;
        UIC_RarityLevel m_WeaponActionRarity;
        public UI_WeaponActionHUD(Transform _transform)
        {
            transform = _transform;
            m_WeaponActionName = _transform.Find("ActionName").GetComponent<UIT_TextExtend>();
            m_WeaponActionRarity = new UIC_RarityLevel(_transform.Find("ActionRarity"));
        }

        public void SetInfo(ActionBase action)
        {
            bool showWeaponAction = action != null;
            m_WeaponActionRarity.transform.SetActivate(showWeaponAction);
            m_WeaponActionName.autoLocalizeText = showWeaponAction ? action.GetNameLocalizeKey() : "UI_Weapon_ActionInvalidName";
            if (showWeaponAction) m_WeaponActionRarity.SetRarity(action.m_rarity);
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
    #endregion
    #endregion
}
