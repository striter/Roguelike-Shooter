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
    public int WeaponPerkOnSpawn = -1;
    public int F5_TestActionNormal = 10001;
    public int F6_TestActionOutstanding = 10001;
    public int F7_TestActionEpic = 10001;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            SetBulletTime(!m_BulletTiming,.1f);

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Z) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
        {
            EntityCharacterBase enermy = GameObjectManager.SpawnEntityCharacter(Z_TestEntitySpawn, hit.point, TestEntityFlag);
            enermy.SetExtraDifficulty(GameExpression.GetAIBaseHealthMultiplier(m_GameLevel.m_GameDifficulty), GameExpression.GetAIMaxHealthMultiplier(m_GameLevel.m_GameStage), GameExpression.GetEnermyGameDifficultyBuffIndex(m_GameLevel.m_GameDifficulty));
            if (TestEntityBuffOnSpawn > 0)
                enermy.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic,DamageDeliverInfo.BuffInfo(-1, TestEntityBuffOnSpawn)));
        }
        if (Input.GetKeyDown(KeyCode.X) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnEquipment<SFXCast>(X_TestCastIndex, hit.point, CastForward?m_LocalPlayer.transform.forward: Vector3.up).Play(DamageDeliverInfo.Default(m_LocalPlayer.m_EntityID));
        if (Input.GetKeyDown(KeyCode.C) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnEquipment<SFXProjectile>(C_TestProjectileIndex, hit.point + Vector3.up, m_LocalPlayer.transform.forward).Play(DamageDeliverInfo.Default(m_LocalPlayer.m_EntityID), m_LocalPlayer.transform.forward, hit.point + m_LocalPlayer.transform.forward * 10);
        if (Input.GetKeyDown(KeyCode.V) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnIndicator(V_TestIndicatorIndex, hit.point + Vector3.up, Vector3.up).Play(-1,3f);
        if (Input.GetKeyDown(KeyCode.Comma) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnSFX<SFXParticles>(Comma_TestParticleIndex, hit.point + Vector3.up, Vector3.up).Play(-1);
        if (Input.GetKeyDown(KeyCode.B))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.BuffInfo(-1, B_TestBuffIndex )));
        if (Input.GetKeyDown(KeyCode.N))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(20, enum_DamageType.Basic,DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.M))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-50, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.F1) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
        {
            GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, hit.point, LevelManager.Instance.m_InteractParent).Play(GameObjectManager.SpawnWeapon(F1_WeaponSpawnType, WeaponPerkOnSpawn>0?ActionDataManager.CreateAction(WeaponPerkOnSpawn, enum_ActionRarity.Normal):null));
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            List<EntityBase> entities = m_Entities.Values.ToList();
            entities.Traversal((EntityBase entity) => {
                if (entity.m_Flag== enum_EntityFlag.Enermy)
                    entity.m_HitCheck.TryHit( new DamageInfo(entity.m_Health.m_MaxHealth, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
            });
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            m_Entities.Traversal((EntityBase entity) => {
                if (entity.m_Flag == enum_EntityFlag.Enermy)
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.EquipmentInfo(-1,0, enum_CharacterEffect.Freeze,2f)));
            });
        }

        if (Input.GetKeyDown(KeyCode.Equals))
            OnStageFinished();
        if (Input.GetKeyDown(KeyCode.Minus))
            OnLevelFinished();

        if (Input.GetKeyDown(KeyCode.F5) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractAction>(enum_Interaction.Action, hit.point).Play(ActionDataManager.CreateAction(F5_TestActionNormal, enum_ActionRarity.Normal));
        if (Input.GetKeyDown(KeyCode.F6) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractAction>(enum_Interaction.Action, hit.point).Play(ActionDataManager.CreateAction(F6_TestActionOutstanding, enum_ActionRarity.OutStanding));
        if (Input.GetKeyDown(KeyCode.F7) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractAction>(enum_Interaction.Action, hit.point).Play(ActionDataManager.CreateAction(F7_TestActionEpic, enum_ActionRarity.Epic));

        if (Input.GetKeyDown(KeyCode.Keypad1) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupCoin, LevelManager.NavMeshPosition(hit.point, false), LevelManager.Instance.m_InteractParent).Play(1, m_LocalPlayer.transform);
        if (Input.GetKeyDown(KeyCode.Keypad2) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupCoin, LevelManager.NavMeshPosition(hit.point, false), LevelManager.Instance.m_InteractParent).Play(10, m_LocalPlayer.transform);
        if (Input.GetKeyDown(KeyCode.Keypad3) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupCoin, LevelManager.NavMeshPosition(hit.point, false), LevelManager.Instance.m_InteractParent).Play(100, m_LocalPlayer.transform);
        if (Input.GetKeyDown(KeyCode.Keypad4) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupArmor, LevelManager.NavMeshPosition(hit.point, false), LevelManager.Instance.m_InteractParent).Play(10, m_LocalPlayer.transform);
        if (Input.GetKeyDown(KeyCode.Keypad5) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
            GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupHealth, LevelManager.NavMeshPosition(hit.point, false), LevelManager.Instance.m_InteractParent).Play(10, m_LocalPlayer.transform);

        if (Input.GetKeyDown(KeyCode.F9))
        {
            CameraController.Instance.m_Effect.StartAreaScan(m_LocalPlayer.tf_Head.position, Color.white, TResources.Load<Texture>(TResources.ConstPath.S_PETex_Holograph), 15f, 1f, 5f, 50, 1f);
            m_Entities.Traversal((EntityBase entity) => {
                if (entity.m_Flag == enum_EntityFlag.Enermy)
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.EquipmentInfo(-1, 0, enum_CharacterEffect.Scan, 10f)));
            });
        }
        if (Input.GetKeyDown(KeyCode.F10))
            Debug.LogError(m_GameLevel.m_Seed);
    }
    #endregion
