using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TExcel;
using TGameSave;
using System.Linq;

public class GameManagerBase : SingletonMono<GameManagerBase>,ICoroutineHelperClass{
    #region Test
    protected virtual void AddConsoleBinding()
    {
        UIT_MobileConsole.Instance.InitConsole((bool show)=> { Time.timeScale = show ? .1f : 1f; });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Clear Console", KeyCode.None, UIT_MobileConsole.Instance.ClearConsoleLog);
    }
    #endregion
    public virtual bool B_InGame => false;
    public Light m_DirectionalLight { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        LoadingManager.OnOtherSceneEnter(B_InGame? enum_Scene.Game: enum_Scene.Camp);
        TBroadCaster<enum_BC_GameStatus>.Init();
        TBroadCaster<enum_BC_UIStatus>.Init();
        GameIdentificationManager.Init();
        GameDataManager.Init();
        OptionsDataManager.Init();
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
    }

    protected virtual void Start()
    {
        GameObjectManager.Init();
        AudioManagerBase.Instance.Init();
        GameObjectManager.PresetRegistCommonObject();
        UIManager.Activate(B_InGame);

        AddConsoleBinding();
        OnCommonOptionChanged();
        OptionsDataManager.event_OptionChanged += OnCommonOptionChanged;
        OptionsDataManager.event_OptionChanged += OnEffectOptionChanged;
        SetBulletTime(false);
    }

    protected virtual void OnDisable()
    {
        this.StopAllSingleCoroutines();
        OptionsDataManager.event_OptionChanged -= OnCommonOptionChanged;
        OptionsDataManager.event_OptionChanged -= OnEffectOptionChanged;
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
        TLocalization.SetRegion(OptionsDataManager.m_OptionsData.m_Region);
        Application.targetFrameRate = (int)OptionsDataManager.m_OptionsData.m_FrameRate;
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
    PE_DepthSSAO m_DepthSSAO;
    protected void InitGameEffects(enum_GameStyle _levelStyle, GameRenderData renderData)
    {
        renderData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);
        CameraController.Instance.m_Effect.RemoveAllPostEffect();
        //CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthOutline>().SetEffect(Color.black,1.2f,0.0001f);
        m_DepthSSAO = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthSSAO>().SetEffect(renderData.c_shadowColor, 2f,15,0.00035f,null,16);
        m_Bloom = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BloomSpecific>();
        CameraController.Instance.m_Effect.SetMainTextureCamera(true);
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
        m_DirectionalLight.shadows = OptionsDataManager.m_OptionsData.m_ShadowOff ? LightShadows.None : LightShadows.Hard;
        if (OptionsDataManager.m_OptionsData.m_Effect == enum_Option_Effect.Normal)
            m_Bloom.m_Blur.SetEffect(PE_Blurs.enum_BlurType.GaussianBlur, 4, 5, 2);
        else if (OptionsDataManager.m_OptionsData.m_Effect == enum_Option_Effect.High)
            m_Bloom.m_Blur.SetEffect(PE_Blurs.enum_BlurType.GaussianBlur, 2, 10, 2);
        m_DepthSSAO.SetAOEnable(OptionsDataManager.m_OptionsData.m_Effect>=  enum_Option_Effect.High);
        m_Bloom.SetBloomEnable(OptionsDataManager.m_OptionsData.m_Effect >= enum_Option_Effect.Normal, OptionsDataManager.m_OptionsData.m_Effect >= enum_Option_Effect.High);
        CameraController.Instance.m_Effect.ResetCameraEffectParams();
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

    public static void Init()
    {
        i_entityIndex = 0;
    }

    public static int I_EntityID(enum_EntityFlag flag)
    {
        i_entityIndex++;
        if (i_entityIndex == int.MaxValue)
            i_entityIndex = 0;
        return i_entityIndex + (int)flag * 100000;
    }
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
        TResources.GetAllEffectSFX().Traversal((int index, SFXBase target) => { ObjectPoolManager<int, SFXBase>.Register(index, target, 1); });
    }
    static void RegisterPlayerCharacter(enum_PlayerCharacter character)
    {
        int characterIndex = (int)character;
        if (ObjectPoolManager<int, EntityBase>.Registed(characterIndex))
            return;
        ObjectPoolManager<int, EntityBase>.Register(characterIndex,TResources.GetPlayerCharacter(character),1);
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
    static T SpawnEntity<T>(int _poolIndex, Vector3 pos, Quaternion rot) where T : EntityBase
    {
        T entity = ObjectPoolManager<int, EntityBase>.Spawn(_poolIndex, TF_Entity, NavigationManager.NavMeshPosition(pos), rot) as T;
        if (entity == null)
            Debug.LogError("Entity ID:" + _poolIndex + ",Type:" + typeof(T).ToString() + " Not Found");
        entity.gameObject.name = entity.m_EntityID.ToString() + "_" + _poolIndex.ToString();
        return entity;
    }

    public static EntityCharacterAI SpawnEntityCharacterAI(int poolIndex, Vector3 toPosition, Quaternion toRot, enum_EntityFlag _flag, int gameDifficulty, enum_Stage _stage) => SpawnEntity<EntityCharacterAI>(poolIndex, toPosition, toRot).OnAIActivate(_flag, GameExpression.GetEnermyMaxHealthMultiplier(_stage, gameDifficulty), GameExpression.GetEnermyGameBuff(_stage, gameDifficulty));

    public static EntityCharacterBase SpawnEntitySubCharacter(int poolIndex, Vector3 toPosition, Vector3 lookPos, enum_EntityFlag _flag, int spawnerID, float startHealth) => SpawnEntity<EntityCharacterAI>(poolIndex, toPosition, Quaternion.LookRotation(TCommon.GetXZLookDirection(toPosition, lookPos), Vector3.up)).OnSubAIActivate(_flag, spawnerID, startHealth);

    public static EntityCharacterPlayer SpawnPlayerCharacter(enum_PlayerCharacter character, Vector3 position, Quaternion rotation)
    {
        RegisterPlayerCharacter(character);
        return SpawnEntity<EntityCharacterPlayer>((int)character, position, rotation); 
    }


    public static EntityNPC SpawnNPC(enum_InteractCharacter npc, Vector3 toPosition, Quaternion rot) => SpawnEntity<EntityNPC>((int)npc, toPosition, rot).OnActivate();

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
    public static void TraversalAllInteracts(Action<InteractGameBase> action) => ObjectPoolManager<enum_Interaction, InteractGameBase>.TraversalAllActive(action);
    #endregion
    #endregion
}

