﻿using GameSetting;
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
    public bool Test_CameraYLock = false;
    #region Test
    public string M_TESTSEED = "";
    public bool B_PhysicsDebugGizmos = true;
    void AddConsoleBinddings()
    {
        List<UIT_MobileConsole.CommandBinding> m_bindings = new List<UIT_MobileConsole.CommandBinding>();
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Show Seed", "", KeyCode.None, (string value) => { Debug.LogError(m_GameLevel.m_GameSeed); }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Skip Level", "", KeyCode.Minus, (string value) => { OnLevelFinished(); }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Skip Stage", "", KeyCode.Equals, (string value) => {OnStageFnished();}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Kill All", "", KeyCode.Alpha0, (string value) => {
            GetCharacters(enum_EntityFlag.Enermy, true).Traversal((EntityCharacterBase character) =>
            {
                character.m_HitCheck.TryHit(new DamageInfo(character.m_Health.m_CurrentHealth, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
            });
        }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Freeze All", "0.5", KeyCode.Alpha8, (string value) =>
        {
            GetCharacters( enum_EntityFlag.Enermy,true).Traversal((EntityCharacterBase entity) => {
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.EquipmentInfo(-1,0, enum_CharacterEffect.Freeze,float.Parse( value))));
            });
        }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Enermy", "101", KeyCode.Z, (string id) => {
            GameObjectManager.SpawnEntityCharacterAI(int.Parse(id), NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position+ TCommon.RandomXZSphere(5f)),Quaternion.identity, enum_EntityFlag.Enermy,m_GameLevel.m_GameDifficulty,m_GameLevel.m_StageIndex);
        }));

        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Damage", "20", KeyCode.N, (string damage) => { m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(int.Parse(damage), enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Heal", "20", KeyCode.M, (string damage) => { m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-int.Parse(damage), enum_DamageType.Basic, DamageDeliverInfo.Default(-1))); }));



        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Player Equipment", "1", KeyCode.F1, (string actionIndex) => { GameObjectManager.SpawnInteract<InteractEquipment>(enum_Interaction.Equipment, NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity).Play(ActionDataManager.CreatePlayerEquipment(EquipmentSaveData.Default(int.Parse(actionIndex), TCommon.RandomEnumValues<enum_EquipmentRarity>(null)))); }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Player Exp", "100", KeyCode.F2, (string actionIndex) => { m_LocalPlayer.m_CharacterInfo.OnExpGain(int.Parse(actionIndex)); }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Coins", "20", KeyCode.F5, (string coins) => { GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupCoin, NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity).Play(int.Parse(coins),!m_Battling);}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Health", "20", KeyCode.F6, (string health) => {GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupHealth, NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity).Play(int.Parse(health), !m_Battling);}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Armor", "20", KeyCode.F7, (string armor) => {GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupArmor,  NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity).Play(int.Parse(armor), !m_Battling);}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Weapon", "102", KeyCode.F8, (string weaponIdentity) => {
            enum_PlayerWeapon spawnWeapon = GameDataManager.TryGetWeaponEnum(weaponIdentity);
            if(spawnWeapon != enum_PlayerWeapon.Invalid)
            GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon,  NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(spawnWeapon))); }));

        UIT_MobileConsole.Instance.AddConsoleBindings(m_bindings, (bool show) => { Time.timeScale = show ? .1f : 1f; });
    }
    #endregion
    protected static GameManager nInstance;
    public static new GameManager Instance => nInstance;
    public GameProgressManager m_GameLevel { get; private set; }
    public EntityCharacterPlayer m_LocalPlayer { get; private set; } = null;
    Transform tf_CameraAttach;
    float m_CameraAttachZ;

    public override bool B_InGame => true;
    public bool m_GameLoading { get; private set; } = false;
    protected override void Awake()
    {
        nInstance=this;
        base.Awake();
        InitEntityDic();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntiyActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterRevive, OnCharacterRevive);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnPlayerRankUp, OnPlayerRankUp);
        if (M_TESTSEED!="")
            GameDataManager.m_BattleData.m_GameSeed = M_TESTSEED;
        m_GameLevel =  new GameProgressManager(GameDataManager.m_GameData,GameDataManager.m_BattleData);
        tf_CameraAttach = transform.Find("CameraAttach");
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
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterRevive, OnCharacterRevive);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnPlayerRankUp, OnPlayerRankUp);
    }

    protected override void Start()
    {
        base.Start();
        AddConsoleBinddings();
        LoadStage();
    }

    private void Update()
    {
        if (m_GameLoading)
            return;

        float deltaTime = Time.deltaTime;
        tf_CameraAttach.position = Test_CameraYLock? new Vector3(m_LocalPlayer.transform.position.x,0, m_CameraAttachZ):m_LocalPlayer.transform.position;
    }
    //Call When Level Changed
    void LoadStage() => this.StartSingleCoroutine(999, DoLoadStage());
    IEnumerator DoLoadStage()     //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        m_GameLoading = true;
        LoadingManager.Instance.ShowLoading(m_GameLevel.m_StageIndex);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameLoad);
        yield return null;
        m_GameLevel.LoadStageData();
        GameLevelManager.Instance.GenerateStage(m_GameLevel.m_GameStyle, m_GameLevel.m_GameLevelDatas, m_GameLevel.m_Random);

        EntityDicReset();
        GameObjectManager.Clear();
        GameObjectManager.PresetRegistCommonObject();
        m_EnermySpawnIDs = GameObjectManager.RegistStyledInGamePrefabs(m_GameLevel.m_GameStyle, m_GameLevel.m_StageIndex);

        InitPostEffects(m_GameLevel.m_GameStyle);
        yield return null;
        Resources.UnloadUnusedAssets();
        GC.Collect();
        yield return null;

        m_LocalPlayer = GameObjectManager.SpawnEntityPlayer(GameDataManager.m_BattleData, Vector3.zero, Quaternion.identity);
        AttachPlayerCamera(tf_CameraAttach);
        OnLevelStart();

        m_GameLoading = false;
        LoadingManager.Instance.EndLoading();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameBegin);
    }


    void OnGenerateChunkRelatives(enum_ChunkEventType eventType, enum_TileObjectType tileType, ChunkGameObjectData objectData)
    {
        switch (tileType)
        {
            case enum_TileObjectType.REntrance2x2:
                m_CameraAttachZ = objectData.pos.z;
                m_LocalPlayer.Teleport(objectData.pos, objectData.rot);
                GameObjectManager.SpawnInteract<InteractTeleport>(enum_Interaction.Teleport, objectData.pos, objectData.rot).Play(m_GameLevel.m_GameStyle);
                break;
            case enum_TileObjectType.RExport4x1:
                GameObjectManager.SpawnInteract<InteractPortal>(enum_Interaction.Portal, objectData.pos, objectData.rot).Play(OnLevelFinished, "Test");
                break;
            case enum_TileObjectType.REnermySpawn1x1:
                m_EnermySpawnPoints.Add(objectData.pos);
                break;
            case enum_TileObjectType.REventArea3x3:
                switch (eventType)
                {
                    case enum_ChunkEventType.PerkAcquire:
                       GameObjectManager.SpawnInteract<InteractPerkAcquire>(enum_Interaction.PerkAcquire, objectData.pos, objectData.rot).Play(ActionDataManager.CreateRandomPlayerEquipment(TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_EventEquipment, m_GameLevel.m_Random), m_GameLevel.m_Random));
                        break;
                    case enum_ChunkEventType.PerkUpgrade:
                         GameObjectManager.SpawnInteract<InteractPerkUpgrade>(enum_Interaction.PerkUpgrade, objectData.pos, objectData.rot).Play();
                        break;
                    case enum_ChunkEventType.WeaponReforge:
                        GameObjectManager.SpawnInteract<InteractWeaponReforge>(enum_Interaction.WeaponReforge, objectData.pos, objectData.rot).Play(GameDataManager.m_WeaponRarities[TCommon.RandomPercentage(GameConst.D_EventWeaponReforgeRate, m_GameLevel.m_Random)].RandomItem(m_GameLevel.m_Random));
                        break;
                    case enum_ChunkEventType.Bonefire:
                        GameObjectManager.SpawnInteract<InteractBonfire>(enum_Interaction.Bonfire, objectData.pos, objectData.rot).Play();
                        break;
                    case enum_ChunkEventType.RewardChest:
                         GameObjectManager.SpawnInteract<InteractRewardChest>(enum_Interaction.RewardChest, objectData.pos, objectData.rot).Play(null, GameDataManager.m_WeaponRarities[TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_RewardWeapon, m_GameLevel.m_Random)].RandomItem(m_GameLevel.m_Random));
                        break;
                    case enum_ChunkEventType.Trader:
                        GameObjectManager.SpawnInteract<InteractTradeContainer>(enum_Interaction.TradeContainer, objectData.pos + LevelConst.I_TileSize * Vector3.left, objectData.rot).Play(GameConst.I_EventEquipmentTradePrice, GameObjectManager.SpawnInteract<InteractEquipment>(enum_Interaction.Equipment, objectData.pos, objectData.rot).Play(ActionDataManager.CreateRandomPlayerEquipment(TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_EventEquipment, m_GameLevel.m_Random), m_GameLevel.m_Random)));

                        GameObjectManager.SpawnInteract<InteractTradeContainer>(enum_Interaction.TradeContainer, objectData.pos + LevelConst.I_TileSize * Vector3.right, objectData.rot).Play(GameConst.I_EventEquipmentTradePrice, GameObjectManager.SpawnInteract<InteractEquipment>(enum_Interaction.Equipment, objectData.pos, objectData.rot).Play(ActionDataManager.CreateRandomPlayerEquipment(TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_EventEquipment, m_GameLevel.m_Random), m_GameLevel.m_Random)));

                        enum_WeaponRarity rarity = TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_TradeWeapon, m_GameLevel.m_Random);
                        GameObjectManager.SpawnInteract<InteractTradeContainer>(enum_Interaction.TradeContainer, objectData.pos + LevelConst.I_TileSize * Vector3.forward, objectData.rot).Play(GameConst.D_EventWeaponTradePrice[rarity].Random(m_GameLevel.m_Random), GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, objectData.pos + LevelConst.I_TileSize * Vector3.forward, objectData.rot).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[rarity].RandomItem(m_GameLevel.m_Random)))));

                        rarity = TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_TradeWeapon, m_GameLevel.m_Random);
                        GameObjectManager.SpawnInteract<InteractTradeContainer>(enum_Interaction.TradeContainer, objectData.pos + LevelConst.I_TileSize * Vector3.back, objectData.rot).Play(GameConst.D_EventWeaponTradePrice[rarity].Random(m_GameLevel.m_Random), GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, objectData.pos + LevelConst.I_TileSize * Vector3.forward, objectData.rot).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[rarity].RandomItem(m_GameLevel.m_Random)))));
                        break;
                }

                break;
        }
    }

    void OnLevelStart()
    {
        GameObjectManager.RecycleAllInteract();
        OnPortalExit(1f,tf_CameraAttach);
        m_EnermySpawnPoints.Clear();
        GameLevelManager.Instance.OnStartLevel(m_GameLevel.m_LevelIndex, m_GameLevel.m_Random, OnGenerateChunkRelatives);
        if (m_GameLevel.B_IsBattleLevel)
            OnBattleStart();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnLevelStart);
    } 

    void OnLevelFinished()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnLevelFinished);
        if (m_GameLevel.LevelFinished())
        {
            OnStageFnished();
            return;
        }
        OnPortalEnter(1f, tf_CameraAttach, OnLevelStart);
    }

    void OnStageFnished()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageFinished);
        if (m_GameLevel.StageFinished())
        {
            OnGameFinished(true);
            return;
        }
        GameDataManager.AdjustInGameData(m_LocalPlayer, m_GameLevel);
        OnPortalEnter(1f, tf_CameraAttach, LoadStage);
    }

    void OnGameFinished(bool win)
    {
        m_GameLevel.GameFinished(win);
        GameDataManager.OnGameFinished(win);
        GameDataManager.OnCreditStatus(m_GameLevel.F_CreditGain);
        GameUIManager.Instance.OnGameFinished(m_GameLevel, OnGameExit);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameFinish, win);
    }

    public void OnGameExit()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameExit);
        LoadingManager.Instance.ShowLoading(m_GameLevel.m_StageIndex);
        SwitchScene(enum_Scene.Camp, () => { LoadingManager.Instance.EndLoading(); return true; });
    }
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
    #region Revive Management
    bool m_revived = false;
    public void CheckRevive(Action _OnRevivePlayer)
    {
        if (m_revived)
            return;
        m_revived = true;
        GameUIManager.Instance.ShowPage<UI_Revive>(true, 0f).Play(_OnRevivePlayer);
    }
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
        if (entity.m_ControllType == enum_EntityController.None)
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
        if (character.m_ControllType == enum_EntityController.Player)
            SetPostEffect_Dead();

        OnBattleEntityKilled(character);
    }

    void OnEntityRecycle(EntityBase entity)
    {
        m_Entities.Remove(entity.m_EntityID);
        if (entity.m_ControllType == enum_EntityController.None)
            return;
        EntityCharacterBase character = entity as EntityCharacterBase;
        m_Characters.Remove(character.m_EntityID);
        m_AllyEntities[entity.m_Flag].Remove(character);
        m_OppositeEntities.Traversal((enum_EntityFlag flag) => {
            if (entity.m_Flag != enum_EntityFlag.Neutal && flag != entity.m_Flag)
                m_OppositeEntities[flag].Remove(character);
        });

        if (entity.m_ControllType == enum_EntityController.Player)
            OnGameFinished(false);
    }


    void OnCharacterRevive(EntityCharacterBase character)
    {
        if (character.m_ControllType == enum_EntityController.Player)
        {
            SetPostEffect_Revive();
            return;
        }
    }
    RaycastHit[] m_Raycasts;
    public bool EntityTargetable(EntityCharacterBase entity) => !entity.m_CharacterInfo.B_Effecting(enum_CharacterEffect.Cloak) && !entity.m_IsDead;
    public bool EntityOpposite(EntityBase sourceEntity, EntityBase targetEntity) => sourceEntity.m_Flag != targetEntity.m_Flag;
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
            if (character.m_EntityID == sourceEntity.m_EntityID || !EntityTargetable(character))
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
        m_Raycasts = Physics.RaycastAll(source.tf_Head.position, TCommon.GetXZLookDirection(source.tf_Head.position, destination.tf_Head.position), Vector3.Distance(source.tf_Head.position, destination.tf_Head.position), GameLayer.Mask.I_StaticEntity);
        for (int i = 0; i < m_Raycasts.Length; i++)
        {
            if (m_Raycasts[i].collider.gameObject.layer == GameLayer.I_Static)
                return true;
        }
        return false;
    }
    #endregion
    #region Game Item Management
    void OnBattleEnermyKilled(EntityCharacterBase entity)
    {
        if (entity.m_Flag != enum_EntityFlag.Enermy || entity.E_SpawnType == enum_EnermyType.Invalid)
            return;

        PickupGenerateData pickupGenerateData = entity.E_SpawnType == enum_EnermyType.Elite ? m_GameLevel.m_InteractGenerate.m_ElitePickupData : m_GameLevel.m_InteractGenerate.m_NormalPickupData;

        if (pickupGenerateData.CanGenerateHealth())
            GameObjectManager.SpawnInteract<InteractPickupHealth>(enum_Interaction.PickupHealth, GetPickupPosition(entity), Quaternion.identity).Play(GameConst.I_HealthPickupAmount, !m_Battling);

        if (pickupGenerateData.CanGenerateArmor())
            GameObjectManager.SpawnInteract<InteractPickupArmor>(enum_Interaction.PickupArmor, GetPickupPosition(entity), Quaternion.identity).Play(GameConst.I_ArmorPickupAmount, !m_Battling);

        int amount;
        if (pickupGenerateData.CanGenerateCoins(m_LocalPlayer.m_CharacterInfo.P_CoinsDropBase, out amount))
            GameObjectManager.SpawnInteract<InteractPickupCoin>(enum_Interaction.PickupCoin, GetPickupPosition(entity), Quaternion.identity).Play(amount, !m_Battling);

        GameObjectManager.SpawnInteract<InteractPickupExp>(enum_Interaction.PickupExp, GetPickupPosition(entity), Quaternion.identity).Play(GameExpression.GetEnermyKillExp(entity.E_SpawnType == enum_EnermyType.Elite, m_GameLevel.m_StageIndex), !m_Battling);

        enum_WeaponRarity weaponRarity = TCommon.RandomPercentage(pickupGenerateData.m_WeaponRate, enum_WeaponRarity.Invalid);
        if (weaponRarity != enum_WeaponRarity.Invalid)
            GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, GetPickupPosition(entity), Quaternion.identity).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[weaponRarity].RandomItem()), null));
    }

    Vector3 GetPickupPosition(EntityCharacterBase dropper) => NavigationManager.NavMeshPosition(dropper.transform.position + TCommon.RandomXZSphere(1.5f));

    void OnPlayerRankUp() => GameUIManager.Instance.ShowPage<UI_EquipmentSelect>(true, 0f).Show(new List<ActionPerkBase>()  {
        ActionDataManager.CreateRandomPlayerEquipment(TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_RankupEquipment),null),
        ActionDataManager.CreateRandomPlayerEquipment(TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_RankupEquipment), null),
        ActionDataManager.CreateRandomPlayerEquipment(TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_RankupEquipment), null) }, m_LocalPlayer.m_CharacterInfo.OnActionPerkAcquire);

    #endregion
    #region Battle Relatives 
    List<Vector3> m_EnermySpawnPoints = new List<Vector3>();
    public Dictionary<enum_EnermyType, List<int>> m_EnermySpawnIDs;
    public bool m_Battling = false;

    void OnBattleEntityKilled(EntityCharacterBase character)
    {
        if (!m_Battling)
            return;

        if (GetCharacters(enum_EntityFlag.Enermy, true).FindAll(p=>!p.m_IsDead).Count<=0)
            OnBattleFinish();
    }

    void OnBattleStart()
    {
        if (m_Battling)
        {
            Debug.LogError("Can't Trigger Another Battle!");
            return;
        }
        m_Battling = true;
        SEnermyGenerate enermyGenerate = m_GameLevel.GetEnermyGenerate();
        int spawnPointCount = 0;
        enermyGenerate.GetEnermyIDList(m_EnermySpawnIDs, m_GameLevel.m_Random).Traversal((int enermyID) => {
            GameObjectManager.SpawnEntityCharacterAI(enermyID,m_EnermySpawnPoints[spawnPointCount],Quaternion.identity, enum_EntityFlag.Enermy,m_GameLevel.m_GameDifficulty,m_GameLevel.m_StageIndex);
            spawnPointCount++;
            if (spawnPointCount == m_EnermySpawnPoints.Count)
                spawnPointCount = 0;
        });


        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleStart);
    }

    void OnBattleFinish()
    {
        m_Battling = false;
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleFinish);
    }
    #endregion
}

