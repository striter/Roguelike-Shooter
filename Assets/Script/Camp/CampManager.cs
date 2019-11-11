using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : GameManagerBase
{
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
    protected override void OnDisable()
    {
        base.OnDisable();
        CampFarmManager.Instance.OnCampExit();
        GameDataManager.SaveCampData();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        nInstance = null;
    }

    protected override void Start()
    {
        base.Start();
        GameObjectManager.PresetRegistCommonObject();
        InitPostEffects(enum_Style.Invalid);
        EntityCharacterPlayer player = GameObjectManager.SpawnEntityPlayer(new CPlayerBattleSave());
        player.transform.SetPositionAndRotation(tf_PlayerStart.position, tf_PlayerStart.rotation);
        tf_Player = player.transform;
        AttachPlayerCamera(tf_Player);
        CameraController.Instance.RotateCamera(new Vector2(tf_PlayerStart.eulerAngles.y, 0));
        CampFarmManager.Instance.OnCampEnter();
    }

    public void OnSceneItemInteract(enum_Scene scene)
    {
        OnPortalEnter(1f,tf_Player, () => { SwitchScene(scene); });
    }

    public bool B_Farming { get; private set; } = false;
    public void OnFarmNPCChatted()
    {
        B_Farming = true;
        AttachSceneCamera(CampFarmManager.Instance.Begin(OnFarmExit));
    }
    void OnFarmExit()
    {
        B_Farming = false;
        AttachPlayerCamera(tf_Player);
    }

    public void OnActionNPCChatted()
    {
        CampUIManager.Instance.ShowPage<UI_ActionStorage>(true).Play(OnActionExit);
    }
    void OnActionExit()
    {
    }

    public void OnCreditStatus(float creditChange)
    {
        GameDataManager.OnCreditStatus(creditChange,false);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampDataStatus);
    }

    public void OnTechPointStatus(float techPoint)
    {
        GameDataManager.OnTechPointStatus(techPoint);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampDataStatus);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            OnCreditStatus(1000);
        if (Input.GetKeyDown(KeyCode.Equals))
            OnTechPointStatus(20);
        if (Input.GetKeyDown(KeyCode.Minus))
            OnTechPointStatus(-15);
    }
#endif
}
