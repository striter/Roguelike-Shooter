﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TExcel;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : SingletonMono<GameManager>, ISingleCoroutine
{
    public enum_TileStyle Test_TileStyle = enum_TileStyle.Desert;
    public enum_EntityStyle Test_EntityStyle = enum_EntityStyle.Test;
    public string M_TESTSEED = "";

    public EntityBase m_LocalPlayer { get; private set; } = null;
    public static CPlayerSave m_PlayerInfo { get; private set; }
    public bool B_TestMode { get; private set; } = false;
    public System.Random m_GameSeed { get; private set; } = null;
    public string m_SeedString { get; private set; } = null;
    protected override void Awake()
    {
#if UNITY_EDITOR
        B_TestMode = true;
#endif
        instance = this;
        ExcelManager.Init();
        ObjectManager.Init();
        TBroadCaster<enum_BC_UIStatusChanged>.Init();
        TBroadCaster<enum_BC_GameStatusChanged>.Init();

        m_PlayerInfo = TGameData<CPlayerSave>.Read();

        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnSpawnEntity, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnRecycleEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnWaveEntityDead);
    }
    void Update()
    {
        if (!B_TestMode)
            return;
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<EntityBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityBase entity) => {
                if (!entity.B_IsPlayer)
                    entity.BroadcastMessage("TryTakeDamage", entity.m_EntityInfo.m_MaxHealth + entity.m_EntityInfo.m_MaxArmor);
            });
        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Time.timeScale = Time.timeScale == 1f ? .1f : 1f;
        }
        UIManager.instance.transform.Find("SeedTest").GetComponent<UnityEngine.UI.Text>().text = m_SeedString;
    }
    private void OnDestroy()
    {
        this.StopAllSingleCoroutines();

        TGameData<CPlayerSave>.Save(m_PlayerInfo);

        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnSpawnEntity, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnRecycleEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnWaveEntityDead);

    }

    private void Start()        //Entrance Of Whole Game
    {
        PreInit(M_TESTSEED);
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnStageStart);
    }
    #region Bigmap/Level Management
    void PreInit(string _GameSeed="")      //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        if (_GameSeed == "")
            _GameSeed = DateTime.Now.ToLongTimeString();

        m_SeedString = _GameSeed;
        m_GameSeed = new System.Random(m_SeedString.GetHashCode());
        m_BattleDifficulty = enum_BattleDifficulty.Default;
        m_BattleEntityStyle = Test_EntityStyle;

        EnviormentManager.Instance.GenerateAllEnviorment(Test_TileStyle, m_GameSeed,OnLevelStart);
        m_StyledEnermyEntities=ObjectManager.RegisterAdditionalEntities(TResources.GetAllStyledEntities(m_BattleEntityStyle));
        GC.Collect();

        m_LocalPlayer = ObjectManager.SpawnEntity(0,Vector3.zero);
    }

    //Call When Level Changed
    void OnLevelStart(SBigmapLevelInfo levelInfo)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelStart);
        m_LocalPlayer.transform.position = levelInfo.m_Level.RandomEmptyTilePosition(m_GameSeed);
        bool battle = false;
        enum_BattleDifficulty difficulty= enum_BattleDifficulty.Default;
        if (levelInfo.m_TileLocking == enum_TileLocking.Unlockable)
            switch (levelInfo.m_Level.m_levelType)
            {
                case enum_TileType.Battle:        //Generate All Enermy To Be Continued
                    {
                        battle = true;
                        difficulty = m_BattleDifficulty;
                        if (m_BattleDifficulty < enum_BattleDifficulty.Hard)
                            m_BattleDifficulty++;
                    }
                    break;
                case enum_TileType.End:
                    battle = true;
                    break;
            }


        if(battle)
            OnBattleStart(ExcelManager.GetEntityGenerateProperties(1,levelInfo.m_TileType, difficulty) , 3);
        else
            OnLevelFinished();
    }
    //Call Enviorment Manager To Generate Portals Or Show Bigmaps, Then Go Back To OnLevelChange From Enviorment Manager
    void OnLevelFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelFinish);
    }
    #endregion
    #region Entity Management
    public static int I_EntityID(int index, bool isPlayer) => index + (isPlayer ? 10000 : 20000);       //Used For Identification Management
    public static bool B_CanHitTarget(HitCheckEntity hb, int sourceID)   //If Match Target Hit Succeed
    {
        if (hb.I_AttacherID == sourceID)
            return false;

        return Instance.m_Entities.ContainsKey(sourceID) && hb.m_Attacher.B_IsPlayer != Instance.m_Entities[sourceID].B_IsPlayer;
    }
    public Dictionary<int, EntityBase> m_Entities { get; private set; } = new Dictionary<int, EntityBase>();
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
            if (NavMesh.SamplePosition(hitcheck.m_Attacher.transform.position, out edgeHit, 5, -1))
                hitcheck.m_Attacher.transform.position = edgeHit.position;
            else
                hitcheck.m_Attacher.transform.position = Vector3.zero;
        }
    }
    #endregion
    #region Battle Management
    public enum_EntityStyle m_BattleEntityStyle { get; private set; }
    public enum_BattleDifficulty m_BattleDifficulty { get; private set; }
    public bool B_Battling { get; private set; } = false;
    public bool B_WaveEntityGenerating { get; private set; } = false;
    public int m_CurrentWave { get; private set; } = -1;
    public int m_WaveCurrentEntity { get; private set; } = -1;
    public SGenerateEntity m_WaveEnermyGenerate { get; private set; }
    public List<int> m_EntityGenerating { get; private set; } = new List<int>();
    public Dictionary<enum_EntityLevel, List<int>> m_StyledEnermyEntities;
    void OnBattleStart(SGenerateEntity enermyType,int _waveCount)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnBattleStart);
        B_Battling = true;
        m_WaveCurrentEntity = 0;
        m_CurrentWave = 1;
        m_WaveEnermyGenerate = enermyType;
        WaveStart();
    }

    void WaveStart()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnWaveStart);
        m_EntityGenerating.Clear();
        m_WaveEnermyGenerate.m_EntityGenerate.Traversal((enum_EntityLevel level, RangeInt range) =>
        {
            int spawnCount = range.Random();
            for (int i = 0; i < spawnCount; i++)
                m_EntityGenerating.Add(m_StyledEnermyEntities[level].RandomItem());
        });
        this.StartSingleCoroutine(0, IE_GenerateEnermy(m_EntityGenerating, .1f));
    }
    void WaveFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnWaveFinish);
        m_CurrentWave++;
        if (m_CurrentWave > m_WaveEnermyGenerate.m_WaveCount)
            OnBattleFinished();
        else
            WaveStart();
    }
    void OnWaveEntityDead(EntityBase entity)
    {
        if (!B_Battling)
            return;
        if (!entity.B_IsPlayer)
            m_WaveCurrentEntity--;

        if (B_WaveEntityGenerating)
            return;

        if (m_WaveCurrentEntity <= 0 || (m_CurrentWave < m_WaveEnermyGenerate.m_WaveCount && m_WaveCurrentEntity <= 2))
            WaveFinished();
    }
    void OnBattleFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnBattleFinish);
        B_Battling = false;
        OnLevelFinished();
    }

    IEnumerator IE_GenerateEnermy(List<int> waveGenerate, float _offset)
    {
        B_WaveEntityGenerating = true;
        int curSpawnCount = 0;
        for (; ; )
        {
            yield return new WaitForSeconds(_offset);
            ObjectManager.SpawnEntity(waveGenerate[curSpawnCount], EnviormentManager.m_currentLevel.m_Level.RandomEmptyTilePosition(m_GameSeed)).SetTarget(m_LocalPlayer);

            m_WaveCurrentEntity++;
            curSpawnCount++;
            if (curSpawnCount >= waveGenerate.Count)
            {
                B_WaveEntityGenerating = false;
                yield break;
            }
        }
    }
    #endregion
}
#region External Tools Packaging Class
public static class ExcelManager
{
    public static void Init()
    {
        Properties<SEntity>.Init();
        Properties<SWeapon>.Init();
        Properties<SBarrage>.Init();
        Properties<SGenerateItem>.Init();
        Properties<SGenerateEntity>.Init();
    }
    public static SGenerateItem GetItemGenerateProperties(enum_TileStyle style,enum_TilePrefabDefinition prefabType)
    {
        SGenerateItem generate = Properties<SGenerateItem>.PropertiesList.Find(p => p.m_LevelStyle == style && p.m_LevelPrefabType == prefabType);
        if (generate.m_LevelStyle == 0 || generate.m_LevelPrefabType == 0||generate.m_ItemGenerate==null)
            Debug.LogError("Error Properties Found Of Index:" + ((int)style*10+ (int)prefabType).ToString());

         return generate;
    }
    public static SGenerateEntity GetEntityGenerateProperties(int stageIndex,enum_TileType type,enum_BattleDifficulty battleDifficulty)
    {
        SGenerateEntity generate = Properties<SGenerateEntity>.PropertiesList.Find(p => p.m_stageIndex == stageIndex && p.m_TileType == type&&p.m_Difficulty==battleDifficulty);
        if (generate.m_stageIndex == 0 || generate.m_TileType == 0 || generate.m_Difficulty == 0)
            Debug.LogError("Error Properties Found Of Index:" + (stageIndex*100+ (int)type * 10 + (int)battleDifficulty).ToString());

        return generate;
    }
    public static SEntity GetEntityProperties(int index)
    {
        SEntity entity= Properties<SEntity>.PropertiesList.Find(p => p.m_Index == index);
        if (entity.m_MaxHealth == 0)
            Debug.LogError("Error Properties Found Of Name Index:" + index);
        return entity;
    }
    public static SWeapon GetWeaponProperties(enum_Weapon type)
    {
        SWeapon weapon= Properties<SWeapon>.PropertiesList.Find(p => p.m_Weapon == type);
        if(weapon.m_Weapon==0)
            Debug.LogError("Error Properties Found Of Index:" +type.ToString()+"|"+((int)type));
        return weapon;
    }
    public static SBarrage GetBarrageProperties(int barrageIndex)
    {
        SBarrage barrage = Properties<SBarrage>.PropertiesList.Find(p => p.m_Index == barrageIndex);
        if (barrageIndex == 0)
            Debug.LogError("Error Properties Found Of Index:" + barrageIndex.ToString() + "|" + barrageIndex);
        return barrage;
    }
}
public static class ObjectManager
{
    public static Transform TF_Entity;
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;
        TCommon.TraversalEnum((enum_SFX type) =>{
            ObjectPoolManager<enum_SFX, SFXBase>.Register(type, TResources.Instantiate<SFXBase>("SFX/" + type.ToString()),
            enum_PoolSaveType.DynamicMaxAmount, 1,
            (SFXBase sfx)=> {sfx.Init(type);});
        });

