using GameSetting;
using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TTiles;
using UnityEngine;
using System.Threading.Tasks;
public class GameManager : GameManagerBase
{
    #region Test
    public bool B_PhysicsDebugGizmos = true;
    protected override void AddConsoleBinding()
    {
        base.AddConsoleBinding();
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Show Seed", KeyCode.None, () => { Debug.LogError(m_GameLevel.m_GameSeed); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Next Level", KeyCode.Minus, () => { OnChunkPortalEnter(m_GameLevel.GetNextStageGenerate()); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Next Stage", KeyCode.Equals, () => { OnChunkPortalEnter(m_GameLevel.m_FinalStage ? enum_GamePortalType.GameWin : enum_GamePortalType.StageEnd); });

        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Signal Tower", KeyCode.None, () => { GameObjectManager.TraversalAllInteracts((InteractGameBase interact) => { if (interact.m_InteractType == enum_Interaction.SignalTower) m_LocalPlayer.Teleport(NavigationManager.NavMeshPosition( interact.transform.position),interact.transform.rotation); }); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Clear Fog", KeyCode.None, () => { GameLevelManager.Instance.ClearAllFog();});
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Pass One Minutes", KeyCode.None, () => { m_GameLevel.Tick(60f,m_LocalPlayer.transform.position); });

        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Kill All", KeyCode.Alpha0, () => {
            GetCharacters(enum_EntityFlag.Enermy, true).Traversal((EntityCharacterBase character) =>
            {
                character.m_HitCheck.TryHit(new DamageInfo(-1, character.m_Health.m_CurrentHealth, enum_DamageType.Basic));
            });
        });

        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Enermy", KeyCode.Z, "101", (string id) => {
            GameObjectManager.SpawnEntityCharacterAI(int.Parse(id), NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity, enum_EntityFlag.Enermy,m_GameLevel.m_MinutesElapsed, m_GameLevel.m_GameDifficulty,false);
        });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Damage", KeyCode.N, "20", (string damage) => { m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-1, int.Parse(damage), enum_DamageType.Basic)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Heal", KeyCode.M, "20", (string damage) => { m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-1, -int.Parse(damage), enum_DamageType.Basic)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Perk Item", KeyCode.F1, "1", (string actionIndex) => { GameObjectManager.SpawnInteract<InteractPerkPickup>(NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity).Play(int.Parse(actionIndex)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Coins", KeyCode.F5, "20", (string coins) => { GameObjectManager.SpawnInteract<InteractPickupCoin>(NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity).Play(int.Parse(coins)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Health", KeyCode.F6, "20", (string health) => { GameObjectManager.SpawnInteract<InteractPickupHealth>(NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity).Play(int.Parse(health)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Armor", KeyCode.F7, "20", (string armor) => { GameObjectManager.SpawnInteract<InteractPickupArmor>(NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity).Play(int.Parse(armor)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Weapon", KeyCode.F8, enum_PlayerWeapon.Railgun, (enum_PlayerWeapon weapon) => {  GameObjectManager.SpawnInteract<InteractPickupWeapon>(NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity).Play(WeaponSaveData.New(weapon,5));  });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Interact", KeyCode.F9, enum_GameEventType.CoinsSack, (enum_GameEventType eventType) => { GenerateStageInteract(eventType, new ChunkGameObjectData(GameLevelManager.Instance.GetPlayerAtQuadrant().m_Identity, enum_TileObjectType.Invalid, NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + Vector3.forward * 2), Quaternion.identity),0f); });

        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Rank Exp", KeyCode.F10, "10", (string value) => { m_LocalPlayer.m_CharacterInfo.OnExpReceived(int.Parse(value)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Toggle HealthBar", KeyCode.None, () => GameUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().m_HealthGrid.transform.SetActivate(!GameUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().m_HealthGrid.transform.gameObject.activeSelf));
    }
    #endregion
    protected static GameManager nInstance;
    public static new GameManager Instance => nInstance;
    public GameProgressManager m_GameLevel { get; private set; }
    public EntityCharacterPlayer m_LocalPlayer { get; private set; } = null;
    Transform tf_CameraAttach;
    
    public override bool B_InGame => true;
    public bool m_GameLoading { get; private set; } = false;
    protected override void Awake()
    {
        base.Awake();
        nInstance = this;
        InitEntityDic();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntiyActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterRevive, OnCharacterRevive);
        m_GameLevel =  new GameProgressManager(GameDataManager.m_GameData,GameDataManager.m_GameProgressData);
        tf_CameraAttach = transform.Find("CameraAttach");
    }
    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        nInstance = null;
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntiyActivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterRevive, OnCharacterRevive);
    }

    protected override void Start()
    {
        base.Start();
        LoadStage();
    }

    private void Update()
    {
        if (m_GameLoading)
            return;

        float deltaTime = Time.deltaTime;
        Vector3 playerPosition = m_LocalPlayer.transform.position;

        m_GameLevel.Tick(deltaTime,playerPosition);
        tf_CameraAttach.position = playerPosition;
        GameLevelManager.Instance.TickGameLevel(playerPosition);
    }

    #region GenerateLevel
    void LoadStage() => this.StartSingleCoroutine(999, GenerateStage());
    IEnumerator GenerateStage() 
    {
        m_GameLoading = true;
        LoadingManager.Instance.ShowLoading(m_GameLevel.m_GameStage);
        CameraController.MainCamera.enabled = false;
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameLoadBegin);
        yield return null;
        m_GameLevel.StageInit();

        EntityDicReset();
        GameObjectManager.Clear();
        GameObjectManager.RegisterGameObjects();

        yield return GameLevelManager.Instance.Generate(m_GameLevel.m_GameStyle, m_GameLevel.m_Random, GenerateStageRelative);

        InitGameEffects(m_GameLevel.m_GameStyle, TResources.GetRenderData(m_GameLevel.m_GameStyle).RandomItem(m_GameLevel.m_Random));
        yield return null;

        Resources.UnloadUnusedAssets();
        GC.Collect();
        yield return null;

        m_GameLoading = false;
        CameraController.MainCamera.enabled = true;
        LoadingManager.Instance.EndLoading();
        OnPortalExit(1f, tf_CameraAttach);
        OnStageStart();
    }

    public List<InteractGameBase> m_StageInteracts { get; private set; } = new List<InteractGameBase>();
    void GenerateStageRelative(Dictionary<enum_ObjectEventType, List<ChunkGameObjectData>> eventObjects)
    {
        #region SpawnPoints Preparation
        List<Vector3> enermySpawns = new List<Vector3>();
        List<ChunkGameObjectData> playerSpawns=new List<ChunkGameObjectData>();
        List<ChunkGameObjectData> signalTowerSpawns = new List<ChunkGameObjectData>();
        List<ChunkGameObjectData> gameEventSpawnsPool = new List<ChunkGameObjectData>();
        eventObjects.Traversal((enum_ObjectEventType eventType, List<ChunkGameObjectData> objectDatas) =>
        {
            objectDatas.Traversal((ChunkGameObjectData objectData) =>
            {
                switch (objectData.m_ObjectType)
                {
                    case enum_TileObjectType.EMainEvent3x3:
                        switch(eventType)
                        {
                            case enum_ObjectEventType.Start:
                                playerSpawns.Add(objectData);
                                break;
                            case enum_ObjectEventType.Final:
                                signalTowerSpawns.Add(objectData);
                                break;
                            case enum_ObjectEventType.Normal:
                                gameEventSpawnsPool.Add(objectData);
                                break;
                        }
                        break;
                    case enum_TileObjectType.EEnermySpawn:
                        enermySpawns.Add(objectData.m_Pos);
                        break;
                    case enum_TileObjectType.ERandomEvent3x3:
                        gameEventSpawnsPool.Add(objectData);
                        break;
                }
            });
        });

        int playerSpawnIndex= playerSpawns.RandomIndex(m_GameLevel.m_Random);
        ChunkGameObjectData playerSpawn = playerSpawns[playerSpawnIndex];
        playerSpawns.RemoveAt(playerSpawnIndex);
        gameEventSpawnsPool.AddRange(playerSpawns);

        int signalTowerIndex = signalTowerSpawns.RandomIndex(m_GameLevel.m_Random);
        ChunkGameObjectData signalTowerSpawn = signalTowerSpawns[signalTowerIndex];
        signalTowerSpawns.RemoveAt(signalTowerIndex);
        gameEventSpawnsPool.AddRange(signalTowerSpawns);

        List<ChunkGameObjectData> gameEventSpawns = new List<ChunkGameObjectData>();
        int eventCount = GameConst.RI_GameEventGenerate.Random(m_GameLevel.m_Random);
        if (gameEventSpawnsPool.Count < eventCount)
            Debug.LogError("Event Shuffle Pool Not Match Required Size!");
        for(int i=0;i<eventCount;i++)
        {
            int shuffleIndex = gameEventSpawnsPool.RandomIndex(m_GameLevel.m_Random);
            gameEventSpawns.Add(gameEventSpawnsPool[shuffleIndex]);
            gameEventSpawnsPool.RemoveAt(shuffleIndex);
        }
        #endregion
        m_StageInteracts.Clear();
        //Spawn Player
        m_LocalPlayer = GameObjectManager.SpawnPlayerCharacter(GameDataManager.m_GameProgressData.m_Character, playerSpawn.m_Pos, playerSpawn.m_Rot).OnPlayerActivate(GameDataManager.m_GameProgressData);
        tf_CameraAttach.position = playerSpawn.m_Pos;
        tf_CameraAttach.rotation = playerSpawn.m_Rot;
        AttachPlayerCamera(tf_CameraAttach);

        //Spawn Signal Tower
        InteractSignalTower signalTower= GameObjectManager.SpawnInteract<InteractSignalTower>(signalTowerSpawn.m_Pos, signalTowerSpawn.m_Rot,GameLevelManager.Instance.GetQuadrantChunk(signalTowerSpawn.m_QuadrantIndex).m_InteractParent).Play(OnSignalTowerTransmitStart,OnSignalTowerTransmitCountFinish);
        m_StageInteracts.Add(signalTower);

        //Spawn Game Events
        float tradePriceMultiply = 1f + m_GameLevel.m_MinutesElapsed * GameConst.F_TradePriceMultiplyAdditivePerMin;
        gameEventSpawns.Traversal((ChunkGameObjectData objectData) =>
        {
            enum_GameEventType eventType = TCommon.RandomPercentage(GameConst.D_GameEventRate, enum_GameEventType.Invalid, m_GameLevel.m_Random);
            GenerateStageInteract(eventType,objectData,tradePriceMultiply);
        });

        m_GameLevel.BattleInit(enermySpawns, m_GameLevel.m_GameDifficulty, m_GameLevel.m_GameStage);
    }

    void GenerateStageInteract(enum_GameEventType eventType,ChunkGameObjectData objectData,float tradePriceMultiply)
    {
        Transform spawnTrans = GameLevelManager.Instance.GetQuadrantChunk(objectData.m_QuadrantIndex).m_InteractParent;
        InteractGameBase interact = null;
        switch (eventType)
        {
            default:
                Debug.LogError("Invalid Convertions Here!");
                return;
            case enum_GameEventType.CoinsSack:
                interact = GameObjectManager.SpawnInteract<InteractCoinSack>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.RI_CoinsSackAmount.Random(m_GameLevel.m_Random));
                break;
            case enum_GameEventType.HealthpackTrade:
                interact = GameObjectManager.SpawnInteract<InteractTradeContainer>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.I_EventMedpackPrice * tradePriceMultiply, GameObjectManager.SpawnInteract<InteractPickupHealthPack>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.I_HealthPackAmount));
                break;
            case enum_GameEventType.WeaponTrade:
                interact = GameObjectManager.SpawnInteract<InteractTradeContainer>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.I_EventWeaponTradePrice * tradePriceMultiply, SpawnRandomUnlockedWeapon(objectData.m_Pos,objectData.m_Rot,GameConst.D_EventWeaponTradeRate,spawnTrans,m_GameLevel.m_Random));
                break;
            case enum_GameEventType.WeaponReforge:
                interact = GameObjectManager.SpawnInteract<InteractWeaponReforge>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameDataManager.m_GameWeaponUnlocked[TCommon.RandomPercentage(GameConst.D_EventWeaponReforgeRate, null)].RandomItem());
                break;
            case enum_GameEventType.WeaponVendor:
                interact = GameObjectManager.SpawnInteract<InteractWeaponVendorMachine>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.I_EventWeaponVendorMachinePrice*tradePriceMultiply);
                break;
            case enum_GameEventType.PerkShrine:
                interact = GameObjectManager.SpawnInteract<InteractPerkShrine>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.I_PerkShrineTradePrice*tradePriceMultiply);
                break;
            case enum_GameEventType.BloodShrine:
                interact = GameObjectManager.SpawnInteract<InteractBloodShrine>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play();
                break;
            case enum_GameEventType.HealShrine:
                interact = GameObjectManager.SpawnInteract<InteractHealShrine>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.I_HealShrineTradePrice*tradePriceMultiply);
                break;
            case enum_GameEventType.PerkLottery:
                interact = GameObjectManager.SpawnInteract<InteractPerkLottery>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.I_EventPerkLotteryPrice * tradePriceMultiply, GameDataManager.RandomPlayerPerk(TCommon.RandomPercentage(GameConst.D_EventPerkLotteryRate), m_LocalPlayer.m_CharacterInfo.m_ExpirePerks, m_GameLevel.m_Random));
                break;
            case enum_GameEventType.PerkSelect:
                interact = GameObjectManager.SpawnInteract<InteractTradeContainer>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play(GameConst.I_EventPerkSelectPrice * tradePriceMultiply, GameObjectManager.SpawnInteract<InteractPerkSelect>(Vector3.zero, Quaternion.identity, spawnTrans).Play(GameDataManager.RandomPlayerPerks(3, GameConst.D_EventPerkSelectRate, m_LocalPlayer.m_CharacterInfo.m_ExpirePerks, m_GameLevel.m_Random)));
                break;
            case enum_GameEventType.SafeBox:
                interact = GameObjectManager.SpawnInteract<InteractSafeCrack>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play();
                break;
            case enum_GameEventType.WeaponRecycle:
                interact = GameObjectManager.SpawnInteract<InteractWeaponRecycle>(objectData.m_Pos, objectData.m_Rot, spawnTrans).Play();
                break;
        }
        m_StageInteracts.Add(interact);
    }
    #endregion
    #region Game Core Event
    void OnStageStart()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageStart);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_GameMissionUpdate, "UI_MISSION_SEARCHSIGNAL");
    }

    void OnSignalTowerTransmitStart(InteractSignalTower signalTower)
    {
        SetPostEffect_AreaScan(signalTower.m_Canvas.position, Color.red);
        m_GameLevel.OnTransmitTrigger(m_LocalPlayer.transform.position);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_GameMissionUpdate, "UI_MISSION_SURVIVE");
    }

    bool OnSignalTowerTransmitCountFinish(InteractSignalTower signalTower)
    {
        if (!m_GameLevel.CheckTransmitFinish())
            return false;

        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_GameMissionUpdate, "UI_MISSION_ENTERPORTAL");
        SetPostEffect_AreaScan(signalTower.m_PortalPos.position,Color.green);
        enum_GamePortalType portal = m_GameLevel.GetNextStageGenerate();
        GameObjectManager.SpawnInteract<InteractPortal>(signalTower.m_PortalPos.position, signalTower.m_PortalPos.rotation).Play(portal, OnChunkPortalEnter);
        return true;
    }

    void OnChunkPortalEnter(enum_GamePortalType portalType)
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageFinished);

        switch(portalType)
        {
            default:
                Debug.LogError("Invalid Portal Here!"+ portalType);
                break;
            case enum_GamePortalType.StageEnd:
                m_GameLevel.NextStage();
                GameDataManager.StageFinishSaveData(m_LocalPlayer, m_GameLevel);
                OnPortalEnter(1f, tf_CameraAttach, LoadStage);
                break;
            case enum_GamePortalType.GameWin:
                OnGameFinished(true);
                break;
          }
    }


    #region Revive Management
    bool m_revived = false;
    public void CheckRevive(Action _OnRevivePlayer)
    {
        if (m_revived)
            return;
        m_revived = true;
        GameUIManager.Instance.ShowPage<UI_Revive>(true, true, 0f).Play(_OnRevivePlayer);
    }
    #endregion

    void OnGameFinished(bool win)
    {
        m_GameLevel.GameFinished(win);
        GameDataManager.OnGameResult(m_GameLevel);
        GameUIManager.Instance.OnGameFinished(m_GameLevel, OnGameExit);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameFinish, win);
    }

    public void OnGameExit()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameExit);
        LoadingManager.Instance.ShowLoading(m_GameLevel.m_GameStage);
        SwitchScene(enum_Scene.Camp, () => { LoadingManager.Instance.EndLoading(); return true; });
    }
    #endregion
    #region Pickup Management
    public InteractPickupWeapon SpawnRandomUnlockedWeapon(Vector3 pos,Quaternion rot,Dictionary<enum_Rarity,float> weaponGenerate,Transform trans=null, System.Random random = null)=> GameObjectManager.SpawnInteract<InteractPickupWeapon>(pos, rot,trans).Play(GameDataManager.RandomUnlockedWeaponData(weaponGenerate.RandomPercentage( enum_Rarity.Ordinary,random),m_GameLevel.m_GameStage,random));

    void SpawnBattleEnermyDeadDrops(EntityCharacterBase entity)
    {
        if (entity.m_Flag != enum_EntityFlag.Enermy||entity.b_isSubEntity)
            return;

        Vector3 sourcePosition = entity.transform.position;

        if (TCommon.RandomPercentage() <= GameConst.F_EnermyKeyGenerate)
            GameObjectManager.SpawnInteract<InteractPickupKey>(sourcePosition, Quaternion.identity).Play(1).PlayDropAnim(GetPickupPosition(entity)).PlayMoveAnim(m_LocalPlayer.transform);

        int amount=GameConst.RI_EnermyCoinsGenerate.Random();
        for(int i=0;i<amount;i++)
            GameObjectManager.SpawnInteract<InteractPickupCoin>(sourcePosition, Quaternion.identity).Play(1).PlayDropAnim(GetPickupPosition(entity)).PlayMoveAnim(m_LocalPlayer.transform);

        enum_PlayerWeapon weaponBlueprint = enum_PlayerWeapon.Invalid;
        enum_Rarity blueprintRarity = TCommon.RandomPercentage(GameConst.m_ArmoryBlueprintGameDropRarities, enum_Rarity.Invalid);
        if (blueprintRarity != enum_Rarity.Invalid)
            weaponBlueprint = GameDataManager.UnlockArmoryBlueprint(blueprintRarity);
        if (weaponBlueprint != enum_PlayerWeapon.Invalid)
            GameObjectManager.SpawnInteract<InteractPickupArmoryBlueprint>(sourcePosition, Quaternion.identity).Play(weaponBlueprint).PlayDropAnim(GetPickupPosition(entity)).PlayMoveAnim(m_LocalPlayer.transform);

        enum_Rarity equipmentRarity = TCommon.RandomPercentage(GameConst.m_EquipmentGameDropRarities, enum_Rarity.Invalid);
        if (equipmentRarity != enum_Rarity.Invalid)
        {
            EquipmentSaveData equipment = GameDataManager.RandomRarityEquipment(equipmentRarity);
            GameDataManager.AcquireEquipment(equipment);
            GameObjectManager.SpawnInteract<InteractPickupEquipment>(sourcePosition, Quaternion.identity).Play(equipment).PlayDropAnim(GetPickupPosition(entity)).PlayMoveAnim(m_LocalPlayer.transform);
        }
    }

    Vector3 GetPickupPosition(EntityCharacterBase dropper) => NavigationManager.NavMeshPosition(dropper.transform.position + TCommon.RandomXZSphere()* 5f);
    #endregion
    #region Entity Management
    Dictionary<int, EntityBase> m_Entities = new Dictionary<int, EntityBase>();
    Dictionary<int, EntityCharacterBase> m_Characters = new Dictionary<int, EntityCharacterBase>();
    Dictionary<enum_EntityFlag, List<EntityCharacterBase>> m_AllyEntities = new Dictionary<enum_EntityFlag, List<EntityCharacterBase>>();
    Dictionary<enum_EntityFlag, List<EntityCharacterBase>> m_OppositeEntities = new Dictionary<enum_EntityFlag, List<EntityCharacterBase>>();
    public int m_FlagEntityCount(enum_EntityFlag flag) => m_AllyEntities[flag].Count;
    public List<EntityCharacterBase> GetCharacters(enum_EntityFlag sourceFlag, bool getAlly) => getAlly ? m_AllyEntities[sourceFlag] : m_OppositeEntities[sourceFlag];
    public bool EntityExists(int entityID) => m_Entities.ContainsKey(entityID);
    public EntityBase GetEntity(int entityID)
    {
        if (!EntityExists(entityID))
            Debug.LogError("Entity Not Exist in ID:" + entityID.ToString());
        return m_Entities[entityID]; ;
    }
    public bool CharacterExists(int entityID) => m_Characters.ContainsKey(entityID);
    public EntityCharacterBase GetCharacter(int entityID)
    {
        if (!CharacterExists(entityID))
            Debug.LogError("Character Not Exist in ID:" + entityID.ToString());
        return m_Characters[entityID];
    }
    void InitEntityDic()
    {
        TCommon.TraversalEnum((enum_EntityFlag flag) => {
            m_AllyEntities.Add(flag, new List<EntityCharacterBase>());
            m_OppositeEntities.Add(flag, new List<EntityCharacterBase>());
        });
    }

    void EntityDicReset()
    {
        m_Entities.Clear();
        m_Characters.Clear();
        TCommon.TraversalEnum((enum_EntityFlag flag) => {
            m_AllyEntities[flag].Clear();
            m_OppositeEntities[flag].Clear();
        });
    }

    void OnEntiyActivate(EntityBase entity)
    {
        m_Entities.Add(entity.m_EntityID, entity);
        if (entity.m_ControllType == enum_EntityType.None)
            return;
        EntityCharacterBase character = entity as EntityCharacterBase;
        m_Characters.Add(character.m_EntityID, character);
        m_AllyEntities[entity.m_Flag].Add(character);
        m_OppositeEntities.Traversal((enum_EntityFlag flag) => {
            if (entity.m_Flag != enum_EntityFlag.Neutal && flag != entity.m_Flag)
                m_OppositeEntities[flag].Add(character);
        });
    }

    void OnCharacterDead(EntityCharacterBase character)
    {
        if (character.m_ControllType == enum_EntityType.Player)
            SetPostEffect_Dead();

        SpawnBattleEnermyDeadDrops(character);
        m_GameLevel.OnCharacterDead(character.m_EntityID);
    }

    void OnEntityRecycle(EntityBase entity)
    {
        m_Entities.Remove(entity.m_EntityID);
        if (entity.m_ControllType == enum_EntityType.None)
            return;
        EntityCharacterBase character = entity as EntityCharacterBase;
        m_Characters.Remove(character.m_EntityID);
        m_AllyEntities[entity.m_Flag].Remove(character);
        m_OppositeEntities.Traversal((enum_EntityFlag flag) => {
            if (entity.m_Flag != enum_EntityFlag.Neutal && flag != entity.m_Flag)
                m_OppositeEntities[flag].Remove(character);
        });

        if (entity.m_ControllType == enum_EntityType.Player)
            OnGameFinished(false);
    }


    void OnCharacterRevive(EntityCharacterBase character)
    {
        if (character.m_ControllType == enum_EntityType.Player)
        {
            SetPostEffect_Revive();
            return;
        }
    }
    RaycastHit[] m_Raycasts;
    public bool EntityOpposite(EntityBase sourceEntity, EntityBase targetEntity) => sourceEntity.m_Flag != targetEntity.m_Flag;
    public bool EntityOpposite(int sourceEntity, int targetEntity) => GetEntity(sourceEntity).m_Flag != GetEntity(targetEntity).m_Flag;
    public EntityCharacterBase GetNeariesCharacter(EntityCharacterBase sourceEntity, bool targetAlly, bool checkObstacle = true, float checkDistance = float.MaxValue, Predicate<EntityCharacterBase> predictMatch = null)
    {
        EntityCharacterBase target = null;
        float f_nearestDistance = float.MaxValue;
        List<EntityCharacterBase> entities = GetNearbyCharacters(sourceEntity, targetAlly, checkObstacle, checkDistance, predictMatch);
        entities.Traversal((EntityCharacterBase character) =>
        {
            float distance = TCommon.GetXZDistance(sourceEntity.tf_Head.position, character.tf_Head.position);
            if (distance < f_nearestDistance)
            {
                target = character;
                f_nearestDistance = distance;
            }
        });
        return target;
    }

    public List<EntityCharacterBase> GetNearbyCharacters(EntityCharacterBase sourceEntity, bool targetAlly, bool checkObstacle = true, float checkDistance = float.MaxValue, Predicate<EntityCharacterBase> predictMatch = null)
    {
        List<EntityCharacterBase> targetCharacters = new List<EntityCharacterBase>();
        List<EntityCharacterBase> characters = GetCharacters(sourceEntity.m_Flag, targetAlly);
        characters.Traversal((EntityCharacterBase character) =>
        {
            if (character.m_EntityID == sourceEntity.m_EntityID || !character.m_TargetAvailable)
                return;

            float distance = TCommon.GetXZDistance(sourceEntity.tf_Head.position, character.tf_Head.position);
            if ((distance > checkDistance) || (checkObstacle && CheckEntityObstacleBetween(sourceEntity, character)))
                return;

            if (predictMatch != null && !predictMatch(character))
                return;

            targetCharacters.Add(character);
        });
        return targetCharacters;
    }

    public int GetNearbyEnermyCount(EntityCharacterBase sourceEntity, float checkDistance = float.MaxValue)
    {
        int nearbyCount = 0;
        List<EntityCharacterBase> entities = GetCharacters(sourceEntity.m_Flag, false);
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].m_EntityID == sourceEntity.m_EntityID)
                continue;
            if (Vector3.Distance(entities[i].transform.position, sourceEntity.transform.position) < checkDistance)
                nearbyCount++;
        }
        return nearbyCount;
    }

    public bool CheckEntityObstacleBetween(EntityCharacterBase source, EntityCharacterBase destination)
    {
        m_Raycasts = Physics.RaycastAll(source.tf_Head.position, TCommon.GetXZLookDirection(source.tf_Head.position, destination.tf_Head.position), Vector3.Distance(source.tf_Head.position, destination.tf_Head.position), GameLayer.Mask.I_ProjectileMask);
        for (int i = 0; i < m_Raycasts.Length; i++)
        {
            if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Static)
                return true;
        }
        return false;
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
        if (hb.I_AttacherID == sourceID || !Instance.m_Entities.ContainsKey(sourceID))
            return false;

        return hb.m_Attacher.m_Flag != Instance.GetEntity(sourceID).m_Flag;
    }
    #endregion
}

