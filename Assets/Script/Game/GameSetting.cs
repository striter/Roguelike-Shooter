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

        public const int I_ActionHoldCount = 3;
        public const float F_MaxActionEnergy = 5f;
        public const float I_MaxArmor = 99999;
        public const float F_RestoreActionEnergy = 0.001f;
        public const float F_ActionShuffleCost = 2f;
        public const float F_ActionShuffleCooldown = 10f;

        public const float F_AimMovementReduction = .6f;
        public const float F_MovementReductionDuration = .1f;
        public const int I_ProjectileMaxDistance = 100;
        public const int I_ProjectileBlinkWhenTimeLeftLessThan = 3;
        public const float F_AimAssistDistance = 100f;
        public const short I_BoltLastTimeAfterHit = 5;

        public const int I_BurstFirePelletsOnceTrigger = 3;       //Times While Burst Fire
        public const int I_ProjectileSpreadAtDistance = 100;       //Meter,  Bullet Spread In A Circle At End Of This Distance

        public const float F_PlayerDamageAdjustmentRange = .1f;
        public const float F_PlayerCameraSmoothParam = 1f;     //Camera Smooth Param For Player .2 is suggested

        public const float F_AIDamageTranslate = 0;   //.003f;
        public const float F_AIMovementCheckParam = .3f;
        public const float F_AITargetCheckParam = 3f;       //AI Retarget Duration,3 is suggested
        public const float F_AITargetCalculationParam = 1f;       //AI Target Param Calculation Duration, 1 is suggested;
        public const float F_AIMaxRepositionDuration = 5f;
        public const int I_AIIdlePercentage = 50;

        public const int I_EnermyCountWaveFinish = 0;       //When Total Enermy Count Reaches This Amount,Wave Finish
        public const int I_EnermySpawnDelay = 4;        //Enermy Spawn Delay Time 

        public const float F_PickupAcceleration = 8f;
        public const int I_HealthPickupAmount = 25;
        public const int I_ArmorPickupAmount = 25;
        public const int I_HealthTradeAmount = 50;

        public const float F_LevelTileSize = 2f;        //Cube Size For Level Tiles

        public const int I_CampFarmPlot4UnlockDifficulty = 3;
        public const int I_CampFarmPlot5UnlockDifficulty = 10;
        public const int I_CampFarmPlot6UnlockTechPoints = 3000;
        public const int I_CampFarmItemAcquire = 50;
        public const float F_CampFarmItemTickAmount = 0.05f;

        public const int I_CampActionStorageNormalCount = 10;
        public const int I_CampActionStorageOutstandingCount = 30;
        public const int I_CampActionStorageEpicCount = 60;

        public const int I_CampActionCreditGainPerRequestSurplus = 100;
        public const int I_CampActionStorageRequestStampDuration = 30;//36000 //10 hours
    }

    public static class GameExpression
    {
        public static int GetPlayerEquipmentIndex(int actionIndex) => actionIndex * 10;
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
        public static Vector3 V3_RangeSpreadDirection(Vector3 aimDirection, float spread, Vector3 up, Vector3 right) => (aimDirection * GameConst.I_ProjectileSpreadAtDistance + up * UnityEngine.Random.Range(-spread, spread) + right * UnityEngine.Random.Range(-spread, spread)).normalized;

        public static int GetEquipmentSubIndex(int weaponIndex) => weaponIndex + 1;
        public static SBuff GetEnermyGameDifficultyBuffIndex(int difficulty) => SBuff.CreateEnermyChallengeDifficultyBuff(difficulty, .04f * (difficulty - 1));
        public static float GetAIBaseHealthMultiplier(int gameDifficulty) => 0.99f + 0.01f * gameDifficulty;
        public static float GetAIMaxHealthMultiplier(enum_StageLevel stageDifficulty) => (int)stageDifficulty ;

        public static float GetActionEnergyRevive(float damageApply) => damageApply * .0025f;

        public static float GetAIIdleDuration() => UnityEngine.Random.Range(1f, 2f);

        public static float GetResultProgress(bool win, enum_StageLevel _stage, int _battleLevelEntered) => win ? 1f : (.33f * ((int)_stage - 1) +.066f*_battleLevelEntered);
        public static float GetResultLevelScore(enum_StageLevel _stage, int _levelPassed) => 200 * ((int)_stage - 1) + 20 * (_levelPassed - 1);
        public static float GetResultKillScore(int _enermyKilled) => _enermyKilled * 1;
        public static float GetResultDifficultyBonus(int _difficulty) =>1f+ _difficulty * .05f;
        public static float GetResultRewardCredits(float _totalScore) => _totalScore;

        public static RangeInt GetTradePrice(enum_Interaction interactType, enum_RarityLevel level)
        {
            switch (interactType)
            {
                default: Debug.LogError("No Coins Can Phrase Here!"); return new RangeInt(0, -1);
                case enum_Interaction.PickupHealth:
                    return new RangeInt(10, 0);
                case enum_Interaction.PickupAction:
                case enum_Interaction.Weapon:
                    switch (level)
                    {
                        default: Debug.LogError("Invalid Level!"); return new RangeInt(0, -1);
                        case enum_RarityLevel.Normal: return new RangeInt(8, 4);
                        case enum_RarityLevel.OutStanding: return new RangeInt(16, 8);
                        case enum_RarityLevel.Epic: return new RangeInt(24, 12);
                    }
            }
        }
        public static int GetActionRemovePrice(enum_StageLevel stage, int removeTimes) => 10 * (removeTimes + 1) ;
        public static int GetActionUpgradePrice(enum_StageLevel stage, int upgradeTimes) => 10 * (upgradeTimes + 1) ;
        
        public static enum_RarityLevel GetStartWeaponPerkRarity(this enum_StageLevel stageLevel) => (stageLevel - 1).ToRarity();
        public static enum_RarityLevel GetTradeWeaponPerkRarity(this enum_StageLevel stageLevel) => stageLevel.ToRarity();
        public static enum_RarityLevel GetBattleTradeActionRarity(this enum_StageLevel stageLevel) => stageLevel.ToRarity() == enum_RarityLevel.Epic ? enum_RarityLevel.Epic : (stageLevel + 1).ToRarity();
        public static StageInteractGenerateData GetInteractGenerate(enum_StageLevel level)
        {
            switch (level)
            {
                default: return StageInteractGenerateData.Create(new Dictionary<enum_RarityLevel, int>(), new Dictionary<enum_RarityLevel, int>(), new Dictionary<enum_CharacterType, CoinsGenerateData>());
                case enum_StageLevel.Rookie:
                    return StageInteractGenerateData.Create(
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.Normal, 100 }, { enum_RarityLevel.OutStanding, 0 } },    //宝箱等级概率
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.Normal, 75 }, { enum_RarityLevel.OutStanding, 25 } },    //交易等级概率
                    new Dictionary<enum_CharacterType, CoinsGenerateData>() {
                     { enum_CharacterType.SubHidden, CoinsGenerateData.Create( 0,0, 0, new RangeInt(0, 0)) },     //实体掉落生成概率 类型,血,护甲,金币,金币数值范围
                     { enum_CharacterType.Fighter, CoinsGenerateData.Create( 8,20, 20, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Rookie, CoinsGenerateData.Create( 8,15, 20, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Veteran, CoinsGenerateData.Create( 8,15, 30, new RangeInt(3, 3)) },
                     { enum_CharacterType.AOECaster, CoinsGenerateData.Create( 8,15, 50, new RangeInt(4, 4)) },
                     { enum_CharacterType.Elite, CoinsGenerateData.Create( 8,15, 100, new RangeInt(6, 6)) }});
                case enum_StageLevel.Veteran:
                    return StageInteractGenerateData.Create(
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.OutStanding, 100 }, { enum_RarityLevel.Epic, 0 } },    //宝箱等级概率
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.OutStanding, 75 }, { enum_RarityLevel.Epic, 25 } },    //交易等级概率
                    new Dictionary<enum_CharacterType, CoinsGenerateData>() {
                     { enum_CharacterType.SubHidden, CoinsGenerateData.Create( 0,0, 0, new RangeInt(0, 0)) },     //实体掉落生成概率 类型,血,护甲,金币,金币数值范围
                     { enum_CharacterType.Fighter, CoinsGenerateData.Create( 8,15, 20, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Rookie, CoinsGenerateData.Create( 8,15, 20, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Veteran, CoinsGenerateData.Create( 8,15, 30, new RangeInt(3, 3)) },
                     { enum_CharacterType.AOECaster, CoinsGenerateData.Create( 8,15, 50, new RangeInt(4, 4)) },
                     { enum_CharacterType.Elite, CoinsGenerateData.Create( 8,15, 100, new RangeInt(6, 6)) }});
                case enum_StageLevel.Ranger:
                    return StageInteractGenerateData.Create(
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.OutStanding, 0 }, { enum_RarityLevel.Epic, 100 } },    //宝箱等级概率
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.OutStanding, 0 }, { enum_RarityLevel.Epic, 100 } },    //交易等级概率
                    new Dictionary<enum_CharacterType, CoinsGenerateData>() {
                     { enum_CharacterType.SubHidden, CoinsGenerateData.Create( 0,0, 0, new RangeInt(0, 0)) },     //实体掉落生成概率 类型,血,护甲,金币,金币数值范围
                     { enum_CharacterType.Fighter, CoinsGenerateData.Create( 8,15, 20, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Rookie, CoinsGenerateData.Create( 8,15, 20, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Veteran, CoinsGenerateData.Create( 8,15, 30, new RangeInt(3, 3)) },
                     { enum_CharacterType.AOECaster, CoinsGenerateData.Create( 8,15, 50, new RangeInt(4, 4)) },
                     { enum_CharacterType.Elite, CoinsGenerateData.Create( 8,15, 100, new RangeInt(6, 6)) }});
            }
        }

        public static readonly Dictionary<enum_CampFarmItemStatus, int> GetFarmGeneratePercentage = new Dictionary< enum_CampFarmItemStatus,int>() { {  enum_CampFarmItemStatus.Progress1 ,60},{enum_CampFarmItemStatus.Progress2 ,30},{enum_CampFarmItemStatus.Progress3 ,6},{enum_CampFarmItemStatus.Progress4,3},{ enum_CampFarmItemStatus.Progress5,1} };   //Farm生成等级百分比
        public static readonly Dictionary<enum_CampFarmItemStatus, CampFarmItemData> GetFarmItemInfo = new Dictionary<enum_CampFarmItemStatus, CampFarmItemData> { { enum_CampFarmItemStatus.Progress1, CampFarmItemData.Create(82800, .1f / 60f) }, { enum_CampFarmItemStatus.Progress2, CampFarmItemData.Create(82800, .2f / 60f) }, { enum_CampFarmItemStatus.Progress3, CampFarmItemData.Create(82800, .3f / 60f) }, { enum_CampFarmItemStatus.Progress4, CampFarmItemData.Create(82800, .5f / 60f) }, { enum_CampFarmItemStatus.Progress5, CampFarmItemData.Create(82800, 1f / 60f) } };      //Farm 等级,持续时间,每秒Credit
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

        public static readonly RangeInt I_CampActionStorageRequestAmount = new RangeInt(1,4);
        public static readonly List<int> I_CampActionStorageDefault = new List<int> { 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10009, 10010 };
    }

    public static class UIConst
    {
        public const int I_AmmoCountToSlider = 180;      //Ammo UI,While Clip Above This Will Turn To Be Slider大于此值变成白条

        public const float F_UIMaxArmor = 100f;
        public const float F_MapAnimateTime = 1.6f;

        public const float F_UIDamageStartOffset = 20f; //血显示区域范围

        public const float F_UIActionBattlePressDuration = .3f; //查看卡片说明长按时间
    }

    public static class UIExpression
    {
        public static Color ActionRarityColor(this enum_RarityLevel level)
        {
            switch (level) {
                case enum_RarityLevel.Normal:
                    return Color.green;
                case enum_RarityLevel.OutStanding:
                    return Color.blue;
                case enum_RarityLevel.Epic:
                    return Color.yellow;
                default:
                    return Color.magenta;
            }
        }
        public static Color TipsColor(this enum_UITipsType type)
        {
            switch(type)
            {
                case enum_UITipsType.Normal:
                    return Color.white;
                case enum_UITipsType.Warning:
                    return Color.yellow;
                case enum_UITipsType.Error:
                    return Color.red;
                default:
                    return Color.magenta;
            }
        }

        public static float F_WeaponStability(float baseRecoilScore) => 100f - baseRecoilScore * 6.25f;
        public static float F_WeaponDamageValue(float baseDamage) => baseDamage / 150f;
        public static float F_WeaponFireRateValue(float baseRPM) => baseRPM / 400f;
        public static float F_WeaponStabilityValue(float baseRecoilScore) => F_WeaponStability(baseRecoilScore) / 100f;
        public static float F_WeaponProjectileSpeedValue(float baseProjectileSpeed) => baseProjectileSpeed / 100f;
    }

    public static class GameEnumConvertions
    {
        public static enum_RarityLevel ToRarity(this enum_StageLevel stageLevel) => (enum_RarityLevel)stageLevel;
        public static enum_LevelGenerateType ToPrefabType(this enum_TileType type)
        {
            switch (type)
            {
                default: Debug.LogError("Please Edit This Please:" + type.ToString()); return enum_LevelGenerateType.Invalid;
                case enum_TileType.Battle:
                case enum_TileType.End:
                    return enum_LevelGenerateType.Big;
                case enum_TileType.BattleTrade:
                case enum_TileType.ActionAdjustment:
                case enum_TileType.CoinsTrade:
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
                case enum_HitCheck.Interact: return GameLayer.I_Interact;
            }
        }
    }

    public static class UIEnumConvertions
    {
        public static string GetSpriteName(this enum_UI_TileBattleStatus status) => "map_info_battle_" + status.ToString();
        public static string GetSpriteName(this enum_TileType type)
        {
            switch (type)
            {
                default: return "map_tile_type_Invalid";
                case enum_TileType.Start:
                case enum_TileType.Battle:
                case enum_TileType.BattleTrade:
                case enum_TileType.CoinsTrade:
                case enum_TileType.ActionAdjustment:
                case enum_TileType.End:
                    return "map_tile_type_" + type.ToString();
            }
        }
        public static string GetIconSprite(this enum_ActionType type)
        {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!"+type.ToString());  return "";
                case enum_ActionType.Basic: return "action_icon_basic";
                case enum_ActionType.Device: return "action_icon_device";
                case enum_ActionType.Equipment: return "action_icon_equipment";
            }
        }
        public static string GetNameBGSprite(this enum_ActionType type)
        {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!" + type.ToString()); return "";
                case enum_ActionType.Basic: return "action_bottom_basic";
                case enum_ActionType.Device: return "action_bottom_device";
                case enum_ActionType.Equipment: return "action_bottom_equipment";
            }
        }
        public static string GetCostBGSprite(this enum_ActionType type)
        {
            switch (type)
            {
                default: Debug.LogError("Invalid Pharse Here!" + type.ToString()); return "";
                case enum_ActionType.Basic: return "action_cost_basic";
                case enum_ActionType.Device: return "action_cost_device";
                case enum_ActionType.Equipment: return "action_cost_equipment";
            }
        }
        public static string GetMainSprite(this EntityCharacterPlayer player)
        {
            string spriteName = "main_fire";
            if (player.m_Interact != null&&player.m_Interact.B_InteractEnable)
                switch (player.m_Interact.m_InteractType)
                {
                    case enum_Interaction.Invalid: Debug.LogError("Invalid Pharse Here!"); break;
                    default: spriteName = "main_pickup"; break;
                    case enum_Interaction.CampFarm:
                    case enum_Interaction.ActionAdjustment:
                        spriteName = "main_chat"; break;
                }
            return spriteName;
        }
        public static string GetSpriteName(this enum_PlayerWeapon weapon) => ((int)weapon).ToString();
        public static string GetCordinates(this TileAxis axis)
        {
            string x = axis.X.ToString();
            char y = (char)(axis.Y + 65);
            return x + "-" + y;
        }
        public static enum_UI_TileBattleStatus GetBattleStatus(this enum_TileType type)
        {
            switch (type)
            {
                default: return enum_UI_TileBattleStatus.Clear;
                case enum_TileType.BattleTrade: return enum_UI_TileBattleStatus.Patrol;
                case enum_TileType.Battle:
                case enum_TileType.End: return enum_UI_TileBattleStatus.HardBattle;
            }
        }
        public static string GetBattlePercentage(this enum_UI_TileBattleStatus status)
        {
            switch (status)
            {
                default: return "¿";
                case enum_UI_TileBattleStatus.Clear: return "0%";
                case enum_UI_TileBattleStatus.Overwatch: return "25%";
                case enum_UI_TileBattleStatus.Patrol: return "50%";
                case enum_UI_TileBattleStatus.HardBattle: return "100%";
            }
        }
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
        public static string GetLocalizeKey(this enum_TileType type) => "UI_TileType_" + type;
        public static string GetLocalizeKey(this enum_RarityLevel rarity) => "UI_Rarity_" + rarity;
        public static string GetLocalizeKey(this enum_Option_FrameRate frameRate) => "UI_Option_" + frameRate;
        public static string GetLocalizeKey(this enum_Option_JoyStickMode joystick) => "UI_Option_" + joystick;
        public static string GetLocalizeKey(this enum_Option_LanguageRegion region) => "UI_Option_" + region;
        public static string GetLocalizeKey(this enum_UI_TileBattleStatus status) => "UI_Battle_" + status;
        public static string GetLocalizeKey(this enum_CampFarmItemStatus status) => "UI_Farm_" + status;

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
    }

    enum enum_BC_UIStatus
    {
        Invalid = -1,
        UI_PlayerCommonStatus,
        UI_PlayerInteractStatus,
        UI_PlayerHealthStatus,
        UI_PlayerAmmoStatus,
        UI_PlayerActionStatus,
        UI_PlayerExpireStatus,
        UI_PlayerWeaponStatus,

        UI_CampDataStatus,

        UI_PageOpen,
        UI_PageClose,
    }
    #endregion

    #region GameEnum
    public enum enum_StageLevel { Invalid = -1, Rookie = 1, Veteran = 2, Ranger = 3 }

    public enum enum_BattleDifficulty { Invalid = -1, Peaceful = 0, Eazy = 1, Normal = 2, Hard = 3, End = 4, BattleTrade = 10, }

    public enum enum_EntityController { Invalid = -1, None = 1, Player = 2, AI = 3, Device = 4, }

    public enum enum_EntityFlag { Invalid = -1, None = 0, Player = 1, Enermy = 2, Neutal = 3, }

    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, Interact = 4, }

    public enum enum_TileLocking { Invalid = -1, Unseen = 0, Unlockable = 1, Unlocked = 2, Locked = 3, }

    public enum enum_Style { Invalid = -1, Forest = 1, Desert = 2, Iceland = 3, Horde = 4, Undead = 5, }

    public enum enum_TileType { Invalid = -1, Start = 0, Battle = 1, End = 2, CoinsTrade = 11, ActionAdjustment = 12, BattleTrade = 13, }

    public enum enum_LevelItemType { Invalid = -1, LargeMore, LargeLess, MediumMore, MediumLess, SmallMore, SmallLess, ManmadeMore, ManmadeLess, NoCollisionMore, NoCollisionLess, BorderLinear, BorderOblique, }

    public enum enum_LevelTileType { Invaid = -1, Empty, Main, Border, Item, Interact, }

    public enum enum_LevelTileOccupy { Invalid = -1, Inner, Outer, Border, }

    public enum enum_LevelGenerateType { Invalid = -1, Big = 1, Small = 2 }

    public enum enum_CharacterType { Invalid = -1, Fighter = 1, Shooter_Rookie = 2, Shooter_Veteran = 3, AOECaster = 4, Elite = 5, SubHidden = 99 }

    public enum enum_Interaction { Invalid = -1, ActionChest ,
        GameBegin,Bonfire, ActionChestStart, ContainerTrade, ContainerBattle, PickupCoin, PickupHealth,PickupHealthPack, PickupArmor, PickupAction, Weapon,PerkUpgrade, ActionAdjustment, Portal, GameEnd,
        CampBegin,CampStage, CampDifficult,CampFarm,CampAction,CampEnd, }

    public enum enum_TriggerType { Invalid = -1, Single = 1, Auto = 2, Burst = 3, Pull = 4, Store = 5, }

    public enum enum_ProjectileFireType { Invalid = -1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, CastSelfDetonate = 4, }

    public enum enum_CastTarget { Invalid=-1,Head=1,Weapon=2,Feet=3}

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, ForwardTrapezium = 4, }

    public enum enum_HealthChangeMessage { Invalid = -1, Default = 0, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Basic = 1, ArmorOnly = 2, HealthOnly = 3,}

    public enum enum_CharacterEffect { Invalid = -1, Freeze = 1, Cloak = 2, Scan = 3, }

    public enum enum_ExpireType { Invalid = -1, Buff = 1, Action = 2, }

    public enum enum_ExpireRefreshType { Invalid = -1, AddUp = 1, Refresh = 2,RefreshIdentity=3, }

    public enum enum_RarityLevel { Invalid = -1, Normal = 1, OutStanding = 2, Epic = 3, }

    public enum enum_ActionType { Invalid = -1, Basic = 1,Device=2,Equipment=3, WeaponPerk = 4, }

    public enum enum_EffectAttach { Invalid = -1,  Head = 1, Feet = 2, Weapon = 3,}

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
//        M16A4 = 4002,
        AKM = 4003,
        //Pistol
        P92 = 5001,
        DE = 5002,
        //Shotgun
        XM1014 = 6001,
        S686 = 6002,
        //Heavy Weapon
        Crossbow = 7001,
