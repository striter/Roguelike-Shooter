using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    public PlayerBase m_LocalPlayer { get; private set; }
    Transform m_Players;
    protected override void Awake()
    {
        base.Awake();
        m_Players= new GameObject("Players").transform;
        Cursor.lockState = CursorLockMode.Locked;
        EntityManager.Init();
        m_LocalPlayer=EntityManager.SpawnLiving<PlayerBase>(enum_LivingType.Player, m_Players);
    }
    private void Start()
    {
        PE_Bloom bloom= PostEffectManager.Instance.SetPostEffect<PE_Bloom>();
        bloom.I_DownSample = 10;
        bloom.I_Iterations = 2;
        bloom.F_BlurSpread = 1f;
        bloom.F_LuminanceThreshold = .8f;
    }
    //Test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Time.timeScale = Time.timeScale == 1 ? .2f : 1;
        }
    }
}

public static class EntityManager
{
    public static Transform tf_PickupRoot = new GameObject("PickupRoot").transform;
    public static void Init()
    {

        TCommon.TraversalEnum((enum_WeaponSFX weaponSFX) => {
            ObjectPoolManager<enum_WeaponSFX, SFXBase>.Register(weaponSFX, TResources.Instantiate<SFXBase>("SFX/" + weaponSFX.ToString()), enum_PoolSaveType.DynamicMaxAmount, 50,
                (SFXBase sfx) => { sfx.Init(weaponSFX); });
        });

        TCommon.TraversalEnum((enum_WeaponType weaponPlayer) =>
        {
            ObjectPoolManager<enum_WeaponType, WeaponPlayer>.Register(weaponPlayer, TResources.Instantiate<WeaponPlayer>("Weapon/Player_" + weaponPlayer.ToString()), enum_PoolSaveType.StaticMaxAmount, 1, null);
        });

        TCommon.TraversalEnum((enum_WeaponType weaponNPC)=>
        {
            ObjectPoolManager<enum_WeaponType, WeaponNPC>.Register(weaponNPC, TResources.Instantiate<WeaponNPC>("Weapon/NPC_" + weaponNPC.ToString()), enum_PoolSaveType.DynamicMaxAmount, 1, null);
        });

        TCommon.TraversalEnum((enum_Pickup pickup) =>
        {
            ObjectPoolManager<enum_Pickup, PickupBase>.Register(pickup, TResources.Instantiate<PickupBase>("Pickup/" + pickup.ToString()), enum_PoolSaveType.DynamicMaxAmount, 5, null);
        });

        TCommon.TraversalEnum((enum_LivingType living) =>
        {
            ObjectPoolManager<enum_LivingType, LivingBase>.Register(living, TResources.Instantiate<LivingBase>("Living/" + living.ToString()), enum_PoolSaveType.DynamicMaxAmount, 1, null);
        });
    }
    public static T SpawnLiving<T>(enum_LivingType type, Transform toTrans) where T:LivingBase
    {
        return ObjectPoolManager<enum_LivingType, LivingBase>.Spawn(type, toTrans) as T;
    }
    public static void RecycleLiving<T>(enum_LivingType type, T target) where T:LivingBase
    {
        ObjectPoolManager<enum_LivingType, LivingBase>.Recycle(type,target);
    }
    public static T SpawnWeaponSFX<T>(enum_WeaponSFX sfx, Transform toTrans) where T: SFXBase
    {
        return ObjectPoolManager<enum_WeaponSFX, SFXBase>.Spawn(sfx, toTrans) as T;
    }
    public static void RecycleWeaponSFX(enum_WeaponSFX sfx, SFXBase sfxTarget)
    {
        ObjectPoolManager<enum_WeaponSFX, SFXBase>.Recycle(sfx, sfxTarget);
    }
    public static WeaponPlayer SpawnPlayerWeapon(enum_WeaponType type, Transform toTrans)
    {
        return ObjectPoolManager<enum_WeaponType, WeaponPlayer>.Spawn(type, toTrans);
    }
    public static WeaponNPC SpawnNPCWeapon(enum_WeaponType type, Transform toTrans)
    {
        return ObjectPoolManager<enum_WeaponType, WeaponNPC>.Spawn(type,toTrans);
    }
    public static void RecycleWeaponBase(enum_WeaponType type, WeaponBase weapon)
    {
        if(weapon as WeaponPlayer != null)
            ObjectPoolManager<enum_WeaponType, WeaponPlayer>.Recycle(type, weapon as WeaponPlayer);
        else if(weapon as WeaponNPC != null)
            ObjectPoolManager<enum_WeaponType, WeaponNPC>.Recycle(type, weapon as WeaponNPC);

    }
    public static T SpawnPickup<T>(PickupInfoBase targetInfo, Transform startTrans) where T: PickupBase
    {
        PickupBase target = ObjectPoolManager<enum_Pickup, PickupBase>.Spawn(targetInfo.E_PickupType, startTrans);
        target.transform.SetParent(tf_PickupRoot);
        target.m_PickUpInfo = targetInfo;
        return target as T;
    }
    public static void SpawnPickup<T>(enum_Pickup type, Transform startTrans) where T : PickupBase
    {
        PickupBase target = ObjectPoolManager<enum_Pickup, PickupBase>.Spawn(type, startTrans);
        target.transform.SetParent(tf_PickupRoot);
        target.m_PickUpInfo = type.ToDefaultPickupInfo();
    }
    public static void RecyclePickup(enum_Pickup identity,PickupBase target)
    {
        ObjectPoolManager<enum_Pickup, PickupBase>.Recycle(identity,target);
    }
}

