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
        UIT_MobileConsole.Instance.InitConsole((bool show) => { TimeScaleController<enum_GameTimeScaleType>.SetScale(enum_GameTimeScaleType.GameBase, show ? .1f : 1f); });
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
        GameObjectManager.Init();
        m_DirectionalLight = transform.Find("Directional Light").GetComponent<Light>();
    }

    protected virtual void Start()
    {
        AudioManagerBase.Instance.Init();
        GameObjectManager.RegisterGameObjects();
        UIManager.Activate(B_InGame);
        TimeScaleController<enum_GameTimeScaleType>.Clear();

        AddConsoleBinding();
        OnCommonOptionChanged();
        OptionsDataManager.event_OptionChanged += OnCommonOptionChanged;
        OptionsDataManager.event_OptionChanged += OnEffectOptionChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.StopAllSingleCoroutines();
        OptionsDataManager.event_OptionChanged -= OnCommonOptionChanged;
        OptionsDataManager.event_OptionChanged -= OnEffectOptionChanged;
    }

    protected void SwitchScene(enum_Scene scene,Func<bool> onfinishLoading=null)
    {
        AudioManagerBase.Instance.Destroy();
        GameObjectManager.Destory();
        LevelObjectManager.Destroy();
        UIManager.Instance.SetActivate(false);
        LoadingManager.BeginLoad(scene,onfinishLoading);
    }

    void OnCommonOptionChanged()
    {
        TLocalization.SetRegion(OptionsDataManager.m_OptionsData.m_Region);
        Application.targetFrameRate = (int)OptionsDataManager.m_OptionsData.m_FrameRate;
    }

    protected virtual void Update()
    {
        TimeScaleTick();
    }

    protected void OnPortalEnter(float duration,Transform vortexTarget, Action OnEnter)=>SetPostEffect_Vortex(true, vortexTarget, 1f,OnEnter);

    protected void OnPortalExit(float duration,Transform vortexTarget)=> SetPostEffect_Vortex(false, vortexTarget, 1f);
    #region Game Effect

    PE_BloomSpecific m_Bloom;
    PE_DepthSSAO m_DepthSSAO;
    protected void InitGameEffects(enum_GameStyle _levelStyle, GameRenderData renderData)
    {
        renderData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);
        CameraController.Instance.m_Effect.RemoveAllPostEffect();
        //CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthOutline>().SetEffect(Color.black,1.2f,0.0001f);
        m_DepthSSAO = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthSSAO>().SetEffect(renderData.c_shadowColor, 2f,20,0.00035f,null,16);
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
        else
            m_Bloom.m_Blur.SetEffect( PE_Blurs.enum_BlurType.Invalid);
        m_DepthSSAO.SetAOEnable(OptionsDataManager.m_OptionsData.m_Effect>=  enum_Option_Effect.High);
        m_Bloom.SetBloomEnable(OptionsDataManager.m_OptionsData.m_Effect >= enum_Option_Effect.Normal, OptionsDataManager.m_OptionsData.m_Effect >= enum_Option_Effect.High);
    }

    protected void SetPostEffect_DepthScan(Vector3 position,Color color)
    {
        CameraController.Instance.m_Effect.StartDepthScanCircle(position,color, 1f, 40, 2f);
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
        vortex.SetTexture(TResources.m_NoiseTex);
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

    #region TimeScale
    TimerBase m_DurationTimeScale = new TimerBase();
    public void SetBaseTimeScale(float timeScale = 1f) => TimeScaleController< enum_GameTimeScaleType>.SetScale( enum_GameTimeScaleType.GameBase, timeScale);
    public void SetExtraTimeScale(float timeScale = 1f) => TimeScaleController<enum_GameTimeScaleType>.SetScale( enum_GameTimeScaleType.GameExtra, timeScale);
    public void SetDurationTimeScale(float duration)
    {
        m_DurationTimeScale.SetTimerDuration(duration);
        TimeScaleController<enum_GameTimeScaleType>.SetScale(enum_GameTimeScaleType.GameImpact, .12f);
    }
    void TimeScaleTick()
    {
        float unscaledDeltaTime = Time.unscaledDeltaTime;
        if (m_DurationTimeScale.m_Timing)
        {
            m_DurationTimeScale.Tick(unscaledDeltaTime);
            if (!m_DurationTimeScale.m_Timing)
                TimeScaleController<enum_GameTimeScaleType>.SetScale(enum_GameTimeScaleType.GameImpact, 1f);
        }
        TimeScaleController<enum_GameTimeScaleType>.Tick();
    }
    #endregion
    protected void AttachSceneCamera(Transform attachTo,Transform lookAt=null)
    {
        CameraController.Instance.Attach(attachTo, true) .SetLookAt(lookAt);
    }
    protected void AttachPlayerCamera(Transform playerTo)
    {
        CameraController.Instance.Attach(playerTo, false).SetLookAt(null) .SetCameraRotation(-1, playerTo.rotation.eulerAngles.y).ForceSetCamera();
    }
}