        TCommon.TraversalEnum((enum_Interact type) =>{
            ObjectPoolManager<enum_Interact, InteractBase>.Register(type, TResources.Instantiate<InteractBase>("Interact/" + type.ToString()),
            enum_PoolSaveType.DynamicMaxAmount, 1, null);
        });
    }

    static int i_entityIndex = 0;
    public static EntityBase SpawnEntity(int index,Vector3 toPosition)
    {
        EntityBase entity= ObjectPoolManager<int, EntityBase>.Spawn(index, TF_Entity);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(toPosition, out hit, 5, -1))
            toPosition = hit.position;
        entity.OnActivate(GameManager.I_EntityID(i_entityIndex++,entity.B_IsPlayer));
        entity.transform.position = toPosition;
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnSpawnEntity, entity);
        return entity;
    }
    public static void RecycleEntity(int index, EntityBase target)
    {
        ObjectPoolManager<int, EntityBase>.Recycle(index, target);
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

    public static void RegisterLevelItem(Dictionary<LevelItemBase,int> registerDic)
    {
        registerDic.Traversal((LevelItemBase item,int count)=> { ObjectPoolManager<LevelItemBase, LevelItemBase>.Register(item, item, enum_PoolSaveType.StaticMaxAmount, count,null); });
    }
    public static LevelItemBase SpawnLevelItem(LevelItemBase itemObject,Transform itemParent,Vector3 localPosition)
    {
        LevelItemBase spawnedItem = ObjectPoolManager<LevelItemBase, LevelItemBase>.Spawn(itemObject, itemParent);
        spawnedItem.transform.localPosition = localPosition;
        return spawnedItem;
    }
    public static void RecycleAllLevelItem()
    {
        ObjectPoolManager<LevelItemBase, LevelItemBase>.RecycleAllManagedItems();
    }

    public static Dictionary<enum_EntityLevel, List<int>> RegisterAdditionalEntities(Dictionary<int,EntityBase> registerDic)
    {
        Dictionary<enum_EntityLevel, List<int>> enermyDic = new Dictionary<enum_EntityLevel, List<int>>();
        registerDic.Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, enum_PoolSaveType.DynamicMaxAmount, 1, 
                (EntityBase entityInstantiate) => { entityInstantiate.Init(ExcelManager.GetEntityProperties(index)); });

            if (!entity.B_IsPlayer)
            {
                if (!enermyDic.ContainsKey(entity.m_EntityInfo.m_Type))
                    enermyDic.Add(entity.m_EntityInfo.m_Type, new List<int>());
                enermyDic[entity.m_EntityInfo.m_Type].Add(index);
            }

        });
        return enermyDic;
    }
}
#endregion