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
        public const int I_NormalProjectileLastTime = 5; // No Collision Recycle Time
        public const int I_BoltMaxLastTime = 10;    //Last Time Of Ammo/Bolt
        public const int I_LaserMaxLastTime = 5;    //Longest Last Time Of Ammo/Laser
        public const int I_BarrageProjectileMaxLastTime = 5;

        public const int I_RocketBlastRadius = 5;        //Meter
        public const float F_LaserRayStartPause = .5f;      //Laser Start Pause

        public const int I_BurstFirePelletsOnceTrigger = 3;       //Times While Burst Fire
        public const int I_ProjectileSpreadAtDistance = 100;       //Meter,  Bullet Spread In A Circle At End Of This Distance

        public const float F_LevelTileSize = 2f;        //Cube Size For Level Tiles

        public const float F_DamagePlayerFallInOcean = 10f;        //Player Ocean Fall Damage

        public const float F_PlayerCameraSmoothParam = 1f;     //Camera Smooth Param For Player .2 is suggested
        public const float F_PlayerFallSpeed = 9.8f;       //Player Fall Speed(Not Acceleration)

        public const int I_TileMapPortalMinusOffset = 3;        //The Minimum Tile Offset Away From Origin Portal Will Generate

        public const float F_EnermyAICheckTime = .3f;       //AI Check Offset Time, 0.3 is suggested;
    }

    public static class GameExpression
    {
        public static float F_BigmapYaw(Vector3 direction) => TCommon.GetAngle(direction, Vector3.forward, Vector3.up);         //Used For Bigmap Direction
        public static enum_TileDirection E_BigmapDirection(Vector3 direction)  //Top 135-225    Right 45 - 135  Bottom 135 - -135 Right -135 - -45
        {
            float angle = F_BigmapYaw(direction);       //0-360
            if (angle <= 45 && angle > -45)
                return enum_TileDirection.Top;
            if (angle <= 135 && angle > 45)
                return enum_TileDirection.Left;
            if (angle <= -135 || angle > 135)
                return enum_TileDirection.Bottom;
            if (angle <= -45 && angle > -135)
                return enum_TileDirection.Right;
            Debug.LogError("GameSetting.WorldOffsetDirection Error? Invalid angle of:"+angle);
            return enum_TileDirection.Invalid;
        }

        public static float F_RocketBlastDamage(float weaponDamage, float distance) => weaponDamage * (1-(distance / GameConst.I_RocketBlastRadius));       //Rocket Blast Damage
        public static Vector3 V3_FireDirectionSpread(Vector3 aimDirection, float spread,Vector3 up,Vector3 right) => (aimDirection*GameConst.I_ProjectileSpreadAtDistance + up* UnityEngine.Random.Range(-spread, spread) + right * UnityEngine.Random.Range(-spread, spread)).normalized;
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
    }

    public static class UIExpression
    {
        public static float F_SporeManagerProfitPerMinute(int level) => 10 *Mathf.Pow(1.3f,level-1);     //Coins Profit Per Unit Time
        public static float F_SporeManagerCoinsMaxAmount(int level) => 100 * Mathf.Pow(10 * Mathf.Pow(1.3f , level - 1) , 1.03f) + 400 * Mathf.Pow(1.4f , level - 1) ;
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
            switch(levelLocking)
            {
                case enum_TileLocking.Unlockable:color=TCommon.ColorAlpha( color ,.2f); break;
            }
            return color;
        }
    }

    public static class Enum_Relative
    {
        public static enum_TilePrefabDefinition ToPrefabType(this enum_TileType type)
        {
            switch (type)
            {
                default:Debug.LogError("Please Edit This Please:" + type.ToString());return enum_TilePrefabDefinition.Invalid;
                case enum_TileType.Battle:
                case enum_TileType.End:
                    return enum_TilePrefabDefinition.Big;
                case enum_TileType.Reward:
                case enum_TileType.Start:
                    return enum_TilePrefabDefinition.Small;
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
    enum enum_BC_UIStatusChanged
    {
        Invalid = -1,
        PlayerInfoChanged,
        PlayerLevelStatusChanged,
    }
    enum enum_BC_GameStatusChanged
    {
        Invalid=-1,
        OnSpawnEntity,
        OnRecycleEntity,
        
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
    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, }
    public enum enum_BattleDifficulty { Invalid = -1, Default = 0, Eazy = 1, Normal = 2, Hard = 3 }
    public enum enum_TilePrefabDefinition { Invalid = -1, Big = 1, Small = 2, }
    public enum enum_TileLocking { Invalid = -1, Locked = 0, Unlockable = 1, Unlocked = 2, }
    public enum enum_TileStyle { Invalid = -1, Forest = 1, Desert = 2, Iceland = 3, Horde = 4, Undead = 5, }
    public enum enum_TileType { Invalid = -1, Start =0, Battle = 1, End = 2, Reward = 3,}
    public enum enum_LevelItemType{ Invalid=-1, LargeMore, LargeLess, MediumMore, MediumLess, SmallMore, SmallLess, ManmadeMore, ManmadeLess, NoCollisionMore, NoCollisionLess,}
    public enum enum_LevelTileType { Invaid = -1, Empty , Main, Item, Portal, }
    public enum enum_EntityLevel { Invalid = -1, Default=0 ,Militia=1 , Veteran=2, Ranger=3 }
    public enum enum_EntityStyle { Invalid=-1, Test=1,}

    public enum enum_Interact
    {
        Invalid=-1,
        Interact_Portal,
    }

    public enum enum_Weapon
    {
        Invalid = -1,
        //Laser
        LaserPistol = 1001,
        LaserCannon = 1002,
        //Snipe Rifle
        MK10 = 2001,
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
    public enum enum_TriggerType
    {
        Invalid = -1,
        Single = 1,
        Auto = 2,
        Burst = 3,
        Pull = 4,
        Store = 5,
    }
    public enum enum_BarrageType
    {
        Invalid=-1,
        Single=1,
        Multiple=2,
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
        public static class Physics
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
    #region GameClass
    class HitCheckDetect
    {
        Action<HitCheckStatic> OnHitCheckStatic;
        Action<HitCheckDynamic> OnHitCheckDynamic;
        Action<HitCheckEntity> OnHitCheckEntity;
        Action OnHitCheckError;
        public HitCheckDetect(Action<HitCheckStatic> _OnHitCheckStatic, Action<HitCheckDynamic> _OnHitCheckDynamic, Action<HitCheckEntity> _OnHitCheckEntity, Action _OnHitCheckError)
        {
            OnHitCheckStatic = _OnHitCheckStatic;
            OnHitCheckDynamic = _OnHitCheckDynamic;
            OnHitCheckEntity = _OnHitCheckEntity;
            OnHitCheckError = _OnHitCheckError;
        }
        public void DoDetect(Collider other)
        {
            if (other.gameObject.layer == GameLayer.I_DynamicDetect)
                return;

            HitCheckBase hitCheck = other.GetComponent<HitCheckBase>();
            if (hitCheck == null)
            {
                Debug.LogWarning("Null Hit Check Attached:" + other.gameObject);
                OnHitCheckError?.Invoke();
                return;
            }
            switch (hitCheck.m_HitCheckType)
            {
                default: Debug.LogError("Add More Convertions Here:" + hitCheck.m_HitCheckType); break;
                case enum_HitCheck.Static:
                    OnHitCheckStatic?.Invoke(hitCheck as HitCheckStatic);
                    break;
                case enum_HitCheck.Dynamic:
                    OnHitCheckDynamic?.Invoke(hitCheck as HitCheckDynamic);
                    break;
                case enum_HitCheck.Entity:
                    OnHitCheckEntity?.Invoke(hitCheck as HitCheckEntity);
                    break;
            }
        }
    }
    public class ProjectilePhysicsSimulator : PhysicsSimulator
    {
        public Vector3 m_HorizontalDirection { get; private set; }
        public Vector3 m_VerticalDirection { get; private set; }
        float m_horizontalSpeed;
        float m_horizontalDistance;
        float m_verticalSpeed;
        float m_verticalAcceleration;
        public ProjectilePhysicsSimulator(Vector3 _startPos, Vector3 _horizontalDirection, Vector3 _verticalDirection, float _horizontalSpeed, float _horizontalDistance, float _verticalSpeed, float _verticalAcceleration)
        {
            m_simulateTime = 0f;
            m_startPos = _startPos;
            m_LastPos = _startPos;
            m_HorizontalDirection = _horizontalDirection;
            m_VerticalDirection = _verticalDirection;
            m_horizontalSpeed = _horizontalSpeed;
            m_horizontalDistance = _horizontalDistance;
            m_verticalSpeed = _verticalSpeed;
            m_verticalAcceleration = _verticalAcceleration;

        }
        public Vector3 Simulate(float deltaTime,out Vector3 prePosition)
        {
            m_simulateTime += deltaTime;
            Vector3 currentPos= GetSimulatedPosition(m_startPos,m_HorizontalDirection,m_VerticalDirection,m_simulateTime,m_horizontalSpeed,m_horizontalDistance,m_verticalSpeed,m_verticalAcceleration);
            prePosition = m_LastPos;
            m_LastPos = currentPos;
            return currentPos;
        }
        public static Vector3 GetSimulatedPosition(Vector3 startPos, Vector3 horizontalDirection, Vector3 verticalDirection, float elapsedTime, float horizontalSpeed, float horizontalDistance, float verticalSpeed, float verticalAcceleration)
        {
            float f_verticalTime = elapsedTime- horizontalDistance / horizontalSpeed;
            f_verticalTime = f_verticalTime > 0 ? f_verticalTime : 0;
            Vector3 horizontalShift = horizontalDirection * Expressions.SpeedShift(horizontalSpeed, elapsedTime);
            Vector3 verticalShift = verticalDirection * Expressions.AccelerationSpeedShift(verticalSpeed, verticalAcceleration, f_verticalTime);
            Vector3 targetPos = startPos + horizontalShift + verticalShift;
            return targetPos;
        }
    }
    #region BigmapTile
    public class SBigmapTileInfo : ITileAxis
    {
        public TileAxis m_TileAxis => m_Tile;
        protected TileAxis m_Tile { get; private set; }
        public enum_TileType m_TileType { get; private set; } = enum_TileType.Invalid;
        public enum_TileStyle m_TileStyle { get; private set; } = enum_TileStyle.Invalid;
        public enum_TileLocking m_TileLocking { get; private set; } = enum_TileLocking.Invalid;
        public Dictionary<enum_TileDirection, TileAxis> m_Connections { get; protected set; } = new Dictionary<enum_TileDirection, TileAxis>();

        public SBigmapTileInfo(TileAxis _tileAxis, enum_TileType _tileType, enum_TileStyle _tileStyle, enum_TileLocking _tileLocking)
        {
            m_Tile = _tileAxis;
            m_TileType = _tileType;
            m_TileStyle = _tileStyle;
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
        protected Transform m_LevelParent;
        public LevelBase m_Level { get; private set; } = null;
        public SBigmapLevelInfo(SBigmapTileInfo tile) : base(tile.m_TileAxis, tile.m_TileType, tile.m_TileStyle,tile.m_TileLocking)
        {
            m_Connections = tile.m_Connections;
        }
        public Dictionary<LevelItemBase, int> GenerateMap(Transform _levelParent, LevelBase _levelPrefab, LevelItemBase[] _levelItemPrefabs,System.Random seed)
        {
            m_LevelParent = _levelParent;
            m_Level = GameObject.Instantiate(_levelPrefab, _levelParent);
            m_Level.transform.localRotation = Quaternion.Euler(0, seed.Next(360), 0);
            m_Level.transform.localPosition = Vector3.zero;
            m_Level.transform.localScale = Vector3.one;
            m_Level.SetActivate(false);
            return m_Level.Init(TResources.GetLevelData(_levelPrefab.name), ExcelManager.GetItemGenerateProperties(m_TileStyle, _levelPrefab.E_PrefabType), _levelItemPrefabs, m_TileType, seed, m_Connections.Keys.ToList().Find(p => m_Connections[p] == new TileAxis(-1, -1)));        //Add Portal For Level End
        }
        public void StartLevel()
        {
            m_Level.ShowAllItems();
            m_Level.SetActivate(true);
        }
    }
    #endregion
    #region LevelTile
    public class LevelTile : TileMapData.TileInfo
    {
        public virtual enum_LevelTileType E_TileType => enum_LevelTileType.Empty;
        public enum_TileDirection E_Direction { get; private set; } = enum_TileDirection.Invalid;
        public LevelTile(TileMapData.TileInfo current, enum_TileDirection _direction) : base(current.m_TileAxis, current.m_Offset, current.m_Status)
        {
            E_Direction = _direction;
        }
        public LevelTile(LevelTile tile) : base(tile.m_TileAxis, tile.m_Offset, tile.m_Status)
        {
            E_Direction = tile.E_Direction;
        }
    }
    public class LevelTilePortal : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Portal;
        public enum_TileDirection E_PortalDirection { get; private set; }
        public Vector3 m_worldPos;
        public LevelTilePortal(LevelTile current, enum_TileDirection _direction,Vector3 _worldPos) : base(current)
        {
            E_PortalDirection = _direction;
            m_worldPos = _worldPos;
        }
    }
    class LevelTileItemSub : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Item;
        public int m_ParentMainIndex { get; private set; }
        public LevelTileItemSub(LevelTile current, int _parentMainIndex) : base(current)
        {
            m_ParentMainIndex = _parentMainIndex;
        }
    }
    class LevelTileItemMain : LevelTile
    {
        public override enum_LevelTileType E_TileType => enum_LevelTileType.Main;
        public int m_LevelItemListIndex { get; private set; }
        public enum_LevelItemType m_LevelItemType { get; private set; }
        public enum_TileDirection m_ItemDirection { get; private set; }
        public List<int> m_AreaTiles { get; private set; }
        
        public LevelTileItemMain(LevelTile current, int levelItemListIndex, enum_LevelItemType levelItemType,enum_TileDirection _ItemDirection, List<int> _AreaTiles) : base(current)
        {
            m_LevelItemListIndex = levelItemListIndex;
            m_LevelItemType = levelItemType;
            m_AreaTiles = _AreaTiles;
            m_ItemDirection = _ItemDirection;
        }
    }
    #endregion
    #endregion
    #region GameStruct
    public struct SEntity : ISExcel
    {
        int i_index;
        string s_name;
        int e_type;
        float f_maxHealth;
        float f_maxArmor;
        float f_maxMana;
        float f_armorRegenSpeed;
        float f_armorRegenDuration;
        float f_moveSpeed;
        float f_chaseRange;
        float f_attackRange;
        bool b_movementObstacleCheck;
        bool b_battleObstacleCheck;
        int i_barrageIndex;
        RangeFloat fr_barrageDuration;
        public int m_Index=>i_index;
        public string m_Name=>s_name;
        public enum_EntityLevel m_Type => (enum_EntityLevel)e_type;
        public float m_MaxHealth => f_maxHealth;
        public float m_MaxArmor => f_maxArmor;
        public float m_MaxMana => f_maxMana;
        public float m_ArmorRegenSpeed => f_armorRegenSpeed;
        public float m_ArmorRegenDuration => f_armorRegenDuration;
        public float m_moveSpeed => f_moveSpeed;
        public float m_AIChaseRange => f_chaseRange;
        public float m_AIAttackRange => f_attackRange;
        public bool m_MovementCheckObstacle => b_movementObstacleCheck;
        public bool m_BattleCheckObsatacle => b_battleObstacleCheck;
        public int m_BarrageIndex => i_barrageIndex;
        public RangeFloat m_BarrageDuration => fr_barrageDuration;
        public void InitOnValueSet()
        {
        }
    }
    public struct SWeapon : ISExcel
    {
        int index;
        string s_name;
        int i_triggerType;
        int i_muzzleIndex;
        int i_projectileIndex;
        int i_impactIndex;
        float f_damage;
        float f_fireRate;
        float f_specialRate;
        float f_manaCost;
        int i_clipAmount;
        float f_spread;
        float f_reloadTime;
        int i_PelletsPerShot;
        float f_stunAfterShot;
        float f_horizontalDistance;
        float f_horizontalSpeed;
        float f_verticalAcceleration;
        float f_recoilHorizontal;
        float f_recoilVertical;

        public enum_Weapon m_Weapon => (enum_Weapon)index;
        public string m_Name => s_name;
        public enum_TriggerType m_TriggerType=>(enum_TriggerType)i_triggerType;
        public int m_MuzzleSFXIndex => i_muzzleIndex;
        public int m_ProjectileSFXIndex => i_projectileIndex;
        public int m_ImpactSFXIndex => i_impactIndex;
        
        public float m_Damage => f_damage;
        public float m_FireRate => f_fireRate;
        public float m_SpecialRate => f_specialRate;
        public float m_ManaCost => f_manaCost;
        public int m_ClipAmount => i_clipAmount;
        public float m_Spread => f_spread;
        public float m_ReloadTime => f_reloadTime;
        public int m_PelletsPerShot => i_PelletsPerShot;
        public float m_stunAfterShot => f_stunAfterShot;
        public float m_HorizontalSpeed => f_horizontalSpeed;
        public float m_HorizontalDistance => f_horizontalDistance;
        public float m_VerticalAcceleration => f_verticalAcceleration;
        public Vector2 m_RecoilPerShot => new Vector2(f_recoilHorizontal, f_recoilVertical);

        public void InitOnValueSet()
        {
        }
    }
    public struct SBarrage : ISExcel
    {
        int i_index;
        int i_barrageType;
        int i_muzzleIndex;
        int i_projectileIndex;
        int i_impactIndex;
        float f_firerate;
        RangeInt i_projectileCount;
        float i_projectileDamage;
        float i_projectileSpeed;
        int i_horizontalSpread;
        float i_projectileSpread;
        RangeInt ir_rangeExtension;
        float f_offsetExtension;
        public int m_Index => i_index;
        public enum_BarrageType m_BarrageType => (enum_BarrageType)i_barrageType;
        public int m_MuzzleSFXIndex => i_muzzleIndex;
        public int m_ProjectileSFXIndex => i_projectileIndex;
        public int m_ImpactSFXIndex => i_impactIndex;
        public float m_Firerate => f_firerate;
        public RangeInt m_ProjectileCount => i_projectileCount;
        public float m_ProjectileDamage => i_projectileDamage;
        public float m_ProjectileSpeed => i_projectileSpeed;
        public int m_HorizontalSpread => i_horizontalSpread;
        public float m_ProjectileSpread => i_projectileSpread;
        public RangeInt m_RangeExtension => ir_rangeExtension;
        public float m_OffsetExtension => f_offsetExtension;
        public void InitOnValueSet()
        {
        }
    }
    public struct SGenerateItem : ISExcel
    {
        int ec_index;
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
        public enum_TileStyle m_LevelStyle;
        public enum_TilePrefabDefinition m_LevelPrefabType;
        public Dictionary<enum_LevelItemType, RangeInt> m_ItemGenerate;
        public void InitOnValueSet()
        {
            m_LevelStyle = (enum_TileStyle)(ec_index / 10);
            m_LevelPrefabType = (enum_TilePrefabDefinition)(ec_index % 10);
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
        int ec_index;
        int i_waveCount;
        float f_eliteChance;
        RangeInt ir_militia;
        RangeInt ir_veteran;
        RangeInt ir_ranger;
        public int m_stageIndex;
        public enum_TileType m_TileType;
        public enum_BattleDifficulty m_Difficulty;
        public int m_WaveCount => i_waveCount;
        public float m_EliteChance => f_eliteChance;
        public Dictionary<enum_EntityLevel, RangeInt> m_EntityGenerate;
        public void InitOnValueSet()
        {
            m_stageIndex = (ec_index / 100);
            m_TileType = (enum_TileType)((ec_index-m_stageIndex*100) / 10);
            m_Difficulty = (enum_BattleDifficulty)(ec_index % 10);
            m_EntityGenerate = new Dictionary<enum_EntityLevel, RangeInt>();
            m_EntityGenerate.Add( enum_EntityLevel.Militia,ir_militia);
            m_EntityGenerate.Add(enum_EntityLevel.Veteran, ir_veteran);
            m_EntityGenerate.Add(enum_EntityLevel.Ranger, ir_ranger);
        }
    }
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
    #region Abandoned
    class Abandoned_WeaponAimAssistAccelerationCurve
    {
        Transform transform;
        Transform tf_Dot;
        LineRenderer m_lineRenderer;
        Abandoned_AimAssistSimulator m_Simulator;
        int m_CurveCount;
        List<Vector3> m_CurvePoints = new List<Vector3>();
        public Abandoned_WeaponAimAssistAccelerationCurve(Transform muzzle, int _curveCount, float duration, SWeapon weaponInfo)
        {
            transform = muzzle;
            tf_Dot = transform.Find("Dot");
            m_lineRenderer = muzzle.GetComponent<LineRenderer>();
            m_lineRenderer.positionCount = _curveCount;
            m_CurveCount = _curveCount;
            m_Simulator = new Abandoned_AimAssistSimulator(duration / _curveCount, muzzle.position, muzzle.forward, Vector3.down, weaponInfo.m_HorizontalSpeed,0, 0, weaponInfo.m_VerticalAcceleration, false);
            ;
        }
        public void Simulate(bool activate)
        {
            m_lineRenderer.enabled = activate;
            tf_Dot.SetActivate(activate);
            if (!activate)
                return;
            m_Simulator.ResetSimulator(transform.position, transform.forward);
            m_CurvePoints.Clear();
            m_CurvePoints.Add(transform.position);
            tf_Dot.SetActivate(false);
            for (int i = 0; i < m_CurveCount; i++)
            {
                Vector3 lookDirection; float offset;
                Vector3 currentPosition = m_Simulator.Next(out lookDirection, out offset);
                RaycastHit hit;
                if (Physics.Raycast(currentPosition, lookDirection, out hit, offset, GameLayer.Physics.I_StaticEntity) &&
                    (hit.collider.gameObject.layer != GameLayer.I_Entity || !hit.collider.GetComponent<HitCheckEntity>().m_Attacher.B_IsPlayer))
                {
                    m_CurvePoints.Add(hit.point);
                    tf_Dot.SetActivate(true);
                    tf_Dot.position = hit.point;
                    break;
                }
                m_CurvePoints.Add(currentPosition);
            }
            m_lineRenderer.positionCount = m_CurvePoints.Count;
            m_lineRenderer.SetPositions(m_CurvePoints.ToArray());
        }
    }

    class Abandoned_AimAssistSimulator : AccelerationSimulator
    {
        float m_simulateDelta;
        public Abandoned_AimAssistSimulator(float _simulateDelta, Vector3 startPos, Vector3 horizontalDirection, Vector3 verticalDirection, float horizontalSpeed, float horizontalAcceleration, float verticalSpeed, float verticalAcceleration, bool speedBelowZero = true) : base(startPos, horizontalDirection, verticalDirection, horizontalSpeed, horizontalAcceleration, verticalSpeed, verticalAcceleration, speedBelowZero)
        {
            m_simulateDelta = _simulateDelta;
        }
        public void ResetSimulator(Vector3 startPos, Vector3 horizontalDirection)
        {
            m_startPos = startPos;
            m_LastPos = startPos;
            m_HorizontalDirection = horizontalDirection;
            m_simulateTime = 0f;
        }
        public Vector3 Next(out Vector3 directionNext, out float offsetNext)
        {
            Vector3 curPos = Simulate(m_simulateTime);
            Vector3 nextPos = Simulate(m_simulateTime + m_simulateDelta);
            directionNext = nextPos - curPos;
            offsetNext = Vector3.Distance(curPos, nextPos);
            m_simulateTime += m_simulateDelta;
            return curPos;
        }
    }
    #endregion
    #endregion
}
