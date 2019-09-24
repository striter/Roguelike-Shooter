using UnityEngine;
using TExcel;
using System.Collections.Generic;
using System;
using TTiles;
using TPhysics;
using System.Linq;
using GameSetting_Action;
using UnityEngine.UI;
#pragma warning disable 0649
namespace GameSetting
{
    #region For Designers Use
    public static class GameConst
    {
        public const float F_Gravity = 9.8f;

        public const float F_EntityDeadFadeTime = 2f;

        public const float F_MaxActionAmount = 4f;
        public const float F_RestoreActionAmount = 2f;
        public const float F_ActionShuffleCost = 3f;
        public const float F_ActionShuffleCooldown = 10f;

        public const float F_FireMovementReduction = .6f;
        public const float F_MovementReductionDuration = .1f;
        public const int I_ProjectileMaxDistance = 100;
        public const int I_ProjectileBlinkWhenTimeLeftLessThan = 3;
        public const float F_AimAssistDistance = 100f;
        public const short I_BoltLastTimeAfterHit = 5;
        public const float F_ParticlesMaxStopTime = 4f;

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

        public const float F_CoinsAcceleration = 8f;
        public const int I_HealthPickupAmount = 25;
        public const int I_ArmorPickupAmount = 25;

        public const float F_LevelTileSize = 2f;        //Cube Size For Level Tiles
    }

    public static class GameExpression
    {
        public static Vector3 V3_TileAxisOffset(TileAxis axis) => new Vector3(axis.X * GameConst.F_LevelTileSize, 0, axis.Y * GameConst.F_LevelTileSize);
        public static float F_BigmapYaw(Vector3 direction) => TCommon.GetAngle(direction, Vector3.forward, Vector3.up);         //Used For Bigmap Direction
        public static bool B_ShowHitMark(enum_HitCheck check) => check != enum_HitCheck.Invalid;
        public static float F_SphereCastDamageReduction(float weaponDamage, float distance, float radius) => weaponDamage * (1 - (distance / radius));       //Rocket Blast Damage
        public static Vector3 V3_RangeSpreadDirection(Vector3 aimDirection, float spread, Vector3 up, Vector3 right) => (aimDirection * GameConst.I_ProjectileSpreadAtDistance + up * UnityEngine.Random.Range(-spread, spread) + right * UnityEngine.Random.Range(-spread, spread)).normalized;

        public static int GetAIEquipment(int entityIndex, int weaponIndex = 0, int subWeaponIndex = 0) => entityIndex * 100 + weaponIndex * 10 + subWeaponIndex;
        public static int GetEquipmentSubIndex(int weaponIndex) => weaponIndex + 1;

        public static float GetActionAmountRevive(float damageApply) => damageApply * .00125f;

        public static float GetAIIdleDuration() => UnityEngine.Random.Range(1f, 2f);

        public static float GetResultLevelScore(enum_StageLevel _stage, int _levelPassed) => 200 * ((int)_stage - 1) + 20 * (_levelPassed - 1);
        public static float GetResultKillScore(int _enermyKilled) => _enermyKilled * 1;
        public static float GetResultRewardCoins(float _totalScore) => _totalScore;