#endif
    public GameLevelManager m_GameLevel { get; private set; }
    public EntityCharacterPlayer m_LocalPlayer { get; private set; } = null;
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
        if (M_TESTSEED!="")
            GameDataManager.m_BattleData.m_GameSeed = M_TESTSEED;
        m_GameLevel =  new GameLevelManager(GameDataManager.m_GameData,GameDataManager.m_BattleData);
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
        LoadStage();
    }
    public void OnExitGame()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameExit);
        LoadingManager.Instance.ShowLoading( enum_StageLevel.Invalid);
        SwitchScene( enum_Scene.Camp,()=> { LoadingManager.Instance.EndLoading();return true; });
    }

    #region Level Management
    //Call When Level Changed
    void LoadStage()
    {
        LoadingManager.Instance.ShowLoading(m_GameLevel.m_GameStage);
        this.StartSingleCoroutine(999, DoLoadStage());
    } 
    IEnumerator DoLoadStage()     //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageBeginLoad);
        GameObjectManager.RecycleAllObject();
        yield return null;
        m_GameLevel.GetStageData();
        EntityReset();
        GC.Collect();
        Resources.UnloadUnusedAssets();
        yield return null;
        GameObjectManager.PresetRegistCommonObject();
        yield return null;
        m_LocalPlayer = GameObjectManager.SpawnEntityPlayer(GameDataManager.m_BattleData);
        CameraController.Instance.Attach(m_LocalPlayer.transform, true);
        m_Enermies = GameObjectManager.RegistStyledIngameEnermies(m_GameLevel.m_GameStyle, m_GameLevel.m_GameStage);
        yield return null;
        InitPostEffects(m_GameLevel.m_GameStyle);
        yield return LevelManager.Instance.GenerateLevel(m_GameLevel.m_GameStyle, m_GameLevel.m_GameSeed);
        OnPortalExit(1f, m_LocalPlayer.tf_Head);
        LoadingManager.Instance.EndLoading();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageStart);
        LevelManager.Instance.GameStart(OnLevelChanged);
    }
    public void OnLevelFinished()
    {
        this.StopAllSingleCoroutines();
        if (m_GameLevel.m_LevelType == enum_LevelType.End)
            OnStageFinished();
        else
            LevelManager.Instance.LoadNextLevel(OnLevelChanged);
    } 
    void OnLevelChanged(SBigmapLevelInfo levelInfo)
    {
        GameObjectManager.RecycleAllWeapon(null);
        GameObjectManager.RecycleAllInteract();
        SpawnLevelInteracts(levelInfo);

        Vector3 randomPositon = levelInfo.m_Level.RandomEmptyTilePosition(m_GameLevel.m_GameSeed);
        m_LocalPlayer.SetSpawnPosRot(randomPositon, Quaternion.LookRotation(-randomPositon, Vector3.up));

        m_GameLevel.OnLevelChange(levelInfo.m_LevelType);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnChangeLevel);
        if (m_GameLevel.WillBattle())
            OnBattleStart();
    }

    void OnStageFinished()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageFinish);
        if (m_GameLevel.StageFinished())
        {
            OnGameFinished(true);
            return;
        }
        GameDataManager.AdjustInGameData(m_LocalPlayer, m_GameLevel);
        OnPortalEnter(1f, m_LocalPlayer.tf_Head, LoadStage);
    }

    void OnGameFinished(bool win)
    {
        m_GameLevel.OnGameFinished(win);
        GameDataManager.OnGameFinished(win);
        GameDataManager.OnCreditStatus(m_GameLevel.F_CreditGain);
        GameUIManager.Instance.OnGameFinished(m_GameLevel, OnExitGame);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameFinish, win);
    }
    #endregion
    #region InteractManagement
    void SpawnLevelInteracts(SBigmapLevelInfo level)
    {
        Transform interactTrans = level.m_Level.tf_Interact;
        switch(level.m_LevelType)
        {
            case enum_LevelType.BattleActionAcquire:
            case enum_LevelType.BattlePerkUpgrade:
            case enum_LevelType.Trade:
            case enum_LevelType.ActionAdjustment:
            case enum_LevelType.Start:
                GameObjectManager.SpawnInteract<InteractPortal>(enum_Interaction.Portal, level.m_Level.RandomEmptyTilePosition(m_GameLevel.m_GameSeed), level.m_Level.tf_Interact).Play(OnLevelFinished, false);
                break;
        }

        switch (level.m_LevelType)
        {
            case enum_LevelType.Start:
                {
                    GameObjectManager.SpawnInteract<InteractBonfire>(enum_Interaction.Bonfire,Vector3.zero, interactTrans).Play(m_LocalPlayer);
                }
                break;
            case enum_LevelType.Trade:
                {
                    GameObjectManager.SpawnNPC( enum_InteractCharacter.Trader, Vector3.back *2f, interactTrans);

                    int priceHealth = GameExpression.GetTradePrice(enum_Interaction.PickupHealth).Random(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade, Vector3.left * 1.5f + Vector3.forward * 1f, interactTrans).Play(priceHealth, GameObjectManager.SpawnInteract<InteractPickupHealth>(enum_Interaction.PickupHealthPack, Vector3.zero, interactTrans).Play(GameConst.I_HealthTradeAmount, null));

                    ActionBase action1 = ActionDataManager.CreateRandomAction(m_GameLevel.m_actionGenerate.GetTradeActionRarity(m_GameLevel.m_GameSeed), m_GameLevel.m_GameSeed);
                    int priceAction = GameExpression.GetTradePrice(enum_Interaction.Action, action1.m_rarity).Random(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade,Vector3.forward*1f, interactTrans).Play(priceAction, GameObjectManager.SpawnInteract<InteractAction>(enum_Interaction.Action, Vector3.zero, interactTrans).Play(action1));
                    
                    WeaponBase weapon = GameObjectManager.SpawnWeapon(GameDataManager.m_WeaponRarities[m_GameLevel.m_actionGenerate.GetTradeWeaponRarity(m_GameLevel.m_GameSeed)].RandomItem(m_GameLevel.m_GameSeed),null);
                    int priceWeapon = GameExpression.GetTradePrice(enum_Interaction.Weapon, enum_ActionRarity.Invalid,weapon.m_WeaponInfo.m_Rarity).Random(m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerTrade>(enum_Interaction.ContainerTrade,Vector3.right * 1.5f + Vector3.forward * 1f, interactTrans).Play(priceWeapon, GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, Vector3.right, interactTrans).Play(weapon));
                }
                break;
            case enum_LevelType.ActionAdjustment:
                {
                    GameObjectManager.SpawnNPC( enum_InteractCharacter.Trainer, Vector3.zero , interactTrans);
                    GameObjectManager.SpawnInteract<InteractActionAdjustment>(enum_Interaction.ActionAdjustment,Vector3.zero, interactTrans).Play(m_GameLevel.m_GameStage);
                }
                break;
            case enum_LevelType.BattleActionAcquire:
                {
                    ActionBase action = ActionDataManager.CreateRandomAction(m_GameLevel.m_GameStage.GetBattleTradeActionRarity(), m_GameLevel.m_GameSeed);
                    GameObjectManager.SpawnInteract<InteractContainerBattle>(enum_Interaction.ContainerBattle, Vector3.zero, interactTrans).Play(OnBattleStart, GameObjectManager.SpawnInteract<InteractAction>(enum_Interaction.Action, Vector3.zero, interactTrans).Play(action));
                }
                break;
            case enum_LevelType.BattlePerkUpgrade:
                {
                    GameObjectManager.SpawnInteract<InteractPerkUpgrade>(enum_Interaction.PerkUpgrade, Vector3.zero, interactTrans).Play(OnBattleStart,ActionDataManager.CreateRandomWeaponAbilityAction( enum_ActionRarity.Normal,m_GameLevel.m_GameSeed));
                }
                break;
        }
    }

    void SpawnBattleEndPortals(Vector3 rewardPos)
    {
        switch(m_GameLevel.m_LevelType)
        {
            case enum_LevelType.End:
            case enum_LevelType.Battle:
                {
                    GameObjectManager.SpawnInteract<InteractPortal>(enum_Interaction.Portal, LevelManager.NavMeshPosition(rewardPos, false), LevelManager.Instance.m_currentLevel.m_Level.tf_Interact).Play(OnLevelFinished, true);
                }
                break;
        }
    }

    void SpawnEntityDeadPickups(EntityCharacterBase entity)
    {
        if (entity.m_Flag != enum_EntityFlag.Enermy)
            return;
        EntityCharacterAI target = entity as EntityCharacterAI;

        if (target.E_EnermyType == enum_EnermyType.SubHidden)
            return;

        PickupGenerateData pickupGenerateData = target.E_EnermyType == enum_EnermyType.Elite ? m_GameLevel.m_actionGenerate.m_ElitePickupData : m_GameLevel.m_actionGenerate.m_NormalPickupData;

        if (pickupGenerateData.CanGenerateHealth(target.E_EnermyType))
            GameObjectManager.SpawnInteract<InteractPickupHealth>(enum_Interaction.PickupHealth, GetPickupPosition(entity)).Play(GameConst.I_HealthPickupAmount, m_LocalPlayer.transform);

        if (pickupGenerateData.CanGenerateArmor(target.E_EnermyType))
            GameObjectManager.SpawnInteract<InteractPickupArmor>(enum_Interaction.PickupArmor, GetPickupPosition(entity)).Play(GameConst.I_ArmorPickupAmount,m_LocalPlayer.transform);
        
        int coinAmount = pickupGenerateData.GetCoinGenerate(target.E_EnermyType);
        if (coinAmount != -1)
            GameObjectManager.SpawnInteract<InteractPickupCoin>(enum_Interaction.PickupCoin, GetPickupPosition(entity)).Play(coinAmount,m_LocalPlayer.transform);

        enum_WeaponRarity weaponRarity = TCommon.RandomPercentage(pickupGenerateData.m_WeaponRate, enum_WeaponRarity.Invalid);
        if (weaponRarity != enum_WeaponRarity.Invalid)
            GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, GetPickupPosition(entity)).Play(GameObjectManager.SpawnWeapon(GameDataManager.m_WeaponRarities[weaponRarity].RandomItem(),null));

        enum_ActionRarity actionRarity = TCommon.RandomPercentage(pickupGenerateData.m_ActionRate, enum_ActionRarity.Invalid);
        if (actionRarity != enum_ActionRarity.Invalid)
            GameObjectManager.SpawnInteract<InteractAction>( enum_Interaction.Action, GetPickupPosition(entity)).Play(ActionDataManager.CreateRandomAction(actionRarity,null));
    }
    Vector3 GetPickupPosition(EntityCharacterBase dropper) => LevelManager.NavMeshPosition(dropper.transform.position + TCommon.RandomXZSphere(1.5f), false);
    #endregion
    #region Entity Management
    Dictionary<int, EntityBase> m_Entities = new Dictionary<int, EntityBase>();
    Dictionary<enum_EntityFlag, List<EntityCharacterBase>> m_AllyEntities = new Dictionary<enum_EntityFlag, List<EntityCharacterBase>>();
    Dictionary<enum_EntityFlag, List<EntityCharacterBase>> m_OppositeEntities = new Dictionary<enum_EntityFlag, List<EntityCharacterBase>>();
    public int m_FlagEntityCount(enum_EntityFlag flag) => m_AllyEntities[flag].Count;
    public List<EntityCharacterBase> GetEntities(enum_EntityFlag sourceFlag, bool getAlly) => getAlly ? m_AllyEntities[sourceFlag] : m_OppositeEntities[sourceFlag];
    public bool EntityExists(int entityID) => m_Entities.ContainsKey(entityID);
    public EntityBase GetEntity(int entityID)
    {
        if (!EntityExists(entityID))
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

    void EntityReset()
    {
        m_Entities.Clear();
        TCommon.TraversalEnum((enum_EntityFlag flag) => {
            m_AllyEntities[flag].Clear();
            m_OppositeEntities[flag].Clear();
        });
    }

    void OnEntiyActivate(EntityBase entity)
    {
        m_Entities.Add(entity.m_EntityID, entity);
        if (entity.m_Controller== enum_EntityController.None)
            return;
        EntityCharacterBase character = entity as EntityCharacterBase;
        m_AllyEntities[entity.m_Flag].Add(character);
        m_OppositeEntities.Traversal((enum_EntityFlag flag)=> {
            if (entity.m_Flag != enum_EntityFlag.Neutal && flag != entity.m_Flag)
                m_OppositeEntities[flag].Add(character);
        });
    }
    void OnEntityRecycle(EntityBase entity)
    {
        m_Entities.Remove(entity.m_EntityID);
        if (entity.m_Controller == enum_EntityController.None)
            return;
        EntityCharacterBase character = entity as EntityCharacterBase;
        m_AllyEntities[entity.m_Flag].Remove(character);
        m_OppositeEntities.Traversal((enum_EntityFlag flag) => {
            if (entity.m_Flag != enum_EntityFlag.Neutal && flag != entity.m_Flag)
                m_OppositeEntities[flag].Remove(character);
        });

        OnBattleCharacterRecycle(character);
        if (entity.m_Controller == enum_EntityController.Player)
            OnGameFinished(false);
    }

    void OnCharacterDead(EntityCharacterBase character)
    {
        if (character.m_Controller == enum_EntityController.Player)
        {
            SetPostEffect_Dead();
            return;
        }

        SpawnEntityDeadPickups(character);
    }

    void OnCharacterRevive(EntityCharacterBase character)
    {
        if (character.m_Controller == enum_EntityController.Player)
        {
            SetPostEffect_Revive();
            return;
        }
    }

    #endregion
    #region SFXHitCheck
    public static bool B_CanSFXHitTarget(HitCheckBase hitCheck, int sourceID)    //If Match Will Hit Target
    {
        bool canHit = hitCheck.I_AttacherID != sourceID;
        if (hitCheck.m_HitCheckType == enum_HitCheck.Entity)    //Entity Special Check
            return canHit && B_CanSFXHitEntity(hitCheck as HitCheckEntity, sourceID);
        return canHit;
    }

    static bool B_CanSFXHitEntity(HitCheckEntity targetHitCheck, int sourceID)    //If Match Will Hit Entity
    {
        if (targetHitCheck.I_AttacherID == sourceID || targetHitCheck.m_Attacher.m_Flag == enum_EntityFlag.Neutal)
            return false;
        return !Instance.m_Entities.ContainsKey(sourceID) || targetHitCheck.m_Attacher.m_Flag != Instance.m_Entities[sourceID].m_Flag;
    }
   
    public static bool B_CanSFXDamageEntity(HitCheckEntity hb, int sourceID)    //After Hit,If Match Target Hit Succeed
    {
        if (!Instance.B_Battling||hb.I_AttacherID == sourceID || !Instance.m_Entities.ContainsKey(sourceID))
            return false;

        return hb.m_Attacher.m_Flag != Instance.GetEntity(sourceID).m_Flag;
    }
    #endregion
    #region Revive Management
    Action OnRevivePlay;
    public void CheckRevive(Action _OnRevivePlayer)
    {
        OnRevivePlay = _OnRevivePlayer;
        UIManager.Instance.ShowPage<UI_Revive>(true, 0f).Play(_OnRevivePlayer, OnCreditRevivePlayer, 50,GameDataManager.CanUseCredit);
    }
    void OnCreditRevivePlayer()
    {
        GameDataManager.OnCreditStatus(-50);
        OnRevivePlay();
    }
    #endregion
    #region Battle Management
    public bool B_Battling { get; private set; } = false;
    public bool B_WaveEntityGenerating { get; private set; } = false;
    public int m_CurrentWave { get; private set; } = -1;
    public List<SGenerateEntity> m_EntityGenerate { get; private set; } = new List<SGenerateEntity>();
    public List<int> m_EntityGenerating { get; private set; } = new List<int>();
    public Dictionary<enum_EnermyType, List<int>> m_Enermies;
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
        bool finalWave = m_CurrentWave + 1==m_EntityGenerate.Count ;
        UIManager.Instance.m_Indicates.ShowWarning("UI_Indicates_EnermyApproching","UI_Indicates_Wave",(m_CurrentWave+1).ToString(),finalWave?"UI_Indicates_FinalWave":"",3f);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnWaveStart, m_EntityGenerate.Count, m_CurrentWave,finalWave);

        m_EntityGenerating.Clear();
        m_EntityGenerate[m_CurrentWave].m_EntityGenerate.Traversal((enum_EnermyType level, RangeInt range) =>
        {
            int spawnCount = range.Random();
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
        this.StartSingleCoroutine(99, IE_GenerateEnermy(m_EntityGenerating, GameConst.F_EnermySpawnOffsetEach));
    }

    void OnBattleCharacterRecycle(EntityCharacterBase entity)
    {
        if (!B_Battling || B_WaveEntityGenerating || entity.m_Flag != enum_EntityFlag.Enermy)
            return;
        
        if (m_FlagEntityCount( enum_EntityFlag.Enermy) <= 0 || (m_CurrentWave < m_EntityGenerate.Count && m_FlagEntityCount(enum_EntityFlag.Enermy) <= GameConst.I_EnermyCountWaveFinish))
            WaveFinished(entity.transform.position);
    }

    void WaveFinished(Vector3 lastEntityPos)
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnWaveFinish);
        m_CurrentWave++;
        if (m_CurrentWave < m_EntityGenerate.Count)
        {
            WaveStart();
            return;
        }

        B_Battling = false;
        SpawnBattleEndPortals(lastEntityPos);
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
            GameObjectManager.SpawnEntityCharacter(entityIndex,position , enum_EntityFlag.Enermy).SetExtraDifficulty(baseHealthMultiplier, maxHealthMultiplier,difficultyBuff);
        }));
    }
    #endregion
}

