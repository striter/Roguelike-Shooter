using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TExcel;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : SingletonMono<GameManager>, ISingleCoroutine
{
    public enum_Style Test_TileStyle = enum_Style.Desert;
    public enum_Style Test_EntityStyle = enum_Style.Invalid;
    public string M_TESTSEED = "";

    public EntityBase m_LocalPlayer { get; private set; } = null;
    public static CPlayerSave m_PlayerInfo { get; private set; }
    public bool B_EditorTestMode { get; private set; } = false;
    public System.Random m_GameSeed { get; private set; } = null;
    public string m_SeedString { get; private set; } = null;
    protected override void Awake()
    {
#if UNITY_EDITOR
        B_EditorTestMode = true;
#endif
        instance = this;
        DataManager.Init();
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
        if (!B_EditorTestMode)
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
            Time.timeScale = Time.timeScale == 1f ? .1f : 1f;

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Z) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Physics.I_Static, ref hit))
            ObjectManager.SpawnEntity(222, hit.point).SetTarget(m_LocalPlayer);
        if (Input.GetKeyDown(KeyCode.X) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Physics.I_Static, ref hit))
            ObjectManager.SpawnSFX<SFXCast>(30003, hit.point, Vector3.forward).Play(1000);
        if (Input.GetKeyDown(KeyCode.C) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Physics.I_Static, ref hit))
            ObjectManager.SpawnSFX<SFXProjectile>(20015, hit.point + Vector3.up , Vector3.forward).Play(1000, Vector3.forward, hit.point+Vector3.forward*10);
        if (Input.GetKeyDown(KeyCode.V) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Physics.I_Static, ref hit))
            ObjectManager.SpawnSFX<SFXIndicator>(50002, hit.point + Vector3.up, Vector3.forward).PlayDuration(1000,hit.point,hit.normal,5);


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
                        if (m_BattleDifficulty < enum_BattleDifficulty.Hard)
                            m_BattleDifficulty++;
                        difficulty = m_BattleDifficulty;
                    }
                    break;
                case enum_TileType.End:
                    battle = true;
                    break;
            }


        if(battle)
            OnBattleStart(DataManager.GetEntityGenerateProperties(1,levelInfo.m_TileType, difficulty) , 3);
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
    public static bool B_CanHitEntity(HitCheckEntity hb, int sourceID)  //If Match Will Hit Target,Player Particles ETC
    {
        if (hb.I_AttacherID == sourceID)
            return false;
        return Instance.m_Entities.ContainsKey(sourceID) && hb.m_Attacher.B_IsPlayer != Instance.m_Entities[sourceID].B_IsPlayer;
    }
    public static bool B_CanDamageEntity(HitCheckEntity hb, int sourceID)   //After Hit,If Match Target Hit Succeed
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
    public enum_Style m_BattleEntityStyle { get; private set; }
    public enum_BattleDifficulty m_BattleDifficulty { get; private set; }
    public bool B_Battling { get; private set; } = false;
    public bool B_WaveEntityGenerating { get; private set; } = false;
    public int m_CurrentWave { get; private set; } = -1;
    public int m_WaveCurrentEntity { get; private set; } = -1;
    public SGenerateEntity m_WaveEnermyGenerate { get; private set; }
    public List<int> m_EntityGenerating { get; private set; } = new List<int>();
    public Dictionary<enum_EntityType, List<int>> m_StyledEnermyEntities;
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
        m_WaveEnermyGenerate.m_EntityGenerate.Traversal((enum_EntityType level, RangeInt range) =>
        {
            int spawnCount = range.Random();
            for (int i = 0; i < spawnCount; i++)
            {
                if (!m_StyledEnermyEntities.ContainsKey(level))
                {
                    Debug.LogWarning("Current Enermy Style:" + m_BattleEntityStyle + " Not Contains Type:" + level);
                    continue;
                }
                m_EntityGenerating.Add(m_StyledEnermyEntities[level].RandomItem());
            }
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

        if (m_WaveCurrentEntity <= 0 || (m_CurrentWave < m_WaveEnermyGenerate.m_WaveCount && m_WaveCurrentEntity <= GameConst.I_EnermyCountWaveFinish))
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
            SpawnEntity(waveGenerate[curSpawnCount], curSpawnCount, EnviormentManager.m_currentLevel.m_Level.RandomEmptyTilePosition(m_GameSeed));
            m_WaveCurrentEntity++;
            curSpawnCount++;
            if (curSpawnCount >= waveGenerate.Count)
            {
                B_WaveEntityGenerating = false;
                yield break;
            }
        }
    }
    void SpawnEntity(int entityIndex, int spawnIndex,Vector3 position)
    {
        ObjectManager.SpawnSFX<SFXIndicator>(50001, position, Vector3.up).Play(entityIndex,GameConst.I_EnermySpawnDelay);
        this.StartSingleCoroutine(100 + spawnIndex, TIEnumerators.PauseDel(GameConst.I_EnermySpawnDelay, () => {
            ObjectManager.SpawnEntity(entityIndex,position ).SetTarget(m_LocalPlayer);
        }));
    }
    #endregion
}
#region External Tools Packaging Class
public static class DataManager
{
    public static void Init()
    {
        Properties<SGenerateItem>.Init();
        Properties<SGenerateEntity>.Init();
        Properties<SWeapon>.Init();
        Properties<SEntity>.Init();
    }
    public static SGenerateItem GetItemGenerateProperties(enum_Style style,enum_TilePrefabDefinition prefabType)
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
    public static SWeapon GetWeaponProperties(enum_PlayerWeapon type)
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
        TResources.GetAllGameSFXs().Traversal((int index,SFXBase target)=>{
            ObjectPoolManager<int, SFXBase>.Register(index, target,
            enum_PoolSaveType.DynamicMaxAmount, 1,
            (SFXBase sfx) => { sfx.Init(index); });
        });