public class GameProgressManager
{
    #region GameData
    public string m_GameSeed { get; private set; }
    public enum_GameDifficulty m_GameDifficulty { get; private set; }
    public enum_GameStage m_GameStage { get; private set; }
    public float m_TimeElapsed { get; private set; }
    public int m_MinutesElapsed { get; private set; }
    Dictionary<enum_GameStage, enum_GameStyle> m_StageStyle = new Dictionary<enum_GameStage, enum_GameStyle>();
    public enum_GameStyle m_GameStyle => m_StageStyle[m_GameStage];
    public bool m_GameWin { get; private set; }
    #endregion
    #region LevelData
    public System.Random m_Random { get; private set; }
    #endregion
    #region Get
    public bool m_FinalStage => m_GameStage == enum_GameStage.Elite;
    #endregion
    public GameProgressManager(CGameSave _gameSave,CGameProgressSave _battleSave)
    {
        m_GameStage = _battleSave.m_Stage;
        m_GameDifficulty = _gameSave.m_GameDifficulty;
        m_GameSeed =_battleSave.m_GameSeed;
        m_TimeElapsed = _battleSave.m_GameTime;
        m_MinutesElapsed = (int)(m_TimeElapsed / 60f);

        m_Random = new System.Random(m_GameSeed.GetHashCode());
        List<enum_GameStyle> styleList = TCommon.GetEnumList<enum_GameStyle>();
        TCommon.TraversalEnum((enum_GameStage level) => {
            enum_GameStyle style = styleList.RandomItem(m_Random);
            styleList.Remove(style);
            m_StageStyle.Add(level, style);
        });
    }
    public void StageInit()
    {
        m_Random = new System.Random((m_GameSeed + m_GameStage.ToString()).GetHashCode());
    }
    public void NextStage() => m_GameStage++;
    public void GameFinished(bool win) => m_GameWin = win;
    public enum_GamePortalType GetNextStageGenerate() => m_FinalStage ? enum_GamePortalType.GameWin : enum_GamePortalType.StageEnd;
    public void Tick(float deltaTime,Vector3 playerPosition)
    {
        m_TimeElapsed += deltaTime;
        m_MinutesElapsed = (int)(m_TimeElapsed / 60f);

        BattleTick(deltaTime,playerPosition);
    }

