using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TExcel;

public class GameManagerBase : SimpleSingletonMono<GameManagerBase>,ISingleCoroutine{
    public virtual bool B_InGame => false;
    protected override void Awake()
    {
        base.Awake();
        GameDataManager.Init();
        GameObjectManager.Init();
        GameIdentificationManager.Init();
        TBroadCaster<enum_BC_GameStatus>.Init();
        TBroadCaster<enum_BC_UIStatus>.Init();
        OptionsManager.Init();
    }
    protected virtual void Start()
    {
        GameAudioManager.Instance.OnInit();
        UIManager.Activate(B_InGame);
        SetBulletTime(false);
    }

    protected void SwitchScene(enum_Scene scene)
    {
        GameAudioManager.Instance.OnRecycle();
        GameObjectManager.RecycleAllObject();
        TSceneLoader.Instance.LoadScene(scene);
    }

    protected virtual void OnDisable()
    {
        this.StopAllSingleCoroutines();
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
    public void SetPostEffects(enum_Style _levelStyle)
    {
        CameraController.Instance.m_Effect.RemoveAllPostEffect();
        //CameraController.Instance.m_Effect.AddPostEffect<PE_DepthOutline>().SetEffect(Color.black,1.2f,0.0001f);
        //CameraController.Instance.m_Effect.AddPostEffect<PE_DepthSSAO>();
        m_BSC = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BSC>();
        m_BSC.SetEffect(1f, 1f, 1f);
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BloomSpecific>().m_GaussianBlur.SetEffect(2, 10, 2);
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<CB_GenerateOpaqueTexture>();
        switch (_levelStyle)
        {
            case enum_Style.Undead:
                CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_FogDepthNoise>().SetEffect<PE_FogDepthNoise>(TCommon.ColorAlpha(Color.white, .3f), .5f, -1f, 5f).SetEffect(TResources.GetNoiseTex(), .4f, 2f);
                break;
            case enum_Style.Iceland:
                CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_FogDepth>().SetEffect<PE_FogDepth>(Color.white, .6f, -1, 5);
                break;
        }
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

    protected void SetPostEffect_VortexOn(Vector2 distortPos,float duration)
    {
        PE_DistortVortex vortex = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DistortVortex>();
        vortex.SetTexture(TResources.GetNoiseTex());
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>
         {
             vortex.SetDistort(distortPos,value);
         },0,1, duration, CameraController.Instance.m_Effect.RemoveCameraEffect<PE_DistortVortex>));
    }

    public void SetEffect_Shake(float amount)
    {
        TPSCameraController.Instance.AddShake(amount*GameConst.F_DamageImpactMultiply);
        Handheld.Vibrate();
    }
    public void SetEffect_Impact(Vector3 direction)
    {
        TPSCameraController.Instance.SetImpact(direction.normalized*GameConst.F_DamageImpactMultiply);
    }
    #endregion
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
    public static CGameOptions m_OptionsData;
    public static float m_Sensitive=1f;
    public static void Init()
    {
        m_OptionsData = TGameData<CGameOptions>.Read();
        OnOptionChanged();
    }

    public static void Save()
    {
        TGameData<CGameOptions>.Save(m_OptionsData);
        OnOptionChanged();
    }

    public static void OnOptionChanged()
    {
        m_Sensitive = GameExpression.F_PlayerSensitive(m_OptionsData.m_SensitiveTap);
        Application.targetFrameRate = (int)m_OptionsData.m_FrameRate;
        TLocalization.SetRegion(m_OptionsData.m_Region);
        event_OptionChanged?.Invoke();
    }
    public static float F_SFXVolume =>GameExpression.F_GameVFXVolume(m_OptionsData.m_VFXVolumeTap);
    public static float F_MusicVolume => GameExpression.F_GameMusicVolume(m_OptionsData.m_MusicVolumeTap);
}
public static class GameDataManager
{
    public static bool m_Inited { get; private set; } = false;
    public static void Init()
    {
        if (m_Inited) return;
        m_Inited = true;
        Properties<SLevelGenerate>.Init();
        Properties<SWeapon>.Init();
        Properties<SBuff>.Init();
        SheetProperties<SGenerateEntity>.Init();
        InitActions();

        m_PlayerGameData = TGameData<CPlayerGameSave>.Read();
        m_PlayerLevelData = TGameData<CPlayerLevelSave>.Read();
    }
    #region GameSave
    public static CPlayerGameSave m_PlayerGameData { get; private set; }
    public static CPlayerLevelSave m_PlayerLevelData { get; private set; }
    public static void AdjuastInGameData(EntityCharacterPlayer data, GameLevelManager level)
    {
        m_PlayerLevelData.Adjust(data, level);
        TGameData<CPlayerLevelSave>.Save(m_PlayerLevelData);
    }
    public static void OnGameFinished(bool win)
    {
        m_PlayerLevelData = new CPlayerLevelSave();
        TGameData<CPlayerLevelSave>.Save(m_PlayerLevelData);

        if (win && m_PlayerGameData.m_GameDifficulty == m_PlayerGameData.m_DifficultyUnlocked)
        {
            m_PlayerGameData.m_DifficultyUnlocked++;
            m_PlayerGameData.m_GameDifficulty++;
        }
        TGameData<CPlayerGameSave>.Save(m_PlayerGameData);
    }
    public static bool CanUseCredit(float credit) => m_PlayerGameData.f_Credits >= credit;
    public static void OnCreditGain(float credit)
    {
        m_PlayerGameData.f_Credits += credit;
        TGameData<CPlayerGameSave>.Save(m_PlayerGameData);
    }

