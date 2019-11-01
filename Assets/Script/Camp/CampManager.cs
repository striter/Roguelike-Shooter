using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : GameManagerBase{
    public static CampManager nInstance;
    public static new CampManager Instance => nInstance;
    
    Transform tf_PlayerStart;
    public Transform tf_Player { get; private set; }
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
        CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_BloomSpecific>();
        GameObjectManager.PresetRegistCommonObject();
        EntityCharacterPlayer player = GameObjectManager.SpawnEntityPlayer(new CPlayerGameSave());
        player.transform.SetPositionAndRotation(tf_PlayerStart.position, tf_PlayerStart.rotation);
        tf_Player = player.transform;
        AttachPlayerCamera(tf_Player);
        CameraController.Instance.RotateCamera(new Vector2(tf_PlayerStart.eulerAngles.y,0));
    }
    
    public void OnSceneItemInteract(enum_Scene scene)
    {
        SwitchScene(scene);
    }

    public bool B_Farming { get; private set; } = false;
    public void OnFarmNPCChatted()
    {
        B_Farming = true;
        AttachSceneCamera( CampFarmManager.Instance.Begin(OnFarmExit));
    }
    void OnFarmExit()
    {
        B_Farming = false;
        AttachPlayerCamera(tf_Player);
    }

    public bool B_Actioning { get; private set; } = false;
    public void OnActionNPCChatted(Transform attachTo,Transform lookAt)
    {
        B_Actioning = true;
        AttachSceneCamera(attachTo, lookAt);
        CampUIManager.Instance.ShowPage<UI_ActionStorage>(true).Play(OnActionExit);
        SetEffect_Focal(true,lookAt,.8f,2f);
    }
    void OnActionExit()
    {
        B_Actioning = false;
        AttachPlayerCamera(tf_Player);
        SetEffect_Focal(false);
    }

    public void OnCreditStatus(float creditChange)
    {
        GameDataManager.OnCreditChange(creditChange);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampCreditStatus);
    }

}