#region External Tools Packaging Class
public class GameProgressManager
{
    #region LevelData
    public string m_GameSeed { get; private set; }
    public System.Random m_Random { get; private set; }

    public int m_GameDifficulty { get; private set; }

    Dictionary<bool, List<SEnermyGenerate>> m_EnermyGenerate;
    public StageInteractGenerateData m_InteractGenerate { get; private set; }
    public SEnermyGenerate GetEnermyGenerate() => m_EnermyGenerate[B_IsFinalLevel].RandomItem(m_Random);
    public enum_StageLevel m_StageIndex { get; private set; }
    public int m_LevelIndex { get; private set; }
    public List<GameLevelData> m_GameLevelDatas { get; private set; }
    public bool B_IsBattleLevel => m_GameLevelDatas[m_LevelIndex].m_ChunkType.IsBattleLevel();
    public bool B_IsFinalLevel => m_LevelIndex == m_GameLevelDatas.Count - 1;
    public bool B_IsFinalStage => m_StageIndex == enum_StageLevel.Ranger;
    Dictionary<enum_StageLevel, enum_GameStyle> m_StageStyle = new Dictionary<enum_StageLevel, enum_GameStyle>();
    public enum_GameStyle m_GameStyle => m_StageStyle[m_StageIndex];
    public bool m_gameWin { get; private set; }
    public int m_LevelPassed { get; private set; }
    #endregion
    public GameProgressManager(CGameSave _gameSave,CBattleSave _battleSave)
    {
        m_StageIndex = _battleSave.m_Stage;
        m_GameDifficulty = _gameSave.m_GameDifficulty;
        m_GameSeed =_battleSave.m_GameSeed;
        m_LevelPassed = _battleSave.m_LevelPassed;
        m_Random = new System.Random(m_GameSeed.GetHashCode());
        List<enum_GameStyle> styleList = TCommon.GetEnumList<enum_GameStyle>();
        TCommon.TraversalEnum((enum_StageLevel level) => {
            enum_GameStyle style = styleList.RandomItem(m_Random);
            styleList.Remove(style);
            m_StageStyle.Add(level, style);
        });

        m_Random = new System.Random((m_GameSeed+m_StageIndex.ToString()).GetHashCode());
        m_GameLevelDatas = GameExpression.GetGameLevelDatas(m_Random);
    }
    public void LoadStageData()
    {
        m_InteractGenerate = GameExpression.GetInteractGenerate(m_StageIndex);
        m_EnermyGenerate = GameDataManager.GetEnermyGenerate(m_StageIndex);
        m_Random = new System.Random((m_GameSeed + m_StageIndex.ToString()).GetHashCode());
    }

    public bool LevelFinished()
    {
        if (B_IsFinalLevel)
            return true;
        m_LevelIndex++;
        m_LevelPassed++;
        return false;
    }

    public bool StageFinished()
    {
        if (B_IsFinalStage)
            return true;
        m_StageIndex++;
        return false;
    }
    
    public void GameFinished(bool win)=> m_gameWin = win;
    #region CalculateData
    public float F_Completion => GameExpression.GetResultCompletion(m_gameWin, m_StageIndex, 0);
    public float F_CompletionScore => GameExpression.GetResultLevelScore(m_StageIndex, 0);
    public float F_DifficultyBonus => GameExpression.GetResultDifficultyBonus(m_GameDifficulty);
    public float F_FinalScore =>  F_CompletionScore *  F_DifficultyBonus;
    public float F_CreditGain => GameExpression.GetResultRewardCredits(F_FinalScore);
    #endregion

    public string GetLevelIconSprite() => m_GameLevelDatas[m_LevelIndex].m_ChunkType.GetLevelIconSprite();
    public string GetLevelInfoKey() => m_GameLevelDatas[m_LevelIndex].m_ChunkType.GetLevelNameLocalizeKey();
}
#endregion