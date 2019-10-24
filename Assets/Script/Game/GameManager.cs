using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : GameManagerBase
{
    protected static GameManager nInstance;
    public static new GameManager Instance => nInstance;
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
    public bool B_GameLevelDebugGizmos = true;
    public enumDebug_LevelDrawMode E_LevelDebug = enumDebug_LevelDrawMode.DrawTypes;
    public int Z_TestEntitySpawn = 221;
    public enum_EntityFlag TestEntityFlag = enum_EntityFlag.Enermy;
    public int TestEntityBuffOnSpawn = 1;
    public int X_TestCastIndex = 30003;
    public bool CastForward = true;
    public int C_TestProjectileIndex = 29001;
    public int V_TestIndicatorIndex = 50002;
    public int B_TestBuffIndex = 1;
    public int Comma_TestParticleIndex = 20001;
    public enum_PlayerWeapon F1_WeaponSpawnType = enum_PlayerWeapon.Invalid;
    public int F5_TestActionNormal = 10001;
    public int F6_TestActionOutstanding = 10001;
    public int F7_TestActionEpic = 10001;
    public int F8_TestAcquireAction = 10001;
    public bool B_AdditionalLight = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            SetBulletTime(!m_BulletTiming,.1f);

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Z) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
        {
            EntityCharacterBase enermy = GameObjectManager.SpawnAI(Z_TestEntitySpawn, hit.point, TestEntityFlag);
            if ((enermy as EntityCharacterAI) != null)
                (enermy as EntityCharacterAI).SetEnermyDifficulty(GameExpression.GetAIBaseHealthMultiplier(m_GameLevel.m_GameDifficulty), GameExpression.GetAIMaxHealthMultiplier(m_GameLevel.m_GameStage), GameExpression.GetEnermyGameDifficultyBuffIndex(m_GameLevel.m_GameDifficulty));
            if (TestEntityBuffOnSpawn > 0)
                enermy.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic,DamageDeliverInfo.BuffInfo(-1, TestEntityBuffOnSpawn)));
        }
        if (Input.GetKeyDown(KeyCode.X) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnEquipment<SFXCast>(X_TestCastIndex, hit.point, CastForward?m_LocalPlayer.transform.forward: Vector3.up).Play(DamageDeliverInfo.Default(m_LocalPlayer.I_EntityID));
        if (Input.GetKeyDown(KeyCode.C) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnEquipment<SFXProjectile>(C_TestProjectileIndex, hit.point + Vector3.up, m_LocalPlayer.transform.forward).Play(DamageDeliverInfo.Default(m_LocalPlayer.I_EntityID), m_LocalPlayer.transform.forward, hit.point + m_LocalPlayer.transform.forward * 10);
        if (Input.GetKeyDown(KeyCode.V) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnIndicator(V_TestIndicatorIndex, hit.point + Vector3.up, Vector3.up).Play(-1,3f);
        if (Input.GetKeyDown(KeyCode.Comma) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnParticles<SFXParticles>(Comma_TestParticleIndex, hit.point + Vector3.up, Vector3.up).Play(-1);
        if (Input.GetKeyDown(KeyCode.B))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.BuffInfo(-1, B_TestBuffIndex )));
        if (Input.GetKeyDown(KeyCode.N))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(20, enum_DamageType.Basic,DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.M))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-50, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.F1) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon,hit.point, LevelManager.Instance.m_InteractParent).Play(GameObjectManager.SpawnWeapon(F1_WeaponSpawnType,null));
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<EntityCharacterBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityCharacterBase entity) => {
                if (entity.m_Flag== enum_EntityFlag.Enermy)
                    entity.m_HitCheck.TryHit( new DamageInfo(entity.m_Health.m_MaxHealth, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
            });
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            m_Entities.Traversal((EntityCharacterBase entity) => {
                if (entity.m_Flag == enum_EntityFlag.Enermy)
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.EquipmentInfo(-1,0, enum_CharacterEffect.Freeze,2f)));
            });
        }

        if (Input.GetKeyDown(KeyCode.Equals))
            OnStageFinished();

        if (Input.GetKeyDown(KeyCode.F5))
            (m_LocalPlayer as EntityCharacterPlayer).TestUseAction(F5_TestActionNormal, enum_RarityLevel.Normal);
        if (Input.GetKeyDown(KeyCode.F6))
            (m_LocalPlayer as EntityCharacterPlayer).TestUseAction(F6_TestActionOutstanding, enum_RarityLevel.OutStanding);
        if (Input.GetKeyDown(KeyCode.F7))
            (m_LocalPlayer as EntityCharacterPlayer).TestUseAction(F7_TestActionEpic, enum_RarityLevel.Epic);
        if (Input.GetKeyDown(KeyCode.F8))
            m_LocalPlayer.m_PlayerInfo.AddStoredAction(GameDataManager.CreateAction(F8_TestAcquireAction, enum_RarityLevel.Normal));
        if (Input.GetKeyDown(KeyCode.F9))
        {
            CameraController.Instance.m_Effect.StartAreaScan(m_LocalPlayer.tf_Head.position, Color.white, TResources.Load<Texture>(TResources.ConstPath.S_PETex_Holograph),15f, 1f, 5f, 50, 1f);
            m_Entities.Traversal((EntityCharacterBase entity) => {
                if (entity.m_Flag == enum_EntityFlag.Enermy)
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.EquipmentInfo(-1, 0, enum_CharacterEffect.Scan, 10f)));
            });
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            LevelManager.Instance.m_MapLevelInfo.Traversal((SBigmapLevelInfo info) => { if (info.m_TileLocking == enum_TileLocking.Unseen) info.SetTileLocking(enum_TileLocking.Unlockable); });
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            m_LocalPlayer.m_PlayerInfo.AddActionEnergy(1);

        if (Input.GetKeyDown(KeyCode.Keypad1) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupCoin, LevelManager.NavMeshPosition(hit.point, false), LevelManager.Instance.m_InteractParent).Play(10, m_LocalPlayer.transform);
        if (Input.GetKeyDown(KeyCode.Keypad2) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupArmor, LevelManager.NavMeshPosition(hit.point, false), LevelManager.Instance.m_InteractParent).Play(10, m_LocalPlayer.transform);
        if (Input.GetKeyDown(KeyCode.Keypad3) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupHealth, LevelManager.NavMeshPosition(hit.point, false), LevelManager.Instance.m_InteractParent).Play(10, m_LocalPlayer.transform);
    }
    #endregion
