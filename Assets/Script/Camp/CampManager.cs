using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : GameManagerBase
{
    #region Test
    protected override void AddConsoleBinding()
    {
        base.AddConsoleBinding();
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Credit", KeyCode.Plus, "100", (string value) => { OnCreditStatus(int.Parse(value)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Credit", KeyCode.Minus, "-50", (string value) => { OnCreditStatus(int.Parse(value)); });
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Enter Game", KeyCode.Alpha9, ()=>OnBattleStart(true));
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Resume Game", KeyCode.Alpha0, () => OnBattleStart(false));
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Unlock Weapon Blueprint", KeyCode.None, enum_Rarity.Ordinary, (enum_Rarity rarity) => { GameDataManager.UnlockArmoryBlueprint(rarity); });
    }
    #endregion
    public static CampManager nInstance;
    public static new CampManager Instance => nInstance;
    Transform tf_PlayerStart,tf_CameraAttach;
    public EntityCharacterPlayer m_LocalPlayer { get; private set; }
    protected override void Awake()
    {
        nInstance = this;
        base.Awake();
        tf_PlayerStart = transform.Find("PlayerStart");
        tf_CameraAttach = transform.Find("CameraAttach");
        transform.Find("Interactions").GetComponentsInChildren<InteractCampBase>().Traversal((InteractCampBase interact) => { interact.Init(); });
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        nInstance = null;
    }
    protected override void Start()
    {
        base.Start();
        InitGameEffects( enum_BattleStyle.Invalid,GameRenderData.Default());
        OnSetCharacter(GameObjectManager.SpawnPlayerCharacter(GameDataManager.m_CharacterData.m_CharacterSelected,tf_PlayerStart.position,tf_PlayerStart.rotation));
        AttachPlayerCamera(tf_CameraAttach);
        CampAudioManager.Instance.PlayBGM(enum_CampMusic.Relax);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCampStart);
    }

    public void RecycleLocalCharacter()
    {
        m_LocalPlayer.DoRecycle();
    }

    public void OnSetCharacter(EntityCharacterPlayer character)
    {
        m_LocalPlayer = character.OnPlayerActivate(new CBattleSave());
        tf_CameraAttach.position = m_LocalPlayer.transform.position;
    }

    protected override void Update()
    {
        base.Update();
        tf_CameraAttach.position = m_LocalPlayer.transform.position;
    }

    public void OnBattleStart(bool resume)
    {
        PlayPostEffect_Vortex(true, tf_CameraAttach, 1f, () => {
            if (resume) GameDataManager.OnNewBattle();
            LoadingManager.Instance.ShowLoading(enum_BattleStage.Rookie);
            SwitchScene( enum_Scene.Game);
        });
    }


    public void OnArmoryInteract(Transform cameraPos)
    {
        if (UIManager.Instance.m_PageOpening)
            return;

        AttachSceneCamera(cameraPos);
        CampUIManager.Instance.ShowCoinsPage<UI_Armory>(true,true, ResetPlayerCamera, .1f);
    }

    public bool OnAcquireDailyRewardInteract()=>GameDataManager.OnDailyRewardRequire();

    public void OnBillboardInteract(Transform cameraPos)
    {
        if (UIManager.Instance.m_PageOpening)
            return;
        AttachSceneCamera(cameraPos);
        CampUIManager.Instance.ShowCoinsPage<UI_Billboard>(true,true, ResetPlayerCamera, .1f);
    }
    public void OnTasksInteract(Transform cameraPos)
    {
        if (UIManager.Instance.m_PageOpening)
            return;
        AttachSceneCamera(cameraPos);
        CampUIManager.Instance.ShowCoinsPage<UI_DailyTasks>(true, true, ResetPlayerCamera, .1f);
    }

    public void OnCharacterSelectInteract(InteractCampCharacterSelect characterSelect)
    {
        if(UIManager.Instance.m_PageOpening)
            return;
        AttachSceneCamera(characterSelect.m_CameraPos);
        CampUIManager.Instance.ShowCoinsPage<UI_CharacterSelect>(true,false,ResetPlayerCamera,1f).Play(characterSelect);
    }

    void ResetPlayerCamera()=> AttachPlayerCamera(tf_CameraAttach);

    public void OnCreditStatus(float creditChange)
    {
        GameDataManager.OnCreditStatus(creditChange);
    }
    
    
}
