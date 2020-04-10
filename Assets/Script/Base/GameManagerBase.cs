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
        OptionsManager.Init();
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
    PE_DepthSSAO m_DepthSSAO;
    protected void InitGameEffects(enum_GameStyle _levelStyle, GameRenderData renderData)
    {
        renderData.DataInit(m_DirectionalLight, CameraController.Instance.m_Camera);
        CameraController.Instance.m_Effect.RemoveAllPostEffect();
        //CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthOutline>().SetEffect(Color.black,1.2f,0.0001f);
        m_DepthSSAO = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthSSAO>().SetEffect(renderData.c_shadowColor, 2f,15,0.00035f,null,16);
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
        m_DirectionalLight.shadows = OptionsManager.m_OptionsData.m_ShadowOff ? LightShadows.None : LightShadows.Hard;
        if (OptionsManager.m_OptionsData.m_Bloom == enum_Option_Bloom.Normal)
            m_Bloom.m_Blur.SetEffect(PE_Blurs.enum_BlurType.GaussianBlur, 4, 5, 2);
        else if (OptionsManager.m_OptionsData.m_Bloom == enum_Option_Bloom.High)
            m_Bloom.m_Blur.SetEffect(PE_Blurs.enum_BlurType.GaussianBlur, 2, 10, 2);
        m_DepthSSAO.SetAOEnable(OptionsManager.m_OptionsData.m_Effect>= enum_Option_Effect.High);
        m_Bloom.SetBloomEnable(OptionsManager.m_OptionsData.m_Bloom >= enum_Option_Bloom.Normal, OptionsManager.m_OptionsData.m_Bloom >= enum_Option_Bloom.High);
        CameraController.Instance.m_Effect.SetCameraEffects(OptionsManager.m_OptionsData.m_Effect >= enum_Option_Effect.Medium? DepthTextureMode.Depth: DepthTextureMode.None );
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

public static class OptionsManager
{
    public static event Action event_OptionChanged;
    public static CGameOptions m_OptionsData => TGameData<CGameOptions>.Data;
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

    public static void OnOptionChanged()=>event_OptionChanged?.Invoke();
    public static float F_SFXVolume =>GameExpression.F_GameVFXVolume(m_OptionsData.m_VFXVolumeTap);
    public static float F_MusicVolume => GameExpression.F_GameMusicVolume(m_OptionsData.m_MusicVolumeTap);
}
public static class GameWeaponDataManager
{

}
public static class GameDataManager
{
    public static CGameSave m_GameData => TGameData<CGameSave>.Data;
    public static CArmoryData m_ArmoryData=> TGameData<CArmoryData>.Data;
    public static CEquipmentDepotData m_EquipmentDepotData => TGameData<CEquipmentDepotData>.Data;
    public static CPlayerBattleSave m_BattleData => TGameData<CPlayerBattleSave>.Data;

    public static bool m_Inited { get; private set; } = false;
    public static void Init()
    {
        if (m_Inited) return;
        m_Inited = true;
        Properties<SWeapon>.Init();
        Properties<SBuff>.Init();
        SheetProperties<SEnermyGenerate>.Init();

        TGameData<CGameSave>.Init();
        TGameData<CEquipmentDepotData>.Init();
        TGameData<CArmoryData>.Init();
        TGameData<CPlayerBattleSave>.Init();

        InitPerks();
        InitArmory();
        InitEquipment();
    }
    #region GameSave
    public static void OnNewGame()
    {
        TGameData<CPlayerBattleSave>.Reset();
        TGameData<CPlayerBattleSave>.Save();
    }

    public static void StageFinishSaveData(EntityCharacterPlayer data, GameProgressManager level)
    {
        m_BattleData.Adjust(data, level);
        TGameData<CPlayerBattleSave>.Save();
    }

    public static void OnGameResult(GameProgressManager progress)
    {
        if (progress.m_GameWin)
            m_GameData.UnlockDifficulty();
        OnCreditStatus(progress.F_CreditGain);

        TGameData<CPlayerBattleSave>.Reset();
        TGameData<CPlayerBattleSave>.Save();
    }
    #endregion
    #region GameData
    public static bool CanUseCredit(float credit) => m_GameData.f_Credits >= credit;
    public static void OnCreditStatus(float credit)
    {
        if (credit == 0)
            return;
        m_GameData.f_Credits += credit;
        TGameData<CGameSave>.Save();
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_GameCurrencyStatus);
    }

    public static int OnCampDifficultySwitch()
    {
        m_GameData.m_GameDifficulty += 1;
        if (m_GameData.m_GameDifficulty > m_GameData.m_DifficultyUnlocked)
            m_GameData.m_GameDifficulty = 1;

        TGameData<CGameSave>.Save();
        return m_GameData.m_GameDifficulty;
    }

    public static bool OnDailyRewardRequire()
    {
        if (!m_GameData.CheckDailyReward())
            return false;
        OnCreditStatus(500f);
        return true;
    }
    #endregion
    #region ArmoryData
    public static Dictionary<enum_PlayerWeapon, SWeapon> m_AvailableWeapons { get; private set; } = new Dictionary<enum_PlayerWeapon, SWeapon>();
    public static Dictionary<enum_Rarity, List<enum_PlayerWeapon>> m_GameWeaponUnlocked { get; private set; } = new Dictionary<enum_Rarity, List<enum_PlayerWeapon>>();
    public static float GetArmoryUnlockPrice(enum_PlayerWeapon weapon)=> GameConst.m_ArmoryBlueprintUnlockPrice[m_AvailableWeapons[weapon].m_Rarity];
    public static bool CanArmoryUnlock(enum_PlayerWeapon weapon) => m_GameData.f_Credits >= GetArmoryUnlockPrice(weapon);
    public static void OnArmoryUnlock(enum_PlayerWeapon weapon)
    {
        if (!m_ArmoryData.m_WeaponBlueprints.Contains(weapon))
        {
            Debug.LogError("Error! Unlock A None Blueprint Weapon" + weapon);
            return;
        }
        if(m_ArmoryData.m_WeaponsUnlocked.Contains(weapon))
        {
            Debug.LogError("Error! Unlock A Unlocked Weapon" + weapon);
            return;
        }
        if (!CanArmoryUnlock(weapon))
        {
            Debug.LogError("Invalid Unlock Behaviour!");
            return;
        }
        OnCreditStatus(-GetArmoryUnlockPrice(weapon));
        m_ArmoryData.m_WeaponBlueprints.Remove(weapon);
        m_ArmoryData.m_WeaponsUnlocked.Add(weapon);
        m_ArmoryData.m_WeaponSelected = weapon;
        TGameData<CArmoryData>.Save();
        InitArmory();
    }
    
    public static void OnArmorySelect(enum_PlayerWeapon weapon)
    {
        if(!m_ArmoryData.m_WeaponsUnlocked.Contains(weapon))
        {
            Debug.LogError("Error! Equipping A Locked Weapon!");
            return;
        }
        if(weapon== m_ArmoryData.m_WeaponSelected)
        {
            Debug.LogError("Error! Equipping A Selcted Weapon!");
            return;
        }
        m_ArmoryData.m_WeaponSelected = weapon;
        TGameData<CArmoryData>.Save();
    }

    static void InitArmory()
    {
        m_AvailableWeapons.Clear();
        m_GameWeaponUnlocked.Clear();

        Properties<SWeapon>.PropertiesList.Traversal((SWeapon weapon) =>
        {
            if (weapon.m_Hidden)
                return;
            m_AvailableWeapons.Add(weapon.m_Weapon,weapon);

            if (!m_ArmoryData.m_WeaponsUnlocked.Contains(weapon.m_Weapon))
                return;
            if (!m_GameWeaponUnlocked.ContainsKey(weapon.m_Rarity))
                m_GameWeaponUnlocked.Add(weapon.m_Rarity, new List<enum_PlayerWeapon>());
            m_GameWeaponUnlocked[weapon.m_Rarity].Add(weapon.m_Weapon);
        });
    }


    public static enum_PlayerWeapon UnlockArmoryBlueprint(enum_Rarity _spawnRarity)
    {
        Dictionary<enum_Rarity, List<enum_PlayerWeapon>> _blueprintAvailable = new Dictionary<enum_Rarity, List<enum_PlayerWeapon>>();
        m_AvailableWeapons.Traversal((enum_PlayerWeapon weapon,SWeapon weaponData) =>
        {
            if (m_ArmoryData.m_WeaponBlueprints.Contains(weapon) || m_ArmoryData.m_WeaponsUnlocked.Contains(weapon) )
                return;
            if (!_blueprintAvailable.ContainsKey(weaponData.m_Rarity))
                _blueprintAvailable.Add(weaponData.m_Rarity, new List<enum_PlayerWeapon>());
            _blueprintAvailable[weaponData.m_Rarity].Add(weapon);
        });

        enum_PlayerWeapon bluePrint = enum_PlayerWeapon.Invalid;
        if (_blueprintAvailable.ContainsKey(_spawnRarity))
            bluePrint = _blueprintAvailable[_spawnRarity].RandomItem();
        else if (_blueprintAvailable.ContainsKey(enum_Rarity.Ordinary))
            bluePrint = _blueprintAvailable[enum_Rarity.Ordinary].RandomItem();

        if(bluePrint!= enum_PlayerWeapon.Invalid)
        {
            m_ArmoryData.m_WeaponBlueprints.Add(bluePrint);
            TGameData<CArmoryData>.Save();
            InitArmory();
        }

        return bluePrint;
    }
    #endregion
    #region EquipmentData
    static Dictionary<int, ExpireEquipmentBase> m_AllEquipments=new Dictionary<int, ExpireEquipmentBase>();

    public static void InitEquipment()
    {
        TReflection.TraversalAllInheritedClasses(((Type type, ExpireEquipmentBase perk) => {
            m_AllEquipments.Add(perk.m_Index, perk);
        }),enum_EquipmentPassitveType.Invalid,new EquipmentSaveData(-1,-1, enum_Rarity.Invalid,new List<EquipmentEntrySaveData>()));
    }

    public static ExpireEquipmentBase CreateEquipment(enum_EquipmentPassitveType passiveType, EquipmentSaveData data)
    {
        if (!m_AllEquipments.ContainsKey(data.m_Index))
            Debug.LogError("Error Equipment:" + data.m_Index + " ,Does not exist");
        ExpireEquipmentBase equipment = TReflection.CreateInstance<ExpireEquipmentBase>(m_AllEquipments[data.m_Index].GetType(),passiveType, data);
        return equipment;
    }
    #endregion
    #region PerkData
    static Dictionary<int, ExpirePerkBase> m_AllPerks = new Dictionary<int, ExpirePerkBase>();
    static Dictionary<enum_Rarity, List<int>> m_PerkRarities = new Dictionary<enum_Rarity, List<int>>();
    public static void InitPerks()
    {
        m_AllPerks.Clear();
        m_PerkRarities.Clear();
        TReflection.TraversalAllInheritedClasses(((Type type, ExpirePerkBase perk) => {
            m_AllPerks.Add(perk.m_Index, perk);
            if (perk.m_DataHidden)
                return;
            if (!m_PerkRarities.ContainsKey(perk.m_Rarity))
                m_PerkRarities.Add(perk.m_Rarity, new List<int>());
            m_PerkRarities[perk.m_Rarity].Add(perk.m_Index);
        }), PerkSaveData.New(-1));
    }
    public static int RandomPerk(enum_Rarity rarity, Dictionary<int, ExpirePerkBase> playerPerks, System.Random random = null)
    {
        List<int> rarityIDs = m_PerkRarities[rarity].DeepCopy();
        playerPerks.Traversal((ExpirePerkBase perk) => { if (perk.m_Rarity == rarity && perk.m_Stack == perk.m_MaxStack) rarityIDs.Remove(perk.m_Index); });
        return rarityIDs.RandomItem(random);
    }

    public static List<int> RandomPerks(int perkCount, Dictionary<enum_Rarity, int> perkGenerate, Dictionary<int, ExpirePerkBase> playerPerks, System.Random random = null)
    {
        Dictionary<enum_Rarity, List<int>> _perkIDs = m_PerkRarities.DeepCopy();
        Dictionary<enum_Rarity, int> _rarities = perkGenerate.DeepCopy();

        playerPerks.Traversal((ExpirePerkBase perk) => { if (perk.m_Stack == perk.m_MaxStack) _perkIDs[perk.m_Rarity].Remove(perk.m_Index); });

        List<int> randomIDs = new List<int>();
        for (int i = 0; i < perkCount; i++)
        {
            enum_Rarity rarity = TCommon.RandomPercentage(_rarities, random);
            if (_perkIDs[rarity].Count == 0)
                rarity = enum_Rarity.Ordinary;

            int perkID = _perkIDs[rarity].RandomItem(random);
            _perkIDs[rarity].Remove(perkID);
            randomIDs.Add(perkID);
        }
        return randomIDs;
    }

    public static ExpirePerkBase GetPerkData(int index) => m_AllPerks[index];

    public static ExpirePerkBase CreatePerk(PerkSaveData data)
    {
        if (!m_AllPerks.ContainsKey(data.m_Index))
            Debug.LogError("Error Perk:" + data.m_Index + " ,Does not exist");
        ExpirePerkBase equipment = TReflection.CreateInstance<ExpirePerkBase>(m_AllPerks[data.m_Index].GetType(), data);
        return equipment;
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
        return enum_PlayerWeapon.Invalid;
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
    public static Dictionary<bool, List<SEnermyGenerate>> GetEnermyGenerate(enum_Stage stage, enum_GameStyle style)
    {
        int sheetIndex = ((int)style - 1) * 3 + (int)stage - 1;
        Dictionary<bool, List<SEnermyGenerate>> m_GenerateDic = new Dictionary<bool, List<SEnermyGenerate>>() { { true, new List<SEnermyGenerate>() }, { false, new List<SEnermyGenerate>() } };
        SheetProperties<SEnermyGenerate>.GetPropertiesList(sheetIndex).Traversal((SEnermyGenerate generate) => { m_GenerateDic[generate.m_IsFinal].Add(generate); });
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

    public static EntityCharacterPlayer SpawnEntityPlayer( CPlayerBattleSave battleSave, Vector3 position, Quaternion rotation) => SpawnEntity((int)battleSave.m_Character, position, rotation, (EntityCharacterPlayer player) => player.OnPlayerActivate(battleSave));

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