public class GameIdentificationManager
{
    static int i_entityIndex = 0;
    static int i_playerWeaponIndex = 0;

    public static void Init()
    {
        i_entityIndex = 0;
        i_playerWeaponIndex = 0;
    }

    public static int GetEntityID(enum_EntityFlag flag)
    {
        i_entityIndex++;
        if (i_entityIndex == int.MaxValue)
            i_entityIndex = 0;
        return i_entityIndex + (int)flag * 100000;
    }

    public static int GetWeaponID() => i_playerWeaponIndex++;
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
    #region Register
    static Dictionary<Type, enum_Interaction> m_GameInteractTypes = new Dictionary<Type, enum_Interaction>();
    public static void RegisterGameObjects()
    {
        TResources.GetAllEffectSFX().Traversal((SFXBase sfx) => { ObjectPoolManager<int, SFXBase>.Register(int.Parse(sfx.name.Split('_')[0]), sfx, 1); });

        m_GameInteractTypes.Clear();
        TCommon.TraversalEnum((enum_Interaction enumValue) =>
        {
            if (enumValue <= enum_Interaction.GameBegin || enumValue >= enum_Interaction.GameEnd)
                return;
            InteractGameBase gameInteract = TResources.GetInteract(enumValue);
            m_GameInteractTypes.Add(gameInteract.GetType(), gameInteract.m_InteractType);
            ObjectPoolManager<enum_Interaction, InteractGameBase>.Register(enumValue, gameInteract, 1);
        });
    }
    static void RegisterPlayerCharacter(enum_PlayerCharacter character)
    {
        int characterIndex = (int)character;
        if (ObjectPoolManager<int, EntityBase>.Registed(characterIndex))
            return;
        ObjectPoolManager<int, EntityBase>.Register(characterIndex,TResources.GetPlayerCharacter(character),1);
    }
    static void RegisterAICharacter(int characterIndex)
    {
        if (ObjectPoolManager<int, EntityBase>.Registed(characterIndex))
            return;

        ObjectPoolManager<int, EntityBase>.Register(characterIndex, TResources.GetGameCharacter(characterIndex), 1);
    }

    public static void Recycle()
    {
        ObjectPoolManager<int, SFXBase>.RecycleAll();
        ObjectPoolManager<int, SFXDamageBase>.RecycleAll();
        ObjectPoolManager<int, EntityBase>.RecycleAll();
        ObjectPoolManager<enum_Interaction, InteractGameBase>.RecycleAll();
        ObjectPoolManager<enum_PlayerWeaponIdentity, WeaponBase>.RecycleAll();
    }

