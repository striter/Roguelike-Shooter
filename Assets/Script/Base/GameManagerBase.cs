using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TExcel;
using TGameSave;
using System.Linq;

public class GameManagerBase : SingletonMono<GameManagerBase>,ICoroutineHelperClass{
    public virtual bool B_InGame => false;
    protected override void Awake()
    {
        base.Awake();
        LoadingManager.OnOtherSceneEnter(B_InGame? enum_Scene.Game: enum_Scene.Camp);
        TBroadCaster<enum_BC_GameStatus>.Init();
        TBroadCaster<enum_BC_UIStatus>.Init();
        GameIdentificationManager.Init();
        GameDataManager.Init();
        PerkDataManager.Init();
        OptionsManager.Init();
    }

    protected virtual void Start()
    {
        GameObjectManager.Init();
        AudioManagerBase.Instance.Init();
        GameObjectManager.PresetRegistCommonObject();
        UIManager.Activate(B_InGame);

        OnCommonOptionChanged();
        OptionsManager.event_OptionChanged += OnCommonOptionChanged;
        OptionsManager.event_OptionChanged += OnEffectOptionChanged;
        SetBulletTime(false);
    }

    protected virtual void OnDisable()
    {
        this.StopAllSingleCoroutines();
        OptionsManager.event_OptionChanged -= OnCommonOptionChanged;
        OptionsManager.event_OptionChanged -= OnEffectOptionChanged;
    }

    protected void SwitchScene(enum_Scene scene,Func<bool> onfinishLoading=null)
    {
        AudioManagerBase.Instance.Recycle();
        GameObjectManager.Clear();
        LevelObjectManager.Clear();
        UIManager.Instance.SetActivate(false);
        LoadingManager.BeginLoad(scene,onfinishLoading);
    }

    void OnCommonOptionChanged()
    {
        TLocalization.SetRegion(OptionsManager.m_OptionsData.m_Region);
        Application.targetFrameRate = (int)OptionsManager.m_OptionsData.m_FrameRate;
    }

    protected void OnPortalEnter(float duration,Transform vortexTarget, Action OnEnter)
    {
        SetBulletTime(true, .1f);
        SetPostEffect_Vortex(true, vortexTarget, 1f,OnEnter);
    }

    protected void OnPortalExit(float duration,Transform vortexTarget)
    {
        SetBulletTime(false);
        SetPostEffect_Vortex(false, vortexTarget, 1f);
    }
    #region Game Effect
    protected static float m_BulletTime = 1f;
    public static bool m_BulletTiming => m_BulletTime != 1f;
    public static void SetBulletTime(bool enter,float timeScale=.8f)
    {
        m_BulletTime = enter ? timeScale:1f ;
        Time.timeScale = m_BulletTime;
    }

    PE_BloomSpecific m_Bloom;
    protected void InitGameEffects(enum_GameStyle _levelStyle)
    {
        CameraController.Instance.m_Effect.RemoveAllPostEffect();
        //CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthOutline>().SetEffect(Color.black,1.2f,0.0001f);
        //CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthSSAO>().SetEffect();
        m_Bloom = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BloomSpecific>();
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<CB_GenerateOpaqueTexture>();
        switch (_levelStyle)
        {
            case  enum_GameStyle.Undead:
                //CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_FogDepthNoise>().SetEffect<PE_FogDepthNoise>(TCommon.ColorAlpha(Color.white, .3f), .5f, -2, -3).SetEffect(TResources.GetNoiseTex(), .4f, 2f);
                break;
            case enum_GameStyle.Frost:
                CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_FogDepth>().SetEffect<PE_FogDepth>(Color.white, .5f, -2, 3);
                break;
        }
        OnEffectOptionChanged();
    }