    public static int SwitchGameDifficulty()
    {
        m_PlayerGameData.m_GameDifficulty += 1;
        if (m_PlayerGameData.m_GameDifficulty > m_PlayerGameData.m_DifficultyUnlocked)
            m_PlayerGameData.m_GameDifficulty = 1;
        TGameData<CPlayerGameSave>.Save(m_PlayerGameData);
        return m_PlayerGameData.m_GameDifficulty;
    }
    #endregion
    #region ExcelData
    public static SLevelGenerate GetItemGenerateProperties(enum_Style style, enum_LevelGenerateType prefabType, bool isInner)
    {
        SLevelGenerate generate = Properties<SLevelGenerate>.PropertiesList.Find(p => p.m_LevelStyle == style && p.m_LevelPrefabType == prefabType && p.m_IsInner == isInner);
        if (generate.m_LevelStyle == 0 || generate.m_LevelPrefabType == 0 || generate.m_ItemGenerate == null)
            Debug.LogError("Error Properties Found Of Index:" + ((int)style * 100 + (int)prefabType * 10 + (isInner ? 0 : 1)).ToString());

        return generate;
    }

    public static List<SGenerateEntity> GetEntityGenerateProperties(enum_StageLevel stage,enum_BattleDifficulty battleDifficulty)
    {
        List<SGenerateEntity> entityList = new List<SGenerateEntity>();
        int waveCount = 1;
        for (int i = 0; i < 10; i++)
        {
            List<SGenerateEntity> randomItems = SheetProperties<SGenerateEntity>.GetPropertiesList((int)stage-1).FindAll(p=>p.m_Difficulty == battleDifficulty && p.m_waveCount == waveCount);
            if (randomItems == null || randomItems.Count == 0)
                break;
            entityList.Add(randomItems.RandomItem());
            waveCount++;
        }
        if (entityList.Count == 0)
            Debug.LogError("Null Entity Generate Found By:"+(int)stage+"_" + (int)battleDifficulty);
        return entityList;
    }

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
    #endregion
    #region ActionData
    static Dictionary<int, ActionBase> m_AllActions = new Dictionary<int, ActionBase>();
    static List<int> m_WeaponActions = new List<int>();
    static List<int> m_PlayerActions = new List<int>();
    static int m_ActionIdentity = 0;
    static void InitActions()
    {
        m_AllActions.Clear();
        m_WeaponActions.Clear();
        m_PlayerActions.Clear();
        TReflection.TraversalAllInheritedClasses((Type type, ActionBase action) => {
            if (action.m_Index <= 0)
                return;

            m_AllActions.Add(action.m_Index, action);
            if (action.m_ActionType == enum_ActionType.WeaponPerk)
                m_WeaponActions.Add(action.m_Index);
            else
                m_PlayerActions.Add(action.m_Index);
        }, -1, enum_RarityLevel.Invalid);
    }
    public static  ActionBase CreateRandomWeaponPerk(enum_RarityLevel level, System.Random seed) =>level==0?null: CreateAction(m_WeaponActions.RandomItem(seed), level) ;
    public static List<ActionBase> CreateRandomPlayerActions(int actionCount, enum_RarityLevel level, System.Random seed)
    {
        List<ActionBase> actions = new List<ActionBase>();
        for (int i = 0; i < actionCount; i++)
        {
            int actionIndex = -1;
            m_PlayerActions.TraversalRandom((int index) =>
            {
                if (actions.Find(p => p.m_Index == index) == null)
                {
                    actionIndex = index;
                    return true;
                }
                return false;
            }, seed);
            if (actionIndex != -1)
                actions.Add(CreateAction(actionIndex, level));
        }
        return actions;
    }
    public static ActionBase CreateRandomPlayerAction(enum_RarityLevel level, System.Random seed) => CreateAction(m_PlayerActions.RandomItem(seed), level);
    public static List<ActionBase> CreateActions(List<ActionInfo> infos)
    {
        List<ActionBase> actions = new List<ActionBase>();
        infos.Traversal((ActionInfo info) => { actions.Add(CreateAction(info)); });
        return actions;
    }
    public static ActionBase CreateAction(ActionInfo info) => info.m_IsNull ? null : CreateAction(info.m_Index, info.m_Level);
    public static ActionBase CreateAction(int actionIndex, enum_RarityLevel level)
    {
        if (!m_AllActions.ContainsKey(actionIndex))
            Debug.LogError("Error Action:" + actionIndex + " ,Does not exist");
        return TReflection.CreateInstance<ActionBase>(m_AllActions[actionIndex].GetType(), m_ActionIdentity++, level);
    }
    public static ActionBase CopyAction(ActionBase targetAction) => TReflection.CreateInstance<ActionBase>(m_AllActions[targetAction.m_Index].GetType(), targetAction.m_Identity, targetAction.m_rarity);
    #endregion
}