public class GameSettings
{
    public const int CI_DecalLifeTime = 20;
    public const int CI_BulletTrailLifeTime = 2;
    public const int CI_BulletSpeed = 800;
    public const int CI_ShellLifeTime = 10;
    public const int CI_ThrowForce = 500;
    public const int CI_DurationThrowWeaponCanBePickedUpAgain = 1;

    public const int CI_BulletMaxDistance = 100;

    public const int CI_PlayerInteractRange = 3;
    public const float CF_PlayerGroundedDetect = .3f;
    public const float CF_NormalGravity = .98f;
    public const float CF_PlayerJumpSpeed = .4f;
    public const float CF_PlayerSlopeSliderParam = 10f;
    public const float CF_PlayerHoldingItemColldown = .5f;

    public const float CF_StepDistanceWalking = 1f;
    public const float CF_StepDistanceSprinting = 2.4f;
    public const int CI_SprintCameraRollAnimation = 4;

    public const int CI_UI_InteractMaxShow = 10;
}
public class GameLayersPhysics
{
    public static readonly int IL_WeaponHit = 1 << GameLayers.IL_Dynamic | 1 << GameLayers.IL_Living|1<< GameLayers.IL_Static|1<<IL_Living;

    public static readonly int IL_Dynamic = 1 << GameLayers.IL_Dynamic;
    public static readonly int IL_Living = 1 << GameLayers.IL_Living;
    public static readonly int IL_Static = 1 << GameLayers.IL_Static;
    public static readonly int IL_Interact = 1 << GameLayers.IL_Static | 1 << GameLayers.IL_Dynamic|1<<GameLayers.IL_Living;
    public static readonly int IL_AllLiving = 1 << GameLayers.IL_Living | 1 << GameLayers.IL_Player;
}
public class GameLayers
{
    public static readonly int IL_Living = LayerMask.NameToLayer("Living");
    public static readonly int IL_Dynamic = LayerMask.NameToLayer("Dynamic");
    public static readonly int IL_Static = LayerMask.NameToLayer("Static");
    public static readonly int IL_Default = LayerMask.NameToLayer("Default");
    public static readonly int IL_Player = LayerMask.NameToLayer("Player");
    public static readonly int IL_DynamicDetector = LayerMask.NameToLayer("DynamicDetector");
    public static readonly int IL_LivingPlayerDetector = LayerMask.NameToLayer("LivingPlayerDetector");
}
public class GameTags
{
    public static readonly string CT_Pickup = "Pickup";
}
#region Enum
public enum enum_Flags
{
    Invalid=-1,
    Neutal=0,
    Humans,
    TesterCO,
}
public enum enum_DamageType
{
    Invalid=-1,
    Melee,
    Range,
    DangerZone,
    Interact,
}
public enum enum_checkObjectType
{
    Invalid = -1,
    Living,
    Static,
    Dynamic,
    Player,
}
public enum enum_EntityHitType
{
    Invalid = -1,
    Normal,
    Critical,
}
public enum enum_MaterialType
{
    Invalid = -1,
    Brick,
    Concrete,
    Dirt,
    Foliage,
    Glass,
    Metal,
    Plaster,
    Rock,
    Water,
    Wood,
    Flesh,
}

