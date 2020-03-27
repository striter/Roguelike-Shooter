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
        
        OnBattleStart,
        OnBattleFinish,

        OnLevelStart,
        OnLevelFinished,
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
        UI_PlayerPerkStatus,
        UI_PlayerWeaponStatus,

        UI_OnWillAIAttack,

        UI_ChunkTeleportUnlock,

        UI_CampDataStatus,

        UI_PageOpen,
        UI_PageClose,
    }
    #endregion

    #region GameEnum
    public enum enum_Stage { Invalid = -1, Rookie = 1, Veteran = 2, Ranger = 3 }

    public enum enum_LevelType
    {
        Invalid = -1,

        Start=0,

        NormalBattle = 1,
        EliteBattle = 2,
        FinalBattle=3,

        Trader,
        Bonefire,
        RewardChest,
        WeaponReforge,

        StageEnd,
        GameEnd,
    }

    public enum enum_EntityController { Invalid = -1, None = 1, Player = 2, AI = 3, Device = 4, }

    public enum enum_EntityFlag { Invalid = -1, None = 0, Player = 1, Enermy = 2, Neutal = 3, }

    public enum enum_HitCheck { Invalid = -1, Static = 1, Entity = 2, Dynamic = 3, Interact = 4, }

    public enum enum_GameStyle { Invalid = -1, Forest = 1, Desert = 2, Frost = 3, Horde = 4, Undead = 5, }

    public enum enum_EnermyType { Invalid = -1, Melee = 1, E2 = 2, E3 = 3, E4 = 4, E5 = 5,E6=6,Elite=7 }

    public enum enum_Interaction
    {
        Invalid = -1,
        GameBegin, Bonfire, TradeContainer, PickupCoin, PickupHealth, PickupHealthPack, PickupArmor, RewardChest, PerkAcquire, WeaponReforge, Equipment, Weapon, Teleport, Portal, GameEnd,
        CampBegin, CampStage, CampDifficult, CampFarm, CampAction, CampEnd,
    }

    public enum enum_ProjectileFireType { Invalid = -1, Single = 1, MultipleFan = 2, MultipleLine = 3, };

    public enum enum_CastControllType { Invalid = -1, CastFromOrigin = 1, CastControlledForward = 2, CastAtTarget = 3, }

    public enum enum_CastTarget { Invalid = -1, Head = 1, Weapon = 2, Feet = 3 }

    public enum enum_CastAreaType { Invalid = -1, OverlapSphere = 1, ForwardBox = 2, ForwardCapsule = 3, ForwardTrapezium = 4, }

    public enum enum_HealthChangeMessage { Invalid = -1, Default = 0, DamageHealth = 1, ReceiveHealth = 2, DamageArmor = 3, ReceiveArmor = 4 }

    public enum enum_DamageType { Invalid = -1, Basic = 1, ArmorOnly = 2, HealthOnly = 3, }

    public enum enum_CharacterEffect { Invalid = -1, Freeze = 1, Cloak = 2, Scan = 3, }

    public enum enum_ExpireType { Invalid = -1, Preset = 1,  Perk = 2, }

    public enum enum_ExpireRefreshType { Invalid = -1, AddUp = 1, Refresh = 2, RefreshIdentity = 3, }

    public enum enum_EffectType { Invalid = -1, HeadAttach = 1, FeetAttach = 2, WeaponMesh = 3, }

    public enum enum_PlayerCharacter { Invalid = -1, Beth = 10001, }

    public enum enum_InteractCharacter { Invalid = -1, Trader = 20001, Trainer = 20002, }

    public enum enum_Rarity { Invalid = -1, Ordinary = 1, Advanced = 2, Rare = 3, Epic = 4 }

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
    }

    public enum enum_WeaponTrigger
    {
        Invalid = -1,
        Auto = 0,
        Storing = 1,
    }

    public enum enum_PlayerAnim
    {
        Invalid = -1,
        Rifle_1001 = 1001,
        Pistol_1002 = 1002,
        Crossbow_1003 = 1003,
        Heavy_1004 = 1004,
        HeavySword_2001 = 2001,
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
        Invalid = -1,

        EntityDamage,
        PlayerDamage,

        PlayerRevive,
    }

    public enum enum_GameMusic { Invalid = -1, StyledStart = 1, Relax, StyledEnd = 10, Fight }

    public enum enum_CampMusic { Invalid = -1, Relax = 0, }

    public enum enum_Option_FrameRate { Invalid = -1, Normal = 45, High = 60, }

    public enum enum_Option_Effect { Invalid = -1, Normal, High }

    public enum enum_Option_Bloom { Invalid = -1, Off, Normal, High }

    public enum enum_CampFarmItemStatus { Invalid = -1, Empty = 0, Locked = 1, Decayed = 2, Progress1 = 10, Progress2, Progress3, Progress4, Progress5 }

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

        public static bool IsBattleLevel(this enum_LevelType levelType)
        {
            switch(levelType)
            {
                default:
                    return false;
                case enum_LevelType.EliteBattle:
                case enum_LevelType.FinalBattle:
                case enum_LevelType.NormalBattle:
                    return true;
            }
        }

        public static enum_ChunkType GetChunkType(this enum_LevelType eventType)
        {
            if (eventType.IsBattleLevel())
                return enum_ChunkType.Battle;

            switch(eventType)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!"+eventType);
                    return enum_ChunkType.Invalid;
                case enum_LevelType.Start:
                    return enum_ChunkType.Start;
                case enum_LevelType.Bonefire:
                case enum_LevelType.RewardChest:
                case enum_LevelType.Trader:
                case enum_LevelType.WeaponReforge:
                    return enum_ChunkType.Event;
            }
        }

        public static enum_ChunkPortalType GetPortalType(this enum_LevelType eventType)
        {
            if (eventType.IsBattleLevel())
                return enum_ChunkPortalType.Battle;

            switch (eventType)
            {
                default:
                    Debug.LogError("Invalid Convertions Here!");
                    return enum_ChunkPortalType.Invalid;
                case enum_LevelType.StageEnd:
                case enum_LevelType.GameEnd:
                    return  enum_ChunkPortalType.Battle;
                case enum_LevelType.Bonefire:
                case enum_LevelType.RewardChest:
                case enum_LevelType.Trader:
                case enum_LevelType.WeaponReforge:
                    return enum_ChunkPortalType.Event;
            }
        
        }
    }
}