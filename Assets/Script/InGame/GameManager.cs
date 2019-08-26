using GameSetting;
using GameSetting_Action;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TExcel;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>, ISingleCoroutine
{
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
    public int Z_TestEntitySpawn = 221;
    public enum_EntityFlag TestEntityFlag = enum_EntityFlag.Enermy;
    public int TestEntityBuffOnSpawn = 1;
    public int X_TestCastIndex = 30003;
    public bool CastForward = true;
    public int C_TestProjectileIndex = 29001;
    public int V_TestIndicatorIndex = 50002;
    public int B_TestBuffIndex = 1;
    public int F5_TestActionIndex = 10001;
    public int F6_TestActionIndex = 10001;
    public int F7_TestActionIndex = 10001;
    public bool B_AdditionalLight = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1f ? .1f : 1f;

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Z) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
        {
            EntityBase enermy = ObjectManager.SpawnEntity(Z_TestEntitySpawn, hit.point, TestEntityFlag);
            if (TestEntityBuffOnSpawn > 0)
                enermy.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Common,DamageDeliverInfo.BuffInfo(-1, TestEntityBuffOnSpawn)));
        }
        if (Input.GetKeyDown(KeyCode.X) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            ObjectManager.SpawnEquipment<SFXCast>(X_TestCastIndex, hit.point, CastForward?m_LocalPlayer.transform.forward: Vector3.up).Play(DamageDeliverInfo.Default(-1));
        if (Input.GetKeyDown(KeyCode.C) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            ObjectManager.SpawnEquipment<SFXProjectile>(C_TestProjectileIndex, hit.point + Vector3.up, m_LocalPlayer.transform.forward).Play(DamageDeliverInfo.Default(-1), m_LocalPlayer.transform.forward, hit.point + m_LocalPlayer.transform.forward * 10);
        if (Input.GetKeyDown(KeyCode.V) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            ObjectManager.SpawnIndicator(V_TestIndicatorIndex, hit.point + Vector3.up, Vector3.up).Play(1000,3f);
        if (Input.GetKeyDown(KeyCode.B))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Common, DamageDeliverInfo.BuffInfo(-1, B_TestBuffIndex )));
        if (Input.GetKeyDown(KeyCode.N))
            m_LocalPlayer.BroadcastMessage("OnReceiveDamage", new DamageInfo(20, enum_DamageType.Common,DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.M))
            m_LocalPlayer.BroadcastMessage("OnReceiveDamage", new DamageInfo(-50, enum_DamageType.Common, DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<EntityBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityBase entity) => {
                if (entity.m_Flag== enum_EntityFlag.Enermy)
                    entity.BroadcastMessage("OnReceiveDamage", new DamageInfo(entity.m_EntityInfo.F_MaxHealth, enum_DamageType.Common, DamageDeliverInfo.Default(-1)));
            });
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            List<EntityBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityBase entity) => {
                if (entity.m_Flag == enum_EntityFlag.Enermy)
                    entity.BroadcastMessage("OnReceiveDamage", new DamageInfo(0, enum_DamageType.Common, DamageDeliverInfo.BuffInfo(-1, 200023)));
            });
        }
        if (Input.GetKeyDown(KeyCode.Equals))
            OnStageFinished();

        if (Input.GetKeyDown(KeyCode.F5))
            (m_LocalPlayer as EntityPlayerBase).TestUseAction(F5_TestActionIndex);
        if (Input.GetKeyDown(KeyCode.F6))
            (m_LocalPlayer as EntityPlayerBase).TestUseAction(F6_TestActionIndex);
        if (Input.GetKeyDown(KeyCode.F7))
            (m_LocalPlayer as EntityPlayerBase).TestUseAction(F7_TestActionIndex);

        UIManager.instance.transform.Find("Test/SeedTest").GetComponent<UnityEngine.UI.Text>().text = LevelManager.m_Seed;

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
    protected override void Awake()
    {
        instance = this;
        InitEntityDic();
        DataManager.Init();
        LevelManager.Init(M_TESTSEED);
        OptionsManager.Init();
        TBroadCaster<enum_BC_GameStatusChanged>.Init();

        m_PlayerInfo = TGameData<CPlayerSave>.Read();

        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnEntitySpawn, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnEntityRecycle, OnRecycleEntity);
        Application.targetFrameRate = 60;
    }
    private void OnDestroy()
    {
        this.StopAllSingleCoroutines();
        TGameData<CPlayerSave>.Save(m_PlayerInfo);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnEntitySpawn, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnEntityRecycle, OnRecycleEntity);
        
    }
    private void Start()
    {
        StartStage();        //Test
    }
    #region Level Management
    //Call When Level Changed
    void StartStage()      //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        enum_Style _style=LevelManager.StageBegin();

        EntityPreset();
        ObjectManager.Preset(_style,LevelManager.E_currentStage);
        m_BattleEntityStyle = _style;
        EnviormentManager.Instance.GenerateAllEnviorment(_style, LevelManager.m_GameSeed, OnLevelStart,OnStageFinished);
        m_StyledEnermyEntities = ObjectManager.RegisterAllEntitiesGetEnermyDic(TResources.GetCommonEntities() ,TResources.GetEnermyEntities(m_BattleEntityStyle));

        m_LocalPlayer = ObjectManager.SpawnEntity(0, Vector3.zero, enum_EntityFlag.Player);
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnStageStart);

        GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    void OnLevelStart(SBigmapLevelInfo levelInfo)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelStart);
        m_LocalPlayer.transform.position = levelInfo.m_Level.RandomEmptyTilePosition(LevelManager.m_GameSeed);
        if (LevelManager.CanLevelBattle(levelInfo))
            OnBattleStart(LevelManager.m_Difficulty);
        else
            OnLevelFinished(Vector3.zero);
    }

    //Call Enviorment Manager To Generate Interacts And Show Bigmaps, Then Go Back To OnLevelChange From Enviorment Manager
    void OnLevelFinished(Vector3 spawnInteractPos)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnLevelFinish,spawnInteractPos);

        //Generate Interacts
        switch (LevelManager.m_LevelType)
        {
            case enum_TileType.Battle:
                ObjectManager.SpawnInteractChest(EnviormentManager.NavMeshPosition(spawnInteractPos, false)).Play();
                break;
            case enum_TileType.Start:
                ObjectManager.SpawnInteractChest(EnviormentManager.NavMeshPosition(Vector3.left, false)).Play();
                ObjectManager.SpawnWeaponContainer(EnviormentManager.NavMeshPosition(Vector3.right, false)).Play(TCommon.RandomEnumValues<enum_PlayerWeapon>(), new List<ActionBase>() { DataManager.RendomWeaponAction(LevelManager.E_currentStage.ToActionLevel()) });
                break;
            case enum_TileType.End:
                ObjectManager.SpawnInteractPortal(EnviormentManager.NavMeshPosition(spawnInteractPos, false)).Play(OnStageFinished);
                break;
        }

    }

    void OnStageFinished()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnStageFinish);
        if (LevelManager.B_NextStage)
            StartStage();
        else
            Debug.Log("All Level Finished");
    }
    #endregion
    #region Entity Management
    Dictionary<int, EntityBase> m_Entities = new Dictionary<int, EntityBase>();
    Dictionary<enum_EntityFlag, List<EntityBase>> m_AllyEntities = new Dictionary<enum_EntityFlag, List<EntityBase>>();
    Dictionary<enum_EntityFlag, List<EntityBase>> m_OppositeEntities = new Dictionary<enum_EntityFlag, List<EntityBase>>();
    public int m_FlagEntityCount(enum_EntityFlag flag) => m_AllyEntities[flag].Count;
    public List<EntityBase> GetEntities(enum_EntityFlag sourceFlag, bool getAlly) => getAlly ? m_AllyEntities[sourceFlag] : m_OppositeEntities[sourceFlag];
    public EntityBase GetEntity(int entityID)
    {
        if (!m_Entities.ContainsKey(entityID))
            Debug.LogError("Entity Not Contains ID:" + entityID.ToString());
        return m_Entities[entityID]; ;
    }
    void InitEntityDic()
    {
        TCommon.TraversalEnum((enum_EntityFlag flag) => {
            m_AllyEntities.Add(flag, new List<EntityBase>());
            m_OppositeEntities.Add(flag, new List<EntityBase>());
        });
    }

    void EntityPreset()
    {
        m_Entities.Clear();
        TCommon.TraversalEnum((enum_EntityFlag flag) => {
            m_AllyEntities[flag].Clear();
            m_OppositeEntities[flag].Clear();
        });
    }

    void OnSpawnEntity(EntityBase entity)
    {
        m_Entities.Add(entity.I_EntityID, entity);
        
        m_AllyEntities[entity.m_Flag].Add(entity);
        m_OppositeEntities.Traversal((enum_EntityFlag flag)=> {
            if (flag != entity.m_Flag)
                m_OppositeEntities[flag].Add(entity);
        });
    }

    void OnRecycleEntity(EntityBase entity)
    {
        m_Entities.Remove(entity.I_EntityID);

        m_AllyEntities[entity.m_Flag].Remove(entity);
        m_OppositeEntities.Traversal((enum_EntityFlag flag) => {
            if (flag != entity.m_Flag)
                m_OppositeEntities[flag].Remove(entity);
        });
        OnBattleEntityRecycle(entity);
    }

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

    #endregion
    #region Battle Management
    public enum_Style m_BattleEntityStyle { get; private set; }
    public bool B_Battling { get; private set; } = false;
    public bool B_WaveEntityGenerating { get; private set; } = false;
    public int m_CurrentWave { get; private set; } = -1;
    public List<SGenerateEntity> m_EntityGenerate { get; private set; } = new List<SGenerateEntity>();
    public List<int> m_EntityGenerating { get; private set; } = new List<int>();
    public Dictionary<enum_EntityType, List<int>> m_StyledEnermyEntities;
    void OnBattleStart(enum_BattleDifficulty difficulty)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnBattleStart);
        m_EntityGenerate = DataManager.GetEntityGenerateProperties(difficulty);
        B_Battling = true;
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
    void OnBattleEntityRecycle(EntityBase entity)
    {
        if (!B_Battling|| B_WaveEntityGenerating)
            return;

        if (m_FlagEntityCount( enum_EntityFlag.Enermy) <= 0 || (m_CurrentWave < m_EntityGenerate.Count && m_FlagEntityCount(enum_EntityFlag.Enermy) <= GameConst.I_EnermyCountWaveFinish))
            WaveFinished(entity.transform.position);
    }
    void WaveFinished(Vector3 lastEntityPos)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnWaveFinish);
        m_CurrentWave++;
        if (m_CurrentWave >= m_EntityGenerate.Count)
            OnBattleFinished(lastEntityPos);
        else
            WaveStart();
    }
    void OnBattleFinished(Vector3 lastEntityPos)
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnBattleFinish);
        B_Battling = false;
        OnLevelFinished(lastEntityPos);
    }

    IEnumerator IE_GenerateEnermy(List<int> waveGenerate, float _offset)
    {
        B_WaveEntityGenerating = true;
        int curSpawnCount = 0;
        for (; ; )
        {
            yield return new WaitForSeconds(_offset);
            SpawnEnermy(waveGenerate[curSpawnCount], curSpawnCount, EnviormentManager.m_currentLevel.m_Level.RandomEmptyTilePosition(LevelManager.m_GameSeed));
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
            ObjectManager.SpawnEntity(entityIndex,position, enum_EntityFlag.Enermy );
        }));
    }
    #endregion
}
#region External Tools Packaging Class
public static class LevelManager
{
    public static enum_StageLevel E_currentStage;
    public static bool B_NextStage => E_currentStage != enum_StageLevel.Ranger;
    static enum_BattleDifficulty m_BattleDifficulty;
    public static enum_TileType m_LevelType { get; private set; }
    static Dictionary<enum_StageLevel, enum_Style> m_StageStyle = new Dictionary<enum_StageLevel, enum_Style>();
    public static string m_Seed { get; private set; }
    public static System.Random m_GameSeed { get; private set; }
    public static void Init(string _seed)
    {
        m_Seed = _seed == "" ? System.DateTime.Now.ToLongTimeString() : _seed;
        m_GameSeed = new System.Random( m_Seed.GetHashCode());
        E_currentStage = 0;
        List<enum_Style> styleList = TCommon.EnumList<enum_Style>();
        TCommon.TraversalEnum((enum_StageLevel level) => {
            enum_Style style = styleList.RandomItem(m_GameSeed);
            styleList.Remove(style);
            m_StageStyle.Add(level,style);
        });
    }
    public static enum_Style StageBegin()
    {
        E_currentStage++;
        m_BattleDifficulty = enum_BattleDifficulty.Peaceful;
        m_LevelType = enum_TileType.Invalid;
        return m_StageStyle[E_currentStage];
    }

