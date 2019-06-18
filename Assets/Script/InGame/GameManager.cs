using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TExcel;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : SingletonMono<GameManager>,ISingleCoroutine
{
    public EntityBase m_LocalPlayer { get; private set; } = null;
    public static CPlayerSave m_PlayerInfo { get; private set; }
    public bool B_TestMode { get; private set; } = false;
    public enum_LevelStyle E_TESTSTYLE = enum_LevelStyle.Desert;
    public System.Random m_GameSeed { get; private set; } = null;
    public string m_SeedString { get; private set; } = null;
    public string M_TESTSEED = "";
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
        //        PostEffectManager.SetPostEffect<PE_BloomSpecific>(); ?
        PostEffectManager.SetPostEffect<PE_FogDepthNoise>();
        PostEffectManager.instance.peb_curEffect.mat_Cur.SetTexture("_NoiseTex", TResources.Load<Texture>("Texture/Noise1"));
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
        EnviormentManager.Instance.GenerateAllEnviorment(E_TESTSTYLE, m_GameSeed,OnLevelStart);
        GC.Collect();
        m_LocalPlayer = ObjectManager.SpawnEntity(enum_Entity.Player,Vector3.zero);
    }

    //Call When Level Changed
    void OnLevelStart(SBigmapLevelInfo levelInfo)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelStart);
        m_LocalPlayer.transform.position = levelInfo.m_Level.RandomEmptyTilePosition(m_GameSeed);
        bool battle = false;
        if (levelInfo.m_TileLocking == enum_LevelLocking.Unlockable)
            switch (levelInfo.m_Level.m_levelType)
            {
                case enum_LevelType.Battle:        //Generate All Enermy To Be Continued
                case enum_LevelType.End:
                    battle = true;
                    break;
            }

        if(battle)
            OnBattleStart(enum_Entity.EnermyAITest, 3, 8);
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
    public bool B_Battling { get; private set; } = false;
    public bool B_WaveEntityGenerating { get; private set; } = false;
    public int m_WaveCount { get; private set; } = -1;
    public int m_CurrentWave { get; private set; } = -1;
    public int m_WaveEntityEach { get; private set; } = -1;
    public int m_WaveCurrentEntity { get; private set; } = -1;
    public enum_Entity m_WaveEntityType { get; private set; } = enum_Entity.Invalid;
    void OnBattleStart(enum_Entity enermyType,int _waveCount,int _waveSpawnCount)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnBattleStart);
        B_Battling = true;
        m_WaveCount = _waveCount;
        m_WaveEntityEach = _waveSpawnCount;
        m_WaveCurrentEntity = 0;
        m_CurrentWave = 1;
        m_WaveEntityType = enermyType;
        WaveStart();
    }

    void WaveStart()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnWaveStart);
        this.StartSingleCoroutine(0, IE_GenerateEnermy(m_WaveEntityType,m_WaveEntityEach,.1f));
    }
    void WaveFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnWaveFinish);
        m_CurrentWave++;
        if (m_CurrentWave > m_WaveCount)
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

        if (m_WaveCurrentEntity <= 0 || (m_CurrentWave < m_WaveCount && m_WaveCurrentEntity <= 2))
            WaveFinished();
    }
    void OnBattleFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnBattleFinish);
        B_Battling = false;
        OnLevelFinished();
    }

    IEnumerator IE_GenerateEnermy(enum_Entity type,int totalCount, float _offset)
    {
        B_WaveEntityGenerating = true;
        int curSpawnCount = 0;
        for (; ; )
        {
            yield return new WaitForSeconds(_offset);
            ObjectManager.SpawnEntity(type, EnviormentManager.m_currentLevel.m_Level.RandomEmptyTilePosition(m_GameSeed)).SetTarget(m_LocalPlayer);

            m_WaveCurrentEntity++;
            curSpawnCount++;
            if (curSpawnCount >= totalCount)
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
        SEntity entity= Properties<SEntity>.PropertiesList.Find(p => p.m_Type == type);
        if (entity.m_Type == 0)
            Debug.LogError("Error Properties Found Of Index:" + type.ToString() + "|" + ((int)type));
        return entity;
    }
    public static SWeapon GetWeaponProperties(enum_Weapon type)
    {
        SWeapon weapon= Properties<SWeapon>.PropertiesList.Find(p => p.m_Weapon == type);
        if(weapon.m_Weapon==0)
            Debug.LogError("Error Properties Found Of Index:" +type.ToString()+"|"+((int)type));
        return weapon;
    }
}
public static class ObjectManager
{
    public static Transform TF_Entity;
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;
        TCommon.TraversalEnum((enum_Entity type) => 
        {
            ObjectPoolManager<enum_Entity,EntityBase>.Register(type,TResources.Instantiate<EntityBase>("Entity/"+type.ToString()), enum_PoolSaveType.DynamicMaxAmount,1,null);
        });
        
        TCommon.TraversalEnum((enum_SFX type) =>{
            ObjectPoolManager<enum_SFX, SFXBase>.Register(type, TResources.Instantiate<SFXBase>("SFX/" + type.ToString()),
            enum_PoolSaveType.DynamicMaxAmount, 5,
            (SFXBase sfx)=> {sfx.Init(type);});
        });

        TCommon.TraversalEnum((enum_Interact type) =>{
            ObjectPoolManager<enum_Interact, InteractBase>.Register(type, TResources.Instantiate<InteractBase>("Interact/" + type.ToString()),
            enum_PoolSaveType.DynamicMaxAmount, 1, null);
        });
    }

    static int i_entityIndex = 0;
    public static EntityBase SpawnEntity(enum_Entity type,Vector3 toPosition)
    {
        EntityBase entity= ObjectPoolManager<enum_Entity, EntityBase>.Spawn(type, TF_Entity);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(toPosition, out hit, 5, -1))
            toPosition = hit.position;
        entity.Init(GameManager.I_EntityID(i_entityIndex++,type== enum_Entity.Player ), ExcelManager.GetEntityGenerateProperties(type));
        entity.Activate();
        entity.transform.position = toPosition;
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
}
#endregion