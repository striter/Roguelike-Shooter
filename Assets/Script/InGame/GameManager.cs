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
#if UNITY_EDITOR
    #region Test
    public enum enumDebug_LevelDrawMode
    {
        DrawTypes,
        DrawOccupation,
        DrawItemDirection,
    }
    public bool B_PhysicsDebugGizmos = true;
    public bool B_LevelDebugGizmos = true;
    public enumDebug_LevelDrawMode E_LevelDebug = enumDebug_LevelDrawMode.DrawTypes;
    public int Z_TestEntityIndex = 221;
    public int TestEntityBuffApplyOnSpawn = 1;
    public int X_TestCastIndex = 30003;
    public bool CastForward = true;
    public int C_TestProjectileIndex = 29001;
    public int V_TestIndicatorIndex = 50002;
    public int B_TestBuffIndex = 1;
    public int F5_TestActionIndex = 10001;
    public bool B_AdditionalLight = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1f ? .1f : 1f;

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Z) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
        {
            EntityBase enermy = ObjectManager.SpawnEntity(Z_TestEntityIndex, hit.point);
            if (TestEntityBuffApplyOnSpawn > 0)
                enermy.m_HitCheck.TryHit(new DamageInfo(-1,TestEntityBuffApplyOnSpawn));
        }
        if (Input.GetKeyDown(KeyCode.X) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            ObjectManager.SpawnDamageSource<SFXCast>(X_TestCastIndex, hit.point, CastForward?m_LocalPlayer.transform.forward: Vector3.up).Play(1000, DamageBuffInfo.Default());
        if (Input.GetKeyDown(KeyCode.C) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            ObjectManager.SpawnDamageSource<SFXProjectile>(C_TestProjectileIndex, hit.point + Vector3.up, m_LocalPlayer.transform.forward).Play(0, m_LocalPlayer.transform.forward, hit.point + m_LocalPlayer.transform.forward * 10, DamageBuffInfo.Default());
        if (Input.GetKeyDown(KeyCode.V) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            ObjectManager.SpawnIndicator(V_TestIndicatorIndex, hit.point + Vector3.up, Vector3.up).Play(1000,3f);
        if (Input.GetKeyDown(KeyCode.B))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-1,B_TestBuffIndex));
        if (Input.GetKeyDown(KeyCode.N))
            m_LocalPlayer.BroadcastMessage("OnReceiveDamage", new DamageInfo(-1,20, enum_DamageType.Common,DamageBuffInfo.Default()));
        if (Input.GetKeyDown(KeyCode.M))
            m_LocalPlayer.BroadcastMessage("OnReceiveDamage", new DamageInfo(-1,-50, enum_DamageType.Common, DamageBuffInfo.Default()));
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<EntityBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityBase entity) => {
                if (entity.m_Flag== enum_EntityFlag.Enermy)
                    entity.BroadcastMessage("OnReceiveDamage", new DamageInfo(-1,entity.m_EntityInfo.F_MaxHealth, enum_DamageType.Common, DamageBuffInfo.Default()));
            });
        }
        if (Input.GetKeyDown(KeyCode.Equals))
            OnStageFinished();

        if (Input.GetKeyDown(KeyCode.F5))
            (m_LocalPlayer as EntityPlayerBase).TestUseAction(F5_TestActionIndex);

        UIManager.instance.transform.Find("Test/SeedTest").GetComponent<UnityEngine.UI.Text>().text = m_SeedString;

        if (OptionsManager.B_AdditionalLight != B_AdditionalLight)
        {
            OptionsManager.B_AdditionalLight = B_AdditionalLight;
            OptionsManager.OnOptionChanged();
        }
    }
