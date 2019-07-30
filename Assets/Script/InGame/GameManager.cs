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
#if UNITY_EDITOR
    #region Test
    public enum_Style Test_TileStyle = enum_Style.Desert;
    public enum_Style Test_EntityStyle = enum_Style.Invalid;
    public string M_TESTSEED = "";
    public bool B_GameDebugGizmos = true;
    public int Z_TestEntityIndex = 221;
    public int TestEntityBuffApplyOnSpawn = 1;
    public int X_TestCastIndex = 30003;
    public int C_TestProjectileIndex = 29001;
    public int V_TestIndicatorIndex = 50002;
    public int B_TestBuffIndex = 1;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1f ? .1f : 1f;

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Z) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Physics.I_Static, ref hit))
        {
            EntityBase enermy = ObjectManager.SpawnEntity(Z_TestEntityIndex, hit.point);
            enermy.OnActivate();
            if (TestEntityBuffApplyOnSpawn > 0)
                enermy.OnReceiveBuff(TestEntityBuffApplyOnSpawn);
        }
        if (Input.GetKeyDown(KeyCode.X) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Physics.I_Static, ref hit))
            ObjectManager.SpawnDamageSource<SFXCast>(X_TestCastIndex, hit.point, Vector3.up).Play(1000, DamageBuffInfo.Create());
        if (Input.GetKeyDown(KeyCode.C) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Physics.I_Static, ref hit))
            ObjectManager.SpawnDamageSource<SFXProjectile>(C_TestProjectileIndex, hit.point + Vector3.up, m_LocalPlayer.transform.forward).Play(0, m_LocalPlayer.transform.forward, hit.point + m_LocalPlayer.transform.forward * 10, DamageBuffInfo.Create());
        if (Input.GetKeyDown(KeyCode.V) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Physics.I_Static, ref hit))
            ObjectManager.SpawnCommonIndicator(V_TestIndicatorIndex, hit.point + Vector3.up, Vector3.up).Play(1000);
        if (Input.GetKeyDown(KeyCode.B))
            m_LocalPlayer.OnReceiveBuff(B_TestBuffIndex);
        if (Input.GetKeyDown(KeyCode.N))
            m_LocalPlayer.BroadcastMessage("OnReceiveDamage", new DamageInfo(20, enum_DamageType.Projectile));
        if (Input.GetKeyDown(KeyCode.M))
            m_LocalPlayer.BroadcastMessage("OnReceiveDamage", new DamageInfo(-50, enum_DamageType.Projectile));
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<EntityBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityBase entity) => {
                if (!entity.B_IsPlayer)
                    entity.BroadcastMessage("OnReceiveDamage", new DamageInfo(entity.m_EntityInfo.F_MaxHealth + entity.m_EntityInfo.F_MaxArmor, enum_DamageType.DOT));
            });
        }

        UIManager.instance.transform.Find("SeedTest").GetComponent<UnityEngine.UI.Text>().text = m_SeedString;
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
        TBroadCaster<enum_BC_UIStatusChanged>.Init();
        TBroadCaster<enum_BC_GameStatusChanged>.Init();

        m_PlayerInfo = TGameData<CPlayerSave>.Read();

        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnSpawnEntity, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnRecycleEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnWaveEntityDead);
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

    public EntityBase GetEntity(int sourceIndex,bool isPlayer,Predicate<EntityBase> predict)
    {
        EntityBase target=null;
        m_Entities.TraversalRandom((int index, EntityBase entity) =>
        {
            if (entity.B_IsPlayer == isPlayer &&entity.I_EntityID!=sourceIndex&& (predict == null || predict(entity)))
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
    public void OnEntityFall(HitCheckEntity hitcheck)      //On Player Falls To Ocean ETC
    {
        hitcheck.TryHit(new DamageInfo( hitcheck.m_Attacher.B_IsPlayer ? GameConst.F_DamagePlayerFallInOcean : hitcheck.m_Attacher.m_HealthManager.F_TotalHealth, enum_DamageType.Fall));

        if (hitcheck.m_Attacher.B_IsPlayer)
            hitcheck.m_Attacher.transform.position = EnviormentManager.NavMeshPosition(hitcheck.m_Attacher.transform.position);
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
        ObjectManager.SpawnCommonIndicator(50001, position, Vector3.up).Play(entityIndex,GameConst.I_EnermySpawnDelay);
        this.StartSingleCoroutine(100 + spawnIndex, TIEnumerators.PauseDel(GameConst.I_EnermySpawnDelay, () => {
            ObjectManager.SpawnEntity(entityIndex,position ).OnActivate();
        }));
    }
    #endregion
}
#region External Tools Packaging Class
public static class DataManager
{
    public static void Init()
    {
        Properties<SLevelGenerate>.Init();
        Properties<SGenerateEntity>.Init();
        Properties<SWeapon>.Init();
        Properties<SEntity>.Init();
        Properties<SBuff>.Init();
    }
    public static SLevelGenerate GetItemGenerateProperties(enum_Style style,enum_LevelGenerateType prefabType,bool isInner)
    {
        SLevelGenerate generate = Properties<SLevelGenerate>.PropertiesList.Find(p => p.m_LevelStyle == style && p.m_LevelPrefabType == prefabType&&p.m_IsInner==isInner);
        if (generate.m_LevelStyle == 0 || generate.m_LevelPrefabType == 0||generate.m_ItemGenerate==null)
            Debug.LogError("Error Properties Found Of Index:" + ((int)style*100+ (int)prefabType*10+(isInner?0:1)).ToString());

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
    public static SBuff GetEntityBuffProperties(int index)
    {
        SBuff buff = Properties<SBuff>.PropertiesList.Find(p => p.m_Index == index);
        if (buff.m_Index == 0)
            Debug.LogError("Error Properties Found Of Index:" + index);
        return buff;
    }
}
public static class ObjectManager
{
    public static Transform TF_Entity;
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;
        TResources.GetAllCommonSFXs().Traversal((int index,SFXBase target)=>{
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
        toPosition = EnviormentManager.NavMeshPosition(toPosition);
        entity.transform.position = toPosition;
        entity.OnSpawn(GameManager.I_EntityID(i_entityIndex++, entity.B_IsPlayer));
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
        if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(type))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(type);
            ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(type, preset, enum_PoolSaveType.DynamicMaxAmount, 1, (WeaponBase weapon) => { weapon.Init(DataManager.GetWeaponProperties(type)); });
        }

        return ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(type, TF_Entity);
    }
    public static T SpawnCommonParticles<T>(int index, Vector3 position, Vector3 normal, Transform attachTo = null) where T:SFXParticles
    {
        T sfx = ObjectPoolManager<int, SFXBase>.Spawn(index, attachTo) as T;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid Common Particles,Index:" + index);
        sfx.transform.position = position;
        sfx.transform.rotation = Quaternion.LookRotation(normal);
        return sfx;
    }
    public static SFXIndicator SpawnCommonIndicator(int index, Vector3 position, Vector3 normal, Transform attachTo = null)
    {
        SFXIndicator sfx = ObjectPoolManager<int, SFXBase>.Spawn(index, attachTo) as SFXIndicator;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid Indicator,Index:" + index);
        sfx.transform.position = position;
        sfx.transform.rotation = Quaternion.LookRotation(normal);
        return sfx;
    }
    public static T SpawnDamageSource<T>(int weaponIndex,Vector3 position,Vector3 normal, Transform attachTo=null) where T:SFXBase
    {
        if (!ObjectPoolManager<int, SFXBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXBase>.Register(weaponIndex, TResources.GetEnermyWeaponSFX(weaponIndex), enum_PoolSaveType.DynamicMaxAmount, 1, (SFXBase sfx) => { sfx.Init(weaponIndex); });

        T template = ObjectPoolManager<int, SFXBase>.Spawn(weaponIndex, attachTo) as T;
        if (template == null)
            Debug.LogError("Enermy Weapon Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        template.transform.position = position;
        template.transform.rotation = Quaternion.LookRotation(normal);
        return template;
    }
    public static T EnermyDamageSourceInfo<T>(int weaponIndex) where T : SFXBase
    {
        if (!ObjectPoolManager<int, SFXBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXBase>.Register(weaponIndex, TResources.GetEnermyWeaponSFX(weaponIndex), enum_PoolSaveType.DynamicMaxAmount, 1, (SFXBase sfx) => { sfx.Init(weaponIndex); });

        T damageSourceInfo = ObjectPoolManager<int, SFXBase>.GetRegistedSpawnItem(weaponIndex) as T;
        if (damageSourceInfo == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return damageSourceInfo;
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
                if (!enermyDic.ContainsKey(entity.m_EntityInfo.m_InfoData.m_Type))
                    enermyDic.Add(entity.m_EntityInfo.m_InfoData.m_Type, new List<int>());
                enermyDic[entity.m_EntityInfo.m_InfoData.m_Type].Add(index);
            }
        });

        return enermyDic;
    }
}
#endregion