    void OnEffectOptionChanged()
    {
        CameraController.Instance.m_Effect.SetCameraEffects(OptionsManager.m_OptionsData.m_Effect == enum_Option_Effect.High ? DepthTextureMode.Depth : DepthTextureMode.None);
        if (OptionsManager.m_OptionsData.m_Bloom == enum_Option_Bloom.Normal)
            m_Bloom.m_Blur.SetEffect(PE_Blurs.enum_BlurType.GaussianBlur, 4, 5, 2);
        else if (OptionsManager.m_OptionsData.m_Bloom == enum_Option_Bloom.High)
            m_Bloom.m_Blur.SetEffect(PE_Blurs.enum_BlurType.GaussianBlur, 2, 10, 2);

        m_Bloom.SetEnable(OptionsManager.m_OptionsData.m_Bloom >= enum_Option_Bloom.Normal, OptionsManager.m_OptionsData.m_Bloom >= enum_Option_Bloom.High);
    }

    protected void SetPostEffect_Dead()
    {
        PE_BSC m_BSC = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BSC>();
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_BSC.SetEffect(Mathf.Lerp(1f,.2f,value), Mathf.Lerp(1f, 0f, value), Mathf.Lerp(1f, .8f, value)); }, 0, 1f, 3f));
    }
    protected void SetPostEffect_Revive()
    {
        PE_BSC m_BSC = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BSC>();
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_Bloom>().SetEffect(2, 3, 4,.9f,2f);
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_BSC.SetEffect(value, 1f, 1f); }, 2f, 1, 2f,
             CameraController.Instance.m_Effect.RemoveCameraEffect<PE_Bloom>));
    }

    private void SetPostEffect_Vortex(bool on,Transform target,float duration,Action OnEnter=null)
    {
        PE_DistortVortex vortex = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DistortVortex>();
        vortex.SetTexture(TResources.GetNoiseTex());
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
         {
             vortex.SetDistort(CameraController.Instance.m_Camera.WorldToViewportPoint(target.position),value);
         },on?0:1,on?1:0, duration,()=> {
             CameraController.Instance.m_Effect.RemoveCameraEffect<PE_DistortVortex>();
             OnEnter?.Invoke();
         } ,false));
    }
    public void SetEffect_Shake(float amount)
    {
        TPSCameraController.Instance.AddShake(amount*GameConst.F_DamageImpactMultiply);
        //Handheld.Vibrate();
    }
    public void SetEffect_Impact(Vector3 direction)
    {
        TPSCameraController.Instance.SetImpact(direction.normalized*GameConst.F_DamageImpactMultiply);
    }

    public void SetEffect_Focal(bool on,Transform target=null, float width=1f,float duration=2f)
    {
        if (!on)
        {
            CameraController.Instance.m_Effect.RemoveCameraEffect<PE_FocalDepth>();
            return;
        }
        PE_FocalDepth focal= CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_FocalDepth>();
        focal.SetEffect(2);
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
         {
             focal.SetFocalTarget(target.position, width);
         },0,1,duration));
    }
    #endregion
    
    protected void AttachSceneCamera(Transform attachTo,Transform lookAt=null)
    {
        TPSCameraController.Instance.Attach(attachTo, false,false);
        CameraController.Instance.LookAt(lookAt);
    }
    protected void AttachPlayerCamera(Transform playerTo)
    {
        TPSCameraController.Instance.Attach(playerTo, true,true);
        CameraController.Instance.LookAt(null);
        CameraController.Instance.SetCameraRotation(-1, playerTo.rotation.eulerAngles.y);
    }
}

public class GameIdentificationManager
{
    static int i_entityIndex = 0;
    static int i_damageInfoIndex = 0;

    public static void Init()
    {
        i_entityIndex = 0;
        i_damageInfoIndex = 0;
    }

    public static int I_EntityID(enum_EntityFlag flag)
    {
        i_entityIndex++;
        if (i_entityIndex == int.MaxValue)
            i_entityIndex = 0;
        return i_entityIndex + (int)flag * 100000;
    }

    public static int I_DamageIdentityID()
    {
        i_damageInfoIndex++;
        if (i_damageInfoIndex == int.MaxValue)
            i_damageInfoIndex = 0;
        return i_damageInfoIndex;
    }
}

public static class OptionsManager
{
    public static event Action event_OptionChanged;
    public static CGameOptions m_OptionsData => TGameData<CGameOptions>.Data;
    public static float m_Sensitive=1f;
    public static void Init()
    {
        TGameData<CGameOptions>.Init();
        OnOptionChanged();
    }

