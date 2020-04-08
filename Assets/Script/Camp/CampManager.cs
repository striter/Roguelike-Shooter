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
        UIT_MobileConsole.Instance.AddConsoleBinding().Play("Enter Game", KeyCode.Plus,OnGameSceneInteract);
    }
    #endregion
    public static CampManager nInstance;
    public static new CampManager Instance => nInstance;
    Transform tf_PlayerStart,tf_CameraAttach;
    EntityCharacterPlayer m_player;
    protected override void Awake()
    {
        nInstance = this;
        base.Awake();
        tf_PlayerStart = transform.Find("PlayerStart");
        tf_CameraAttach = transform.Find("CameraAttach");
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        nInstance = null;
    }
    protected override void Start()
    {
        base.Start();
        InitGameEffects( enum_GameStyle.Invalid,GameRenderData.Default());
        m_player = GameObjectManager.SpawnEntityPlayer(new PlayerSaveData( enum_PlayerCharacter.Beth, enum_PlayerWeapon.P92),tf_PlayerStart.position, tf_PlayerStart.rotation);
        tf_CameraAttach.position = m_player.transform.position;
        AttachPlayerCamera(tf_CameraAttach);
        CampAudioManager.Instance.PlayBGM(enum_CampMusic.Relax);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCampStart);
    }

    private void Update()
    {
        tf_CameraAttach.position = m_player.transform.position;
    }

    public void OnGameSceneInteract()
    {
        OnPortalEnter(1f, tf_CameraAttach, () => {
            LoadingManager.Instance.ShowLoading(enum_Stage.Rookie);
            SwitchScene( enum_Scene.Game);
        });
    }

    public void OnArmoryInteract(Transform cameraPos)
    {
        if (UIPageBase.m_PageOpening)
            return;

        AttachSceneCamera(cameraPos);
        CampUIManager.Instance.ShowPage<UI_Armory>(true, ResetPlayerCamera, .1f).Play();
    }

    public void OnDailyRewardInteract()
    {

    }

    public void OnBillboardInteract(Transform cameraPos)
    {
        if (UIPage.m_PageOpening)
            return;
        AttachSceneCamera(cameraPos);
        CampUIManager.Instance.ShowPage<UI_Billboard>(true, ResetPlayerCamera, .1f);
    }

    void ResetPlayerCamera()=> AttachPlayerCamera(tf_CameraAttach);

    public void OnCreditStatus(float creditChange)
    {
        GameDataManager.OnCreditStatus(creditChange);
    }
    
    
}