//        RocketLauncher = 7002,
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

    public enum enum_GameMusic { Invalid=-1, Relax=1, Fight=2, Win=3, Lost=4,}

    public enum enum_Option_FrameRate { Invalid = -1, Normal = 45, High = 60, }

    public enum enum_Option_ScreenEffect { Invalid=-1,Normal,High,Epic}

    public enum enum_CampFarmItemStatus { Invalid=-1, Empty = 0, Locked=1 , Decayed = 2, Progress1=10,Progress2,Progress3,Progress4,Progress5}

    public enum enum_UITipsType { Invalid=-1,Normal=0,Warning=1,Error=2}
    #endregion
    
    #region Structs
    #region Default Readonly
    public struct CoinsGenerateData
    {
        public int m_HealthRate { get; private set; }
        public int m_ArmorRate { get; private set; }
        public int m_CoinRate { get; private set; }
        public RangeInt m_CoinRange { get; private set; }
        public static CoinsGenerateData Create(int healthRate, int armorRate, int coinRate, RangeInt coinAmount) => new CoinsGenerateData() { m_HealthRate = healthRate, m_ArmorRate = armorRate, m_CoinRate = coinRate, m_CoinRange = coinAmount };
    }

    public struct StageInteractGenerateData
    {
        Dictionary<enum_CharacterType, CoinsGenerateData> m_CoinRate;
        Dictionary<enum_RarityLevel, int> m_ActionRate;
        Dictionary<enum_RarityLevel, int> m_TradeRate;
        public bool CanGenerateHealth(enum_CharacterType entityType) => TCommon.RandomPercentage() <= m_CoinRate[entityType].m_HealthRate;
        public bool CanGenerateArmor(enum_CharacterType entityType) => TCommon.RandomPercentage() <= m_CoinRate[entityType].m_ArmorRate;
        public int GetCoinGenerate(enum_CharacterType entityType) => TCommon.RandomPercentage() <= m_CoinRate[entityType].m_CoinRate ? m_CoinRate[entityType].m_CoinRange.Random() : -1;
        public enum_RarityLevel GetActionRarityLevel(System.Random seed) => TCommon.RandomPercentage(m_ActionRate, seed);
        public enum_RarityLevel GetTradeRarityLevel(System.Random seed) => TCommon.RandomPercentage(m_TradeRate, seed);
        public static StageInteractGenerateData Create(Dictionary<enum_RarityLevel, int> _actionRate, Dictionary<enum_RarityLevel, int> _tradeRate, Dictionary<enum_CharacterType, CoinsGenerateData> _coinRate) => new StageInteractGenerateData() { m_ActionRate = _actionRate, m_TradeRate = _tradeRate, m_CoinRate = _coinRate };
    }

    public struct CampFarmItemData
    {
        public int m_ItemDuration { get; private set; }
        public float m_CreditPerSecond { get; private set; }
        public static CampFarmItemData Create(int _duration, float _creditPersecond) => new CampFarmItemData { m_ItemDuration = _duration, m_CreditPerSecond = _creditPersecond };
    }
    #endregion

    #region SaveData
    public class CGameSave : ISave
    {
        public float f_Credits;
        public float f_TechPoints;
        public int m_GameDifficulty;
        public int m_DifficultyUnlocked;
        public List<ActionStorageData> m_StorageActions;
        public int m_StorageRequestStamp;
        public CGameSave()
        {
            f_Credits = 0;
            f_TechPoints = 0;
            m_GameDifficulty = 1;
            m_DifficultyUnlocked = 1;
            m_StorageRequestStamp = -1;
            m_StorageActions = new List<ActionStorageData>();
            for (int i = 0; i < GameExpression.I_CampActionStorageDefault.Count; i++)
                m_StorageActions.Add(ActionStorageData.CreateDefault(GameExpression.I_CampActionStorageDefault[i]));
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
    }

    public class CBattleSave : ISave
    {
        public enum_PlayerWeapon m_weapon;
        public int m_coins;
        public int m_kills;
        public ActionGameData m_weaponAction;
        public List<ActionGameData> m_battleAction;
        public List<ActionStorageData> m_startAction;
        public string m_GameSeed;
        public enum_StageLevel m_StageLevel;
        public CBattleSave()
        {
            m_coins = 0;
            m_weapon = enum_PlayerWeapon.P92;
            m_weaponAction = new ActionGameData();
            m_battleAction = new List<ActionGameData>();
            m_StageLevel = enum_StageLevel.Rookie;
            m_GameSeed = DateTime.Now.ToLongTimeString().ToString();
            m_startAction = ActionDataManager.CreateRandomPlayerUnlockedAction(GameDataManager.m_GameData.m_StorageActions, 9);
        }
        public void Adjust(EntityCharacterPlayer _player, GameLevelManager _level)
        {
            m_coins = _player.m_PlayerInfo.m_Coins;
            m_weapon = _player.m_WeaponCurrent.m_WeaponInfo.m_Weapon;
            m_weaponAction = ActionGameData.Create(_player.m_WeaponCurrent.m_WeaponAction);
            m_battleAction = ActionGameData.Create(_player.m_PlayerInfo.m_BattleAction);
            m_startAction = _level.m_startAction;
            m_GameSeed = _level.m_Seed;
            m_StageLevel = _level.m_GameStage;
            m_kills = _level.m_enermiesKilled;
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
    }

    public struct ActionGameData : IXmlPhrase
    {
        public bool m_IsNull => m_Index <= 0;
        public int m_Index { get; private set; }
        public enum_RarityLevel m_Level { get; private set; }

        public string ToXMLData() => m_Index.ToString() + "," + m_Level.ToString();
        public ActionGameData(string xmlData)
        {
            string[] split = xmlData.Split(',');
            m_Index = int.Parse(split[0]);
            m_Level = (enum_RarityLevel)Enum.Parse(typeof(enum_RarityLevel), split[1]);
        }

        public static ActionGameData Create(int index, enum_RarityLevel level) => new ActionGameData { m_Index = index, m_Level = level };
        public static ActionGameData Create(ActionBase action) => action == null ? new ActionGameData() { m_Index = -1, m_Level = 0 } : new ActionGameData { m_Index = action.m_Index, m_Level = action.m_rarity };
        public static List<ActionGameData> Create(List<ActionBase> actions)
        {
            List<ActionGameData> infos = new List<ActionGameData>();
            actions.Traversal((ActionBase action) => { infos.Add(Create(action)); });
            return infos;
        }
    }

    public struct ActionStorageData : IXmlPhrase
    {
        public int m_Index { get; private set; }
        public int m_Count { get; private set; }
        public string ToXMLData() => m_Index.ToString() + "," + m_Count.ToString();
        public ActionStorageData(string xmlData)
        {
            string[] split = xmlData.Split(',');
            m_Index = int.Parse(split[0]);
            m_Count = int.Parse(split[1]);
        }
        
        public int OnRequestCount(int count)
        {
            m_Count += count;
            int surplus = 0;
            if (m_Count >  GameConst.I_CampActionStorageEpicCount)
            {
                surplus = m_Count - GameConst.I_CampActionStorageEpicCount;
                m_Count -= surplus;
            }
            return surplus;
        }
        public enum_RarityLevel GetRarityLevel()
        {
            if (m_Count >= GameConst.I_CampActionStorageEpicCount)
                return enum_RarityLevel.Epic;
            else if (m_Count >= GameConst.I_CampActionStorageOutstandingCount)
                return enum_RarityLevel.OutStanding;
            else if (m_Count >= GameConst.I_CampActionStorageNormalCount)
                return enum_RarityLevel.Normal;
            return enum_RarityLevel.Invalid;
        } 
        
        public static ActionStorageData CreateDefault(int index) => new ActionStorageData { m_Index = index, m_Count = GameConst.I_CampActionStorageNormalCount };
        public static ActionStorageData CreateNewData(int index) => new ActionStorageData { m_Index = index, m_Count = 0 };
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
        float f_fireRate;
        float f_specialRate;
        int i_clipAmount;
        float f_spread;
        float f_reloadTime;
        int i_PelletsPerShot;
        float f_weight;
        float f_recoilHorizontal;
        float f_recoilVertical;
        float f_RPM;
        float f_recoilScore;
        public int m_Index => index;
        public enum_PlayerWeapon m_Weapon => (enum_PlayerWeapon)index;
        public float m_FireRate => f_fireRate;
        public float m_SpecialRate => f_specialRate;
        public int m_ClipAmount => i_clipAmount;
        public float m_Spread => f_spread;
        public float m_ReloadTime => f_reloadTime;
        public int m_PelletsPerShot => i_PelletsPerShot;
        public float m_Weight => f_weight;
        public Vector2 m_RecoilPerShot => new Vector2(f_recoilHorizontal, f_recoilVertical);
        public float m_RPM => f_RPM;
        public float m_RecoilScore => f_recoilScore;
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
        public Dictionary<enum_CharacterType, RangeInt> m_EntityGenerate;
        public void InitOnValueSet()
        {
            string[] defineSplit = em_defines.Split('_');
            m_Difficulty = (enum_BattleDifficulty)(int.Parse(defineSplit[0]));
            m_waveCount = (int.Parse(defineSplit[1]));
            m_EntityGenerate = new Dictionary<enum_CharacterType, RangeInt>();
            m_EntityGenerate.Add(enum_CharacterType.Fighter, ir_fighter);
            m_EntityGenerate.Add(enum_CharacterType.Shooter_Rookie, ir_shooterRookie);
            m_EntityGenerate.Add(enum_CharacterType.Shooter_Veteran, ir_shooterVeteran);
            m_EntityGenerate.Add(enum_CharacterType.AOECaster, ir_aoeCaster);
            m_EntityGenerate.Add(enum_CharacterType.Elite, ir_elite);
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
        public static SBuff CreateActionHealthDrainBuff(int actionIndex, float healthDrainAmount, float duration)
        {
            SBuff buff = new SBuff();
            buff.index = actionIndex * 10;
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
        public static SBuff CreateActionHealthBuff(int actionIndex, float healthPerTick, float healthTick, float duration)
        {
            SBuff buff = new SBuff();
            buff.index = actionIndex * 10;
            buff.i_addType = (int)enum_ExpireRefreshType.RefreshIdentity;
            buff.i_damageType = (int)enum_DamageType.HealthOnly;
            buff.f_damageTickTime = healthTick;
            buff.f_damagePerTick = -healthPerTick;
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
        public float m_BaseMaxHealth { get; private set; }
        public virtual float m_MaxHealth => m_BaseMaxHealth;
        public virtual float F_TotalEHP => m_CurrentHealth;
        public bool B_HealthFull => m_CurrentHealth >= m_MaxHealth;
        public float F_HealthBaseScale => m_CurrentHealth / m_BaseMaxHealth;
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
        public void OnSetHealth(float baseMaxHealth, bool restoreHealth)
        {
            m_BaseMaxHealth = baseMaxHealth;
            if (restoreHealth)
                m_CurrentHealth = m_MaxHealth;
        }
        public void OnRevive(float reviveHealth)
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
        public float m_MaxHealthAdditive { get; private set; }
        public override float m_MaxHealth => base.m_MaxHealth * m_HealthMultiplier + m_MaxHealthAdditive;
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
            m_MaxHealthAdditive = 0;
        }
        public void OnActivate(float maxHealth, float defaultArmor, bool restoreHealth)
        {
            base.OnSetHealth(maxHealth, restoreHealth);
            m_DefaultArmor = defaultArmor;
            m_CurrentArmor = m_DefaultArmor;
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        public void OnRevive(float reviveHealth, float reviveArmor)
        {
            base.OnRevive(reviveHealth);
            m_CurrentArmor = reviveArmor;
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        public void SetHealthMultiplier(float healthMultiplier)
        {
            m_HealthMultiplier = healthMultiplier;
            OnSetHealth(m_BaseMaxHealth, true);
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        public void OnMaxHealthAdditive(float maxHealthAdditive)
        {
            if (m_MaxHealthAdditive == maxHealthAdditive)
                return;

            m_MaxHealthAdditive = maxHealthAdditive;
            if (m_CurrentHealth > m_MaxHealth)
                OnSetHealth(m_BaseMaxHealth, true);
            OnHealthChanged(enum_HealthChangeMessage.Default);
        }
        public void OnRestoreArmor()
        {
            m_CurrentArmor = m_DefaultArmor;
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
        Dictionary<int, SFXEffect> m_BuffEffects = new Dictionary<int, SFXEffect>();
        public float F_MaxHealthAdditive { get; private set; } = 0f;
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
        Action OnExpireChange;

        bool b_expireUpdated = false;
        public void EntityInfoChange() => b_expireUpdated = false;

        public CharacterInfoManager(EntityCharacterBase _attacher, Func<DamageInfo, bool> _OnReceiveDamage, Action _OnExpireChange)
        {
            m_Entity = _attacher;
            OnReceiveDamage = _OnReceiveDamage;
            OnExpireChange = _OnExpireChange;
            TCommon.TraversalEnum((enum_CharacterEffect effect) => { m_Effects.Add(effect, new EffectCounterBase(enum_ExpireRefreshType.AddUp)); });
        }

        public virtual void OnActivate()
        {
            Reset();
            m_DamageBuffOverride = null;
            TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        }
        public virtual void OnDeactivate()
        {
            TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        }
        protected virtual void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
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
            EntityInfoChange();
        }

        public virtual void Tick(float deltaTime) {
            m_Expires.Traversal((ExpireBase expire) => { expire.OnTick(deltaTime); });
            m_Effects.Traversal((enum_CharacterEffect type) => { m_Effects[type].Tick(deltaTime); });

            if (b_expireUpdated)
                return;
            UpdateExpireInfo();
            UpdateExpireEffect();
            OnExpireChange();
            b_expireUpdated = true;
        }

        protected virtual void AddExpire(ExpireBase expire)
        {
            m_Expires.Add(expire);
            EntityInfoChange();
        }
        void RefreshExpire(ExpireBase expire)
        {
            expire.ExpireRefresh();
            EntityInfoChange();
        }
        protected virtual void OnExpireElapsed(ExpireBase expire)
        {
            m_Expires.Remove(expire);
            EntityInfoChange();
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
        protected virtual void OnResetInfo()
        {
            F_MaxHealthAdditive = 0f;
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
            F_MaxHealthAdditive += expire.m_MaxHealthAdditive;
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
        void UpdateExpireInfo()
        {
            OnResetInfo();
            m_Expires.Traversal(OnSetExpireInfo);
            AfterInfoSet();
        }
        #endregion
        #region ExpireEffect
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
        #endregion
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

        public float m_ActionEnergy { get; private set; } = 0f;
        List<ActionBase> m_ActionEquiping = new List<ActionBase>();
        public List<ActionBase> m_BattleAction { get; private set; } = new List<ActionBase>();
        public List<ActionBase> m_BattleActionPooling { get; private set; } = new List<ActionBase>();
        public List<ActionBase> m_BattleActionPicking { get; private set; } = new List<ActionBase>();
        Action OnActionChange;
        protected bool b_actionChangeIndicated = true;
        protected void IndicateActionUI() => b_actionChangeIndicated = false;
        protected float f_shuffleCheck = -1;
        protected bool b_shuffling => f_shuffleCheck > 0;
        public float f_shuffleScale => f_shuffleCheck / GameConst.F_ActionShuffleCooldown;
        public int m_Coins { get; private set; } = 0;
        public PlayerInfoManager(EntityCharacterPlayer _attacher, Func<DamageInfo, bool> _OnReceiveDamage, Action _OnExpireChange, Action _OnActionChange) : base(_attacher, _OnReceiveDamage, _OnExpireChange)
        {
            m_Player = _attacher;
            OnActionChange = _OnActionChange;
        }
        public override void OnActivate()
        {
            base.OnActivate();
            m_prePos = m_Entity.transform.position;
            TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
            TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase>(enum_BC_GameStatus.OnCharacterHealthWillChange, OnCharacterHealthWillChange);
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
            TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase>(enum_BC_GameStatus.OnCharacterHealthWillChange, OnCharacterHealthWillChange);
        }
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            EntityInfoChange();
            if (!b_actionChangeIndicated)
            {
                b_actionChangeIndicated = true;
                OnActionChange?.Invoke();
            }

            OnPlayerMove(TCommon.GetXZDistance(m_prePos, m_Entity.transform.position));
            m_prePos = m_Entity.transform.position;

            if (b_shuffling)
            {
                f_shuffleCheck -= deltaTime;
                if (f_shuffleCheck <= 0)
                    OnShuffle();
            }
        }
        public void InitActionInfo(List<ActionBase> _actions)
        {
            for (int i = 0; i < _actions.Count; i++)
                AddStoredAction(_actions[i]);
            m_ActionEnergy = GameConst.F_RestoreActionEnergy;
        }

        public void OnBattleStart()
        {
            OnShuffle();
        }

        public void OnBattleFinish()
        {
            Reset();
            m_ActionEnergy = GameConst.F_RestoreActionEnergy;
            m_ActionEquiping.Traversal((ActionBase action) => { if (action.m_ActionType != enum_ActionType.WeaponPerk) action.ForceExpire(); });
            m_BattleActionPooling.Clear();
            ClearHoldingActions();
        }

        protected override void Reset()
        {
            base.Reset();
            f_shuffleCheck = -1;
        }
        #region Player Info
        public void TestUseAction(ActionBase targetAction)        //Test Will Be Remove Soon
        {
            m_ActionEquiping.Traversal((ActionBase action) => { action.OnUseActionElse(targetAction); });
            if (targetAction.m_ActionType!= enum_ActionType.WeaponPerk)
                GameObjectManager.PlayMuzzle(m_Player.m_EntityID, m_Player.transform.position, Vector3.up, GameExpression.GetActionMuzzleIndex(targetAction.m_ActionType));
            OnAddAction(targetAction);
        }
        protected void OnUseAcion(ActionBase targetAction)
        {
            m_ActionEquiping.Traversal((ActionBase action) => {action.OnUseActionElse(targetAction); });
            GameObjectManager.PlayMuzzle(m_Player.m_EntityID, m_Player.transform.position, Vector3.up, GameExpression.GetActionMuzzleIndex(targetAction.m_ActionType));
            OnAddAction(targetAction);
        }
        protected void OnAddAction(ActionBase targetAction)
        {
            targetAction.Activate(m_Player, OnExpireElapsed);
            m_ActionEquiping.Add(targetAction);
            targetAction.OnActivate();
            AddExpire(targetAction);    
        }
        protected override void OnExpireElapsed(ExpireBase expire)
        {
            base.OnExpireElapsed(expire);
            ActionBase action = expire as ActionBase;
            if (action != null)
                m_ActionEquiping.Remove(action);
        }
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
        public override DamageDeliverInfo GetDamageBuffInfo()
        {
            ResetEffect(enum_CharacterEffect.Cloak);
            float randomDamageMultiply = UnityEngine.Random.Range(-GameConst.F_PlayerDamageAdjustmentRange, GameConst.F_PlayerDamageAdjustmentRange);
            DamageDeliverInfo info = DamageDeliverInfo.DamageInfo(m_Entity.m_EntityID, F_DamageMultiply + randomDamageMultiply, F_DamageAdditive);
            m_ActionEquiping.Traversal((ActionBase action) => { action.OnFire(info.I_IdentiyID); });
            return info;
        }

        public void OnAttachWeapon(WeaponBase weapon)
        {
            if(weapon.m_WeaponAction!=null)
                OnAddAction(weapon.m_WeaponAction);
        } 
        public void OnDetachWeapon() => m_ActionEquiping.Traversal((ActionBase action) => { action.OnWeaponDetach(); });
        public void OnPlayerMove(float distance) => m_ActionEquiping.Traversal((ActionBase action) => { action.OnMove(distance); });
        public void OnReloadFinish() => m_ActionEquiping.Traversal((ActionBase action) => { action.OnReloadFinish(); });

        protected void OnEntityActivate(EntityBase targetEntity)
        {
            if (targetEntity.m_Controller != enum_EntityController.AI||targetEntity.m_Flag != m_Entity.m_Flag || targetEntity.m_EntityID == m_Entity.m_EntityID)
                return;

            EntityCharacterBase ally = (targetEntity as EntityCharacterBase);
            if (F_AllyHealthMultiplierAdditive > 0)
                ally.m_Health.SetHealthMultiplier(F_AllyHealthMultiplierAdditive);
            m_ActionEquiping.Traversal((ActionBase action) => { action.OnAllyActivate(targetEntity as EntityCharacterBase); });
        }
        protected void OnCharacterHealthWillChange(DamageInfo damageInfo, EntityCharacterBase damageEntity)
        {
            if (damageInfo.m_AmountApply <= 0)
                return;

            if (damageInfo.m_detail.I_SourceID == m_Entity.m_EntityID)
            {
                m_ActionEquiping.Traversal((ActionBase action) => { action.OnDealtDamageSetEffect(damageEntity, damageInfo); });
                m_ActionEquiping.Traversal((ActionBase action) => { action.OnDealtDamageSetDamage(damageEntity, damageInfo); });
            }
            else if (damageEntity.m_EntityID == m_Player.m_EntityID)
            {
                m_ActionEquiping.Traversal((ActionBase action) => { action.OnBeforeReceiveDamage(damageInfo); });
            }
        }

        protected override void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
        {
            base.OnCharacterHealthChange(damageInfo, damageEntity, amountApply);
            if (damageInfo.m_detail.I_SourceID <= 0)
                return;
            OnCharacterHealthChangeAddPlayerEnergy(damageInfo.m_detail.I_SourceID, damageEntity, amountApply);


            if (damageInfo.m_detail.I_SourceID == m_Player.m_EntityID)
            {
                if(amountApply>0)
                    m_ActionEquiping.Traversal((ActionBase action) => { action.OnAfterDealtDemage(damageEntity, damageInfo, amountApply); });
            }

            if (damageEntity.m_EntityID == m_Player.m_EntityID)
            {
                if (amountApply > 0)
                    m_ActionEquiping.Traversal((ActionBase action) => { action.OnAfterReceiveDamage(damageInfo, amountApply); });
                else
                    m_ActionEquiping.Traversal((ActionBase action) => { action.OnReceiveHealing(damageInfo, amountApply); });
            }
        }

        void OnCharacterHealthChangeAddPlayerEnergy(int sourceID,EntityCharacterBase entity,float amountApply)
        {
            if (amountApply<=0||entity.b_isSubEntity)
                return;

            if (sourceID == m_Player.m_EntityID || GameManager.Instance.GetEntity(sourceID).m_SpawnerEntityID == m_Player.m_EntityID)
                    AddActionEnergy(GameExpression.GetActionEnergyRevive(amountApply));
        }
        #endregion

        #region Action Interact
        public bool B_EnergyCostable(ActionBase action) => m_ActionEnergy >= action.I_Cost;
        public bool TryUseHoldingAction(int index)
        {
            ActionBase action = m_BattleActionPicking[index];
            if (b_shuffling||!B_EnergyCostable(action))
                return false;

            m_ActionEnergy -= action.I_Cost;
            OnUseAcion(action);
            m_BattleActionPicking.RemoveAt(index);
            RefillHoldingActions();
            IndicateActionUI();
            return true;
        }
        public bool TryShuffle()
        {
            if (b_shuffling||m_ActionEnergy < GameConst.F_ActionShuffleCost)
                return false;
            m_ActionEnergy -= GameConst.F_ActionShuffleCost;
            f_shuffleCheck = GameConst.F_ActionShuffleCooldown;
            ClearHoldingActions();
            return true;
        }
        void OnShuffle()
        {
            m_BattleActionPooling.Clear();
            for (int i = 0; i < m_BattleAction.Count; i++)
            {
                if(m_BattleAction[i].m_ActionType!= enum_ActionType.Equipment||m_ActionEquiping.Find(p=>p.m_Identity==m_BattleAction[i].m_Identity)==null)
                     m_BattleActionPooling.Add( m_BattleAction[i]);
            }
            ClearHoldingActions();
            RefillHoldingActions();
        }
        void RefillHoldingActions()
        {
            if (m_BattleActionPooling.Count <= 0 || m_BattleActionPicking.Count >= GameConst.I_ActionHoldCount)
                return;

            int index = m_BattleActionPooling.RandomIndex();
            m_BattleActionPicking.Add(ActionDataManager.CopyAction(m_BattleActionPooling[index]));
            m_BattleActionPooling.RemoveAt(index);
            RefillHoldingActions();
        }
        void ClearHoldingActions()
        {
            m_BattleActionPicking.Clear();
            IndicateActionUI();
        }
        public void UpgradeAllHoldingAction()
        {
            if (m_BattleActionPicking.Count == 0)
                return;

            m_BattleActionPicking.Traversal((ActionBase action) => { action.Upgrade(); });
            IndicateActionUI();
        }
        public void OverrideHoldingActionCost(int cost)
        {
            m_BattleActionPicking.Traversal((ActionBase action) => { action.OverrideCost(cost); });
            IndicateActionUI();
        }
        public void AddStoredAction(ActionBase action)
        {
            m_BattleAction.Add(action);
            IndicateActionUI();
        }
        public void RemoveStoredAction(int index)
        {
            m_BattleAction.RemoveAt(index);
            IndicateActionUI();
        }
        public void UpgradeStoredAction(int index)
        {
            m_BattleAction[index].Upgrade();
            IndicateActionUI();
        }
        public void AddActionEnergy(float amount)
        {
            m_ActionEnergy += amount;
            if (m_ActionEnergy > GameConst.F_MaxActionEnergy)
                m_ActionEnergy = GameConst.F_MaxActionEnergy;
        }

        #endregion

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
        public virtual float m_MaxHealthAdditive => 0;
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
        public enum_RarityLevel m_rarity { get; private set; } = enum_RarityLevel.Invalid;
        public int m_Identity { get; private set; } = -1;
        public virtual int I_BaseCost => -1;
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
        protected ActionBase(int _identity,enum_RarityLevel _level)
        {
            m_Identity = _identity;
            m_rarity = _level;
            m_CostOverride = -1;
        }
        public void Activate(EntityCharacterPlayer _actionEntity, Action<ExpireBase> OnExpired) { m_ActionEntity = _actionEntity; OnActivate(F_Duration, OnExpired); }
        public bool B_Upgradable => m_rarity < enum_RarityLevel.Epic;
        public void Upgrade()
        {
            if (m_rarity < enum_RarityLevel.Epic)
                m_rarity++;
        }

        public int m_CostOverride { get; private set; } = -1;
        public int I_Cost => m_CostOverride == -1 ? I_BaseCost : m_CostOverride;
        public void OverrideCost(int overrideCost)=> m_CostOverride = overrideCost;
        #region Interact
        public virtual void OnActivate() { }
        public virtual void OnUseActionElse(ActionBase targetAction) { }
        public virtual void OnBeforeReceiveDamage(DamageInfo info) { }
        public virtual void OnAfterReceiveDamage(DamageInfo info, float amount) { }
        public virtual void OnDealtDamageSetEffect(EntityCharacterBase receiver,DamageInfo info) { }
        public virtual void OnDealtDamageSetDamage(EntityCharacterBase receiver, DamageInfo info) { }
        public virtual void OnAfterDealtDemage(EntityCharacterBase receiver,DamageInfo info, float applyAmount) { }
        public virtual void OnReceiveHealing(DamageInfo info, float applyAmount)  {}
        public virtual void OnReloadFinish() { }
        public virtual void OnFire(int identity) { }
        public virtual void OnWeaponDetach() { }
        public virtual void OnMove(float distsance) { }
        public virtual void OnAllyActivate(EntityCharacterBase ally) { }
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
    public class SBigmapTileInfo : ITileAxis
    {
        public TileAxis m_TileAxis => m_Tile;
        protected TileAxis m_Tile { get; private set; }
        public enum_TileType m_LevelType { get; private set; } = enum_TileType.Invalid;
        public enum_TileLocking m_TileLocking { get; private set; } = enum_TileLocking.Invalid;
        public Dictionary<enum_TileDirection, TileAxis> m_Connections { get; protected set; } = new Dictionary<enum_TileDirection, TileAxis>();

        public SBigmapTileInfo(TileAxis _tileAxis, enum_TileType _tileType, enum_TileLocking _tileLocking)
        {
            m_Tile = _tileAxis;
            m_LevelType = _tileType;
            m_TileLocking = _tileLocking;
        }
        public void ResetTileType(enum_TileType _tileType)
        {
            m_LevelType = _tileType;
        }
        public void SetTileLocking(enum_TileLocking _lockType)
        {
               m_TileLocking = _lockType;
        }
    }

    public class SBigmapLevelInfo : SBigmapTileInfo
    {
        public LevelBase m_Level { get; private set; } = null;
        public SBigmapLevelInfo(SBigmapTileInfo tile) : base(tile.m_TileAxis, tile.m_LevelType,tile.m_TileLocking)
        {
            m_Connections = tile.m_Connections;
        }
        public void GenerateMap(LevelBase levelSpawned,SLevelGenerate innerData,SLevelGenerate outerData, Dictionary<enum_LevelItemType,List<LevelItemBase>> _levelItemPrefabs,System.Random seed)
        {
            m_Level = levelSpawned;
            m_Level.transform.localRotation = Quaternion.Euler(0, seed.Next(360), 0);
            m_Level.transform.localPosition = Vector3.zero;
            m_Level.transform.localScale = Vector3.one;
            m_Level.GenerateTileItems(innerData,outerData, _levelItemPrefabs, m_LevelType,seed, m_LevelType== enum_TileType.End);
        }
        public void SetLevelShow(bool show)
        {
            if(m_Level)
            m_Level.SetActivate(show);
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
    class LevelTileInteract : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Interact;

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
        public EquipmentBase(SFXBase weaponBase, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo)
        {
            I_Index = weaponBase.I_SFXIndex;
            m_Entity = _controller;
            GetDamageDeliverInfo = _GetBuffInfo;
        }
        protected virtual Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target) => _target.tf_Head.position;
        public void Play(bool _preAim, EntityCharacterBase _target) => Play(_target,GetTargetPosition(_preAim, _target));
        public virtual void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {

        }
        public virtual void Tick(float deltaTime)
        {

        }
        public virtual void OnPlayAnim(bool play)
        {
        }
        public virtual void OnDeactivate()
        {

        }

        public static EquipmentBase AcquireEquipment(int weaponIndex, EntityCharacterBase _entity, Func<DamageDeliverInfo> GetDamageBuffInfo)
        {
            SFXBase weaponInfo = GameObjectManager.GetEquipmentData<SFXBase>(weaponIndex);
            SFXProjectile projectile = weaponInfo as SFXProjectile;
            if (projectile)
            {
                switch (projectile.E_ProjectileType)
                {
                    default: Debug.LogError("Invalid Type:" + projectile.E_ProjectileType); break;
                    case enum_ProjectileFireType.Single: return new EquipmentBarrageRange(projectile, _entity, GetDamageBuffInfo); 
                    case enum_ProjectileFireType.MultipleFan: return new EquipmentBarrageMultipleFan(projectile, _entity, GetDamageBuffInfo); 
                    case enum_ProjectileFireType.MultipleLine: return new EquipmentBarrageMultipleLine(projectile, _entity, GetDamageBuffInfo); 
                }
            }

            SFXCast cast = weaponInfo as SFXCast;
            if (cast)
            {
                switch (cast.E_CastType)
                {
                    default: Debug.LogError("Invalid Type:" + cast.E_CastType); break;
                    case enum_CastControllType.CastFromOrigin: return new EquipmentCaster(cast, _entity, GetDamageBuffInfo);
                    case enum_CastControllType.CastSelfDetonate: return new EquipmentCasterSelfDetonateAnimLess(cast, _entity, GetDamageBuffInfo, _entity.tf_Model.Find("BlinkModel")); 
                    case enum_CastControllType.CastControlledForward: return new EquipmentCasterControlled(cast, _entity, GetDamageBuffInfo);
                    case enum_CastControllType.CastAtTarget: return new EquipmentCasterTarget(cast, _entity, GetDamageBuffInfo);
                }
            }

            SFXBuffApply buffApply = weaponInfo as SFXBuffApply;
            if (buffApply)
                return new BuffApply(buffApply, _entity, GetDamageBuffInfo);

            SFXSubEntitySpawner entitySpawner = weaponInfo as SFXSubEntitySpawner;
            if (entitySpawner)
                return new EquipmentEntitySpawner(entitySpawner, _entity, GetDamageBuffInfo);

            SFXShield shield = weaponInfo as SFXShield;
            if (shield)
                return new EquipmentShieldAttach(shield, _entity, GetDamageBuffInfo);

            return null;
        }
    }
    public class EquipmentCaster : EquipmentBase
    {
        protected int i_muzzleIndex { get; private set; }
        protected enum_CastTarget m_CastAt { get; private set; }
        protected bool m_castForward { get; private set; }
        public EquipmentCaster(SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(_castInfo, _controller, _GetBuffInfo)
        {
            i_muzzleIndex = _castInfo.I_MuzzleIndex;
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
        public EquipmentCasterTarget(SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(_castInfo, _controller, _GetBuffInfo)
        {
        }
        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target)
        {
            Transform castAt = GetCastAt(_target);
            return LevelManager.NavMeshPosition(castAt.position + TCommon.RandomXZSphere(m_Entity.F_AttackSpread)) + new Vector3(0, castAt.position.y, 0);
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            GameObjectManager.PlayMuzzle(m_Entity.m_EntityID,m_Entity.tf_Weapon.position, m_Entity.tf_Weapon.forward,i_muzzleIndex);
            GameObjectManager.SpawnEquipment<SFXCast>(I_Index, _calculatedPosition,m_castForward?m_Entity.tf_Weapon.forward:Vector3.up).Play(GetDamageDeliverInfo());
        }
    }
    public class EquipmentCasterSelfDetonateAnimLess : EquipmentCaster
    {
        ModelBlink m_Blink;
        float timeElapsed;
        bool b_activating;
        public EquipmentCasterSelfDetonateAnimLess(SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo, Transform _blinkModels) : base(_castInfo, _controller, _GetBuffInfo)
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
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            if (b_activating)
                return;

            timeElapsed = 0;
            b_activating = true;
        }
    }
    public class EquipmentCasterControlled : EquipmentCaster
    {
        SFXCast m_Cast;
        public EquipmentCasterControlled(SFXCast _castInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(_castInfo, _controller, _GetBuffInfo)
        {
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            OnPlayAnim(false);
            m_Cast = GameObjectManager.SpawnEquipment<SFXCast>(I_Index, m_Entity.tf_Weapon.position, m_Entity.tf_Weapon.forward);
            m_Cast.PlayControlled(m_Entity.m_EntityID, m_Entity.tf_Weapon, attacherHead, true, GetDamageDeliverInfo());
        }

        public override void OnPlayAnim(bool play)
        {
            if (!play && m_Cast)
            {
                m_Cast.PlayControlled(m_Entity.m_EntityID, m_Entity.tf_Weapon, attacherHead, false, GetDamageDeliverInfo());
                m_Cast = null;
            }
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            OnPlayAnim(false);
        }
    }
    public class EquipmentBarrageRange : EquipmentBase
    {
        protected float f_projectileSpeed { get; private set; }
        protected RangeInt m_CountExtension { get; private set; }
        protected float m_OffsetExtension { get; private set; }
        int i_muzzleIndex;
        AudioClip m_MuzzleClip;

        public EquipmentBarrageRange(SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(projectileInfo, _controller, _GetBuffInfo)
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
            float startDistance = TCommon.GetXZDistance(m_Entity.tf_Weapon.position, _target.tf_Head.position);
            Vector3 targetPosition = preAim ? _target.m_PrecalculatedTargetPos(startDistance / f_projectileSpeed) : _target.tf_Head.position;

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
        public EquipmentBarrageMultipleLine(SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(projectileInfo, _controller, _GetBuffInfo)
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
        public EquipmentBarrageMultipleFan(SFXProjectile projectileInfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(projectileInfo, _controller, _GetBuffInfo)
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
        public BuffApply(SFXBuffApply buffApplyinfo, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(buffApplyinfo, _controller, _GetBuffInfo)
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
        public EquipmentEntitySpawner(SFXSubEntitySpawner spawner, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(spawner, _controller, _GetBuffInfo)
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
        public EquipmentShieldAttach(SFXShield shield, EntityCharacterBase _controller, Func<DamageDeliverInfo> _GetBuffInfo) : base(shield, _controller, _GetBuffInfo)
        {
        }
        Action<SFXShield> OnSpawn;
        public void SetOnSpawn(Action<SFXShield> _OnSpawn)
        {
            OnSpawn = _OnSpawn;
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            SFXShield shield = GameObjectManager.SpawnEquipment<SFXShield>(I_Index, m_Entity.tf_Weapon.position, Vector3.up);
            OnSpawn?.Invoke(shield);
            shield.Attach(_target);
        }
    }
    #endregion

    #endregion

    #region For UI Usage
    public enum enum_UI_TileBattleStatus
    {
        Invalid=-1,
        Clear=1,
        Overwatch=2,
        Patrol=3,
        HardBattle=4,
    }

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
        public Transform transform { get; private set; }
        ObjectPoolSimple<int> m_Grid;
        public UIC_RarityLevel(Transform _transform)
        {
            transform = _transform;
            m_Grid = new ObjectPoolSimple<int>(transform.Find("GridItem").gameObject, transform);
            m_Grid.ClearPool();
        }
        public void SetLevel(enum_RarityLevel level)
        {
            m_Grid.ClearPool();
            for (int i = 0; i < (int)level; i++)
                m_Grid.AddItem(i);
        }
    }
    public class UIC_RarityLevel_BG
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
        ObjectPoolSimple<int> m_Grid;
        Dictionary<int, RarityLevel> m_Levels = new Dictionary<int, RarityLevel>();
        public UIC_RarityLevel_BG(Transform _transform)
        {
            transform = _transform;
            m_Grid = new ObjectPoolSimple<int>(transform.Find("GridItem").gameObject,transform);
            m_Grid.ClearPool();
            TCommon.TraversalEnum((enum_RarityLevel rarity) => { m_Levels.Add((int)rarity,new RarityLevel( m_Grid.AddItem((int)rarity))); });
        }
        public void SetLevel(enum_RarityLevel level)
        {
            m_Levels.Traversal((int index, RarityLevel rarity) => rarity.SetHighlight(index <= (int)level));
        }
    }
    public class UI_WeaponActionHUD
    {
        public Transform transform { get; private set; }
        UIT_TextExtend m_WeaponActionName;
        UIC_RarityLevel_BG m_WeaponActionRarity;
        public UI_WeaponActionHUD(Transform _transform)
        {
            transform = _transform;
            m_WeaponActionName = _transform.Find("ActionName").GetComponent<UIT_TextExtend>();
            m_WeaponActionRarity = new UIC_RarityLevel_BG(_transform.Find("ActionRarity"));
        }

        public void SetInfo(ActionBase action)
        {
            bool showWeaponAction = action != null;
            m_WeaponActionRarity.transform.SetActivate(showWeaponAction);
            m_WeaponActionName.autoLocalizeText = showWeaponAction ? action.GetNameLocalizeKey() : "UI_WeaponStatus_ActionInvalidName";
            if (showWeaponAction) m_WeaponActionRarity.SetLevel(action.m_rarity);
        }
    }
    public class UIC_ActionEnergy
    {
        public RectTransform rectTransform { get; private set; }
        Image img_Full, img_Fill;
        Text txt_amount;

        float m_value;
        public UIC_ActionEnergy(Transform _transform)
        {
            rectTransform = _transform.GetComponent<RectTransform>();
            txt_amount = rectTransform.Find("Amount").GetComponent<Text>();
            img_Full = rectTransform.Find("Full").GetComponent<Image>();
            img_Fill = rectTransform.Find("Fill").GetComponent<Image>();
        }
        public void SetValue(float value)
        {
            if (m_value == value)
                return;

            m_value =value;
            float detail = m_value % 1f;
            bool full = m_value == GameConst.F_MaxActionEnergy;
            img_Full.SetActivate(full);
            img_Fill.SetActivate(!full);
            txt_amount.text = ((int)m_value).ToString();
            if (!full) img_Fill.fillAmount = detail;
        }
    }

    public class UIC_Button
    {
        Button m_Button;
        Image m_Show;
        Transform m_Hide;
        public UIC_Button(Transform _transform, UnityEngine.Events.UnityAction OnButtonClick)
        {
            m_Button = _transform.GetComponent<Button>();
            m_Button.onClick.AddListener(OnButtonClick);
            m_Show = _transform.GetComponent<Image>();
            m_Hide = _transform.Find("Hide");
            SetInteractable(true);
        }
        public void SetInteractable(bool interactable)
        {
            m_Hide.SetActivate(!interactable);
            m_Show.color = TCommon.ColorAlpha(m_Show.color, interactable ? 1 : 0);
            m_Button.interactable = interactable;
        }
    }
    #endregion
    #endregion
}