    #region Battle
    List<Vector3> m_EnermySpawnPoints = new List<Vector3>();
    TimerBase m_BattleCheckTimer = new TimerBase();
    public bool m_BattleTransmiting =>m_TransmitEliteID>0;
    int m_TransmitEliteID;
    List<SEnermyGenerate> m_EnermyGenerate;

    public void BattleInit(List<Vector3> _spawnPoints, enum_GameDifficulty difficulty, enum_GameStage stage)
    {
        m_EnermySpawnPoints = _spawnPoints;
        m_EnermyGenerate = GameDataManager.GetEnermyGenerate(stage,difficulty);
        m_BattleCheckTimer.SetTimerDuration(GameConst.RI_EnermyGenerateDuration.Random());
        m_TransmitEliteID = -1;
    }

    protected void BattleTick(float deltaTime, Vector3 playerPosition)
    {

        float tickMultiplier = 1f+GameConst.F_EnermyGenerateTickMultiplierPerMinute * m_MinutesElapsed +(m_BattleTransmiting?GameConst.F_EnermyGenerateTickMultiplierTransmiting:0f);
        m_BattleCheckTimer.Tick(deltaTime*tickMultiplier);
        if (m_BattleCheckTimer.m_Timing)
            return;

        GenerateCommonEnermies(m_EnermyGenerate.RandomItem().m_EnermyGenerate, playerPosition);
        m_BattleCheckTimer.SetTimerDuration(GameConst.RI_EnermyGenerateDuration.Random());
    }

