using LevelSetting;
using UnityEngine;

namespace GameSetting
{
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
        
        OnStageStart,
        OnStageFinished,

        OnGameLoadBegin,
        OnGameFinish,
        OnGameExit,

        OnCampStart,
    }

    enum enum_BC_UIStatus
    {
        Invalid = -1,

        UI_PlayerCommonUpdate,
        UI_PlayerCurrencyUpdate,
        UI_PlayerInteractUpdate,
        UI_PlayerInteractPickup,
        UI_PlayerHealthUpdate,
        UI_PlayerPerkStatus,
        UI_PlayerWeaponUpdate,

        UI_GameMissionUpdate,

        UI_OnWillAIAttack,

        UI_CampCurrencyStatus,

        UI_PageOpen,
        UI_PageClose,
    }
    #endregion

    #region GameEnum
    public enum enum_GameStage {
        Invalid = -1,
        Rookie = 1,
        Militia = 2,
        Veteran = 3,
        Ranger = 4,
        Elite = 5,
    }

    public enum enum_GameDifficulty
    {
        Invalid=-1,
        Normal=1,
        Hard=2,
        Hell=3,
    }

    public enum enum_GamePortalType
    {
        Invalid = -1,
        StageEnd,
        GameWin,
    }

    public enum enum_GameEventType
    {
        Invalid=-1,
        CoinsSack=1,
        HealthpackTrade=2,
        WeaponTrade=3,
        WeaponReforge=4,
        WeaponVendor=5,
        WeaponRecycle=6,
        PerkLottery=7,
        PerkSelect=8,
        PerkShrine=9,
        BloodShrine=10,
        HealShrine=11,
        SafeBox=12,
    }

    public enum enum_EntityType { Invalid = -1, None = 1, Player = 2, AIWeaponHelper = 3, AIWeaponModel = 4, Device = 5, }

    public enum enum_EntityFlag { Invalid = -1, None = 0, Player = 1, Enermy = 2, Neutal = 3, }

    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, Interact = 4, }

    public enum enum_GameStyle { Invalid = -1, Forest = 1, Desert = 2, Frost = 3, Horde = 4, Undead = 5, }
    
    public enum enum_Interaction
    {
        Invalid = -1,
        GameBegin,
        Bonfire, WeaponReforge,WeaponRecycle,SafeCrack,CoinSack, WeaponVendorMachine,
        PickupCoin, PickupHealth, PickupHealthPack, PickupArmor, PerkPickup, PickupWeapon,PickupKey,
        TradeContainer, PerkSelect,PerkLottery,PerkShrine,BloodShrine,HealShrine,
        PickupArmoryBlueprint,PickupArmoryParts,
        SignalTower, Portal,
        GameEnd,

        CampBegin, CampGameEnter, CampDifficulty, CampArmory,CampDailyReward,CampBillboard,CampCharaceterSelect, CampEnd,
    }

    public enum enum_ProjectileFireType { Invalid = -1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, }

    public enum enum_CastTarget { Invalid = -1, Head = 1, Weapon = 2, Feet = 3 }

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, ForwardTrapezium = 4, }

    public enum enum_HealthChangeMessage { Invalid = -1, Default = 0, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Basic = 1, Armor = 2, Health = 3,HealthPenetrate=10, }
    
    public enum enum_ExpireType { Invalid = -1, PresetBuff = 1,  Perk = 2,EnermyElite=4, }

    public enum enum_ExpireRefreshType { Invalid = -1, AddUp = 1, Refresh = 2,  }

    public enum enum_EffectType { Invalid = -1, HeadAttach = 1, FeetAttach = 2, WeaponMesh = 3, }

    public enum enum_PlayerCharacter { Invalid = -1, Beth = 1001,Assassin=1002,Railer=1003, }
    
    public enum enum_InteractCharacter { Invalid = -1, Trader = 2001, Trainer = 2002, }

    public enum enum_MercenaryCharacter { Invalid = -1, Militia = 3001, Veteran = 3002 }

    public enum enum_Rarity { Invalid = -1, Ordinary = 1, Advanced = 2, Rare = 3, Epic = 4 }

    public enum enum_PlayerWeaponType
    {
        Invalid=-1,
        ProjectileShot,
        ProjectileShotMulti,
        ProjectileStore,

        ThrowableProjectle,

        CastMelee,
        CastDuration,
    }
    public enum enum_PlayerWeapon
    {
        Invalid = -1,
        RailPistol = 101,
        Railgun = 102,
        M82A1 = 103,
        Kar98 = 104,
        UZI = 105,
        UMP45 = 106,
        SCAR = 107,
        M16A4 = 108,
        AKM = 109,
        P92 = 110,
        DE = 111,
        XM1014 = 112,
        S686 = 113,
        Crossbow = 114,
        RocketLauncher = 115,
        Minigun = 116,

        Flamer = 201,
        Driller = 202,
        Bouncer = 203,
        Tesla = 204,

        HeavySword = 301,

        LavaWand = 401,
        PoisonWand = 402,
        FrostWand = 403,

        Grenade=501,
    }


    public enum enum_GameVFX
    {
        Invalid = -1,

        EntityDamage,
        PlayerDamage,

        PlayerRevive,
    }


    public enum enum_GameMusic { Invalid = -1, StyledStart = 1, Relax, StyledEnd = 10, Fight }

    public enum enum_CampMusic { Invalid = -1, Relax = 0, }

    public enum enum_Option_FrameRate { Invalid = -1, Normal = 45, High = 60, }

    public enum enum_Option_Effect { Invalid = -1, Off, Normal, High }
    
    public enum enum_UITipsType { Invalid = -1, Normal = 0, Warning = 1, Error = 2 }
    #endregion

    public static class GameEnumConvertions
    {
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

        public static bool IsInteractExpire(this enum_ExpireType type) =>  type == enum_ExpireType.Perk;

        public static bool IsGameInteract(this enum_Interaction interact) => interact > enum_Interaction.GameBegin && interact < enum_Interaction.GameEnd;

        public static enum_ChunkPortalType GetPortalType(this enum_GamePortalType eventType)
        {
            switch (eventType)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!"+ eventType);
                    return enum_ChunkPortalType.Invalid;
                case enum_GamePortalType.StageEnd:
                    return enum_ChunkPortalType.Event;
                case enum_GamePortalType.GameWin:
                    return enum_ChunkPortalType.Reward;
            }
        
        }
    }
}