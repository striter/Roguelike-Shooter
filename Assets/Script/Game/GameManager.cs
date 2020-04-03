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
    public string M_TESTSEED = "";
    public bool B_PhysicsDebugGizmos = true;
    void AddConsoleBinddings()
    {
        UIT_MobileConsole.Instance.InitConsole((bool show) => { Time.timeScale = show ? .1f : 1f; });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Show Seed",KeyCode.None,()=> { Debug.LogError(m_GameLevel.m_GameSeed); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Next Level", KeyCode.Minus, () => { OnChunkPortalEnter(m_GameLevel.GetNextLevelGenerate(m_LocalPlayer).m_PortalMain); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Next Stage", KeyCode.Equals, () => { OnChunkPortalEnter(m_GameLevel.m_FinalStage ? enum_LevelType.GameWin : enum_LevelType.StageEnd); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Test Level", KeyCode.Backspace, (int)enum_LevelType.Trader, enum_LevelType.Trader,(int index)=> { OnChunkPortalEnter((enum_LevelType)index); });

        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Kill All",  KeyCode.Alpha0, () => {
            GetCharacters(enum_EntityFlag.Enermy, true).Traversal((EntityCharacterBase character) =>
            {
                character.m_HitCheck.TryHit(new DamageInfo(-1,character.m_Health.m_CurrentHealth, enum_DamageType.Basic));
            });
        });

        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Enermy", KeyCode.Z, "101", (string id) => {
            GameObjectManager.SpawnEntityCharacterAI(int.Parse(id), NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity, enum_EntityFlag.Enermy, m_GameLevel.m_GameDifficulty, m_GameLevel.m_StageIndex);
        });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Damage", KeyCode.N, "20", (string damage) => { m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-1,int.Parse(damage), enum_DamageType.Basic));});
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Heal",  KeyCode.M, "20", (string damage) => { m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-1,-int.Parse(damage), enum_DamageType.Basic)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Perk Item",  KeyCode.F1, "1", (string actionIndex) => { GameObjectManager.SpawnInteract<InteractPerkPickup>( NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere()* 5f), Quaternion.identity).Play(int.Parse(actionIndex)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Perk Select", KeyCode.F2, SpawnBattleFinishReward);
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Coins", KeyCode.F5, "20", (string coins) => { GameObjectManager.SpawnInteract<InteractPickupCoin>( NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere()*5f), Quaternion.identity).Play(int.Parse(coins),!m_Battling);});
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Health", KeyCode.F6, "20", (string health) => {GameObjectManager.SpawnInteract<InteractPickupHealth>( NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere()*5f), Quaternion.identity).Play(int.Parse(health), !m_Battling);});
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Armor", KeyCode.F7, "20", (string armor) => {GameObjectManager.SpawnInteract<InteractPickupArmor>(  NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere()* 5f), Quaternion.identity).Play(int.Parse(armor), !m_Battling);});
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Weapon", KeyCode.F8, (int)enum_PlayerWeapon.Railgun, enum_PlayerWeapon.Railgun,(int weaponIdentity) => {
            GameObjectManager.SpawnInteract<InteractWeaponPickup>( NavigationManager.NavMeshPosition(m_LocalPlayer.transform.position + TCommon.RandomXZSphere()* 5f), Quaternion.identity).Play(WeaponSaveData.CreateNew((enum_PlayerWeapon)weaponIdentity)); }); UIT_MobileConsole.Instance.AddConsoleBinding().Play("Toggle HealthBar", KeyCode.None,()=> GameUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().m_HealthGrid.transform.SetActivate(!GameUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().m_HealthGrid.transform.gameObject.activeSelf));
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Clear Console", KeyCode.None, UIT_MobileConsole.Instance.ClearConsoleLog);
    }
    #endregion
    protected static GameManager nInstance;
    public static new GameManager Instance => nInstance;
    public GameProgressManager m_GameLevel { get; private set; }
    public EntityCharacterPlayer m_LocalPlayer { get; private set; } = null;
    Transform tf_CameraAttach;
    ChunkGameObjectData m_PlayerStart,m_PortalMain,m_PortalExtra;

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
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        if (M_TESTSEED!="")
            GameDataManager.m_BattleData.m_GameSeed = M_TESTSEED;
        m_GameLevel =  new GameProgressManager(GameDataManager.m_GameData,GameDataManager.m_BattleData);
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
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
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
        tf_CameraAttach.position = CalculateCameraPosition();
    }

    Vector3 CalculateCameraPosition()
    {
        Vector3 position = GameLevelManager.Instance.m_LevelCenter;
        Vector3 offset =   m_LocalPlayer.transform.position- position;
        position += Vector3.right * GameExpression.GetCameraSmoothInterpolate(offset.x,GameLevelManager.Instance.m_LevelWidth); 
        position += Vector3.forward * GameExpression.GetCameraSmoothInterpolate(offset.z, GameLevelManager.Instance.m_LevelHeight);
        return position;
    }

    //Call When Level Changed
    void LoadStage() => this.StartSingleCoroutine(999, DoLoadStage());
    IEnumerator DoLoadStage()     //PreInit Bigmap , Levels LocalPlayer Before  Start The game
    {
        m_GameLoading = true;
        LoadingManager.Instance.ShowLoading(m_GameLevel.m_StageIndex);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameLoad);
        yield return null;
        m_GameLevel.StageInit();
        InitGameEffects(m_GameLevel.m_GameStyle);
        GameLevelManager.Instance.GenerateStage(m_GameLevel.m_GameStyle, m_GameLevel.m_Random);

        EntityDicReset();
        GameObjectManager.Clear();
        GameObjectManager.PresetRegistCommonObject();
        m_EnermySpawnIDs = GameObjectManager.RegistStyledInGamePrefabs(m_GameLevel.m_GameStyle, m_GameLevel.m_StageIndex);

        yield return null;
        Resources.UnloadUnusedAssets();
        GC.Collect();
        yield return null;

        m_LocalPlayer = GameObjectManager.SpawnEntityPlayer(GameDataManager.m_BattleData.m_PlayerData, Vector3.zero, Quaternion.identity);
        AttachPlayerCamera(tf_CameraAttach);
        OnLevelStart();

        m_GameLoading = false;
        LoadingManager.Instance.EndLoading();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnGameBegin);
    }


    void OnLevelStart()
    {
        GameObjectManager.RecycleAllInteract();
        m_EnermySpawnPoints.Clear();
        GameLevelManager.Instance.OnStartLevel(m_GameLevel.m_LevelType.GetChunkType(), m_GameLevel.m_Random, OnGenerateLevelGameRelatives);
        m_LocalPlayer.Teleport(m_PlayerStart.pos, m_PlayerStart.rot);
        CameraController.Instance.SetCameraPosition(CalculateCameraPosition());

        if (m_GameLevel.m_BattleLevel)
            OnBattleStart();
        else
            OnGenerateLevelPortals();
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnLevelStart);
        OnPortalExit(1f, tf_CameraAttach);
    }

    void OnGenerateLevelGameRelatives(enum_TileObjectType tileType, ChunkGameObjectData objectData)
    {
        switch (tileType)
        {
            case enum_TileObjectType.EEntrance1x1:
                m_PlayerStart=objectData;
                break;
            case enum_TileObjectType.EPortalMain2x1:
                m_PortalMain= objectData;
                break;
            case enum_TileObjectType.EPortalExtra2x1:
                m_PortalExtra= objectData;
                break;
            case enum_TileObjectType.EEnermySpawn1x1:
                m_EnermySpawnPoints.Add(objectData.pos);
                break;
            case enum_TileObjectType.EEventArea3x3:
                switch (m_GameLevel.m_LevelType)
                {
                    case enum_LevelType.StageStart:
                         GameObjectManager.SpawnInteract<InteractPerkSelect>(objectData.pos, objectData.rot).Play(PerkDataManager.RandomPerks(3, GameConst.D_EventPerkRareSelectRate, m_LocalPlayer.m_CharacterInfo.m_ExpirePerks, m_GameLevel.m_Random));
                        break;
                    case enum_LevelType.Trader:
                        enum_Rarity rarity = TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_TradePerk, m_GameLevel.m_Random);

                        GameObjectManager.SpawnInteract<InteractTradeContainer>( objectData.pos + LevelConst.I_TileSize * Vector3.left, objectData.rot).Play(GameExpression.GetEventTradePrice( enum_Interaction.PerkPickup,rarity).Random(m_GameLevel.m_Random), GameObjectManager.SpawnInteract<InteractPerkPickup>( objectData.pos, objectData.rot).Play(PerkDataManager.RandomPerk(rarity, m_GameLevel.m_Random)));

                        rarity = TCommon.RandomPercentage(m_GameLevel.m_InteractGenerate.m_TradeWeapon, m_GameLevel.m_Random);
                        GameObjectManager.SpawnInteract<InteractTradeContainer>( objectData.pos + LevelConst.I_TileSize * Vector3.forward, objectData.rot).Play(GameConst.D_EventWeaponTradePrice[rarity].Random(m_GameLevel.m_Random), GameObjectManager.SpawnInteract<InteractWeaponPickup>( Vector3.zero,Quaternion.identity).Play(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[rarity].RandomItem(m_GameLevel.m_Random))));

                        GameObjectManager.SpawnInteract<InteractTradeContainer>( objectData.pos + LevelConst.I_TileSize * Vector3.right, objectData.rot).Play(GameExpression.GetEventTradePrice(enum_Interaction.PickupHealthPack).Random(m_GameLevel.m_Random), GameObjectManager.SpawnInteract<InteractPickupHealthPack>( objectData.pos, objectData.rot).Play(GameConst.I_HealthPackAmount,false));
                        break;
                    case enum_LevelType.WeaponReforge:
                        GameObjectManager.SpawnInteract<InteractWeaponReforge>(objectData.pos, objectData.rot).Play();
                        break;
                    case enum_LevelType.Bonefire:
                        GameObjectManager.SpawnInteract<InteractBonfire>(objectData.pos, objectData.rot).Play();
                        break;
                    case enum_LevelType.WeaponVendorRare:
                        GameObjectManager.SpawnInteract<InteractWeaponVendorMachineRare>(objectData.pos, objectData.rot).Play();
                        break;
                    case enum_LevelType.WeaponVendorNormal:
                        GameObjectManager.SpawnInteract<InteractWeaponVendorMachineNormal>(objectData.pos, objectData.rot).Play();
                        break;
                    case enum_LevelType.PerkRare:
                        GameObjectManager.SpawnInteract<InteractTradeContainer>(objectData.pos, objectData.rot).Play(GameConst.I_EventPerkRarePrice,GameObjectManager.SpawnInteract<InteractPerkPickup>(Vector3.zero,Quaternion.identity).Play(PerkDataManager.RandomPerk( enum_Rarity.Rare,m_GameLevel.m_Random)));
                        break;
                    case enum_LevelType.PerkFill:
                        GameObjectManager.SpawnInteract<InteractPerkFill>(objectData.pos, objectData.rot).Play(PerkDataManager.RandomPerks(2,GameConst.D_EventPerkRareRate,m_LocalPlayer.m_CharacterInfo.m_ExpirePerks));
                        break;
                    case enum_LevelType.PerkRareSelect:
                        GameObjectManager.SpawnInteract<InteractTradeContainer>(objectData.pos, objectData.rot).Play(GameConst.I_EventPerkRareSelectPrice, GameObjectManager.SpawnInteract<InteractPerkSelect>(Vector3.zero, Quaternion.identity).Play(PerkDataManager.RandomPerks(3, GameConst.D_EventPerkRareSelectRate, m_LocalPlayer.m_CharacterInfo.m_ExpirePerks, m_GameLevel.m_Random)));
                        break;
                    case enum_LevelType.WeaponRecycle:
                        GameObjectManager.SpawnInteract<InteractWeaponRecycle>(objectData.pos, objectData.rot).Play();
                        break;
                    case enum_LevelType.SafeCrack:
                        GameObjectManager.SpawnInteract<InteractSafeCrack>(objectData.pos, objectData.rot).Play();
                        break;
                }
                break;
        }
    }

    void OnGenerateLevelPortals()
    {
        GameLevelPortalData data = m_GameLevel.GetNextLevelGenerate(m_LocalPlayer);
        GameObjectManager.SpawnInteract<InteractPortal>( m_PortalMain.pos, m_PortalMain.rot).Play(data.m_PortalMain, OnChunkPortalEnter);

        if(data.m_PortalExtra!= enum_LevelType.Invalid)
            GameObjectManager.SpawnInteract<InteractPortal>( m_PortalExtra.pos, m_PortalExtra.rot).Play( data.m_PortalExtra, OnChunkPortalEnter);
    }

    void OnChunkPortalEnter(enum_LevelType levelType)
    {
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnLevelFinished);

        if (levelType== enum_LevelType.StageEnd)
        {
            m_GameLevel.StageFinished();
            TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnStageFinished);
            GameDataManager.AdjustInGameData(m_LocalPlayer, m_GameLevel);
            OnPortalEnter(1f, tf_CameraAttach, LoadStage);
            return;
        }
        else if(levelType== enum_LevelType.GameWin)
        {
            OnGameFinished(true);
            return;
        }

        m_GameLevel.LevelFinished(levelType);
        OnPortalEnter(1f, tf_CameraAttach, OnLevelStart);
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

        OnBattleEntityKilled(character);
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
    #region Game Item Management
    void SpawnBattleFinishReward()
    {
        GameObjectManager.SpawnInteract<InteractPerkSelect>( GetPickupPosition(m_LocalPlayer), Quaternion.identity).Play(PerkDataManager.RandomPerks(3,GameConst.D_BattleFinishPerkGenerate,m_LocalPlayer.m_CharacterInfo.m_ExpirePerks));
    }

    void SpawnBattleEntityDeadDrops(EntityCharacterBase entity)
    {
        EntityCharacterAI enermy = entity as EntityCharacterAI;
        if (enermy==null|| enermy.E_SpawnType== enum_EnermyType.Invalid)
            return;

        PickupGenerateData pickupGenerateData = enermy.E_SpawnType == enum_EnermyType.E5 ? m_GameLevel.m_InteractGenerate.m_ElitePickupData : m_GameLevel.m_InteractGenerate.m_NormalPickupData;

        if (pickupGenerateData.CanGenerateHealth())
            GameObjectManager.SpawnInteract<InteractPickupHealth>( GetPickupPosition(entity), Quaternion.identity).Play(GameConst.I_HealthPickupAmount, false);

        if (pickupGenerateData.CanGenerateArmor())
            GameObjectManager.SpawnInteract<InteractPickupArmor>( GetPickupPosition(entity), Quaternion.identity).Play(GameConst.I_ArmorPickupAmount, false);

        int amount;
        if (pickupGenerateData.CanGenerateCoins(out amount))
            GameObjectManager.SpawnInteract<InteractPickupCoin>( GetPickupPosition(entity), Quaternion.identity).Play(amount, false);
        
        enum_Rarity weaponRarity = TCommon.RandomPercentage(pickupGenerateData.m_WeaponRate, enum_Rarity.Invalid);
        if (weaponRarity != enum_Rarity.Invalid)
            GameObjectManager.SpawnInteract<InteractWeaponPickup>( GetPickupPosition(entity), Quaternion.identity).Play(WeaponSaveData.CreateNew(GameDataManager.m_WeaponRarities[weaponRarity].RandomItem()));
    }

    Vector3 GetPickupPosition(EntityCharacterBase dropper) => NavigationManager.NavMeshPosition(dropper.transform.position + TCommon.RandomXZSphere()* 1.5f);
    #endregion
    #region Battle Relatives 
    List<Vector3> m_EnermySpawnPoints = new List<Vector3>();
    public Dictionary<enum_EnermyType, int> m_EnermySpawnIDs;
    public bool m_Battling { get; private set; } = false;

    void OnBattleStart()
    {
        if (m_Battling)
        {
            Debug.LogError("Can't Trigger Another Battle!");
            return;
        }
        m_Battling = true;
        GenerateBattleEnermies(m_GameLevel.OnBattleStart());
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleStart);
    }

    void OnBattleEntityKilled(EntityCharacterBase character)
    {
        if (!m_Battling)
            return;

        if (character.m_Flag != enum_EntityFlag.Enermy)
            return;

        SpawnBattleEntityDeadDrops(character);
        m_GameLevel.OnBattleEnermyKilled();

        if (GetCharacters(enum_EntityFlag.Enermy, true).Any(p => !p.m_IsDead))
            return;
        SEnermyGenerate generate;
        if(!m_GameLevel.OnBattleWaveFinished(out generate))
        {
            GenerateBattleEnermies(generate);
            return;
        }
        OnBattleFinish();
    }
    void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float applyAmount)
    {
        if (damageEntity.m_Flag== enum_EntityFlag.Enermy && applyAmount <= 0)
            return;
        m_GameLevel.OnBattleDamageDealt(applyAmount);
    }


    void OnBattleFinish()
    {
        m_Battling = false;
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnBattleFinish);
        SpawnBattleFinishReward();
        OnGenerateLevelPortals();
    }

    void GenerateBattleEnermies(SEnermyGenerate enermyGenerate)
    {
        int spawnPointCount = 0;
        enermyGenerate.GetEnermyIDList(m_EnermySpawnIDs).Traversal((int enermyID) => {
            GameObjectManager.SpawnEntityCharacterAI(enermyID, m_EnermySpawnPoints[spawnPointCount], Quaternion.identity, enum_EntityFlag.Enermy, m_GameLevel.m_GameDifficulty, m_GameLevel.m_StageIndex);
            spawnPointCount++;
            if (spawnPointCount == m_EnermySpawnPoints.Count)
                spawnPointCount = 0;
        });
    }
    #endregion
}