    public static void Save()
    {
        TGameData<CGameOptions>.Save();
        OnOptionChanged();
    }

    public static void OnOptionChanged()
    {
        m_Sensitive = GameExpression.F_PlayerSensitive(m_OptionsData.m_SensitiveTap);
        event_OptionChanged?.Invoke();
    }
    public static float F_SFXVolume =>GameExpression.F_GameVFXVolume(m_OptionsData.m_VFXVolumeTap);
    public static float F_MusicVolume => GameExpression.F_GameMusicVolume(m_OptionsData.m_MusicVolumeTap);
}
public static class GameDataManager
{
    public static Dictionary<enum_Rarity, List<enum_PlayerWeapon>> m_WeaponRarities { get; private set; } = new Dictionary<enum_Rarity, List<enum_PlayerWeapon>>();
    public static bool m_Inited { get; private set; } = false;
    public static void Init()
    {
        if (m_Inited) return;
        m_Inited = true;
        Properties<SWeapon>.Init();
        Properties<SBuff>.Init();
        SheetProperties<SEnermyGenerate>.Init();

        TGameData<CGameSave>.Init();
        TGameData<CFarmSave>.Init();
        TGameData<CBattleSave>.Init();

        Properties<SWeapon>.PropertiesList.Traversal((SWeapon weapon) =>
        {
            if (weapon.m_Hidden)
                return;

            if (!m_WeaponRarities.ContainsKey(weapon.m_Rarity))
                m_WeaponRarities.Add(weapon.m_Rarity, new List<enum_PlayerWeapon>());
            m_WeaponRarities[weapon.m_Rarity].Add( weapon.m_Weapon);
        });
    }
    #region GameSave
    public static CGameSave m_GameData => TGameData<CGameSave>.Data;
    public static CFarmSave m_CampFarmData => TGameData<CFarmSave>.Data;
    public static CBattleSave m_BattleData => TGameData<CBattleSave>.Data;
    public static void AdjustInGameData(EntityCharacterPlayer data, GameProgressManager level)
    {
        m_BattleData.Adjust(data, level);
        TGameData<CBattleSave>.Save();
    }
    public static void OnGameFinished(bool win)
    {
        TGameData<CBattleSave>.Reset();
        TGameData<CBattleSave>.Save();

        if (!win)
            return;
        m_CampFarmData.UnlockPlot(m_GameData.m_GameDifficulty);
        TGameData<CFarmSave>.Save();

        m_GameData.UnlockDifficulty();
        TGameData<CGameSave>.Save();
    }
    public static bool CanUseCredit(float credit) => m_GameData.f_Credits >= credit;
    public static void OnCreditStatus(float credit)
    {
        if (credit == 0)
            return;
        m_GameData.f_Credits += credit;
        TGameData<CGameSave>.Save();
    }

    public static bool CanUseTechPoint(float techPoint) => m_GameData.f_TechPoints >= techPoint;
    public static void OnTechPointStatus(float techPoint)
    {
        if (techPoint == 0)
            return;
        m_GameData.f_TechPoints += techPoint;
        TGameData<CGameSave>.Save();
    }

    public static int OnCampDifficultySwitch()
    {
        m_GameData.m_GameDifficulty += 1;
        if (m_GameData.m_GameDifficulty > m_GameData.m_DifficultyUnlocked)
            m_GameData.m_GameDifficulty = 1;

        TGameData<CGameSave>.Save();
        return m_GameData.m_GameDifficulty;
    }

    public static void RecreateCampFarmData()
    {
        TGameData<CFarmSave>.Reset();
        TGameData<CFarmSave>.Save();
    }
    public static void SaveCampFarmData(CampFarmManager farmManager)
    {
        m_CampFarmData.Save(farmManager);
        TGameData<CFarmSave>.Save();
    }
    
    public static void SaveActionStorageData()
    {
        TGameData<CGameSave>.Save();
    }