        TCommon.TraversalEnum((enum_Interaction type) =>{
            ObjectPoolManager<enum_Interaction, InteractBase>.Register(type, TResources.Instantiate<InteractBase>("Interact/" + type.ToString()),
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
        entity.OnSpawn(GameManager.I_EntityID(i_entityIndex++,entity.B_IsPlayer));
        entity.transform.position = toPosition;
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnSpawnEntity, entity);
        return entity;
    }
    public static void RecycleEntity(int index, EntityBase target)
    {
        ObjectPoolManager<int, EntityBase>.Recycle(index, target);
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnRecycleEntity, target);
    }

    public static WeaponBase SpawnWeapon(enum_PlayerWeapon type, EntityPlayerBase toPlayer)
    {
        try
        {
            if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(type))
            {
                WeaponBase preset = TResources.Instantiate<WeaponBase>("Weapon/" + type.ToString());
                ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(type, preset, enum_PoolSaveType.DynamicMaxAmount, 1, null);
            }

            return ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(type, TF_Entity);
        }
        catch       //Error Check
        {
            Debug.LogWarning("Model Null Weapon Model Found:Resources/Weapon/" + type);

            WeaponBase target = TResources.Instantiate<WeaponBase>("Weapon/Error");
            target.Init(DataManager.GetWeaponProperties(type));
            return target;
        }
    }
    public static T SpawnSFX<T>(int index, Vector3 position,Vector3 normal,Transform attachTo=null) where T:SFXBase
    {
        T sfx = ObjectPoolManager<int, SFXBase>.Spawn(index, attachTo) as T;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid Type:"+typeof(T).ToString()+"|Index:"+index);
        sfx.transform.position = position;
        sfx.transform.rotation = Quaternion.LookRotation(normal);
        return sfx;
    }
    public static T GetSFX<T>(int index) where T : SFXBase
    {
        T sfx = ObjectPoolManager<int, SFXBase>.GetRegistedSpawnItem(index) as T;
        if (sfx == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + index);
        return sfx;
    }
    public static void RecycleSFX(int index, SFXBase sfx)
    {
        ObjectPoolManager<int, SFXBase>.Recycle(index, sfx);
    }

    public static T SpawnInteract<T>(enum_Interaction type, Vector3 toPos) where T:InteractBase
    {
        InteractBase sfx = ObjectPoolManager<enum_Interaction, InteractBase>.Spawn(type, TF_Entity);
        sfx.transform.position = toPos;
        return sfx as T;
    }
    public static void RecycleInteract(enum_Interaction type, InteractBase target)
    {
        ObjectPoolManager<enum_Interaction, InteractBase>.Recycle(type, target);
    }
    public static void RecycleAllInteract(enum_Interaction type)
    {
        ObjectPoolManager<enum_Interaction, InteractBase>.RecycleAll(type);
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

    public static Dictionary<enum_EntityType, List<int>> RegisterAdditionalEntities(Dictionary<int,EntityBase> registerDic)
    {
        Dictionary<enum_EntityType, List<int>> enermyDic = new Dictionary<enum_EntityType, List<int>>();
        registerDic.Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, enum_PoolSaveType.DynamicMaxAmount, 1, 
                (EntityBase entityInstantiate) => { entityInstantiate.Init(DataManager.GetEntityProperties(index)); });

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