#endif
    public GameLevelManager m_GameLevel { get; private set; }
    public EntityCharacterPlayer m_LocalPlayer { get; private set; } = null;
    InteractActionChest m_RewardChest;
    public bool B_ShowChestTips=>m_RewardChest!=null&&m_RewardChest.B_Interactable;
    public override bool B_InGame => true;
    protected override void Awake()
    {
        nInstance=this;
        base.Awake();
        InitEntityDic();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntiyActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterRevive, OnCharacterRevive);
        m_GameLevel = M_TESTSEED != "" ? new GameLevelManager(M_TESTSEED, enum_StageLevel.Rookie, 1) : new GameLevelManager(GameDataManager.m_PlayerGameData,GameDataManager.m_PlayerLevelData);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        nInstance = null;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntiyActivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterRevive, OnCharacterRevive);
    }
    protected override void Start()
    {
        base.Start();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameStart);
        StartStage();
    }
    public void OnExitGame()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameExit);
        SwitchScene( enum_Scene.Camp);
    }

    #region Level Management
    //Call When Level Changed
    void StartStage()      //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        GameObjectManager.RecycleAllObject();
        GameObjectManager.PresetRegistCommonObject();

        EntityPreset();
        m_GameLevel.OnStageBegin();
        m_Enermies = GameObjectManager.RegistStyledIngameEnermies(m_GameLevel.m_GameStyle, m_GameLevel.m_GameStage);
        m_LocalPlayer = GameObjectManager.SpawnEntityPlayer(GameDataManager.m_PlayerLevelData);
        LevelManager.Instance.GenerateAllEnviorment(m_GameLevel.m_GameStyle, m_GameLevel.m_GameSeed, OnLevelChanged, OnStageFinished);
        SetPostEffects(m_GameLevel.m_GameStyle);
        GC.Collect();
        Resources.UnloadUnusedAssets();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageStart);
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

        bool levelUnlocked = levelInfo.m_TileLocking == enum_TileLocking.Unlocked;
        m_GameLevel.OnLevelChange(levelInfo.m_LevelType,levelUnlocked);
        if (levelUnlocked)
            return;
        if (m_GameLevel.WillBattle())
            OnBattleStart();
        else 
            SpawnInteracts();
    }

    void OnStageFinished()
    {
        m_RewardChest = null;
        m_GameLevel.StageFinished();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageFinish);
        if (m_GameLevel.B_NextStage)
        {
            GameDataManager.AdjuastInGameData(m_LocalPlayer,m_GameLevel);
            StartStage();
        }
        else
        {
            OnGameFinished(true);
        }
    }

    void OnGameFinished(bool win)
    {
        m_GameLevel.OnGameFinished(win);
        GameDataManager.OnGameFinished(win);
        GameDataManager.OnCreditGain(m_GameLevel.F_CreditGain);
        UIManager.Instance.OnGameFinished(m_GameLevel, OnExitGame);
    }
    #endregion
    #region InteractManagement
    void SpawnInteracts()
    {
        switch (m_GameLevel.m_LevelType)
        {
            case enum_TileType.Start:
                {
                    GameObjectManager.SpawnInteract<InteractBonfire>(enum_Interaction.Bonfire, LevelManager.NavMeshPosition( Vector3.zero,false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play();

                    m_RewardChest = GameObjectManager.SpawnInteract<InteractActionChest>(enum_Interaction.ActionChestStart, LevelManager.NavMeshPosition(Vector3.left * 3, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact);
                    m_RewardChest.Play(GameDataManager.CreateRandomPlayerActions(3, m_GameLevel.m_GameStage.GetStartChestRarity(), m_GameLevel.m_GameSeed), 1);
                    
                    GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, LevelManager.NavMeshPosition(Vector3.right * 3, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(GameObjectManager.SpawnWeapon(TCommon.RandomEnumValues<enum_PlayerWeapon>(m_GameLevel.m_GameSeed),  GameDataManager.CreateRandomWeaponPerk(m_GameLevel.m_GameStage.GetStartWeaponPerkRarity(), m_GameLevel.m_GameSeed)));
                }
                break;
            case enum_TileType.CoinsTrade:
                {
                    GameObjectManager.SpawnTrader(1, Vector3.back *2f, LevelManager.Instance.m_currentLevel.m_Level.tf_Interact);

                    int priceHealth = GameExpression.GetTradePrice(enum_Interaction.PickupHealth, enum_RarityLevel.Invalid).RandomRangeInt(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade, LevelManager.NavMeshPosition(Vector3.left * 1.5f + Vector3.forward * 1f, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(priceHealth, GameObjectManager.SpawnInteract<InteractPickupHealth>(enum_Interaction.PickupHealth, Vector3.zero, LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(GameConst.I_HealthTradeAmount, null));

                    ActionBase action1 = GameDataManager.CreateRandomPlayerAction(m_GameLevel.m_actionGenerate.GetTradeRarityLevel(m_GameLevel.m_GameSeed), m_GameLevel.m_GameSeed);
                    int priceAction = GameExpression.GetTradePrice(enum_Interaction.PickupAction, action1.m_rarity).RandomRangeInt(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade, LevelManager.NavMeshPosition(Vector3.forward*1f, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(priceAction, GameObjectManager.SpawnInteract<InteractPickupAction>(enum_Interaction.PickupAction, Vector3.zero, LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(action1));

                    WeaponBase weapon = GameObjectManager.SpawnWeapon(TCommon.RandomEnumValues<enum_PlayerWeapon>(m_GameLevel.m_GameSeed), GameDataManager.CreateRandomWeaponPerk(m_GameLevel.m_GameStage.GetTradeWeaponPerkRarity(), m_GameLevel.m_GameSeed));
                    int priceWeapon = GameExpression.GetTradePrice(enum_Interaction.Weapon, weapon.m_WeaponAction.m_rarity).RandomRangeInt(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade, LevelManager.NavMeshPosition(Vector3.right * 1.5f + Vector3.forward * 1f, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(priceWeapon, GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, LevelManager.NavMeshPosition(Vector3.right, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(weapon));
                }
                break;
            case enum_TileType.ActionAdjustment:
                {
                    GameObjectManager.SpawnTrader(2, Vector3.zero ,LevelManager.Instance.m_currentLevel.m_Level.tf_Interact);
                    GameObjectManager.SpawnInteract<InteractActionAdjustment>(enum_Interaction.ActionAdjustment,Vector3.zero, LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(m_GameLevel.m_GameStage);
                }
                break;
            case enum_TileType.BattleTrade:
                {
                    if (TCommon.RandomBool(m_GameLevel.m_GameSeed))
                    {
                        ActionBase action = GameDataManager.CreateRandomPlayerAction(m_GameLevel.m_GameStage.GetBattleTradeActionRarity(), m_GameLevel.m_GameSeed);
                        GameObjectManager.SpawnInteract<InteractContainerBattle>(enum_Interaction.ContainerBattle, Vector3.zero, LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(OnBattleStart, GameObjectManager.SpawnInteract<InteractPickupAction>(enum_Interaction.PickupAction, Vector3.zero, LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(action));
                    }
                    else
                    {
                        GameObjectManager.SpawnInteract<InteractPerkUpgrade>(enum_Interaction.PerkUpgrade, Vector3.zero, LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(OnBattleStart,GameDataManager.CreateRandomWeaponPerk( enum_RarityLevel.Normal,m_GameLevel.m_GameSeed));
                    }
                }
                break;
        }
    }

    void SpawnRewards(Vector3 rewardPos)
    {
        switch (m_GameLevel.m_LevelType)
        {
            case enum_TileType.End:
                GameObjectManager.SpawnInteract<InteractPortal>(enum_Interaction.Portal, LevelManager.NavMeshPosition(rewardPos, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(OnStageFinished);
                break;
            case enum_TileType.Battle:
                {
                    enum_RarityLevel level = m_GameLevel.m_actionGenerate.GetActionRarityLevel(m_GameLevel.m_GameSeed);
                    m_RewardChest = GameObjectManager.SpawnInteract<InteractActionChest>(enum_Interaction.ActionChest, LevelManager.NavMeshPosition(rewardPos, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact);
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
            GameObjectManager.SpawnInteract<InteractPickupHealth>(enum_Interaction.PickupHealth, LevelManager.NavMeshPosition(entity.transform.position, false)).Play(GameConst.I_HealthPickupAmount, m_LocalPlayer.transform);

        if (m_GameLevel.m_actionGenerate.CanGenerateArmor(target.E_EnermyType))
            GameObjectManager.SpawnInteract<InteractPickupArmor>(enum_Interaction.PickupArmor, LevelManager.NavMeshPosition(entity.transform.position, false)).Play(GameConst.I_ArmorPickupAmount,m_LocalPlayer.transform);
        
        int coinAmount = m_GameLevel.m_actionGenerate.GetCoinGenerate(target.E_EnermyType);
        if (coinAmount != -1)
            GameObjectManager.SpawnInteract<InteractPickupCoin>(enum_Interaction.PickupCoin, LevelManager.NavMeshPosition(entity.transform.position, false)).Play(coinAmount,m_LocalPlayer.transform);
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

    void OnEntityRecycle(EntityBase entity)
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

        SpawnEntityDeadPickups(character);
        OnBattleCharacterDead(character);
        if (entity.m_Controller == enum_EntityController.Player)
            OnGameFinished(false);
    }
    public static bool B_CanDamageEntity(HitCheckEntity hb, int sourceID)   //After Hit,If Match Target Hit Succeed
    {
        if (hb.I_AttacherID == sourceID)
            return false;
        return Instance.m_Entities.ContainsKey(sourceID) &&hb.m_Attacher.m_Flag != Instance.m_Entities[sourceID].m_Flag;
    }
    public static bool B_CanHitTarget(HitCheckBase hitCheck,int sourceID)
    {
        bool canHit = hitCheck.I_AttacherID!=sourceID;
        if (hitCheck.m_HitCheckType == enum_HitCheck.Entity)
            return canHit&&B_CanHitEntity(hitCheck as HitCheckEntity, sourceID);
        return canHit;
    }

    static bool B_CanHitEntity(HitCheckEntity targetHitCheck, int sourceID)  //If Match Will Hit Target,Player Particles ETC
    {
        if (targetHitCheck.I_AttacherID == sourceID || targetHitCheck.m_Attacher.m_Flag == enum_EntityFlag.Neutal)
            return false;
        return !Instance.m_Entities.ContainsKey(sourceID) || targetHitCheck.m_Attacher.m_Flag != Instance.m_Entities[sourceID].m_Flag;
    }
    #endregion
    #region Player Management
    List<RangeFloat> m_PlayerReviveHealing = new List<RangeFloat>();
    void OnCharacterDead(EntityCharacterBase character)
    {
        if (character.m_Controller != enum_EntityController.Player)
            return;
        SetPostEffect_Dead();
        this.StartSingleCoroutine(10, TIEnumerators.PauseDel(GameConst.F_PlayerReviveCheckAfterDead, CheckRevive));
    }

    void OnCharacterRevive(EntityCharacterBase character)
    {
        if (character.m_Controller != enum_EntityController.Player)
            return;
        SetPostEffect_Revive();
    }

    public void AddPlayerReviveCheck(RangeFloat reviveData)
    {
        m_PlayerReviveHealing.Add(reviveData);
    }

    void CheckRevive()
    {
        if (m_PlayerReviveHealing.Count > 0)
        {
            m_LocalPlayer.OnRevive(m_PlayerReviveHealing[m_PlayerReviveHealing.Count - 1].start, m_PlayerReviveHealing[m_PlayerReviveHealing.Count-1].length);
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.BuffInfo(-1,SBuff.SystemPlayerReviveInfo(GameConst.F_PlayerReviveBuffDuration,GameExpression.I_PlayerReviveBuffIndex))));
            m_PlayerReviveHealing.RemoveAt(m_PlayerReviveHealing.Count - 1);
            return;
        }
        UIManager.Instance.ShowPage<UI_Revive>(true, 0f).Play(ForceRevivePlayer, OnCreditRevivePlayer, 50,GameDataManager.CanUseCredit);
    }
    void OnCreditRevivePlayer()
    {
        GameDataManager.OnCreditGain(-50);
        ForceRevivePlayer();
    }
    void ForceRevivePlayer()
    {
        m_PlayerReviveHealing.Add(new RangeFloat(m_LocalPlayer.m_Health.m_MaxHealth,m_LocalPlayer.m_Health.m_DefaultArmor));  
        CheckRevive();
    }
    #endregion
    #region Battle Management
    public bool B_Battling { get; private set; } = false;
    public bool B_WaveEntityGenerating { get; private set; } = false;
    public int m_CurrentWave { get; private set; } = -1;
    public List<SGenerateEntity> m_EntityGenerate { get; private set; } = new List<SGenerateEntity>();
    public List<int> m_EntityGenerating { get; private set; } = new List<int>();
    public Dictionary<enum_CharacterType, List<int>> m_Enermies;
    void OnBattleStart()
    {
        m_EntityGenerate = GameDataManager.GetEntityGenerateProperties(m_GameLevel.m_GameStage, m_GameLevel.m_Difficulty);
        B_Battling = true;
        m_CurrentWave = 0;
        WaveStart();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleStart);
    }

    void WaveStart()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnWaveStart, m_EntityGenerate.Count,m_CurrentWave);
        m_EntityGenerating.Clear();
        m_EntityGenerate[m_CurrentWave].m_EntityGenerate.Traversal((enum_CharacterType level, RangeInt range) =>
        {
            int spawnCount = range.RandomRangeInt();
            for (int i = 0; i < spawnCount; i++)
            {
                if (!m_Enermies.ContainsKey(level))
                {
                    Debug.LogWarning("Current Enermy Style:" + m_GameLevel.m_GameStyle + " Not Contains Type:" + level);
                    continue;
                }
                m_EntityGenerating.Add(m_Enermies[level].RandomItem());
            }
        });
        this.StartSingleCoroutine(99, IE_GenerateEnermy(m_EntityGenerating, .1f));
    }

    void OnBattleCharacterDead(EntityCharacterBase entity)
    {
        if (!B_Battling || B_WaveEntityGenerating || entity.m_Flag != enum_EntityFlag.Enermy)
            return;

        m_GameLevel.OnEntityKilled();
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
        m_PlayerReviveHealing.Clear();
        GameObjectManager.RecycleAllInteract(enum_Interaction.PickupArmor);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleFinish);
    }

    IEnumerator IE_GenerateEnermy(List<int> waveGenerate, float _offset)
    {
        B_WaveEntityGenerating = true;
        int curSpawnCount = 0;
        SBuff enermyDifficultyBuff = GameExpression.GetEnermyGameDifficultyBuffIndex(m_GameLevel.m_GameDifficulty);
        float baseHealthMultiplier = GameExpression.GetAIBaseHealthMultiplier(m_GameLevel.m_GameDifficulty);
        float maxHealthMultiplier = GameExpression.GetAIMaxHealthMultiplier(m_GameLevel.m_GameStage);
        for (; ; )
        {
            yield return new WaitForSeconds(_offset);
            SpawnEnermy(waveGenerate[curSpawnCount], curSpawnCount, LevelManager.Instance.m_currentLevel.m_Level.RandomEmptyTilePosition(m_GameLevel.m_GameSeed),baseHealthMultiplier,maxHealthMultiplier,enermyDifficultyBuff);
            curSpawnCount++;
            if (curSpawnCount >= waveGenerate.Count)
            {
                B_WaveEntityGenerating = false;
                yield break;
            }
        }
    }

    void SpawnEnermy(int entityIndex, int spawnIndex,Vector3 position, float baseHealthMultiplier, float maxHealthMultiplier, SBuff difficultyBuff)
    {
        GameObjectManager.SpawnIndicator(30001, position, Vector3.up).Play(entityIndex, GameConst.I_EnermySpawnDelay);
        this.StartSingleCoroutine(100 + spawnIndex, TIEnumerators.PauseDel(GameConst.I_EnermySpawnDelay, () => {
            GameObjectManager.SpawnAI(entityIndex,position , enum_EntityFlag.Enermy).SetEnermyDifficulty(baseHealthMultiplier, maxHealthMultiplier,difficultyBuff);
        }));
    }
    #endregion
}

#region External Tools Packaging Class
public class GameLevelManager
{
    #region LevelData
    public StageInteractGenerate m_actionGenerate { get; private set; }
    public bool B_NextStage => m_GameStage <= enum_StageLevel.Ranger;
    public enum_TileType m_LevelType { get; private set; }
    public enum_StageLevel m_GameStage { get; private set; }
    public int m_GameDifficulty { get; private set; }
    Dictionary<enum_StageLevel, enum_Style> m_StageStyle = new Dictionary<enum_StageLevel, enum_Style>();
    public enum_Style m_GameStyle => m_StageStyle[m_GameStage];
    public string m_Seed { get; private set; }
    public System.Random m_GameSeed { get; private set; }
    static enum_BattleDifficulty m_BattleDifficulty;
    public enum_BattleDifficulty m_Difficulty
    {
        get
        {
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
    #endregion
    #region RecordData
    public int m_enermiesKilled { get; private set; }
    public bool m_gameWin { get; private set; }
    int m_levelEntered;
    int m_battleLevelEntered;
    #endregion
    public GameLevelManager(CPlayerGameSave _gameSave,CPlayerLevelSave _playerSave):this(_playerSave.m_GameSeed, _playerSave.m_StageLevel, _gameSave.m_GameDifficulty)
    {
        m_enermiesKilled = _playerSave.m_kills;
    }
    public GameLevelManager(string _seed,enum_StageLevel _stage, int _gameDifficulty)
    {
        m_Seed =  _seed;
        m_GameSeed = new System.Random( m_Seed.GetHashCode());
        m_GameStage = _stage;
        m_GameDifficulty = _gameDifficulty;
        List<enum_Style> styleList = TCommon.EnumList<enum_Style>();
        TCommon.TraversalEnum((enum_StageLevel level) => {
            enum_Style style = styleList.RandomItem(m_GameSeed);
            styleList.Remove(style);
            m_StageStyle.Add(level,style);
        });
    }
    public void OnStageBegin()
    {
        m_actionGenerate = GameExpression.GetInteractGenerate(m_GameStage);
        m_BattleDifficulty = enum_BattleDifficulty.Peaceful;
        m_LevelType = enum_TileType.Invalid;

        m_battleLevelEntered = 0;
        m_levelEntered = 0;
    }
    public void StageFinished()
    {
        m_GameStage++;
    }

    public void OnLevelChange(enum_TileType type, bool levelUnlocked)
    {
        m_LevelType = type;
        if (levelUnlocked)
            return;
        if (type == enum_TileType.Battle)
            m_battleLevelEntered ++ ;
        m_levelEntered++;
    }

    public bool WillBattle()
    {
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

    public void OnEntityKilled() => m_enermiesKilled++;
    public void OnGameFinished(bool win)=> m_gameWin = win;
    #region CalculateData
    public float F_Progress => GameExpression.GetResultProgress(m_gameWin, m_GameStage, m_battleLevelEntered);
    public float F_KillScore => GameExpression.GetResultKillScore(m_enermiesKilled);
    public float F_LevelScore => GameExpression.GetResultLevelScore(m_GameStage, m_levelEntered);
    public float F_DifficultyBonus => GameExpression.GetResultDifficultyBonus(m_GameDifficulty);
    public float F_FinalScore => (F_KillScore + F_LevelScore) *  F_DifficultyBonus;
    public float F_CreditGain => GameExpression.GetResultRewardCredits(F_FinalScore);
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
    public static void PresetRegistCommonObject()
    {
        TResources.GetAllEffectSFX().Traversal((int index, SFXBase target) => {
            ObjectPoolManager<int, SFXBase>.Register(index, target, 1,
            (SFXBase sfx) => { sfx.Init(index); });
        });

        TResources.GetCommonEntities().Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, 1,
                (EntityBase entityInstantiate) => { entityInstantiate.Init(index); });
        });
    }
    public static void RegisterLevelItem(Dictionary<LevelItemBase, int> registerDic)
    {
        registerDic.Traversal((LevelItemBase item, int count) => { ObjectPoolManager<LevelItemBase, LevelItemBase>.Register(item, GameObject.Instantiate(item), count, null); });
    }
    public static Dictionary<enum_CharacterType, List<int>> RegistStyledIngameEnermies(enum_Style currentStyle, enum_StageLevel stageLevel)
    {
        RegisterInGameInteractions(currentStyle, stageLevel);
        ObjectPoolManager<int, LevelBase>.Register(0, TResources.GetLevelBase(currentStyle), 1, (LevelBase level) => { level.Init(); });

        Dictionary<enum_CharacterType, List<int>> enermyDic = new Dictionary<enum_CharacterType, List<int>>();
        TResources.GetEnermyEntities(currentStyle).Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, 1,
                (EntityBase entityInstantiate) => { entityInstantiate.Init(index); });

            EntityCharacterAI enermy = entity as EntityCharacterAI;
            if (!enermyDic.ContainsKey(enermy.E_EnermyType))
                enermyDic.Add(enermy.E_EnermyType, new List<int>());
            enermyDic[enermy.E_EnermyType].Add(index);
        });
        return enermyDic;
    }
    static void RegisterInGameInteractions(enum_Style portalStyle, enum_StageLevel stageIndex)
    {
        ObjectPoolManager<enum_Interaction, InteractBase>.Register(enum_Interaction.Portal, TResources.GetInteractPortal(portalStyle), 5, (InteractBase interact) => { interact.Init(); });
        ObjectPoolManager<enum_Interaction, InteractBase>.Register(enum_Interaction.ActionChest, TResources.GetInteractActionChest(stageIndex), 5, (InteractBase interact) => { interact.Init(); });
        TCommon.TraversalEnum((enum_Interaction enumValue) =>
        {
            if (enumValue > enum_Interaction.GameBegin&&enumValue< enum_Interaction.GameEnd)
                ObjectPoolManager<enum_Interaction, InteractBase>.Register(enumValue, TResources.GetInteract(enumValue),5, (InteractBase interact) => { interact.Init(); });
        });
    }
    #endregion
    #region Spawn/Recycle
    #region Entity
    //Start Health 0:Use Preset I_MaxHealth
    static T SpawnEntity<T>(int _poolIndex, Vector3 toPos,enum_EntityFlag _flag, float _startHealth = 0, Transform parentTrans = null) where T:EntityBase
    {
        T entity = ObjectPoolManager<int, EntityBase>.Spawn(_poolIndex, TF_Entity) as T;
        if (entity == null)
            Debug.LogError("Entity ID:" + _poolIndex + ",Type:" + typeof(T).ToString() + " Not Found");
        entity.OnActivate(_flag,_startHealth);
        entity.gameObject.name = entity.I_EntityID.ToString() + "_" + _poolIndex.ToString();
        entity.transform.position = LevelManager.NavMeshPosition(toPos, true);
        if (parentTrans) entity.transform.SetParent(parentTrans);
        return entity;
    }
    static T SpawnEntityCharacter<T>(int poolIndex, Vector3 toPosition, enum_EntityFlag _flag, float _startHealth = 0, Transform parentTrans = null) where T:EntityCharacterBase => SpawnEntity<T>(poolIndex,toPosition,_flag,_startHealth,parentTrans);
    public static EntityCharacterAI SpawnAI(int index, Vector3 toPosition, enum_EntityFlag _flag)=> SpawnEntityCharacter<EntityCharacterAI>(index, toPosition, _flag);
    public static EntityCharacterBase SpawnSubCharacter(int index, Vector3 toPosition, int spanwer, enum_EntityFlag _flag, float _startHealth =0)
    {
        EntityCharacterBase entity = SpawnEntityCharacter<EntityCharacterBase>(index, toPosition, _flag,_startHealth,null);
        entity.SetSpawnerID(spanwer);
        return entity;
    }
    public static EntityCharacterPlayer SpawnEntityPlayer(CPlayerLevelSave playerSave)
    {
        EntityCharacterPlayer player = SpawnEntity<EntityCharacterPlayer>(0,Vector3.up*10f, enum_EntityFlag.Player);
        player.SetPlayerInfo(playerSave.m_coins,GameDataManager.CreateActions(playerSave.m_storedActions));
        player.ObtainWeapon(SpawnWeapon(playerSave.m_weapon,GameDataManager.CreateAction(playerSave.m_weaponAction)));
        return player;
    }
    public static EntityTrader SpawnTrader(int index, Vector3 toPosition, Transform attachTo) => SpawnEntity<EntityTrader>(index, toPosition, enum_EntityFlag.Neutal,0, attachTo);
    public static void RecycleEntity(int index, EntityBase target) => ObjectPoolManager<int, EntityBase>.Recycle(index, target);
    #endregion
    #region Weapon
    public static WeaponBase SpawnWeapon(enum_PlayerWeapon type,ActionBase perk,Transform toTrans=null)
    {
        if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(type))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(type);
            ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(type, preset, 1, (WeaponBase targetWeapon) => { targetWeapon.Init(GameDataManager.GetWeaponProperties(type)); });
        }
        WeaponBase weapon = ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(type, toTrans ? toTrans : TF_Entity);
        weapon.OnSpawn(perk);
        return weapon;
    }
    public static void RecycleWeapon(WeaponBase weapon)=> ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Recycle(weapon.m_WeaponInfo.m_Weapon,weapon);
    #endregion
    #region SFX
    public static T SpawnSFX<T>(int index) where T : SFXBase
    {
        T sfx = ObjectPoolManager<int, SFXBase>.Spawn(index,  TF_SFXPlaying) as T;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid SFX Type:" + typeof(T) + ",Index:" + index);
        return sfx;
    }

    public static T SpawnParticles<T>(int index, Vector3 position, Vector3 normal) where T : SFXParticles
    {
        T sfx = SpawnSFX<T>(index);
        sfx.transform.position = position;
        sfx.transform.rotation = Quaternion.LookRotation(normal);
        return sfx;
    }
    public static SFXIndicator SpawnIndicator(int _sourceID, Vector3 position, Vector3 normal) => SpawnParticles<SFXIndicator>(_sourceID, position, normal);
    public static SFXEffect SpawnBuffEffect(int _sourceID) => SpawnParticles<SFXEffect>(_sourceID,Vector3.zero,Vector3.up);
    public static void PlayMuzzle(int _sourceID,Vector3 position, Vector3 direction, int muzzleIndex, AudioClip muzzleClip=null)
    {
        if (muzzleIndex > 0)
            SpawnParticles<SFXMuzzle>(muzzleIndex, position, direction).Play(_sourceID);
        if (muzzleClip)
            AudioManager.Instance.PlayClip(_sourceID,muzzleClip,position , false);
    }
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
#endregion