    #endregion
    #region ExcelData
    public static enum_PlayerWeapon TryGetWeaponEnum(string weaponIdentity)
    {
        int idTry = -1;
        if (int.TryParse(weaponIdentity, out idTry) && Enum.IsDefined(typeof(enum_PlayerWeapon), idTry))
            return (enum_PlayerWeapon)idTry;

        enum_PlayerWeapon targetWeapon = enum_PlayerWeapon.Invalid;
        if (Enum.TryParse(weaponIdentity, out targetWeapon))
            return targetWeapon;

        if (Properties<SWeapon>.PropertiesList.Any(p => TLocalization.GetKeyLocalized(p.m_Weapon.GetLocalizeNameKey()) == weaponIdentity))
            return Properties<SWeapon>.PropertiesList.Find(p => TLocalization.GetKeyLocalized(p.m_Weapon.GetLocalizeNameKey()) == weaponIdentity).m_Weapon;

        Debug.LogError("Invalid Player Weapon Found!");
        return  enum_PlayerWeapon.Invalid;
    }

    public static SWeapon GetWeaponProperties(enum_PlayerWeapon type)
    {
        SWeapon weapon = Properties<SWeapon>.PropertiesList.Find(p => p.m_Weapon == type);
        if (weapon.m_Weapon == 0)
            Debug.LogError("Error Properties Found Of Index:" + type.ToString() + "|" + ((int)type));
        else if (weapon.m_Hidden)
            Debug.LogWarning("You've Spawned A Hidden Weapon!");
        return weapon;
    }
    public static SBuff GetPresetBuff(int index)
    {
        SBuff buff = Properties<SBuff>.PropertiesList.Find(p => p.m_Index == index);
        if (buff.m_Index == 0)
            Debug.LogError("Error Properties Found Of Index:" + index);
        return buff;
    }
    public static Dictionary<bool,List<SEnermyGenerate>> GetEnermyGenerate(enum_Stage stage,enum_GameStyle style)
    {
        int sheetIndex = ((int)style - 1) * 3 + (int)stage - 1;
        Dictionary<bool, List<SEnermyGenerate>> m_GenerateDic = new Dictionary<bool, List<SEnermyGenerate>>() { { true, new List<SEnermyGenerate>() }, { false, new List<SEnermyGenerate>() } };
        SheetProperties<SEnermyGenerate>.GetPropertiesList(sheetIndex).Traversal((SEnermyGenerate generate)=> {   m_GenerateDic[generate.m_IsFinal].Add(generate); });
        return m_GenerateDic;
    }
    #endregion
}

public static class GameObjectManager
{
    static Transform TF_Entity;
    static Transform TF_SFXPlaying;
    static Transform TF_SFXWeapon;
    static Transform TF_Interacts;
    public static Transform TF_SFXWaitForRecycle { get; private set; }
    public static void Init()
    {
        TF_Entity = new GameObject("Entity").transform;
        TF_Interacts = new GameObject("Interacts").transform;
        TF_SFXWaitForRecycle = new GameObject("SFX_WaitForRecycle").transform;
        TF_SFXPlaying = new GameObject("SFX_CommonPlaying").transform;
        TF_SFXWeapon = new GameObject("SFX_Weapon").transform;
        ObjectPoolManager.Init();
    }
    public static void Clear()
    {
        ObjectPoolManager<int, SFXBase>.Destroy();
        ObjectPoolManager<int, SFXWeaponBase>.Destroy();
        ObjectPoolManager<int, EntityBase>.Destroy();
        ObjectPoolManager<enum_Interaction, InteractGameBase>.Destroy();
        ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Destroy();
    }
    #region Register
    public static void PresetRegistCommonObject()
    {
        TResources.GetAllEffectSFX().Traversal((int index, SFXBase
            target) => { ObjectPoolManager<int, SFXBase>.Register(index, target, 1); });
        TResources.GetCommonEntities().Traversal((int index, EntityBase entity) => { ObjectPoolManager<int, EntityBase>.Register(index, entity, 1); });
    }
    public static Dictionary<enum_EnermyType, int> RegistStyledInGamePrefabs(enum_GameStyle currentStyle, enum_Stage stageLevel)
    {
        RegisterInGameInteractions(currentStyle, stageLevel);

        Dictionary<enum_EnermyType, int> enermyDic = new Dictionary<enum_EnermyType, int>();
        TResources.GetEnermyEntities(currentStyle).Traversal((int index, EntityCharacterAI enermy) => {
            ObjectPoolManager<int, EntityBase>.Register(index, enermy, 1);
            if (enermy.E_SpawnType == enum_EnermyType.Invalid)
                return;
            if (enermyDic.ContainsKey(enermy.E_SpawnType))
                Debug.LogError("Same Enermy Type Found!" + enermy.name );
            enermyDic.Add(enermy.E_SpawnType, index);
        });
        return enermyDic;
    }

