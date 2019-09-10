﻿using GameSetting;
using GameSetting_Action;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TExcel;
using UnityEngine;

public class GameManager : SimpleSingletonMono<GameManager>, ISingleCoroutine
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
    public enum_PlayerWeapon F1_WeaponSpawnType = enum_PlayerWeapon.Invalid;
    public int F5_TestActionIndex = 10001;
    public int F6_TestActionIndex = 10001;
    public int F7_TestActionIndex = 10001;
    public int F8_TestAcquireAction = 10001;
    public bool B_AdditionalLight = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            SetBulletTime(!m_BulletTiming);

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Z) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
        {
            EntityCharacterBase enermy = GameObjectManager.SpawnAI(Z_TestEntitySpawn, hit.point, m_GameLevel.m_currentStage, TestEntityFlag);
            if (TestEntityBuffOnSpawn > 0)
                enermy.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Common,DamageDeliverInfo.BuffInfo(-1, TestEntityBuffOnSpawn)));
        }
        if (Input.GetKeyDown(KeyCode.X) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnEquipment<SFXCast>(X_TestCastIndex, hit.point, CastForward?m_LocalPlayer.transform.forward: Vector3.up).Play(DamageDeliverInfo.Default(m_LocalPlayer.I_EntityID));
        if (Input.GetKeyDown(KeyCode.C) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnEquipment<SFXProjectile>(C_TestProjectileIndex, hit.point + Vector3.up, m_LocalPlayer.transform.forward).Play(DamageDeliverInfo.Default(m_LocalPlayer.I_EntityID), m_LocalPlayer.transform.forward, hit.point + m_LocalPlayer.transform.forward * 10);
        if (Input.GetKeyDown(KeyCode.V) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnIndicator(V_TestIndicatorIndex, hit.point + Vector3.up, Vector3.up).Play(1000,3f);
        if (Input.GetKeyDown(KeyCode.B))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Common, DamageDeliverInfo.BuffInfo(-1, B_TestBuffIndex )));
        if (Input.GetKeyDown(KeyCode.N))
            m_LocalPlayer.BroadcastMessage("OnReceiveDamage", new DamageInfo(20, enum_DamageType.Common,DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.M))
            m_LocalPlayer.BroadcastMessage("OnReceiveDamage", new DamageInfo(-50, enum_DamageType.Common, DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.F1))
            GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, Vector3.zero, EnvironmentManager.Instance.m_InteractParent).Play(GameObjectManager.SpawnWeapon(F1_WeaponSpawnType, new List<ActionBase>()));
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<EntityCharacterBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityCharacterBase entity) => {
                if (entity.m_Flag== enum_EntityFlag.Enermy)
                    entity.BroadcastMessage("OnReceiveDamage", new DamageInfo(entity.m_Health.m_MaxHealth, enum_DamageType.Common, DamageDeliverInfo.Default(-1)));
            });
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            List<EntityCharacterBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityCharacterBase entity) => {
                if (entity.m_Flag == enum_EntityFlag.Enermy)
                    entity.BroadcastMessage("OnReceiveDamage", new DamageInfo(0, enum_DamageType.Common, DamageDeliverInfo.BuffInfo(-1, 200023)));
            });
        }
        if (Input.GetKeyDown(KeyCode.Equals))
            OnStageFinished();

        if (Input.GetKeyDown(KeyCode.F5))
            (m_LocalPlayer as EntityCharacterPlayer).TestUseAction(F5_TestActionIndex);
        if (Input.GetKeyDown(KeyCode.F6))
            (m_LocalPlayer as EntityCharacterPlayer).TestUseAction(F6_TestActionIndex);
        if (Input.GetKeyDown(KeyCode.F7))
            (m_LocalPlayer as EntityCharacterPlayer).TestUseAction(F7_TestActionIndex);
        if (Input.GetKeyDown(KeyCode.F8))
            m_LocalPlayer.m_PlayerInfo.AddStoredAction(GameDataManager.CreateAction(F8_TestAcquireAction, enum_RarityLevel.L1));
        if (Input.GetKeyDown(KeyCode.F9))
            CameraEffectManager.StartAreaScan(m_LocalPlayer.tf_Head.position,Color.white, TResources.Load<Texture>(TResources.ConstPath.S_PETex_Holograph),15f,.7f,2f,50,3f);
        if (Input.GetKeyDown(KeyCode.F10))
            CameraEffectManager.AddCameraEffect<CB_DepthOfFieldSpecificStatic>().SetStaticTarget(m_RewardChest.GetComponentsInChildren<Renderer>());
        if (Input.GetKeyDown(KeyCode.F12))
        {
            EnvironmentManager.Instance.m_MapLevelInfo.Traversal((SBigmapLevelInfo info) => { if (info.m_TileLocking == enum_TileLocking.Locked) info.SetTileLocking(enum_TileLocking.Unlockable); });
            TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_LevelStatusChange, EnvironmentManager.Instance.m_MapLevelInfo, EnvironmentManager.Instance.m_currentLevel.m_TileAxis);
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            m_LocalPlayer.m_PlayerInfo.OnCoinsReceive(10);
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            m_LocalPlayer.m_PlayerInfo.AddActionAmount(1);
        if (OptionsManager.B_AdditionalLight != B_AdditionalLight)
        {
            OptionsManager.B_AdditionalLight = B_AdditionalLight;
            OptionsManager.OnOptionChanged();
        }
    }
    #endregion
