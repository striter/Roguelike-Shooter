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
    protected static GameManager nInstance;
    public static new GameManager Instance => nInstance;
    #region Test
    public string M_TESTSEED = "";
    public bool B_PhysicsDebugGizmos = true;
    void AddConsoleBinddings()
    {
        List<UIT_MobileConsole.CommandBinding> m_bindings = new List<UIT_MobileConsole.CommandBinding>();
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Show Seed", "", KeyCode.None, (string value) => { Debug.LogError(m_GameLevel.m_GameSeed); }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Skip Stage", "", KeyCode.Equals, (string value) => {OnStageFnished();}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Kill All", "", KeyCode.Alpha0, (string value) => {
            GetCharacters(enum_EntityFlag.Enermy, true).Traversal((EntityCharacterBase character) =>
            {
                EntityCharacterAI ai = character as EntityCharacterAI;
                if (ai&& ai.m_AIBattleActivating)
                    ai.m_HitCheck.TryHit(new DamageInfo(ai.m_Health.m_CurrentHealth, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
            });
        }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Freeze All", "0.5", KeyCode.Alpha8, (string value) =>
        {
            GetCharacters( enum_EntityFlag.Enermy,true).Traversal((EntityCharacterBase entity) => {
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.EquipmentInfo(-1,0, enum_CharacterEffect.Freeze,float.Parse( value))));
            });
        }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Enermy", "101", KeyCode.Z, (string id) => {
            GameObjectManager.SpawnEntityCharacterAI(int.Parse(id), NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position+ TCommon.RandomXZSphere(5f)), m_LocalPlayer.transform.position, enum_EntityFlag.Enermy,m_GameLevel.m_GameDifficulty,m_GameLevel.m_GameStage,true);
        }));

        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Damage", "20", KeyCode.N, (string damage) => { m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(int.Parse(damage), enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Heal", "20", KeyCode.M, (string damage) => { m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-int.Parse(damage), enum_DamageType.Basic, DamageDeliverInfo.Default(-1))); }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Coins", "20", KeyCode.F5, (string coins) => { GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupCoin, NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity, tf_Interacts).Play(int.Parse(coins));}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Health", "20", KeyCode.F6, (string health) => {GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupHealth, NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity, tf_Interacts).Play(int.Parse(health));}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Armor", "20", KeyCode.F7, (string armor) => {GameObjectManager.SpawnInteract<InteractPickupAmount>(enum_Interaction.PickupArmor,  NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity, tf_Interacts).Play(int.Parse(armor));}));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Weapon", "102", KeyCode.F8, (string weapon) => { GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon,  NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity, tf_Interacts).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew((enum_PlayerWeapon)int.Parse(weapon)))); }));
        m_bindings.Add(UIT_MobileConsole.CommandBinding.Create("Player Equipment", "1", KeyCode.F1, (string actionIndex) => { GameObjectManager.SpawnInteract<InteractEquipment>(enum_Interaction.Equipment, NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere(5f)), Quaternion.identity, tf_Interacts).Play(ActionDataManager.CreatePlayerEquipment(EquipmentSaveData.Default(int.Parse(actionIndex), TCommon.RandomEnumValues<enum_EquipmentRarity>(null)))); }));

        UIT_MobileConsole.Instance.AddConsoleBindings(m_bindings,(bool show)=> { Time.timeScale = show ? .1f : 1f; });
    }
    #endregion
    public GameProgressManager m_GameLevel { get; private set; }
    public EntityCharacterPlayer m_LocalPlayer { get; private set; } = null;
    public Transform tf_Interacts { get; private set; } = null;
    Transform tf_PlayerStart;
    ObjectPoolListMono<int, GamePlayerChunkTrigger> m_ChunkEnterTriggers;
    ObjectPoolListMono<int, GameEnermyCommander> m_EnermyCommand;

    public override bool B_InGame => true;
    protected override void Awake()
    {
        nInstance=this;
        base.Awake();
        InitEntityDic();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntiyActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterRevive, OnCharacterRevive);
        if (M_TESTSEED!="")
            GameDataManager.m_BattleData.m_GameSeed = M_TESTSEED;
        m_GameLevel =  new GameProgressManager(GameDataManager.m_GameData,GameDataManager.m_BattleData);
        tf_Interacts = transform.Find("Interacts");
        tf_PlayerStart = transform.Find("PlayerStart");
        m_ChunkEnterTriggers = new ObjectPoolListMono<int, GamePlayerChunkTrigger>(transform.Find("Triggers/ChunkTrigger"), "TriggerItem");
        m_EnermyCommand = new ObjectPoolListMono<int, GameEnermyCommander>(transform.Find("EnermyPool"), "EnermyItem");
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
    }
    protected override void Start()
    {
        base.Start();
        AddConsoleBinddings();
        LoadStage();
    }
    public void OnExitGame()
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameExit);
        LoadingManager.Instance.ShowLoading(m_GameLevel.m_GameStage);
        SwitchScene( enum_Scene.Camp,()=> { LoadingManager.Instance.EndLoading();return true; });
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        GameBattleTick(deltaTime);
    }
    #region Stage Management
    //Call When Level Changed
    void LoadStage()=>this.StartSingleCoroutine(999, DoLoadStage());
    IEnumerator DoLoadStage()     //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        LoadingManager.Instance.ShowLoading(m_GameLevel.m_GameStage);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameLoad);
        yield return null;
        m_GameLevel.LoadStageData();
        yield return GameLevelManager.Instance.Generate(m_GameLevel.m_GameStyle, m_GameLevel.m_GameSeed,m_GameLevel.m_GameRandom);
        GenerateGameRelatives();
        yield return null;

        Resources.UnloadUnusedAssets();
        GC.Collect();

        yield return null;
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameBegin);
        LoadingManager.Instance.EndLoading();
        OnPortalExit(1f, m_LocalPlayer.tf_CameraAttach);
    }

    void GenerateGameRelatives()
    {
        EntityDicReset();
        GameObjectManager.Clear();
        GameObjectManager.PresetRegistCommonObject();
        m_EnermyIDs = GameObjectManager.RegistStyledInGamePrefabs(m_GameLevel.m_GameStyle, m_GameLevel.m_GameStage);

        GenerateChunkRelatives();
        m_LocalPlayer = GameObjectManager.SpawnEntityPlayer(GameDataManager.m_BattleData, tf_PlayerStart.position, tf_PlayerStart.rotation);
        AttachPlayerCamera(m_LocalPlayer.tf_CameraAttach);
        GenerateBattleRelatives();

        InitPostEffects(m_GameLevel.m_GameStyle);
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
    
    void SpawnEntityDeadPickups(EntityCharacterBase entity)
    {
        if (entity.m_Flag != enum_EntityFlag.Enermy||entity.E_SpawnType== enum_EnermyType.Invalid)
            return;

        PickupGenerateData pickupGenerateData = entity.E_SpawnType == enum_EnermyType.Elite ? m_GameLevel.m_InteractGenerate.m_ElitePickupData : m_GameLevel.m_InteractGenerate.m_NormalPickupData;

        if (pickupGenerateData.CanGenerateHealth())
            GameObjectManager.SpawnInteract<InteractPickupHealth>(enum_Interaction.PickupHealth, GetPickupPosition(entity), Quaternion.identity, tf_Interacts).Play(GameConst.I_HealthPickupAmount);

        if (pickupGenerateData.CanGenerateArmor())
            GameObjectManager.SpawnInteract<InteractPickupArmor>(enum_Interaction.PickupArmor, GetPickupPosition(entity), Quaternion.identity, tf_Interacts).Play(GameConst.I_ArmorPickupAmount);

        int amount;
        if(pickupGenerateData.CanGenerateCoins(m_LocalPlayer.m_CharacterInfo.P_CoinsDropBase,out amount))
            GameObjectManager.SpawnInteract<InteractPickupCoin>(enum_Interaction.PickupCoin, GetPickupPosition(entity), Quaternion.identity, tf_Interacts).Play(amount);
        
        enum_WeaponRarity weaponRarity = TCommon.RandomPercentage(pickupGenerateData.m_WeaponRate, enum_WeaponRarity.Invalid);
        if (weaponRarity != enum_WeaponRarity.Invalid)
            GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, GetPickupPosition(entity), Quaternion.identity, tf_Interacts).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[weaponRarity].RandomItem()),null));
        
    }
    
    Vector3 GetPickupPosition(EntityCharacterBase dropper) => NavigationManager.NavMeshPosition(dropper.transform.position + TCommon.RandomXZSphere(1.5f));
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
    #region Revive Management
    bool m_revived = false;
    public void CheckRevive(Action _OnRevivePlayer)
    {
        if (m_revived)
            return;
        m_revived = true;
        UIManager.Instance.ShowPage<UI_Revive>(true, 0f).Play(_OnRevivePlayer);
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
        
        OnBattleCharatcerKilled(character);
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
    #region Chunk Relative Management
    public Dictionary<int, GameChunk> m_GameChunkData = new Dictionary<int, GameChunk>();
    void GenerateChunkRelatives()
    {
        m_GameChunkData.Clear();
        m_ChunkEnterTriggers.ClearPool();
        GameLevelManager.Instance.m_GameChunks.Traversal((int chunkIndex, LevelChunkGame chunkData) =>
        {
        bool isFinal = chunkData.m_ChunkType == enum_ChunkType.Final;
        bool isBattle = isFinal || chunkData.m_ChunkType == enum_ChunkType.Battle;
        m_GameChunkData.Add(chunkIndex, isBattle ? new GameChunkBattle(chunkIndex, isFinal) : new GameChunk(chunkIndex));

        chunkData.m_ChunkObjects.Traversal((enum_TileObjectType tileType, List<ChunkGameObjectData> objects) => {
                objects.Traversal((ChunkGameObjectData objectData) =>
                {
                    switch (tileType)
                    {
                        case enum_TileObjectType.RPlayerSpawn1x1:
                            tf_PlayerStart.position = objectData.pos;
                            tf_PlayerStart.rotation = objectData.rot;
                            break;
                        case enum_TileObjectType.RStagePortal2x2:
                            GameObjectManager.SpawnInteract<InteractPortal>(enum_Interaction.Portal, objectData.pos, objectData.rot, tf_Interacts).Play(OnStageFnished, "Test");
                            break;
                        case enum_TileObjectType.RConnection1x5:
                            int triggerIndex = m_ChunkEnterTriggers.Count;
                            GamePlayerChunkTrigger trigger = m_ChunkEnterTriggers.AddItem(triggerIndex);
                            trigger.transform.position = objectData.pos;
                            trigger.transform.rotation = objectData.rot;
                            trigger.Play(chunkIndex, chunkData.m_WorldChunkBounds, OnChunkEnterDetect);
                            m_GameChunkData[chunkIndex].m_ChunkTriggers.Add(triggerIndex);
                            break;
                        case enum_TileObjectType.REventArea3x3:
                            switch (chunkData.m_ChunkEventType)
                            {
                                case enum_ChunkEventType.Bonefire:
                                    GameObjectManager.SpawnInteract<InteractBonfire>(enum_Interaction.Bonfire, objectData.pos, objectData.rot, tf_Interacts).Play();
                                    break;
                                case enum_ChunkEventType.RewardChest:
                                    GameObjectManager.SpawnInteract<InteractRewardChest>(enum_Interaction.RewardChest, objectData.pos, objectData.rot, tf_Interacts).Play(null, GameDataManager.m_WeaponRarities[TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_RewardWeapon, m_GameLevel.m_GameRandom)].RandomItem(m_GameLevel.m_GameRandom));
                                    break;
                                case enum_ChunkEventType.Trader:
                                    GameObjectManager.SpawnNPC(enum_InteractCharacter.Trader, objectData.pos,objectData.rot);

                                    GameObjectManager.SpawnInteract<InteractTradeContainer>(enum_Interaction.TradeContainer, objectData.pos+LevelConst.I_TileSize*Vector3.left, objectData.rot, tf_Interacts).Play(GameConst.I_EventEquipmentTradePrice, GameObjectManager.SpawnInteract<InteractEquipment>(enum_Interaction.Equipment, objectData.pos, objectData.rot).Play(ActionDataManager.CreateRandomPlayerEquipment(TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_TradeEquipment,m_GameLevel.m_GameRandom),m_GameLevel.m_GameRandom)));

                                    GameObjectManager.SpawnInteract<InteractTradeContainer>(enum_Interaction.TradeContainer, objectData.pos + LevelConst.I_TileSize * Vector3.right, objectData.rot, tf_Interacts).Play(GameConst.I_EventEquipmentTradePrice, GameObjectManager.SpawnInteract<InteractEquipment>(enum_Interaction.Equipment, objectData.pos, objectData.rot).Play(ActionDataManager.CreateRandomPlayerEquipment(TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_TradeEquipment, m_GameLevel.m_GameRandom), m_GameLevel.m_GameRandom)));

                                    enum_WeaponRarity rarity = TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_TradeWeapon,m_GameLevel.m_GameRandom);
                                    GameObjectManager.SpawnInteract<InteractTradeContainer>(enum_Interaction.TradeContainer, objectData.pos + LevelConst.I_TileSize * Vector3.forward, objectData.rot, tf_Interacts).Play(GameConst.D_EventWeaponTradePrice[rarity].Random(m_GameLevel.m_GameRandom),GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, objectData.pos + LevelConst.I_TileSize * Vector3.forward, objectData.rot).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[rarity].RandomItem(m_GameLevel.m_GameRandom)))));

                                    rarity = TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_TradeWeapon, m_GameLevel.m_GameRandom);
                                    GameObjectManager.SpawnInteract<InteractTradeContainer>(enum_Interaction.TradeContainer, objectData.pos + LevelConst.I_TileSize * Vector3.back, objectData.rot, tf_Interacts).Play(GameConst.D_EventWeaponTradePrice[rarity].Random(m_GameLevel.m_GameRandom), GameObjectManager.SpawnInteract<InteractWeapon>(enum_Interaction.Weapon, objectData.pos + LevelConst.I_TileSize * Vector3.forward, objectData.rot).Play(GameObjectManager.SpawnWeapon(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[rarity].RandomItem(m_GameLevel.m_GameRandom)))));
                                    break;
                            }
                            break;
                    }
                });
            });
        });
    }

    void OnChunkEnterDetect(int chunkIndex)
    {
        m_GameChunkData[chunkIndex].m_ChunkTriggers.Traversal((int triggerIndex) => {
            m_ChunkEnterTriggers.RemoveItem(triggerIndex);
        });

        if (m_GameChunkData[chunkIndex].m_IsBattle)
            OnBattleTrigger(chunkIndex);
    }

    #endregion
    #region Battle Relatives 
    public Dictionary<enum_EnermyType, List<int>> m_EnermyIDs;
    public bool m_Battling => m_BattleChunkData != null;
    GameChunkBattle m_BattleChunkData = null;

    TimeCounter m_TimerFinalBattle = new TimeCounter();
    bool m_FinalBattling;
    int m_FinalEliteCommandIndex;
    float m_PreviousEliteHealthScale;
    public List<Vector3> m_FinalBattleGeneratePoints { get; private set; } = new List<Vector3>();

    void GenerateBattleRelatives()
    {
        m_FinalBattling = false;

        m_BattleChunkData = null;
        m_EnermyCommand.ClearPool();

        int enermyCommandIndex = 0;
        GameLevelManager.Instance.m_GameChunks.Traversal((int chunkIndex,LevelChunkGame chunkData) => {
            bool isBattleChunk = chunkData.m_ChunkType == enum_ChunkType.Battle || chunkData.m_ChunkType == enum_ChunkType.Final;
            bool isFinalChunk = chunkData.m_ChunkType == enum_ChunkType.Final;

            if (!isBattleChunk)
                return;

            GameChunkBattle chunkBattleData = m_GameChunkData[chunkIndex] as GameChunkBattle;

            if (isFinalChunk)
            {
                ChunkGameObjectData objectData = chunkData.m_ChunkObjects[enum_TileObjectType .REliteEnermySpawn1x1].RandomItem(m_GameLevel.m_GameRandom);
                GameEnermyCommander m_Command = m_EnermyCommand.AddItem(enermyCommandIndex);
                m_Command.transform.position = NavigationManager.NavMeshPosition(objectData.pos + TCommon.RandomXZSphere(1f));
                m_Command.transform.rotation = objectData.rot;
                m_Command.Play( m_EnermyIDs[enum_EnermyType.Elite].RandomItem(m_GameLevel.m_GameRandom), m_LocalPlayer.transform);
                chunkBattleData.m_BattleEnermyCommands.Add(enermyCommandIndex);
                m_FinalEliteCommandIndex = enermyCommandIndex;
                enermyCommandIndex++;

                chunkData.m_ChunkObjects[enum_TileObjectType.REnermySpawn1x1].Traversal((ChunkGameObjectData data) =>{
                    m_FinalBattleGeneratePoints.Add(data.pos);
                });
            }
            else
            {
                m_GameLevel.m_EnermyGenerate[isFinalChunk].RandomItem(m_GameLevel.m_GameRandom).GetEnermyIDList(m_EnermyIDs,m_GameLevel.m_GameRandom).Traversal((int enermyID) =>
                {
                    ChunkGameObjectData objectData = chunkData.m_ChunkObjects[enum_TileObjectType.REnermySpawn1x1].RandomItem(m_GameLevel.m_GameRandom);
                    GameEnermyCommander m_Command = m_EnermyCommand.AddItem(enermyCommandIndex);
                    m_Command.transform.position = NavigationManager.NavMeshPosition(objectData.pos + TCommon.RandomXZSphere(3f));
                    m_Command.transform.rotation = objectData.rot;
                    m_Command.Play(enermyID, m_LocalPlayer.transform);
                    chunkBattleData.m_BattleEnermyCommands.Add(enermyCommandIndex);
                    enermyCommandIndex++;
                });
            }
        });
    }

    void GameBattleTick(float deltaTime)
    {
        FinalBattleTick(deltaTime);
    }

    void OnBattleTrigger(int chunkIndex)
    {
        if (m_Battling)
            return;
        
        m_BattleChunkData = m_GameChunkData[chunkIndex] as GameChunkBattle;
        m_BattleChunkData.m_BattleEnermyCommands.Traversal((int commandIndex) => {
            m_EnermyCommand.m_ActiveItemDic[commandIndex].DoBattle();
        });

        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleStart, chunkIndex);
        if (m_BattleChunkData.m_IsFinal)
            OnFinalBattleStart();

    }

    void OnBattleCharatcerKilled(EntityCharacterBase character)
    {
        if (!m_Battling)
            return;

        bool chunkEntityKilled = false;
        m_EnermyCommand.m_ActiveItemDic.TraversalBreak((GameEnermyCommander command) =>
        {
            chunkEntityKilled = command.OnCharacterDead(character);
            return chunkEntityKilled;
        });

        if (!chunkEntityKilled)
            return;

        SpawnEntityDeadPickups(character);
        bool commandAliveStill=false;
        m_BattleChunkData.m_BattleEnermyCommands.TraversalBreak((int commandIndex) => {
            commandAliveStill = m_EnermyCommand.m_ActiveItemDic[commandIndex].m_Playing;
            return commandAliveStill;
        });

        if (commandAliveStill)
            return;
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleFinish);
        if (m_BattleChunkData.m_IsFinal)
            OnFinalBattleFinish();
        m_BattleChunkData = null;

    }

    void OnFinalBattleStart()
    {
        m_FinalBattling = true;
        m_PreviousEliteHealthScale = 1f;
        m_TimerFinalBattle.SetTimer(GameConst.RI_GameFinalBattleEnermySpawnCheck.Random());
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnFinalBattleStart);
    }

    void OnFinalBattleFinish()
    {
        GetCharacters(enum_EntityFlag.Enermy, true).Traversal((EntityCharacterBase entity) => {
            entity.m_HitCheck.TryHit(new DamageInfo(entity.m_Health.m_CurrentHealth, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
        });
        m_FinalBattling = false;
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnFinalBattleFinish);
    }

    void FinalBattleTick(float deltaTime)
    {
        if (!m_FinalBattling)
            return;

        m_TimerFinalBattle.Tick(deltaTime);
        if (m_TimerFinalBattle.m_Timing)
            return;
        m_TimerFinalBattle.SetTimer(GameConst.RI_GameFinalBattleEnermySpawnCheck.Random());

        float eliteHealthScale = m_EnermyCommand.GetItem(m_FinalEliteCommandIndex).m_EntityHealthScale;
        if (eliteHealthScale <= 0|| m_PreviousEliteHealthScale - eliteHealthScale < GameConst.F_FinalBattleEnermySpawnEliteHealthScaleOffset)
            return;
        m_PreviousEliteHealthScale = eliteHealthScale;

        m_GameLevel.m_EnermyGenerate[true].RandomItem(m_GameLevel.m_GameRandom).GetEnermyIDList(m_EnermyIDs, m_GameLevel.m_GameRandom).Traversal((int enermyID) =>
        {
            Vector3 spawnPos = m_FinalBattleGeneratePoints.RandomItem();
            GameObjectManager.SpawnEntityCharacterAI(enermyID, NavigationManager.NavMeshPosition(spawnPos), m_LocalPlayer.transform.position, enum_EntityFlag.Enermy,m_GameLevel.m_GameDifficulty,m_GameLevel.m_GameStage,true);
        });

    }
    #endregion
}

