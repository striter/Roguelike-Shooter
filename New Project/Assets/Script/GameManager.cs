using GameSetting;
using UnityEngine;

public class GameManager : SingletonMono<GameManager> {
    public bool B_TestMode { get; private set; } = false;
    Transform TF_PlayerStart;
    protected override void Awake()
    {
#if UNITY_EDITOR
        B_TestMode = true;
#else
        B_TestMode=false;
#endif
        base.Awake();
        TF_PlayerStart = transform.Find("PlayerStart");
        ObjectManager.Init();
        TBroadCaster<enum_BC_UIStatusChanged>.Init();
    }
    private void Start()
    {
        ObjectManager.SpawnEntity(enum_Entity.Player, TF_PlayerStart);
    }
}
public static class ObjectManager
{
    static Transform TF_Entity;
    
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;

        TExcel.Properties<SEntity>.Init();
        TExcel.Properties<SWeapon>.Init();

        TCommon.TraversalEnum((enum_Entity type) => 
        {
            ObjectPoolManager<enum_Entity,EntityBase>.Register(type,TResources.Instantiate<EntityBase>("Entity/"+type.ToString()), enum_PoolSaveType.DynamicMaxAmount,1,null);
        });

        TCommon.TraversalEnum((enum_Weapon type) =>
        {
            ObjectPoolManager<enum_Weapon, WeaponBase>.Register(type, TResources.Instantiate<WeaponBase>("Weapon/" + type.ToString()), enum_PoolSaveType.DynamicMaxAmount,1,null);
        });

        TCommon.TraversalEnum((enum_SFX type) =>
        {
            ObjectPoolManager<enum_SFX, SFXBase>.Register(type, TResources.Instantiate<SFXBase>("SFX/" + type.ToString()), enum_PoolSaveType.StaticMaxAmount, 100, null);
        });

    }
    public static EntityBase SpawnEntity(enum_Entity type,Transform toTrans=null)
    {
        EntityBase entity= ObjectPoolManager<enum_Entity, EntityBase>.Spawn(type, TF_Entity);
        entity.Init(TExcel.Properties<SEntity>.PropertiesList.Find(p => p.m_Type == type));
        entity.transform.position = toTrans.position;
        return entity;
    }
    public static void RecycleEntity(enum_Entity type, EntityBase target)
    {
        ObjectPoolManager<enum_Entity, EntityBase>.Recycle(type, target);
    }
    public static WeaponBase SpawnWeapon(enum_Weapon type, EntityPlayerBase toPlayer)
    {
        WeaponBase weapon = ObjectPoolManager<enum_Weapon, WeaponBase>.Spawn(type, TF_Entity);
        weapon.Init(TExcel.Properties<SWeapon>.PropertiesList.Find(p => p.m_Type == type));
        return weapon;
    }
    public static SFXBase SpawnSFX(enum_SFX type, Transform toTrans)
    {
        SFXBase sfx = ObjectPoolManager<enum_SFX, SFXBase>.Spawn(type, toTrans);
        sfx.transform.SetParent(TF_Entity);

        sfx.Init(type);
        return sfx;
    }
    public static void RecycleSFX(enum_SFX type, SFXBase sfx)
    {
        ObjectPoolManager<enum_SFX, SFXBase>.Recycle(type, sfx);
    }
}