#endregion
#endif
    public EntityBase m_LocalPlayer { get; private set; } = null;
    public static CPlayerSave m_PlayerInfo { get; private set; }
    public System.Random m_GameSeed { get; private set; } = null;
    public string m_SeedString { get; private set; } = null;
    protected override void Awake()
    {
        instance = this;
        DataManager.Init();
        ObjectManager.Init();
        LevelManager.Init();
        OptionsManager.Init();
        TBroadCaster<enum_BC_GameStatusChanged>.Init();

        m_PlayerInfo = TGameData<CPlayerSave>.Read();

        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnEntitySpawn, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnEntityDead, OnEntityDead);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnEntityRecycle, OnRecycleEntity);
        Application.targetFrameRate = 60;
    }
    private void OnDestroy()
    {
        this.StopAllSingleCoroutines();

        TGameData<CPlayerSave>.Save(m_PlayerInfo);

        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnEntitySpawn, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnEntityDead, OnEntityDead);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnEntityRecycle, OnRecycleEntity);

    }
    private void Start()
    {
        StartStage(Test_TileStyle, Test_EntityStyle,M_TESTSEED);        //Test
    }
    #region Level Management
    //Call When Level Changed
    void StartStage(enum_Style _levelStyle, enum_Style _entityStyle, string _GameSeed = "")      //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        if (_GameSeed == "")
            _GameSeed = DateTime.Now.ToLongTimeString();

        ObjectManager.Preset();
        LevelManager.StageBegin();
        m_SeedString = _GameSeed;
        m_GameSeed = new System.Random(m_SeedString.GetHashCode());
        m_BattleEntityStyle = _entityStyle;
        ObjectManager.RegisterLevelBase(TResources.GetLevelBase(_levelStyle));
        EnviormentManager.Instance.GenerateAllEnviorment(_levelStyle, m_GameSeed, OnLevelStart,OnStageFinished);
        ObjectManager.RegisterEnermyDamageSource(TResources.GetAllDamageSources(m_BattleEntityStyle));
        m_StyledEnermyEntities = ObjectManager.RegisterAdditionalEntities(TResources.GetAllStyledEntities(m_BattleEntityStyle));

        m_LocalPlayer = ObjectManager.SpawnEntity(0, Vector3.zero);
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnStageStart);

        GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    void OnLevelStart(SBigmapLevelInfo levelInfo)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelStart);
        m_LocalPlayer.transform.position = levelInfo.m_Level.RandomEmptyTilePosition(m_GameSeed);

        if (levelInfo.m_TileLocking == enum_TileLocking.Unlockable&&LevelManager.OnLevelBeginBattle(levelInfo.m_TileType))
            OnBattleStart(LevelManager.m_Difficulty);
        else
            OnLevelFinished();
    }

    //Call Enviorment Manager To Generate Portals Or Show Bigmaps, Then Go Back To OnLevelChange From Enviorment Manager
    void OnLevelFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelFinish);
    }

    void OnStageFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnStageFinish);
        if (LevelManager.B_NextStage)
            StartStage(TCommon.RandomEnumValues<enum_Style>(), TCommon.RandomEnumValues<enum_Style>(), "");
        else
            Debug.Log("All Level Finished");
    }
    #endregion
    #region Battle Management
    public static int I_EntityID(int index, enum_EntityFlag flag) => index + (int)flag*10000;       //Used For Identification Management
    public static bool B_CanHitEntity(HitCheckEntity targetHitCheck, int sourceEntityID)  //If Match Will Hit Target,Player Particles ETC
    {
        if (targetHitCheck.I_AttacherID == sourceEntityID)
            return false;
        return !Instance.m_Entities.ContainsKey(sourceEntityID) || targetHitCheck.m_Attacher.m_Flag != Instance.m_Entities[sourceEntityID].m_Flag;
    }
    public static bool B_CanDamageEntity(HitCheckEntity hb, int sourceID)   //After Hit,If Match Target Hit Succeed
    {
        if (hb.I_AttacherID == sourceID)
            return false;

        return Instance.m_Entities.ContainsKey(sourceID) && hb.m_Attacher.m_Flag != Instance.m_Entities[sourceID].m_Flag;
    }

    public static bool B_DoHitCheck(HitCheckBase hitCheck,int sourceID)
    {
        bool canHit = hitCheck.I_AttacherID!=sourceID;
        if (hitCheck.m_HitCheckType == enum_HitCheck.Entity)
            return canHit&&B_CanHitEntity(hitCheck as HitCheckEntity, sourceID);
        return canHit;
    }

    public EntityBase GetRandomEntity(int sourceIndex,enum_EntityFlag targetFlag,Predicate<EntityBase> predict)
    {
        EntityBase target=null;
        m_Entities.TraversalRandom((int index, EntityBase entity) =>
        {
            if (entity.m_Flag == targetFlag && entity.I_EntityID!=sourceIndex&& (predict == null || predict(entity)))
            {
                target = entity;
                return true;
            }
            return false;
        });
        return target;
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
    #endregion
    #region Battle Management
    public enum_Style m_BattleEntityStyle { get; private set; }
    public bool B_Battling { get; private set; } = false;
    public bool B_WaveEntityGenerating { get; private set; } = false;
    public int m_CurrentWave { get; private set; } = -1;
    public int m_WaveCurrentEntity { get; private set; } = -1;
    public List<SGenerateEntity> m_EntityGenerate { get; private set; } = new List<SGenerateEntity>();
    public List<int> m_EntityGenerating { get; private set; } = new List<int>();
    public Dictionary<enum_EntityType, List<int>> m_StyledEnermyEntities;
    void OnBattleStart(enum_BattleDifficulty difficulty)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnBattleStart);
        m_EntityGenerate = DataManager.GetEntityGenerateProperties(difficulty);
        B_Battling = true;
        m_WaveCurrentEntity = 0;
        m_CurrentWave = 0;
        WaveStart();
    }

    void WaveStart()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnWaveStart);
        m_EntityGenerating.Clear();
        m_EntityGenerate[m_CurrentWave].m_EntityGenerate.Traversal((enum_EntityType level, RangeInt range) =>
        {
            int spawnCount = range.RandomRangeInt();
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
        if (m_CurrentWave >= m_EntityGenerate.Count)
            OnBattleFinished();
        else
            WaveStart();
    }
    void OnEntityDead(EntityBase entity)
    {
        if (!B_Battling)
            return;
        if (entity.m_Flag== enum_EntityFlag.Enermy)
            m_WaveCurrentEntity--;

        if (B_WaveEntityGenerating)
            return;

        if (m_WaveCurrentEntity <= 0 || (m_CurrentWave < m_EntityGenerate.Count && m_WaveCurrentEntity <= GameConst.I_EnermyCountWaveFinish))
            WaveFinished();
    }
    void OnBattleFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnBattleFinish);
        m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-1,m_LocalPlayer.m_HealthManager.m_CurrentArmor-m_LocalPlayer.m_HealthManager.m_DefaultArmor ,enum_DamageType.ArmorOnly, DamageBuffInfo.Default()));
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
            SpawnEnermy(waveGenerate[curSpawnCount], curSpawnCount, EnviormentManager.m_currentLevel.m_Level.RandomEmptyTilePosition(m_GameSeed));
            m_WaveCurrentEntity++;
            curSpawnCount++;
            if (curSpawnCount >= waveGenerate.Count)
            {
                B_WaveEntityGenerating = false;
                yield break;
            }
        }
    }
    void SpawnEnermy(int entityIndex, int spawnIndex,Vector3 position)
    {
        ObjectManager.SpawnIndicator(30001, position, Vector3.up).Play(entityIndex,GameConst.I_EnermySpawnDelay);
        this.StartSingleCoroutine(100 + spawnIndex, TIEnumerators.PauseDel(GameConst.I_EnermySpawnDelay, () => {
            ObjectManager.SpawnEntity(entityIndex,position );
        }));
    }
    #endregion
}
#region External Tools Packaging Class
public static class LevelManager
{
    public static int I_currentStageIndex;
    public static bool B_NextStage => I_currentStageIndex < GameConst.I_TotalStageCount;
    static enum_BattleDifficulty m_BattleDifficulty;
    public static enum_TileType m_LevelType { get; private set; }
    public static void Init()
    {
        I_currentStageIndex = 0;
    }
    public static void StageBegin()
    {
        m_BattleDifficulty = enum_BattleDifficulty.Peaceful;
        m_LevelType = enum_TileType.Invalid;
        I_currentStageIndex++;
    }
    public static bool OnLevelBeginBattle(enum_TileType type)
    {
        m_LevelType = type;
        switch (type)
        {
            default:
                return false;
            case enum_TileType.Battle:
                if (m_BattleDifficulty < enum_BattleDifficulty.Hard)
                    m_BattleDifficulty++;
                return true;
            case enum_TileType.End:
                return true;
        }
    }
    public static enum_BattleDifficulty m_Difficulty
    {
        get {
            switch (m_LevelType)
            {
                default:
                    return enum_BattleDifficulty.Peaceful;
                case enum_TileType.End:
                    return enum_BattleDifficulty.Final;
                case enum_TileType.Battle:
                    return m_BattleDifficulty;
            }
        }
    }
}
public static class DataManager
{
    public static void Init()
    {
        Properties<SLevelGenerate>.Init();
        Properties<SGenerateEntity>.Init();
        Properties<SWeapon>.Init();
        Properties<SBuff>.Init();
        InitActions();
    }

