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

        OnGameTransmitStatus,
        OnGameTransmitEliteStatus,

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

        UI_OnWillAIAttack,

        UI_CampCurrencyStatus,

        UI_PageOpen,
        UI_PageClose,
    }
    #endregion

    #region GameEnum
    public enum enum_BattleStage {
        Invalid = -1,
        Rookie = 1,
        Militia = 2,
        Veteran = 3,
        Ranger = 4,
        Elite = 5,
    }

    public enum enum_BattleDifficulty
    {
        Invalid=-1,
        Normal=1,
        Hard=2,
        Hell=3,
    }

    public enum enum_BattlePortalTye
    {
        Invalid = -1,
        StageEnd,
        BattleWin,
    }

    public enum enum_BattleEvent
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

    public enum enum_EntityType { Invalid = -1, None = 1, Player = 2, BattleEntity = 3,BattleDevice=4, }

    public enum enum_EntityFlag { Invalid = -1, None = 0, Player = 1, Enermy = 2, Neutal = 3, }

    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, Interact = 4, }

    public enum enum_BattleStyle { Invalid = -1, Forest = 1, Desert = 2, Frost = 3, Horde = 4, Undead = 5, }
    
    public enum enum_Interaction
    {
        Invalid = -1,
        BattleBegin,
        Bonfire, WeaponReforge,WeaponRecycle,SafeCrack,CoinSack, WeaponVendorMachine,
        PickupCoin, PickupHealth, PickupHealthPack, PickupArmor, PerkPickup, PickupWeapon,PickupKey,
        TradeContainer, PerkSelect,PerkLottery,PerkShrine,BloodShrine,HealShrine,
        PickupArmoryBlueprint,
        SignalTower, Portal,
        BattleEnd,

        CampBegin, CampBattleEnter,CampBattleResume, CampDifficulty, CampArmory,CampDailyReward,CampBillboard,CampCharaceterSelect, CampEnd,
    }

    public enum enum_DamageIdentity { Invalid=-1,Default,Environment,Expire,PlayerWeapon,PlayerAbility, }

    public enum enum_ProjectileFireType { Invalid = -1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, }

    public enum enum_CastTarget { Invalid = -1, Head = 1, Weapon = 2, Feet = 3 }

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, ForwardTrapezium = 4, }

    public enum enum_HealthChangeMessage { Invalid = -1, Default = 0, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Basic = 1, Armor = 2, Health = 3,True=4, }
    
    public enum enum_ExpireType { Invalid = -1, PresetBuff = 1,  Perk = 2,EnermyElite=4, }

    public enum enum_ExpireRefreshType { Invalid = -1, AddUp = 1, Refresh = 2,  }

    public enum enum_EffectType { Invalid = -1, HeadAttach = 1, FeetAttach = 2, WeaponMesh = 3, }

    public enum enum_PlayerCharacter { Invalid = -1, Beth = 1001,Vampire=1002,Railer=1003, }
    
    public enum enum_PlayerCharacterEnhance {Invalid=-1,None=0,Health,Armor,MovementSpeed,StartWeapon,StageCoin,Critical,Ability,DropWeapon, Max, }

    public enum enum_InteractCharacter { Invalid = -1, Trader = 2001, Trainer = 2002, }

    public enum enum_MercenaryCharacter { Invalid = -1, Militia = 3001, Veteran = 3002 }

    public enum enum_Rarity { Invalid = -1, Ordinary = 1, Advanced = 2, Rare = 3, Epic = 4 }

    public enum enum_PlayerWeaponBaseType { Invalid = -1, Projectile, Paracurve, Cast, Item, Shield, }

    public enum enum_PlayerWeaponTriggerType  {  Invalid = -1,  Auto = 1,  Store = 2,  }

    public enum enum_PlayerWeaponIdentity
    {
        Invalid = -1,
        RailPistol = 101, Railgun = 102,M82A1 = 103,Kar98 = 104,UZI = 105,UMP45 = 106,SCAR = 107,M16A4 = 108,AKM = 109,P92 = 110,DE = 111,XM1014 = 112,S686 = 113,Crossbow = 114,RocketLauncher = 115,Minigun = 116,Bow=117,MultishotBow=118,

        Flamer = 201,Driller = 202,Bouncer = 203,Tesla = 204,

        HeavySword = 301,Katana=302,HammerOfDawn=303,BloodThirster=304,

        LavaWand = 401,PoisonWand = 402,FrostWand = 403,BlastWand=404,DestroyerWand=405,

        Grenade=501,HealPotion=502, SpawnerBlastTurret =503,SpawnerBlastFreezeTurret=504,SpawnerSkeleton=505,

        SheildDrain=601,
    }

    public enum enum_GameTimeScaleType { Invalid=-1,Base,Extra,Impact}

    public enum enum_BattleVFX
    {
        Invalid = -1,

        EntityDamage,
        PlayerDamage,

        PlayerRevive,
    }


    public enum enum_BattleMusic { Invalid = -1, StyledStart = 1, Relax, StyledEnd = 10, Fight }

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

        public static bool IsBattleInteract(this enum_Interaction interact) => interact > enum_Interaction.BattleBegin && interact < enum_Interaction.BattleEnd;

        public static enum_ChunkPortalType GetPortalType(this enum_BattlePortalTye eventType)
        {
            switch (eventType)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!"+ eventType);
                    return enum_ChunkPortalType.Invalid;
                case enum_BattlePortalTye.StageEnd:
                    return enum_ChunkPortalType.Event;
                case enum_BattlePortalTye.BattleWin:
                    return enum_ChunkPortalType.Reward;
            }
        
        }
    }
}