#region External Tools Packaging Class
public class GameProgressManager
{
    #region GameData
    public string m_GameSeed { get; private set; }
    public int m_GameDifficulty { get; private set; }
    public enum_Stage m_StageIndex { get; private set; }
    Dictionary<enum_Stage, enum_GameStyle> m_StageStyle = new Dictionary<enum_Stage, enum_GameStyle>();
    public enum_GameStyle m_GameStyle => m_StageStyle[m_StageIndex];
    public bool m_GameWin { get; private set; }
    public int m_LevelPassed { get; private set; }
    #endregion
    #region LevelData
    public System.Random m_Random { get; private set; }
    public StageInteractGenerateData m_InteractGenerate { get; private set; }
    public enum_LevelType m_LevelType { get; private set; }
    public int m_LevelIndex { get; private set; }
    Dictionary<bool, List<SEnermyGenerate>> m_EnermyGenerate;
    public List<int> m_SelectLevelIndexes { get; private set; } = new List<int>();
    public int m_BattleWave { get; private set; }
    public int m_BattleEnermiesKilled { get; private set; }
    public float m_BattleDamageDealt { get; private set; }
    #endregion
    #region Get
    public bool m_FinalStage => m_StageIndex == enum_Stage.Ranger;
    public bool m_BattleLevel => m_LevelType.IsBattleLevel();
    public bool m_FinalLevel => m_LevelIndex == 9;
    #endregion
    public GameProgressManager(CGameSave _gameSave,CBattleSave _battleSave)
    {
        m_StageIndex = _battleSave.m_Stage;
        m_GameDifficulty = _gameSave.m_GameDifficulty;
        m_GameSeed =_battleSave.m_GameSeed;
        m_LevelPassed = _battleSave.m_LevelPassed;
        m_Random = new System.Random(m_GameSeed.GetHashCode());
        List<enum_GameStyle> styleList = TCommon.GetEnumList<enum_GameStyle>();
        TCommon.TraversalEnum((enum_Stage level) => {
            enum_GameStyle style = styleList.RandomItem(m_Random);
            styleList.Remove(style);
            m_StageStyle.Add(level, style);
        });
    }
    public void StageInit()
    {
        m_InteractGenerate = GameExpression.GetInteractGenerate(m_StageIndex);
        m_EnermyGenerate = GameDataManager.GetEnermyGenerate(m_StageIndex,m_GameStyle);
        m_Random = new System.Random((m_GameSeed + m_StageIndex.ToString()).GetHashCode());

        m_LevelIndex = 0;
        m_LevelType = enum_LevelType.StageStart;

        List<int> selectionIndex = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
        m_SelectLevelIndexes.Clear();
        for (int i=0;i<3;i++)
        {
            int randomSelect = selectionIndex.RandomItem(m_Random);
            selectionIndex.Remove(randomSelect);
            m_SelectLevelIndexes.Add(randomSelect);
        }
        m_SelectLevelIndexes.Sort((int a, int b) => a - b);
    }