#endif

    public GameLevelManager m_GameLevel { get; private set; }
    public GameRecordManager m_PlayerRecord { get; private set; }
    public EntityCharacterPlayer m_LocalPlayer { get; private set; } = null;
    InteractActionChest m_RewardChest;
    public bool B_ShowChestTips=>m_RewardChest!=null&&m_RewardChest.B_Interactable;
    protected override void Awake()
    {
        base.Awake();
        InitEntityDic();
        GameDataManager.Init();
        OptionsManager.Init();
        GameObjectManager.Init();
        m_GameLevel = M_TESTSEED != ""? new GameLevelManager(M_TESTSEED, enum_StageLevel.Rookie):new GameLevelManager(GameDataManager.m_PlayerGameInfo);
        TBroadCaster<enum_BC_GameStatus>.Init();
        TBroadCaster<enum_BC_UIStatus>.Init();  
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntiyActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityDeactivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
        Application.targetFrameRate = 60;
    }
    private void OnDisable()
    {
        this.StopAllSingleCoroutines();
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntiyActivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityDeactivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
    }
    private void Start()
    {
        StartStage();        //Test
    }
    #region Level Management
    //Call When Level Changed
    void StartStage()      //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        m_GameLevel.StageBegin();
        EntityPreset();
        GameObjectManager.Preset(m_GameLevel.m_currentStyle,m_GameLevel.m_currentStage);
        EnvironmentManager.Instance.GenerateAllEnviorment(m_GameLevel.m_currentStyle, m_GameLevel.m_GameSeed, OnLevelChanged,OnStageFinished);
        m_StyledEnermyEntities = GameObjectManager.RegisterAllCharacters(TResources.GetCommonEntities() ,TResources.GetEnermyEntities(m_GameLevel.m_currentStyle));
        m_LocalPlayer = GameObjectManager.SpawnEntityPlayer(GameDataManager.m_PlayerGameInfo);
        m_PlayerRecord = new GameRecordManager(GameDataManager.m_PlayerGameInfo);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageStart);

        GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    void OnLevelChanged(SBigmapLevelInfo levelInfo)
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnChangeLevel);
        m_LocalPlayer.transform.position = levelInfo.m_Level.RandomEmptyTilePosition(m_GameLevel.m_GameSeed);

        if (m_RewardChest != null)
        {
            GameObjectManager.RecycleInteract(m_RewardChest);
            m_RewardChest = null;
        }

        if (levelInfo.m_TileLocking == enum_TileLocking.Unlocked)
            return;
        m_PlayerRecord.OnLevelPassed();
        if (m_GameLevel.CanLevelBattle(levelInfo.m_TileType))
            OnBattleStart();
        else 
            SpawnInteracts();
    }
    void OnCharacterDead(EntityCharacterBase entity)
    {
        SpawnEntityDeadPickups(entity);
        if (entity.m_Flag == enum_EntityFlag.Enermy)
            m_PlayerRecord.OnEntityKilled();

        if(entity.m_Controller== enum_EntityController.Player)
            OnGameFinished(false);

        OnBattleCharacterDead(entity);
    }

    void OnStageFinished()
    {
        m_GameLevel.StageFinished();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageFinish);
        if (m_GameLevel.B_NextStage)
        {
            GameDataManager.AdjustGameData(m_LocalPlayer,m_GameLevel,m_PlayerRecord);
            StartStage();
        }
        else
        {
            OnGameFinished(true);
        }
    }

    void OnGameFinished(bool win)
    {
        GameDataManager.ClearGameData();
        float levelScore = GameExpression.GetResultLevelScore(m_GameLevel.m_currentStage,m_PlayerRecord.i_levelPassed);
        float killScore = GameExpression.GetResultKillScore( m_PlayerRecord.i_entitiesKilled);
        float coin = GameExpression.GetResultRewardCoins(levelScore+killScore);
        GameDataManager.m_PlayerInfo.f_blue += coin;
        GameDataManager.SavePlayerData();
        UIManager.Instance.ShowPage<UI_GameResult>(true).Play(win, levelScore, killScore, coin,()=> {
            GameObjectManager.RecycleAllObject();
            TSceneLoader.Instance.LoadScene( enum_Scene.Main);
        });
    }
    #endregion
    #region InteractManagement
    void SpawnInteracts()
    {
        switch (m_GameLevel.m_LevelType)
        {
            case enum_TileType.Start:
                {
                    enum_RarityLevel level = m_GameLevel.m_currentStage.ToActionLevel();
                    m_RewardChest = GameObjectManager.SpawnInteract<InteractActionChest>(enum_Interaction.ActionChest, EnvironmentManager.NavMeshPosition(Vector3.left * 2, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact);
                    if(m_GameLevel.m_currentStage== enum_StageLevel.Rookie)
                    m_RewardChest.Play(GameDataManager.CreateRandomPlayerActions(6,level,m_GameLevel.m_GameSeed),2);
                    else
                        m_RewardChest.Play(GameDataManager.CreateRandomPlayerActions(2, level, m_GameLevel.m_GameSeed), 1);
                    GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, EnvironmentManager.NavMeshPosition(Vector3.right * 2, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(GameObjectManager.SpawnWeapon(TCommon.RandomEnumValues<enum_PlayerWeapon>(m_GameLevel.m_GameSeed), new List<ActionBase>() { GameDataManager.CreateRendomWeaponAction(level, m_GameLevel.m_GameSeed) }));
                }
                break;
            case enum_TileType.CoinsTrade:
                {
                    GameObjectManager.SpawnTrader(1, Vector3.back * 2, EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact);
                    ActionBase action1 = GameDataManager.CreateRandomPlayerAction(m_GameLevel.m_actionGenerate.GetTradeRarityLevel(m_GameLevel.m_GameSeed), m_GameLevel.m_GameSeed);
                    int price1 = GameExpression.GetTradePrice(enum_Interaction.PickupAction, action1.m_Level).RandomRangeInt(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade, EnvironmentManager.NavMeshPosition(Vector3.left * 2, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(price1, GameObjectManager.SpawnInteract<InteractPickupAction>(enum_Interaction.PickupAction, Vector3.zero, EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(action1));
                    ActionBase action2 = GameDataManager.CreateRandomPlayerAction(m_GameLevel.m_actionGenerate.GetTradeRarityLevel(m_GameLevel.m_GameSeed), m_GameLevel.m_GameSeed);
                    int price2 = GameExpression.GetTradePrice(enum_Interaction.PickupAction, action2.m_Level).RandomRangeInt(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade, EnvironmentManager.NavMeshPosition(Vector3.right * 1.6f + Vector3.forward * 2, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(price2, GameObjectManager.SpawnInteract<InteractPickupAction>(enum_Interaction.PickupAction, Vector3.zero, EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(action2));

                    int price3 = GameExpression.GetTradePrice(enum_Interaction.PickupHealth, enum_RarityLevel.Invalid).RandomRangeInt(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade, EnvironmentManager.NavMeshPosition(Vector3.left * 1.6f + Vector3.forward * 2, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(price3, GameObjectManager.SpawnInteract<InteractPickupHealth>(enum_Interaction.PickupHealth, Vector3.zero, EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(20));

                    WeaponBase weapon = GameObjectManager.SpawnWeapon(TCommon.RandomEnumValues<enum_PlayerWeapon>(m_GameLevel.m_GameSeed), new List<ActionBase>() { GameDataManager.CreateRendomWeaponAction(m_GameLevel.m_actionGenerate.GetTradeRarityLevel(m_GameLevel.m_GameSeed), m_GameLevel.m_GameSeed) });
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade, EnvironmentManager.NavMeshPosition(Vector3.right * 2, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(10, GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, EnvironmentManager.NavMeshPosition(Vector3.right, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(weapon));
                }
                break;
            case enum_TileType.ActionAdjustment:
                {
                    GameObjectManager.SpawnTrader(2, Vector3.zero ,EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact);
                    GameObjectManager.SpawnInteract<InteractActionAdjustment>(enum_Interaction.ActionAdjustment,Vector3.zero, EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(m_GameLevel.m_currentStage);
                }
                break;
            case enum_TileType.BattleTrade:
                {
                    ActionBase action = GameDataManager.CreateRandomPlayerAction(enum_RarityLevel.L3, m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerBattle>(enum_Interaction.ContainerBattle, Vector3.zero, EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(OnBattleStart, GameObjectManager.SpawnInteract<InteractPickupAction>(enum_Interaction.PickupAction, Vector3.zero, EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(action));
                }
                break;
        }
    }

    void SpawnRewards(Vector3 rewardPos)
    {
        switch (m_GameLevel.m_LevelType)
        {
            case enum_TileType.End:
                GameObjectManager.SpawnInteract<InteractPortal>(enum_Interaction.Portal, EnvironmentManager.NavMeshPosition(rewardPos, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(OnStageFinished);
                break;
            case enum_TileType.Battle:
                {
                    enum_RarityLevel level = m_GameLevel.m_actionGenerate.GetActionRarityLevel(m_GameLevel.m_GameSeed);
                    m_RewardChest = GameObjectManager.SpawnInteract<InteractActionChest>(enum_Interaction.ActionChest, EnvironmentManager.NavMeshPosition(rewardPos, false), EnvironmentManager.Instance.m_currentLevel.m_Level.tf_Interact);
                    m_RewardChest.Play(GameDataManager.CreateRandomPlayerActions(2, level, m_GameLevel.m_GameSeed), 1);
                }
                break;
        }
    }
    void SpawnEntityDeadPickups(EntityCharacterBase entity)
    {
        if (entity.m_Flag != enum_EntityFlag.Enermy)
            return;
        EntityCharacterAI target = entity as EntityCharacterAI;

        if (m_GameLevel.m_actionGenerate.CanGenerateHealth(target.E_EnermyType))
            GameObjectManager.SpawnInteract<InteractPickupHealth>(enum_Interaction.PickupHealth, EnvironmentManager.NavMeshPosition(entity.transform.position, false), EnvironmentManager.Instance.m_currentLevel.m_Level.transform).Play(GameConst.I_HealthPickupAmount);

        if ( m_GameLevel.m_actionGenerate.CanGenerateArmor(target.E_EnermyType))
            GameObjectManager.SpawnInteract<InteractPickupArmor>(enum_Interaction.PickupArmor, EnvironmentManager.NavMeshPosition(entity.transform.position, false), EnvironmentManager.Instance.m_currentLevel.m_Level.transform).Play(GameConst.I_ArmorPickupAmount);

        int coinAmount = m_GameLevel.m_actionGenerate.GetCoinGenerate(target.E_EnermyType);
        if (coinAmount != -1)
            GameObjectManager.SpawnInteract<InteractPickupCoin>(enum_Interaction.PickupCoin, EnvironmentManager.NavMeshPosition(entity.transform.position, false), EnvironmentManager.Instance.m_currentLevel.m_Level.transform).Play(coinAmount);
    }
    #endregion
    #region Entity Management
    Dictionary<int, EntityCharacterBase> m_Entities = new Dictionary<int, EntityCharacterBase>();
    Dictionary<enum_EntityFlag, List<EntityCharacterBase>> m_AllyEntities = new Dictionary<enum_EntityFlag, List<EntityCharacterBase>>();
    Dictionary<enum_EntityFlag, List<EntityCharacterBase>> m_OppositeEntities = new Dictionary<enum_EntityFlag, List<EntityCharacterBase>>();
    public int m_FlagEntityCount(enum_EntityFlag flag) => m_AllyEntities[flag].Count;
    public List<EntityCharacterBase> GetEntities(enum_EntityFlag sourceFlag, bool getAlly) => getAlly ? m_AllyEntities[sourceFlag] : m_OppositeEntities[sourceFlag];
    public EntityCharacterBase GetEntity(int entityID)
    {
        if (!m_Entities.ContainsKey(entityID))
            Debug.LogError("Entity Not Contains ID:" + entityID.ToString());
        return m_Entities[entityID]; ;
    }
    void InitEntityDic()
    {
        TCommon.TraversalEnum((enum_EntityFlag flag) => {
            m_AllyEntities.Add(flag, new List<EntityCharacterBase>());
            m_OppositeEntities.Add(flag, new List<EntityCharacterBase>());
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

    void OnEntiyActivate(EntityBase entity)
    {
        if (entity.m_Controller== enum_EntityController.None)
            return;
        EntityCharacterBase character = entity as EntityCharacterBase;
        m_Entities.Add(entity.I_EntityID, character);
        m_AllyEntities[entity.m_Flag].Add(character);
        m_OppositeEntities.Traversal((enum_EntityFlag flag)=> {
            if (entity.m_Flag != enum_EntityFlag.Neutal && flag != entity.m_Flag)
                m_OppositeEntities[flag].Add(character);
        });
    }

    void OnEntityDeactivate(EntityBase entity)
    {
        if (entity.m_Controller == enum_EntityController.None)
            return;
        EntityCharacterBase character = entity as EntityCharacterBase;
        m_Entities.Remove(entity.I_EntityID);
        m_AllyEntities[entity.m_Flag].Remove(character);
        m_OppositeEntities.Traversal((enum_EntityFlag flag) => {
            if (entity.m_Flag != enum_EntityFlag.Neutal && flag != entity.m_Flag)
                m_OppositeEntities[flag].Remove(character);
        });
    }

    static bool B_CanHitEntity(HitCheckEntity targetHitCheck, int sourceID)  //If Match Will Hit Target,Player Particles ETC
    {
        if (targetHitCheck.I_AttacherID == sourceID)
            return false;
        return !Instance.m_Entities.ContainsKey(sourceID) || targetHitCheck.m_Attacher.m_Flag != Instance.m_Entities[sourceID].m_Flag;
    }
    public static bool B_CanDamageEntity(HitCheckEntity hb, int sourceID)   //After Hit,If Match Target Hit Succeed
    {
        if (hb.I_AttacherID == sourceID)
            return false;
        return Instance.m_Entities.ContainsKey(sourceID) &&hb.m_Attacher.m_Flag!= enum_EntityFlag.Neutal&&hb.m_Attacher.m_Flag != Instance.m_Entities[sourceID].m_Flag;
    }
    public static bool B_CanHitTarget(HitCheckBase hitCheck,int sourceID)
    {
        bool canHit = hitCheck.I_AttacherID!=sourceID;
        if (hitCheck.m_HitCheckType == enum_HitCheck.Entity)
            return canHit&&B_CanHitEntity(hitCheck as HitCheckEntity, sourceID);
        return canHit;
    }

    #endregion
    #region Battle Management
    public bool B_Battling { get; private set; } = false;
    public bool B_WaveEntityGenerating { get; private set; } = false;
    public int m_CurrentWave { get; private set; } = -1;
    public List<SGenerateEntity> m_EntityGenerate { get; private set; } = new List<SGenerateEntity>();
    public List<int> m_EntityGenerating { get; private set; } = new List<int>();
    public Dictionary<enum_CharacterType, List<int>> m_StyledEnermyEntities;
    void OnBattleStart()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleStart);
        m_EntityGenerate = GameDataManager.GetEntityGenerateProperties(m_GameLevel.m_Difficulty);
        B_Battling = true;
        m_CurrentWave = 0;
        WaveStart();
    }

    void WaveStart()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnWaveStart);
        m_EntityGenerating.Clear();
        m_EntityGenerate[m_CurrentWave].m_EntityGenerate.Traversal((enum_CharacterType level, RangeInt range) =>
        {
            int spawnCount = range.RandomRangeInt();
            for (int i = 0; i < spawnCount; i++)
            {
                if (!m_StyledEnermyEntities.ContainsKey(level))
                {
                    Debug.LogWarning("Current Enermy Style:" + m_GameLevel.m_currentStyle + " Not Contains Type:" + level);
                    continue;
                }
                m_EntityGenerating.Add(m_StyledEnermyEntities[level].RandomItem());
            }
        });
        this.StartSingleCoroutine(0, IE_GenerateEnermy(m_EntityGenerating, .1f));
    }

    void OnBattleCharacterDead(EntityCharacterBase entity)
    {
        if (!B_Battling|| B_WaveEntityGenerating)
            return;

        if (m_FlagEntityCount( enum_EntityFlag.Enermy) <= 0 || (m_CurrentWave < m_EntityGenerate.Count && m_FlagEntityCount(enum_EntityFlag.Enermy) <= GameConst.I_EnermyCountWaveFinish))
            WaveFinished(entity.transform.position);
    }

    void WaveFinished(Vector3 lastEntityPos)
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnWaveFinish);
        m_CurrentWave++;
        if (m_CurrentWave >= m_EntityGenerate.Count)
            OnBattleFinished(lastEntityPos);
        else
            WaveStart();
    }

    void OnBattleFinished(Vector3 lastEntityPos)
    {
        B_Battling = false;
        SpawnRewards(lastEntityPos);
        GameObjectManager.RecycleAllInteract(enum_Interaction.PickupArmor);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleFinish);
    }

    IEnumerator IE_GenerateEnermy(List<int> waveGenerate, float _offset)
    {
        B_WaveEntityGenerating = true;
        int curSpawnCount = 0;
        for (; ; )
        {
            yield return new WaitForSeconds(_offset);
            SpawnEnermy(waveGenerate[curSpawnCount], curSpawnCount, EnvironmentManager.Instance.m_currentLevel.m_Level.RandomEmptyTilePosition(m_GameLevel.m_GameSeed));
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
        GameObjectManager.SpawnIndicator(30001, position, Vector3.up).Play(entityIndex,GameConst.I_EnermySpawnDelay);
        this.StartSingleCoroutine(100 + spawnIndex, TIEnumerators.PauseDel(GameConst.I_EnermySpawnDelay, () => {
            GameObjectManager.SpawnAI(entityIndex,position , m_GameLevel.m_currentStage,enum_EntityFlag.Enermy);
        }));
    }

    static bool m_BulletTiming = false;
    public static void SetBulletTime(bool enter)
    {
        m_BulletTiming = enter;
        Time.timeScale = m_BulletTiming ? .1f : 1f;
    }
    #endregion
}
#region External Tools Packaging Class
public class GameLevelManager
{
    public StageInteractGenerate m_actionGenerate { get; private set; }
    public bool B_NextStage => m_currentStage <= enum_StageLevel.Ranger;
    public enum_TileType m_LevelType { get; private set; }
    public enum_StageLevel m_currentStage { get; private set; }
    Dictionary<enum_StageLevel, enum_Style> m_StageStyle = new Dictionary<enum_StageLevel, enum_Style>();
    public enum_Style m_currentStyle => m_StageStyle[m_currentStage];
    public string m_Seed { get; private set; }
    public System.Random m_GameSeed { get; private set; }
    public GameLevelManager(CPlayerGameSave _saveData):this(_saveData.m_GameSeed, _saveData.m_StageLevel)
    {
    }
    public GameLevelManager(string _seed,enum_StageLevel _stage)
    {
        m_Seed =  _seed;
        m_GameSeed = new System.Random( m_Seed.GetHashCode());
        m_currentStage = _stage;
        List<enum_Style> styleList = TCommon.EnumList<enum_Style>();
        TCommon.TraversalEnum((enum_StageLevel level) => {
            enum_Style style = styleList.RandomItem(m_GameSeed);
            styleList.Remove(style);
            m_StageStyle.Add(level,style);
        });
    }
    public void StageBegin()
    {
        m_actionGenerate = GameExpression.GetInteractGenerate(m_currentStage);
        m_BattleDifficulty = enum_BattleDifficulty.Peaceful;
        m_LevelType = enum_TileType.Invalid;
    }
    public void StageFinished()
    {
        m_currentStage++;
    }

    public bool CanLevelBattle(enum_TileType type)
    {
        m_LevelType = type;
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

    static enum_BattleDifficulty m_BattleDifficulty;
    public enum_BattleDifficulty m_Difficulty
    {
        get {
            switch (m_LevelType)
            {
                default:
                    return enum_BattleDifficulty.Peaceful;
                case enum_TileType.BattleTrade:
                    return enum_BattleDifficulty.BattleTrade;
                case enum_TileType.End:
                    return enum_BattleDifficulty.End;
                case enum_TileType.Battle:
                    return m_BattleDifficulty;
            }
        }
    }
}
public class GameRecordManager
{
    public int i_entitiesKilled { get; private set; } = 0;
    public int i_levelPassed { get; private set; } = 0;
    public GameRecordManager(CPlayerGameSave save)
    {
        i_entitiesKilled = save.m_kills;
        i_levelPassed = 0;
    }
    public void OnEntityKilled() => i_entitiesKilled++;
    public void OnLevelPassed() => i_levelPassed++;
}
public static class GameIdentificationManager
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
public static class GameDataManager
{
    public static CPlayerDataSave m_PlayerInfo { get; private set; }
    public static CPlayerGameSave m_PlayerGameInfo { get; private set; }
    public static void Init()
    {
        Properties<SLevelGenerate>.Init();
        Properties<SGenerateEntity>.Init();
        Properties<SWeapon>.Init();
        Properties<SBuff>.Init();
        InitActions();

        m_PlayerInfo = TGameData<CPlayerDataSave>.Read();
        m_PlayerGameInfo = TGameData<CPlayerGameSave>.Read();
    }
    #region GameSave
    public static void AdjustGameData(EntityCharacterPlayer data,GameLevelManager level,GameRecordManager record)
    {
        m_PlayerGameInfo.Adjust(data, level,record);
        TGameData<CPlayerGameSave>.Save(m_PlayerGameInfo);
    }
    public static void ClearGameData()
    {
        m_PlayerGameInfo = new CPlayerGameSave();
        TGameData<CPlayerGameSave>.Save(m_PlayerGameInfo);
    }
    public static void SavePlayerData()
    {
        TGameData<CPlayerDataSave>.Save(m_PlayerInfo);
    }
    #endregion
    #region ExcelData
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
        if (entityList.Count == 0)
            Debug.LogError("Null Entity Generate Found By:" + (int)battleDifficulty);
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
    #endregion
    #region ActionData
    static Dictionary<int, ActionBase> m_AllActions = new Dictionary<int, ActionBase>();
    static List<int> m_WeaponActions = new List<int>();
    static List<int> m_PlayerActions = new List<int>();
    static int m_ActionIdentity = 0;
    static void InitActions()
    {
        m_AllActions.Clear();
        m_WeaponActions.Clear();
        m_PlayerActions.Clear();
        TReflection.TraversalAllInheritedClasses((Type type, ActionBase action) => {
            if (action.m_Index <= 0)
                return;

            m_AllActions.Add(action.m_Index, action);
            if (action.m_ActionExpireType == enum_ActionExpireType.AfterWeaponSwitch)
                m_WeaponActions.Add(action.m_Index);
            else
                m_PlayerActions.Add(action.m_Index);
        },-1, enum_RarityLevel.Invalid);
    }
    public static ActionBase CreateRendomWeaponAction(enum_RarityLevel level,System.Random seed)=> CreateAction(m_WeaponActions.RandomItem(seed),level);
    public static List<ActionBase> CreateRandomPlayerActions(int actionCount, enum_RarityLevel level, System.Random seed)
    {
        List<ActionBase> actions = new List<ActionBase>();
        for (int i = 0; i < actionCount; i++)
        {
            int actionIndex = -1;
            m_PlayerActions.TraversalRandom((int index) =>
            {
                if (actions.Find(p => p.m_Index == index)==null)
                {
                    actionIndex = index;
                    return true;
                }
                return false;
            }, seed);
            if (actionIndex != -1)
                actions.Add(CreateAction(actionIndex, level));
        }
        return actions;
    }
    public static ActionBase CreateRandomPlayerAction(enum_RarityLevel level, System.Random seed) => CreateAction(m_PlayerActions.RandomItem(seed),level);
    public static List<ActionBase> CreateActions(List<ActionInfo> infos)
    {
        List<ActionBase> actions = new List<ActionBase>();
        infos.Traversal((ActionInfo info) => { actions.Add(CreateAction(info.m_Index, info.m_Level)); });
        return actions;
    }
    public static ActionBase CreateAction(int actionIndex, enum_RarityLevel level)
    {
        if (!m_AllActions.ContainsKey(actionIndex))
            Debug.LogError("Error Action:" + actionIndex + " ,Does not exist");
        return TReflection.CreateInstance<ActionBase>(m_AllActions[actionIndex].GetType(), m_ActionIdentity++, level);
    }
    public static ActionBase CopyAction(ActionBase targetAction)=> TReflection.CreateInstance<ActionBase>(m_AllActions[targetAction.m_Index].GetType(), targetAction.m_Identity, targetAction.m_Level);
    #endregion
}
public static class GameObjectManager
{
    static Transform TF_Entity;
    static Transform TF_SFXPlaying;
    public static Transform TF_SFXWaitForRecycle { get; private set; }
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;
        TF_SFXWaitForRecycle = new GameObject("SFX_WaitForRecycle").transform;
        TF_SFXPlaying = new GameObject("SFX_Playing").transform;
        ObjectPoolManager.Init();
    }
    public static void RecycleAllObject()
    {
        ObjectPoolManager<int, SFXBase>.ClearAll();
        ObjectPoolManager<int, EntityBase>.ClearAll();
        ObjectPoolManager<enum_Interaction, InteractBase>.ClearAll();
        ObjectPoolManager<LevelItemBase, LevelItemBase>.ClearAll();
        ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.ClearAll();
        ObjectPoolManager<int, LevelBase>.ClearAll();
    }
    #region Register
    public static void Preset(enum_Style levelStyle,enum_StageLevel stageLevel)
    {
        RecycleAllObject();
        RegisterLevelBase(TResources.GetLevelBase(levelStyle));
        RegisterInteractions(levelStyle,stageLevel);
        TResources.GetAllEffectSFX().Traversal((int index, SFXBase target) => {
            ObjectPoolManager<int, SFXBase>.Register(index, target, 1,
            (SFXBase sfx) => { sfx.Init(index); });
        });
    }
    static void RegisterLevelBase(LevelBase levelprefab)
    {
        ObjectPoolManager<int, LevelBase>.Register(0, levelprefab, 1, (LevelBase level) => { level.Init(); });
    }
    public static void RegisterLevelItem(Dictionary<LevelItemBase, int> registerDic)
    {
        registerDic.Traversal((LevelItemBase item, int count) => { ObjectPoolManager<LevelItemBase, LevelItemBase>.Register(item, GameObject.Instantiate(item), count, null); });
    }
    public static Dictionary<enum_CharacterType, List<int>> RegisterAllCharacters(Dictionary<int, EntityBase> commonEntities, Dictionary<int, EntityBase> styledAICharacters)
    {
        commonEntities.Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, 1,
                (EntityBase entityInstantiate) => { entityInstantiate.Init(index); });
        });

        Dictionary<enum_CharacterType, List<int>> enermyDic = new Dictionary<enum_CharacterType, List<int>>();
        styledAICharacters.Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, 1,
                (EntityBase entityInstantiate) => { entityInstantiate.Init(index); });

            EntityCharacterAI enermy = entity as EntityCharacterAI;
            if (!enermyDic.ContainsKey(enermy.E_EnermyType))
                enermyDic.Add(enermy.E_EnermyType, new List<int>());
            enermyDic[enermy.E_EnermyType].Add(index);
        });

        return enermyDic;
    }
    static void RegisterInteractions(enum_Style portalStyle, enum_StageLevel stageIndex)
    {
        ObjectPoolManager<enum_Interaction, InteractBase>.Register(enum_Interaction.Portal, TResources.GetInteractPortal(portalStyle), 5, (InteractBase interact) => { interact.Init(); });
        ObjectPoolManager<enum_Interaction, InteractBase>.Register(enum_Interaction.ActionChest, TResources.GetInteractActionChest(stageIndex), 5, (InteractBase interact) => { interact.Init(); });
        TCommon.TraversalEnum((enum_Interaction enumValue) =>
        {
            if (enumValue >= enum_Interaction.ContainerTrade)
                ObjectPoolManager<enum_Interaction, InteractBase>.Register(enumValue, TResources.GetInteract(enumValue),5, (InteractBase interact) => { interact.Init(); });
        });
    }
    #endregion
    #region Spawn/Recycle
    #region Entity
    static T SpawnEntity<T>(int _poolIndex, Vector3 toPos,enum_EntityFlag _flag, Transform parentTrans = null) where T:EntityBase
    {
        T entity = ObjectPoolManager<int, EntityBase>.Spawn(_poolIndex, TF_Entity) as T;
        if (entity == null)
            Debug.LogError("Entity ID:" + _poolIndex + ",Type:" + typeof(T).ToString() + " Not Found");
        entity.OnActivate(_flag);
        entity.gameObject.name = entity.I_EntityID.ToString() + "_" + _poolIndex.ToString();
        entity.transform.position = EnvironmentManager.NavMeshPosition(toPos, true);
        if (parentTrans) entity.transform.SetParent(parentTrans);
        return entity;
    }
    static T SpawnEntityCharacter<T>(int poolIndex, Vector3 toPosition, enum_EntityFlag _flag, Transform parentTrans = null) where T:EntityCharacterBase => SpawnEntity<T>(poolIndex,toPosition,_flag,parentTrans);
    public static EntityCharacterAI SpawnAI(int index, Vector3 toPosition, enum_StageLevel _stage, enum_EntityFlag _flag)
    {
        EntityCharacterAI entity= SpawnEntityCharacter<EntityCharacterAI>(index, toPosition, _flag);
        entity.SetDifficulty(_stage);
        return entity;
    }
    public static EntityTrader SpawnTrader(int index, Vector3 toPosition,Transform attachTo) => SpawnEntity<EntityTrader>(index, toPosition, enum_EntityFlag.Neutal,attachTo);
    public static EntityCharacterAISub SpawnSubAI(int index, Vector3 toPosition,int spanwer, enum_EntityFlag _flag)
    {
        EntityCharacterAISub entity = SpawnEntityCharacter<EntityCharacterAISub>(index, toPosition, _flag);
        entity.SetDifficulty(enum_StageLevel.Rookie);
        entity.OnRegister(spanwer);
        return entity;
    }
    public static EntityCharacterPlayer SpawnEntityPlayer(CPlayerGameSave playerSave)
    {
        EntityCharacterPlayer player = SpawnEntity<EntityCharacterPlayer>(0,Vector3.zero, enum_EntityFlag.Player);
        player.SetPlayerInfo(playerSave.m_coins,GameDataManager.CreateActions(playerSave.m_storedActions));
        player.ObtainWeapon(SpawnWeapon(playerSave.m_weapon,GameDataManager.CreateActions(playerSave.m_weaponActions)));
        return player;
    }
    public static void RecycleEntity(int index, EntityBase target) => ObjectPoolManager<int, EntityBase>.Recycle(index, target);
    #endregion
    #region Weapon
    public static WeaponBase SpawnWeapon(enum_PlayerWeapon type,List<ActionBase> actions,Transform toTrans=null)
    {
        if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(type))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(type);
            ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(type, preset, 1, (WeaponBase targetWeapon) => { targetWeapon.Init(GameDataManager.GetWeaponProperties(type)); });
        }
        WeaponBase weapon = ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(type, toTrans ? toTrans : TF_Entity);
        weapon.OnSpawn(actions);
        return weapon;
    }
    public static void RecycleWeapon(WeaponBase weapon)=> ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Recycle(weapon.m_WeaponInfo.m_Weapon,weapon);
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
    public static SFXBuffEffect SpawnBuffEffect(int index, EntityCharacterBase attachTo) => SpawnParticles<SFXBuffEffect>(index, attachTo.transform.position, attachTo.transform.forward, attachTo.transform);

    public static T SpawnEquipment<T>(int weaponIndex, Vector3 position, Vector3 normal, Transform attachTo = null) where T : SFXBase
    {
        if (!ObjectPoolManager<int, SFXBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1, (SFXBase sfx) => { sfx.Init(weaponIndex); });

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
            ObjectPoolManager<int, SFXBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1, (SFXBase sfx) => { sfx.Init(weaponIndex); });

        T damageSourceInfo = ObjectPoolManager<int, SFXBase>.GetRegistedSpawnItem(weaponIndex) as T;
        if (damageSourceInfo == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return damageSourceInfo;
    }
    public static void RecycleSFX(int index, SFXBase sfx) => ObjectPoolManager<int, SFXBase>.Recycle(index, sfx);
    #endregion
    #region Interact
    public static T SpawnInteract<T>(enum_Interaction type, Vector3 toPos, Transform toTrans=null) where T : InteractBase
    {
        T target = ObjectPoolManager<enum_Interaction, InteractBase>.Spawn(type , toTrans==null?TF_SFXPlaying:toTrans) as T;
        target.transform.position = toPos;
        return target;
    }
    public static void RecycleInteract(InteractBase target) => ObjectPoolManager<enum_Interaction, InteractBase>.Recycle(target.m_InteractType,target);
    public static void RecycleAllInteract(enum_Interaction interact) => ObjectPoolManager<enum_Interaction, InteractBase>.RecycleAll(interact);
    #endregion
    #region Level/LevelItem
    public static LevelBase SpawnLevelPrefab(Transform toTrans)=>ObjectPoolManager<int, LevelBase>.Spawn(0, toTrans);
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
public static class OptionsManager
{
    public static bool B_AdditionalLight = true;
    public static event Action event_OptionChanged;
    public static enum_LanguageRegion m_currentLanguage = enum_LanguageRegion.CN;
    public static void Init()
    {
        TLocalization.SetRegion(m_currentLanguage);
    }

    public static void OnOptionChanged()
    {
        event_OptionChanged?.Invoke();
    }
}
#endregion