    static Dictionary<Type, enum_Interaction> m_GameInteractTypes = new Dictionary<Type, enum_Interaction>();
    static void RegisterInGameInteractions(enum_GameStyle portalStyle, enum_Stage stageIndex)
    {
        m_GameInteractTypes.Clear();
        TCommon.TraversalEnum((enum_Interaction enumValue) =>
        {
            if (enumValue <= enum_Interaction.GameBegin || enumValue >= enum_Interaction.GameEnd)
                return;
            InteractGameBase gameInteract = TResources.GetInteract(enumValue);
            m_GameInteractTypes.Add(gameInteract.GetType(),gameInteract.m_InteractType);
            ObjectPoolManager<enum_Interaction, InteractGameBase>.Register(enumValue,gameInteract , 5);
        });
    }
    #endregion
    #region Spawn/Recycle
    #region Entity
    //Start Health 0:Use Preset I_MaxHealth
    static T SpawnEntity<T>(int _poolIndex, Vector3 pos, Quaternion rot, Action<T> OnActivate) where T : EntityBase
    {
        T entity = ObjectPoolManager<int, EntityBase>.Spawn(_poolIndex, TF_Entity, NavigationManager.NavMeshPosition(pos), rot) as T;
        if (entity == null)
            Debug.LogError("Entity ID:" + _poolIndex + ",Type:" + typeof(T).ToString() + " Not Found");
        entity.gameObject.name = entity.m_EntityID.ToString() + "_" + _poolIndex.ToString();
        OnActivate(entity);
        return entity;
    }

    public static EntityCharacterAI SpawnEntityCharacterAI(int poolIndex, Vector3 toPosition, Quaternion toRot, enum_EntityFlag _flag, int gameDifficulty, enum_Stage _stage) => SpawnEntity(poolIndex, toPosition, toRot, (EntityCharacterAI ai) => ai.OnAIActivate(_flag, GameExpression.GetEnermyMaxHealthMultiplier(_stage, gameDifficulty), GameExpression.GetEnermyGameBuff(_stage, gameDifficulty)));

    public static EntityCharacterBase SpawnEntitySubCharacter(int poolIndex, Vector3 toPosition, Vector3 lookPos, enum_EntityFlag _flag, int spawnerID, float startHealth) => SpawnEntity(poolIndex, toPosition, Quaternion.LookRotation(TCommon.GetXZLookDirection(toPosition, lookPos), Vector3.up), (EntityCharacterBase character) => character.OnSubCharacterActivate(_flag, spawnerID, startHealth==0?character.I_MaxHealth:startHealth));

    public static EntityCharacterPlayer SpawnEntityPlayer(PlayerSaveData playerSave, Vector3 position, Quaternion rotation) => SpawnEntity((int)playerSave.m_Character, position, rotation, (EntityCharacterPlayer player) => player.OnPlayerActivate(playerSave));

    public static EntityNPC SpawnNPC(enum_InteractCharacter npc, Vector3 toPosition, Quaternion rot) => SpawnEntity((int)npc, toPosition, rot, (EntityNPC npcCharacter) => npcCharacter.OnActivate());