    public static SLevelGenerate GetItemGenerateProperties(enum_Style style,enum_LevelGenerateType prefabType,bool isInner)
    {
        SLevelGenerate generate = Properties<SLevelGenerate>.PropertiesList.Find(p => p.m_LevelStyle == style && p.m_LevelPrefabType == prefabType&&p.m_IsInner==isInner);
        if (generate.m_LevelStyle == 0 || generate.m_LevelPrefabType == 0||generate.m_ItemGenerate==null)
            Debug.LogError("Error Properties Found Of Index:" + ((int)style*100+ (int)prefabType*10+(isInner?0:1)).ToString());

         return generate;
    }

    public static List<SGenerateEntity> GetEntityGenerateProperties(enum_BattleDifficulty battleDifficulty)
    {
        List<SGenerateEntity> entityList = new List<SGenerateEntity>();
        int waveCount = 1;
        for (int i = 0; i < 10; i++)
        {
            List<SGenerateEntity> randomItems = Properties<SGenerateEntity>.PropertiesList.FindAll(p => p.m_Difficulty == battleDifficulty&&p.m_waveCount==waveCount);
            if (randomItems == null || randomItems.Count == 0)
                break;
            entityList.Add(randomItems.RandomItem());
            waveCount++;
        }
        return entityList;
    }