#region External Tools Packaging Class
public class GameLevelManager
{
    #region LevelData
    public string m_Seed { get; private set; }
    public int m_GameDifficulty { get; private set; }

    public System.Random m_GameSeed { get; private set; }
    public StageInteractGenerateData m_actionGenerate { get; private set; }
    public bool B_IsFinalLevel => B_IsFinalStage&&m_LevelType == enum_LevelType.End;
    public bool B_IsFinalStage => m_GameStage == enum_StageLevel.Ranger;
    public bool B_IsFirstLevel => m_GameStage == enum_StageLevel.Rookie && m_LevelType == enum_LevelType.Start;
    public enum_LevelType m_LevelType { get; private set; }
    public enum_StageLevel m_GameStage { get; private set; }
    Dictionary<enum_StageLevel, enum_Style> m_StageStyle = new Dictionary<enum_StageLevel, enum_Style>();
    public enum_Style m_GameStyle => m_StageStyle[m_GameStage];
    static enum_BattleDifficulty m_BattleDifficulty;
    public enum_BattleDifficulty m_Difficulty
    {
        get
        {
            switch (m_LevelType)
            {
                default:
                    return enum_BattleDifficulty.Peaceful;
                case enum_LevelType.BattlePerkUpgrade:
                case enum_LevelType.BattleActionAcquire:
                    return enum_BattleDifficulty.BattleReward;
                case enum_LevelType.End:
                    return enum_BattleDifficulty.End;
                case enum_LevelType.Battle:
                    return m_BattleDifficulty;
            }
        }
    }
    #endregion
    #region RecordData
    public bool m_gameWin { get; private set; }
    int m_levelEntered;
    int m_battleLevelEntered;
    #endregion
    public GameLevelManager(CGameSave _gameSave,CBattleSave _battleSave)
    {
        m_Seed =_battleSave.m_GameSeed;
        m_GameSeed = new System.Random(m_Seed.GetHashCode());
        m_GameStage = _battleSave.m_Stage;
        m_GameDifficulty = _gameSave.m_GameDifficulty;
        List<enum_Style> styleList = TCommon.GetEnumList<enum_Style>();
        TCommon.TraversalEnum((enum_StageLevel level) => {
            enum_Style style = styleList.RandomItem(m_GameSeed);
            styleList.Remove(style);
            m_StageStyle.Add(level, style);
        });
    }
    public void GetStageData()
    {
        m_actionGenerate = GameExpression.GetInteractGenerate(m_GameStage);
        m_BattleDifficulty = enum_BattleDifficulty.Peaceful;
        m_LevelType = enum_LevelType.Invalid;

        m_battleLevelEntered = 0;
        m_levelEntered = 0;
    }
    public bool StageFinished()
    {
        if (B_IsFinalStage)
            return true;
        m_GameStage++;
        return false;
    }

