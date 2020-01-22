using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TExcel;
using TGameSave;
public class GameManagerBase : SimpleSingletonMono<GameManagerBase>,ISingleCoroutine{
    public virtual bool B_InGame => false;
    protected override void Awake()
    {
        base.Awake();
        LoadingManager.OnOtherSceneEnter(B_InGame? enum_Scene.Game: enum_Scene.Camp);
        TBroadCaster<enum_BC_GameStatus>.Init();
        TBroadCaster<enum_BC_UIStatus>.Init();
        GameIdentificationManager.Init();
        GameDataManager.Init();
        ActionDataManager.Init();
        OptionsManager.Init();
    }

    protected virtual void Start()
    {
        GameObjectManager.Init();
        AudioManagerBase.Instance.Init();
        GameObjectManager.PresetRegistCommonObject();
        UIManager.Activate(B_InGame);
        OnOptionChanged();
        OptionsManager.event_OptionChanged += OnOptionChanged;
        SetBulletTime(false);
    }

    protected virtual void OnDisable()
    {
        this.StopAllSingleCoroutines();
        OptionsManager.event_OptionChanged -= OnOptionChanged;
    }

    protected void SwitchScene(enum_Scene scene,Func<bool> onfinishLoading=null)
    {
        AudioManagerBase.Instance.Recycle();
        GameObjectManager.Clear();
        LevelObjectManager.Clear();
        UIManager.Instance.SetActivate(false);
        LoadingManager.BeginLoad(scene,onfinishLoading);
    }

    void OnOptionChanged()
    {
        TLocalization.SetRegion(OptionsManager.m_OptionsData.m_Region);
        Application.targetFrameRate = (int)OptionsManager.m_OptionsData.m_FrameRate;
        CameraController.Instance.m_Effect.SetCostyEffectEnable(OptionsManager.m_OptionsData.m_ScreenEffect>= enum_Option_ScreenEffect.High,false);
    }

    protected void OnPortalEnter(float duration,Transform vortexTarget, Action OnEnter)
    {
        SetPostEffect_Vortex(true, vortexTarget, 1f,OnEnter);
    }

    protected void OnPortalExit(float duration,Transform vortexTarget)
    {
        SetPostEffect_Vortex(false, vortexTarget, 1f);
    }
    #region Effect
    protected static float m_BulletTime = 1f;
    public static bool m_BulletTiming => m_BulletTime != 1f;
    public static void SetBulletTime(bool enter,float duration=.8f)
    {
        m_BulletTime = enter ? duration:1f ;
        Time.timeScale = m_BulletTime;
    }

    PE_BSC m_BSC;
    public void InitPostEffects(enum_LevelStyle _levelStyle)
    {
        CameraController.Instance.m_Effect.RemoveAllPostEffect();
        //CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthOutline>().SetEffect(Color.black,1.2f,0.0001f);
        m_BSC = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BSC>();
        m_BSC.SetEffect(1f, 1f, 1f);
//        CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthSSAO>().SetEffect();
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BloomSpecific>().m_Blur.SetEffect( PE_Blurs.enum_BlurType.GaussianBlur,3, 10,2);
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<CB_GenerateOpaqueTexture>();
        //switch (_levelStyle)
        //{
            //case enum_LevelStyle.Undead:
            //    CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_FogDepthNoise>().SetEffect<PE_FogDepthNoise>(TCommon.ColorAlpha(Color.white, .3f), .5f, -1f, 5f).SetEffect(TResources.GetNoiseTex(), .4f, 2f);
            //    break;
            //case enum_LevelStyle.Frost:
            //    CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_FogDepth>().SetEffect<PE_FogDepth>(Color.white, .6f, -12, 20);
            //    break;
        //}
    }

    protected void SetPostEffect_Dead()
    {
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => { m_BSC.SetEffect(Mathf.Lerp(1f,.2f,value), Mathf.Lerp(1f, 0f, value), Mathf.Lerp(1f, .8f, value)); }, 0, 1f, 3f));
    }
    protected void SetPostEffect_Revive()
    {
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
         } ));
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
    public static Dictionary<enum_WeaponRarity, List<enum_PlayerWeapon>> m_WeaponRarities { get; private set; } = new Dictionary<enum_WeaponRarity, List<enum_PlayerWeapon>>();
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
    public static SWeapon GetWeaponProperties(enum_PlayerWeapon type)
    {
        SWeapon weapon = Properties<SWeapon>.PropertiesList.Find(p => p.m_Weapon == type);
        if (weapon.m_Weapon == 0)
            Debug.LogError("Error Properties Found Of Index:" + type.ToString() + "|" + ((int)type));
        return weapon;
    }
    public static SBuff GetPresetBuff(int index)
    {
        SBuff buff = Properties<SBuff>.PropertiesList.Find(p => p.m_Index == index);
        if (buff.m_Index == 0)
            Debug.LogError("Error Properties Found Of Index:" + index);
        return buff;
    }
    public static Dictionary<bool,List<SEnermyGenerate>> GetEnermyGenerate(enum_StageLevel stage)
    {
        Dictionary<bool, List<SEnermyGenerate>> m_GenerateDic = new Dictionary<bool, List<SEnermyGenerate>>();
        SheetProperties<SEnermyGenerate>.GetPropertiesList((int)stage-1).Traversal((SEnermyGenerate generate)=> {
            if (!m_GenerateDic.ContainsKey(generate.m_IsFinal))
                m_GenerateDic.Add(generate.m_IsFinal, new List<SEnermyGenerate>());
            m_GenerateDic[generate.m_IsFinal].Add(generate);
        });
        return m_GenerateDic;
    }
    #endregion
}

public static class ActionDataManager
{
    static Dictionary<int, Type> m_EquipmentTypes = new Dictionary<int, Type>();
    public static List<int> m_AllEquipment { get; private set; } = new List<int>();
    static int m_ActionIdentity = 0;
    public static void Init()
    {
        m_EquipmentTypes.Clear();
        m_AllEquipment.Clear();

        TReflection.TraversalAllInheritedClasses(((Type type, EquipmentBase action) => {
            if (action.m_Index <= 0)
                return;

            m_EquipmentTypes.Add(action.m_Index, action.GetType());
            m_AllEquipment.Add(action.m_Index);
        }), -1,EquipmentSaveData.Default(-1, enum_EquipmentType.Invalid));
    }
    public static EquipmentBase CreateRandomEquipment(enum_EquipmentType type, System.Random seed)=> CreateAction(m_AllEquipment.RandomItem(seed),type);

    public static EquipmentBase CreateAction(int actionIndex, enum_EquipmentType type)
    {
        if (!m_EquipmentTypes.ContainsKey(actionIndex))
            Debug.LogError("Error Action:" + actionIndex + " ,Does not exist");
        EquipmentSaveData data = EquipmentSaveData.Default(actionIndex, type);
        return TReflection.CreateInstance<EquipmentBase>(m_EquipmentTypes[actionIndex],  m_ActionIdentity++, data);
    }
    public static EquipmentBase CreateAction(EquipmentSaveData data) => CreateAction(data.m_Index, data.m_Type);
    public static List<EquipmentBase> CreateActions(List<EquipmentSaveData> datas)
    {
        List<EquipmentBase> actions = new List<EquipmentBase>();
        datas.Traversal((EquipmentSaveData data) => { actions.Add(CreateAction(data)); });
        return actions;
    }
}