    public void OnTransmitTrigger(Vector3 playerPos)
    {
        m_TransmitEliteID = GameObjectManager.SpawnEntityCharacterAI(GameConst.L_GameEliteIndexes.RandomItem(), GetSpawnList(playerPos)[0], Quaternion.identity, enum_EntityFlag.Enermy, m_MinutesElapsed, m_GameDifficulty, false).m_EntityID;
    } 

    public void OnCharacterDead(int enermyID)
    {
        if (enermyID != m_TransmitEliteID)
            return;
        m_TransmitEliteID = -1;
    }

    public bool CheckTransmitFinish() => !m_BattleTransmiting;

    void GenerateCommonEnermies(List<int> enermyGenerate, Vector3 targetPoint)
    {
        List<Vector3> spawnPoint = GetSpawnList(targetPoint);
        int pointLength = spawnPoint.Count > 5 ? 5 : spawnPoint.Count;
        float eliteGenerateRate = GameConst.F_EnermyEliteGenerateBase + GameConst.F_EnermyEliteGeneratePerMinuteMultiplier * m_MinutesElapsed;
        enermyGenerate.Traversal((int enermyID) => {
            bool isElite = eliteGenerateRate > 100f ? true : eliteGenerateRate > TCommon.RandomPercentage();
            GameObjectManager.SpawnEntityCharacterAI(enermyID, spawnPoint[TCommon.RandomLength(pointLength)], Quaternion.identity, enum_EntityFlag.Enermy, m_MinutesElapsed, m_GameDifficulty,isElite );
        });
    }

    List<Vector3> GetSpawnList(Vector3 sourcePos)
    {
        List<Vector3> list = new List<Vector3>();

        m_EnermySpawnPoints.Traversal((Vector3 point) =>
        {
            if (Vector3.SqrMagnitude(sourcePos - point) >= GameConst.F_SqrEnermyGenerateMinDistance)
                list.Add(point);
        });
        list.Sort((a,b)=>Vector3.Magnitude(sourcePos-a)>Vector3.Magnitude(sourcePos-b)?1:-1);
        return list;
    }
    #endregion

    #region CalculateData
    public float F_Completion => GameExpression.GetResultCompletion(m_GameWin, m_GameStage, 0);
    public float F_CompletionScore => GameExpression.GetResultLevelScore(m_GameStage, 0);
    public float F_DifficultyBonus => GameExpression.GetResultDifficultyBonus(m_GameDifficulty);
    public float F_FinalScore =>  F_CompletionScore *  F_DifficultyBonus;
    public float F_CreditGain => GameExpression.GetResultRewardCredits(F_FinalScore);
    #endregion
}