#region External Tools Packaging Class
public class GameProgressManager
{
    #region LevelData
    public string m_GameSeed { get; private set; }
    public int m_GameDifficulty { get; private set; }

    public System.Random m_GameRandom { get; private set; }
    public StageInteractGenerateData m_InteractGenerate { get; private set; }
    public Dictionary<bool, List<SEnermyGenerate>> m_EnermyGenerate { get; private set; }
    public bool B_IsFinalStage => m_GameStage == enum_StageLevel.Ranger;
    Dictionary<enum_StageLevel, enum_LevelStyle> m_StageStyle = new Dictionary<enum_StageLevel, enum_LevelStyle>();
    public enum_LevelStyle m_GameStyle => m_StageStyle[m_GameStage];
    public enum_StageLevel m_GameStage { get; private set; }
   
    #endregion
    #region RecordData
    public bool m_gameWin { get; private set; }
    int m_levelEntered;
    int m_battleLevelEntered;
    #endregion
    public GameProgressManager(CGameSave _gameSave,CBattleSave _battleSave)
    {
        m_GameSeed =_battleSave.m_GameSeed;
        m_GameRandom = new System.Random(m_GameSeed.GetHashCode());
        m_GameStage = _battleSave.m_Stage;
        m_GameDifficulty = 3; //_gameSave.m_GameDifficulty;
        List<enum_LevelStyle> styleList = TCommon.GetEnumList<enum_LevelStyle>();
        TCommon.TraversalEnum((enum_StageLevel level) => {
            enum_LevelStyle style = styleList.RandomItem(m_GameRandom);
            styleList.Remove(style);
            m_StageStyle.Add(level, style);
        });
    }
    public void LoadStageData()
    {
        m_InteractGenerate = GameExpression.GetInteractGenerate(m_GameStage);
        m_EnermyGenerate = GameDataManager.GetEnermyGenerate(m_GameStage);

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
    public static void Clear()
    {
        ObjectPoolManager<int, SFXBase>.DestroyAll();
        ObjectPoolManager<int, SFXWeaponBase>.DestroyAll();
        ObjectPoolManager<int, EntityBase>.DestroyAll();
        ObjectPoolManager<enum_Interaction, InteractGameBase>.DestroyAll();
        ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.DestroyAll();
    }
    #region Register
    public static void PresetRegistCommonObject()
    {
        TResources.GetAllEffectSFX().Traversal((int index, SFXBase 
            target) => {ObjectPoolManager<int, SFXBase>.Register(index, target, 1); });
        TResources.GetCommonEntities().Traversal((int index, EntityBase entity) => { ObjectPoolManager<int, EntityBase>.Register(index, entity, 1); });
    }
    public static Dictionary<enum_EnermyType, List<int>> RegistStyledInGamePrefabs(enum_LevelStyle currentStyle, enum_StageLevel stageLevel)
    {
        RegisterInGameInteractions(currentStyle, stageLevel);

        Dictionary<enum_EnermyType, List<int>> enermyDic = new Dictionary<enum_EnermyType, List<int>>();
        TResources.GetEnermyEntities(currentStyle).Traversal((int index, EntityBase entity) => {
            ObjectPoolManager<int, EntityBase>.Register(index, entity, 1 );
            EntityCharacterBase enermy = entity as EntityCharacterBase;
            if (enermy.E_SpawnType == enum_EnermyType.Invalid)
                return;
            if (!enermyDic.ContainsKey(enermy.E_SpawnType))
                enermyDic.Add(enermy.E_SpawnType, new List<int>());
            enermyDic[enermy.E_SpawnType].Add(index);
        });
        return enermyDic;
    }
    static void RegisterInGameInteractions(enum_LevelStyle portalStyle, enum_StageLevel stageIndex)
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
    static T SpawnEntity<T>(int _poolIndex, Vector3 pos,Quaternion rot,Action<T> OnActivate, Transform parentTrans = null) where T:EntityBase
    {
        T entity = ObjectPoolManager<int, EntityBase>.Spawn(_poolIndex, TF_Entity, NavigationManager.NavMeshPosition(pos),rot) as T;
        if (entity == null)
            Debug.LogError("Entity ID:" + _poolIndex + ",Type:" + typeof(T).ToString() + " Not Found");
        OnActivate(entity);
        entity.gameObject.name = entity.m_EntityID.ToString() + "_" + _poolIndex.ToString();
        if (parentTrans) entity.transform.SetParent(parentTrans);
        return entity;
    }

    public static EntityCharacterAI SpawnEntityCharacterAI(int poolIndex, Vector3 toPosition, Vector3 lookPos, enum_EntityFlag _flag,int gameDifficulty,enum_StageLevel _stage,bool battling, int spawnerID = -1, float _startHealth = 0)=> SpawnEntity(poolIndex, toPosition, Quaternion.LookRotation(TCommon.GetXZLookDirection(toPosition, lookPos),Vector3.up), (EntityCharacterAI ai)=> ai.OnActivate(_flag, spawnerID, _startHealth, GameExpression.GetEnermyMaxHealthMultiplier(_stage, gameDifficulty), GameExpression.GetEnermyGameBuff(_stage, gameDifficulty), battling), null);

    public static EntityCharacterPlayer SpawnEntityPlayer(CBattleSave playerSave,Vector3 position,Quaternion rotation)=> SpawnEntity((int)playerSave.m_character, position,rotation, (EntityCharacterPlayer player)=>player.OnActivate(playerSave));

    public static EntityNPC SpawnNPC(enum_InteractCharacter npc, Vector3 toPosition,Quaternion rot) => SpawnEntity((int)npc, toPosition,rot,(EntityNPC npcCharacter)=> npcCharacter.OnActivate());

    public static void RecycleEntity(int index, EntityBase target) => ObjectPoolManager<int, EntityBase>.Recycle(index, target);
    #endregion
    #region Weapon
    public static WeaponBase SpawnWeapon(WeaponSaveData weaponData,Transform toTrans=null)
    {
        if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(weaponData.m_Weapon))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(weaponData.m_Weapon);
            ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(weaponData.m_Weapon, preset, 1);
        }
        WeaponBase targetWeapon = ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(weaponData.m_Weapon, toTrans ? toTrans : TF_Entity,Vector3.zero,Quaternion.identity);
        return targetWeapon;
    }
    public static void RecycleWeapon(WeaponBase weapon)=> ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Recycle(weapon.m_WeaponInfo.m_Weapon,weapon);
    #endregion
    #region SFX
    public static T SpawnSFX<T>(int index, Vector3 position, Vector3 normal) where T : SFXBase
    {
        T sfx = ObjectPoolManager<int, SFXBase>.Spawn(index, TF_SFXPlaying,position, Quaternion.LookRotation(normal)) as T;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid SFX Type:" + typeof(T) + ",Index:" + index);
        return sfx;
    }
    public static SFXIndicator SpawnIndicator(int _sourceID, Vector3 position, Vector3 normal) => SpawnSFX<SFXIndicator>(_sourceID, position, normal);
    public static SFXEffect SpawnBuffEffect(int _sourceID) => SpawnSFX<SFXEffect>(_sourceID,Vector3.zero,Vector3.up);

    public static void PlayMuzzle(int _sourceID,Vector3 position, Vector3 direction, int muzzleIndex, AudioClip muzzleClip=null)
    {
        if (muzzleIndex > 0)
            SpawnSFX<SFXMuzzle>(muzzleIndex, position, direction).Play(_sourceID);
        if (muzzleClip)
            AudioManager.Instance.Play3DClip(_sourceID, muzzleClip, false, position);
    }

    public static T SpawnEquipment<T>(int weaponIndex, Vector3 position, Vector3 normal) where T : SFXWeaponBase
    {
        if (!ObjectPoolManager<int, SFXWeaponBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXWeaponBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1);

        T template = ObjectPoolManager<int, SFXWeaponBase>.Spawn(weaponIndex, TF_SFXWeapon,position,Quaternion.LookRotation(normal)) as T;
        if (template == null)
            Debug.LogError("Enermy Weapon Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return template;
    }
    public static T GetEquipmentData<T>(int weaponIndex) where T : SFXWeaponBase
    {
        if (!ObjectPoolManager<int, SFXWeaponBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXWeaponBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1);

        T damageSourceInfo = ObjectPoolManager<int, SFXWeaponBase>.GetRegistedSpawnItem(weaponIndex) as T;
        if (damageSourceInfo == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return damageSourceInfo;
    }
    public static void RecycleAllWeapon(Predicate<SFXWeaponBase> predicate) => ObjectPoolManager<int, SFXWeaponBase>.RecycleAll(predicate);
    #endregion
    #region Interact
    public static T SpawnInteract<T>(enum_Interaction type, Vector3 pos,Quaternion rot, Transform toTrans=null) where T : InteractGameBase
    {
        T target = ObjectPoolManager<enum_Interaction, InteractGameBase>.Spawn(type , toTrans==null? TF_SFXPlaying : toTrans,pos,rot) as T;
        return target;
    }
    public static void RecycleInteract(InteractGameBase target) => ObjectPoolManager<enum_Interaction, InteractGameBase>.Recycle(target.m_InteractType,target);
    public static void RecycleAllInteract() => ObjectPoolManager<enum_Interaction, InteractGameBase>.RecycleAll();
    #endregion
    #endregion
}

#endregion