    public void OnLevelChange(enum_LevelType type)
    {
        m_LevelType = type;
        if (type == enum_LevelType.Battle)
            m_battleLevelEntered ++ ;
        m_levelEntered++;
    }

    public bool WillBattle()
    {
        switch (m_LevelType)
        {
            default:
                return false;
            case enum_LevelType.Battle:
                if (m_BattleDifficulty < enum_BattleDifficulty.Hard)
                    m_BattleDifficulty++;
                return true;
            case enum_LevelType.End:
                return true;
        }
    }
    
    public void OnGameFinished(bool win)=> m_gameWin = win;
    #region CalculateData
    public float F_Completion => GameExpression.GetResultCompletion(m_gameWin, m_GameStage, m_battleLevelEntered);
    public float F_CompletionScore => GameExpression.GetResultLevelScore(m_GameStage, m_levelEntered);
    public float F_DifficultyBonus => GameExpression.GetResultDifficultyBonus(m_GameDifficulty);
    public float F_FinalScore =>  F_CompletionScore *  F_DifficultyBonus;
    public float F_CreditGain => GameExpression.GetResultRewardCredits(F_FinalScore);
    #endregion
}
public static class GameObjectManager
{
    static Transform TF_Entity;
    static Transform TF_SFXPlaying;
    static Transform TF_SFXWeapon;
    public static Transform TF_SFXWaitForRecycle { get; private set; }
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;
        TF_SFXWaitForRecycle = new GameObject("SFX_WaitForRecycle").transform;
        TF_SFXPlaying = new GameObject("SFX_CommonPlaying").transform;
        TF_SFXWeapon = new GameObject("SFX_Weapon").transform;
        ObjectPoolManager.Init();
    }
    public static void RecycleAllObject()
    {
        ObjectPoolManager<int, SFXBase>.DestroyAll();
        ObjectPoolManager<int, SFXEquipmentBase>.DestroyAll();
        ObjectPoolManager<int, EntityBase>.DestroyAll();
        ObjectPoolManager<enum_Interaction, InteractGameBase>.DestroyAll();
        ObjectPoolManager<LevelItemBase, LevelItemBase>.DestroyAll();
        ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.DestroyAll();
        ObjectPoolManager<int, LevelBase>.DestroyAll();
    }
    #region Register
    public static void PresetRegistCommonObject()
    {
        TResources.GetAllEffectSFX().Traversal((int index, SFXBase target) => {ObjectPoolManager<int, SFXBase>.Register(index, target, 1); });
        TResources.GetCommonEntities().Traversal((int index, EntityBase entity) => { ObjectPoolManager<int, EntityBase>.Register(index, entity, 1); });
    }
    public static Dictionary<enum_LevelItemType,List<LevelItemBase>> RegisterLevelItem(enum_Style _style)
    {
        Dictionary<enum_LevelItemType, List<LevelItemBase>> items = TResources.GetAllLevelItems(_style, null);
        items.Traversal((enum_LevelItemType type,List< LevelItemBase> levelItems) => { levelItems.Traversal((LevelItemBase item) => { ObjectPoolManager<LevelItemBase, LevelItemBase>.Register(item, GameObject.Instantiate(item), 1); }); });
        return items;
    }
    public static Dictionary<enum_EnermyType, List<int>> RegistStyledIngameEnermies(enum_Style currentStyle, enum_StageLevel stageLevel)
    {
        RegisterInGameInteractions(currentStyle, stageLevel);
        ObjectPoolManager<int, LevelBase>.Register(0, TResources.GetLevelBase(currentStyle), 1);

        Dictionary<enum_EnermyType, List<int>> enermyDic = new Dictionary<enum_EnermyType, List<int>>();
        TResources.GetEnermyEntities(currentStyle).Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, 1 );

            EntityCharacterAI enermy = entity as EntityCharacterAI;
            if (!enermyDic.ContainsKey(enermy.E_EnermyType))
                enermyDic.Add(enermy.E_EnermyType, new List<int>());
            enermyDic[enermy.E_EnermyType].Add(index);
        });
        return enermyDic;
    }
    static void RegisterInGameInteractions(enum_Style portalStyle, enum_StageLevel stageIndex)
    {
        TCommon.TraversalEnum((enum_Interaction enumValue) =>
        {
            if (enumValue > enum_Interaction.GameBegin&&enumValue< enum_Interaction.GameEnd)
                ObjectPoolManager<enum_Interaction, InteractGameBase>.Register(enumValue, TResources.GetInteract(enumValue),5);
        });
    }
    #endregion
    #region Spawn/Recycle
    #region Entity
    //Start Health 0:Use Preset I_MaxHealth
    static T SpawnEntity<T>(int _poolIndex, Vector3 toPos,enum_EntityFlag _flag,int spawnerID, float _startHealth, Transform parentTrans = null) where T:EntityBase
    {
        T entity = ObjectPoolManager<int, EntityBase>.Spawn(_poolIndex, TF_Entity) as T;
        if (entity == null)
            Debug.LogError("Entity ID:" + _poolIndex + ",Type:" + typeof(T).ToString() + " Not Found");
        entity.OnActivate(_flag,spawnerID,_startHealth);
        entity.gameObject.name = entity.m_EntityID.ToString() + "_" + _poolIndex.ToString();
        entity.transform.position = LevelManager.NavMeshPosition(toPos, true);
        if (parentTrans) entity.transform.SetParent(parentTrans);
        return entity;
    }
    public static EntityCharacterBase SpawnEntityCharacter(int poolIndex, Vector3 toPosition, enum_EntityFlag _flag,int spawnerID=-1, float _startHealth = 0, Transform parentTrans = null) => SpawnEntity<EntityCharacterBase>(poolIndex,toPosition,_flag,spawnerID,_startHealth,parentTrans);
    public static EntityCharacterPlayer SpawnEntityPlayer(CBattleSave playerSave)
    {
        EntityCharacterPlayer player = SpawnEntity<EntityCharacterPlayer>((int)playerSave.m_character,Vector3.up*10f, enum_EntityFlag.Player,-1,0);
        player.SetPlayerInfo(playerSave);
        return player;
    }
    public static EntityNPC SpawnNPC(enum_InteractCharacter npc, Vector3 toPosition, Transform attachTo) => SpawnEntity<EntityNPC>((int)npc, toPosition, enum_EntityFlag.Neutal,-1,0, attachTo);
    public static void RecycleEntity(int index, EntityBase target) => ObjectPoolManager<int, EntityBase>.Recycle(index, target);
    #endregion
    #region Weapon
    public static WeaponBase SpawnWeapon(enum_PlayerWeapon weapon,ActionBase action,Transform toTrans=null)
    {
        if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(weapon))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(weapon);
            ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(weapon, preset, 1);
        }
        WeaponBase targetWeapon = ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(weapon, toTrans ? toTrans : TF_Entity);
        targetWeapon.SetWeaponAction(action);
        return targetWeapon;
    }
    public static void RecycleWeapon(WeaponBase weapon)=> ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Recycle(weapon.m_WeaponInfo.m_Weapon,weapon);
    #endregion
    #region SFX
    public static T SpawnSFX<T>(int index, Vector3 position, Vector3 normal) where T : SFXBase
    {
        T sfx = ObjectPoolManager<int, SFXBase>.Spawn(index, TF_SFXPlaying) as T;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid SFX Type:" + typeof(T) + ",Index:" + index);
        sfx.transform.position = position;
        sfx.transform.rotation = Quaternion.LookRotation(normal);
        return sfx;
    }
    public static SFXIndicator SpawnIndicator(int _sourceID, Vector3 position, Vector3 normal) => SpawnSFX<SFXIndicator>(_sourceID, position, normal);
    public static SFXEffect SpawnBuffEffect(int _sourceID) => SpawnSFX<SFXEffect>(_sourceID,Vector3.zero,Vector3.up);

    public static void PlayMuzzle(int _sourceID,Vector3 position, Vector3 direction, int muzzleIndex, AudioClip muzzleClip=null)
    {
        if (muzzleIndex > 0)
            SpawnSFX<SFXMuzzle>(muzzleIndex, position, direction).Play(_sourceID);
        if (muzzleClip)
            AudioManager.Instance.PlayClip(_sourceID, muzzleClip, false, position);
    }

    public static T SpawnEquipment<T>(int weaponIndex, Vector3 position, Vector3 normal) where T : SFXEquipmentBase
    {
        if (!ObjectPoolManager<int, SFXEquipmentBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXEquipmentBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1);

        T template = ObjectPoolManager<int, SFXEquipmentBase>.Spawn(weaponIndex, TF_SFXWeapon) as T;
        if (template == null)
            Debug.LogError("Enermy Weapon Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        template.transform.position = position;
        template.transform.rotation = Quaternion.LookRotation(normal);
        return template;
    }
    public static T GetEquipmentData<T>(int weaponIndex) where T : SFXEquipmentBase
    {
        if (!ObjectPoolManager<int, SFXEquipmentBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXEquipmentBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1);

        T damageSourceInfo = ObjectPoolManager<int, SFXEquipmentBase>.GetRegistedSpawnItem(weaponIndex) as T;
        if (damageSourceInfo == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return damageSourceInfo;
    }
    public static void RecycleAllWeapon(Predicate<SFXEquipmentBase> predicate) => ObjectPoolManager<int, SFXEquipmentBase>.RecycleAll(predicate);
    #endregion
    #region Interact
    public static T SpawnInteract<T>(enum_Interaction type, Vector3 toPos, Transform toTrans=null) where T : InteractGameBase
    {
        T target = ObjectPoolManager<enum_Interaction, InteractGameBase>.Spawn(type , toTrans==null? TF_SFXPlaying : toTrans) as T;
        target.transform.position = toPos;
        return target;
    }
    public static void RecycleInteract(InteractGameBase target) => ObjectPoolManager<enum_Interaction, InteractGameBase>.Recycle(target.m_InteractType,target);
    public static void RecycleAllInteract() => ObjectPoolManager<enum_Interaction, InteractGameBase>.RecycleAll();
    #endregion
    #region Level/LevelItem
    public static LevelBase SpawnLevelPrefab(Transform toTrans)=>ObjectPoolManager<int, LevelBase>.Spawn(0, toTrans);
    public static LevelItemBase SpawnLevelItem(LevelItemBase itemObject, Transform itemParent, Vector3 localPosition)
    {
        LevelItemBase spawnedItem = ObjectPoolManager<LevelItemBase, LevelItemBase>.Spawn(itemObject, itemParent);
        spawnedItem.transform.localPosition = localPosition;
        return spawnedItem;
    }
    #endregion
    #endregion
}
#endregion