    public static void Destory()
    {
        ObjectPoolManager<int, SFXBase>.Destroy();
        ObjectPoolManager<int, SFXDamageBase>.Destroy();
        ObjectPoolManager<int, EntityBase>.Destroy();
        ObjectPoolManager<enum_Interaction, InteractGameBase>.Destroy();
        ObjectPoolManager<enum_PlayerWeaponIdentity, WeaponBase>.Destroy();
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

    public static EntityCharacterGameBase SpawnGameCharcter(int poolIndex, Vector3 toPosition, Quaternion toRot)
    {
        RegisterAICharacter(poolIndex);
        return SpawnEntity<EntityCharacterGameBase>(poolIndex, toPosition, toRot); 
    }

    public static EntityCharacterPlayer SpawnPlayerCharacter(enum_PlayerCharacter character, Vector3 position, Quaternion rotation)
    {
        RegisterPlayerCharacter(character);
        return SpawnEntity<EntityCharacterPlayer>((int)character, position, rotation); 
    }

    public static EntityNPC SpawnNPC(enum_InteractCharacter npc, Vector3 toPosition, Quaternion rot) => SpawnEntity<EntityNPC>((int)npc, toPosition, rot).OnActivate();

    public static void RecycleEntity(int index, EntityBase target) => ObjectPoolManager<int, EntityBase>.Recycle(index, target);
    #endregion
    #region Model Weapon
    static void CheckWeaponExists(enum_PlayerWeaponIdentity identity)
    {
        if (!ObjectPoolManager<enum_PlayerWeaponIdentity, WeaponBase>.Registed(identity))
        {
            WeaponBase preset = TResources.GetPlayerWeapon(identity);
            ObjectPoolManager<enum_PlayerWeaponIdentity, WeaponBase>.Register(identity, preset, 1);
        }
    }
    public static WeaponBase SpawnWeapon(WeaponSaveData weaponData, Transform toTrans = null)
    {
        CheckWeaponExists(weaponData.m_Weapon);
        return ObjectPoolManager<enum_PlayerWeaponIdentity, WeaponBase>.Spawn(weaponData.m_Weapon, toTrans ? toTrans : TF_Entity, Vector3.zero, Quaternion.identity).InitWeapon(weaponData);
    }
    public static WeaponBase GetWeaponData(enum_PlayerWeaponIdentity weapon)
    {
        CheckWeaponExists(weapon);
        return ObjectPoolManager<enum_PlayerWeaponIdentity, WeaponBase>.GetRegistedSpawnItem(weapon);
    }
    public static void RecycleWeapon(WeaponBase weapon) => ObjectPoolManager<enum_PlayerWeaponIdentity, WeaponBase>.Recycle(weapon.m_WeaponInfo.m_Weapon, weapon);
    #endregion
    #region SFXWeapon
    public static T SpawnSFXWeapon<T>(int weaponIndex, Vector3 position, Vector3 normal) where T : SFXDamageBase
    {
        if (!ObjectPoolManager<int, SFXDamageBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXDamageBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1);

        T template = ObjectPoolManager<int, SFXDamageBase>.Spawn(weaponIndex, TF_SFXWeapon, position, Quaternion.LookRotation(normal)) as T;
        if (template == null)
            Debug.LogError("Enermy Weapon Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return template;
    }
    public static T GetSFXWeaponData<T>(int weaponIndex) where T : SFXDamageBase
    {
        if (!ObjectPoolManager<int, SFXDamageBase>.Registed(weaponIndex))
            ObjectPoolManager<int, SFXDamageBase>.Register(weaponIndex, TResources.GetDamageSource(weaponIndex), 1);

        T damageSourceInfo = ObjectPoolManager<int, SFXDamageBase>.GetRegistedSpawnItem(weaponIndex) as T;
        if (damageSourceInfo == null)
            Debug.LogError("SFX Get Error! Invalid Type:" + typeof(T).ToString() + "|Index:" + weaponIndex);
        return damageSourceInfo;
    }

    public static void TraversalAllSFXWeapon(Action<SFXDamageBase> OnEachSFXWeapon) => ObjectPoolManager<int, SFXDamageBase>.TraversalAllActive(OnEachSFXWeapon);
    public static void RecycleAllSFXWeapon(Predicate<SFXDamageBase> predicate) => ObjectPoolManager<int, SFXDamageBase>.RecycleAll(predicate);
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
    public static T SpawnInteract<T>(Vector3 pos, Quaternion rot,Transform trans=null) where T : InteractGameBase
    {
        Type targetType = typeof(T);
        if (!m_GameInteractTypes.ContainsKey(targetType))
            Debug.LogError("None Type Registed!" + targetType);
        return ObjectPoolManager<enum_Interaction, InteractGameBase>.Spawn(m_GameInteractTypes[targetType], trans==null? TF_Interacts:trans, pos, rot) as T;
    } 
    public static void RecycleInteract(InteractGameBase target) => ObjectPoolManager<enum_Interaction, InteractGameBase>.Recycle(target.m_InteractType, target);
    public static void TraversalAllInteracts(Action<InteractGameBase> action) => ObjectPoolManager<enum_Interaction, InteractGameBase>.TraversalAllActive(action);
    #endregion
    #endregion
}

