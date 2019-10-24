using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class CampManager : GameManagerBase{
    public static CampManager nInstance;
    public static new CampManager Instance => nInstance;

    Transform tf_PlayerStart;
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
        GameObjectManager.SpawnEntityPlayer(new CPlayerLevelSave()).transform.SetPositionAndRotation(tf_PlayerStart.position,tf_PlayerStart.rotation);
        CameraController.Instance.RotateCamera(new Vector2(tf_PlayerStart.eulerAngles.y,0));
    }
    
    public void OnSceneItemInteract(enum_Scene scene)
    {
        SwitchScene(scene);
    }
}