    public static SWeapon GetWeaponProperties(enum_PlayerWeapon type)
    {
        SWeapon weapon= Properties<SWeapon>.PropertiesList.Find(p => p.m_Weapon == type);
        if(weapon.m_Weapon==0)
            Debug.LogError("Error Properties Found Of Index:" +type.ToString()+"|"+((int)type));
        return weapon;
    }
    public static SBuff GetEntityBuffProperties(int index)
    {
        SBuff buff = Properties<SBuff>.PropertiesList.Find(p => p.m_Index == index);
        if (buff.m_Index == 0)
            Debug.LogError("Error Properties Found Of Index:" + index);
        return buff;
    }

    static Dictionary<int,Type> m_AllActions=new Dictionary<int, Type>();
    static void InitActions()=> TReflection.GetAllInheritClasses((Type type,ActionBase action)=> { m_AllActions.Add(action.m_Index, type); }, enum_ActionLevel.Invalid,null);
    public static ActionBase GetAction(int index,enum_ActionLevel level,Action<ExpireBase> OnExpired)
    {
        if (!m_AllActions.ContainsKey(index))
            Debug.LogError("Error Action:" + index + " ,Does not exist");

        return TReflection.CreateInstance<ActionBase>(m_AllActions[index],level,OnExpired);
    }
}
public static class ObjectManager
{
    public static Transform TF_Entity;
    public static Transform TF_SFXWaitForRecycle;
    public static Transform TF_SFXPlaying;
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;
        TF_SFXWaitForRecycle = new GameObject("SFX_WaitForRecycle").transform;
        TF_SFXPlaying = new GameObject("SFX_Playing").transform;
    }
    #region Register
    public static void Preset()
    {
        ObjectPoolManager<int, SFXBase>.ClearAll();
        ObjectPoolManager<int, EntityBase>.ClearAll();
        ObjectPoolManager<enum_Interaction, InteractBase>.ClearAll();
        ObjectPoolManager<LevelItemBase, LevelItemBase>.ClearAll();
        ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.ClearAll();
        ObjectPoolManager<int, LevelBase>.ClearAll();

        TResources.GetAllCommonSFXs().Traversal((int index, SFXBase target) => {
            ObjectPoolManager<int, SFXBase>.Register(index, target,
            enum_PoolSaveType.DynamicMaxAmount, 1,
            (SFXBase sfx) => { sfx.Init(index); });
        });

        TCommon.TraversalEnum((enum_Interaction type) => {
            ObjectPoolManager<enum_Interaction, InteractBase>.Register(type, TResources.Instantiate<InteractBase>("Interact/" + type.ToString()),
            enum_PoolSaveType.DynamicMaxAmount, 1, null);
        });
    }
    public static void RegisterLevelBase(LevelBase levelprefab)
    {
        ObjectPoolManager<int, LevelBase>.Register(0,levelprefab, enum_PoolSaveType.DynamicMaxAmount,1,(LevelBase level)=>{ level.Init(); });
    }
    public static void RegisterLevelItem(Dictionary<LevelItemBase, int> registerDic)
    {
        registerDic.Traversal((LevelItemBase item, int count) => { ObjectPoolManager<LevelItemBase, LevelItemBase>.Register(item, item, enum_PoolSaveType.StaticMaxAmount, count, null); });
    }
    public static Dictionary<enum_EntityType, List<int>> RegisterAdditionalEntities(Dictionary<int, EntityBase> registerDic)
    {
        Dictionary<enum_EntityType, List<int>> enermyDic = new Dictionary<enum_EntityType, List<int>>();
        registerDic.Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, enum_PoolSaveType.DynamicMaxAmount, 1,
                (EntityBase entityInstantiate) => { entityInstantiate.Init(index); });

            if (entity.m_Flag== enum_EntityFlag.Enermy)
            {
                EntityEnermyBase enermy = entity as EntityEnermyBase;
                if (!enermyDic.ContainsKey(enermy.E_EnermyType))
                    enermyDic.Add(enermy.E_EnermyType, new List<int>());
                enermyDic[enermy.E_EnermyType].Add(index);
            }
        });

        return enermyDic;
    }
    #endregion
    #region Spawn/Recycle
    #region Entity
    static int i_entityIndex = 0;
    public static EntityBase SpawnEntity(int index,Vector3 toPosition)
    {
        EntityBase entity= ObjectPoolManager<int, EntityBase>.Spawn(index, TF_Entity);
        toPosition = EnviormentManager.NavMeshPosition(toPosition);
        entity.transform.position = toPosition;
        entity.OnSpawn(GameManager.I_EntityID(i_entityIndex++, entity.m_Flag));
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnEntitySpawn, entity);
        return entity;
    }

    public static void RecycleEntity(int index, EntityBase target)=>ObjectPoolManager<int, EntityBase>.Recycle(index, target);
    #endregion
    #region Weapon
    public static WeaponBase SpawnWeapon(enum_PlayerWeapon type, EntityPlayerBase toPlayer)
    {
        if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(type))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(type);
            ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(type, preset, enum_PoolSaveType.DynamicMaxAmount, 1, (WeaponBase weapon) => { weapon.Init(DataManager.GetWeaponProperties(type)); });
        }

        return ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(type, TF_Entity);
    }
    public static void RecycleWeapon(enum_PlayerWeapon type, WeaponBase weapon)=>ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Recycle(type,weapon);
    #endregion
    #region SFX
    public static T SpawnSFX<T>(int index,Transform attachTo=null) where T:SFXBase
    {
        T sfx = ObjectPoolManager<int, SFXBase>.Spawn(index, attachTo?attachTo: TF_SFXPlaying) as T;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid SFX Type:" + typeof(T) + ",Index:" + index);
        return sfx;
    }

    public static T SpawnParticles<T>(int index, Vector3 position, Vector3 normal, Transform attachTo = null) where T:SFXParticles
    {
        T sfx = SpawnSFX<T>(index,attachTo);
        sfx.transform.position = position;
        sfx.transform.rotation = Quaternion.LookRotation(normal);
        return sfx;
    }
    public static SFXIndicator SpawnIndicator(int index, Vector3 position, Vector3 normal, Transform attachTo = null)=> SpawnParticles<SFXIndicator>(index, position, normal, attachTo);
    public static SFXBuffEffect SpawnBuffEffect(int index, EntityBase attachTo) => SpawnParticles<SFXBuffEffect>(index, attachTo.transform.position, attachTo.transform.forward, attachTo.transform);

    public static void RegisterEnermyDamageSource(Dictionary<int, SFXBase> damageSources)
    {
        damageSources.Traversal((int weaponIndex, SFXBase damageSFX) => {
            ObjectPoolManager<int, SFXBase>.Register(weaponIndex, damageSFX, enum_PoolSaveType.DynamicMaxAmount, 1, (SFXBase sfx) => {
                sfx.Init(weaponIndex); });
            }) ;
    }
    public static T SpawnDamageSource<T>(int weaponIndex,Vector3 position,Vector3 normal, Transform attachTo=null) where T:SFXBase
    {
        T template = ObjectPoolManager<int, SFXBase>.Spawn(weaponIndex, attachTo) as T;
        if (template == null)
            Debug.LogError("Enermy Weapon Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        template.transform.position = position;
        template.transform.rotation = Quaternion.LookRotation(normal);
        return template;
    }
    public static T GetDamageSource<T>(int weaponIndex) where T : SFXBase
    {
        T damageSourceInfo = ObjectPoolManager<int, SFXBase>.GetRegistedSpawnItem(weaponIndex) as T;
        if (damageSourceInfo == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return damageSourceInfo;
    }
    public static void RecycleSFX(int index, SFXBase sfx)=> ObjectPoolManager<int, SFXBase>.Recycle(index, sfx);
    #endregion
    public static T SpawnInteract<T>(enum_Interaction type, Vector3 toPos) where T:InteractBase
    {
        InteractBase sfx = ObjectPoolManager<enum_Interaction, InteractBase>.Spawn(type, TF_Entity);
        sfx.transform.position = toPos;
        return sfx as T;
    }
    public static void RecycleInteract(enum_Interaction type, InteractBase target)=> ObjectPoolManager<enum_Interaction, InteractBase>.Recycle(type, target);
    #region Level/LevelItem
    public static LevelBase SpawnLevelPrefab(Transform toTrans)
    {
       return ObjectPoolManager<int, LevelBase>.Spawn(0,toTrans);
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
    #endregion
    #endregion
}
public static class OptionsManager
{
    public static bool B_AdditionalLight = false;
    public static event Action event_OptionChanged;
    public static void Init()
    {
    }
    
    public static void OnOptionChanged()
    {
        event_OptionChanged?.Invoke();
    }
}
#endregion