    public static bool CanLevelBattle(SBigmapLevelInfo level)
    {
        m_LevelType = level.m_TileType;
        if (level.m_TileLocking != enum_TileLocking.Unlockable)
            return false;
        switch (m_LevelType)
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
public static class OptionsManager
{
    public static bool B_AdditionalLight = true;
    public static event Action event_OptionChanged;
    public static void Init()
    {
    }
    
    public static void OnOptionChanged()
    {
        event_OptionChanged?.Invoke();
    }
}
public static class IdentificationManager
{
    static int i_entityIndex = 0;
    public static int I_EntityID(enum_EntityFlag flag)
    {
        i_entityIndex++;
        if (i_entityIndex == int.MaxValue)
            i_entityIndex = 0;
        return i_entityIndex + (int)flag * 100000;
    }
    static int i_damageInfoIndex = 0;
    public static int I_DamageIdentityID()
    {
        i_damageInfoIndex++;
        if (i_damageInfoIndex == int.MaxValue)
            i_damageInfoIndex = 0;
        return i_damageInfoIndex;
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

    public static SLevelGenerate GetItemGenerateProperties(enum_Style style, enum_LevelGenerateType prefabType, bool isInner)
    {
        SLevelGenerate generate = Properties<SLevelGenerate>.PropertiesList.Find(p => p.m_LevelStyle == style && p.m_LevelPrefabType == prefabType && p.m_IsInner == isInner);
        if (generate.m_LevelStyle == 0 || generate.m_LevelPrefabType == 0 || generate.m_ItemGenerate == null)
            Debug.LogError("Error Properties Found Of Index:" + ((int)style * 100 + (int)prefabType * 10 + (isInner ? 0 : 1)).ToString());

        return generate;
    }

    public static List<SGenerateEntity> GetEntityGenerateProperties(enum_BattleDifficulty battleDifficulty)
    {
        List<SGenerateEntity> entityList = new List<SGenerateEntity>();
        int waveCount = 1;
        for (int i = 0; i < 10; i++)
        {
            List<SGenerateEntity> randomItems = Properties<SGenerateEntity>.PropertiesList.FindAll(p => p.m_Difficulty == battleDifficulty && p.m_waveCount == waveCount);
            if (randomItems == null || randomItems.Count == 0)
                break;
            entityList.Add(randomItems.RandomItem());
            waveCount++;
        }
        return entityList;
    }

    public static SWeapon GetWeaponProperties(enum_PlayerWeapon type)
    {
        SWeapon weapon = Properties<SWeapon>.PropertiesList.Find(p => p.m_Weapon == type);
        if (weapon.m_Weapon == 0)
            Debug.LogError("Error Properties Found Of Index:" + type.ToString() + "|" + ((int)type));
        return weapon;
    }
    public static SBuff GetEntityBuffProperties(int index)
    {
        SBuff buff = Properties<SBuff>.PropertiesList.Find(p => p.m_Index == index);
        if (buff.m_Index == 0)
            Debug.LogError("Error Properties Found Of Index:" + index);
        return buff;
    }

    static Dictionary<int, ActionBase> m_AllActions = new Dictionary<int, ActionBase>();
    static List<int> m_WeaponActions = new List<int>();
    static void InitActions() => TReflection.GetAllInheritClasses((Type type, ActionBase action) => {
        if (action.m_Index > 0)
            m_AllActions.Add(action.m_Index, action);
        if (action.m_ExpireType == enum_ActionExpireType.AfterWeaponSwitch)
            m_WeaponActions.Add(action.m_Index);
    }, enum_ActionLevel.Invalid);
    public static ActionBase RendomWeaponAction(enum_ActionLevel level)=> CreateAction(m_WeaponActions.RandomItem(),level);
    public static ActionBase CreateAction(int index, enum_ActionLevel level)
    {
        if (!m_AllActions.ContainsKey(index))
            Debug.LogError("Error Action:" + index + " ,Does not exist");
        return TReflection.CreateInstance<ActionBase>(m_AllActions[index].GetType(), level);
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
    public static void Preset(enum_Style levelStyle,enum_StageLevel stageLevel)
    {
        ObjectPoolManager<int, SFXBase>.ClearAll();
        ObjectPoolManager<int, EntityBase>.ClearAll();
        ObjectPoolManager<enum_Interaction, InteractBase>.ClearAll();
        ObjectPoolManager<LevelItemBase, LevelItemBase>.ClearAll();
        ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.ClearAll();
        ObjectPoolManager<int, LevelBase>.ClearAll();

        RegisterLevelBase(TResources.GetLevelBase(levelStyle));
        RegisterInteractions(levelStyle,stageLevel);
        TResources.GetAllEffectSFX().Traversal((int index, SFXBase target) => {
            ObjectPoolManager<int, SFXBase>.Register(index, target,
            enum_PoolSaveType.DynamicMaxAmount, 1,
            (SFXBase sfx) => { sfx.Init(index); });
        });
    }
    static void RegisterLevelBase(LevelBase levelprefab)
    {
        ObjectPoolManager<int, LevelBase>.Register(0, levelprefab, enum_PoolSaveType.DynamicMaxAmount, 1, (LevelBase level) => { level.Init(); });
    }
    public static void RegisterLevelItem(Dictionary<LevelItemBase, int> registerDic)
    {
        registerDic.Traversal((LevelItemBase item, int count) => { ObjectPoolManager<LevelItemBase, LevelItemBase>.Register(item, GameObject.Instantiate(item), enum_PoolSaveType.StaticMaxAmount, count, null); });
    }
    public static Dictionary<enum_EntityType, List<int>> RegisterAllEntitiesGetEnermyDic(Dictionary<int, EntityBase> common, Dictionary<int, EntityBase> enermies)
    {
        common.Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, enum_PoolSaveType.DynamicMaxAmount, 1,
                (EntityBase entityInstantiate) => { entityInstantiate.Init(index); });
        });

        Dictionary<enum_EntityType, List<int>> enermyDic = new Dictionary<enum_EntityType, List<int>>();
        enermies.Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, enum_PoolSaveType.DynamicMaxAmount, 1,
                (EntityBase entityInstantiate) => { entityInstantiate.Init(index); });

            EntityAIBase enermy = entity as EntityAIBase;
            if (!enermyDic.ContainsKey(enermy.E_EnermyType))
                enermyDic.Add(enermy.E_EnermyType, new List<int>());
            enermyDic[enermy.E_EnermyType].Add(index);
        });

        return enermyDic;
    }
    #endregion
    #region Spawn/Recycle
    #region Entity
    public static EntityBase SpawnEntity(int index, Vector3 toPosition, enum_EntityFlag _flag)
    {
        EntityBase entity = ObjectPoolManager<int, EntityBase>.Spawn(index, TF_Entity);
        toPosition = EnviormentManager.NavMeshPosition(toPosition);
        entity.transform.position = toPosition;
        entity.OnSpawn(IdentificationManager.I_EntityID(_flag), _flag);
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.OnEntitySpawn, entity);
        return entity;
    }

    public static void RecycleEntity(int index, EntityBase target) => ObjectPoolManager<int, EntityBase>.Recycle(index, target);
    #endregion
    #region Weapon
    public static WeaponBase SpawnWeapon(enum_PlayerWeapon type,Transform toTrans=null)
    {
        if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(type))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(type);
            ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(type, preset, enum_PoolSaveType.DynamicMaxAmount, 1, (WeaponBase weapon) => { weapon.Init(DataManager.GetWeaponProperties(type)); });
        }

        return ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(type, toTrans?toTrans:TF_Entity);
    }
    public static void RecycleWeapon(enum_PlayerWeapon type, WeaponBase weapon) => ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Recycle(type, weapon);
    #endregion
    #region SFX
    public static T SpawnSFX<T>(int index, Transform attachTo = null) where T : SFXBase
    {
        T sfx = ObjectPoolManager<int, SFXBase>.Spawn(index, attachTo ? attachTo : TF_SFXPlaying) as T;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid SFX Type:" + typeof(T) + ",Index:" + index);
        return sfx;
    }

    public static T SpawnParticles<T>(int index, Vector3 position, Vector3 normal, Transform attachTo = null) where T : SFXParticles
    {
        T sfx = SpawnSFX<T>(index, attachTo);
        sfx.transform.position = position;
        sfx.transform.rotation = Quaternion.LookRotation(normal);
        return sfx;
    }
    public static SFXIndicator SpawnIndicator(int index, Vector3 position, Vector3 normal, Transform attachTo = null) => SpawnParticles<SFXIndicator>(index, position, normal, attachTo);
    public static SFXBuffEffect SpawnBuffEffect(int index, EntityBase attachTo) => SpawnParticles<SFXBuffEffect>(index, attachTo.transform.position, attachTo.transform.forward, attachTo.transform);

    public static T SpawnEquipment<T>(int weaponIndex, Vector3 position, Vector3 normal, Transform attachTo = null) where T : SFXBase
    {
        if (!ObjectPoolManager<int, SFXBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), enum_PoolSaveType.DynamicMaxAmount, 1, (SFXBase sfx) => { sfx.Init(weaponIndex); });

        T template = ObjectPoolManager<int, SFXBase>.Spawn(weaponIndex, attachTo) as T;
        if (template == null)
            Debug.LogError("Enermy Weapon Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        template.transform.position = position;
        template.transform.rotation = Quaternion.LookRotation(normal);
        return template;
    }
    public static T GetEquipmentData<T>(int weaponIndex) where T : SFXBase
    {
        if (!ObjectPoolManager<int, SFXBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), enum_PoolSaveType.DynamicMaxAmount, 1, (SFXBase sfx) => { sfx.Init(weaponIndex); });

        T damageSourceInfo = ObjectPoolManager<int, SFXBase>.GetRegistedSpawnItem(weaponIndex) as T;
        if (damageSourceInfo == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return damageSourceInfo;
    }
    public static void RecycleSFX(int index, SFXBase sfx) => ObjectPoolManager<int, SFXBase>.Recycle(index, sfx);
    #endregion
    #region Interact
    static void RegisterInteractions(enum_Style portalStyle, enum_StageLevel stageIndex)
    {
        ObjectPoolManager<enum_Interaction, InteractBase>.Register( enum_Interaction.Portal,TResources.GetInteractPortal(portalStyle), enum_PoolSaveType.StaticMaxAmount,1,(InteractBase interact)=> { interact.Init(); });
        ObjectPoolManager<enum_Interaction, InteractBase>.Register(enum_Interaction.ActionChest, TResources.GetInteractActionChest(stageIndex), enum_PoolSaveType.StaticMaxAmount, 1, (InteractBase interact) => { interact.Init(); });
        ObjectPoolManager<enum_Interaction, InteractBase>.Register(enum_Interaction.WeaponContainer, TResources.GetInteractWeaponContainer(), enum_PoolSaveType.StaticMaxAmount, 1, (InteractBase interact) => { interact.Init(); });
    }
    public static InteractPortal SpawnInteractPortal(Vector3 toPos)
    {
        InteractPortal portal= ObjectPoolManager<enum_Interaction, InteractBase>.Spawn(enum_Interaction.Portal,TF_SFXPlaying) as InteractPortal;
        portal.transform.position = toPos;
        return portal;
    }
    public static InteractActionChest SpawnInteractChest(Vector3 toPos)
    {
        InteractActionChest chest = ObjectPoolManager<enum_Interaction, InteractBase>.Spawn(enum_Interaction.ActionChest, TF_SFXPlaying) as InteractActionChest;
        chest.transform.position = toPos;
        return chest;
    }
    public static InteractWeaponContainer SpawnWeaponContainer(Vector3 toPos)
    {
        InteractWeaponContainer container = ObjectPoolManager<enum_Interaction, InteractBase>.Spawn(enum_Interaction.WeaponContainer, TF_SFXPlaying) as InteractWeaponContainer;
        container.transform.position = toPos;
        return container;
    }
    public static void RecycleAllInteract() => ObjectPoolManager<enum_Interaction, InteractBase>.RecycleAll();
    #endregion
    #region Level/LevelItem
    public static LevelBase SpawnLevelPrefab(Transform toTrans)
    {
        return ObjectPoolManager<int, LevelBase>.Spawn(0, toTrans);
    }

    public static LevelItemBase SpawnLevelItem(LevelItemBase itemObject, Transform itemParent, Vector3 localPosition)
    {
        LevelItemBase spawnedItem = ObjectPoolManager<LevelItemBase, LevelItemBase>.Spawn(itemObject, itemParent);
        spawnedItem.transform.localPosition = localPosition;
        return spawnedItem;
    }
    public static void RecycleAllLevelItem()
    {
        ObjectPoolManager<LevelItemBase, LevelItemBase>.RecycleAll();
    }
    #endregion
    #endregion
}
public static class ActionsManager
{
    static List<ActionBase> m_ActionStored;
    static List<ActionBase> m_ActionInPool;
    static List<ActionBase> m_ActionHodling;
    
    public static void Init()
    {
        m_ActionStored = new List<ActionBase>();
        m_ActionInPool = new List<ActionBase>();
        m_ActionHodling = new List<ActionBase>();
    }
}
#endregion