    public static void RecycleEntity(int index, EntityBase target) => ObjectPoolManager<int, EntityBase>.Recycle(index, target);
    #endregion
    #region ModeledWeapon
    public static WeaponBase SpawnWeapon(WeaponSaveData weaponData, Transform toTrans = null)
    {
        if (!ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Registed(weaponData.m_Weapon))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(weaponData.m_Weapon);
            ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Register(weaponData.m_Weapon, preset, 1);
        }
        WeaponBase targetWeapon = ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Spawn(weaponData.m_Weapon, toTrans ? toTrans : TF_Entity, Vector3.zero, Quaternion.identity);
        return targetWeapon;
    }
    public static void RecycleWeapon(WeaponBase weapon) => ObjectPoolManager<enum_PlayerWeapon, WeaponBase>.Recycle(weapon.m_WeaponInfo.m_Weapon, weapon);
    #endregion
    #region SFXWeapon
    public static T SpawnSFXWeapon<T>(int weaponIndex, Vector3 position, Vector3 normal) where T : SFXWeaponBase
    {
        if (!ObjectPoolManager<int, SFXWeaponBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXWeaponBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1);

        T template = ObjectPoolManager<int, SFXWeaponBase>.Spawn(weaponIndex, TF_SFXWeapon, position, Quaternion.LookRotation(normal)) as T;
        if (template == null)
            Debug.LogError("Enermy Weapon Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return template;
    }
    public static T GetSFXWeaponData<T>(int weaponIndex) where T : SFXWeaponBase
    {
        if (!ObjectPoolManager<int, SFXWeaponBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXWeaponBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1);

        T damageSourceInfo = ObjectPoolManager<int, SFXWeaponBase>.GetRegistedSpawnItem(weaponIndex) as T;
        if (damageSourceInfo == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return damageSourceInfo;
    }

    public static void TraversalAllSFXWeapon(Action<SFXWeaponBase> OnEachSFXWeapon) => ObjectPoolManager<int, SFXWeaponBase>.TraversalAllActive(OnEachSFXWeapon);
    public static void RecycleAllSFXWeapon(Predicate<SFXWeaponBase> predicate) => ObjectPoolManager<int, SFXWeaponBase>.RecycleAll(predicate);
    #endregion
    #region SFXEffects
    public static T SpawnSFX<T>(int index, Vector3 position, Vector3 normal) where T : SFXBase
    {
        T sfx = ObjectPoolManager<int, SFXBase>.Spawn(index, TF_SFXPlaying, position, Quaternion.LookRotation(normal)) as T;
        if (sfx == null)
            Debug.LogError("SFX Spawn Error! Invalid SFX Type:" + typeof(T) + ",Index:" + index);
        return sfx;
    }
    public static SFXParticles SpawnParticles(int particleID, Vector3 position, Vector3 direction) => SpawnSFX<SFXParticles>(particleID, position, direction);
    public static SFXTrail SpawnTrail(int trailID, Vector3 position, Vector3 direction) => SpawnSFX<SFXTrail>(trailID, position, direction);
    public static SFXIndicator SpawnIndicator(int _sourceID, Vector3 position, Vector3 normal) => SpawnSFX<SFXIndicator>(_sourceID, position, normal);
    public static SFXEffect SpawnBuffEffect(int _sourceID) => SpawnSFX<SFXEffect>(_sourceID, Vector3.zero, Vector3.up);

    public static void PlayMuzzle(int _sourceID, Vector3 position, Vector3 direction, int muzzleIndex, AudioClip muzzleClip = null)
    {
        if (muzzleIndex > 0)
            SpawnSFX<SFXMuzzle>(muzzleIndex, position, direction).PlayUncontrolled(_sourceID);
        if (muzzleClip)
            AudioManager.Instance.Play3DClip(_sourceID, muzzleClip, false, position);
    }
    #endregion
    #region Interact
    public static T SpawnInteract<T>( Vector3 pos, Quaternion rot) where T : InteractGameBase=> ObjectPoolManager<enum_Interaction, InteractGameBase>.Spawn(m_GameInteractTypes[typeof(T)], TF_Interacts, pos, rot) as T;
    public static void RecycleInteract(InteractGameBase target) => ObjectPoolManager<enum_Interaction, InteractGameBase>.Recycle(target.m_InteractType, target);
    public static void RecycleAllInteract() => ObjectPoolManager<enum_Interaction, InteractGameBase>.RecycleAll();
    #endregion
    #endregion
}