        public static RangeInt GetTradePrice(enum_Interaction interactType, enum_RarityLevel level)
        {
            switch (interactType)
            {
                default: Debug.LogError("No Coins Can Phrase Here!"); return new RangeInt(0, -1);
                case enum_Interaction.PickupHealth:
                    return new RangeInt(4, 2);
                case enum_Interaction.PickupAction:
                case enum_Interaction.Weapon:
                    switch (level)
                    {
                        default: Debug.LogError("Invalid Level!"); return new RangeInt(0, -1);
                        case enum_RarityLevel.L1: return new RangeInt(8, 4);
                        case enum_RarityLevel.L2: return new RangeInt(16, 8);
                        case enum_RarityLevel.L3: return new RangeInt(24, 12);
                    }
            }
        }
        public static int GetActionRemovePrice(enum_StageLevel stage, int removeTimes) => 8 + 2 * (removeTimes + 1) * (int)stage;
        public static int GetActionUpgradePrice(enum_StageLevel stage, int upgradeTimes) => 8 + 2 * (upgradeTimes + 1) * (int)stage;
        public static StageInteractGenerate GetInteractGenerate(enum_StageLevel level)
        {
            switch (level)
            {
                default: return StageInteractGenerate.Create(new Dictionary<enum_RarityLevel, int>(), new Dictionary<enum_RarityLevel, int>(), new Dictionary<enum_CharacterType, CoinsGenerateInfo>());
                case enum_StageLevel.Rookie:
                    return StageInteractGenerate.Create(
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.L1, 75 }, { enum_RarityLevel.L2, 25 } },    //宝箱等级概率
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.L1, 75 }, { enum_RarityLevel.L2, 25 } },    //交易等级概率
                    new Dictionary<enum_CharacterType, CoinsGenerateInfo>() {
                     { enum_CharacterType.SubHidden, CoinsGenerateInfo.Create( 0,0, 0, new RangeInt(0, 0)) },     //实体掉落生成概率 类型,血,护甲,金币,金币数值范围
                     { enum_CharacterType.Fighter, CoinsGenerateInfo.Create( 8,20, 10, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Rookie, CoinsGenerateInfo.Create( 8,15, 10, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Veteran, CoinsGenerateInfo.Create( 8,15, 20, new RangeInt(3, 3)) },
                     { enum_CharacterType.AOECaster, CoinsGenerateInfo.Create( 8,15, 40, new RangeInt(4, 4)) },
                     { enum_CharacterType.Elite, CoinsGenerateInfo.Create( 8,15, 100, new RangeInt(6, 6)) }});
                case enum_StageLevel.Veteran:
                    return StageInteractGenerate.Create(
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.L2, 75 }, { enum_RarityLevel.L3, 25 } },    //宝箱等级概率
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.L2, 75 }, { enum_RarityLevel.L3, 25 } },    //交易等级概率
                    new Dictionary<enum_CharacterType, CoinsGenerateInfo>() {
                     { enum_CharacterType.SubHidden, CoinsGenerateInfo.Create( 0,0, 0, new RangeInt(0, 0)) },     //实体掉落生成概率 类型,血,护甲,金币,金币数值范围
                     { enum_CharacterType.Fighter, CoinsGenerateInfo.Create( 8,15, 10, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Rookie, CoinsGenerateInfo.Create( 8,15, 10, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Veteran, CoinsGenerateInfo.Create( 8,15, 20, new RangeInt(3, 3)) },
                     { enum_CharacterType.AOECaster, CoinsGenerateInfo.Create( 8,15, 40, new RangeInt(4, 4)) },
                     { enum_CharacterType.Elite, CoinsGenerateInfo.Create( 8,15, 100, new RangeInt(6, 6)) }});
                case enum_StageLevel.Ranger:
                    return StageInteractGenerate.Create(
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.L2, 25 }, { enum_RarityLevel.L3, 75 } },    //宝箱等级概率
                    new Dictionary<enum_RarityLevel, int>() { { enum_RarityLevel.L2, 25 }, { enum_RarityLevel.L3, 75 } },    //交易等级概率
                    new Dictionary<enum_CharacterType, CoinsGenerateInfo>() {
                     { enum_CharacterType.SubHidden, CoinsGenerateInfo.Create( 0,0, 0, new RangeInt(0, 0)) },     //实体掉落生成概率 类型,血,护甲,金币,金币数值范围
                     { enum_CharacterType.Fighter, CoinsGenerateInfo.Create( 8,15, 10, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Rookie, CoinsGenerateInfo.Create( 8,15, 10, new RangeInt(2, 2)) },
                     { enum_CharacterType.Shooter_Veteran, CoinsGenerateInfo.Create( 8,15, 20, new RangeInt(3, 3)) },
                     { enum_CharacterType.AOECaster, CoinsGenerateInfo.Create( 8,15, 40, new RangeInt(4, 4)) },
                     { enum_CharacterType.Elite, CoinsGenerateInfo.Create( 8,15, 100, new RangeInt(6, 6)) }});
            }
        }
    }

    public static class UIConst
    {
        public const int I_AmmoCountToSlider = 30;      //Ammo UI,While Clip Above This Will Turn To Be Slider

        public const float F_UIMaxArmor = 100f;
        public const float F_MapAnimateTime = 1.6f;
    }

    public static class UIExpression
    {
        public static Color BigmapTileColor(enum_TileLocking levelLocking, enum_TileType levelType)
        {
            Color color;
            switch (levelType)
            {
                default: color = TCommon.ColorAlpha(Color.blue, .5f); break;
                case enum_TileType.Battle: color = TCommon.ColorAlpha(Color.red, .5f); break;
                case enum_TileType.Start: color = TCommon.ColorAlpha(Color.grey, .5f); break;
                case enum_TileType.End: color = TCommon.ColorAlpha(Color.black, .5f); break;
                case enum_TileType.CoinsTrade: color = TCommon.ColorAlpha(Color.green, .5f); break;
                case enum_TileType.BattleTrade: color = TCommon.ColorAlpha(Color.yellow, .5f); break;
                case enum_TileType.ActionAdjustment: color = TCommon.ColorAlpha(Color.white, .5f); break;
            }
            switch (levelLocking)
            {
                case enum_TileLocking.Unlockable: color = TCommon.ColorAlpha(color, .2f); break;
            }
            return color;
        }
        public static Color ActionRarityColor(this enum_RarityLevel level)
        {
            switch (level) {
                case enum_RarityLevel.L1:
                    return Color.green;
                case enum_RarityLevel.L2:
                    return Color.blue;
                case enum_RarityLevel.L3:
                    return Color.yellow;
                default:
                    return Color.magenta;
            }
        }

        public static float F_WeaponDamageValue(float baseDamage) => baseDamage / 150f;
        public static float F_WeaponFireRateValue(float baseFireRate) =>   0.05f/ baseFireRate;
        public static float F_WeaponStabilityValue(float baseStability) =>  baseStability/3f;
        public static float F_WeaponProjectileSpeedValue(float baseProjectileSpeed) => baseProjectileSpeed / 100f;
    }

    public static class Enum_Relative
    {
        public static enum_RarityLevel ToActionLevel(this enum_StageLevel stageLevel) => (enum_RarityLevel)stageLevel;

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

    #endregion

    #region For Developers Use

    #region BroadCastEnum
    enum enum_BC_GameStatus
    {
        Invalid = -1,

        OnEntityActivate,
        OnEntityDeactivate,
        OnEntityRecycle,

        OnCharacterDamage,
        OnCharacterDead,

        OnStageStart,       //Total Stage Start
        OnStageFinish,
        OnChangeLevel,       //Change Between Each Level
        OnBattleStart,      //Battle Against Entity
        OnBattleFinish,
        OnWaveStart,     //Battle Wave
        OnWaveFinish,
    }

    enum enum_BC_UIStatus
    {
        Invalid = -1,
        UI_PlayerCommonStatus,
        UI_PlayerHealthStatus,
        UI_PlayerAmmoStatus,
        UI_PlayerActionStatus,
        UI_PlayerExpireStatus,
        UI_PlayerWeaponStatus,
    }
    #endregion

    #region GameEnum
    public enum enum_StageLevel { Invalid = -1, Rookie = 1, Veteran = 2, Ranger = 3 }

    public enum enum_BattleDifficulty { Invalid = -1, Peaceful = 0, Eazy = 1, Normal = 2, Hard = 3, End = 4, BattleTrade = 10, }

    public enum enum_EntityController { Invalid = -1, None = 1, AI = 2, Player = 3, }

    public enum enum_EntityFlag { Invalid = -1, None = 0, Player = 1, Enermy = 2, Neutal = 3, }

    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, Interact = 4, }

    public enum enum_TileLocking { Invalid = -1, Locked = 0, Unlockable = 1, Unlocked = 2, }

    public enum enum_Style { Invalid = -1, Forest = 1, Desert = 2, Iceland = 3, Horde = 4, Undead = 5, }

    public enum enum_TileType { Invalid = -1, Start = 0, Battle = 1, End = 2, CoinsTrade = 11, ActionAdjustment = 12, BattleTrade = 13, }

    public enum enum_LevelItemType { Invalid = -1, LargeMore, LargeLess, MediumMore, MediumLess, SmallMore, SmallLess, ManmadeMore, ManmadeLess, NoCollisionMore, NoCollisionLess, BorderLinear, BorderOblique, }

    public enum enum_LevelTileType { Invaid = -1, Empty, Main, Border, Item, Interact, }

    public enum enum_LevelTileOccupy { Invalid = -1, Inner, Outer, Border, }

    public enum enum_LevelGenerateType { Invalid = -1, Big = 1, Small = 2 }

    public enum enum_CharacterType { Invalid = -1, Fighter = 1, Shooter_Rookie = 2, Shooter_Veteran = 3, AOECaster = 4, Elite = 5, SubHidden = 99 }

    public enum enum_Interaction { Invalid = -1, Portal = 1, ActionChest = 2, ContainerTrade = 10, ContainerBattle = 11, PickupCoin = 20, PickupHealth = 21, PickupArmor = 22, PickupAction = 23, Weapon = 24, ActionAdjustment = 25, }

    public enum enum_TriggerType { Invalid = -1, Single = 1, Auto = 2, Burst = 3, Pull = 4, Store = 5, }

    public enum enum_ProjectileFireType { Invalid = -1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, CastSelfDetonate = 4, }

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, ForwardTrapezium = 4, }

    public enum enum_HealthChangeMessage { Invalid = -1, Begin = 0, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Common = 1, ArmorOnly = 2, HealthOnly = 3, }

    public enum enum_CharacterEffect { Invalid = -1, Stun = 1, Cloak = 2, Scan = 3, }

    public enum enum_ExpireType { Invalid = -1, Buff = 1, Action = 2, }

    public enum enum_ExpireRefreshType { Invalid = -1, AddUp = 1, Refresh = 2 }

    public enum enum_RarityLevel { Invalid = -1, L1 = 1, L2 = 2, L3 = 3, }

    public enum enum_ActionExpireType { Invalid = -1, Common = 1, AfterUse = 2, AfterFire = 3, AfterBattle = 4, AfterWeaponSwitch = 5, }

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
        Heavy_HipShot_161 = 161,
        Heavy_Mortal_162 = 162,
        Heavy_Shield_Spear_163 = 163,
        Heavy_Remote_164 = 164,
    }

    public enum enum_Option_FrameRate { Invalid=-1,FPS30=30,FPS60=60,}
    #endregion

    #region GameLayer
    public static class GameLayer
    {
        public static readonly int I_Static = LayerMask.NameToLayer("static");
        public static readonly int I_Entity = LayerMask.NameToLayer("entity");
        public static readonly int I_Dynamic = LayerMask.NameToLayer("dynamic");
        public static readonly int I_Interact = LayerMask.NameToLayer("interact");
        public static readonly int I_InteractDetect = LayerMask.NameToLayer("interactDetect");
        public static readonly int I_MovementDetect = LayerMask.NameToLayer("movementDetect");
        public static class Mask
        {
            public static readonly int I_All = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity | 1 << GameLayer.I_Dynamic;
            public static readonly int I_StaticEntity = 1 << GameLayer.I_Static | 1 << GameLayer.I_Entity;
            public static readonly int I_Entity = (1 << GameLayer.I_Entity);
            public static readonly int I_Interact = (1 << GameLayer.I_Interact);
            public static readonly int I_Static = (1 << GameLayer.I_Static);
        }
    }
    #endregion

    #region GameSave
    public class CPlayerDataSave : ISave        //Save Outta Game
    {
        public float f_blue;
        public CPlayerDataSave()
        {
            f_blue = 100;
        }
        public void OnBlueUsed(float amount)
        {
            f_blue -= amount;
        }
    }

    public class CPlayerGameSave : ISave
    {
        public enum_PlayerWeapon m_weapon;
        public int m_coins;
        public int m_kills;
        public List<ActionInfo> m_weaponActions;
        public List<ActionInfo> m_storedActions;
        public string m_GameSeed;
        public enum_StageLevel m_StageLevel;
        public CPlayerGameSave()
        {
            m_coins = 0;
            m_weapon = enum_PlayerWeapon.P92;
            m_weaponActions = new List<ActionInfo>();
            m_storedActions = new List<ActionInfo>();
            m_GameSeed = DateTime.Now.ToLongTimeString().ToString();
            m_StageLevel = enum_StageLevel.Rookie;
        }
        public void Adjust(EntityCharacterPlayer _player,GameLevelManager _level,GameRecordManager _record)
        {
            m_coins = _player.m_PlayerInfo.m_Coins;
            m_weapon = _player.m_WeaponCurrent.m_WeaponInfo.m_Weapon;
            m_weaponActions = ActionInfo.Create(_player.m_WeaponCurrent.m_WeaponAction);
            m_storedActions = ActionInfo.Create(_player.m_PlayerInfo.m_ActionStored);
            m_GameSeed = _level.m_Seed;
            m_StageLevel = _level.m_currentStage;
            m_kills = _record.i_entitiesKilled;
        }
    }

    public class CGameOptions : ISave
    {
        public enum_Option_JoyStickMode m_JoyStickMode;
        public enum_Option_FrameRate m_FrameRate;
        public enum_Option_LanguageRegion m_Region;
        public float m_MusicVolume;
        public float m_VFXVolume;
        public float m_Sensitive;
        public bool m_AdditionalLight;

        public CGameOptions()
        {
            m_JoyStickMode = enum_Option_JoyStickMode.Retarget;
            m_FrameRate = enum_Option_FrameRate.FPS60;
            m_Region = enum_Option_LanguageRegion.CN;
            m_Sensitive = .4f;
            m_MusicVolume = .8f;
            m_VFXVolume = .8f;
            m_AdditionalLight = false;
        }
    }
    #endregion

    #region DataStruct
    public struct CoinsGenerateInfo
    {
        public int m_HealthRate { get; private set; }
        public int m_ArmorRate { get; private set; }
        public int m_CoinRate { get; private set; }
        public RangeInt m_CoinRange { get; private set; }
        public static CoinsGenerateInfo Create(int healthRate,int armorRate,int coinRate, RangeInt coinAmount) => new CoinsGenerateInfo() {m_HealthRate=healthRate,m_ArmorRate=armorRate,m_CoinRate = coinRate, m_CoinRange = coinAmount };
    }
    public struct StageInteractGenerate
    {
        Dictionary<enum_CharacterType, CoinsGenerateInfo> m_CoinRate;
        Dictionary<enum_RarityLevel, int> m_ActionRate;
        Dictionary<enum_RarityLevel, int> m_TradeRate;
        public bool CanGenerateHealth(enum_CharacterType entityType) => TCommon.RandomPercentage() <= m_CoinRate[entityType].m_HealthRate;
        public bool CanGenerateArmor(enum_CharacterType entityType) => TCommon.RandomPercentage() <= m_CoinRate[entityType].m_ArmorRate;
        public int GetCoinGenerate(enum_CharacterType entityType)=> TCommon.RandomPercentage() <= m_CoinRate[entityType].m_CoinRate? m_CoinRate[entityType].m_CoinRange.RandomRangeInt():-1;
        public enum_RarityLevel GetActionRarityLevel(System.Random seed) => TCommon.RandomPercentage(m_ActionRate, seed);
        public enum_RarityLevel GetTradeRarityLevel(System.Random seed) => TCommon.RandomPercentage(m_TradeRate, seed);
        public static StageInteractGenerate Create(Dictionary<enum_RarityLevel, int> _actionRate, Dictionary<enum_RarityLevel, int> _tradeRate, Dictionary<enum_CharacterType, CoinsGenerateInfo> _coinRate) => new StageInteractGenerate() { m_ActionRate = _actionRate,m_TradeRate=_tradeRate,m_CoinRate=_coinRate };
    }
    public struct ActionInfo : IXmlPhrase
    {
        public int m_Index { get; private set; }
        public enum_RarityLevel m_Level { get; private set; }
        public string ToXMLData() => m_Index.ToString() + "," + m_Level.ToString();
        public ActionInfo(string xmlData)
        {
            string[] split = xmlData.Split(',');
            m_Index = int.Parse(split[0]);
            m_Level = (enum_RarityLevel)Enum.Parse(typeof(enum_RarityLevel),split[1]);
        }
        public static ActionInfo Create(int index, enum_RarityLevel level) => new ActionInfo { m_Index = index, m_Level = level };
        public static ActionInfo Create(ActionBase action) => new ActionInfo { m_Index = action.m_Index, m_Level = action.m_rarity };
        public static List<ActionInfo> Create(List<ActionBase> actions)
        {
            List<ActionInfo> infos = new List<ActionInfo>();
            actions.Traversal((ActionBase action) => { infos.Add(Create(action)); });
            return infos;
        }
    }

    public struct SWeapon : ISExcel
    {
        int index;
        float f_fireRate;
        float f_specialRate;
        int i_clipAmount;
        float f_spread;
        float f_reloadTime;
        int i_PelletsPerShot;
        float f_stunAfterShot;
        float f_recoilHorizontal;
        float f_recoilVertical;
        float f_movementReduction;
        public int m_Index => index;
        public enum_PlayerWeapon m_Weapon => (enum_PlayerWeapon)index;
        public float m_FireRate => f_fireRate;
        public float m_SpecialRate => f_specialRate;
        public int m_ClipAmount => i_clipAmount;
        public float m_Spread => f_spread;
        public float m_ReloadTime => f_reloadTime;
        public int m_PelletsPerShot => i_PelletsPerShot;
        public float m_stunAfterShot => f_stunAfterShot;
        public Vector2 m_RecoilPerShot => new Vector2(f_recoilHorizontal, f_recoilVertical);
        public float m_movementReduction => f_movementReduction;
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
        public int m_waveCount;
        public enum_BattleDifficulty m_Difficulty;
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
        float f_damageMultiply;
        float f_damageReduce;
        float f_damageTickTime;
        float f_damagePerTick;
        int i_damageType;
        bool b_stun;
        public int m_Index => index;
        public enum_ExpireRefreshType m_AddType => (enum_ExpireRefreshType)i_addType;
        public float m_ExpireDuration => f_expireDuration;
        public int m_EffectIndex => i_effect;
        public float m_MovementSpeedMultiply => f_movementSpeedMultiply;
        public float m_FireRateMultiply => f_fireRateMultiply;
        public float m_ReloadRateMultiply => f_reloadRateMultiply;
        public float m_DamageMultiply => f_damageMultiply;
        public float m_DamageReduction => f_damageReduce;
        public float m_DamageTickTime => f_damageTickTime;
        public float m_DamagePerTick => f_damagePerTick;
        public enum_DamageType m_DamageType => (enum_DamageType)i_damageType;
        public bool m_Stun => b_stun;
        public void InitOnValueSet()
        {
            f_movementSpeedMultiply = f_movementSpeedMultiply > 0 ? f_movementSpeedMultiply / 100f : 0;
            f_fireRateMultiply = f_fireRateMultiply > 0 ? f_fireRateMultiply / 100f : 0;
            f_reloadRateMultiply = f_reloadRateMultiply > 0 ? f_reloadRateMultiply / 100f : 0;
            f_damageMultiply = f_damageMultiply > 0 ? f_damageMultiply / 100f : 0;
            f_damageReduce = f_damageReduce > 0 ? f_damageReduce / 100f : 0;
        }
    }

    #endregion

    #region GameClass/Structs
    #region GameBase
    public class HealthBase
    {
        public float m_CurrentHealth { get; private set; }
        public float m_BaseMaxHealth { get; private set; }
        public virtual float m_MaxHealth => m_BaseMaxHealth;
        public virtual float F_TotalEHP => m_CurrentHealth;
        public bool B_HealthFull => m_CurrentHealth >= m_MaxHealth;
        public float F_BaseHealthScale =>m_CurrentHealth / m_BaseMaxHealth;
        public virtual float F_EHPScale => Mathf.Clamp01( m_CurrentHealth / m_MaxHealth);
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
        public HealthBase( Action<enum_HealthChangeMessage> _OnHealthChanged, Action _OnDead)
        {
            OnHealthChanged = _OnHealthChanged;
            OnDead = _OnDead;
        }
        public void OnActivate(float baseMaxHealth,bool restoreHealth)
        {
            m_BaseMaxHealth = baseMaxHealth;
            if(restoreHealth)
            m_CurrentHealth = m_MaxHealth;
        }
        public virtual bool OnReceiveDamage(DamageInfo damageInfo, float damageReduction = 1)
        {
            if (b_IsDead||damageInfo.m_Type== enum_DamageType.ArmorOnly)
                return false;

            if (damageInfo.m_AmountApply < 0)
            {
                DamageHealth(damageInfo.m_AmountApply);
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
    public class AIHealth : EntityHealth
    {
        int m_HealthMultiplier = 0;
        public override float m_MaxHealth => m_BaseMaxHealth * m_HealthMultiplier;
        public AIHealth(EntityCharacterBase _character,Action<enum_HealthChangeMessage> _OnHealthChanged, Action _OnDead) : base(_character,_OnHealthChanged, _OnDead)
        {
        }
        public void SetDifficulty(enum_StageLevel _stage)
        {
            m_HealthMultiplier = (int)_stage;
            OnActivate(m_BaseMaxHealth,true);
        }
    }
    public class EntityHealth:HealthBase
    {
        public float m_CurrentArmor { get; private set; }
        public float m_DefaultArmor { get; private set; }

        public override float F_TotalEHP => m_CurrentArmor + base.F_TotalEHP;
        public override float F_EHPScale => Mathf.Clamp01((m_CurrentArmor + m_CurrentHealth) / (m_DefaultArmor + m_MaxHealth));
        protected EntityCharacterBase m_Entity;
        protected void DamageArmor(float amount)
        {
            m_CurrentArmor -= amount;
            if (m_CurrentArmor < 0)
                m_CurrentArmor = 0;
        }

        public EntityHealth(EntityCharacterBase entity, Action<enum_HealthChangeMessage> _OnHealthChanged, Action _OnDead) :base(_OnHealthChanged,_OnDead)
        {
            m_Entity = entity;
        }
        public void OnActivate(float maxHealth, float defaultArmor,bool restoreHealth)
        {
            base.OnActivate(maxHealth,restoreHealth);
            m_DefaultArmor= defaultArmor;
            m_CurrentArmor = m_DefaultArmor;
            OnHealthChanged(enum_HealthChangeMessage.Begin);
        }
        public override bool OnReceiveDamage(DamageInfo damageInfo,float damageMultiply=1)
        {
            if (b_IsDead)
                return false;

            if (damageInfo.m_AmountApply > 0)    //Damage
            {
                float damageReceive = damageInfo.m_AmountApply*damageMultiply;

                switch (damageInfo.m_Type)
                {
                    case enum_DamageType.ArmorOnly:
                        {
                            if (m_CurrentArmor <= 0)
                                return false;
                            DamageArmor(damageReceive);
                            OnHealthChanged(enum_HealthChangeMessage.DamageArmor);
                        }
                        break;
                    case enum_DamageType.HealthOnly:
                        {
                            DamageHealth(damageReceive);
                            OnHealthChanged(enum_HealthChangeMessage.DamageHealth);
                        }
                        break;
                    case enum_DamageType.Common:
                        {
                            float healthDamage = damageReceive-m_CurrentArmor ;
                            DamageArmor(damageReceive);
                            if (healthDamage > 0)
                                DamageHealth(healthDamage);
                            OnHealthChanged(healthDamage >= 0 ? enum_HealthChangeMessage.DamageHealth : enum_HealthChangeMessage.DamageArmor);
                        }
                        break;
                }

                TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCharacterDamage, damageInfo.m_detail, m_Entity, damageReceive);
                if (b_IsDead)
                    OnDead();
            }
            else if (damageInfo.m_AmountApply < 0)    //Healing
            {
                float amountHeal = damageInfo.m_AmountApply;
                switch (damageInfo.m_Type)
                {
                    case enum_DamageType.ArmorOnly:
                        {
                            DamageArmor(amountHeal);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveArmor);
                        }
                        break;
                    case enum_DamageType.HealthOnly:
                        {
                            if (B_HealthFull)
                                return false;
                            DamageHealth(amountHeal);
                            OnHealthChanged( enum_HealthChangeMessage.ReceiveHealth);
                        }
                        break;
                    case enum_DamageType.Common:
                        {
                            float armorReceive = amountHeal  - m_CurrentHealth+m_BaseMaxHealth;
                            DamageHealth(amountHeal);
                            if (armorReceive>0)
                            {
                                OnHealthChanged(enum_HealthChangeMessage.ReceiveHealth);
                                return true;
                            }
                            
                            DamageArmor(armorReceive);
                            OnHealthChanged(enum_HealthChangeMessage.ReceiveArmor);
                        }
                        break;
                }
            }
            
            return true;
        }
    }

    public class DamageInfo
    {
        public float m_AmountApply => m_baseDamage  *(1+  m_detail.m_DamageMultiply)+ m_detail.m_DamageAdditive;
        private float m_baseDamage;
        public enum_DamageType m_Type { get; private set; }
        public DamageDeliverInfo m_detail { get; private set; }
        public DamageInfo(float damage,enum_DamageType type,DamageDeliverInfo detail)
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
        public List<int> m_BaseBuffApply { get; private set; } = new List<int>();
        public enum_CharacterEffect m_DamageEffect = enum_CharacterEffect.Invalid;
        public float m_EffectDuration = 0;

        public static DamageDeliverInfo Default(int sourceID) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageMultiply = 0f, m_DamageAdditive = 0f };
        public static DamageDeliverInfo BuffInfo(int sourceID, int buffApply) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageMultiply = 0f, m_DamageAdditive = 0f, m_BaseBuffApply = new List<int>() { buffApply } };
        public static DamageDeliverInfo EquipmentInfo(int sourceID, float _damageAdditive, enum_CharacterEffect _effect, float _duration) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageAdditive = _damageAdditive, m_DamageEffect = _effect, m_EffectDuration = _duration };
        public static DamageDeliverInfo DamageInfo(int sourceID, float _damageEnhanceMultiply, float _damageAdditive) => new DamageDeliverInfo() { I_IdentiyID = GameIdentificationManager.I_DamageIdentityID(), I_SourceID = sourceID, m_DamageMultiply = _damageEnhanceMultiply, m_DamageAdditive = _damageAdditive, };
        public static DamageDeliverInfo DamageOverrideInfo(int sourceID, DamageDeliverInfo info) => new DamageDeliverInfo() {I_SourceID = sourceID, I_IdentiyID =info.I_IdentiyID,m_DamageMultiply=info.m_DamageMultiply,m_DamageAdditive=info.m_DamageAdditive,m_BaseBuffApply=info.m_BaseBuffApply,m_DamageEffect=info.m_DamageEffect,m_EffectDuration=info.m_EffectDuration};
    }
    #endregion

    #region Entity Info Manager
    public class CharacterInfoManager
    {
        protected EntityCharacterBase m_Entity { get; private set; }
        public List<ExpireBase> m_Expires { get; private set; } = new List<ExpireBase>();
        List< SFXBuffEffect> m_BuffEffects = new List<SFXBuffEffect>();
        public float F_DamageReceiveMultiply { get; private set; } = 1f;
        public float F_MovementSpeedMultiply { get; private set; } = 1f;
        protected float F_FireRateMultiply { get; private set; } = 1f;
        protected float F_ReloadRateMultiply { get; private set; } = 1f;
        protected float F_DamageMultiply { get; private set; } = 0f;
        public float F_FireRateTick(float deltaTime) => deltaTime * F_FireRateMultiply;
        public float F_ReloadRateTick(float deltaTime) => deltaTime * F_ReloadRateMultiply;
        public float F_MovementSpeed => m_Entity.m_baseMovementSpeed * F_MovementSpeedMultiply;

        protected Dictionary<enum_CharacterEffect, EffectCounterBase> m_Effects = new Dictionary<enum_CharacterEffect, EffectCounterBase>();
        public bool B_Effecting(enum_CharacterEffect type) => m_Effects[type].m_Effecting;
        protected void ResetEffect(enum_CharacterEffect type) => m_Effects[type].Reset();
        public void OnSetEffect(enum_CharacterEffect type, float duration) => m_Effects[type].OnSet(duration);

        public virtual DamageDeliverInfo GetDamageBuffInfo() => DamageDeliverInfo.DamageInfo(m_Entity.I_EntityID ,F_DamageMultiply,0f);
        Func<DamageInfo, bool> OnReceiveDamage;
        Action OnExpireChange;

        bool b_expireUpdated = false;
        public void EntityInfoChange() => b_expireUpdated = false;

        public CharacterInfoManager(EntityCharacterBase _attacher,Func<DamageInfo, bool> _OnReceiveDamage,Action _OnExpireChange)
        {
            m_Entity = _attacher;
            OnReceiveDamage = _OnReceiveDamage;
            OnExpireChange = _OnExpireChange;
            TCommon.TraversalEnum((enum_CharacterEffect effect) => { m_Effects.Add(effect, new EffectCounterBase()); });
        }

        public virtual void OnActivate()=> Reset();
        public virtual void OnDeactivate()=> Reset();
        protected virtual void Reset()
        {
            m_Effects.Traversal((enum_CharacterEffect type) => { m_Effects[type].Reset(); });
            m_Expires.Clear();
            m_BuffEffects.Clear();
            EntityInfoChange();
            OnExpireChange();
        }

        public virtual void Tick(float deltaTime) {
            m_Expires.Traversal((ExpireBase buff) => { buff.OnTick(deltaTime); });
            m_Effects.Traversal((enum_CharacterEffect type) => { m_Effects[type].Tick(deltaTime); });
            if (!b_expireUpdated) UpdateExpireInfo();
        }

        protected virtual void AddExpire(ExpireBase expire)
        {
            m_Expires.Add(expire);
            EntityInfoChange();
            OnExpireChange();
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
            OnExpireChange();
        }
        public void AddBuff(int sourceID,int buffIndex)
        {
            BuffBase buff = new BuffBase(sourceID, GameDataManager.GetEntityBuffProperties(buffIndex), OnReceiveDamage, OnExpireElapsed);
            OnSetEffect(enum_CharacterEffect.Stun,buff.m_StunDuration);
            switch (buff.m_RefreshType)
            {
                case enum_ExpireRefreshType.AddUp:
                        AddExpire(buff);
                    break;
                case enum_ExpireRefreshType.Refresh:
                    {
                        ExpireBase buffRefresh = m_Expires.Find(p =>p.m_Index == buffIndex);
                        if (buffRefresh != null)
                            RefreshExpire(buffRefresh);
                        else
                            AddExpire(buff);
                    }
                    break;
            }
        }
        protected virtual void OnSetExpireInfo(ExpireBase expire)
        {
            F_DamageMultiply += expire.m_DamageMultiply;
            F_DamageReceiveMultiply -= expire.m_DamageReduction;
            F_MovementSpeedMultiply += expire.m_MovementSpeedMultiply;
            F_FireRateMultiply += expire.m_FireRateMultiply;
            F_ReloadRateMultiply += expire.m_ReloadRateMultiply;

            if (F_DamageReceiveMultiply < 0)
                F_DamageReceiveMultiply = 0;
        }
        protected virtual void OnResetInfo()
        {
            F_DamageReceiveMultiply = 1f;
            F_MovementSpeedMultiply = 1f;
            F_FireRateMultiply = 1f;
            F_ReloadRateMultiply = 1f;
            F_DamageMultiply = 0f;
        }
        void UpdateExpireInfo()
        {
            b_expireUpdated = true;
            OnResetInfo();
            m_Expires.Traversal(OnSetExpireInfo);

            //Do Effect Removal Check
            for (int i = 0; i < m_BuffEffects.Count; i++)
            {
                ExpireBase expire = m_Expires.Find(p => p.m_EffectIndex == m_BuffEffects[i].I_SFXIndex);
                if (expire == null)
                {
                    m_BuffEffects[i].StopParticles();
                    m_BuffEffects.RemoveAt(i);
                }
            }

            //Refresh Or Add Effects
            for (int i = 0; i < m_Expires.Count; i++)
            {
                if (m_Expires[i].m_EffectIndex <= 0)
                    return;

                SFXBuffEffect particle = m_BuffEffects.Find(p => p.I_SFXIndex == m_Expires[i].m_EffectIndex);

                if (particle)
                    particle.Refresh(m_Expires[i].m_EffectDuration);
                else
                {
                    particle = GameObjectManager.SpawnBuffEffect(m_Expires[i].m_EffectIndex, m_Entity);
                    particle.Play(m_Entity.I_EntityID, m_Expires[i].m_EffectDuration);
                    m_BuffEffects.Add(particle);
                }
            }
        }

    }

    public class EntitySubInfoManager : CharacterInfoManager
    {
        Func<DamageDeliverInfo> m_DamageBuffOverride;
        public void AddDamageOverride(Func<DamageDeliverInfo> _damageOverride)=> m_DamageBuffOverride = _damageOverride;
        public override DamageDeliverInfo GetDamageBuffInfo() => m_DamageBuffOverride != null ? DamageDeliverInfo.DamageOverrideInfo(m_Entity.I_EntityID, m_DamageBuffOverride()) : base.GetDamageBuffInfo();
        public override void OnActivate()
        {
            base.OnActivate();
            m_DamageBuffOverride = null;
        }
        public EntitySubInfoManager(EntityCharacterBase _attacher, Func<DamageInfo, bool> _OnReceiveDamage, Action _OnExpireChange) : base(_attacher, _OnReceiveDamage, _OnExpireChange)
        {

        }
    }

    public class PlayerInfoManager : CharacterInfoManager
    {
        EntityCharacterPlayer m_Player;
        public float F_RecoilMultiply { get; private set; } = 1f;
        public float F_ProjectileSpeedMuiltiply { get; private set; } = 1f;
        protected bool B_OneOverride { get; private set; } = false;
        protected int I_ClipAdditive { get; private set; } = 0;
        protected float F_ClipMultiply { get; private set; } = 1f;
        public int I_ClipAmount(int baseClipAmount) =>baseClipAmount==0?0:(int)(((B_OneOverride ? 1 : baseClipAmount) + I_ClipAdditive) * F_ClipMultiply);
        protected float F_DamageAdditive = 0f;

        public float m_ActionAmount { get; private set; } = 0f;
        List<ActionBase> m_ActionEquiping = new List<ActionBase>();
        protected List<ActionAfterFire> m_AfterFire = new List<ActionAfterFire>();
        public List<ActionBase> m_ActionStored { get; private set; } = new List<ActionBase>();
        public List<ActionBase> m_ActionInPool { get; private set; } = new List<ActionBase>();
        public List<ActionBase> m_ActionHolding { get; private set; } = new List<ActionBase>();
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
            TBroadCaster<enum_BC_GameStatus>.Add<DamageDeliverInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterDamage, OnCharacterDamage);
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            TBroadCaster<enum_BC_GameStatus>.Remove<DamageDeliverInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterDamage, OnCharacterDamage);
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
            m_ActionAmount = GameConst.F_RestoreActionAmount;
        }

        public void OnBattleStart()
        {
            OnShuffle();
        }
        public void OnBattleFinish()
        {
            m_ActionAmount = GameConst.F_RestoreActionAmount;
            m_ActionInPool.Clear();
            m_ActionEquiping.Traversal((ActionBase action) => { action.OnAfterBattle(); }, true);
            m_AfterFire.Clear();
            f_shuffleCheck = -1;
            ClearHoldingActions();
        }

        #region Player Info
        public void OnUseAcion(ActionBase targetAction)
        {
            m_ActionEquiping.Traversal((ActionBase action) => { action.OnAddActionElse(targetAction.m_Index); });
            OnSetEffect( enum_CharacterEffect.Cloak,targetAction.F_CloakDuration);
            AddExpire(targetAction);
            m_ActionEquiping.Add(targetAction);
            targetAction.Activate(m_Player, OnExpireElapsed);
            targetAction.OnActionUse();
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
            F_RecoilMultiply = 1f;
            F_ProjectileSpeedMuiltiply = 1f;
        }
        protected override void OnSetExpireInfo(ExpireBase expire)
        {
            base.OnSetExpireInfo(expire);
            ActionBase action = expire as ActionBase;
            if (action == null)
                return;

            F_DamageAdditive += action.F_DamageAdditive;
            F_RecoilMultiply -= action.F_RecoilReduction;
            F_ProjectileSpeedMuiltiply += action.F_ProjectileSpeedMultiply;
            F_ClipMultiply += action.F_ClipMultiply;
            B_OneOverride |= action.B_ClipOverride;
            I_ClipAdditive += action.I_ClipAdditive;

            if (F_RecoilMultiply < 0)
                F_RecoilMultiply = 0;
        }
        public override DamageDeliverInfo GetDamageBuffInfo()
        {
            float randomDamageMultiply = UnityEngine.Random.Range(-GameConst.F_PlayerDamageAdjustmentRange,GameConst.F_PlayerDamageAdjustmentRange);
            DamageDeliverInfo info = DamageDeliverInfo.DamageInfo(m_Entity.I_EntityID, F_DamageMultiply+ randomDamageMultiply, F_DamageAdditive);
            base.ResetEffect(enum_CharacterEffect.Cloak);
            m_ActionEquiping.Traversal((ActionBase action) => {
                action.OnFire(info.I_IdentiyID);

                if (action.m_ActionExpireType == enum_ActionExpireType.AfterFire)
                    m_AfterFire.Add(action as ActionAfterFire);
            }, true);
            return info;
        }
        public void OnAttachWeapon(WeaponBase weapon) => weapon.m_WeaponAction.Traversal((ActionBase action) => { OnUseAcion(action); });
        public void OnDetachWeapon() => m_ActionEquiping.Traversal((ActionBase action) => { action.OnWeaponDetach(); }, true);
        public void OnReloadFinish() => m_ActionEquiping.Traversal((ActionBase action) => { action.OnReloadFinish(); });
        void OnCharacterDamage(DamageDeliverInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
        {
            m_AfterFire.Traversal((ActionAfterFire action) => { if (action.OnActionHitEntity(damageInfo.I_IdentiyID, damageEntity)) m_AfterFire.Remove(action); }, true);

            if (damageInfo.I_SourceID == m_Player.I_EntityID)
            {
                m_ActionEquiping.Traversal((ActionBase action) => { action.OnDealtDemage(damageEntity, amountApply); });
                AddActionAmount(GameExpression.GetActionAmountRevive(amountApply));
            }
            else if (damageEntity.I_EntityID == m_Player.I_EntityID)
            {
                m_ActionEquiping.Traversal((ActionBase action) => { action.OnReceiveDamage(damageInfo.I_SourceID, amountApply); });
            }
        }
        #endregion

        #region Action Interact

        public bool TryUseAction(int index)
        {
            ActionBase action = m_ActionHolding[index];
            if (b_shuffling||m_ActionAmount < action.I_ActionCost)
                return false;

            m_ActionAmount -= action.I_ActionCost;
            OnUseAcion(action);
            m_ActionHolding.RemoveAt(index);
            RefillHoldingActions();
            IndicateActionUI();
            return true;
        }
        public bool TryShuffle()
        {
            if (b_shuffling||m_ActionAmount < GameConst.F_ActionShuffleCost)
                return false;
            m_ActionAmount -= GameConst.F_ActionShuffleCost;
            f_shuffleCheck = GameConst.F_ActionShuffleCooldown;
            ClearHoldingActions();
            return true;
        }
        void OnShuffle()
        {
            m_ActionInPool.Clear();
            for (int i = 0; i < m_ActionStored.Count; i++)
            {
                if(m_ActionStored[i].m_ActionExpireType!= enum_ActionExpireType.AfterBattle||m_ActionEquiping.Find(p=>p.m_Identity==m_ActionStored[i].m_Identity)==null)
                     m_ActionInPool.Add( m_ActionStored[i]);
            }
            ClearHoldingActions();
            RefillHoldingActions();
        }
        void RefillHoldingActions()
        {
            if (m_ActionInPool.Count <= 0 || m_ActionHolding.Count >= 3)
                return;

            int index = m_ActionInPool.RandomIndex();
            m_ActionHolding.Add(GameDataManager.CopyAction(m_ActionInPool[index]));
            m_ActionInPool.RemoveAt(index);
            RefillHoldingActions();
        }
        void ClearHoldingActions()
        {
            m_ActionHolding.Clear();
            IndicateActionUI();
        }
        public void UpgradeRandomHoldingAction()
        {
            if (m_ActionHolding.Count == 0)
                return;

            m_ActionHolding.RandomItem().Upgrade();
            IndicateActionUI();
        }
        public void AddStoredAction(ActionBase action)
        {
            m_ActionStored.Add(action);
            IndicateActionUI();
        }
        public void RemoveStoredAction(int index)
        {
            m_ActionStored.RemoveAt(index);
            IndicateActionUI();
        }
        public void UpgradeStoredAction(int index)
        {
            m_ActionStored[index].Upgrade();
            IndicateActionUI();
        }
        public void AddActionAmount(float amount)
        {
            m_ActionAmount += amount;
            if (m_ActionAmount > GameConst.F_MaxActionAmount)
                m_ActionAmount = GameConst.F_MaxActionAmount;
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
        public float m_duration { get; private set; }
        public bool m_Effecting => m_duration > 0;
        public EffectCounterBase()
        {
            Reset();
        }
        public void OnSet(float _duration)
        {
            if (_duration <= 0)
                return;

            if (m_duration < _duration)
                m_duration = _duration;
        }
        public void Reset()
        {
            m_duration = 0;
        }
        public void Tick(float deltaTime)
        {
            if (m_duration > 0)
                m_duration -= deltaTime;
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
        public virtual float m_DamageMultiply => 0;
        public virtual float m_DamageReduction => 0;
        public virtual float m_EffectDuration => m_ExpireDuration;
        private Action<ExpireBase> OnExpired;
        public float m_ExpireDuration { get; private set; } = 0;
        public float f_expireCheck { get; private set; }
        protected void Activate(float _ExpireDuration,Action<ExpireBase> _OnExpired)
        {
            m_ExpireDuration = _ExpireDuration;
            ExpireRefresh();
            OnExpired = _OnExpired;
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
        protected void ForceExpire() => OnExpired(this);
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
        public float m_StunDuration => m_buffInfo.m_Stun ? m_buffInfo.m_ExpireDuration : 0f;
        SBuff m_buffInfo;
        Func<DamageInfo, bool> OnDOTDamage;
        int I_SourceID;
        float f_dotCheck;
        public BuffBase(int sourceID, SBuff _buffInfo, Func<DamageInfo, bool> _OnDOTDamage, Action<ExpireBase> _OnExpired)
        {
            I_SourceID = sourceID;
            m_buffInfo = _buffInfo;
            OnDOTDamage = _OnDOTDamage;
            base.Activate(_buffInfo.m_ExpireDuration, _OnExpired);
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
        public virtual int I_ActionCost => -1;
        public virtual bool B_ActionAble => true;
        public virtual enum_ActionExpireType m_ActionExpireType => enum_ActionExpireType.Common;
        public virtual float Value1 => 0;
        public virtual float Value2 => 0;
        public virtual float Value3 => 0;
        public virtual float F_DamageAdditive => 0;
        public virtual float F_RecoilReduction => 0;
        public virtual float F_ProjectileSpeedMultiply => 0;
        public virtual bool B_ClipOverride => false;
        public virtual int I_ClipAdditive => 0;
        public virtual float F_ClipMultiply => 0;
        public virtual float F_Duration => 0;
        protected ActionBase(int _identity,enum_RarityLevel _level)
        {
            m_Identity = _identity;
            m_rarity = _level;
        }
        public virtual float F_CloakDuration => 0;
        public void Activate(EntityCharacterPlayer _actionEntity,Action<ExpireBase> OnExpired) { m_ActionEntity = _actionEntity;Activate(F_Duration,OnExpired); }
        public virtual void OnActionUse() { }
        public virtual void OnAddActionElse(float actionAmount) { }
        public virtual void OnReceiveDamage(int applier, float amount) { }
        public virtual void OnDealtDemage(EntityCharacterBase receiver, float amount) { }
        public virtual void OnReloadFinish() { }
        public virtual void OnAfterBattle() { }
        public virtual void OnFire(int identity) { }
        public virtual void OnWeaponDetach() { }
        public bool B_Upgradable => m_rarity < enum_RarityLevel.L3;
        public void Upgrade()
        {
            if (m_rarity < enum_RarityLevel.L3)
                m_rarity++;
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
        Color c_blinkColor;
        public ModelBlink(Transform BlinkModel, float _blinkRate, float _blinkTime,Color _blinkColor)
        {
            if (BlinkModel == null)
                Debug.LogError("Error! Blink Model Init, BlinkModel Folder Required!");

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
        public void OnReset()
        {
            f_simulate = 0f;
            this.StopSingleCoroutine(0);
            m_materials.Traversal((Material material) => { material.SetColor("_Color", TCommon.ColorAlpha(c_blinkColor, 0)); }); 
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
        protected int i_weaponIndex;
        protected EntityCharacterBase m_Entity;
        protected Transform attacherTransform => m_Entity.tf_Head;
        protected Transform transformBarrel;
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
        public EquipmentBase(SFXBase weaponBase, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo)
        {
            i_weaponIndex = weaponBase.I_SFXIndex;
            m_Entity = _controller;
            transformBarrel = _transform;
            if (_transform == null)
                Debug.LogError("Null Weapon Barrel Found!");
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

        public static EquipmentBase AcquireEquipment(int weaponIndex, EntityCharacterBase _entity, Transform tf_Barrel, Func<DamageDeliverInfo> GetDamageBuffInfo)
        {
            SFXBase weaponInfo = GameObjectManager.GetEquipmentData<SFXBase>(weaponIndex);
            SFXProjectile projectile = weaponInfo as SFXProjectile;
            if (projectile)
            {
                switch (projectile.E_ProjectileType)
                {
                    default: Debug.LogError("Invalid Type:" + projectile.E_ProjectileType); break;
                    case enum_ProjectileFireType.Single: return new EquipmentBarrageRange(projectile, _entity, tf_Barrel, GetDamageBuffInfo); 
                    case enum_ProjectileFireType.MultipleFan: return new EquipmentBarrageMultipleFan(projectile, _entity, tf_Barrel, GetDamageBuffInfo); 
                    case enum_ProjectileFireType.MultipleLine: return new EquipmentBarrageMultipleLine(projectile, _entity, tf_Barrel, GetDamageBuffInfo); 
                }
            }

            SFXCast cast = weaponInfo as SFXCast;
            if (cast)
            {
                switch (cast.E_CastType)
                {
                    default: Debug.LogError("Invalid Type:" + cast.E_CastType); break;
                    case enum_CastControllType.CastFromOrigin: return new EquipmentCaster(cast, _entity, tf_Barrel, GetDamageBuffInfo);
                    case enum_CastControllType.CastSelfDetonate: return new EnermyCasterSelfDetonateAnimLess(cast, _entity, tf_Barrel, GetDamageBuffInfo, _entity.tf_Model.Find("BlinkModel")); 
                    case enum_CastControllType.CastControlledForward: return new EquipmentCasterControlled(cast, _entity, tf_Barrel, GetDamageBuffInfo);
                    case enum_CastControllType.CastAtTarget: return new EquipmentCasterTarget(cast, _entity, tf_Barrel, GetDamageBuffInfo);
                }
            }

            SFXBuffApply buffApply = weaponInfo as SFXBuffApply;
            if (buffApply)
                return new BuffApply(buffApply, _entity, tf_Barrel, GetDamageBuffInfo);

            SFXSubEntitySpawner entitySpawner = weaponInfo as SFXSubEntitySpawner;
            if (entitySpawner)
                return new EquipmentEntitySpawner(entitySpawner, _entity, tf_Barrel, GetDamageBuffInfo);

            SFXShield shield = weaponInfo as SFXShield;
            if (shield)
                return new EquipmentShieldAttach(shield, _entity, tf_Barrel, GetDamageBuffInfo);

            return null;
        }
    }
    public class EquipmentCaster : EquipmentBase
    {
        public EquipmentCaster(SFXCast _castInfo, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(_castInfo, _controller, _transform, _GetBuffInfo)
        {
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            GameObjectManager.SpawnEquipment<SFXCast>(i_weaponIndex, attacherTransform.position, attacherTransform.forward).Play(GetDamageDeliverInfo());
        }
    }
    public class EnermyCasterSelfDetonateAnimLess : EquipmentCaster
    {
        ModelBlink m_Blink;
        float timeElapsed;
        bool b_activating;
        public EnermyCasterSelfDetonateAnimLess(SFXCast _castInfo, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo, Transform _blinkModels) : base(_castInfo, _controller, _transform, _GetBuffInfo)
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
                GameObjectManager.SpawnEquipment<SFXCast>(i_weaponIndex, attacherTransform.position, attacherTransform.forward).Play(GetDamageDeliverInfo());
                m_Entity.m_HitCheck.TryHit(new DamageInfo(m_Entity.m_Health.F_TotalEHP, enum_DamageType.Common,DamageDeliverInfo.Default(-1)));
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
        public EquipmentCasterControlled(SFXCast _castInfo, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(_castInfo, _controller, _transform, _GetBuffInfo)
        {
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            OnPlayAnim(false);
            m_Cast = GameObjectManager.SpawnEquipment<SFXCast>(i_weaponIndex, transformBarrel.position, transformBarrel.forward);
            m_Cast.PlayControlled(m_Entity.I_EntityID, transformBarrel, attacherTransform, true, GetDamageDeliverInfo());
        }

        public override void OnPlayAnim(bool play)
        {
            if (m_Cast)
                m_Cast.PlayControlled(m_Entity.I_EntityID, transformBarrel, attacherTransform, play, GetDamageDeliverInfo());
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            OnPlayAnim(false);
        }
    }

    public class EquipmentCasterTarget : EquipmentCaster
    {
        public EquipmentCasterTarget(SFXCast _castInfo, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(_castInfo, _controller, _transform, _GetBuffInfo)
        {
        }
        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target) => LevelManager.NavMeshPosition(_target.transform.position + TCommon.RandomXZSphere(m_Entity.F_AttackSpread));
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            GameObjectManager.SpawnEquipment<SFXCast>(i_weaponIndex,_calculatedPosition,Vector3.up).Play( GetDamageDeliverInfo());
        }
    }
    public class EquipmentBarrageRange : EquipmentBase
    {
        protected float f_projectileSpeed { get; private set; }
        protected int i_muzzleIndex { get; private set; }
        protected RangeInt m_CountExtension { get; private set; }
        protected float m_OffsetExtension { get; private set; }

        public EquipmentBarrageRange(SFXProjectile projectileInfo, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(projectileInfo, _controller, _transform, _GetBuffInfo)
        {
            i_muzzleIndex = projectileInfo.I_MuzzleIndex;
            f_projectileSpeed = projectileInfo.F_Speed;
            m_CountExtension = projectileInfo.RI_CountExtension;
            m_OffsetExtension = projectileInfo.F_OffsetExtension;
        }


        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Vector3 startPosition = transformBarrel.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            FireBullet(startPosition, direction, _calculatedPosition);
        }
        protected override Vector3 GetTargetPosition(bool preAim, EntityCharacterBase _target)
        {
            float startDistance = TCommon.GetXZDistance(transformBarrel.position, _target.tf_Head.position);
            Vector3 targetPosition = preAim ? _target.m_PrecalculatedTargetPos(startDistance / f_projectileSpeed) : _target.tf_Head.position;

            if (preAim && Mathf.Abs(TCommon.GetAngle(transformBarrel.forward, TCommon.GetXZLookDirection(transformBarrel.position, targetPosition), Vector3.up)) > 90)    //Target Positioned Back, Return Target
                targetPosition = _target.tf_Head.position;

            if (TCommon.GetXZDistance(transformBarrel.position, targetPosition) > m_Entity.F_AttackSpread)      //Target Outside Spread Sphere,Add Spread
                targetPosition += TCommon.RandomXZSphere(m_Entity.F_AttackSpread);
            return targetPosition;
        }

        protected void FireBullet(Vector3 startPosition, Vector3 direction, Vector3 targetPosition)
        {
            if (i_muzzleIndex > 0)
                GameObjectManager.SpawnParticles<SFXMuzzle>(i_muzzleIndex, startPosition, direction).Play(m_Entity.I_EntityID);
            GameObjectManager.SpawnEquipment<SFXProjectile>(i_weaponIndex, startPosition, direction).Play(GetDamageDeliverInfo(),direction, targetPosition);
        }
    }
    public class EquipmentBarrageMultipleLine : EquipmentBarrageRange
    {
        public EquipmentBarrageMultipleLine(SFXProjectile projectileInfo, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(projectileInfo, _controller, _transform, _GetBuffInfo)
        {
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Vector3 startPosition = transformBarrel.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            int waveCount = m_CountExtension.RandomRangeInt();
            float distance = TCommon.GetXZDistance(startPosition, _calculatedPosition);
            Vector3 lineBeginPosition = startPosition - attacherTransform.right * m_OffsetExtension * ((waveCount - 1) / 2f);
            for (int i = 0; i < waveCount; i++)
                FireBullet(lineBeginPosition + attacherTransform.right * m_OffsetExtension * i, direction, transformBarrel.position + direction * distance);
        }
    }
    public class EquipmentBarrageMultipleFan : EquipmentBarrageRange
    {
        public EquipmentBarrageMultipleFan(SFXProjectile projectileInfo, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(projectileInfo, _controller, _transform, _GetBuffInfo)
        {
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            Vector3 startPosition = transformBarrel.position;
            Vector3 direction = TCommon.GetXZLookDirection(startPosition, _calculatedPosition);
            int waveCount = m_CountExtension.RandomRangeInt();
            float beginAnle = -m_OffsetExtension * (waveCount - 1) / 2f;
            float distance = TCommon.GetXZDistance(transformBarrel.position, _calculatedPosition);
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 fanDirection = direction.RotateDirection(Vector3.up, beginAnle + i * m_OffsetExtension);
                FireBullet(transformBarrel.position, fanDirection, transformBarrel.position + fanDirection * distance);
            }
        }
    }
    public class BuffApply : EquipmentBase
    {
        public override bool B_TargetAlly => true;
        SBuff m_buffInfo;
        SFXBuffApply m_Effect;
        public BuffApply(SFXBuffApply buffApplyinfo, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(buffApplyinfo, _controller, _transform, _GetBuffInfo)
        {
            m_buffInfo = GameDataManager.GetEntityBuffProperties(buffApplyinfo.I_BuffIndex);
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            if (!m_Effect || !m_Effect.b_Playing)
                m_Effect = GameObjectManager.SpawnEquipment<SFXBuffApply>(i_weaponIndex, transformBarrel.position, Vector3.up);

            m_Effect.Play(m_Entity.I_EntityID, m_buffInfo, transformBarrel, _target);
        }
    }

    public class EquipmentEntitySpawner : EquipmentBase
    {
        public EquipmentEntitySpawner(SFXSubEntitySpawner spawner, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(spawner, _controller, _transform, _GetBuffInfo)
        {
        }
        Action<EntityCharacterBase> OnSpawn;
        public void SetOnSpawn(Action<EntityCharacterBase> _OnSpawn)
        {
            OnSpawn = _OnSpawn;
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            GameObjectManager.SpawnEquipment<SFXSubEntitySpawner>(i_weaponIndex, transformBarrel.position, Vector3.up).Play(m_Entity.I_EntityID, m_Entity.m_Flag,GetDamageDeliverInfo,m_Entity.I_EntityID,OnSpawn);
        }
    }

    public class EquipmentShieldAttach : EquipmentBase
    {
        public override bool B_TargetAlly => true;
        public EquipmentShieldAttach(SFXShield shield, EntityCharacterBase _controller, Transform _transform, Func<DamageDeliverInfo> _GetBuffInfo) : base(shield, _controller, _transform, _GetBuffInfo)
        {
        }
        Action<SFXShield> OnSpawn;
        public void SetOnSpawn(Action<SFXShield> _OnSpawn)
        {
            OnSpawn = _OnSpawn;
        }
        public override void Play(EntityCharacterBase _target, Vector3 _calculatedPosition)
        {
            SFXShield shield = GameObjectManager.SpawnEquipment<SFXShield>(i_weaponIndex, transformBarrel.position, Vector3.up);
            OnSpawn?.Invoke(shield);
            shield.Attach(_target);
        }
    }
    #endregion

    #endregion

    #region For UI Usage
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
        UIT_GridController m_Grid;
        public UIC_RarityLevel(Transform _transform)
        {
            transform = _transform;
            m_Grid = new UIT_GridController(transform);
        }
        public void SetLevel(enum_RarityLevel level)
        {
            m_Grid.ClearGrid();
            for (int i = 0; i < (int)level; i++)
                m_Grid.AddItem(i);
        }
    }
    public class UIC_ActionAmount
    {
        Transform transform;
        Image img_Full, img_Fill;
        Text txt_amount, txt_Max;

        float m_value;
        public UIC_ActionAmount(Transform _transform)
        {
            transform = _transform;
            txt_amount = transform.Find("Amount").GetComponent<Text>();
            txt_Max = transform.Find("Max").GetComponent<Text>();
            img_Full = transform.Find("Full").GetComponent<Image>();
            img_Fill = transform.Find("Fill").GetComponent<Image>();

            txt_Max.text = GameConst.F_MaxActionAmount.ToString();
        }
        public void SetValue(float value)
        {
            if (m_value == value)
                return;
            m_value = value;
            float detail =value%1f;
            bool full = value == GameConst.F_MaxActionAmount;
            img_Full.SetActivate(full);
            img_Fill.SetActivate(!full);
            txt_amount.text = ((int)value).ToString();
            if (!full) img_Fill.fillAmount = detail;
        }
    }
    public enum enum_UI_ActionUpgradeType
    {
        Invalid=-1,
        Upgradeable=1,
        LackOfCoins=2,
        MaxLevel=3,
    }

    public static class LocalizationKeyJoint
    {
        public static string GetNameLocalizeKey(this BuffBase buff) => "Buff_Name_" + buff.m_Index;
        public static string GetNameLocalizeKey(this ActionBase action) => "Action_Name_" + action.m_Index;
        public static string GetIntroLocalizeKey(this ActionBase action) => "Action_Intro_" + action.m_Index;
        public static string GetLocalizeKey(this enum_RarityLevel level) => "Action_Level_" + level;
        public static string GetNameLocalizeKey(this enum_PlayerWeapon weapon) => "Weapon_Name_" + weapon;
        public static string GetInteractTitle(this enum_Interaction interact) => "Interact_" + interact;
        public static string GetLocalizeKey(this enum_Option_FrameRate frameRate) => "UI_Option_" + frameRate;
        public static string GetLocalizeKey(this enum_Option_JoyStickMode joystick) => "UI_Option_" + joystick;
        public static string GetLocalizeKey(this enum_Option_LanguageRegion region) => "UI_Option_" + region;
    }
    #endregion

    #endregion
}