    #region BattleData
    public SEnermyGenerate OnBattleStart()
    {
        m_BattleWave = 0;
        m_BattleEnermiesKilled = 0;
        m_BattleDamageDealt = 0;
        return GetBattleWaveData();
    }

    public void OnBattleEnermyKilled()
    {
        m_BattleEnermiesKilled++;
        if (m_LevelType == enum_LevelType.EndlessBattle)
            TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEndlessData, m_BattleEnermiesKilled, m_BattleDamageDealt);
    }

    public void OnBattleDamageDealt(float damage)
    {
        m_BattleDamageDealt += damage;
        if (m_LevelType == enum_LevelType.EndlessBattle)
            TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnEndlessData,m_BattleEnermiesKilled,m_BattleDamageDealt);
    }

    public bool OnBattleWaveFinished(out SEnermyGenerate nextWaveData)
    {
        m_BattleWave++;
        nextWaveData = GetBattleWaveData();
        if (m_LevelType== enum_LevelType.EndlessBattle)
            return false;
        return true;
    }

    protected SEnermyGenerate GetBattleWaveData()
    {
        switch(m_LevelType)
        {
            case enum_LevelType.EndlessBattle:
                {
                    SEnermyGenerate generate = m_EnermyGenerate[false].RandomItem();
                    int generateAdditive = m_BattleWave;
                    for (int i = 0; i < generateAdditive; i++)
                        generate += m_EnermyGenerate[false].RandomItem();
                    return generate;
                }
            case enum_LevelType.NormalBattle:
                return m_EnermyGenerate[false].RandomItem();
            case enum_LevelType.StageFinalBattle:
                return m_EnermyGenerate[true].RandomItem();
            case enum_LevelType.EliteBattle:
                return m_EnermyGenerate[false].RandomItem() + m_EnermyGenerate[false].RandomItem();
        }
        Debug.LogError("Invalid Battle Wave Data Found!");
        return new SEnermyGenerate();
    }
    #endregion
    #region Level
    public GameLevelPortalData GetNextLevelGenerate(EntityCharacterPlayer player)
    {
        if (m_LevelIndex == 8)
            return new GameLevelPortalData(enum_LevelType.StageFinalBattle, enum_LevelType.Invalid);

        if (m_FinalLevel)
            return new GameLevelPortalData(m_FinalStage? enum_LevelType.GameWin:enum_LevelType.StageEnd, enum_LevelType.EndlessBattle);

        int selectionIndex = m_SelectLevelIndexes.FindIndex(p=>p==m_LevelIndex);
        if (selectionIndex == -1)
            return new GameLevelPortalData(enum_LevelType.NormalBattle, enum_LevelType.Invalid);

        enum_LevelType _mainType = (player.m_CharacterInfo.m_Coins <= 10 || TCommon.RandomBool(m_Random)) ? enum_LevelType.NormalBattle : GetNextPortal(enum_LevelType.Invalid, m_Random);
        enum_LevelType _subType = selectionIndex == 1 ? enum_LevelType.Trader : GetNextPortal(_mainType, m_Random);
        return new GameLevelPortalData(_mainType, _subType);
    }

    enum_LevelType GetNextPortal(enum_LevelType exclude, System.Random _random)
    {
        List<enum_LevelType> levelPool;
        if (TCommon.RandomPercentage(_random) <= GameConst.I_RewardLevelRate)
            levelPool = new List<enum_LevelType>(GameConst.m_RewardLevelsPool);
        else
            levelPool = new List<enum_LevelType>(GameConst.m_NormalLevelsPool);
        if (levelPool.Contains(exclude))
            levelPool.Remove(exclude);
        return levelPool.RandomItem(_random);
    }

    public void LevelFinished(enum_LevelType nextLevel)
    {
        m_LevelIndex++;
        m_LevelPassed++;
        m_LevelType = nextLevel;
    }
    #endregion
    public void StageFinished()=>  m_StageIndex++;
    
    public void GameFinished(bool win)=> m_GameWin = win;

    #region CalculateData
    public float F_Completion => GameExpression.GetResultCompletion(m_GameWin, m_StageIndex, 0);
    public float F_CompletionScore => GameExpression.GetResultLevelScore(m_StageIndex, 0);
    public float F_DifficultyBonus => GameExpression.GetResultDifficultyBonus(m_GameDifficulty);
    public float F_FinalScore =>  F_CompletionScore *  F_DifficultyBonus;
    public float F_CreditGain => GameExpression.GetResultRewardCredits(F_FinalScore);
    #endregion

    public string GetLevelIconSprite() => m_LevelType.GetLevelIconSprite();
    public string GetLevelInfoKey() => m_LevelType.GetLocalizeNameKey();
}
#endregion