public enum enum_PickupType
{
    Invalid = -1,
    Ammo = 1,
    Weapon = 2,
}

#region EntityManagerUsing Enums
public enum enum_WeaponType
{
    Invalid = -1,
    Shotgun,
    Rifle,
    Pistol,
}
public enum enum_AmmoType
{
    Invalid = -1,
    Shotgun12GUAGE,
    Rifle556MM,
    Dot45ACP,
}
public enum enum_LivingType
{
    Invalid = -1,
    Player,
    Tester32,
    Soldier,
    ContainerChips,
}

public enum enum_Pickup
{
    Invalid = -1,
    WeaponShotgun,
    Shotgun12GUAGEBox,
    Shotgun12GUAGEShell,
    WeaponRifle,
    Rifle556Bullet,
    Rifle556Magzine,
    WeaponPistol,
    Dot45ACPBullet,
    Dot45ACPMagzine,
    AttachmentPistolSilencer,
}
public enum enum_WeaponSFX
{
    Invalid = -1,
    Bullet = 100,
    MuzzleShotgun,
    MuzzleRifle,
    MuzzlePistol,
    Shell12GUAGE,
    Shell556MM,
    ShellDot45ACP,

    ImpactConcrete = 200,
    ImpactBrick,
    ImpactDirt,
    ImpactFoliage,
    ImpactGlass,
    ImpactMetal,
    ImpactPlaster,
    ImpactRock,
    ImpactWater,
    ImpactWood,
    ImpactFleshNormal,
    ImpactFleshCritical,
}
#endregion
public static class GameEnums_Relations_Extend
{
    public static string ToWeaponName(this enum_WeaponType weaponType)
    {
        switch (weaponType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + weaponType.ToString()); return "";
            case enum_WeaponType.Rifle: return "M16 Carbine";
            case enum_WeaponType.Pistol: return "Cal .45 Auto";
            case enum_WeaponType.Shotgun: return "SPAS 13 Combat";
        }
    }
    public static int ToGameLayer(this enum_checkObjectType type)
    {
        switch (type)
        {
            default:Debug.LogError("Add More GameLayers Here:"+type.ToString());break;
            case enum_checkObjectType.Dynamic: return GameLayers.IL_Dynamic;
            case enum_checkObjectType.Static:return GameLayers.IL_Static;
            case enum_checkObjectType.Living: return GameLayers.IL_Living;
            case enum_checkObjectType.Player:return GameLayers.IL_Player;
        }
        return GameLayers.IL_Default;
    }
    public static enum_WeaponSFX ToWeaponSFX(this enum_EntityHitType hitType)
    {
        switch (hitType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + hitType.ToString()); return enum_WeaponSFX.Invalid;
            case enum_EntityHitType.Normal:return enum_WeaponSFX.ImpactFleshNormal;
            case enum_EntityHitType.Critical:return enum_WeaponSFX.ImpactFleshCritical;
        }
    }
    public static enum_WeaponSFX ToWeaponSFX(this enum_MaterialType materialType)
    {
        switch (materialType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + materialType.ToString()); return enum_WeaponSFX.Invalid;
            case enum_MaterialType.Brick: return enum_WeaponSFX.ImpactBrick;
            case enum_MaterialType.Concrete:return enum_WeaponSFX.ImpactConcrete;
            case enum_MaterialType.Dirt: return enum_WeaponSFX.ImpactDirt;
            case enum_MaterialType.Foliage: return enum_WeaponSFX.ImpactFoliage;
            case enum_MaterialType.Glass: return enum_WeaponSFX.ImpactGlass;
            case enum_MaterialType.Metal:return enum_WeaponSFX.ImpactMetal;
            case enum_MaterialType.Plaster:return enum_WeaponSFX.ImpactPlaster;
            case enum_MaterialType.Rock: return enum_WeaponSFX.ImpactRock;
            case enum_MaterialType.Water: return enum_WeaponSFX.ImpactWater;
            case enum_MaterialType.Wood: return enum_WeaponSFX.ImpactWood;
            case enum_MaterialType.Flesh: return enum_WeaponSFX.ImpactFleshNormal;
        }
    }
    public static enum_Pickup ToPickup(this enum_WeaponType weaponType)
    {
        switch(weaponType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + weaponType.ToString()); return enum_Pickup.Invalid;
            case enum_WeaponType.Shotgun:return enum_Pickup.WeaponShotgun;
            case enum_WeaponType.Rifle:return enum_Pickup.WeaponRifle;
            case enum_WeaponType.Pistol:return enum_Pickup.WeaponPistol;
        }
        
    }
    public static enum_WeaponType ToWeaponType(this enum_Pickup pickupType)
    {
        switch (pickupType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + pickupType.ToString()); return enum_WeaponType.Invalid;
            case enum_Pickup.WeaponRifle:return enum_WeaponType.Rifle;
            case enum_Pickup.WeaponShotgun:return enum_WeaponType.Shotgun;
            case enum_Pickup.WeaponPistol:return enum_WeaponType.Pistol;
        }
    }
    public static enum_WeaponSFX ToBulletShell(this enum_AmmoType ammoType)
    {
        switch (ammoType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + ammoType.ToString()); return enum_WeaponSFX.Invalid;
            case enum_AmmoType.Shotgun12GUAGE:return enum_WeaponSFX.Shell12GUAGE;
            case enum_AmmoType.Rifle556MM:return enum_WeaponSFX.Shell556MM;
            case enum_AmmoType.Dot45ACP:return enum_WeaponSFX.ShellDot45ACP;
        }
    }
    public static enum_WeaponSFX ToWeaponMuzzle(this enum_WeaponType weaponType)
    {
        switch (weaponType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + weaponType.ToString()); return enum_WeaponSFX.Invalid;
            case enum_WeaponType.Shotgun: return enum_WeaponSFX.MuzzleShotgun;
            case enum_WeaponType.Rifle: return enum_WeaponSFX.MuzzleRifle;
            case enum_WeaponType.Pistol:return enum_WeaponSFX.MuzzlePistol;
        }
       
    }
    public static enum_Pickup ToPickup(this enum_AmmoType ammoType,int count)
    {
        switch (ammoType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + ammoType.ToString()); return enum_Pickup.Invalid;
            case enum_AmmoType.Shotgun12GUAGE: return count == 1 ? enum_Pickup.Shotgun12GUAGEShell : enum_Pickup.Shotgun12GUAGEBox;
            case  enum_AmmoType.Rifle556MM:  return count == 1 ? enum_Pickup.Rifle556Bullet:enum_Pickup.Rifle556Magzine;
            case enum_AmmoType.Dot45ACP:return count == 1 ? enum_Pickup.Dot45ACPBullet : enum_Pickup.Dot45ACPMagzine;
        }
    }
    public static PickupInfoBase ToDefaultPickupInfo(this enum_Pickup pickupType)
    {
        switch (pickupType)
        {
            default: Debug.LogError("Please Add More Type Convert Here!:" + pickupType.ToString()); return new PickupInfoBase(pickupType);
            case enum_Pickup.WeaponShotgun: return  new PickupInfoWeaponShotgun(7, true);
            case enum_Pickup.WeaponPistol: return new PickupInfoWeapon(12, enum_WeaponType.Pistol);
            case enum_Pickup.WeaponRifle: return new PickupInfoWeapon(30, enum_WeaponType.Rifle);

            case enum_Pickup.Shotgun12GUAGEBox: return new PickupInfoAmmo(enum_AmmoType.Shotgun12GUAGE, 10);
            case enum_Pickup.Shotgun12GUAGEShell:return new PickupInfoAmmo(enum_AmmoType.Shotgun12GUAGE, 1);
            case enum_Pickup.Rifle556Bullet: return new PickupInfoAmmo(enum_AmmoType.Rifle556MM, 1);
            case enum_Pickup.Rifle556Magzine: return new PickupInfoAmmo(enum_AmmoType.Rifle556MM, 30);
            case enum_Pickup.Dot45ACPBullet:return new PickupInfoAmmo(enum_AmmoType.Dot45ACP, 1);
            case enum_Pickup.Dot45ACPMagzine:return new PickupInfoAmmo(enum_AmmoType.Dot45ACP, 12);
        }
    }
}
#endregion