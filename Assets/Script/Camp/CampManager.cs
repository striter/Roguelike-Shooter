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
    Transform tf_PlayerStart;
    public Transform tf_PlayerCameraAttach { get; private set; }
    protected override void Awake()
    {
        nInstance = this;
        base.Awake();
        tf_PlayerStart = transform.Find("PlayerStart");
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
        EntityCharacterPlayer player = GameObjectManager.SpawnEntityPlayer(new PlayerSaveData( enum_PlayerCharacter.Beth, enum_PlayerWeapon.P92),tf_PlayerStart.position, tf_PlayerStart.rotation);
        tf_PlayerCameraAttach = player.transform;
        AttachPlayerCamera(tf_PlayerCameraAttach);
        CampAudioManager.Instance.PlayBGM(enum_CampMusic.Relax);
        TBroadCaster<enum_BC_GameStatus>.Trigger(enum_BC_GameStatus.OnCampStart);
    }
    
    public void OnGameSceneInteract()
    {
        OnPortalEnter(1f,tf_PlayerCameraAttach, () => {
            LoadingManager.Instance.ShowLoading(enum_Stage.Rookie);
            SwitchScene( enum_Scene.Game);
        });
    }

    public void OnArmoryInteract()
    {
        if (UIPageBase.m_PageOpening)
            return;
        CampUIManager.Instance.ShowCurrentcyPage<UI_Armory>(true, .1f).Play();
    }

    public void OnCreditStatus(float creditChange)
    {
        GameDataManager.OnCreditStatus(creditChange);
    }
    
    
}
