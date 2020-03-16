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
        CameraController.Instance.m_Effect.SetCostyEffectEnable(OptionsManager.m_OptionsData.m_ScreenEffect > enum_Option_ScreenEffect.Off,OptionsManager.m_OptionsData.m_ScreenEffect>= enum_Option_ScreenEffect.High);
    }

    protected void OnPortalEnter(float duration,Transform vortexTarget, Action OnEnter)
    {
        SetBulletTime(true, 0f);
        SetPostEffect_Vortex(true, vortexTarget, 1f,OnEnter);
    }

    protected void OnPortalExit(float duration,Transform vortexTarget)
    {
        SetBulletTime(false);
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
    public void InitPostEffects(enum_GameStyle _levelStyle)
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
    static Dictionary<int, Type> m_ActionBuffTypes = new Dictionary<int, Type>();
    static int m_ActionIdentity = 0;
    public static void Init()
    {
        m_EquipmentTypes.Clear();
        m_ActionBuffTypes.Clear();

        TReflection.TraversalAllInheritedClasses(((Type type, ActionBase action) => {
            if (action.m_Index <= 0)
                return;

            if (action.m_ExpireType== enum_ExpireType.ActionPerk)
                m_EquipmentTypes.Add(action.m_Index, action.GetType());
            else if (action.m_ExpireType == enum_ExpireType.ActionBuff)
                m_ActionBuffTypes.Add(action.m_Index, action.GetType());
        }));
    }
    public static ActionPerkBase CreateRandomPlayerEquipment(enum_EquipmentRarity rarity, System.Random seed)=> CreatePlayerEquipment(EquipmentSaveData.Default( m_EquipmentTypes.RandomKey(seed),rarity));

    public static ActionPerkBase CreatePlayerEquipment(EquipmentSaveData data)
    {
        if (!m_EquipmentTypes.ContainsKey(data.m_ActionData.m_Index))
            Debug.LogError("Error Action Equipment:" + data.m_ActionData.m_Index + " ,Does not exist");
        ActionPerkBase equipment= TReflection.CreateInstance<ActionPerkBase>(m_EquipmentTypes[data.m_ActionData.m_Index]);
        equipment.OnManagerSetData(m_ActionIdentity,data);
        return equipment;
    }
    public static List<ActionPerkBase> CreatePlayerEquipments(List<EquipmentSaveData> datas)
    {
        List<ActionPerkBase> equipments = new List<ActionPerkBase>();
        datas.Traversal((EquipmentSaveData data) => { equipments.Add(CreatePlayerEquipment(data)); });
        return equipments;
    }

    public static ActionBuffBase CreateRandomActionBuff( System.Random seed=null)=> CreateActionBuff(ActionSaveData.Default(m_ActionBuffTypes.RandomKey(seed)));
    public static ActionBuffBase CreateActionBuff(int buffIndex) => CreateActionBuff(ActionSaveData.Default(buffIndex));
    public static ActionBuffBase CreateActionBuff(ActionSaveData data)
    {
        if (!m_ActionBuffTypes.ContainsKey(data.m_Index))
            Debug.LogError("Error Action Buff:" + data.m_Index + " ,Does not exist");
        ActionBuffBase actionBuff = TReflection.CreateInstance<ActionBuffBase>(m_ActionBuffTypes[data.m_Index]);
        actionBuff.OnManagerSetData(m_ActionIdentity++, data);
        return actionBuff;
    }
    public static List<ActionBuffBase> CreateActionBuffs(List<ActionSaveData> datas)
    {
        List<ActionBuffBase> actions = new List<ActionBuffBase>();
        datas.Traversal((ActionSaveData data) => { actions.Add(CreateActionBuff(data)); });
        return actions;
    }
}
