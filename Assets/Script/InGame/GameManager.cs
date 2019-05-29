using GameSetting;
using System;
using System.Collections.Generic;
using TExcel;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : SingletonMono<GameManager>
{
    public Dictionary<int, EntityBase> m_Entities { get; private set; } = new Dictionary<int, EntityBase>();
    public EntityBase m_LocalPlayer { get; private set; } = null;
    public static CPlayerSave m_PlayerInfo { get; private set; }
    public bool B_TestMode { get; private set; } = false;
    public enum_LevelStyle E_TESTSTYLE = enum_LevelStyle.Desert;
    protected override void Awake()
    {
#if UNITY_EDITOR
        B_TestMode = true;
#endif
        base.Awake();
        ExcelManager.Init();
        ObjectManager.Init();
        TBroadCaster<enum_BC_UIStatusChanged>.Init();
        TBroadCaster<enum_BC_GameStatusChanged>.Init();

        m_PlayerInfo = TGameData<CPlayerSave>.Read();

        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnSpawnEntity, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnRecycleEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<Vector3>(enum_BC_GameStatusChanged.OnLevelStart, OnLevelStart);
    }

    private void OnDestroy()
    {
        TGameData<CPlayerSave>.Save(m_PlayerInfo);

        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnSpawnEntity, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnRecycleEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<Vector3>(enum_BC_GameStatusChanged.OnLevelStart, OnLevelStart);
    }

    private void Start()        //Entrance Of Whole Game
    {
        PreInit();
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnGameStart);
    }
    void PreInit()      //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        EnviormentManager.Instance.GenerateEnviorment(E_TESTSTYLE);
        GC.Collect();
        m_LocalPlayer = ObjectManager.SpawnEntity(enum_Entity.Player);
    }

    //Call Enviorment Manager To Prepare And Start Generate All Enermy
    void OnLevelStart(Vector3 levelStartPos)
    {
        m_LocalPlayer.transform.position = levelStartPos;
        //Generate All Enermy To Be Continued,
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelFinish);       //Test
    }
    //Call Enviorment Manager To Generate Portals , Then Go Back To OnLevelChange From Enviorment Manager
    void OnLevelFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelFinish);

    }

    //Entity Management
    void OnSpawnEntity(EntityBase entity)
    {
        m_Entities.Add(entity.I_EntityID, entity);
    }
    void OnRecycleEntity(EntityBase entity)
    {
        m_Entities.Remove(entity.I_EntityID);
    }

    public void OnEntityFall(HitCheckEntity hitcheck)      //On Player Falls To Ocean ETC
    {
        hitcheck.TryHit(hitcheck.m_Attacher.B_IsPlayer ? GameConst.F_DamagePlayerFallInOcean : hitcheck.m_Attacher.F_TotalHealth);

        if (hitcheck.m_Attacher.B_IsPlayer)
        {
            NavMeshHit edgeHit;
            if (NavMesh.SamplePosition(hitcheck.m_Attacher.transform.position, out edgeHit,5,-1))
                hitcheck.m_Attacher.transform.position = edgeHit.position;
            else
                hitcheck.m_Attacher.transform.position = Vector3.zero;
        }
    }
}
public static class ExcelManager
{
    public static void Init()
    {
        Properties<SLevelGenerate>.Init();
        Properties<SEntity>.Init();
        Properties<SWeapon>.Init();
    }
    public static SLevelGenerate GetLevelGenerateProperties(enum_LevelStyle style,enum_LevelPrefabType prefabType)
    {
        SLevelGenerate generate = Properties<SLevelGenerate>.PropertiesList.Find(p => p.m_LevelStyle == style && p.m_LevelPrefabType == prefabType);
        if (generate.m_LevelStyle == 0 || generate.m_LevelPrefabType == 0||generate.m_ItemGenerate==null)
            Debug.LogError("Error Properties Found Of Index:" + ((int)style*10+ (int)prefabType).ToString());

         return generate;
    }
    public static SEntity GetEntityGenerateProperties(enum_Entity type)
    {
       return Properties<SEntity>.PropertiesList.Find(p => p.m_Type == type);
    }
    public static SWeapon GetWeaponProperties(enum_Weapon type)
    {
        return Properties<SWeapon>.PropertiesList.Find(p => p.m_Weapon == type);
    }
}
public static class ObjectManager
{
    static Transform TF_Entity;
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;

        TCommon.TraversalEnum((enum_Entity type) => 
        {
            ObjectPoolManager<enum_Entity,EntityBase>.Register(type,TResources.Instantiate<EntityBase>("Entity/"+type.ToString()), enum_PoolSaveType.DynamicMaxAmount,1,null);
        });
        
        TCommon.TraversalEnum((enum_SFX type) =>
        {
            ObjectPoolManager<enum_SFX, SFXBase>.Register(type, TResources.Instantiate<SFXBase>("SFX/" + type.ToString()), enum_PoolSaveType.DynamicMaxAmount, 5, null);
        });

        TCommon.TraversalEnum((enum_Interact type) =>
        {
            ObjectPoolManager<enum_Interact, InteractBase>.Register(type, TResources.Instantiate<InteractBase>("Interact/" + type.ToString()), enum_PoolSaveType.DynamicMaxAmount, 1, null);
        });
    }

    static int i_entityIndex = 0;
    public static EntityBase SpawnEntity(enum_Entity type,Transform toTrans=null)
    {
        EntityBase entity= ObjectPoolManager<enum_Entity, EntityBase>.Spawn(type, TF_Entity);
        entity.Init(GameExpression.I_EntityID(i_entityIndex++,type== enum_Entity.Player ), ExcelManager.GetEntityGenerateProperties(type));
        if(toTrans!=null) entity.transform.position = toTrans.position;
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnSpawnEntity, entity);
        return entity;
    }
    public static void RecycleEntity(enum_Entity type, EntityBase target)
    {
        ObjectPoolManager<enum_Entity, EntityBase>.Recycle(type, target);
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnRecycleEntity, target);
    }

    public static WeaponBase SpawnWeapon(enum_Weapon type, EntityPlayerBase toPlayer)
    {
        try
        {
            if (!ObjectPoolManager<enum_Weapon, WeaponBase>.Registed(type))
            {
                WeaponBase preset = TResources.Instantiate<WeaponBase>("Weapon/" + type.ToString());
                ObjectPoolManager<enum_Weapon, WeaponBase>.Register(type, preset, enum_PoolSaveType.DynamicMaxAmount, 1, null);
            }

            return ObjectPoolManager<enum_Weapon, WeaponBase>.Spawn(type, TF_Entity);
        }
        catch       //Error Check
        {
            Debug.LogWarning("Model Null Weapon Model Found:Resources/Weapon/" + type);

            WeaponBase target = TResources.Instantiate<WeaponBase>("Weapon/Error");
            target.Init(ExcelManager.GetWeaponProperties(type));
            return target;
        }
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

    public static T SpawnInteract<T>(enum_Interact type, Vector3 toPos) where T:InteractBase
    {
        InteractBase sfx = ObjectPoolManager<enum_Interact, InteractBase>.Spawn(type, TF_Entity);
        sfx.transform.position = toPos;
        return sfx as T;
    }
    public static void RecycleInteract(enum_Interact type, InteractBase target)
    {
        ObjectPoolManager<enum_Interact, InteractBase>.Recycle(type, target);
    }
    public static void RecycleAllInteract(enum_Interact type)
    {
        ObjectPoolManager<enum_Interact, InteractBase>.RecycleAll(type);
    }
}