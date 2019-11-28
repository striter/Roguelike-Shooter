﻿using System.Collections;
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
    protected override void OnDestroy()
    {
        base.OnDestroy();
        nInstance = null;
    }

    protected override void Start()
    {
        base.Start();
        InitPostEffects(enum_Style.Invalid);
        EntityCharacterPlayer player = GameObjectManager.SpawnEntityPlayer(new CBattleSave());
        player.transform.SetPositionAndRotation(tf_PlayerStart.position, tf_PlayerStart.rotation);
        tf_Player = player.transform;
        AttachPlayerCamera(tf_Player);
        CameraController.Instance.RotateCamera(new Vector2(tf_PlayerStart.eulerAngles.y, 0));
        CampFarmManager.Instance.OnCampEnter();
    }

    public void OnSceneItemInteract()
    {
        OnPortalEnter(1f,tf_Player, () => {
            LoadingManager.Instance.ShowLoading(enum_StageLevel.Rookie);
            SwitchScene( enum_Scene.Game);
        });
    }

    public bool B_Farming { get; private set; } = false;
    public void OnFarmNPCChatted()
    {
        if (B_Farming)
            return;

        B_Farming = true;
        AttachSceneCamera(CampFarmManager.Instance.Begin(OnFarmExit));
    }
    void OnFarmExit()
    {
        B_Farming = false;
        AttachPlayerCamera(tf_Player);
    }

    public bool B_ActionStorage { get; private set; } = false;
    public void OnActionNPCChatted()
    {
        if (B_ActionStorage)
            return;

        B_ActionStorage = true;
        CampUIManager.Instance.ShowPage<UI_ActionStorage>(true).Play(OnActionExit);
    }
    void OnActionExit()
    {
        B_ActionStorage = false;
    }

    public void OnCreditStatus(float creditChange)
    {
        GameDataManager.OnCreditStatus(creditChange);
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
            OnCreditStatus(100);
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            OnCreditStatus(-50);
        if (Input.GetKeyDown(KeyCode.Equals))
            OnTechPointStatus(20);
        if (Input.GetKeyDown(KeyCode.Minus))
            OnTechPointStatus(-15);
    }
#endif
}
